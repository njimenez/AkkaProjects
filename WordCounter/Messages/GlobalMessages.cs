using Akka.Actor;
using System;

namespace WordCounter.Messages
{
    public class StartSearch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartSearch"/> class.
        /// </summary>
        public StartSearch( String folders, String extension )
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
    public class ValidArgs
    {
        public ValidArgs( String fullpath )
        {
            Fullpath = fullpath;
        }
        public String Fullpath { get; private set; }
    }
    public class ProcessLine
    {
        public ProcessLine( string line, int lineNumber )
        {
            LineNumber = lineNumber;
            LineToProcess = line;
        }
        public string LineToProcess { get; private set; }
        public int LineNumber { get; private set; }
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
    public class FileToProcess
    {
        public FileToProcess( string fileName, int fileno )
        {
            Fileno = fileno;
            FileName = fileName;
        }

        public string FileName { get; private set; }
        public int Fileno { get; private set; }
    }
    public class FailureMessage
    {
        public FailureMessage( Exception ex, IActorRef actor )
        {
            Cause = ex;
            Child = actor;
        }

        public Exception Cause { get; private set; }
        public IActorRef Child { get; private set; }
    }
    public class WordCount
    {
        public WordCount( int wordsInLine )
        {
            WordsInLine = wordsInLine;
        }
        public int WordsInLine { get; private set; }
    }
    public class CompletedFile
    {
        public CompletedFile( string fileName, int wordsInFile, int linesInFile, long elapsedMilliseconds )
        {
            LinesInFile = linesInFile;
            ElapsedMilliseconds = elapsedMilliseconds;
            FileName = fileName;
            WordsInFile = wordsInFile;
        }
        public int WordsInFile { get; private set; }
        public string FileName { get; private set; }
        public long ElapsedMilliseconds { get; private set; }
        public int LinesInFile { get; private set; }
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
        public TimeSpan ElapsedTime { get; private set; }
    }

    public class DoneEnumeratingFiles 
    {
        /// <summary>
        /// Initializes a new instance of the DoneEnumeratingFiles class.
        /// </summary>
        public DoneEnumeratingFiles( int count, TimeSpan elapsedMs )
        {
            ElapsedTime = elapsedMs;
            Count = count;
        }
        public int Count
        { get; private set; }
        public TimeSpan ElapsedTime
        { get; private set; }
    }
}
