using System;
using System.Collections.Generic;
using System.Linq;

namespace AkkaStats.Messages
{
    public class UpdateCounterMessage
    {
        public UpdateCounterMessage( string metricName, int delta, double sampleRate )
        {
            SampleRate = sampleRate;
            Delta = delta;
            MetricName = metricName;
        }
        public string MetricName
        {
            get; private set;
        }
        public int Delta
        {
            get; private set;
        }
        public double SampleRate
        {
            get; private set;
        }
    }
}
