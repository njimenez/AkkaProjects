using Akka.Actor;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GithubActors_WPF.Actors
{
    /// <summary>
    /// Actor has one job - ensure that a public repo exists at the specified address
    /// </summary>
    public class GithubValidatorActor : ReceiveActor
    {
        #region Messages

        public class ValidateRepo
        {
            public ValidateRepo( string repoUri )
            {
                RepoUri = repoUri;
            }

            public string RepoUri { get; private set; }
        }

        public class InvalidRepo
        {
            public InvalidRepo( string repoUri, string reason )
            {
                Reason = reason;
                RepoUri = repoUri;
            }

            public string RepoUri { get; private set; }

            public string Reason { get; private set; }
        }

        /// <summary>
        /// System is unable to process additional repos at this time
        /// </summary>
        public class SystemBusy { }

        /// <summary>
        /// This is a valid repository
        /// </summary>
        public class RepoIsValid
        {
            /*
             * Using singleton pattern here since it's a stateless message.
             * 
             * Considered to be a good practice to eliminate unnecessary garbage collection,
             * and it's used internally inside Akka.NET for similar scenarios.
             */
            private RepoIsValid() { }
            private static readonly RepoIsValid _instance = new RepoIsValid();
            public static RepoIsValid Instance { get { return _instance; } }
        }

        #endregion

        private readonly IGitHubClient _gitHubClient;

        public GithubValidatorActor( IGitHubClient gitHubClient )
        {
            _gitHubClient = gitHubClient;
            ReadyToValidate();
        }

        private void ReadyToValidate()
        {
            //Outright invalid URLs
            Receive<GithubValidatorActor.ValidateRepo>(
                repo => IsUriCorrect( repo ),
                repo => SendInvalidRepo( Sender, new InvalidRepo( repo.RepoUri, "Not a valid absolute URI" ) )
            );

            //Repos that at least have a valid absolute URL
            Receive<GithubValidatorActor.ValidateRepo>( repo =>
            {
                var userOwner = SplitIntoOwnerAndRepo( repo.RepoUri );

                //close over the sender in an instance variable
                var sender = Sender;
                _gitHubClient.Repository.Get( userOwner.Item1, userOwner.Item2 ).ContinueWith<object>( t =>
                {
                    //Rule #1 of async in Akka.NET - turn exceptions into messages your actor understands
                    if ( t.IsCanceled )
                    {
                        return new InvalidRepo( repo.RepoUri, "Repo lookup timed out" );
                    }
                    if ( t.IsFaulted )
                    {
                        return new InvalidRepo( repo.RepoUri, t.Exception != null ? t.Exception.GetBaseException().Message : "Unknown Octokit error" );
                    }

                    return t.Result;
                } ).PipeTo( Self, sender );
            } );

            // something went wrong while querying github, sent to ourselves via PipeTo
            // however - Sender gets preserved on the call, so it's safe to use Forward here.
            Receive<InvalidRepo>( repo => ForwardTo( Sender, repo ) );

            // Octokit was able to retrieve this repository
            Receive<Repository>( repository =>
            {
                //ask the GithubCommander if we can accept this job
                var commander = Context.ActorSelection( ActorPaths.GithubCommanderActor.Path );
                commander.Tell( new GithubCommanderActor.CanAcceptJob( new Messages.RepoKey( repository.Owner.Login, repository.Name ) ) );
            } );


            /* REPO is valid, but can we process it at this time? */

            //yes
            Receive<GithubCommanderActor.UnableToAcceptJob>( job => SendUnableToAcceptJob( Context.ActorSelection( ActorPaths.MainFormActor.Path ),job.Repo ) );

            //no
            Receive<GithubCommanderActor.AbleToAcceptJob>( job => SendAbleToAcceptJob( Context.ActorSelection( ActorPaths.MainFormActor.Path ), job.Repo ) );
        }

        public static Tuple<string, string> SplitIntoOwnerAndRepo( string repoUri )
        {
            var split = new Uri( repoUri, UriKind.Absolute ).PathAndQuery.TrimEnd( '/' ).Split( '/' ).Reverse().ToList(); //uri path without trailing slash
            return Tuple.Create( split[ 1 ], split[ 0 ] ); //User, Repo
        }

        private static bool IsUriCorrect( ValidateRepo repo )
        {
            return string.IsNullOrEmpty( repo.RepoUri )
                || !Uri.IsWellFormedUriString( repo.RepoUri, UriKind.Absolute );
        }

        private void SendInvalidRepo( IActorRef sender, GithubValidatorActor.InvalidRepo invalidRepo )
        {
            sender.Tell( invalidRepo );
        }
        private void ForwardTo( IActorRef sender, GithubValidatorActor.InvalidRepo repo )
        {
            sender.Forward( repo );
        }
        private void SendAbleToAcceptJob( ActorSelection actor, Messages.RepoKey acceptedJob )
        {
            actor.Tell( new GithubCommanderActor.AbleToAcceptJob( acceptedJob ) );
        }
        private void SendUnableToAcceptJob( ActorSelection actor, Messages.RepoKey job )
        {
            actor.Tell( new GithubCommanderActor.UnableToAcceptJob( job ) );
        }

    }
}
