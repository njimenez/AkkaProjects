using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
