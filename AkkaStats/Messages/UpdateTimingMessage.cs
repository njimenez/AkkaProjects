using System;
using System.Collections.Generic;
using System.Linq;

namespace AkkaStats.Messages
{
    public class UpdateTimingMessage
    {
        public UpdateTimingMessage( string metricName, long time, double sampleRate )
        {
            SampleRate = sampleRate;
            Time = time;
            MetricName = metricName;
        }
        public string MetricName
        {
            get; private set;
        }
        public long Time
        {
            get; private set;
        }
        public double SampleRate
        {
            get; private set;
        }
    }
}
