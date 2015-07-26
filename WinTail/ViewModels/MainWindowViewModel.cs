using Akka.Actor;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WinTail.Actors;
using WinTail.Messages;

namespace WinTail.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private Boolean m_Crawling = false;
        private string m_Extension = String.Empty;
        private string m_Folders = String.Empty;
        private FileInfoViewModel m_SelectedItem;
        private string m_Status = String.Empty;

        private IActorRef m_tailCoordinator;
        private IActorRef m_vmActor;

        public MainWindowViewModel()
        {
            Extension = "*.txt";
            Folders = @"c:\";
            Items = new ReactiveList<FileInfoViewModel>();

            CrawlCommand = ReactiveCommand.Create();
            CrawlCommand.Subscribe( x => Handle() );

            ObserveCommand = ReactiveCommand.Create();
            ObserveCommand.Subscribe( x => ObserveFile() );

            AddItem = new Subject<FileInfoViewModel>();
            AddItem.ObserveOnDispatcher().Subscribe( item => Items.Add( item ) );

            m_vmActor = App.WinTailSystem.ActorOf( WinTailSupervisor.GetProps( this ), "supervisor" );
            m_tailCoordinator = App.WinTailSystem.ActorOf( TailCoordinatorActor.GetProps(), "tailcoordinator" );
        }

        public ReactiveList<FileInfoViewModel> Items
        { get; set; }
        public Subject<FileInfoViewModel> AddItem
        { get; set; }
        public ReactiveCommand<object> CrawlCommand
        { get; private set; }
        public ReactiveCommand<object> ObserveCommand
        { get; private set; }

        public FileInfoViewModel SelectedItem
        {
            get
            {
                return m_SelectedItem;
            }
            set
            {
                this.RaiseAndSetIfChanged( ref m_SelectedItem, value );
            }
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
        public Boolean Crawling
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
        private void Handle()
        {
            if ( Crawling )
            {
                return;
            }
            Crawling = true;
            Items.Clear();
            m_vmActor.Tell( new EnumerateFiles( Folders, Extension ) );
        }
        public void ObserveFile()
        {
            // had to put this catch here; because if you double click on the vertical scroll bar this gets fired.
            if ( SelectedItem == null )
            {
                return;
            }
            var form = new ObserveWindow( SelectedItem.FullName, m_tailCoordinator );
            form.Show();
        }
    }
}
