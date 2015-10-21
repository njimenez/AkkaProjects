using Akka.Actor;
using System;
using System.Diagnostics;
using System.IO;
using WordCounter.Messages;

namespace WordCounter.Actors
{
    public class DirectoryCrawler : BaseMonitoringActor
    {
        private readonly IActorRef EnumeratorActor;

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
            EnumeratorActor = Context.ActorOf( FileEnumeratorActor.GetProps() );
            Ready();
        }

        private void Ready()
        {
            Receive<DirectoryToSearchMessage>( msg => Handle( msg ) );
            Receive<FileInfo>( msg => Handle( msg ) );
            Receive<CompletedFile>( msg => Handle( msg ) );
            Receive<Done>( msg => Handle( msg ) );
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
            EnumeratorActor.Tell( message );
        }

        private void Handle( FileInfo msg )
        {
            IncrementMessagesReceived();
            var counterActor = Context.ActorOf( WordCounterActor.GetProps() );
            counterActor.Tell( new FileToProcess( msg.FullName, fileno ) );
            fileno++;
            Context.Parent.Tell( new StatusMessage( "Processing file " + msg.FullName ) );
        }

        public void Handle( CompletedFile message )
        {
            IncrementMessagesReceived();
            fileProcessed++;
            Context.Parent.Tell( message );
            CrawlingFinished();
        }

        private void Handle( Done msg )
        {
            IncrementMessagesReceived();
            filesCrawled = msg.Count;
            CrawlingDone = true;
            CrawlingFinished();
        }

        public void Handle( FailureMessage fail )
        {
            IncrementMessagesReceived();
            var exception = fail.Cause;
            if ( exception is AggregateException )
            {
                var agg = ( AggregateException )exception;
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
            }
        }
    }
}
