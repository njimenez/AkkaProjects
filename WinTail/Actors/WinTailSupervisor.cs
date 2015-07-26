using Akka.Actor;
using System;
using System.IO;
using WinTail.Messages;
using WinTail.ViewModels;

namespace WinTail.Actors
{
    public class WinTailSupervisor : ReceiveActor
    {
        private IActorRef fileEnumerator;
        private IActorRef validator;
        private IActorRef tailCoordinator;

        private readonly MainWindowViewModel m_vm;

        public static Props GetProps( MainWindowViewModel vm )
        {
            return Props.Create( () => new WinTailSupervisor( vm ) );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WordCounterSupervisor"/> class.
        /// </summary>
        public WinTailSupervisor( MainWindowViewModel vm )
        {
            m_vm = vm;
            validator = Context.ActorOf( FileValidatorActor.GetProps(), "filevalidator" );
            fileEnumerator = Context.ActorOf( FileEnumeratorActor.GetProps(), "file-enumerator" );
            tailCoordinator = Context.ActorOf( TailCoordinatorActor.GetProps(), "tailcoordinator" );
            Ready();
        }

        private void Ready()
        {
            // receive from parent
            Receive<EnumerateFiles>( msg => Handle( msg ) );

            // receive from validator
            Receive<ValidateArgs>( msg => Handle( msg ) );

            // receive from crawler
            Receive<FileInfo>( msg => Handle( msg ) );

            // receive from children
            Receive<StatusMessage>( msg => Handle( msg ) );
            Receive<Done>( msg => Handle( msg ) );
        }

        private void Handle( EnumerateFiles msg )
        {
            validator.Tell( new ValidateArgs( msg.Folders, msg.Extension ) );
        }
        private void Handle( ValidateArgs msg )
        {
            fileEnumerator.Tell( new EnumerateFiles( msg.Folders, msg.Extension ) );
        }
        private void Handle( FileInfo msg )
        {
            m_vm.Status = "Processing file " + msg.FullName;
            m_vm.AddItem.OnNext( new FileInfoViewModel()
            {
                DirectoryName = msg.DirectoryName.ToLower(),
                Name = msg.Name,
                Length = msg.Length,
                FullName = msg.FullName
            } );
        }
        private void Handle( StatusMessage msg )
        {
            m_vm.Status = msg.Message;
            m_vm.Crawling = false;
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
