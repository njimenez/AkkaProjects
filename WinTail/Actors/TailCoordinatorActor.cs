using Akka.Actor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WinTail.Actors
{
    public class TailCoordinatorActor : ReceiveActor
    {
        #region Message types
        /// <summary>
        /// Start tailing the file at user-specified path.
        /// </summary>
        public class StartTail
        {
            public StartTail( string filePath, IObserveViewModel vm )
            {
                FilePath = filePath;
                ViewModel = vm;
            }

            public string FilePath
            { get; private set; }

            public IObserveViewModel ViewModel
            { get; private set; }
        }

        /// <summary>
        /// Stop tailing the file at user-specified path.
        /// </summary>
        public class StopTail
        {
            public StopTail( string filePath )
            {
                FilePath = filePath;
            }

            public string FilePath
            { get; private set; }
        }

        #endregion

        public static Props GetProps()
        {
            return Props.Create( () => new TailCoordinatorActor() );
        }

        /// <summary>
        /// Initializes a new instance of the TailCoordinatorActor class.
        /// </summary>
        public TailCoordinatorActor()
        {
            Ready();
        }

        protected void Ready()
        {
            Receive<StartTail>( msg => Handle( msg ) );
            Receive<StopTail>( msg => Handle( msg ) );
        }

        private void Handle( TailCoordinatorActor.StartTail msg )
        {
            Context.ActorOf( Props.Create( () => new TailActor( msg.ViewModel, msg.FilePath ) ) );
        }

        private void Handle( TailCoordinatorActor.StopTail msg )
        {
            foreach ( var child in Context.GetChildren() )
            {
                child.Tell( msg );
            }
        }

        // here we are overriding the default SupervisorStrategy
        // which is a One-For-One strategy w/ a Restart directive
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                10, // maxNumberOfRetries
                TimeSpan.FromSeconds( 30 ), // duration
                x =>
                {
                    //Maybe we consider ArithmeticException to not be application critical
                    //so we just ignore the error and keep going.
                    if ( x is ArithmeticException )
                        return Directive.Resume;

                    //Error that we cannot recover from, stop the failing actor
                    else if ( x is NotSupportedException )
                        return Directive.Stop;

                    //In all other cases, just restart the failing actor
                    else
                        return Directive.Restart;
                } );
        }
    }
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
    }

    public class FileReaderActor : ReceiveActor
    {
        private FileStream _fileStream;
        private StreamReader _fileStreamReader;
        private readonly string m_filename;

        public static Props GetProps( string filename )
        {
            return Props.Create( () => new FileReaderActor( filename ) );
        }

        public FileReaderActor( string filename )
        {
            m_filename = filename;
            Ready();
        }

        protected override void PreStart()
        {
            base.PreStart();

            // open the file stream with shared read/write permissions (so file can be written to while open)
            _fileStream = new FileStream( m_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite );
            _fileStreamReader = new StreamReader( _fileStream, Encoding.UTF8 );

        }

        protected override void PostStop()
        {
            _fileStreamReader.Close();
            _fileStreamReader.Dispose();
            base.PostStop();
        }

        private void Ready()
        {
            Receive<TailActor.ReadToEnd>( msg => Handle( msg ) );
        }

        private void Handle( TailActor.ReadToEnd msg )
        {
            var line = _fileStreamReader.ReadLine();
            while ( !String.IsNullOrEmpty( line ) )
            {
                Context.Parent.Tell( new TailActor.LineRead( line ) );
                line = _fileStreamReader.ReadLine();
            }
        }
    }

}
