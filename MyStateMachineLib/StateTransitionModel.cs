using HelperLib.Helpers;
using System.IO;
using System.Reflection;
using System.Xml;
using static System.Windows.Forms.AxHost;

namespace MyStateMachineLib
{
    public class StateTransitionModel
    {
		public string nameSpace;
		public State StartState { get; set; }

		public XmlNodeList stateNodes;
        public XmlNodeList inputNodes;
        public XmlNodeList transitionNodes;

        public Dictionary<string, State> states = new Dictionary<string, State>();
        public Dictionary<string, StateMachineInput> inputs = new Dictionary<string, StateMachineInput>();
        public Dictionary<string, Transition> transitions = new Dictionary<string ,Transition>();
        public StateTransitionModel()
        {
            ReadDataFromXml();
            DoStates(stateNodes);
            DoInputs(inputNodes);
            DoTransitions(transitionNodes);

            StartState = states["NotStarted"];
        }

        public void ReadDataFromXml()
        {
			TestData data = new TestData();
            bool bSel = FileHelper.SelectFile(data, "Xml");
            if (bSel)
            {
				XmlDocument doc = new XmlDocument();
				doc.Load(data.FileName);

				XmlElement xnModel = (XmlElement)doc.SelectSingleNode("/StateModels/StateModel");
				nameSpace = xnModel.GetAttribute("ID");

				stateNodes = xnModel.SelectNodes("descendant::State");
				inputNodes = xnModel.SelectNodes("descendant::Input");
				transitionNodes = xnModel.SelectNodes("descendant::Transition");
			}
		}

        void DoStates(XmlNodeList stateNodes)
        {
            int i = 0;
            AddState("NotStarted", i);

            for (i = 1; i <= stateNodes.Count; i++)
            {
                XmlElement element = stateNodes[i-1] as XmlElement;

                AddState(element.GetAttribute("ID"), i);
            }
            AddState("Complete", i );
        }

        void AddState(string stateName, int ident)
        {
            LeafState state = new LeafState()
            {
                CanExit = true,
                Identifier = ident,
                Name = stateName
            };

            state.OnEntryEvent += (object sender, StateEventArgs args) =>
            {
                Logger.Log($"State OnEntryEvent: {state.Name}");
            };

            state.OnExitEvent += (object sender, StateEventArgs args) =>
            {
				Logger.Log($"State OnExitEvent: {state.Name}");
			}; 
            states.Add(stateName, state);
        }

        void DoInputs(XmlNodeList inputNodes)
        {
            int i = 0;
            AddInput("Start", i);
            for (i = 1; i <= inputNodes.Count; i++)
            {
                XmlElement element = inputNodes[i-1] as XmlElement;

                AddInput(element.GetAttribute("ID"), i);
            }
            AddInput("Finish", i);
        }

        void AddInput(string inputName, int ident)
        {
            StateMachineInput input = new StateMachineInput()
            {
                Name = inputName,
                Value = ident
            };

            inputs.Add(inputName, input);
        }

        void DoTransitions(XmlNodeList transitionNodes)
        {
            if (stateNodes.Count > 0)
            {
                XmlElement firstElement = stateNodes[0] as XmlElement;

                AddTransition("NotStarted", firstElement.GetAttribute("ID"), "Start");

                for (int i = 0; i < transitionNodes.Count; i++)
                {
                    XmlElement element = transitionNodes[i] as XmlElement;

                    string fromState = element.GetAttribute("from");
                    string toState = element.GetAttribute("to");
                    string onInput = element.GetAttribute("on");

                    AddTransition(fromState, toState, onInput);
                }

                for (int i = 0; i < stateNodes.Count; i++)
                {
                    XmlElement element = stateNodes[i] as XmlElement;

                    AddTransition(element.GetAttribute("ID"), "Complete", "Finish");
                }
            }
            else
            {
                AddTransition("NotStarted", "Complete", "Finish");
            }
        }

        void AddTransition(string fromState, string toState, string inputName)
        {
            string transitionName = String.Format("{0}_{1}", fromState, inputName);

            Transition transition = new Transition()
            {
                FromState = states[fromState],
                ToState = states[toState],
                OnInput = inputs[inputName]

            };

            transition.OnStateTransitioningEvent += (object sender, TransitionEventArgs args) =>
            {
				Logger.Log($"Transition OnStateTransitioningEvent: from: {states[fromState].Name} to: {states[toState].Name} on input: {inputs[inputName].Name}");
			};

            transition.OnStateTransitionedEvent += (object sender, TransitionEventArgs args) =>
            {
				Logger.Log($"Transition OnStateTransitionedEvent: from: {states[fromState].Name} to: {states[toState].Name} on input: {inputs[inputName].Name}");
			};

            transitions.Add(transitionName, transition);
        }
    }
}
