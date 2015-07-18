using Akka.Actor;
using System;
using System.Collections.Generic;
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
            // receive from parent
            Receive<StartSearch>( msg => DoValidate( msg ) );

            // receive from validator
            Receive<ValidArgs>( msg => DoCrawl( msg ) );

            // receive from crawler
            Receive<WordsInFileMessage>( msg => DoDisplay( msg ) );

            // receive from children
            Receive<StatusMessage>( msg => DoStatus( msg ) );
        }

        private void DoValidate( StartSearch msg )
        {
            validator.Tell( new ValidateArgs( msg.Folders, msg.Extension ) );
        }
        private void DoCrawl( ValidArgs msg )
        {
            // TODO : if we click the button again we get exception
            var crawler = Context.ActorOf<DirectoryCrawler>( "directoryCrawler" );
            crawler.Tell( new DirectoryToSearchMessage( msg.Fullpath ) );
        }
        private void DoDisplay( WordsInFileMessage msg )
        {
            m_vm.AddItem.OnNext( new ResultItem()
            {
                FilePath = msg.FileName,
                DirectoryPath = Path.GetDirectoryName( msg.FileName ),
                FileName = Path.GetFileName( msg.FileName ),
                TotalWords = msg.WordsInFile,
                ElapsedMs = msg.ElapsedMilliseconds
            } );
        }
        private void DoStatus( StatusMessage msg )
        {
            m_vm.Status = msg.Message;
        }
    }
}