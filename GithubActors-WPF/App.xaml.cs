using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Akka.Actor;

namespace GithubActors_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// ActorSystem we'll be using to collect and process data
        /// from Github using their official .NET SDK, Octokit
        /// </summary>
        public static ActorSystem GithubSystem;

        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );
            GithubSystem = ActorSystem.Create( "github-system" );
        }
    }
}
