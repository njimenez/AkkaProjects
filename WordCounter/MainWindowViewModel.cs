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
        private StatsWindow statsWindow;
        private readonly IActorRef m_wordCounterSupervisor;

        public MainWindowViewModel()
        {
            Extension = "*.txt";
            Folders = @"d:\downloads";
            Items = new ReactiveList<ResultItem>();

            // create the condition in which the count command is enabled.
            var canCount = this.WhenAny( x => x.m_Crawling, x => !x.Value );

            // create the command that will be executed when the 
            // count button is clicked.
            CountCommand = ReactiveCommand.Create(canCount);
            CountCommand.Subscribe( x => DoCount() );

            // this is how we can update the viewmodel 
            // from the actor. 
            AddItem = new Subject<ResultItem>();
            AddItem.ObserveOnDispatcher().Subscribe( item => Items.Add( item ) );

            // create the word counter supervisor
            m_wordCounterSupervisor = AkkaSystem.System.ActorOf( WordCounterSupervisor.GetProps( this ), ActorPaths.WordCounterSupervisorActor.Name );
        }

        public ReactiveList<ResultItem> Items { get; set; }
        public Subject<ResultItem> AddItem { get; set; }
        public ReactiveCommand<object> CountCommand { get; private set; }

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
        public void Closing()
        {
            statsWindow.Close();
        }

        /// <summary>
        /// Event method called when we press the count button on the Front End.
        /// </summary>
        private void DoCount()
        {
            // if we are crawling already
            if ( Crawling )
                return;

            CreateStatsWindow();
            Crawling = true;
            Items.Clear();
            m_wordCounterSupervisor.Tell( new StartSearch( Folders, Extension ) );
        }
        /// <summary>
        /// Creates the statistics window
        /// </summary>
        private void CreateStatsWindow()
        {
            if ( statsWindow == null )
            {
                statsWindow = new StatsWindow();
                statsWindow.Show();
            }
        }
    }
}
