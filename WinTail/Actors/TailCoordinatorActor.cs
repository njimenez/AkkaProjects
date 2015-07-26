using Akka.Actor;
using System;
using WinTail.ViewModels;

namespace WinTail.Actors
{
    public class TailCoordinatorActor : ReceiveActor
    {
        #region Message types
        /// <summary>
        /// Start tailing the file at user-specified path.
        /// </summary>
        public class StartTail
        {
            public StartTail( string filePath, IObserveViewModel vm )
            {
                FilePath = filePath;
                ViewModel = vm;
            }

            public string FilePath
            { get; private set; }

            public IObserveViewModel ViewModel
            { get; private set; }
        }

        /// <summary>
        /// Stop tailing the file at user-specified path.
        /// </summary>
        public class StopTail
        {
            public StopTail( string filePath )
            {
                FilePath = filePath;
            }

            public string FilePath
            { get; private set; }
        }

        #endregion

        public static Props GetProps()
        {
            return Props.Create( () => new TailCoordinatorActor() );
        }

        /// <summary>
        /// Initializes a new instance of the TailCoordinatorActor class.
        /// </summary>
        public TailCoordinatorActor()
        {
            Ready();
        }

        protected void Ready()
        {
            Receive<StartTail>( msg => Handle( msg ) );
            Receive<StopTail>( msg => Handle( msg ) );
        }

        private void Handle( TailCoordinatorActor.StartTail msg )
        {
            Context.ActorOf( Props.Create( () => new TailActor( msg.ViewModel, msg.FilePath ) ) );
        }

        private void Handle( TailCoordinatorActor.StopTail msg )
        {
            foreach ( var child in Context.GetChildren() )
            {
                child.Tell( msg );
            }
        }

        // here we are overriding the default SupervisorStrategy
        // which is a One-For-One strategy w/ a Restart directive
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                10, // maxNumberOfRetries
                TimeSpan.FromSeconds( 30 ), // duration
                x =>
                {
                    //Maybe we consider ArithmeticException to not be application critical
                    //so we just ignore the error and keep going.
                    if ( x is ArithmeticException )
                        return Directive.Resume;

                    //Error that we cannot recover from, stop the failing actor
                    else if ( x is NotSupportedException )
                        return Directive.Stop;

                    //In all other cases, just restart the failing actor
                    else
                        return Directive.Restart;
                } );
        }
    }

}
