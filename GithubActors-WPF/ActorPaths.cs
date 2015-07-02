using System;
using System.Collections.Generic;
using System.Linq;

namespace GithubActors_WPF
{
    /// <summary>
    /// Meta-data class
    /// </summary>
    public class ActorMetaData
    {
        public ActorMetaData( string name, string path )
        {
            Name = name;
            Path = path;
        }

        public string Name { get; private set; }
        public string Path { get; private set; }
    }

    /// <summary>
    /// Static helper class used to define paths to fixed-name actors
    /// (helps eliminate errors when using <see cref="ActorSelection"/>)
    /// </summary>
    public static class ActorPaths
    {
        public static readonly ActorMetaData GithubAuthenticatorActor = new ActorMetaData( "authenticator", "akka://github-system/user/authenticator" );
        public static readonly ActorMetaData MainFormActor = new ActorMetaData( "mainform", "akka://github-system/user/mainform" );
        public static readonly ActorMetaData GithubValidatorActor = new ActorMetaData( "validator", "akka://github-system/user/validator" );
        public static readonly ActorMetaData GithubCommanderActor = new ActorMetaData( "commander", "akka://github-system/user/commander" );
        public static readonly ActorMetaData GithubCoordinatorActor = new ActorMetaData( "coordinator", "akka://github-system/user/commander/coordinator" );
    }
}
