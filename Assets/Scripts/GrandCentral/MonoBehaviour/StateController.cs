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
            StateShard shard = new StateShard();
            shard.Add("cylinders-seen", 0);
            shard.Add("seen-one-robin-01", false);

            State.Add("player", shard);
        }
    }
}