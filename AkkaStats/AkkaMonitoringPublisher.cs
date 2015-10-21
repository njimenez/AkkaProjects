using Akka.Actor;
using Akka.Monitoring.Impl;
using AkkaStats.Actors;
using AkkaStats.Messages;
using System;

namespace AkkaStats
{
    public class AkkaMonitoringPublisher : AbstractActorMonitoringClient
    {
        private readonly IActorRef m_Publisher;
        private static readonly Guid MonitorName = new Guid( "E453A501-F9AC-4168-8682-4B3B288D4DB3" );

        /// <summary>
        /// Initializes a new instance of the <see cref="AkkaMonitoringPublisher"/> class.
        /// </summary>
        public AkkaMonitoringPublisher( ActorSystem system )
        {
            m_Publisher = system.ActorOf( AkkaSystemMonitorActor.GetProps(), AkkaSystemMonitorActor.Name );
        }

        public IActorRef Publisher
        {
            get
            {
                return m_Publisher;
            }
        }

        public override int MonitoringClientId
        {
            get
            {
                return MonitorName.GetHashCode();
            }
        }

        public override void DisposeInternal()
        {
            m_Publisher.GracefulStop( TimeSpan.FromSeconds( 2 ) );
        }

        public override void UpdateCounter( string metricName, int delta, double sampleRate )
        {
            m_Publisher.Tell( new UpdateCounterMessage( metricName, delta, sampleRate ) );
        }

        public override void UpdateGauge( string metricName, int value, double sampleRate )
        {
            m_Publisher.Tell( new UpdateGaugeMessage( metricName, value, sampleRate ) );
        }

        public override void UpdateTiming( string metricName, long time, double sampleRate )
        {
            m_Publisher.Tell( new UpdateTimingMessage( metricName, time, sampleRate ) );
        }
    }

}
