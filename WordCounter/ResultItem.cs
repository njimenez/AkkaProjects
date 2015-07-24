using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordCounter
{
    public class ResultItem
    {
        public ResultItem()
        {
        }

        public string DirectoryPath
        { get; set; }
        public string FileName
        { get; set; }
        public string FilePath
        { get; set; }
        public int TotalWords
        { get; set; }
        public int TotalLines
        { get; set; }
        public long ElapsedMs
        { get; set; }

    }
}
