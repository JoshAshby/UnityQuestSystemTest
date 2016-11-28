using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Adnc.Quest {
	public abstract class QuestWindowBase<D, Q> : EditorWindow {
		public D database;
		public List<Q> questTmp;

		public virtual string DisplayName { get { return "Quest Database"; } }

		string _noDbMessage;
		public virtual string NoDbMessage {
			get {
				if (string.IsNullOrEmpty(_noDbMessage)) {
					_noDbMessage = string.Format("Please select a {0} from the assets and click the edit " +
					                             "button in the inspector pannel (or create one if you haven't).", 
					                             DisplayName);
				}

				return _noDbMessage;
			}
		}

		public GUIStyle titleStyle; // Style used for title in upper left
		public GUIStyle errorFoldoutStyle;
		int paddingSize = 15; // Total padding wrapping the window
		GUIStyle containerPadding;

		Vector2 scrollPos;

		public virtual void OnEnable () {
			containerPadding = new GUIStyle();
			containerPadding.padding = new RectOffset(paddingSize, paddingSize, paddingSize, paddingSize);
			
			titleStyle = new GUIStyle();
			titleStyle.fontSize = 20;
		}

		public virtual void OnGUI () {
			if (errorFoldoutStyle == null) {
				errorFoldoutStyle = GetFoldoutStyle(Color.red);
			}

			EditorGUILayout.BeginVertical(containerPadding); // BEGIN Padding

			/***** BEGIN Header *****/
			if (database == null) {
				GUILayout.Label(DisplayName, titleStyle);
				GUILayout.Label(NoDbMessage);
			} else {
				Header();
			}

			EditorGUILayout.EndVertical(); // END Padding
			/***** END Header *****/

			scrollPos = GUILayout.BeginScrollView(scrollPos);
			EditorGUILayout.BeginVertical(containerPadding); // BEGIN Padding
			
			if (database != null) {
				DisplayDatabase();
			}
			
			EditorGUILayout.EndVertical(); // END Padding
			GUILayout.EndScrollView();
		}

		public abstract void Header ();

		// Display the current database
		public abstract void DisplayDatabase ();

		// Logic triggered when the selction changes, used to load in a database
		public abstract void OnSelectionChange ();

		public void BeginIndent (float indent) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(indent);
			EditorGUILayout.BeginVertical();
		}
		
		public void EndIndent () {
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}
		
		public static GUIStyle GetFoldoutStyle (Color myStyleColor) {
			GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
			myFoldoutStyle.normal.textColor = myStyleColor;
			myFoldoutStyle.onNormal.textColor = myStyleColor;
			myFoldoutStyle.hover.textColor = myStyleColor;
			myFoldoutStyle.onHover.textColor = myStyleColor;
			myFoldoutStyle.focused.textColor = myStyleColor;
			myFoldoutStyle.onFocused.textColor = myStyleColor;
			myFoldoutStyle.active.textColor = myStyleColor;
			myFoldoutStyle.onActive.textColor = myStyleColor;
			
			return myFoldoutStyle;
		}

		public bool ConfirmDelete (string itemName) {
			return EditorUtility.DisplayDialog("Delete Item", 
			                                   string.Format("Are you sure you want to delete '{0}'", itemName), 
			                                   string.Format("Delete '{0}'", itemName),
			                                   "Cancel"
			                                   );
		}
	}
}
