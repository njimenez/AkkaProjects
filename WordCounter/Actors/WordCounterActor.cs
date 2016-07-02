using Akka.Actor;
using Akka.Routing;
using System;
using System.Diagnostics;
using System.IO;
using WordCounter.Messages;

namespace WordCounter.Actors
{
    public class WordCounterActor : BaseMonitoringActor
    {
        private string fileName;
        private int lineCount = 0;
        private int linesProcessed = 0;
        private readonly Stopwatch m_sw;
        private IActorRef router;
        private int result = 0;

        public static Props GetProps()
        {
            return Props.Create<WordCounterActor>();
        }

        /// <summary>
        /// Initializes a new instance of the WordCounterActor class.
        /// </summary>
        public WordCounterActor()
        {
            fileName = String.Empty;
            m_sw = new Stopwatch();
            Ready();
        }
        private void Ready()
        {
            Receive<FileToProcess>( msg => Handle( msg ) );
            Receive<WordCount>( msg => Handle( msg ) );
            Receive<FailureMessage>( msg => Handle( msg ) );
        }

        /// <summary>
        /// Handle file processing message.
        /// Actor will count one file.
        /// </summary>
        /// <param name="message"></param>
        public void Handle( FileToProcess message )
        {
            IncrementMessagesReceived();

            lineCount = 0;
            linesProcessed = 0;
            result = 0;
            m_sw.Start();
            fileName = message.FileName;
            router = Context.ActorOf( new RoundRobinPool( 8 ).Props( StringCounterActor.GetProps() ), String.Format( "liner{0}", message.Fileno ) );
            try
            {
                foreach ( var line in File.ReadLines( fileName ) )
                {
                    lineCount++;
                    router.Tell( new ProcessLine( line, lineCount ) );
                }
            }
            catch ( Exception ex )
            {
                Sender.Tell( new FailureMessage( ex, Self ) );
            }

            // handle when file is empty
            if ( lineCount == 0 )
            {
                Sender.Tell( new CompletedFile( fileName, result, lineCount, 0 ) );
            }
        }
        /// <summary>
        /// This message is sent by the StringCounter Actor when it finishes with one line.
        /// When all lines counted then it will send a CompletedFile message to its parent
        /// and stop itself.
        /// </summary>
        /// <param name="message"></param>
        public void Handle( WordCount message )
        {
            IncrementMessagesReceived();

            // aggregate the results
            result += message.WordsInLine;
            // update lines processed
            linesProcessed++;

            // make sure that all lines have been visited.
            if ( linesProcessed >= lineCount )
            {
                m_sw.Stop();
                Context.Parent.Tell( new CompletedFile( fileName, result, lineCount, m_sw.ElapsedMilliseconds ) );
                Context.Stop( Self );
            }
        }
        public void Handle( FailureMessage fail )
        {
            IncrementMessagesReceived();
            Context.Parent.Tell( fail );
        }
        protected override void PreRestart( Exception reason, object message )
        {
            Context.Parent.Tell( new FailureMessage( reason, Self ) );
        }
    }
}
