using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public ValidArgs(String path)
        {
            Path = path;
        }
        public String Path { get; private set; }
    }
    public class LineToProcessMessage
    {
        public LineToProcessMessage( string line )
        {
            LineToProcess = line;
        }
        public string LineToProcess { get; private set; }
    }
    public class FinishMessage
    {
    }
    public class DirectoryToSearchMessage
    {
        public DirectoryToSearchMessage( string path, string searchPattern )
        {
            SearchPattern = searchPattern;
            Path = path;
        }

        public string Path { get; private set; }
        public string SearchPattern { get; private set; }
    }
    public class FileToProcessMessage
    {
        public FileToProcessMessage( string fileName, int fileno )
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
    public class WordsInLineMessage
    {
        public WordsInLineMessage( int wordsInLine )
        {
            WordsInLine = wordsInLine;
        }
        public int WordsInLine { get; private set; }
    }
    public class WordsInFileMessage
    {
        public WordsInFileMessage( string fileName, int wordsInFile, long elapsedMilliseconds )
        {
            ElapsedMilliseconds = elapsedMilliseconds;
            FileName = fileName;
            WordsInFile = wordsInFile;
        }
        public int WordsInFile { get; private set; }
        public string FileName { get; private set; }
        public long ElapsedMilliseconds { get; private set; }
    }
}
