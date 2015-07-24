using Akka.Actor;
using System.Windows;

namespace WordCounter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ActorSystem WordCounterSystem;

        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );
            WordCounterSystem = ActorSystem.Create( "word-counter" );
        }
    }
}
