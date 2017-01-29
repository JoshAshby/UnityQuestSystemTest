using UnityEditor;
using UnityEngine;

namespace Ashode
{
    namespace Example
    {
        public class ExampleCanvas : NodeCanvas
        {
            public ExampleCanvas() : base() { }
        }

        public class AshodeEditor : EditorWindow
        {
            private static AshodeEditor _editor;
            public static AshodeEditor editor { get { AssureEditor(); return _editor; } }
            public static void AssureEditor() { if (_editor == null) OpenEditor(); }

            private ExampleCanvas Canvas;

            [MenuItem("Window/Ashode/Example Editor")]
            public static AshodeEditor OpenEditor()
            {
                _editor = EditorWindow.GetWindow<AshodeEditor>();
                _editor.titleContent = new GUIContent("Example Editor");

                return _editor;
            }

            private void Setup()
            {
                Canvas = new ExampleCanvas();
                Canvas.Repaint += Repaint;
            }

            private void OnDestroy()
            {
                Canvas.Repaint -= Repaint;
            }

            private void OnGUI()
            {
                if (Canvas == null)
                    Setup();

                Rect canvasRect = new Rect(0, 0, position.width, position.height);
                Canvas.Draw(canvasRect);
            }
        }
    }
}