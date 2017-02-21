using UnityEngine;
using System.Collections;
using UnityEditor;

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
            shard.Add("cylinders-seen", new GenericValue<int>(0));

            State.Add("player", shard);
        }
    }

    [CustomEditor(typeof(StateController))]
    public class StateControllerEditor : Editor 
    {
        private StateController _target;

        public override void OnInspectorGUI()
        {
            _target = (StateController)target;

            foreach (var partition in _target.State)
            {
                EditorGUILayout.LabelField(partition.Key);
                EditorGUI.indentLevel++;
                foreach(var val in partition.Value)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(val.Key);
                    if(val.Value.Type == typeof(int))
                        ((GenericValue<int>)val.Value).Value = (object)EditorGUILayout.IntField(val.Value.OfType<int>().Value);
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
}