using Akka.Actor;
using GithubActors_WPF.Factories;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace GithubActors_WPF.Actors
{
    public class GithubAuthenticationActor : ReceiveActor
    {
        #region Messages

        public class Authenticate
        {
            public Authenticate( string oAuthToken )
            {
                OAuthToken = oAuthToken;
            }

            public string OAuthToken { get; private set; }
        }

        public class AuthenticationFailed { }

        public class AuthenticationCancelled { }

        public class AuthenticationSuccess { }

        #endregion

        private readonly GithubAuthViewModel m_vm;

        /// <summary>
        /// Initializes a new instance of the <see cref="GithubAuthenticationActor"/> class.
        /// </summary>
        public GithubAuthenticationActor( GithubAuthViewModel vm )
        {
            m_vm = vm;
            UnAuthenticated();
        }

        private void UnAuthenticated()
        {
            Receive<Authenticate>( auth =>
            {
                //need a client to test our credentials with
                var client = GithubClientFactory.GetUnauthenticatedClient();
                GithubClientFactory.OAuthToken = auth.OAuthToken;
                client.Credentials = new Credentials( auth.OAuthToken );
                BecomeAuthenticating();

                client.User.Current().ContinueWith<object>( tr =>
                {
                    if ( tr.IsFaulted )
                        return new AuthenticationFailed();
                    if ( tr.IsCanceled )
                        return new AuthenticationCancelled();
                    return new AuthenticationSuccess();
                } ).PipeTo( Self );
            } );
        }

        private void BecomeAuthenticating()
        {
            m_vm.StatusLabelForeColor = Colors.Yellow;
            m_vm.Status = "Authenticating...";
            Become( Authenticating );
        }

        private void Authenticating()
        {
            Receive<AuthenticationFailed>( failed => BecomeUnauthenticated( "Authentication failed." ) );
            Receive<AuthenticationCancelled>( cancelled => BecomeUnauthenticated( "Authentication timed out." ) );
            Receive<AuthenticationSuccess>( success =>
            {
                m_vm.Authenticated();
            } );
        }

        private void BecomeUnauthenticated( string reason )
        {
            m_vm.StatusLabelForeColor = Colors.Red;
            m_vm.Status = "Authentication failed. Please try again.";
            Become( UnAuthenticated );
        }
    }
}
