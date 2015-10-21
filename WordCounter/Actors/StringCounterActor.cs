using Akka.Actor;
using System;
using WordCounter.Messages;

namespace WordCounter.Actors
{
    public class StringCounterActor : BaseMonitoringActor
    {
        public static Props GetProps()
        {
            return Props.Create<StringCounterActor>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringCounterActor"/> class.
        /// </summary>
        public StringCounterActor()
        {
            Ready();
        }

        private void Ready()
        {
            Receive<ProcessLine>( msg => Handle( msg ) );
        }
        public void Handle( ProcessLine message )
        {
            IncrementMessagesReceived();
            var wordsInLine = message.LineToProcess.Split( ' ' ).Length;
            Sender.Tell( new WordCount( wordsInLine ) );
        }

        protected override void PreRestart( Exception reason, object message )
        {
            Context.Parent.Tell( new FailureMessage( reason, Self ) );
        }
    }
}
