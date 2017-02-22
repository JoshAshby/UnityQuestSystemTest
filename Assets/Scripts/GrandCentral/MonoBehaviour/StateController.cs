using UnityEngine;

namespace GrandCentral
{
    [Prefab("State Controller", true)]
    public class StateController : Singleton<StateController>
    {
        [SerializeField]
        private State _state = new State();
        public State State { get { return _state; } }

        private void Awake()
        {
            State.Add("global", new StateShard());
            State.Add("player", new StateShard());
        }
    }
}