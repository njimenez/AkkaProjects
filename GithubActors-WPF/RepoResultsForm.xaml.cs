using Akka.Actor;
using GithubActors_WPF.Actors;
using MahApps.Metro.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace GithubActors_WPF
{
    /// <summary>
    /// Interaction logic for RepoResultsForm.xaml
    /// </summary>
    public partial class RepoResultsForm : MetroWindow
    {
        private RepoResultsViewModel m_vm;

        public RepoResultsForm( IActorRef githubCoordinator, Messages.RepoKey repo )
        {
            InitializeComponent();
            m_vm = new RepoResultsViewModel( githubCoordinator, repo );
            DataContext = m_vm;
        }

        private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            m_vm.Stop();
        }
    }

    public class RepoViewModel : ReactiveObject
    {
        // Fields...
        private int forksCount;
        private int stargazers;
        private int sharedStars;
        private int subscribers;
        private string url;
        private string name;
        private string user;

        public string User
        {
            get { return user; }
            set
            {
                this.RaiseAndSetIfChanged( ref user, value );
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                this.RaiseAndSetIfChanged( ref name, value );
            }
        }
        public string Url
        {
            get { return url; }
            set
            {
                this.RaiseAndSetIfChanged( ref url, value );
            }
        }
        public int SharedStars
        {
            get { return sharedStars; }
            set
            {
                this.RaiseAndSetIfChanged( ref sharedStars, value );
            }
        }
        public int Subscribers
        {
            get { return subscribers; }
            set
            {
                this.RaiseAndSetIfChanged( ref subscribers, value );
            }
        }
        public int Stargazers
        {
            get { return stargazers; }
            set
            {
                this.RaiseAndSetIfChanged( ref stargazers, value );
            }
        }
        public int ForksCount
        {
            get { return forksCount; }
            set
            {
                this.RaiseAndSetIfChanged( ref forksCount, value );
            }
        }
    }

    public class RepoResultsViewModel : ReactiveObject
    {
        private Color m_color;
        private int progressMax = 0;
        private int progressValue = 0;
        private string status = String.Empty;
        private string title = String.Empty;
        private IActorRef _formActor;
        public ReactiveList<RepoViewModel> Items { get; set; }

        public RepoResultsViewModel( IActorRef githubCoordinator, Messages.RepoKey repo )
        {
            //run on the UI thread;
            var props = Props.Create( () => new RepoResultsActor( this ) )
                                .WithDispatcher( "akka.actor.synchronized-dispatcher" ); 

            _formActor = App.GithubSystem.ActorOf( props );

            Title = string.Format( "Repos Similar to {0} / {1}", repo.Owner, repo.Repo );

            //start subscribing to updates
            githubCoordinator.Tell( new GithubCoordinatorActor.SubscribeToProgressUpdates( _formActor ) );

            Items = new ReactiveList<RepoViewModel>();
        }

        public void Stop()
        {
            // kill the form actor
            _formActor.Tell( PoisonPill.Instance );
        }
        public string Title
        {
            get { return title; }
            set
            {
                this.RaiseAndSetIfChanged( ref title, value );
            }
        }
        public string Status
        {
            get { return status; }
            set
            {
                this.RaiseAndSetIfChanged( ref status, value );
            }
        }
        public int ProgressValue
        {
            get { return progressValue; }
            set
            {
                this.RaiseAndSetIfChanged( ref progressValue, value );
            }
        }
        public int ProgressMax
        {
            get { return progressMax; }
            set
            {
                this.RaiseAndSetIfChanged( ref progressMax, value );
            }
        }
        public Color Background
        {
            get { return m_color; }
            set
            {
                this.RaiseAndSetIfChanged( ref m_color, value );
            }
        }
    }
}