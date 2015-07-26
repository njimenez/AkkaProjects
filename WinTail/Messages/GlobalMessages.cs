using System;

namespace WinTail.Messages
{
    public class EnumerateFiles
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartSearch"/> class.
        /// </summary>
        public EnumerateFiles( String folders, String extension )
        {
            Extension = extension;
            Folders = folders;
        }
        public String Folders { get; private set; }
        public String Extension { get; private set; }
    }

    public class ValidateArgs
    {

        public ValidateArgs( String folders, String extension )
        {
            Extension = extension;
            Folders = folders;
        }
        public String Folders { get; private set; }
        public String Extension { get; private set; }
    }

    public class InvalidArgs
    {
        public InvalidArgs( String errorMessage )
        {
            ErrorMessage = errorMessage;
        }
        public String ErrorMessage { get; private set; }
    }

    public class DirectoryToSearchMessage
    {
        public DirectoryToSearchMessage( string directory, string searchPattern )
        {
            Directory = directory;
            SearchPattern = searchPattern;
        }
        public string SearchPattern { get; private set; }
        public string Directory { get; private set; }
    }

    public class StatusMessage
    {

        public StatusMessage( String message )
        {
            Message = message;
        }
        public String Message { get; private set; }
    }

    public class Done
    {
        /// <summary>
        /// Initializes a new instance of the Done class.
        /// </summary>
        public Done( int count, TimeSpan elapsedMs )
        {
            ElapsedTime = elapsedMs;
            Count = count;
        }
        public int Count { get; private set; }
        public TimeSpan ElapsedTime
        { get; private set; }
    }
}
