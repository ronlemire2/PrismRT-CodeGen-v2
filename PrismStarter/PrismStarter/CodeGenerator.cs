using EnvDTE;
using EnvDTE80;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrismStarter
{
    public class CodeGenerator
    {
        protected DTE2 dte2;
        protected Solution2 sln2;
        protected Project prj;
        protected ProjectItem folder;
        protected ProjectItem file;
        protected string templatePath;
        protected string projectPath;
        protected Project currentProject;
        protected ProjectItem currentFolder;
        
        // TemplateMethod DesignPattern
        private RemoveT4Templates removeT4Templates;
        private RenameGeneratedCode renameGeneratedCode;

        EnvDTE.CommandEvents ce;

        public CodeGenerator(DTE2 dte) {
            dte2 = dte;
            sln2 = (Solution2)dte2.Solution;

            removeT4Templates = new RemoveT4Templates(sln2);
            renameGeneratedCode = new RenameGeneratedCode(sln2);

            SetFindReplaceList();
        }

        #region CreateSolution

        public void CreateSolution() {
            // Create Target solution from PrismStarter Visual Studio project templates 
            // (zip files in ..\Users\...\Documents\My Exported Templates).
            MessageService.WriteMessage(string.Empty);
            MessageService.WriteMessage("===================================================");
            MessageService.WriteMessage(string.Format("Creating {0}", Connect.settingsObject.TargetSolutionPath));
            MessageService.WriteMessage("===================================================");
            
            sln2.Create(Utilities.RemoveEndBackSlash(Connect.settingsObject.TargetSolutionPath), Connect.settingsObject.TargetSolutionName);
            MessageService.WriteMessage(string.Format("{0} created", Connect.settingsObject.TargetSolutionName));

            try {
                int numProjects = Connect.settingsObject.ProjectNames.Count;
                for (int i = 0; i < numProjects; i++) {
                    templatePath = sln2.GetProjectTemplate(Connect.settingsObject.ZipFileNames[i], "CSharp");
                    projectPath = string.Format("{0}{1}", Connect.settingsObject.TargetSolutionPath, Utilities.RemoveEndBackSlash(Connect.settingsObject.ProjectPaths[i]));
                    sln2.AddFromTemplate(templatePath, projectPath, Utilities.RemoveEndBackSlash(Connect.settingsObject.ProjectPaths[i]), false);
                    MessageService.WriteMessage(string.Format("{0} created", Connect.settingsObject.ProjectNames[i]));
                }
            }
            catch (Exception x) {
                throw x;
            }

            sln2.SaveAs(string.Format("{0}{1}", Connect.settingsObject.TargetSolutionPath, Connect.settingsObject.TargetSolutionName));
            CloseSolution();
            MessageService.WriteMessage(string.Format("{0} created", Connect.settingsObject.TargetSolutionName));
        }

        private void SaveProjectsAndSolution() {
            MessageService.WriteMessage("===================================================");
            int numProjects = Connect.settingsObject.ProjectNames.Count;
            for (int i = 0; i < numProjects; i++) {
                try {
                    prj = sln2.Item(string.Format("{0}{1}", Connect.settingsObject.ProjectPaths[i], Connect.settingsObject.ProjectNames[i]));
                    prj.Save(string.Format("{0}{1}{2}", Connect.settingsObject.TargetSolutionPath, Connect.settingsObject.ProjectPaths[i], Connect.settingsObject.ProjectNames[i]));
                    MessageService.WriteMessage(string.Format("{0} saved", Connect.settingsObject.ProjectNames[i]));
                }
                catch { }
            }

            sln2.SaveAs(string.Format("{0}{1}", Connect.settingsObject.TargetSolutionPath, Connect.settingsObject.TargetSolutionName));
            MessageService.WriteMessage(string.Format("{0} saved", Connect.settingsObject.TargetSolutionName));
        }

        #endregion

        #region OpenSolution

        public virtual void OpenSolution() {
            MessageService.WriteMessage(string.Empty);
            MessageService.WriteMessage("===================================================");
            MessageService.WriteMessage(string.Format("Opening {0}...Please be patient...", Connect.settingsObject.TargetSolutionName));
            MessageService.WriteMessage("===================================================");

            sln2.Open(string.Format("{0}{1}", Connect.settingsObject.TargetSolutionPath, Connect.settingsObject.TargetSolutionName));

            MessageService.WriteMessage(string.Empty);
            MessageService.WriteMessage(string.Format("{0} opened", Connect.settingsObject.TargetSolutionName));

        }

        //public bool CheckForOpenSolution() {
        //    if (dte2 == null) {
        //        MessageService.WriteMessage("Solution is not opened. Please open solution.");
        //        return false;
        //    }
        //    return true;
        //}

        #endregion

        #region CloseSolution

        public virtual void CloseSolution() {
            MessageService.WriteMessage(string.Empty);
            MessageService.WriteMessage("===================================================");
            MessageService.WriteMessage(string.Format("Closing {0}", Connect.settingsObject.TargetSolutionName));
            MessageService.WriteMessage("===================================================");

            if (sln2 != null) {
                sln2.Close();
            }

            MessageService.WriteMessage(string.Format("{0} closed", Connect.settingsObject.TargetSolutionName));
        }

        #endregion

        #region ReplaceInT4Templates

        public void ReplaceInT4Templates() {
            MessageService.WriteMessage(string.Empty);
            MessageService.WriteMessage("===================================================");
            MessageService.WriteMessage("FindAndReplace");
            MessageService.WriteMessage("===================================================");
            Application.DoEvents();

            OpenSolution();
            FindAndReplaceSolutionFiles(Connect.settingsObject.TargetSolutionPath);
            CloseSolution();
        }

        #endregion

        #region TransformAllT4Templates

        public void TransformAllT4Templates() {
            // At this point the generic .tt files have been renamed to Domain specific names and all the generic words 
            // in the .tt files have been replaced with Domain specific words. Now run the Transform on all the 
            // .tt files to generate Domain specific code.
            MessageService.WriteMessage(string.Empty);
            MessageService.WriteMessage("==================================================================================================");
            MessageService.WriteMessage("Transforming templates....Generating Target Code....Please be patient....Takes about 30 seconds...");
            MessageService.WriteMessage("==================================================================================================");
            Application.DoEvents();

            try {
                OpenSolution();
                ce = dte2.Events.CommandEvents;
                ce.AfterExecute += new _dispCommandEvents_AfterExecuteEventHandler(TransformAllT4Templates_AfterExecute); 
                
                dte2.ExecuteCommand("Build.TransformAllT4Templates");
            }
            catch (Exception ex) {
                MessageService.WriteMessage(ex.Message);
            }
        }

        private void TransformAllT4Templates_AfterExecute(string guid, int ID, object customIn, object customOut) {
            if (ID == 234) {
                ce.AfterExecute -= new _dispCommandEvents_AfterExecuteEventHandler(TransformAllT4Templates_AfterExecute);
                CloseSolution();
                OnT4TransformCompleted();
            }
        }

        #endregion

        #region RemoveFromSolution

        public void RemoveFromSolution() {
            OpenSolution();
            RemoveProject("CommonStarterTemplate");
            removeT4Templates.TraverseAllProjects();
            SaveProjectsAndSolution();
            CloseSolution();
        }

        private void RemoveProject(string projectName) {
            MessageService.WriteMessage(string.Empty);
            MessageService.WriteMessage("==================================================================================================");
            MessageService.WriteMessage(string.Format("Removing Project: {0}....Please be patient....", projectName));
            MessageService.WriteMessage("==================================================================================================");
            Application.DoEvents();

            foreach (Project project in sln2.Projects) {
                if (project.UniqueName.IndexOf(projectName) != -1) {
                    sln2.Remove(project);
                    MessageService.WriteMessage(string.Format("Project: {0} removed", projectName));
                    break;
                }
            }
        }

        #endregion

        #region RenameGeneratedCodeFiles

        public void RenameGeneratedCodeFiles() {
            OpenSolution();
            renameGeneratedCode.TraverseAllProjects();
            SaveProjectsAndSolution();
            //CloseSolution();
        }
        #endregion

        #region FindAndReplace methods

        public void FindAndReplace() {
            MessageService.WriteMessage(string.Empty);
            MessageService.WriteMessage("===================================================");
            MessageService.WriteMessage("FindAndReplace");
            MessageService.WriteMessage("===================================================");
            Application.DoEvents();

            FindAndReplaceSolutionFiles(Connect.settingsObject.TargetSolutionPath);
        }

        protected void FindAndReplaceSolutionFiles(string solutionPath) {
            // PrismStarter is a generic solution. It contains no Domain specific names.
            // e.g. They contain many placeholders with the word 'Entity'. These generic words need to be
            // replace with Domain specific words e.g. 'Enity' needs to be replaced with 'BigBrother'.
            // All solution files do not need to be processed just the ones in the filespecs fed to 
            // the following WalkDirectoryTree method.
            DirectoryInfo directoryInfo = new DirectoryInfo(solutionPath);
            WalkDirectoryTree(directoryInfo, "*.tt", findReplaceList);
            WalkDirectoryTree(directoryInfo, "*.cs", findReplaceList);
            WalkDirectoryTree(directoryInfo, "*.xaml", findReplaceList);
        }

        protected List<KeyValuePair<string, string>> findReplaceList = null;

        protected void SetFindReplaceList() {
            findReplaceList = new List<KeyValuePair<string, string>>();
            findReplaceList.Add(new KeyValuePair<string, string>("\"EntityStringsTableName\"", string.Format("\"{0}\"", Connect.settingsObject.EntityStringsTableName)));
            findReplaceList.Add(new KeyValuePair<string, string>("DELL15z", Connect.settingsObject.DELL15z));
            findReplaceList.Add(new KeyValuePair<string, string>("PrismStarterDatabaseName", Connect.settingsObject.PrismStarterDatabaseName));
            findReplaceList.Add(new KeyValuePair<string, string>("PrismStarterEntities", Connect.settingsObject.PrismStarterEntities));
            findReplaceList.Add(new KeyValuePair<string, string>("PrismStarter", Connect.settingsObject.PrismStarter));
        }

        protected void WalkDirectoryTree(System.IO.DirectoryInfo root, string fileSpec, List<KeyValuePair<string, string>> findReplaceList) {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder 
            try {
                files = root.GetFiles(fileSpec);
            }
            // This is thrown if even one of the files requires permissions greater 
            // than the application provides. 
            catch (UnauthorizedAccessException e) {
                // This code just writes out the message and continues to recurse. 
                // You may decide to do something different here. For example, you 
                // can try to elevate your privileges and access the file again.
                MessageService.WriteMessage(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e) {
                MessageService.WriteMessage(e.Message);
            }

            if (files != null) {
                foreach (System.IO.FileInfo fi in files) {
                    // In this example, we only access the existing FileInfo object. If we 
                    // want to open, delete or modify the file, then 
                    // a try-catch block is required here to handle the case 
                    // where the file has been deleted since the call to TraverseTree().
                    MessageService.WriteMessage(fi.FullName);
                    foreach (KeyValuePair<string, string> kvp in findReplaceList) {
                        try {
                            FindAndReplace(fi.FullName, kvp);
                        }
                        catch (Exception ex) {
                            MessageService.WriteMessage(ex.Message);
                        }
                    }
                }

                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs) {
                    // Resursive call for each subdirectory.
                    WalkDirectoryTree(dirInfo, fileSpec, findReplaceList);
                }
            }
        }

        protected void FindAndReplace(string fileName, KeyValuePair<string, string> kvp) {
            StringBuilder newFile = new StringBuilder();
            string temp = "";
            string[] file = File.ReadAllLines(fileName);

            foreach (string line in file) {
                if (line.Contains(kvp.Key)) {
                    temp = line.Replace(kvp.Key, kvp.Value);
                    newFile.Append(temp + "\r\n");
                    continue;
                }
                newFile.Append(line + "\r\n");
            }
            File.WriteAllText(fileName, newFile.ToString());
        }

        #endregion

        #region Messaging

        public delegate void T4TransformCompletedEventHandler(object sender, EventArgs args);
        public static event T4TransformCompletedEventHandler T4TransformCompleted;
        public static void OnT4TransformCompleted() {
            if (T4TransformCompleted != null) {
                T4TransformCompleted(null, new EventArgs());
            }
        }

        #endregion
    }
}
