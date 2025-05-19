using HelperLib.Helpers;
using MyStateMachine.StateMachines;
using System.Reflection;
using System.Windows;

namespace MyStateMachine
{
    public partial class App : Application
    {
        public static CustomStateMachine CustomStateMachine { get; set; }

        public static Assembly Assembly;

        #region StartUp


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

			Assembly = AssemblyHelper.GetAssemblyInfo();

            ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            Exit += new ExitEventHandler(App_Exit);

            CustomStateMachine = new CustomStateMachine();
            CustomStateMachine.Run();

            Shutdown();
        }

        #endregion StartUp

        protected void App_Exit(object sender, ExitEventArgs e)
        {
			Logger.Log("Thanks for using CustomStateMachine", "CustomStateMachine", MessageBoxButton.OK);
			MessageBox.Show("Thanks for using CustomStateMachine", "CustomStateMachine", MessageBoxButton.OK);
		}
	}
}
