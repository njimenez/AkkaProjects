using Akka.Actor;
using System;
using System.IO;
using WordCounter.Messages;

namespace WordCounter.Actors
{
    public class WordCounterSupervisor : ReceiveActor
    {
        private IActorRef crawler;
        private IActorRef validator;

        public static Props GetProps( MainWindowViewModel vm )
        {
            return Props.Create( () => new WordCounterSupervisor( vm ) );
        }

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

            // receive when arguments are invalid
            Receive<InvalidArgs>( msg => Handle( msg ) );

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
        private void Handle( InvalidArgs msg )
        {
            m_vm.Status = msg.ErrorMessage;
            m_vm.Crawling = false;
        }
        private void Handle( CompletedFile msg )
        {
            m_vm.AddItem.OnNext( new ResultItem()
            {
                FilePath = msg.FileName,
                DirectoryPath = Path.GetDirectoryName( msg.FileName ),
                FileName = Path.GetFileName( msg.FileName ),
                TotalWords = msg.WordsInFile,
                TotalLines = msg.LinesInFile,
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
            m_vm.Status = string.Format( "Processed {0:N0} file(s) in total time of {1}", msg.Count, Convert( msg.ElapsedTime ) );
        }
        private String Convert( TimeSpan ts )
        {
            var result = String.Empty;

            if ( ts.Hours > 0 )
            {
                result = string.Format( "{0:D} hours", ts.Hours );
            }

            if ( ts.Minutes > 0 )
            {
                result += string.Format( " {0:D} min", ts.Minutes );
            }
            if ( ts.Seconds > 0 )
            {
                result += string.Format( " {0:D} secs", ts.Seconds );
            }

            if ( ts.Milliseconds > 0 )
            {
                result += string.Format( " {0:D3} millsecs", ts.Milliseconds );
            }
            return result;
        }

    }
}