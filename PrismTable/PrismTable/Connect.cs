using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;
using System.IO;

namespace PrismTable
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{
        public static Settings settingsObject;

        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
		}

		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
            try
            {
                settingsObject = new Settings();
                if (IfSolutionPathExists())
                {
                    MessageBox.Show(string.Format("SolutionPath exists already.{0}{1}{2}{3}",
                        Environment.NewLine, settingsObject.TableSolutionPath, Environment.NewLine, "Delete SolutionPath then try again."),
                        "Canceling Run");
                    return;
                }

                _applicationObject = (DTE2)application;
                _addInInstance = (AddIn)addInInst;

                SetupOutputWindow(_applicationObject);
                startTime = DateTime.Now.ToLongTimeString();


                // Create generator
                codeGenerator = new CodeGenerator(_applicationObject);

                // Event Handlers
                MessageService.MessageGenerated += new MessageService.MessageGeneratedEventHandler(MessageWriter_MessageGenerated);
                CodeGenerator.T4TransformCompleted += TemplatesGenerator_T4TransformCompleted;

                // CodeGeneration Part 1 
                // Part 2 after TransformAllT4Templates
                codeGenerator.CreateTableSolution();
                codeGenerator.RenameT4Templates();
                codeGenerator.ReplaceInT4Templates();
                codeGenerator.TransformAllT4Templates();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CodeGeneration Part 1 failed");
            }

			if(connectMode == ext_ConnectMode.ext_cm_UISetup)
			{
				object []contextGUIDS = new object[] { };
				Commands2 commands = (Commands2)_applicationObject.Commands;
				string toolsMenuName = "Tools";

				//Place the command on the tools menu.
				//Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
				Microsoft.VisualStudio.CommandBars.CommandBar menuBarCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"];

				//Find the Tools command bar on the MenuBar command bar:
				CommandBarControl toolsControl = menuBarCommandBar.Controls[toolsMenuName];
				CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;

				//This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
				//  just make sure you also update the QueryStatus/Exec method to include the new command names.
				try
				{
					//Add a command to the Commands collection:
					Command command = commands.AddNamedCommand2(_addInInstance, "PrismTable", "PrismTable", "Executes the command for PrismTable", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported+(int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

					//Add a control for the command to the tools menu:
					if((command != null) && (toolsPopup != null))
					{
						command.AddControl(toolsPopup.CommandBar, 1);
					}
				}
				catch(System.ArgumentException)
				{
					//If we are here, then the exception is probably because a command with that name
					//  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
				}
			}
		}

        void TemplatesGenerator_T4TransformCompleted(object sender, EventArgs args)
        {
            MessageService.WriteMessage("TransformAllT4Templates completed");

            // CodeGeneration Part 2
            try
            {
                codeGenerator.CopyGeneratedTableCode();
                codeGenerator.DeleteTableSolution();
                codeGenerator.OpenTargetSolution2();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CodeGeneration Part 2 failed");
            }

            endTime = DateTime.Now.ToLongTimeString();
            MessageBox.Show(string.Format("EndTime: {0}{1}StartTime: {2}", endTime, Environment.NewLine, startTime), "CodeGeneration Successful");
        }

        private void SetupOutputWindow(DTE2 applicationObject)
        {
            window = applicationObject.Windows.Item(Constants.vsWindowKindOutput);
            outputWindow = (OutputWindow)window.Object;
            owp = outputWindow.OutputWindowPanes.Add("new pane");
            owp.OutputString("hello\n");
        }

        private void MessageWriter_MessageGenerated(object sender, MessageService.MessageGeneratedEventArgs e)
        {
            owp.OutputString(e.GetCodeGenMessage + "\n");
        }

        private bool IfSolutionPathExists()
        {
            return Directory.Exists(settingsObject.TableSolutionPath);
        }

		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
		}

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom)
		{
		}

		/// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom)
		{
		}
		
		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if(neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
				if(commandName == "PrismTable.Connect.PrismTable")
				{
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
					return;
				}
			}
		}

		/// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
		/// <param term='commandName'>The name of the command to execute.</param>
		/// <param term='executeOption'>Describes how the command should be run.</param>
		/// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
		/// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
		/// <param term='handled'>Informs the caller if the command was handled or not.</param>
		/// <seealso class='Exec' />
		public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;
			if(executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
			{
				if(commandName == "PrismTable.Connect.PrismTable")
				{
					handled = true;
					return;
				}
			}
		}
		private DTE2 _applicationObject;
		private AddIn _addInInstance;
        Window window;
        OutputWindow outputWindow;
        OutputWindowPane owp;
        private CodeGenerator codeGenerator;
        private string startTime;
        private string endTime;
    }
}