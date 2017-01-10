using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Ashode
{
    public class AshodeEditor : EditorWindow
    {
        private static AshodeEditor _editor;
        public static AshodeEditor editor { get { AssureEditor(); return _editor; } }
        public static void AssureEditor() { if (_editor == null) OpenEditor(); }

        private Texture2D _resizeHandle;

        private Canvas Canvas;

        [MenuItem("Window/Ashode/Example Editor")]
        public static AshodeEditor OpenEditor()
        {
            _editor = EditorWindow.GetWindow<AshodeEditor>();
            _editor.titleContent = new GUIContent("Example Editor");

            return _editor;
        }

        private void Setup()
        {
            _resizeHandle = AssetDatabase.LoadAssetAtPath("Assets/ResizeHandle.png", typeof(Texture2D)) as Texture2D;

            State state = new State
            {
                Nodes = new List<Node> {
                    new SimpleNode(),
                    new SimpleNode { Rect = new Rect(230, 30, 200, 100) }
                }
            };

            state.Repaints -= Repaint;
            state.Repaints += Repaint;

            InputSystem IS = new InputSystem(typeof(InputControls));

            Canvas = new Canvas { InputSystem = IS, State = state };
        }

        private void OnGUI()
        {
            if (Canvas == null)
                Setup();

            Canvas.Draw(GUILayoutUtility.GetRect (600, 600));
        }
    }
}