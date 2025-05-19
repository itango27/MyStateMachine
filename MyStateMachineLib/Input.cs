namespace MyStateMachineLib
{
    public class StateMachineInput
    {
        //public static readonly StateMachineInput Start = new StateMachineInput(1);
        //public static readonly StateMachineInput Finish = new StateMachineInput(2);

        public string Name { get; set; }
        public int Value { get; set; }

        public new string ToString()
        {
            //string result = null;
            //foreach (FieldInfo fi in this.GetType().GetFields())
            //{
            //    if (fi.FieldType == typeof(StateMachineInput))
            //    {
            //        dynamic fiVal = fi.GetValue(this);
            //        if (fiVal.Value == this.Value)
            //        {
            //            result = fi.Name;
            //            break;
            //        }
            //    }
            //}
            return Name;
        }

        public StateMachineInput()
        {
        }

        //public StateMachineInput(int value)
        //{
        //    this.Value = value;
        //}

    }
}
