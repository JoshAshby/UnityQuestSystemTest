using UnityEngine;
using System.Collections;

namespace Quest {
	public enum States {
		Unstarted,
		Started,
		Completed,
		Failed
	}

	public interface IQuestCondition {
		bool met ();
	}

	public interface IQuestStep {
		bool canAdvance ();
	}

	public interface IQuest {
		bool canStart ();
		bool canRestart ();

		void start ();
		void advance ();

		string name { get; set; }
		string description { get; set; }

		States state { get; set; }
	}

	public class AudioStep : IQuestStep {
		public bool canAdvance () {
			return true;
		}
	}

	public class MainQuest : IQuest {

		public string q_name = "Chapter 1";
		public string q_description = "Learn about Sleeping Trouble";

		public States q_state = States.Unstarted;

		public IQuestCondition[] q_startConditions = {};

		public IQuestStep[] q_steps = { new AudioStep() };
		public int q_activeStep = 0;

		public string name {
			get { return q_name; }
			set { q_name = value; }
		} 

		public string description {
			get { return q_description; }
			set { q_description = value; }
		}

		public States state {
			get { return q_state; }
			set { q_state = value; }
		}

		public bool canStart() {
			foreach (IQuestCondition condition in q_startConditions) {
				if (!condition.met ())
					return false;
			}

			return true;
		}

		public bool canRestart () {
			return false;
		}

		public void start () {
			q_state = States.Started;
		}

		public void advance () {
			if (q_state == States.Completed)
				return;
			
			IQuestStep step = q_steps [q_activeStep];

			if(step.canAdvance ())
				q_activeStep++;

			if (q_activeStep >= q_steps.Length) {
				q_state = States.Completed;
				return;
			}
		}
	}
}