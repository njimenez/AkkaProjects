using Akka.Actor;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WordCounter.Actors;
using WordCounter.Messages;

namespace WordCounter
{
    public class MainWindowViewModel : ReactiveObject
    {
        private bool m_Crawling;
        private string m_Extension = String.Empty;
        private string m_Folders = String.Empty;
        private string m_Status = String.Empty;
        private readonly IActorRef m_vmActor;

        public MainWindowViewModel()
        {
            Extension = "*.txt";
            Folders = @"c:\Users\njimenez\Documents\Projects\CSharp\Games\Poker";
            Items = new ReactiveList<ResultItem>();

            CountCommand = ReactiveCommand.Create();
            CountCommand.Subscribe( x => DoCount() );

            // this is how we can update the viewmodel 
            // from the actor. 
            AddItem = new Subject<ResultItem>();
            AddItem.ObserveOnDispatcher().Subscribe( item => Items.Add( item ) );

            m_vmActor = AkkaSystem.System.ActorOf( WordCounterSupervisor.GetProps( this ), ActorPaths.WordCounterSupervisorActor.Name );
        }

        public ReactiveList<ResultItem> Items
        {
            get; set;
        }
        public Subject<ResultItem> AddItem
        {
            get; set;
        }
        public ReactiveCommand<object> CountCommand
        {
            get; private set;
        }

        public string Folders
        {
            get
            {
                return m_Folders;
            }
            set
            {
                this.RaiseAndSetIfChanged( ref m_Folders, value );
            }
        }
        public string Extension
        {
            get
            {
                return m_Extension;
            }
            set
            {
                this.RaiseAndSetIfChanged( ref m_Extension, value );
            }
        }
        public string Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                this.RaiseAndSetIfChanged( ref m_Status, value );
            }
        }
        public bool Crawling
        {
            get
            {
                return m_Crawling;
            }
            set
            {
                this.RaiseAndSetIfChanged( ref m_Crawling, value );
            }
        }

        public void Done()
        {
            Crawling = false;
        }

        private void DoCount()
        {
            if ( Crawling )
                return;

            CreateStatsWindow();
            Crawling = true;
            Items.Clear();
           m_vmActor.Tell( new StartSearch( Folders, Extension ) );
        }
        private void CreateStatsWindow()
        {
            var window = new StatsWindow();
            window.Show();
        }
    }
}
