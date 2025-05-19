using HelperLib.Commands;
using HelperLib.Controllers;
using HelperLib.Helpers;
using HelperLib.ViewModels;
using HelperLib.Views;
using MyStateMachineLib;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using static System.Windows.Forms.AxHost;

namespace MyStateMachine.StateMachines
{
	public class CustomStateMachine : StateMachineBase
	{
        #region Properties

        public MainWindow MainWindow { get; set; }
        public bool CanExit { get; set; }
        bool? bResult { get; set; }
        public Controller Controller { get; set; }
        dynamic genViewModel { get; set; }

        string newCommand;

        #endregion Properties

        public CustomStateMachine() : base()
		{
            Controller = new Controller();
            Controller.Initialize(AssemblyData.EntryAssembly.GetName().Name, false);
            InitializeStateMachine();
        }
        #region State Machine

        public void InitializeStateMachine()
        {

            OnEntry = () => CustomStateMachine_OnEntry();
            OnProcessState = () => CustomStateMachine_OnProcessState();
            OnExit = () => CustomStateMachine_OnExit();

            foreach (LeafState state in Model.states.Values)
            {
                state.OnEntryEvent += (object sender, StateEventArgs args) =>
                {
                    (sender as LeafState).CanExit = true;
                    Logger.Log(String.Format("\r\nState {0}: {1}\r\n", (sender as LeafState).Name, args.StateEventType));
                };

                state.OnExitEvent += (object sender, StateEventArgs args) =>
                {
                    if ((sender as LeafState).Name != "NotStarted" && (sender as LeafState).Name != "Complete")
                    //if (CanExit)
                    {
                        ViewViewModel vvm = Controller.FindVvm((sender as LeafState).Name);
                        dynamic vm = vvm.vm;

                        int ix = vm.AddItem();
                        int ct = vm.Items.Count;

                        Logger.Log(String.Format("Added {0} detail = {1} of {2}", (sender as LeafState).Name, ix, ct));

                        PropertyViewer.ShowProperties(vm.Item);
						PropertyViewer.ShowPropertyList(Controller, CurrentState.Name, vm.Item);
                    }
                    Logger.Log(String.Format("State {0}: {1}", (sender as LeafState).Name, args.StateEventType));
                };
            }
        }

        public void CustomStateMachine_OnEntry()
        {
			Logger.Log($"CustomStateMachine_OnEntry");
			MainWindow = new MainWindow();

            MainWindow.Closing += (object sender, CancelEventArgs args) =>
            {
                if (!CanExit)
                {
                    var bres = MessageBox.Show("Are you sure you want to exit?", "RunStateMachine", MessageBoxButton.YesNo);
                    if (bres == MessageBoxResult.Yes)
                        args.Cancel = false;
                    else if (CurrentState.Identifier == Model.states["Complete"].Identifier)
                    {
                        MessageBox.Show("But the State Machine is complete, Dude", "RunStateMachine", MessageBoxButton.OK);
                        args.Cancel = false;
                    }
                    else
                        args.Cancel = true;
                }
                else
                    args.Cancel = false;
            };
            CanExit = false;
        }

