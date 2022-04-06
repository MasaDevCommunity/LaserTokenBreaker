using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Editor
{
    public class EditorWindowSample : EditorWindow
    {
        private string _inputFilePath;
        private string _outputFilePath;

        [MenuItem("Editor/MapExport")]
        private static void Create()
        {
            GetWindow<EditorWindowSample>("Sample");
        }

        private void OnGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Input",GUILayout.Width(120));
                _inputFilePath = GUILayout.TextField(_inputFilePath);
            }
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Output",GUILayout.Width(120));
                _outputFilePath = GUILayout.TextField(_outputFilePath);
            }

            using (new GUILayout.VerticalScope())
            {
                if (GUILayout.Button("Button"))
                {
                    Export();
                }
            }
        }

        private void Export()
        {
            var mesh = new Mesh();
            AssetDatabase.CreateAsset(mesh,_outputFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}