using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaStats.Messages
{
    public class SubscribeMonitorMessage
    {       
        public SubscribeMonitorMessage(IActorRef observer)
        {
            Observer = observer;
        }
        public IActorRef Observer
        {
            get; private set;
        }
    }
    public class GetValues
    {
    }
    public class PublishMetrics
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublishMetrics"/> class.
        /// </summary>
        public PublishMetrics( string metricName, long value )
        {
            Value = value;
            MetricName = metricName;
        }
        public string MetricName
        {
            get; private set;
        }
        public long Value
        {
            get; private set;
        }
    }

}
