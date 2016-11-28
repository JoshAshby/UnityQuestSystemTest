using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Adnc.Quest {
	public class QuestController : MonoBehaviour {
		[SerializeField] QuestDatabase defenitions;

		Dictionary<string, QuestEntryDetails> questLib = new Dictionary<string, QuestEntryDetails>();
		Dictionary<string, QuestTaskDetails> questTaskLib = new Dictionary<string, QuestTaskDetails>();

		public QuestConditions status;

		void Awake () {
			// Populate defenition dictionary with error checking
			foreach (QuestEntry quest in defenitions.Quests) {
				Debug.Assert(string.IsNullOrEmpty(quest.id), string.Format("QuestEntry {0} is missing an ID", quest.displayName));
				Debug.Assert(questLib.ContainsKey(quest.id), string.Format("Duplicate QuestEntry ID on {0}", quest.displayName));
				questLib[quest.id] = new QuestEntryDetails {
					defenition = quest
				};

				foreach (QuestTask task in quest.Tasks) {
					Debug.Assert(string.IsNullOrEmpty(task.id), string.Format("QuestTask {0} is missing an ID", task.displayName));
					Debug.Assert(questTaskLib.ContainsKey(task.id), string.Format("Duplicate QuestTask ID on {0}", task.displayName));
					questTaskLib[task.id] = new QuestTaskDetails {
						defenition = task
					};
				}
			}

			status = new QuestConditions {
				ctrl = this
			};
		}

		// @TODO Test and make sure sorting actually works how it should
		// Returns a list of all currently active quests
		public List<QuestEntryDetails> GetActive () {
			List<QuestEntryDetails> active = new List<QuestEntryDetails>();
			foreach (QuestEntry q in defenitions.Quests) {
				QuestEntryDetails quest = _GetQuest(q.id);
				if (quest.status != QuestStatus.Undefined) active.Add(quest);
			}

			// Sort by start date, priorty, ongoing items
			active.OrderByDescending(q => q.startTime)
				.OrderBy(q => q.defenition.sideQuest)
				.OrderByDescending(q => q.status == QuestStatus.Ongoing);

			return active;
		}

		public QuestEntryDetails _GetQuest (string id) {
			QuestEntryDetails quest;
			if (questLib.TryGetValue(id, out quest)) {
				return quest;
			} else {
				Debug.LogErrorFormat("Quest ID {0} does not exist. Please fix.", id);
				return null;
			}
		}

		public QuestTaskDetails _GetQuest (string id, string taskId) {
			QuestEntryDetails quest = _GetQuest(id);
			QuestTaskDetails task = _GetTask(id);

			if (quest == null || task == null) return null;

			if (quest.defenition.Tasks.Contains(task.defenition)) {
				return task;
			} else {
				Debug.LogErrorFormat("Quest ID {0} does not contain task ID {1}. Please fix.", id, taskId);
				return null;
			}
		}

		/// <summary>
		/// Gets the task. Kept private to prevent trying to access tasks individually and accidentally altering the
		/// wrong quest.
		/// </summary>
		/// <returns>The task.</returns>
		/// <param name="id">Identifier.</param>
		public QuestTaskDetails _GetTask (string id) {
			QuestTaskDetails task;
			if (questTaskLib.TryGetValue(id, out task)) {
				return task;
			} else {
				Debug.LogErrorFormat("Quest task ID {0} does not exist. Please fix.", id);
				return null;
			}
		}

		void SetQuest (string id, QuestStatus status) {
			QuestEntryDetails quest = _GetQuest(id);
			if (quest != null) {
				quest.status = status;
			}
		}

		void SetQuest (string id, string idTask, QuestStatus status) {
			QuestTaskDetails task = _GetQuest(id, idTask);
			if (task != null) {
				task.status = status;
			}
		}

		// User boots up and starts a quest
		public void BeginQuest (string id) {
			// Get the quest and first task, mark them as ongoing, set start timestamp
			QuestEntryDetails quest = _GetQuest(id);
			if (quest != null) {
				quest.status = QuestStatus.Ongoing;
				quest.startTime = System.DateTime.Now;
				if (quest.defenition.Tasks != null && quest.defenition.Tasks.Count > 0) {
					QuestTaskDetails task = _GetQuest(id, quest.defenition.Tasks[0].id);
					if (task != null) {
						// @TODO Poll the requirements of the task to check if they've already been completed
						task.status = QuestStatus.Ongoing;
						task.startTime = System.DateTime.Now;
					}
				}
			}
		}

		public void EndQuest (string id, bool success = true) {
			QuestEntryDetails quest = _GetQuest(id);
			if (quest == null) return;

			if (success) {
				quest.status = QuestStatus.Success;
				quest.defenition.Reward();
			} else {
				quest.status = QuestStatus.Failure;
			}
		}

		// @TODO Incomplete
		public bool NextTask (string questId) {
			if (status.IsQuestActive(questId)) {
				Debug.LogErrorFormat("Quest {0} is no longer active or not started.", questId);
				return false;
			}
			
			QuestEntryDetails quest = _GetQuest(questId);
			QuestTaskDetails task = GetCurrentTask(questId);

			// Checks if all requirements have been fullfilled on the current task
			if (task.defenition.CheckRequirements()) {
				task.status = QuestStatus.Success;

				if (quest.currentTaskIndex + 1 >= quest.defenition.Tasks.Count) {
					// Quest has been completed
					EndQuest(questId);
					Debug.LogWarning("Quest {0} was marked as success from calling NextTask(). Please use EndQuest() to mark a quest as complete.");
				} else {
					// Update the next task and quest index
					quest.currentTaskIndex += 1;
					quest.status = QuestStatus.Ongoing;
				}

				quest.currentTaskIndex += 1;
			} else {
				return false;
			}

			return true;
		}

		public QuestTaskDetails GetCurrentTask (string questId) {
			QuestEntryDetails quest = _GetQuest(questId);
			return _GetTask(quest.defenition.Tasks[quest.currentTaskIndex].id);
		}

		public QuestText GetQuestText (string questId) {
			QuestEntryDetails quest = _GetQuest(questId);
			if (quest == null) return null;
			
			QuestText questText = new QuestText {
				title = quest.Title,
				description = quest.defenition.description
			};
			
			if (quest.status == QuestStatus.Success) {
				questText.description += string.Format(" \n {0}", quest.defenition.successMessage);
			} else if (quest.status == QuestStatus.Failure) {
				questText.description += string.Format(" \n {0}", quest.defenition.failMessage);
			} else {
				// Generate a list of tasks
				List<QuestTaskText> tasks = new List<QuestTaskText>();
				foreach (QuestTask t in quest.defenition.Tasks) {
					QuestTaskDetails task = _GetTask(t.id);
					tasks.Add(new QuestTaskText {
						title = t.displayName,
						status = task.status
					});
				}

				questText.tasks = tasks;
			}

			return questText;
		}
	}
}

