using Akka.Actor;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WordCounter.Messages;

namespace WordCounter.Actors
{
    public class DirectoryCrawler : BaseMonitoringActor
    {
        private bool CrawlingDone = false;
        private int fileno = 0;
        private int fileProcessed = 0;
        private int filesCrawled;
        private readonly Stopwatch m_sw = new Stopwatch();

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryCrawler"/> class.
        /// </summary>
        public DirectoryCrawler()
        {
            Ready();
        }

        private void Ready()
        {
            Receive<DirectoryToSearchMessage>( msg => Handle( msg ) );
            Receive<FileInfo>( msg => Handle( msg ) );
            Receive<CompletedFile>( msg => Handle( msg ) );
            Receive<DoneEnumeratingFiles>( msg => DoneEnumerating( msg ) );
            Receive<FailureMessage>( msg => Handle( msg ) );
        }

        public void Handle( DirectoryToSearchMessage message )
        {
            IncrementMessagesReceived();
            fileno = 0;
            fileProcessed = 0;
            filesCrawled = 0;
            CrawlingDone = false;

            m_sw.Start();
            var EnumeratorActor = Context.ActorOf( FileEnumeratorActor.GetProps() );
            EnumeratorActor.Tell( message );
        }
        /// <summary>
        /// Message received from Enumerator Actor when a file is found that meets the search criteria.
        /// </summary>
        /// <param name="msg">FileInfo message; contains the file's full name to be processed</param>
        private void Handle( FileInfo msg )
        {
            IncrementMessagesReceived();
            fileno++;
            var counterActor = Context.ActorOf( WordCounterActor.GetProps() );
            counterActor.Tell( new FileToProcess( msg.FullName, fileno ) );
            Context.Parent.Tell( new StatusMessage( "Processing file " + msg.FullName ) );
        }

        public void Handle( CompletedFile message )
        {
            IncrementMessagesReceived();
            fileProcessed++;
            Context.Parent.Tell( message );
            CrawlingFinished();
        }
        private void DoneEnumerating( DoneEnumeratingFiles msg )
        {
            IncrementMessagesReceived();
            filesCrawled = msg.Count;
            CrawlingDone = true;
        }
        public void Handle( FailureMessage fail )
        {
            IncrementMessagesReceived();
            var exception = fail.Cause;
            if ( exception is AggregateException )
            {
                var agg = (AggregateException)exception;
                exception = agg.InnerException;
                agg.Handle( exception1 => true );
            }
            Context.Parent.Tell( new StatusMessage( "Error " + fail.Child.Path + " " + exception != null ? exception.Message : "no exception object" ) );
        }

        private void CrawlingFinished()
        {
            if ( CrawlingDone && ( fileProcessed == fileno ) )
            {
                m_sw.Stop();
                Context.Parent.Tell( new Done( fileProcessed, m_sw.Elapsed ) );
                m_sw.Reset();
                Sender.Tell( PoisonPill.Instance );
            }
        }
        protected override void PreRestart( Exception reason, object message )
        {
            // preserve all children in the event of a restart
        }
    }
}
