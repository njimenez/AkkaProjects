using Octokit;
using Octokit.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GithubActors_WPF.Factories
{

    /// <summary>
    /// Creates <see cref="GitHubClient"/> instances.
    /// </summary>
    public static class GithubClientFactory
    {
        /// <summary>
        /// OAuth token - necessary to generate authenticated requests
        /// and achieve non-terrible hourly API rate limit
        /// </summary>
        public static string OAuthToken { get; set; }

        public static GitHubClient GetUnauthenticatedClient()
        {
            return new GitHubClient( new ProductHeaderValue( "AkkaBootcamp-Unit3" ) );
        }

        public static GitHubClient GetClient()
        {
            return new GitHubClient( new ProductHeaderValue( "AkkaBootcamp-Unit3" ),
                                     new InMemoryCredentialStore( new Credentials( OAuthToken ) ) );
        }
    }
}
