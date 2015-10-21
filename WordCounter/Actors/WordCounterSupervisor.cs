using Akka.Actor;
using System;
using System.IO;
using Akka.Monitoring;
using WordCounter.Messages;

namespace WordCounter.Actors
{
    public class WordCounterSupervisor : BaseMonitoringActor
    {
        private readonly IActorRef crawler;
        private readonly IActorRef validator;
        private readonly MainWindowViewModel m_vm;

        public static Props GetProps( MainWindowViewModel vm )
        {
            return Props.Create( () => new WordCounterSupervisor( vm ) );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WordCounterSupervisor"/> class.
        /// </summary>     
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
            IncrementMessagesReceived();
            validator.Tell( new ValidateArgs( msg.Folders, msg.Extension ) );
        }
        private void Handle( ValidateArgs msg )
        {
            IncrementMessagesReceived();
            crawler.Tell( new DirectoryToSearchMessage( msg.Folders, msg.Extension ) );
        }
        private void Handle( InvalidArgs msg )
        {
            IncrementMessagesReceived();
            m_vm.Status = msg.ErrorMessage;
            m_vm.Crawling = false;
        }
        private void Handle( CompletedFile msg )
        {
            IncrementMessagesReceived();
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
            IncrementMessagesReceived();
            m_vm.Status = msg.Message;
        }
        private void Handle( Done msg )
        {
            IncrementMessagesReceived();
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