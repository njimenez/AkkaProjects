using Akka.Actor;
using AkkaStats.Messages;

namespace AkkaStats.Actors
{
    public class AkkaCounter : ReceiveActor
    {
        private int m_counter = 0;
        private int m_gauge = 0;
        private long m_timing = 0L;
        private readonly string _MetricName;

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
            _MetricName = metricName;
        }
        private void Handle( UpdateCounterMessage msg )
        {
            m_counter += msg.Delta;
            Sender.Tell( new PublishMetrics( msg.MetricName, m_counter ) );
        }
        private void Handle( UpdateGaugeMessage msg )
        {
            m_gauge += msg.Value;
            Sender.Tell( new PublishMetrics( msg.MetricName, m_gauge ) );
        }
        private void Handle( UpdateTimingMessage msg )
        {
            m_timing += msg.Time;
            Sender.Tell( new PublishMetrics( msg.MetricName, m_timing ) );
        }
    }
}
