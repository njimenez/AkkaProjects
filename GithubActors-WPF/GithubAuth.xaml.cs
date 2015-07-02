using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ReactiveUI;
using System.Diagnostics;
using Akka.Actor;
using GithubActors_WPF.Actors;
using System.Windows.Media;
using System.Windows.Controls;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using MahApps.Metro.Controls;

namespace GithubActors_WPF
{
    /// <summary>
    /// Interaction logic for GithubAuth.xaml
    /// </summary>
    public partial class GithubAuth : MetroWindow
    {
        public GithubAuth()
        {
            InitializeComponent();
            DataContext = new GithubAuthViewModel( this );
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GithubAuthViewModel : ReactiveObject
    {
        // Fields...
        private Color m_StatusLabelForeColor;
        private string m_Status;
        private string m_OAuthToken;
        private bool m_StatusLabelVisible = false;
        private IActorRef m_vmActor;
        private readonly ContentControl m_View;

        /// <summary>
        /// Initializes a new instance of the <see cref="GithubAuthViewModel"/> class.
        /// </summary>
        public GithubAuthViewModel( ContentControl view )
        {
            m_View = view;
            m_OAuthToken = "";
            m_Status = "Press the button to authenticate";

            GetHelp = ReactiveCommand.Create( null );
            GetHelp.Subscribe( x => OpenLink() );

            Authenticate = ReactiveCommand.Create( null );
            Authenticate.Subscribe( x => CanAuthenticate() );

            var props = Props.Create( () => new GithubAuthenticationActor( this ) );
            m_vmActor = App.GithubSystem.ActorOf( props, ActorPaths.GithubAuthenticatorActor.Name );

        }

        public string OAuthToken
        {
            get { return m_OAuthToken; }
            set
            {
                this.RaiseAndSetIfChanged( ref m_OAuthToken, value );
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
        public bool StatusLabelVisible
        {
            get { return m_StatusLabelVisible; }
            set
            {
                this.RaiseAndSetIfChanged( ref m_StatusLabelVisible, value );
            }
        }
        public Color StatusLabelForeColor
        {
            get
            {
                return m_StatusLabelForeColor;
            }
            set
            {
                this.RaiseAndSetIfChanged( ref m_StatusLabelForeColor, value );
            }
        }

        public ReactiveCommand<object> GetHelp { get; private set; }
        public ReactiveCommand<object> Authenticate { get; private set; }

        public void Authenticated()
        {
            var launcherForm = new LauncherForm();
            launcherForm.Show();
            m_View.Visibility = Visibility.Hidden;
        }

        private void OpenLink()
        {
            Process.Start( "https://help.github.com/articles/creating-an-access-token-for-command-line-use/" );
        }

        private void CanAuthenticate()
        {
            m_vmActor.Tell( new GithubAuthenticationActor.Authenticate( OAuthToken ) );
        }
    }
}
