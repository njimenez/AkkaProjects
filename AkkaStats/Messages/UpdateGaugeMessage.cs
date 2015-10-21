using System;
using System.Collections.Generic;
using System.Linq;

namespace AkkaStats.Messages
{
    public class UpdateGaugeMessage
    {
        public UpdateGaugeMessage( string metricName, int value, double sampleRate )
        {
            SampleRate = sampleRate;
            Value = value;
            MetricName = metricName;
        }
        public string MetricName
        {
            get; private set;
        }
        public int Value
        {
            get; private set;
        }
        public double SampleRate
        {
            get; private set;
        }
    }
}
