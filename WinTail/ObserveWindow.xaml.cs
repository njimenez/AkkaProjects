using Akka.Actor;
using MahApps.Metro.Controls;
using System;
using WinTail.ViewModels;

namespace WinTail
{
    /// <summary>
    /// Interaction logic for ObserveWindow.xaml
    /// </summary>
    public partial class ObserveWindow : MetroWindow
    {
        private IObserveViewModel _vm;

        public ObserveWindow( String filename, IActorRef tailCoordinator )
        {
            InitializeComponent();
            _vm = new ObserveWindowViewModel( filename, tailCoordinator );
            DataContext = _vm;
        }

        private void MetroWindow_Closing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            _vm.Stop();
        }
    }
}
