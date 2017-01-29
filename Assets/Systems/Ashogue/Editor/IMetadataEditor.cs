using UnityEditor;
using UnityEngine;
using Ashogue.Data;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Ashogue
{
    namespace Editor
    {
        public static class MetadataEditor
        {
            public static List<Type> AllSubTypes<T>()
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(T)))
                    .OrderBy(x => x.Name)
                    .ToList();
            }

            public static List<Type> AllImplementTypes<T>()
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => type.IsClass && !type.IsAbstract && type.GetInterfaces().Contains(typeof(T)))
                    .OrderBy(x => x.Name)
                    .ToList();
            }

            public static List<Type> metadataTypes = AllImplementTypes<IMetadata>();

            public static string ChoiceSelector(string[] options, string selected, Action<string> callback)
            {
                int choiceIndex = Array.IndexOf(options, selected);
                if (choiceIndex == -1)
                    choiceIndex = 0;

                EditorGUI.BeginChangeCheck();
                choiceIndex = EditorGUILayout.Popup(choiceIndex, options);
                if (EditorGUI.EndChangeCheck())
                {
                    string newSelection = options[choiceIndex];
                    callback(newSelection);
                    return newSelection;
                }

                return selected;
            }

            private static Dictionary<string, string> nodeTypeChoice = new Dictionary<string, string>();
            public static string TypeField(List<Type> options, string buttonText, Action<string> callback)
            {
                if (!options.Any())
                    return null;

                string[] optionNames = options.Select(x => x.Name).OrderBy(x => x).ToArray();
                if (!nodeTypeChoice.ContainsKey(buttonText))
                    nodeTypeChoice.Add(buttonText, optionNames.First());

                ChoiceSelector(
                    optionNames,
                    nodeTypeChoice[buttonText],
                    (newTypeName) => { nodeTypeChoice[buttonText] = newTypeName; }
                );

                if (GUILayout.Button(buttonText))
                    callback(nodeTypeChoice[buttonText]);

                return nodeTypeChoice[buttonText];
            }

            public static void Editor(Ashogue.Data.INode Target)
            {
                GUILayout.BeginHorizontal();
                TypeField(
                    metadataTypes,
                    "Add Metadata",
                    (newTypeName) => { Target.AddMetadata(metadataTypes.Find(type => type.Name == newTypeName)); }
                );
                GUILayout.EndHorizontal();

                IMetadata removalMetadata = null;

                foreach (IMetadata metadata in Target.Metadata.Values)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Key");
                    metadata.ID = EditorGUILayout.TextField(metadata.ID);

                    if (metadata.Type == typeof(bool))
                    {
                        IMetadata<bool> handle = metadata.OfType<bool>();
                        handle.Value = GUILayout.Toggle(handle.Value, "");
                    }
                    else if (metadata.Type == typeof(float))
                    {
                        IMetadata<float> handle = metadata.OfType<float>();
                        handle.Value = EditorGUILayout.FloatField(handle.Value);
                    }
                    else if (metadata.Type == typeof(string))
                    {
                        IMetadata<string> handle = metadata.OfType<string>();
                        handle.Value = EditorGUILayout.TextField(handle.Value);
                    }

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("X"))
                        if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to remove this metadata?", "Yup", "NO!"))
                            removalMetadata = metadata;

                    GUILayout.EndHorizontal();
                }

                if (removalMetadata != null)
                    Target.RemoveMetadata(removalMetadata.ID);
            }
        }
    }
}