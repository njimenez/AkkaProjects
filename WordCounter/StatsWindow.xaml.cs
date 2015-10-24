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
    }

    public class StatsWindowViewModel : ReactiveObject
    {
        private ActorSelection m_publisher;
        private readonly IActorRef m_vmActor;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatsWindowViewModel"/> class.
        /// </summary>
        public StatsWindowViewModel()
        {
            Items = new ReactiveList<StatViewModel>();

            // this is how we can update the viewmodel 
            // from the actor. 
            AddItem = new Subject<PublishMetrics>();
            AddItem.ObserveOnDispatcher().Subscribe( item => HandleNewItemOnList( item ) );

            m_vmActor = AkkaSystem.System.ActorOf( StatObserver.GetProps( this ), "stat-window" );
            Publisher = AkkaSystem.System.ActorSelection( "/user/" + AkkaSystemMonitorActor.Name );

            // TODO: what to do if publisher not found.
            Publisher.Tell( new SubscribeMonitorMessage( m_vmActor ) );
        }
        public ReactiveList<StatViewModel> Items { get; set; }
        public Subject<PublishMetrics> AddItem { get; set; }
        private void HandleNewItemOnList( PublishMetrics msg )
        {
            var found = Items.FirstOrDefault( x => x.MetricName == msg.MetricName );
            if ( found == null )
            {
                Items.Add( new StatViewModel() { MetricName = msg.MetricName, Value = msg.Value } );
            }
            else
            {
                found.Value = msg.Value;
                msg = null;
            }
        }
        public ActorSelection Publisher
        {
            get
            {
                return m_publisher;
            }
            set
            {
                m_publisher = value;
            }
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
            m_Vm.AddItem.OnNext( msg );
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
