using Akka.Actor;
using AkkaStats.Messages;
using System;
using System.Collections.Generic;

namespace AkkaStats.Actors
{
    public class AkkaSystemMonitorActor : ReceiveActor
    {
        private ICancelable m_CancelToken;
        private bool m_ScheduleOn;
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
        /// <summary>
        /// Prepare to receive 
        /// </summary>
        private void Ready()
        {
            Receive<UpdateCounterMessage>( msg => Handle( msg ) );
            Receive<UpdateGaugeMessage>( msg => Handle( msg ) );
            Receive<UpdateTimingMessage>( msg => Handle( msg ) );
            Receive<SubscribeMonitorMessage>( msg => Handle( msg ) );
            Receive<GetValues>( msg => Handle( msg ) );
            Receive<PublishMetrics>( msg => Handle( msg ) );
        }
        private void Handle( UpdateCounterMessage msg )
        {
            var child = GetChild( msg.MetricName );
            child.Tell( msg );
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
        private void Handle( GetValues msg )
        {
            foreach ( var child in Context.GetChildren() )
            {
                child.Tell( msg );
            }
        }
        private void Handle( PublishMetrics msg )
        {
            // stop scheduler since we are going to publish
            if ( m_CancelToken != null )
            {
                m_CancelToken.Cancel();
                m_CancelToken = null;
            }
            foreach ( var item in subscribers )
            {
                item.Tell( msg );
            }
        }
        private void Handle( SubscribeMonitorMessage msg )
        {
            subscribers.Add( msg.Observer );
        }
        private void SetupSchedulePublishing()
        {
            if ( m_CancelToken == null && subscribers.Count > 0 )
            {
                m_CancelToken = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable( TimeSpan.FromSeconds( 1 ),
                TimeSpan.FromSeconds( 3 ),
                Self, new GetValues(), Self );
                m_ScheduleOn = true;
            }
        }
        /// <summary>
        /// Given a metric name; check to see if we already have an actor handling the metric
        /// otherwise create one.
        /// </summary>
        /// <param name="metricName"></param>
        /// <returns></returns>
        private IActorRef GetChild( string metricName )
        {
            SetupSchedulePublishing();
            var child = Context.Child( metricName );
            if ( child.Equals( ActorRefs.Nobody ) )
            {
                child = Context.ActorOf( AkkaCounter.GetProps( metricName ), metricName );
            }
            return child;
        }
    }
}