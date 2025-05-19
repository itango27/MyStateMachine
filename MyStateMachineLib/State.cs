namespace MyStateMachineLib
{
    public class StateTest
    {
        public StateTest()
        {
            CompositeState root = new CompositeState("Picture");
            root.Add(new LeafState("Red Line"));
            root.Add(new LeafState("Blue Circle"));
            root.Add(new LeafState("Green Box"));

            CompositeState comp = new CompositeState("Two Circles");
            comp.Add(new LeafState("Black Circle"));
            comp.Add(new LeafState("White Circle"));

            root.Add(comp);

            LeafState pe = new LeafState("Yellow Line");
            root.Add(pe);

            root.Remove(pe);

            root.Display(1);

            Console.ReadKey();
        }
    }

    public enum StateEventType : int
    {
        Entry = 1,
        Exit
    }

    public class StateEventArgs : EventArgs
    {
        public StateEventType StateEventType;
    }

    public abstract class State
    {
        protected string _name;

        public dynamic Identifier { get; set; }

        public bool CanExit { get; set; }
        public string Name { get; set; }

        
        public State(string name)
        {
            this.Name = name;
            this.CanExit = true;
        }

		public State()
		{
		}

		public abstract void Add(State d);
        public abstract void Remove(State d);
        public abstract void Display(int indent);
        public abstract void OnEntry();
        public abstract void OnExit();
    }

    public class LeafState : State
    {
        public delegate void OnEntryEventHandler(object sender, StateEventArgs e);
        public event OnEntryEventHandler OnEntryEvent;

        public delegate void OnExitEventHandler(object sender, StateEventArgs e);
        public event OnExitEventHandler OnExitEvent;


        public LeafState(string name)
            : base(name)
        {
        }

        public LeafState()
            : base()
        {
        }

        public override void Add(State c)
        {
            Console.WriteLine("Cannot add to a LeafState");
        }

        public override void Remove(State c)
        {
            Console.WriteLine("Cannot remove from a LeafState");
        }

        public override void Display(int indent)
        {
            Console.WriteLine(new String('-', indent) + " " + _name);
        }

        public override void OnEntry()
        {
            OnEntryEvent(this, new StateEventArgs()
            {
                StateEventType = StateEventType.Entry
            });
        }

        public override void OnExit()
        {
            OnExitEvent(this, new StateEventArgs()
            {
                StateEventType = StateEventType.Exit
            });
        }
    }

    public class CompositeState : State
    {
        public delegate void OnEntryEventHandler(object sender, StateEventArgs e);
        public event OnEntryEventHandler OnEntryEvent;

        public delegate void OnExitEventHandler(object sender, StateEventArgs e);
        public event OnExitEventHandler OnExitEvent;


        private List<State> subStates = new List<State>();

        public CompositeState(string name)
            : base(name)
        {
        }

		public CompositeState() : base()
		{
		}

		public override void Add(State d)
        {
            subStates.Add(d);
        }

        public override void Remove(State d)
        {
            subStates.Remove(d);
        }

        public override void Display(int indent)
        {
            Console.WriteLine(new String('-', indent) + "+ " + _name);

            foreach (State d in subStates)
            {
                d.Display(indent + 2);
            }
        }

        public override void OnEntry()
        {
            OnEntryEvent(this, new StateEventArgs()
            {
                StateEventType = StateEventType.Entry
            });

            foreach (State d in subStates)
            {
                d.OnEntry();
            }
        }

        public override void OnExit()
        {
            foreach (State d in subStates)
            {
                d.OnExit();
            }

            OnExitEvent(this, new StateEventArgs()
            {
                StateEventType = StateEventType.Exit
            });
        }
    }
}
