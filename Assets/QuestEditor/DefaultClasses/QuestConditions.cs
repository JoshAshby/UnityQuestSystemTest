using UnityEngine;
using System.Collections;

namespace Adnc.Quest {
	public class QuestConditions {
		public QuestController ctrl;

		/// <summary>
		/// Verifies a quest is of a specific status
		/// </summary>
		/// <returns><c>true</c> if this instance is quest status the specified id status; otherwise, <c>false</c>.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="status">Status.</param>
		bool IsQuestStatus (string id, QuestStatus status) {
			QuestEntryDetails quest = ctrl._GetQuest(id) ;
			if (quest != null) {
				return quest.status == status;
			}
			
			return false;
		}

		/// <summary>
		/// Verifies a task is of a specific status
		/// </summary>
		/// <returns><c>true</c> if this instance is task status the specified id status; otherwise, <c>false</c>.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="status">Status.</param>
		bool IsTaskStatus (string id, QuestStatus status) {
			QuestTaskDetails task = ctrl._GetTask(id);
			if (task != null) {
				return task.status == status;
			}
			
			return false;
		}
		
		/// <summary>
		/// Is this quest currently active?
		/// </summary>
		/// <returns><c>true</c> if this instance is quest active the specified id; otherwise, <c>false</c>.</returns>
		/// <param name="id">Identifier.</param>
		public bool IsQuestActive (string id) {
			return IsQuestStatus(id, QuestStatus.Ongoing);
		}
		
		/// <summary>
		/// Is the quest and task currently active?
		/// </summary>
		/// <returns><c>true</c> if this instance is quest task active the specified id taskId; otherwise, <c>false</c>.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="taskId">Task identifier.</param>
		public bool IsQuestTaskActive (string id, string taskId) {
			if (IsQuestActive(id)) {
				return IsTaskStatus(taskId, QuestStatus.Ongoing);
			}
			
			return false;
		}
		
		/// <summary>
		/// Has the quest been started at some point in time
		/// </summary>
		/// <returns><c>true</c> if this instance is quest started the specified id; otherwise, <c>false</c>.</returns>
		/// <param name="id">Identifier.</param>
		public bool IsQuestStarted (string id) {
			return !IsQuestStatus(id, QuestStatus.Undefined);
		}
		
		/// <summary>
		/// Has the quest been started and the targeted task?
		/// </summary>
		/// <returns><c>true</c> if this instance is quest task started the specified id taskId; otherwise, <c>false</c>.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="taskId">Task identifier.</param>
		public bool IsQuestTaskStarted (string id, string taskId) {
			if (IsQuestStarted(id)) {
				return !IsTaskStatus(taskId, QuestStatus.Undefined);
			}
			
			return false;
		}
		
		/// <summary>
		/// Has the quest has been completed? Returns true on success or false.
		/// </summary>
		/// <returns><c>true</c> if this instance is quest completed the specified id; otherwise, <c>false</c>.</returns>
		/// <param name="id">Identifier.</param>
		public bool IsQuestComplete (string id) {
			return IsQuestStatus(id, QuestStatus.Success) || IsQuestStatus(id, QuestStatus.Failure);
		}

		/// <summary>
		/// Retursn true if the quest was a success
		/// </summary>
		/// <returns><c>true</c> if this instance is quest success the specified id; otherwise, <c>false</c>.</returns>
		/// <param name="id">Identifier.</param>
		public bool IsQuestSuccess (string id) {
			return IsQuestStatus(id, QuestStatus.Success);
		}

		/// <summary>
		/// Returns true if the quest was a failure
		/// </summary>
		/// <returns><c>true</c> if this instance is quest failure the specified id; otherwise, <c>false</c>.</returns>
		/// <param name="id">Identifier.</param>
		public bool IsQuestFailure (string id) {
			return IsQuestStatus(id, QuestStatus.Failure);
		}

		/// <summary>
		/// Checks if a quest task has been completed
		/// </summary>
		/// <returns><c>true</c> if this instance is quest task completed the specified id taskId; otherwise, <c>false</c>.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="taskId">Task identifier.</param>
		public bool IsQuestTaskCompleted (string id, string taskId) {
			if (IsQuestComplete(id)) {
				return true;
			} else if (IsQuestActive(id)) {
				return IsTaskStatus(taskId, QuestStatus.Failure) || IsTaskStatus(taskId, QuestStatus.Success);
			}
			
			return false;
		}
	}
}
