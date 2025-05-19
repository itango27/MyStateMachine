namespace MyStateMachineLib
{
    public class TransitionEventArgs : EventArgs
    {
        public TransitionEventType TransitionEventType;
        public StateMachineInput Input;
        public State FromState { get; set; }
        public State ToState { get; set; }
    }

    public class TransitionEventType
    {
        public static readonly TransitionEventType Transitioning = new TransitionEventType(1);
        public static readonly TransitionEventType Transitioned = new TransitionEventType(2);

        public override string ToString()
        {
            return Value.ToString();
        }

        protected TransitionEventType(int value)
        {
            this.Value = value;
        }

        public int Value { get; private set; }
    }

    public class Transition
    {
        public State FromState { get; set; }
        public State ToState { get; set; }
        public StateMachineInput OnInput { get; set; }

        public delegate void OnStateTransitioningEventHandler(object sender, TransitionEventArgs e);
        public delegate void OnStateTransitionedEventHandler(object sender, TransitionEventArgs e);

        public event OnStateTransitioningEventHandler OnStateTransitioningEvent;
        public event OnStateTransitionedEventHandler OnStateTransitionedEvent;

        public State DoTransition(StateMachineInput newInput)
        {
            State resultState = this.FromState;

			Func<bool> guardEntry = () =>
			{
				return true;
			};

			Func<bool> guardExit = () =>
			{
				return FromState.CanExit;
			};

			if (Guard(guardExit))
            {
                FromState.OnExit();

                OnStateTransitioningEvent(this, new TransitionEventArgs()
                {
                    TransitionEventType = TransitionEventType.Transitioning,
                    Input = newInput,
                    FromState = this.FromState,
                    ToState = this.ToState
                });

                ToState.OnEntry();

                OnStateTransitionedEvent(this, new TransitionEventArgs()
                {
                    TransitionEventType = TransitionEventType.Transitioned,
                    Input = newInput,
                    FromState = this.FromState,
                    ToState = this.ToState
                });

                resultState = this.ToState;
            }
            return resultState;
        }

        public bool Guard(Func<bool> action)
        {
            bool bResult = false;

            if (action.Invoke())
            {
                bResult = true;
            }
            return bResult;
        }
    }
}
