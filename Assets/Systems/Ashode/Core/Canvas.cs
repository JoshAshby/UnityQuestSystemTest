using UnityEngine;

namespace Ashode
{
    public class Canvas
    {
        public State State;
        public InputSystem InputSystem;

        public virtual void OnGUI() { }

        public void Draw(Rect canvasRect)
        {
            InputSystem.HandleEvents(State, false);

            DrawConnections();
            DrawNodes();

            OnGUI();

            InputSystem.HandleEvents(State, true);
        }

        private void DrawConnections() { }

        private void DrawNodes()
        {
            for (int i = 0; i < State.Nodes.Count; i++)
            {
                State.Nodes[i].DrawNodeWindow(State);
            }
        }
    }
}