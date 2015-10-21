using Akka.Actor;
using System;
using System.IO;
using WordCounter.Messages;

namespace WordCounter.Actors
{
    public class FileValidatorActor : BaseMonitoringActor
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
            IncrementMessagesReceived();
            if ( String.IsNullOrEmpty( msg.Folders ) )
            {
                Sender.Tell( new InvalidArgs( "Folders argument is empty." ) );
                
            }
            else
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
                Sender.Tell( new InvalidArgs( String.Format( "Invalid Folder [{0}] [{1}]", msg.Folders, msg.Extension ) ) );
            }
        }
    }
}
