using Akka.Actor;
using MahApps.Metro.Controls;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WinTail.Actors;

namespace WinTail
{
    /// <summary>
    /// Interaction logic for ObserveWindow.xaml
    /// </summary>
    public partial class ObserveWindow : MetroWindow
    {
        private IObserveViewModel _vm;

        public ObserveWindow( String filename, IActorRef tailCoordinator )
        {
            InitializeComponent();
            _vm = new ObserveWindowViewModel( filename, tailCoordinator );
            DataContext = _vm;
        }

        private void MetroWindow_Closing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            _vm.Stop();
        }
    }

    public interface IObserveViewModel
    {
        Subject<String> Lines { get; }
        void Stop();
    }
    
    public class ObserveWindowViewModel : ReactiveObject, IObserveViewModel
    {
        private string m_Filename = String.Empty;
        private string m_Status = String.Empty;
        private string m_Title = String.Empty;
        private readonly IActorRef _tailCoordinator;

        public ObserveWindowViewModel( string filename, IActorRef tailCoordinator )
        {
            _tailCoordinator = tailCoordinator;
            Filename = filename;
            Title = filename;
            Items = new ReactiveList<String>();

            // start coordinator
            _tailCoordinator.Tell( new TailCoordinatorActor.StartTail( filename, this ) );

            // this is how we can update the viewmodel 
            // from the actor. 
            Lines = new Subject<String>();
            Lines.ObserveOnDispatcher().Subscribe( item => Items.Add( item ) );
        }

        public ReactiveList<String> Items { get; set; }
        public Subject<String> Lines { get; }

        public string Filename
        {
            get { return m_Filename; }
            set
            {
                this.RaiseAndSetIfChanged( ref m_Filename, value );
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
        public string Title
        {
            get { return m_Title; }
            set
            {
                this.RaiseAndSetIfChanged( ref m_Title, value );
            }
        }

        public void Stop()
        {
            _tailCoordinator.Tell( new TailCoordinatorActor.StopTail( Filename ) );
        }
    }

}
