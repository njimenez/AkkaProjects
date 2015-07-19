using Akka.Actor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WinTail.Messages;

namespace WinTail.Actors
{
    public class WinTailSupervisor : ReceiveActor
    {
        private IActorRef crawler;
        private IActorRef validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="WordCounterSupervisor"/> class.
        /// </summary>
        private readonly MainWindowViewModel m_vm;
        public WinTailSupervisor( MainWindowViewModel vm )
        {
            m_vm = vm;
            validator = Context.ActorOf( FileValidatorActor.GetProps(), "filevalidator" );
            crawler = Context.ActorOf( FileEnumeratorActor.GetProps(), "file-enumerator" );
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
            crawler.Tell( new EnumerateFiles( msg.Folders, msg.Extension ) );
        }
        private void Handle( FileInfo msg )
        {
            m_vm.AddItem.OnNext( msg );
           
        }
        private void Handle( StatusMessage msg )
        {
            m_vm.Status = msg.Message;
        }
        private void Handle( Done msg )
        {
            m_vm.Crawling = false;
            m_vm.Status = string.Format( "Found {0} file(s) in {1} ms", msg.Count, msg.ElapsedMilliseconds );
        }
    }
}
