using Akka.Actor;
using MahApps.Metro.Controls;
using System;
using System.IO;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WinTail.Actors;
using WinTail.Messages;
using WinTail.ViewModels;

namespace WinTail
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MainWindowViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
            vm = new MainWindowViewModel();
            DataContext = vm;
        }

        private void ListBox_MouseDoubleClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
        {
            vm.ObserveFile();
        }
    }

    
}
