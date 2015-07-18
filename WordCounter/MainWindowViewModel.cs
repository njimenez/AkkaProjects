using Akka.Actor;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WordCounter.Actors;
using WordCounter.Messages;

namespace WordCounter
{
    public class Results : ReactiveList<ResultItem>
    {
        /// <summary>
        /// Initializes a new instance of the Results class.
        /// </summary>
        public Results()
        {
        }
    }

    public class MainWindowViewModel : ReactiveObject
    {
        private string m_Extension = String.Empty;
        private string m_Folders = String.Empty;
        private string m_Status = String.Empty;
        private IActorRef m_vmActor;


        public MainWindowViewModel()
        {
            Extension = "*.txt";
            Folders = @"C:\Users\njimenez\Documents\Projects\BondFire\BondFire.Net\";
            //Folders = @"D:\Projects\HelixProjects\GovBond\GovBond\Production";
            Items = new Results();

            CountCommand = ReactiveCommand.Create(  );
            CountCommand.Subscribe( x => DoCount() );
            
            // this is how we can update the viewmodel 
            // from the actor. 
            AddItem = new Subject<ResultItem>();
            AddItem.ObserveOnDispatcher().Subscribe( item => Items.Add( item ) );

            var props = Props.Create( () => new WordCounterSupervisor( this ) );
            m_vmActor = App.WordCounterSystem.ActorOf( props, ActorPaths.WordCounterSupervisorActor.Name );
        }

        public Results Items { get; set; }
        public string Folders
        {
            get { return m_Folders; }
            set
            {
                this.RaiseAndSetIfChanged( ref m_Folders, value );
            }
        }
        public string Extension
        {
            get { return m_Extension; }
            set
            {
                this.RaiseAndSetIfChanged( ref m_Extension, value );
            }
        }
        public string Status
        {
            get { return m_Status; }
            set
            {
                this.RaiseAndSetIfChanged( ref m_Status, value );
            }
        }
        public Subject<ResultItem> AddItem { get; set; }
        public ReactiveCommand<object> CountCommand { get; private set; }

        private void DoCount()
        {
            Items.Clear();
            m_vmActor.Tell( new StartSearch( Folders, Extension ) );
        }
    }

    public class MockMainWindowViewModel : MainWindowViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockMainViewModel"/> class.
        /// </summary>
        public MockMainWindowViewModel()
        {
            Items = new Results();
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file1.txt", TotalWords = 50, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file2.txt", TotalWords = 150, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file3.txt", TotalWords = 1250, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350, ElapsedMs = 1000 } );
            Items.Add( new ResultItem() { FilePath = @"c:\temp\file4.txt", TotalWords = 12350, ElapsedMs = 1000 } );
        }
    }
}
