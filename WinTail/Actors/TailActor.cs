using Akka.Actor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WinTail.ViewModels;

namespace WinTail.Actors
{
    /// <summary>
    /// Monitors the file at <see cref="_filePath"/> for changes and sends file updates to console.
    /// </summary>
    public class TailActor : ReceiveActor
    {
        #region Message types

        /// <summary>
        /// Signal that the file has changed, and we need to read the next line of the file.
        /// </summary>
        public class FileWrite
        {
            public FileWrite( /*string fileName*/ )
            {
                //FileName = fileName;
            }

            public string FileName
            { get; private set; }
        }

        /// <summary>
        /// Signal that the OS had an error accessing the file.
        /// </summary>
        public class FileError
        {
            public FileError( string fileName, string reason )
            {
                FileName = fileName;
                Reason = reason;
            }

            public string FileName
            { get; private set; }

            public string Reason
            { get; private set; }
        }

        public class FileDeleted {}


        /// <summary>
        /// Signal to read the initial contents of the file at actor startup.
        /// </summary>
        public class InitialRead
        {
        }
        public class ReadToEnd
        {
        }

        public class LineRead
        {
            /// <summary>
            /// Initializes a new instance of the LineRead class.
            /// </summary>
            public LineRead( String line )
            {
                Line = line;
            }

            public String Line
            { get; private set; }
        }


        #endregion

        private readonly string _filePath;
        private IActorRef _filereader;
        private readonly IObserveViewModel _vm;
        private FileObserver _observer;

        public TailActor( IObserveViewModel vm, string filePath )
        {
            _vm = vm;
            _filePath = filePath;
            Ready();
        }

        /// <summary>
        /// Initialization logic for actor that will tail changes to a file.
        /// </summary>
        protected override void PreStart()
        {
            var fullPath = Path.GetFullPath( _filePath );

            // start watching file for changes
            _observer = new FileObserver( Self, fullPath );
            _observer.Start();

            // read the initial contents of the file and send it to console as first message
            _filereader = Context.ActorOf( FileReaderActor.GetProps( fullPath ) );
            Self.Tell( new InitialRead() );
        }

        /// <summary>
        /// Cleanup OS handles for <see cref="_fileStreamReader"/> and <see cref="FileObserver"/>.
        /// </summary>
        protected override void PostStop()
        {
            _observer.Dispose();
            _observer = null;
            base.PostStop();
        }

        private void Ready()
        {
            Receive<InitialRead>( msg => Handle( msg ) );
            Receive<FileWrite>( msg => Handle( msg ) );
            Receive<FileError>( msg => Handle( msg ) );
            Receive<FileDeleted>( msg => Handle( msg ) );
            Receive<LineRead>( msg => Handle( msg ) );
            Receive<TailCoordinatorActor.StopTail>( msg => Handle( msg ) );

        }

        private void Handle( TailActor.InitialRead msg )
        {
            _filereader.Tell( new ReadToEnd() );
        }
        private void Handle( TailActor.FileWrite msg )
        {
            _filereader.Tell( new ReadToEnd() );
        }
        private void Handle( TailActor.LineRead msg )
        {
            if ( String.IsNullOrEmpty( msg.Line ) )
            {
                _vm.Lines.OnNext( "null line" );
            }
            else
            {
                _vm.Lines.OnNext( msg.Line );
            }
        }
        private void Handle( TailActor.FileError msg )
        {
            Context.Parent.Tell( string.Format( "Tail error: {0}", msg.Reason ) );
        }
        private void Handle( TailCoordinatorActor.StopTail msg )
        {
            if ( msg.FilePath == _filePath )
            {
                Context.Stop( Self );
            }
        }
        private void Handle( TailActor.FileDeleted msg )
        {
            _vm.FileDeleted();
            Context.Stop( Self );
        }
    }
}
