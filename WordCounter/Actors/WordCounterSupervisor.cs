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
        private IActorRef crawler;
        private IActorRef validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="WordCounterSupervisor"/> class.
        /// </summary>
        private readonly MainWindowViewModel m_vm;
        public WordCounterSupervisor( MainWindowViewModel vm )
        {
            m_vm = vm;
            validator = Context.ActorOf( FileValidatorActor.GetProps(), ActorPaths.FileValidator.Name );
            crawler = Context.ActorOf<DirectoryCrawler>( "directoryCrawler" );
            Ready();
        }

        private void Ready()
        {
            // receive from parent
            Receive<StartSearch>( msg => Handle( msg ) );

            // receive from validator
            Receive<ValidateArgs>( msg => Handle( msg ) );

            // receive from crawler
            Receive<CompletedFile>( msg => Handle( msg ) );

            // receive from children
            Receive<StatusMessage>( msg => Handle( msg ) );
            Receive<Done>( msg => Handle( msg ) );
        }

        private void Handle( StartSearch msg )
        {
            validator.Tell( new ValidateArgs( msg.Folders, msg.Extension ) );
        }
        private void Handle( ValidateArgs msg )
        {
            crawler.Tell( new DirectoryToSearchMessage( msg.Folders, msg.Extension ) );
        }
        private void Handle( CompletedFile msg )
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
        private void Handle( StatusMessage msg )
        {
            m_vm.Status = msg.Message;
        }
        private void Handle( Done msg )
        {
            m_vm.Crawling = false;
        }
    }
}