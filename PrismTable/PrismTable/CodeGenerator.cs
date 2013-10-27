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

namespace PrismTable
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

        protected DTE2 targetDTE2;
        protected Solution2 targetSolution2;
        protected Project targetProject;

        // TemplateMethod DesignPattern
        private RenameT4Templates renameT4Templates;
        private RemoveT4Templates removeT4Templates;
        private RenameGeneratedCode renameGeneratedCode;
        private CopyGeneratedTableCode copyGeneratedTableCode;

        EnvDTE.CommandEvents ce;

        public CodeGenerator(DTE2 dte)
        {
            dte2 = dte;
            sln2 = (Solution2)dte2.Solution;

            renameT4Templates = new RenameT4Templates(sln2);
            removeT4Templates = new RemoveT4Templates(sln2);
            renameGeneratedCode = new RenameGeneratedCode(sln2);
            copyGeneratedTableCode = new CopyGeneratedTableCode(sln2);

            SetFindReplaceList();
        }

        #region CreateTableSolution

        public void CreateTableSolution()
        {
            // Create Table solution from PrismTemplates Visual Studio project templates 
            // (zip files in ..\Users\...\Documents\My Exported Templates).
            MessageService.WriteMessage(string.Empty);
            MessageService.WriteMessage("===================================================");
            MessageService.WriteMessage(string.Format("Creating {0}", Connect.settingsObject.TableSolutionPath));
            MessageService.WriteMessage("===================================================");

            sln2.Create(Utilities.RemoveEndBackSlash(Connect.settingsObject.TableSolutionPath), Connect.settingsObject.TableSolutionName);
            MessageService.WriteMessage(string.Format("{0} created", Connect.settingsObject.TableSolutionName));

            try
            {
                int numProjects = Connect.settingsObject.TableProjectNames.Count;
                for (int i = 0; i < numProjects; i++)
                {
                    templatePath = sln2.GetProjectTemplate(Connect.settingsObject.ZipFileNames[i], "CSharp");
                    projectPath = string.Format("{0}{1}", Connect.settingsObject.TableSolutionPath, Utilities.RemoveEndBackSlash(Connect.settingsObject.TableProjectPaths[i]));
                    sln2.AddFromTemplate(templatePath, projectPath, Utilities.RemoveEndBackSlash(Connect.settingsObject.TableProjectPaths[i]), false);
                    MessageService.WriteMessage(string.Format("{0} created", Connect.settingsObject.TableProjectNames[i]));
                }
            }
            catch (Exception x)
            {
                throw x;
            }

            sln2.SaveAs(string.Format("{0}{1}", Connect.settingsObject.TableSolutionPath, Connect.settingsObject.TableSolutionName));
            CloseTableSolution();
            MessageService.WriteMessage(string.Format("{0} created", Connect.settingsObject.TableSolutionName));
        }

        #endregion

        #region TargetDTE

        public void InitTargetDTE()
        {
            System.Type t = Connect.settingsObject.VisualStudioVersion;
            object obj = Activator.CreateInstance(t, true);
            targetDTE2 = (DTE2)obj;
            targetSolution2 = (Solution2)targetDTE2.Solution;
        }

        public void QuitTargetDTE()
        {
            if (targetDTE2 != null)
            {
                targetDTE2.Quit();
                targetDTE2 = null;
            }
        }
        
        #endregion

        #region OpenSolutions

        public virtual void OpenTableSolution()
        {
            MessageService.WriteMessage(string.Empty);
            MessageService.WriteMessage("===================================================");
            MessageService.WriteMessage(string.Format("Opening {0}...Please be patient...", Connect.settingsObject.TableSolutionName));
            MessageService.WriteMessage("===================================================");

            sln2.Open(string.Format("{0}{1}", Connect.settingsObject.TableSolutionPath, Connect.settingsObject.TableSolutionName));

            MessageService.WriteMessage(string.Empty);
            MessageService.WriteMessage(string.Format("{0} opened", Connect.settingsObject.TableSolutionName));

        }

        public virtual void OpenTargetSolution()
        {
            MessageService.WriteMessage(string.Empty);
            MessageService.WriteMessage("===================================================");
            MessageService.WriteMessage(string.Format("Opening {0}...Please be patient...", Connect.settingsObject.TargetSolutionName));
            MessageService.WriteMessage("===================================================");

            targetSolution2.Open(string.Format("{0}{1}", Connect.settingsObject.TargetSolutionPath, Connect.settingsObject.TargetSolutionName));

            MessageService.WriteMessage(string.Empty);
            MessageService.WriteMessage(string.Format("{0} opened", Connect.settingsObject.TargetSolutionName));

        }

        public virtual void OpenTargetSolution2()
        {
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

        #region SaveSolutions

        private void SaveTableSolution()
        {
            MessageService.WriteMessage("===================================================");
            int numProjects = Connect.settingsObject.TableProjectNames.Count;
            for (int i = 0; i < numProjects; i++)
            {
                try
                {
                    prj = sln2.Item(string.Format("{0}{1}", Connect.settingsObject.TableProjectPaths[i], Connect.settingsObject.TableProjectNames[i]));
                    prj.Save(string.Format("{0}{1}{2}", Connect.settingsObject.TableSolutionPath, Connect.settingsObject.TableProjectPaths[i], Connect.settingsObject.TableProjectNames[i]));
                    MessageService.WriteMessage(string.Format("{0} saved", Connect.settingsObject.TableProjectNames[i]));
                }
                catch { }
            }

            sln2.SaveAs(string.Format("{0}{1}", Connect.settingsObject.TableSolutionPath, Connect.settingsObject.TableSolutionName));
            MessageService.WriteMessage(string.Format("{0} saved", Connect.settingsObject.TableSolutionName));
        }

        private void SaveTargetSolution()
        {
            MessageService.WriteMessage("===================================================");
            int numProjects = Connect.settingsObject.TargetProjectNames.Count;
            for (int i = 0; i < numProjects; i++)
            {
                targetProject = targetSolution2.Item(string.Format("{0}{1}", Connect.settingsObject.TargetProjectPaths[i], Connect.settingsObject.TargetProjectNames[i]));
                targetProject.Save(string.Format("{0}{1}{2}", Connect.settingsObject.TargetSolutionPath, Connect.settingsObject.TargetProjectPaths[i], Connect.settingsObject.TargetProjectNames[i]));
                MessageService.WriteMessage(string.Format("{0} saved", Connect.settingsObject.TargetProjectNames[i]));
            }

            targetSolution2.SaveAs(string.Format("{0}{1}", Connect.settingsObject.TargetSolutionPath, Connect.settingsObject.TargetSolutionName));
            MessageService.WriteMessage(string.Format("{0} saved", Connect.settingsObject.TargetSolutionName));
        }
        #endregion

        #region CloseSolutions

        public virtual void CloseTableSolution()
        {
            MessageService.WriteMessage(string.Empty);
            MessageService.WriteMessage("===================================================");
            MessageService.WriteMessage(string.Format("Closing {0}", Connect.settingsObject.TableSolutionName));
            MessageService.WriteMessage("===================================================");

            if (sln2 != null)
            {
                sln2.Close();
            }

            MessageService.WriteMessage(string.Format("{0} closed", Connect.settingsObject.TableSolutionName));
        }

        public virtual void CloseTargetSolution()
        {
            MessageService.WriteMessage(string.Empty);
            MessageService.WriteMessage("===================================================");
            MessageService.WriteMessage(string.Format("Closing {0}", Connect.settingsObject.TargetSolutionName));
            MessageService.WriteMessage("===================================================");

            if (targetSolution2 != null)
            {
                targetSolution2.Close();
            }

            MessageService.WriteMessage(string.Format("{0} closed", Connect.settingsObject.TargetSolutionName));
        }

        #endregion

        #region DeleteSolution

        public void DeleteTableSolution()
        {
            Directory.Delete(Connect.settingsObject.DeleteTableSolutionPath, true);
        }

        #endregion

        #region RenameT4Templates

        public void RenameT4Templates()
        {
            OpenTableSolution();
            renameT4Templates.TraverseAllProjects();
            SaveTableSolution();
            CloseTableSolution();
        }

        #endregion

        #region ReplaceInT4Templates

        public void ReplaceInT4Templates()
        {
            MessageService.WriteMessage(string.Empty);
            MessageService.WriteMessage("===================================================");
            MessageService.WriteMessage("FindAndReplace");
            MessageService.WriteMessage("===================================================");
            Application.DoEvents();

            OpenTableSolution();
            FindAndReplaceSolutionFiles(Connect.settingsObject.TableSolutionPath);
            CloseTableSolution();
        }

        #endregion

        #region TransformAllT4Templates

        public void TransformAllT4Templates()
        {
            // At this point the generic .tt files have been renamed to Domain specific names and all the generic words 
            // in the .tt files have been replaced with Domain specific words. Now run the Transform on all the 
            // .tt files to generate Domain specific code.
            MessageService.WriteMessage(string.Empty);
            MessageService.WriteMessage("==================================================================================================");
            MessageService.WriteMessage("Transforming templates....Generating Target Code....Please be patient....Takes about 30 seconds...");
            MessageService.WriteMessage("==================================================================================================");
            Application.DoEvents();

            try
            {
                OpenTableSolution();
                ce = dte2.Events.CommandEvents;
                ce.AfterExecute += new _dispCommandEvents_AfterExecuteEventHandler(TransformAllT4Templates_AfterExecute);

                dte2.ExecuteCommand("Build.TransformAllT4Templates");
            }
            catch (Exception ex)
            {
                MessageService.WriteMessage(ex.Message);
            }
        }

        private void TransformAllT4Templates_AfterExecute(string guid, int ID, object customIn, object customOut)
        {
            if (ID == 234)
            {
                ce.AfterExecute -= new _dispCommandEvents_AfterExecuteEventHandler(TransformAllT4Templates_AfterExecute);
                CloseTableSolution();
                OnT4TransformCompleted();
            }
        }

        #endregion

        #region CopyGeneratedTableCode

        public void CopyGeneratedTableCode()
        {
            OpenTableSolution();

            InitTargetDTE();
            OpenTargetSolution();
            copyGeneratedTableCode.targetSolution2 = targetSolution2;
            copyGeneratedTableCode.TraverseAllProjects();
            SaveTargetSolution();
            CloseTargetSolution();
            QuitTargetDTE();

            SaveTableSolution();
            CloseTableSolution();

        }

        #endregion

        #region FindAndReplace methods

        public void FindAndReplace()
        {
            MessageService.WriteMessage(string.Empty);
            MessageService.WriteMessage("===================================================");
            MessageService.WriteMessage("FindAndReplace");
            MessageService.WriteMessage("===================================================");
            Application.DoEvents();

            FindAndReplaceSolutionFiles(Connect.settingsObject.TableSolutionPath);
        }

        protected void FindAndReplaceSolutionFiles(string solutionPath)
        {
            // PrismTemplates is a generic solution. It contains no Domain specific names.
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

        protected void SetFindReplaceList()
        {
            findReplaceList = new List<KeyValuePair<string, string>>();
            findReplaceList.Add(new KeyValuePair<string, string>("\"EntityId\"", string.Format("\"{0}Id\"", Connect.settingsObject.EntityName)));
            findReplaceList.Add(new KeyValuePair<string, string>("\"Entity\"", string.Format("\"{0}\"", Connect.settingsObject.EntityName)));
            findReplaceList.Add(new KeyValuePair<string, string>("\"Entities\"", string.Format("\"{0}\"", Connect.settingsObject.EntityNamePlural)));
            findReplaceList.Add(new KeyValuePair<string, string>("\"entityId\"", string.Format("\"{0}Id\"", Connect.settingsObject.EntityNameParameter)));
            findReplaceList.Add(new KeyValuePair<string, string>("\"entity\"", string.Format("\"{0}\"", Connect.settingsObject.EntityNameParameter)));
            findReplaceList.Add(new KeyValuePair<string, string>("\"entities\"", string.Format("\"{0}\"", Connect.settingsObject.EntityNameParameterPlural)));
            findReplaceList.Add(new KeyValuePair<string, string>("\"EntityRulesTableName\"", string.Format("\"{0}\"", Connect.settingsObject.EntityRulesTableName)));
            findReplaceList.Add(new KeyValuePair<string, string>("\"EntityStringsTableName\"", string.Format("\"{0}\"", Connect.settingsObject.EntityStringsTableName)));
            findReplaceList.Add(new KeyValuePair<string, string>("\"EntityRules\"", string.Format("\"{0}Rules\"", Connect.settingsObject.EntityName)));
            findReplaceList.Add(new KeyValuePair<string, string>("\"EntityListPageViewModel\"", string.Format("\"{0}PageViewModel\"", Connect.settingsObject.EntityName)));
            findReplaceList.Add(new KeyValuePair<string, string>("\"EntityListPage\"", string.Format("\"{0}ListPage\"", Connect.settingsObject.EntityName)));
            findReplaceList.Add(new KeyValuePair<string, string>("\"EntityList\"", string.Format("\"{0}List\"", Connect.settingsObject.EntityName)));
            findReplaceList.Add(new KeyValuePair<string, string>("\"EntityDetailPageViewModel\"", string.Format("\"{0}DetailPageViewModel\"", Connect.settingsObject.EntityName)));
            findReplaceList.Add(new KeyValuePair<string, string>("\"EntityDetailPage\"", string.Format("\"{0}DetailPage\"", Connect.settingsObject.EntityName)));
            findReplaceList.Add(new KeyValuePair<string, string>("\"EntityDetail\"", string.Format("\"{0}Detail\"", Connect.settingsObject.EntityName)));
            findReplaceList.Add(new KeyValuePair<string, string>("DELL15z", Connect.settingsObject.DELL15z));
            findReplaceList.Add(new KeyValuePair<string, string>("PrismTableDatabaseName", Connect.settingsObject.PrismTableDatabaseName));
            findReplaceList.Add(new KeyValuePair<string, string>("PrismTableEntities", Connect.settingsObject.PrismTableEntities));
            findReplaceList.Add(new KeyValuePair<string, string>("PrismTable", Connect.settingsObject.PrismTable));
            findReplaceList.Add(new KeyValuePair<string, string>("Entity", Connect.settingsObject.EntityName));
        }

        protected void WalkDirectoryTree(System.IO.DirectoryInfo root, string fileSpec, List<KeyValuePair<string, string>> findReplaceList)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder 
            try
            {
                files = root.GetFiles(fileSpec);
            }
            // This is thrown if even one of the files requires permissions greater 
            // than the application provides. 
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse. 
                // You may decide to do something different here. For example, you 
                // can try to elevate your privileges and access the file again.
                MessageService.WriteMessage(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                MessageService.WriteMessage(e.Message);
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    // In this example, we only access the existing FileInfo object. If we 
                    // want to open, delete or modify the file, then 
                    // a try-catch block is required here to handle the case 
                    // where the file has been deleted since the call to TraverseTree().
                    MessageService.WriteMessage(fi.FullName);
                    foreach (KeyValuePair<string, string> kvp in findReplaceList)
                    {
                        try
                        {
                            FindAndReplace(fi.FullName, kvp);
                        }
                        catch (Exception ex)
                        {
                            MessageService.WriteMessage(ex.Message);
                        }
                    }
                }

                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    WalkDirectoryTree(dirInfo, fileSpec, findReplaceList);
                }
            }
        }

        protected void FindAndReplace(string fileName, KeyValuePair<string, string> kvp)
        {
            StringBuilder newFile = new StringBuilder();
            string temp = "";
            string[] file = File.ReadAllLines(fileName);

            foreach (string line in file)
            {
                if (line.Contains(kvp.Key))
                {
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
        public static void OnT4TransformCompleted()
        {
            if (T4TransformCompleted != null)
            {
                T4TransformCompleted(null, new EventArgs());
            }
        }

        #endregion

        #region old code

        //private void dte2Init() {
        //    System.Type t = Connect.settingsObject.VisualStudioVersion;
        //    object obj = Activator.CreateInstance(t, true);
        //    dte2 = (DTE2)obj;
        //    sln2 = (Solution2)dte2.Solution;
        //}

        //protected void dte2Quit() {
        //    if (dte2 != null) {
        //        dte2.Quit();
        //        dte2 = null;
        //    }
        //}

        //#region RemoveFromSolution

        //public void RemoveFromSolution()
        //{
        //    OpenSolution();
        //    RemoveProject("CommonTemplate");
        //    removeT4Templates.TraverseAllProjects();
        //    SaveProjectsAndSolution();
        //    CloseSolution();
        //}

        //private void RemoveProject(string projectName)
        //{
        //    MessageService.WriteMessage(string.Empty);
        //    MessageService.WriteMessage("==================================================================================================");
        //    MessageService.WriteMessage(string.Format("Removing Project: {0}....Please be patient....", projectName));
        //    MessageService.WriteMessage("==================================================================================================");
        //    Application.DoEvents();

        //    foreach (Project project in sln2.Projects)
        //    {
        //        if (project.UniqueName.IndexOf(projectName) != -1)
        //        {
        //            sln2.Remove(project);
        //            MessageService.WriteMessage(string.Format("Project: {0} removed", projectName));
        //            break;
        //        }
        //    }
        //}

        //#endregion

        //#region RenameGeneratedCodeFiles

        //public void RenameGeneratedCodeFiles()
        //{
        //    OpenSolution();
        //    renameGeneratedCode.TraverseAllProjects();
        //    SaveProjectsAndSolution();
        //    CloseSolution();
        //}

        //#endregion

        #endregion

    }
}
