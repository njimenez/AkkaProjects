using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using Akka.Actor;
using GithubActors_WPF.Actors;
using GithubActors_WPF.Factories;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Media;
using MahApps.Metro.Controls;

namespace GithubActors_WPF
{
    /// <summary>
    /// Interaction logic for LauncherForm.xaml
    /// </summary>
    public partial class LauncherForm : MetroWindow
    {
        public LauncherForm()
        {
            InitializeComponent();
            DataContext = new LauncherFormViewModel();
        }

        private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            App.Current.Shutdown();
        }
    }

    public class LauncherFormViewModel : ReactiveObject
    {
        private IActorRef m_mainFormActor;
        private Subject<MainFormActor.LaunchRepoResultsWindow> m_LaunchWindow;
        private string m_RepoUrl = "";
        private string m_Status = String.Empty;
        private Color m_StatusForeColor = new Color();

        public LauncherFormViewModel()
        {
            /* INITIALIZE ACTORS */
            var props = Props.Create( () => new MainFormActor( this ) );
            m_mainFormActor = App.GithubSystem.ActorOf( props, ActorPaths.MainFormActor.Name );

            props = Props.Create( () => new GithubCommanderActor() );
            App.GithubSystem.ActorOf( props, ActorPaths.GithubCommanderActor.Name );

            props = Props.Create( () => new GithubValidatorActor( GithubClientFactory.GetClient() ) );
            App.GithubSystem.ActorOf( props, ActorPaths.GithubValidatorActor.Name );

            LaunchSearch = ReactiveCommand.Create( null );
            LaunchSearch.Subscribe( x => DoLaunchSearch() );

            // to be able to launch a window from the actor system
            m_LaunchWindow = new Subject<MainFormActor.LaunchRepoResultsWindow>();
            m_LaunchWindow.ObserveOnDispatcher().Subscribe( x => DoLaunchWindow( x ) );
        }

        public ReactiveCommand<object> LaunchSearch { get; private set; }

        public string RepoUrl
        {
            get { return m_RepoUrl; }
            set
            {
                this.RaiseAndSetIfChanged( ref m_RepoUrl, value );
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
        public Color StatusForeColor
        {
            get
            {
                return m_StatusForeColor;
            }
            set
            {
                this.RaiseAndSetIfChanged( ref m_StatusForeColor, value );
            }
        }

        private void DoLaunchSearch()
        {
            m_mainFormActor.Tell( new Messages.ProcessRepo( RepoUrl ) );
        }

        public void LaunchResults( MainFormActor.LaunchRepoResultsWindow window )
        {
            m_LaunchWindow.OnNext( window );
        }

        public void DoLaunchWindow( MainFormActor.LaunchRepoResultsWindow window )
        {
            var form = new RepoResultsForm( window.Coordinator, window.Repo );
            form.Show();
        }

        public void SetStatus( Color foreColor, string message )
        {
            Status = message;
            StatusForeColor = foreColor;
        }
    }

}
