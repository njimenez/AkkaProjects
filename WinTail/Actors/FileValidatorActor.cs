using Akka.Actor;
using System;
using System.IO;
using WinTail.Messages;

namespace WinTail.Actors
{
    public class FileValidatorActor : ReceiveActor
    {
        public static Props GetProps()
        {
            return Props.Create( () => new FileValidatorActor() );
        }
        public FileValidatorActor()
        {
            Receive<ValidateArgs>( msg => Handle( msg ) );
        }
        private void Handle( ValidateArgs msg )
        {
            if ( String.IsNullOrEmpty( msg.Folders ) )
            {
                Sender.Tell( new InvalidArgs( "Folders argument is empty." ) );
            }

            if ( Directory.Exists( msg.Folders ) )
            {
                var extension = msg.Extension;
                if ( String.IsNullOrEmpty( extension ) )
                {
                    extension = "*.txt";
                }
                Sender.Tell( new ValidateArgs( msg.Folders, extension ) );
            }
            else
            {
                Sender.Tell( new StatusMessage( String.Format( "Invalid Folder [{0}] [{1}]", msg.Folders, msg.Extension ) ) );
            }
        }
    }
}
