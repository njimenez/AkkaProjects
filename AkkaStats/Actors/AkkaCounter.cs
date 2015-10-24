using Akka.Actor;
using AkkaStats.Messages;
using System;

namespace AkkaStats.Actors
{
    public class AkkaCounter : ReceiveActor
    {
        private long m_value = 0L;
        private readonly string m_MetricName;

        public static Props GetProps( string metricName )
        {
            return Props.Create( () => new AkkaCounter( metricName ) );
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceCounter"/> class.
        /// </summary>
        public AkkaCounter( string metricName )
        {
            Receive<UpdateCounterMessage>( msg => Handle( msg ) );
            Receive<UpdateGaugeMessage>( msg => Handle( msg ) );
            Receive<UpdateTimingMessage>( msg => Handle( msg ) );
            Receive<GetValues>( msg => Handle( msg ) );
            m_MetricName = metricName;
        }
        private void Handle( UpdateCounterMessage msg )
        {
            m_value += msg.Delta;
        }
        private void Handle( UpdateGaugeMessage msg )
        {
            m_value += msg.Value;
        }
        private void Handle( UpdateTimingMessage msg )
        {
            m_value += msg.Time;
        }
        private void Handle( GetValues msg )
        {
            Sender.Tell( new PublishMetrics( m_MetricName, m_value) );
        }
    }
}
