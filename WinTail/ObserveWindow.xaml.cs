using MahApps.Metro.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WinTail
{
    /// <summary>
    /// Interaction logic for ObserveWindow.xaml
    /// </summary>
    public partial class ObserveWindow : MetroWindow
    {
        public ObserveWindow( String filename )
        {
            InitializeComponent();
            DataContext = new ObserveWindowViewModel( filename );
        }
    }

    public class ObserveWindowViewModel : ReactiveObject
    {
        private string m_Filename = String.Empty;
        private string m_Status = String.Empty;
        private string m_Title = String.Empty;

        public ObserveWindowViewModel( string filename )
        {
            Filename = filename;
            Title = filename;
        }

        public string Filename
        {
            get { return m_Filename; }
            set
            {
                this.RaiseAndSetIfChanged( ref m_Filename, value );
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
        public string Title
        {
            get { return m_Title; }
            set
            {
                this.RaiseAndSetIfChanged( ref m_Title, value );
            }
        }
    }

}
