using MahApps.Metro.Controls;

namespace WordCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MainWindowViewModel m_vm;

        public MainWindow()
        {
            InitializeComponent();
            m_vm = new MainWindowViewModel();
            DataContext = m_vm;
        }

        private void MetroWindow_Closing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            m_vm.Closing();
        }
    }
}
