using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;

namespace Adnc.Quest {
	public abstract class CreateDatabaseBase {
		// Implement this method in the sub-class where you want to call CreateDatabase
		// [MenuItem("Assets/Create/Quest Database")]
		public static void CreateDatabase (System.Type databaseType, string fileName) {
			UnityEngine.Object asset = (UnityEngine.Object)ScriptableObject.CreateInstance(databaseType);

			AssetDatabase.CreateAsset(asset, string.Format("{0}/{1}.asset", GetPath(), fileName));
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;        
		}
		
		public static string GetPath () {
			string path = "Assets";
			foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)) {
				path = AssetDatabase.GetAssetPath(obj);
				if (File.Exists(path)) {
					path = Path.GetDirectoryName(path);
				}
				break;
			}
			
			return path;
		}
	}
}

