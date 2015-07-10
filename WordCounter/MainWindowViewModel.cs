using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WordCounter
{
    public class Results : ObservableCollection<ResultItem> {
        /// <summary>
        /// Initializes a new instance of the Results class.
        /// </summary>
        public Results()
        {
            
        }
    }

    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            Items = new Results();

            Items.Add( new ResultItem() { FilePath = @"c:\temp\file1.txt", TotalWords = 50 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file2.txt", TotalWords = 150 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file3.txt", TotalWords = 1250 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350 } );

        }

        public Results Items { get; set; }
    }
}
