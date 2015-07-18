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
            Receive<Done>( msg => Handle( msg ) );
        }

        private void DoValidate( StartSearch msg )
        {
            validator.Tell( new ValidateArgs( msg.Folders, msg.Extension ) );
        }
        private void DoCrawl( ValidArgs msg )
        {          
            crawler.Tell( new DirectoryToSearchMessage( msg.Fullpath ) );
        }
        private void DoDisplay( WordsInFileMessage msg )
        {
            m_vm.AddItem.OnNext( new ResultItem() { FilePath = msg.FileName, TotalWords = msg.WordsInFile, ElapsedMs = msg.ElapsedMilliseconds } );
        }
        private void DoStatus( StatusMessage msg )
        {
            m_vm.Status = msg.Message;
        }
        private void Handle( Done msg )
        {
           // m_vm.EnableButton( true );
        }
    }
}