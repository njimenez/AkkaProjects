using Akka.Actor;
using Akka.Monitoring;
using AkkaStats;
using System.Windows;

namespace WordCounter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );
            AkkaSystem.Start( "word-counter" );
        }
    }
    public static class AkkaSystem
    {
        /// <summary>
        /// Reference to the <see cref="ActorSystem"/>
        /// </summary>
        public static ActorSystem System;

        public static void Start( string systemName )
        {
            /*
            * Initialize ActorSystem and essential system actors
            */
            System = ActorSystem.Create( systemName );
            var monitor = new AkkaMonitoringPublisher( System );
            ActorMonitoringExtension.RegisterMonitor( System, monitor );
        }


    }
}
