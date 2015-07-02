using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace GithubActors_WPF.Actors
{
    
    /// <summary>
    /// Actor that runs on the UI thread and handles
    /// UI events for <see cref="LauncherForm"/>
    /// </summary>
    public class MainFormActor : ReceiveActor, IWithUnboundedStash
    {
        #region Messages

        public class LaunchRepoResultsWindow
        {
            public LaunchRepoResultsWindow( Messages.RepoKey repo, IActorRef coordinator )
            {
                Repo = repo;
                Coordinator = coordinator;
            }

            public Messages.RepoKey Repo { get; private set; }

            public IActorRef Coordinator { get; private set; }
        }

        #endregion

        public IStash Stash { get; set; }

        private readonly LauncherFormViewModel m_vm;
        public MainFormActor( LauncherFormViewModel vm )
        {
            m_vm = vm;
            Ready();
        }

        /// <summary>
        /// State for when we're able to accept new jobs
        /// </summary>
        private void Ready()
        {
            Receive<Messages.ProcessRepo>( repo =>
            {
                var validator = Context.ActorSelection( ActorPaths.GithubValidatorActor.Path );
                validator.Tell( new GithubValidatorActor.ValidateRepo( repo.RepoUri ) );
                BecomeBusy( repo.RepoUri );
            } );


            // launch the window
            Receive<LaunchRepoResultsWindow>( window =>
            {
                m_vm.LaunchResults(window);

            } );
        }

        /// <summary>
        /// Make any necessary URI updates, then switch our state to busy
        /// </summary>
        private void BecomeBusy( string repoUrl )
        {
            m_vm.SetStatus( Colors.Gold, string.Format( "Validating {0}...", repoUrl ));
            Become( Busy );
        }

        /// <summary>
        /// State for when we're currently processing a job
        /// </summary>
        private void Busy()
        {
            Receive<GithubValidatorActor.RepoIsValid>( valid => BecomeReady( "Valid!" ) );
            Receive<GithubValidatorActor.InvalidRepo>( invalid => BecomeReady( invalid.Reason, false ) );
            //yes
            Receive<GithubCommanderActor.UnableToAcceptJob>( job => BecomeReady( string.Format( "{0}/{1} is a valid repo, but system can't accept additional jobs", job.Repo.Owner, job.Repo.Repo ), false ) );

            //no
            Receive<GithubCommanderActor.AbleToAcceptJob>( job => BecomeReady( string.Format( "{0}/{1} is a valid repo - starting job!", job.Repo.Owner, job.Repo.Repo ) ) );
            Receive<LaunchRepoResultsWindow>( window => Stash.Stash() );
        }

        private void BecomeReady( string message, bool isValid = true )
        {
            m_vm.SetStatus( isValid ? Colors.Green : Colors.Red, message );
            Stash.UnstashAll();
            Become( Ready );
        }
    }
}
