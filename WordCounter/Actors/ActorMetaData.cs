namespace WordCounter.Actors
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
}
