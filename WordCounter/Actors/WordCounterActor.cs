using Akka.Actor;
using Akka.Routing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WordCounter.Messages;

namespace WordCounter.Actors
{
    public class WordCounterActor : ReceiveActor
    {
        private string fileName;
        private int lineno = 0;
        private int linesProcessed = 0;
        private Stopwatch m_sw;
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
            //m_Sender = sender;
            fileName = String.Empty;
            m_sw = new Stopwatch();
            Ready();
        }


        private void Ready()
        {
            Receive<FileToProcessMessage>( msg => Handle( msg ) );
            Receive<WordsInLineMessage>( msg => Handle( msg ) );
            Receive<FailureMessage>( msg => Handle( msg ) );
        }
        public void Handle( FileToProcessMessage message )
        {
            lineno = 0;
            linesProcessed = 0;
            result = 0;
            m_sw.Start();
            fileName = message.FileName;
            var router = Context.ActorOf( new RoundRobinPool( 8 )
                                            .Props( StringCounterActor.Config() ),
                                            String.Format( "liner{0}", message.Fileno ) );

            foreach ( var line in FileLines( fileName ) )
            {
                // var counterActor = Context.ActorOf<StringCounterActor>();
                // counterActor.Tell( new LineToProcessMessage( line ) );
                router.Tell( new LineToProcessMessage( line ) );
                lineno++;
            }

            // handle when file is empty
            if ( lineno == 0 )
            {
                Sender.Tell( new WordsInFileMessage( fileName, result, 0 ) );
            }
        }

        public void Handle( WordsInLineMessage message )
        {
            result += message.WordsInLine;
            linesProcessed++;
            // make sure that all lines have been visited.
            if ( linesProcessed == lineno )
            {
                m_sw.Stop();
                Context.Parent.Tell( new WordsInFileMessage( fileName, result, m_sw.ElapsedMilliseconds ) );
                Context.Stop( Self );
            }
        }

        public void Handle( FailureMessage fail )
        {
            var exception = fail.Cause;
            if ( exception is AggregateException )
            {
                var agg = (AggregateException)exception;
                exception = agg.InnerException;
                agg.Handle( exception1 => true );
            }
            //writer.Tell( "Error " + fail.Child.Path + " " + exception != null ? exception.Message : "no exception object" );
        }

        protected override void PreRestart( Exception reason, object message )
        {
            Context.Parent.Tell( new FailureMessage( reason, Self ) );
        }

        private IEnumerable<string> FileLines( string filename )
        {
            using ( var sr = File.OpenText( filename ) )
            {
                while ( true )
                {
                    string line = sr.ReadLine();
                    if ( line == null )
                        yield break;
                    yield return line;
                }
            }
        }
    }
}
