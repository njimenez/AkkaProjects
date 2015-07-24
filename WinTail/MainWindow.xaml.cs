using Akka.Actor;
using MahApps.Metro.Controls;
using System;
using System.IO;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WinTail.Actors;
using WinTail.Messages;


namespace WinTail
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MainWindowViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
            vm = new MainWindowViewModel();
            DataContext = vm;
        }

        private void ListBox_MouseDoubleClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
        {
            vm.ObserveFile();
        }
    }

    public class MainWindowViewModel : ReactiveObject
    {
        private Boolean m_Crawling = false;
        private string m_Extension = String.Empty;
        private string m_Folders = String.Empty;
        private FileInfo m_SelectedItem;
        private string m_Status = String.Empty;

        private IActorRef m_tailCoordinator;
        private IActorRef m_vmActor;

        public MainWindowViewModel()
        {
            Extension = "*.txt";
            Folders = @"D:\Projects\TEMP\tailtest";
            Items = new ReactiveList<FileInfo>();

            CrawlCommand = ReactiveCommand.Create();
            CrawlCommand.Subscribe( x => Handle() );

            ObserveCommand = ReactiveCommand.Create();
            ObserveCommand.Subscribe( x => ObserveFile() );

            // this is how we can update the viewmodel 
            // from the actor. 
            AddItem = new Subject<FileInfo>();
            AddItem.ObserveOnDispatcher().Subscribe( item => Items.Add( item ) );

            m_vmActor = App.WinTailSystem.ActorOf( WinTailSupervisor.GetProps( this ), "supervisor" );
            m_tailCoordinator = App.WinTailSystem.ActorOf( TailCoordinatorActor.GetProps(), "tailcoordinator" );
        }

        public ReactiveList<FileInfo> Items { get; set; }
        public Subject<FileInfo> AddItem { get; set; }
        public ReactiveCommand<object> CrawlCommand { get; private set; }
        public ReactiveCommand<object> ObserveCommand { get; private set; }

        public FileInfo SelectedItem
        {
            get { return m_SelectedItem; }
            set
            {
                this.RaiseAndSetIfChanged( ref m_SelectedItem, value );
                Status = value.FullName;
            }
        }

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
        public void Done()
        {
            Crawling = false;
        }
        private void Handle()
        {
            // there is supposed to be a better way to do this using Rx
            // but I have not been able to figure it out
            if ( Crawling )
                return;

            Crawling = true;
            Items.Clear();
            m_vmActor.Tell( new EnumerateFiles( Folders, Extension ) );

        }
        public void ObserveFile()
        {
            var form = new ObserveWindow( SelectedItem.FullName, m_tailCoordinator );
            form.Show();
        }
    }
}
