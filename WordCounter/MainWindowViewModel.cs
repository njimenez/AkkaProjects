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
        private Boolean m_Crawling = false;
        private string m_Extension = String.Empty;
        private string m_Folders = String.Empty;
        private string m_Status = String.Empty;
        private IActorRef m_vmActor;

        public MainWindowViewModel()
        {
            Extension = "*.txt";
            Folders = @"c:\";
            Items = new ReactiveList<ResultItem>();

            CountCommand = ReactiveCommand.Create();
            CountCommand.Subscribe( x => DoCount() );

            // this is how we can update the viewmodel 
            // from the actor. 
            AddItem = new Subject<ResultItem>();
            AddItem.ObserveOnDispatcher().Subscribe( item => Items.Add( item ) );

            m_vmActor = App.WordCounterSystem.ActorOf( WordCounterSupervisor.GetProps( this ), ActorPaths.WordCounterSupervisorActor.Name );
        }

        public ReactiveList<ResultItem> Items
        { get; set; }
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
        public Boolean Crawling
        {
            get { return m_Crawling; }
            set
            {
                this.RaiseAndSetIfChanged( ref m_Crawling, value );
            }
        }

        public Subject<ResultItem> AddItem
        { get; set; }
        public ReactiveCommand<object> CountCommand
        { get; private set; }

        public void Done()
        {
            Crawling = false;
        }

        private void DoCount()
        {
            if ( Crawling )
                return;

            Crawling = true;
            Items.Clear();
            m_vmActor.Tell( new StartSearch( Folders, Extension ) );

        }
    }
}
