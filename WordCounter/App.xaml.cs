using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WordCounter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {  /// <summary>
        /// ActorSystem we'll be using to collect and process data
        /// from Github using their official .NET SDK, Octokit
        /// </summary>
        public static ActorSystem WordCounterSystem;

        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );
            WordCounterSystem = ActorSystem.Create( "word-counter" );
        }
    }
}
