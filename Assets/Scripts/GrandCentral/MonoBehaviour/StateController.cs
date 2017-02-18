namespace GrandCentral
{
    [Prefab("State Controller", true)]
    public class StateController : Singleton<StateController>
    {
        public State State { get; private set; }

        private void Awake()
        {
            State = new State();
        }
    }
}