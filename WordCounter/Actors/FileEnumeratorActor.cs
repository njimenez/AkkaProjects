using Akka.Actor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    
    public class FileEnumeratorActor : ReceiveActor
    {
        private int fileCount = 0;
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
            fileCount = 0;
            EnumerateFiles( Sender, message.Directory, message.SearchPattern );
            Sender.Tell( new Done( fileCount ) );
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
