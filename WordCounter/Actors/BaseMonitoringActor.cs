using Akka.Actor;
using Akka.Monitoring;

namespace WordCounter.Actors
{
    public class BaseMonitoringActor : ReceiveActor
    {
        protected override void PreStart()
        {
            Context.IncrementActorCreated();
            base.PreStart();
        }

        protected override void PostStop()
        {
            Context.IncrementActorStopped();
            base.PostStop();
        }

        public void IncrementMessagesReceived()
        {
            Context.IncrementMessagesReceived();
        }
    }
}
