namespace GrandCentral
{
    [Prefab("State Controller", true)]
    public class StateController : Singleton<StateController>
    {
        public State State { get; private set; }

        private void Awake()
        {
            State = new State();

            StateShard shard = new StateShard();
            shard.Add("cylinders", new GenericData<int>(0));

            State.Add("test", shard);
        }
    }
}