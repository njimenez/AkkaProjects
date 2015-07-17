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
    public class WordCounterSupervisor : ReceiveActor
    {
        private IActorRef validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="WordCounterSupervisor"/> class.
        /// </summary>
        private readonly MainWindowViewModel m_vm;
        public WordCounterSupervisor( MainWindowViewModel vm )
        {
            m_vm = vm;
            validator = Context.ActorOf( FileValidatorActor.GetProps(), ActorPaths.FileValidator.Name );

            ValidatingInput();
        }

        private void ValidatingInput()
        {
            Receive<StartSearch>( msg => DoValidate( msg ) );
            Receive<InvalidArgs>( msg => DoInvalidArgs( msg ) );
            Receive<ValidArgs>( msg => DoCrawl( msg ) );
        }

        private void DoValidate( StartSearch msg )
        {
            validator.Tell( new ValidateArgs( msg.Folders, msg.Extension ) );
        }
        private void DoInvalidArgs( InvalidArgs msg )
        {
            m_vm.AddItem.OnNext( new ResultItem() { FilePath = msg.ErrorMessage } );
        }
        private void DoCrawl( ValidArgs msg )
        {
            throw new NotImplementedException();
        }
    }

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

            var fullPath = IsFileUri( Path.Combine( msg.Folders, msg.Extension ) );
            if ( String.IsNullOrEmpty( fullPath ) )
            {
                Sender.Tell( new InvalidArgs( String.Format( "Invalid Folder [{0}] [{1}]", msg.Folders, msg.Extension ) ) );
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
        private static String IsFileUri( string path )
        {
            var directory = String.Empty;
            var searchPattern = String.Empty;
            if ( Directory.Exists( path ) )
            {
                searchPattern = Path.GetExtension( path );
                if ( String.IsNullOrWhiteSpace( searchPattern ) )
                {
                    searchPattern = "*.txt";
                }
                directory = Path.GetDirectoryName( Path.Combine( path, searchPattern ) );
            }

            return directory;
        }
    }
}