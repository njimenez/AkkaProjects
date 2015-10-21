using Akka.Actor;
using AkkaStats.Messages;
using System;
using System.Collections.Generic;

namespace AkkaStats.Actors
{
    public class AkkaSystemMonitorActor : ReceiveActor
    {
        private readonly HashSet<IActorRef> subscribers;

        public static Props GetProps()
        {
            return Props.Create( () => new AkkaSystemMonitorActor() );
        }

        public static string Name
        {
            get
            {
                return "system-monitor";
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceCounterActor"/> class.
        /// </summary>
        public AkkaSystemMonitorActor()
        {
            subscribers = new HashSet<IActorRef>();
            Ready();
        }

        private void Ready()
        {
            Receive<UpdateCounterMessage>( msg => Handle( msg ) );
            Receive<UpdateGaugeMessage>( msg => Handle( msg ) );
            Receive<UpdateTimingMessage>( msg => Handle( msg ) );
            Receive<SubscribeMonitorMessage>( msg => Handle( msg ) );
            Receive<PublishMetrics>( msg => Handle( msg ) );
        }
        private void Handle( UpdateCounterMessage msg )
        {
            var child = GetChild( msg.MetricName );
            child.Tell( msg );
            Self.Tell( new PublishMetrics( msg.MetricName, msg.Delta ) );
        }
        private void Handle( UpdateGaugeMessage msg )
        {
            var child = GetChild( msg.MetricName );
            child.Tell( msg );
        }
        private void Handle( UpdateTimingMessage msg )
        {
            var child = GetChild( msg.MetricName );
            child.Tell( msg );
        }
        private void Handle( PublishMetrics msg )
        {
            foreach ( var item in subscribers )
            {
                item.Tell( msg );
            }
        }
        private void Handle( SubscribeMonitorMessage msg )
        {
            subscribers.Add( msg.Observer );
        }
        /// <summary>
        /// Given a metric name; check to see if we already have an actor handling the metric
        /// otherwise create one.
        /// </summary>
        /// <param name="metricName"></param>
        /// <returns></returns>
        private IActorRef GetChild( string metricName )
        {
            var child = Context.Child( metricName );
            if ( child.Equals( ActorRefs.Nobody ) )
            {
                child = Context.ActorOf( AkkaCounter.GetProps( metricName ), metricName );
            }
            return child;
        }
    }
}