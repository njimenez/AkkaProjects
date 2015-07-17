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

        public string FilePath { get; set; }
        public int TotalWords { get; set; }
        public long ElapsedMs { get; set; }

    }
}
