using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Adnc.Quest {
	public class QuestWindow : QuestWindowBase<QuestDatabase, QuestEntry> {
		string filter;
		int deleteIndex = -1;
		float textAreaWidth = 250f;

		// Properties for moving a quest task
		int taskQuestIndex;
		int taskIndex;
		TaskAction taskAction;
		enum TaskAction {
			Undefined,
			Delete,
			MoveUp,
			MoveDown
		}

		// Loop vars
		QuestEntry quest;
		QuestTask task;
		bool errorId;

		[MenuItem("Window/Quest Editor")]
		public static void ShowEditor () {
			EditorWindow.GetWindow<QuestWindow>("Quest Editor");
		}

		public override void Header () {
			EditorGUILayout.BeginHorizontal();
			
			EditorGUI.BeginChangeCheck();
			
			GUILayout.Label(string.Format("{0}: {1}", DisplayName, database.title), titleStyle);
			
			GUI.SetNextControlName("Filter");
			if (GUI.GetNameOfFocusedControl() == "Filter") {
				filter = EditorGUILayout.TextField(filter);
			} else {
				EditorGUILayout.TextField("Filter");
			}
			
			if (EditorGUI.EndChangeCheck()) {
				FilterDecisions(filter);
			}
			
			EditorGUILayout.EndHorizontal();
			
			if (GUILayout.Button("Add Quest")) AddQuest();
		}


		public override void DisplayDatabase () {
			EditorGUI.BeginChangeCheck();

			for (int i = 0, l = questTmp.Count; i < l; i++) {
				quest = questTmp[i];
				
				errorId = string.IsNullOrEmpty(quest.id);

				if (!errorId) {
					quest.expanded = EditorGUILayout.Foldout(quest.expanded, quest.displayName);
				} else {
					quest.expanded = EditorGUILayout.Foldout(quest.expanded, 
                                     	string.Format("{0}: ID cannot be left blank and must be unique", quest.displayName), 
                                    	errorFoldoutStyle);
				}
				
				if (quest.expanded) {
					EditorGUILayout.BeginVertical(EditorStyles.helpBox); // BEGIN Wrapper
					
					quest.displayName = EditorGUILayout.TextField("Display Name", quest.displayName);
					quest.id = EditorGUILayout.TextField("ID", quest.id);
					quest.sideQuest = EditorGUILayout.Toggle("Sidequest?", quest.sideQuest);
					
					/***** BEGIN Description *****/
					EditorGUILayout.BeginHorizontal();

					EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField("Description");
					quest.description = GUILayout.TextArea(quest.description, GUILayout.MaxHeight(60f), GUILayout.MaxWidth(textAreaWidth));
					EditorGUILayout.EndVertical();

					EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField("Notes");
					quest.notes = GUILayout.TextArea(quest.notes, GUILayout.MaxHeight(60f), GUILayout.MaxWidth(textAreaWidth));
					EditorGUILayout.EndVertical();

					EditorGUILayout.EndHorizontal();
					/***** END Description *****/

					/***** BEGIN Messages *****/
					EditorGUILayout.BeginHorizontal();
					
					EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField("Success Message");
					quest.successMessage = GUILayout.TextArea(quest.successMessage, GUILayout.MaxHeight(60f), GUILayout.MaxWidth(textAreaWidth));
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField("Fail Message");
					quest.failMessage = GUILayout.TextArea(quest.failMessage, GUILayout.MaxHeight(60f), GUILayout.MaxWidth(textAreaWidth));
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.EndHorizontal();
					/***** END Messages *****/

					/***** BEGIN Tasks *****/
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Tasks");
					if (GUILayout.Button("Add Task")) AddTask(quest);
					EditorGUILayout.EndHorizontal();

					for (int j = 0, jl = quest.Tasks.Count; j < jl; j++) {
						EditorGUILayout.BeginVertical(EditorStyles.helpBox); // BEGIN Wrapper
						task = questTmp[i].Tasks[j];

						EditorGUILayout.LabelField(string.Format("{0}. {1}", j + 1, task.displayName), EditorStyles.boldLabel);
						task.displayName = EditorGUILayout.TextField("Display Text", task.displayName);
						task.id = EditorGUILayout.TextField("ID", task.id);

						EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(200f)); // BEGIN Meta

						if (GUILayout.Button("Up")) MoveTaskUp(i, j);
						if (GUILayout.Button("Down")) MoveTaskDown(i, j);
						if (GUILayout.Button("Delete") && ConfirmDelete("Delete Task", task.displayName)) RemoveTask(i, j);
						
						EditorGUILayout.EndHorizontal(); // END Meta

						EditorGUILayout.EndVertical(); // END Wrapper
					}

					if (taskAction != TaskAction.Undefined) UpdateTasks();
					/***** END Tasks *****/

					if (GUILayout.Button(string.Format("Remove {0}", quest.displayName)) && ConfirmDelete("Delete Quest", quest.displayName)) {
						deleteIndex = i;
					}

					EditorGUILayout.EndVertical(); // END Wrapper
				}
			}

			if (EditorGUI.EndChangeCheck()) {
				EditorUtility.SetDirty(database);
			}
			
			if (deleteIndex != -1) {
				RemoveQuest(deleteIndex);
				deleteIndex = -1;
			}
		}

		bool ConfirmDelete (string title, string itemName) {
			return EditorUtility.DisplayDialog(title, 
			                                   string.Format("Are you sure you want to delete '{0}'", itemName), 
			                                   string.Format("Delete '{0}'", itemName),
			                                   "Cancel"
			                                   );
		}

		void AddTask (QuestEntry quest) {
			QuestTask task = new QuestTask {
				id = System.Guid.NewGuid().ToString()
			};
			quest.Tasks.Add(task);

			EditorUtility.SetDirty(database);
		}

		void RemoveTask (int taskQuestIndex, int taskIndex) {
			this.taskQuestIndex = taskQuestIndex;
			this.taskIndex = taskIndex;
			taskAction = TaskAction.Delete;
		}

		void MoveTaskUp (int taskQuestIndex, int taskIndex) {
			this.taskQuestIndex = taskQuestIndex;
			this.taskIndex = taskIndex;
			taskAction = TaskAction.MoveUp;
		}

		void MoveTaskDown (int taskQuestIndex, int taskIndex) {
			this.taskQuestIndex = taskQuestIndex;
			this.taskIndex = taskIndex;
			taskAction = TaskAction.MoveDown;
		}

		void UpdateTasks () {
			List<QuestTask> tasks = questTmp[taskQuestIndex].Tasks;

			if (taskAction == TaskAction.Delete) {
				tasks.RemoveAt(taskIndex);
			} else if (taskAction == TaskAction.MoveUp) {
				ShiftTask(tasks, taskIndex, taskIndex - 1);
			} else if (taskAction == TaskAction.MoveDown) {
				ShiftTask(tasks, taskIndex, taskIndex + 1);
			}

			taskAction = TaskAction.Undefined;

			EditorUtility.SetDirty(database);
		}

		void ShiftTask (List<QuestTask> tasks, int oldIndex, int newIndex) {
			QuestTask task = tasks[oldIndex];
			tasks.RemoveAt(oldIndex);
			newIndex = Mathf.Clamp(newIndex, 0, tasks.Count);
			tasks.Insert(newIndex, task);
		}

		void AddQuest () {
			database.Quests.Insert(0, new QuestEntry());
			
			FilterDecisions(filter);
			EditorUtility.SetDirty(database);
		}

		void RemoveQuest (int index) {
			database.Quests.RemoveAt(index);
			FilterDecisions(filter);
			EditorUtility.SetDirty(database);
		}
		
		void FilterDecisions (string search) {
			if (string.IsNullOrEmpty(search)) {
				questTmp = database.Quests;
				return;
			}
			
			string[] searchBits = search.ToLower().Split(' ');
			List<QuestEntry> matches = database.Quests.Where(d => searchBits.All(n => d.displayName.ToLower().Contains(n))).ToList();
			
			questTmp = matches;
		}

		public override void OnSelectionChange () {
			Object current = Selection.activeObject;
			if (current != null && current.GetType() == typeof(QuestDatabase)) {
				SetDatabase(Selection.activeObject as QuestDatabase);
			}
		}

		public void SetDatabase (QuestDatabase db) {
			database = db;
			questTmp = db.Quests;
		}
	}
}
