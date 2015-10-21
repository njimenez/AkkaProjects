using Akka.Actor;
using AkkaStats.Actors;
using AkkaStats.Messages;
using System.Linq;
using System.Windows;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace WordCounter
{
    /// <summary>
    /// Interaction logic for StatsWindow.xaml
    /// </summary>
    public partial class StatsWindow : Window
    {
        public StatsWindow()
        {
            InitializeComponent();
            DataContext = new StatsWindowViewModel();
        }

        private void Window_Loaded( object sender, RoutedEventArgs e )
        {
            ( DataContext as StatsWindowViewModel ).Initialize();
        }
    }

    public class StatsWindowViewModel : ReactiveObject
    {
        private readonly IActorRef m_vmActor;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatsWindowViewModel"/> class.
        /// </summary>
        public StatsWindowViewModel()
        {
            Items = new ReactiveList<StatViewModel>();

            // this is how we can update the viewmodel 
            // from the actor. 
            AddItem = new Subject<StatViewModel>();
            AddItem.ObserveOnDispatcher().Subscribe( item => Items.Add( item ) );

            m_vmActor = AkkaSystem.System.ActorOf( StatObserver.GetProps( this ), "stat-window" );
        }
        public void Initialize()
        {
            var publisher = AkkaSystem.System.ActorSelection( "/user/" + AkkaSystemMonitorActor.Name );
            // TODO: what to do if publisher not found.
            //var publisher = AkkaSystem.Publisher;
            publisher.Tell( new SubscribeMonitorMessage( m_vmActor ) );
            //   publisher.Tell( new PublishMetrics( "nelson", 10 ) );
        }

        public ReactiveList<StatViewModel> Items
        {
            get; set;
        }
        public Subject<StatViewModel> AddItem
        {
            get; set;
        }
    }

    public class StatObserver : ReceiveActor
    {
        private readonly StatsWindowViewModel m_Vm;

        public static Props GetProps( StatsWindowViewModel vm )
        {
            return Props.Create( () => new StatObserver( vm ) );
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="StatObserver"/> class.
        /// </summary>
        public StatObserver( StatsWindowViewModel vm )
        {
            m_Vm = vm;
            Receive<PublishMetrics>( msg => Handle( msg ) );
        }
        private void Handle( PublishMetrics msg )
        {
            var item = m_Vm.Items.FirstOrDefault( x => x.MetricName == msg.MetricName );
            if ( item == null )
            {
                m_Vm.AddItem.OnNext( new StatViewModel() { MetricName = msg.MetricName, Value = msg.Value } );
            }
            else
            {
                item.Value = msg.Value;
            }
        }
    }

    public class StatViewModel : ReactiveObject
    {
        // Fields...
        private long m_Value;
        private string m_MetricName;

        public string MetricName
        {
            get
            {
                return m_MetricName;
            }
            set
            {
                this.RaiseAndSetIfChanged( ref m_MetricName, value );
            }
        }

        public long Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                this.RaiseAndSetIfChanged( ref m_Value, value );
            }
        }


    }
}
