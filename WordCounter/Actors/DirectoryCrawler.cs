using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WordCounter.Messages;

namespace WordCounter.Actors
{
    public class DirectoryCrawler : ReceiveActor
    {
        private int fileno = 0;
        private int fileProcessed = 0;
        private Stopwatch m_sw = new Stopwatch();
        private Dictionary<string, Boolean> filesProcessed = new Dictionary<string, bool>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryCrawler"/> class.
        /// </summary>
        public DirectoryCrawler()
        {
            Receive<DirectoryToSearchMessage>( msg => Handle( msg ) );
            Receive<WordsInFileMessage>( msg => Handle( msg ) );
            Receive<FailureMessage>( msg => Handle( msg ) );
        }

        public void Handle( DirectoryToSearchMessage message )
        {
            filesProcessed.Clear();
            fileno = 0;
            fileProcessed = 0;

            m_sw.Start();
            foreach ( var file in Directory.EnumerateFiles( message.DirectoryPath, message.SearchPattern, SearchOption.AllDirectories ) )
            {
                var counterActor = Context.ActorOf( WordCounterActor.GetProps() );
                counterActor.Tell( new FileToProcessMessage( file, fileno ) );
                fileno++;
                filesProcessed.Add( file, false );
                Context.Parent.Tell( new StatusMessage( "Processing file " + file ) );
            }
        }

        public void Handle( WordsInFileMessage message )
        {
            fileProcessed++;
            Context.Parent.Tell( message );

            //writer.Tell( "-----------------------------------------------------------------------------------------------" );
            //writer.Tell( String.Format( "{0} ", message.FileName ) );
            //writer.Tell( String.Format( "has a total number of words: {0}", message.WordsInFile ) );
            //writer.Tell( String.Format( "{0} out of {1}", fileProcessed, fileno ) );
            filesProcessed[ message.FileName ] = true;
            if ( fileProcessed == fileno )
            {
                m_sw.Stop();
                Context.Parent.Tell( new StatusMessage( string.Format( "Processed {0} file(s) in {1} ms", fileno, m_sw.ElapsedMilliseconds ) ) );
                Context.Parent.Tell( new Done() );
                m_sw.Reset();
                // writer.Tell( String.Format( "Done....{0:N}", m_sw.ElapsedMilliseconds ) );
                // Self.GracefulStop(new TimeSpan(0,0,5));
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

        //private void DisplayUnCountedFiles()
        //{
        //    writer.Tell( ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>" );
        //    foreach ( KeyValuePair<string, Boolean> entry in filedProcessed )
        //    {
        //        if ( !entry.Value )
        //        {
        //            writer.Tell( ">>>  Waiting on: " + entry.Key );
        //        }
        //    }
        //}
    }
}
