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
    }
}