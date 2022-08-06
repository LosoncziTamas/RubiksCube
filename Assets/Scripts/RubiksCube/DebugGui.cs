using System.Collections;
using System.Text;
using UnityEngine;

namespace RubiksCube
{
    public class DebugGui : MonoBehaviour
    {
        private static readonly Rect WindowRect = new(0, 0, Screen.width, Screen.height);

        public static DebugGui Instance;
        
        private readonly StringBuilder _stringBuilder = new();
        private readonly GUIStyle _guiStyle = GUIStyle.none;
        private readonly WaitForEndOfFrame _waitForEndOfFrame = new();

        private string _currentContent;
        private Rect _windowRect = new(20, 20, 400, 300);

        private void Awake()
        {
            Instance = this;
            _guiStyle.fontSize = 30;
            _guiStyle.normal.textColor = Color.white;
            StartCoroutine(Loop());
        }

        public void Print(string text)
        {
            _stringBuilder.Append(text);
            _stringBuilder.Append("\n");
        }
        
        public void Print(params object[] values)
        {
            foreach (var obj in values)
            {
                _stringBuilder.Append(obj);
                _stringBuilder.Append(" ");
            }
            _stringBuilder.Append("\n");
        }

        private void OnGUI()
        {
            _windowRect = GUILayout.Window(0, _windowRect, RenderDebugWindow, "Debug GUI");
        }
        
        private void RenderDebugWindow(int id)
        {
            GUI.DragWindow(WindowRect);
            GUILayout.Label(_currentContent, _guiStyle);
        }

        private IEnumerator Loop()
        {
            while (true)
            {
                yield return _waitForEndOfFrame;
                if (_stringBuilder.Length > 0)
                {
                    _currentContent = _stringBuilder.ToString();
                    _stringBuilder.Clear();
                }
            }
        }
    }
}