using Akka.Actor;
using System.Windows;

namespace WinTail
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ActorSystem WinTailSystem;

        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );
            WinTailSystem = ActorSystem.Create( "wintail" );
        }
    }
}
