using Akka.Actor;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GithubActors_WPF.Actors
{
    /// <summary>
    /// Individual actor responsible for querying the Github API
    /// </summary>
    public class GithubWorkerActor : ReceiveActor
    {
        #region Message classes

        public class QueryStarrers
        {
            public QueryStarrers( Messages.RepoKey key )
            {
                Key = key;
            }

            public Messages.RepoKey Key { get; private set; }
        }

        /// <summary>
        /// Query an individual starrer
        /// </summary>
        public class QueryStarrer
        {
            public QueryStarrer( string login )
            {
                Login = login;
            }

            public string Login { get; private set; }
        }

        public class StarredReposForUser
        {
            public StarredReposForUser( string login, IEnumerable<Repository> repos )
            {
                Repos = repos;
                Login = login;
            }

            public string Login { get; private set; }

            public IEnumerable<Repository> Repos { get; private set; }
        }

        #endregion

        private IGitHubClient _gitHubClient;
        private readonly Func<IGitHubClient> _gitHubClientFactory;

        public GithubWorkerActor( Func<IGitHubClient> gitHubClientFactory )
        {
            _gitHubClientFactory = gitHubClientFactory;
            InitialReceives();
        }

        protected override void PreStart()
        {
            _gitHubClient = _gitHubClientFactory();
        }

        private void InitialReceives()
        {
            //query an individual starrer
            Receive<Messages.RetryableQuery>( query => query.Query is QueryStarrer, query =>
            {
                // ReSharper disable once PossibleNullReferenceException (we know from the previous IS statement that this is not null)
                var starrer = ( query.Query as QueryStarrer ).Login;
                var sender = Sender;

                _gitHubClient.Activity.Starring.GetAllForUser( starrer )
                    .ContinueWith<object>( tr =>
                    {
                        if ( tr.IsFaulted || tr.IsCanceled )
                            return query.NextTry();
                        return new StarredReposForUser( starrer, tr.Result );
                    } ).PipeTo( sender );
            } );

            //query all starrers for a repository
            Receive<Messages.RetryableQuery>( query => query.Query is QueryStarrers, query =>
            {
                // ReSharper disable once PossibleNullReferenceException (we know from the previous IS statement that this is not null)
                var starrers = ( query.Query as QueryStarrers ).Key;
                // close over the sender in an instance variable
                var sender = Sender;
                _gitHubClient.Activity.Starring.GetAllStargazers( starrers.Owner, starrers.Repo )
                    .ContinueWith<object>( tr =>
                    {
                        if ( tr.IsFaulted || tr.IsCanceled )
                            return query.NextTry();
                        return tr.Result.ToArray();
                    } )
                    .PipeTo( sender );

            } );
        }
    }
}
