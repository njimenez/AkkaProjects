using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCounter.Actors
{

    /// <summary>
    /// Static helper class used to define paths to fixed-name actors
    /// (helps eliminate errors when using <see cref="ActorSelection"/>)
    /// </summary>
    public static class ActorPaths
    {
        public static readonly ActorMetaData WordCounterSupervisorActor = new ActorMetaData( "wordcountersupervisor", "akka://github-system/user/wordcountersupervisor" );
        public static readonly ActorMetaData FileValidator = new ActorMetaData( "validator", "akka://github-system/user/validator" );
        //    public static readonly ActorMetaData GithubValidatorActor = new ActorMetaData( "validator", "akka://github-system/user/validator" );
        //    public static readonly ActorMetaData GithubCommanderActor = new ActorMetaData( "commander", "akka://github-system/user/commander" );
        //    public static readonly ActorMetaData GithubCoordinatorActor = new ActorMetaData( "coordinator", "akka://github-system/user/commander/coordinator" );
    }
}