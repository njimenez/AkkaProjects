using Akka.Actor;
using System;
using System.Diagnostics;
using System.IO;
using WordCounter.Messages;

namespace WordCounter.Actors
{
    /*
     *  FileEnumeratorActor
     * 
     *  Receives :
     *      DirectoryToSearchMessage
     * 
     *  Tells Sender 
     *      FileInfo
     *      StatusMessage
     *      Done
     * 
     */

    public class FileEnumeratorActor : BaseMonitoringActor
    {
        private int fileCount = 0;
        private readonly Stopwatch m_sw = new Stopwatch();
        public static Props GetProps()
        {
            return Props.Create<FileEnumeratorActor>();
        }

        /// <summary>
        /// Initializes a new instance of the CrawlForFiles class.
        /// </summary>        
        public FileEnumeratorActor()
        {
            Ready();
        }

        private void Ready()
        {
            Receive<DirectoryToSearchMessage>( msg => Handle( msg ) );
        }

        private void Handle( DirectoryToSearchMessage message )
        {
            IncrementMessagesReceived();
            fileCount = 0;
            m_sw.Start();
            EnumerateFiles( Sender, message.Directory, message.SearchPattern );
            m_sw.Stop();
            Sender.Tell( new Done( fileCount, m_sw.Elapsed ) );
            m_sw.Reset();
        }
        
        private void EnumerateFiles( IActorRef sender, string directory, String searchPattern )
        {
            try
            {
                foreach ( var file in Directory.GetFiles( directory, searchPattern, SearchOption.TopDirectoryOnly ) )
                {
                    fileCount++;
                    sender.Tell( new FileInfo( file ) );
                }

                EnumerateDirectories( sender, directory, searchPattern );

            }
            catch ( Exception )
            {
                sender.Tell( new StatusMessage( string.Format( "Error getting file in directory : [{0}]", directory ) ) );
            }
        }

        private void EnumerateDirectories( IActorRef sender, string staringdir, String searchPattern )
        {
            foreach ( var dir in Directory.GetDirectories( staringdir, "*.*", SearchOption.TopDirectoryOnly ) )
            {
                EnumerateFiles( sender, dir, searchPattern );
            }
        }

    }
}
