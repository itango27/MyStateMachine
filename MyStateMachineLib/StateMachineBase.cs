using HelperLib.Helpers;
using Microsoft.VisualBasic.ApplicationServices;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace MyStateMachineLib
{
    public class StateMachineBase
    {
        public virtual State CurrentState { get; set; }

		private SessionSnapshot? _session;

		public delegate void OnEntryDelegate();
        public OnEntryDelegate OnEntry;

        public delegate void OnProcessStateDelegate();
        public OnProcessStateDelegate OnProcessState;

        public delegate void OnExitDelegate();
        public OnExitDelegate OnExit;

        public StateTransitionModel Model;
		private string userID;
		private string email;

		public StateMachineBase()
        {
            Model = new StateTransitionModel();

            SetStartState();
        }

        public void Run()
        {
            Entry();
            ProcessStates();
            Exit();
        }


        private void Entry()
        {
            if (OnEntry != null) OnEntry();
        }

        private void ProcessStates()
        {
            ProcessInput(Model.inputs[Model.inputs.Keys.First()]);

            do
            {
                if (OnProcessState != null) OnProcessState();

            } while (CurrentState.Identifier != Model.states["Complete"].Identifier);

            Logger.Log("State machine ended", "RunStateMachine");
        }

        private void Exit()
        {
            if (OnExit != null) OnExit();
        }

        public virtual void ProcessInput(StateMachineInput newInput)
        {
            foreach (string key in Model.transitions.Keys)
            {
                if (key.StartsWith(CurrentState.Name) && key.EndsWith(newInput.ToString()))
                {
                    Transition transition = Model.transitions[key];

                    State newState = transition.DoTransition(newInput);
                    CurrentState = newState;

                    // How do I know if the user bailed out early?
                    if (newState.Name != "Complete") 
                    {
						_session.CurrentStateName = CurrentState.Name;
						_session.LastSaved = DateTime.UtcNow;
						SessionSnapshot.Save(_session);
						Logger.Log($"State saved: {CurrentState.Name}");
						MessageBox.Show($"State saved: {CurrentState.Name}");
					}

					break;
                }
            }
        }

        public virtual State GetNextState(StateMachineInput newInput)
        {
            State newState = default(State);

            foreach (string key in Model.transitions.Keys)
            {
                if (key.StartsWith(CurrentState.Identifier.ToString()) && key.EndsWith(newInput.ToString()))
                {
                    newState = Model.transitions[key].ToState;
                    break;
                }
            }
            return newState;
        }
		protected virtual void SetStartState()
		{
            CurrentState = Model.states[Model.states.Keys.First()];


			// Load previous session if available
			_session = SessionSnapshot.Load(userID) ?? new SessionSnapshot
			{
				//UserID = userID,
				//Email = email,
				//StateModelID = Model.ID,
				CurrentStateName = Model.StartState.Name,
				//ShoppingCart = new List<string>()
			};

			// Restore state from session
			if (Model.states.ContainsKey(_session.CurrentStateName))
			{
				CurrentState = Model.states[_session.CurrentStateName];
			}
			else
			{
				CurrentState = Model.states[Model.StartState.Name];
			}

			MessageBox.Show($"User {userID} resumed at state: {CurrentState.Name}");
		}
	}
}
