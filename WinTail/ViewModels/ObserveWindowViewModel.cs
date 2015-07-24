using Akka.Actor;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WinTail.Actors;

namespace WinTail.ViewModels
{
    public interface IObserveViewModel
    {
        Subject<String> Lines
        { get; }

        void Stop();

        void FileDeleted();
    }

    public class ObserveWindowViewModel : ReactiveObject, IObserveViewModel
    {
        private string m_Filename = String.Empty;
        private int m_SelectedLine = 0;
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
            Lines.ObserveOnDispatcher().Subscribe( item => { Items.Add( item ); SelectedLine = Items.Count - 1; } );
        }

        public ReactiveList<String> Items
        { get; set; }
        public Subject<String> Lines
        { get; }

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

        public int SelectedLine
        {
            get { return m_SelectedLine; }
            set
            {
                this.RaiseAndSetIfChanged( ref m_SelectedLine, value );
            }
        }

        public void Stop()
        {
            _tailCoordinator.Tell( new TailCoordinatorActor.StopTail( Filename ) );
        }

        public void FileDeleted()
        {
            Lines.OnNext( "File has been deleted!" );
        }
    }
}
