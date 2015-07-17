using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using WordCounter.Messages;

namespace WordCounter.Actors
{
    public class StringCounterActor : ReceiveActor
    {
        public static Props Config()
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
            Receive<LineToProcessMessage>( msg => Handle( msg ) );
        }
        public void Handle( LineToProcessMessage message )
        {
            var wordsInLine = message.LineToProcess.Split( ' ' ).Length;
            Sender.Tell( new WordsInLineMessage( wordsInLine ) );
        }

        protected override void PreRestart( Exception reason, object message )
        {
            Context.Parent.Tell( new FailureMessage( reason, Self ) );
        }
    }
}
