using Akka.Actor;
using System;
using System.IO;
using System.Text;

namespace WinTail.Actors
{
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
            while ( line != null )
            {
                Context.Parent.Tell( new TailActor.LineRead( line ) );
                line = _fileStreamReader.ReadLine();
            }
        }
    }
}