        public void CustomStateMachine_OnProcessState()
        {
			Logger.Log($"CustomStateMachine_OnProcessState");

			int tabIndex = 99;
            ViewViewModel vvm = default;

			if (CurrentState.Name == "SearchResult")
            {
                vvm = Controller.WireUPDynamicListVvm(CurrentState.Name);
            }
            else
            {
                vvm = Controller.WireUPDynamicVvm(CurrentState.Name, false);
            }

			dynamic genView = vvm.vw;
            dynamic dynview;

            if (CurrentState.Name == "SearchResult")
            {
                dynview = genView.ListView;
                dynview.ButtonPanel.Children.Clear();
            }
            else
            {
                dynview = genView.DetailView;
                dynview.ButtonPanel.Children.Clear();
            }

			genViewModel = vvm.vm;

			genViewModel.RequestOk += new RoutedEventHandler(vm_RequestOk);
			genViewModel.RequestCancel += new RoutedEventHandler(vm_RequestCancel);
			genViewModel.RequestClose += new RoutedEventHandler(vm_RequestClose);

            foreach (Transition tran in Model.transitions.Values)
            {
                if (tran.FromState.Name == CurrentState.Name)
                {
                    string inputName = tran.OnInput.Name;
                    string inputCommandName = String.Format("{0}Command", inputName);

                    genViewModel.RegisterDynamicCommand(inputCommandName);

                    RelayCommand rc = genViewModel.GetDynamicCommand(inputCommandName);

                    dynview.BuildButton(dynview.ButtonPanel, ref tabIndex, inputName, false, rc, true);
                }
            }
            genViewModel.RequestDynamic += new DynamicCommandEventHandler(vm_RequestDynamic);

            MainWindow.Content = genView;
            MainWindow.Title = String.Format("Current state: {0}", CurrentState.Name);
            MainWindow.SizeToContent = SizeToContent.WidthAndHeight;
            MainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            bResult = null;

            //XamlExporter.ExportXaml(genView.DetailView.Panel, String.Format("{0}DialogPanel", CurrentState.Name)); 

            var ccc = MainWindow.ShowDialog();

            StateMachineInput newInput = Model.inputs["Finish"];

            if (bResult != null)
                newInput = Model.inputs[newCommand.Replace("Command", "")];

            ProcessInput(newInput);
        }

        public void CustomStateMachine_OnExit()
        {
			Logger.Log($"CustomStateMachine_OnExit");
			CanExit = true;
        }

        #region ViewModel Handlers

        private void vm_RequestOk(object sender, EventArgs args)
        {
			Logger.Log($"vm_RequestOk");
			bResult = true;
            MainWindow.Hide();

			var item = genViewModel.Item;

			genViewModel.RequestOk -= new RoutedEventHandler(vm_RequestOk);
            genViewModel.RequestCancel -= new RoutedEventHandler(vm_RequestCancel);
            genViewModel.RequestClose -= new RoutedEventHandler(vm_RequestClose); 
            genViewModel.RequestDynamic -= new DynamicCommandEventHandler(vm_RequestDynamic);
		}
        private void vm_RequestCancel(object sender, EventArgs args)
        {
			Logger.Log($"vm_RequestCancel");
			bResult = false;
            MainWindow.Hide();

            genViewModel.RequestOk -= new RoutedEventHandler(vm_RequestOk);
            genViewModel.RequestCancel -= new RoutedEventHandler(vm_RequestCancel);
            genViewModel.RequestClose -= new RoutedEventHandler(vm_RequestClose);
			genViewModel.RequestDynamic -= new DynamicCommandEventHandler(vm_RequestDynamic);
		}
		private void vm_RequestClose(object sender, RoutedEventArgs args)
        {
			Logger.Log($"vm_RequestClose");
			bResult = null;
            MainWindow.Hide();

            genViewModel.RequestOk -= new RoutedEventHandler(vm_RequestOk);
            genViewModel.RequestCancel -= new RoutedEventHandler(vm_RequestCancel);
            genViewModel.RequestClose -= new RoutedEventHandler(vm_RequestClose);
			genViewModel.RequestDynamic -= new DynamicCommandEventHandler(vm_RequestDynamic);
		}
		private void vm_RequestDynamic(object sender, DynamicCommandEventArgs args)
        {
			Logger.Log($"vm_RequestDynamic");
			bResult = true;
            MainWindow.Hide();

			var items = genViewModel.Items;

			newCommand = args.CommandName;

            genViewModel.RequestOk -= new RoutedEventHandler(vm_RequestOk);
            genViewModel.RequestCancel -= new RoutedEventHandler(vm_RequestCancel);
            genViewModel.RequestClose -= new RoutedEventHandler(vm_RequestClose);
			genViewModel.RequestDynamic -= new DynamicCommandEventHandler(vm_RequestDynamic);
		}

		#endregion ViewModel Handlers
		#endregion State Machine
	}
}
