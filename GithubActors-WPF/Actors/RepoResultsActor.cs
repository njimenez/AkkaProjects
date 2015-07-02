using Akka.Actor;
using GithubActors_WPF.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Reactive.Linq;

namespace GithubActors_WPF.Actors
{
    /// <summary>
    /// Actor responsible for printing the results and progress from a <see cref="GithubCoordinatorActor"/>
    /// onto a <see cref="RepoResultsForm"/> (runs on the UI thread)
    /// </summary>
    public class RepoResultsActor : ReceiveActor
    {
        private readonly RepoResultsViewModel vm;

        public RepoResultsActor( RepoResultsViewModel vm )
        {
            this.vm = vm;
            InitialReceives();
        }

        private void InitialReceives()
        {
            //progress update
            Receive<GithubProgressStats>( stats =>
            {
                //time to start animating the progress bar
                if ( vm.ProgressMax == 0 )
                {
                    vm.ProgressMax = stats.ExpectedUsers;
                    vm.ProgressValue = stats.UsersThusFar;
                }

                vm.Status = string.Format( "{0} out of {1} users ({2} failures) [{3} elapsed]",
                                stats.UsersThusFar, stats.ExpectedUsers, stats.QueryFailures, stats.Elapsed );
                vm.ProgressValue = stats.UsersThusFar + stats.QueryFailures;
            } );

            //user update
            Receive<IEnumerable<SimilarRepo>>( repos =>
            {
                var srepo = from seq in repos
                            select new RepoViewModel()
                            {
                                User = seq.Repo.Owner.Login,
                                Name = seq.Repo.Name,
                                Url = seq.Repo.HtmlUrl,
                                SharedStars = seq.SharedStarrers,
                                Subscribers = seq.Repo.SubscribersCount,
                                Stargazers = seq.Repo.StargazersCount,
                                ForksCount = seq.Repo.ForksCount
                            };

                var obs = srepo.ToObservable();
                obs.Subscribe( x => vm.Items.Add( x ) );
            } );

            //critical failure, like not being able to connect to Github
            Receive<GithubCoordinatorActor.JobFailed>( failed =>
            {
                vm.Status = string.Format( "Failed to gather data for Github repository {0} / {1}",
                    failed.Repo.Owner, failed.Repo.Repo );

                vm.Background = Colors.Red;
            } );
        }
    }
}
