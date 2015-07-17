using Akka.Actor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WordCounter.Messages;

namespace WordCounter.Actors
{
    public class FileValidatorActor : ReceiveActor
    {
        public static Props GetProps()
        {
            return Props.Create( () => new FileValidatorActor() );
        }
        public FileValidatorActor()
        {
            Receive<ValidateArgs>( msg => DoValidate( msg ) );
        }
        private void DoValidate( ValidateArgs msg )
        {
            if ( String.IsNullOrEmpty( msg.Folders ) )
            {
                Sender.Tell( new InvalidArgs( "Folders argument is empty." ) );
            }

            var fullPath = IsFileUri( msg.Folders, msg.Extension );
            if ( String.IsNullOrEmpty( fullPath ) )
            {
                Sender.Tell( new StatusMessage( String.Format( "Invalid Folder [{0}] [{1}]", msg.Folders, msg.Extension ) ) );
            }
            else
            {
                Sender.Tell( new ValidArgs( fullPath ) );
            }
        }
        /// <summary>
        /// Checks if file exists at path provided by user.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static String IsFileUri( string path, string extension )
        {
            var directory = String.Empty;
            var searchPattern = extension;
            if ( Directory.Exists( path ) )
            {
                searchPattern = extension;
                if ( String.IsNullOrWhiteSpace( searchPattern ) )
                {
                    searchPattern = "*.txt";
                }
                directory = Path.Combine( path, searchPattern );
            }

            return directory;
        }
    }
}
