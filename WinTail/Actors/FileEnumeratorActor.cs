using Akka.Actor;
using System;
using System.Diagnostics;
using System.IO;
using WinTail.Messages;

namespace WinTail.Actors
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

    public class FileEnumeratorActor : ReceiveActor
    {
        private int fileCount = 0;
        private Stopwatch m_sw = new Stopwatch();

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
            Receive<EnumerateFiles>( msg => Handle( msg ) );
        }

        private void Handle( EnumerateFiles message )
        {
            m_sw.Start();
            fileCount = 0;

            var multiplePatterns = message.Extension.Split( ',' );
            foreach ( string extension in multiplePatterns )
            {
                EnumerateFiles( Sender, message.Folders, extension );
            }

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