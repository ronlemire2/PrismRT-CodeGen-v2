using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismStarter
{
    // =====================================================================================
    // TemplateMethod DesignPattern
    // =====================================================================================
    // Abstract ProjectTreeOperation navigates from solution->projects->folders->projectitems
    //    which is common to all derived classes.
    // Derived classes implement the ProjectItem processing in the ProcessFolderItem method.
    public abstract class ProjectTreeOperation
    {        
        protected Solution2 sln2;
        protected List<string> folderList;
        protected ProjectItem folder;
        protected ProjectItem file;

        #region Common Functionality

        public ProjectTreeOperation(Solution2 sln2) {
            this.sln2 = sln2;
        }

        protected ProjectTreeOperation() {
        }

        public void TraverseAllProjects() {
            string currentProjectName = string.Empty;
            string currentFilePath = string.Empty;

            foreach (Project project in sln2.Projects) {
                currentProjectName = project.Name;
                MessageService.WriteMessage(string.Empty);
                MessageService.WriteMessage("===================================================");
                MessageService.WriteMessage(string.Format("Project: {0}", project.Name));
                MessageService.WriteMessage("===================================================");
                TraverseOneProject(project);
            }
        }

        private void TraverseOneProject(Project project) {
            foreach (ProjectItem projectItem in project.ProjectItems) {
                if (projectItem.Kind == EnvDTEConstants.vsProjectItemKindPhysicalFolder) {
                    folderList = new List<string>();
                    TraverseProjectFolders(projectItem);
                }
                else {
                    if (projectItem.Kind == EnvDTEConstants.vsProjectItemKindPhysicalFile && IsKindT4(projectItem)) {
                        ProcessFolderItem(projectItem);
                        MessageService.WriteMessage(string.Empty);
                    }
                }
            }
        }

        private void TraverseProjectFolders(ProjectItem projectItem) {
            if (projectItem.Kind == EnvDTEConstants.vsProjectItemKindPhysicalFolder) {
                folderList.Add(projectItem.Name);
                folder = projectItem;
                foreach (ProjectItem subFolder in projectItem.ProjectItems) {
                    // recurse folders
                    TraverseProjectFolders(subFolder);
                }
            }
            else {
                TraverseFolderItems(projectItem);
                return;
            }
        }

        private void TraverseFolderItems(ProjectItem projectItem) {
            if (projectItem.Kind == EnvDTEConstants.vsProjectItemKindPhysicalFile && IsKindT4(projectItem)) {
                MessageService.WriteMessage(string.Format("TemplateFile: {0}", projectItem.Name));

                ProcessFolderItem(projectItem);
                MessageService.WriteMessage(string.Empty);

                return;
            }
            if (projectItem.ProjectItems.Count == 0) {
                return;
            }
            else {
                foreach (ProjectItem subProjectItem in projectItem.ProjectItems) {
                    // recurse folderitems
                    TraverseFolderItems(subProjectItem);
                }
            }
        }

        protected string GetLocalPath(ProjectItem fileItem) {
            string localPath = string.Empty;
            foreach (EnvDTE.Property property in fileItem.Properties) {
                if (property.Name.ToLower() == "localpath") {
                    localPath = property.Value.ToString();
                    break;
                }
            }
            return localPath;
        }


        protected string GetPathToT4Child(ProjectItem templateItem) {
            string pathToT4Child = string.Empty;

            foreach (ProjectItem childItem in templateItem.ProjectItems) {
                if (childItem.ProjectItems.Count == 0) {
                    pathToT4Child = GetLocalPath(childItem);
                    break;
                }
                else {
                    // should not get here because templateItem should have only 1 child
                    throw new Exception("Template File has more that 1 child.");
                }
            }

            return pathToT4Child;
        }
        
        protected void DeleteT4Child(ProjectItem templateItem) {
            foreach (ProjectItem childItem in templateItem.ProjectItems) {
                if (childItem.ProjectItems.Count == 0) {
                    string localPath = GetLocalPath(childItem);
                    File.Delete(localPath);
                    MessageService.WriteMessage(string.Format("ChildFile: {0} deleted", localPath));
                }
                else {
                    // should not get here because templateItem should have only 1 child
                    throw new Exception("Template File has more that 1 child.");
                }
            }
        }

        public abstract void ProcessFolderItem(ProjectItem projectItem);
        public abstract bool IsKindT4(ProjectItem projectItem);

        #endregion
    }

    #region RemoveT4Templates

    public class RemoveT4Templates : ProjectTreeOperation
    {
        // Delete the T4 (.tt) files from the project and the file system.
        // But before doing that, make a copy of the file that the .tt generated 
        // adding a .gc to the end of its file name.
        public RemoveT4Templates(Solution2 sln2) {
            base.sln2 = sln2;
        }

        public override void ProcessFolderItem(ProjectItem projectItem) {
            string templateFileFullName = string.Empty;

            try {
                file = folder.ProjectItems.Item(projectItem.Name);
                // Remove the template file and add its generated file to the project.
                if (file != null) {
                    // Get the complete template file name including full path.
                    templateFileFullName = GetLocalPath(file);

                    // Make a copy of the file that's generated by the template file.
                    string generatedFilePath = GetPathToT4Child(file);
                    string generatedFilePathCopy = generatedFilePath + ".gc";
                    File.Copy(generatedFilePath, generatedFilePathCopy);

                    // Remove the template file from the project.
                    file.Remove();

                    // Delete the template file (.tt) and the file it generated from the file system.
                    File.Delete(templateFileFullName);
                    File.Delete(generatedFilePath);

                    // Add the copy of the file that the template generated to the project.
                    folder.ProjectItems.AddFromFile(generatedFilePathCopy);

                    MessageService.WriteMessage(string.Format("Generated file add:  {0}", generatedFilePathCopy));
                }
                else {
                    MessageService.WriteMessage(string.Format("Template: {0} not found", projectItem.Name));
                }
            }
            catch (Exception ex) {
                if (ex.Message == "Value does not fall within the expected range.") {
                    MessageService.WriteMessage(string.Format("Template: {0} not found", projectItem.Name));
                }
                else {
                    throw ex;
                }
            }
        }

        public override bool IsKindT4(ProjectItem projectItem) {
            // Check if .tt file.
            return projectItem.Name.ToLower().EndsWith(".tt");
        }
    }

    #endregion

    #region RenameGeneratedCode

    public class RenameGeneratedCode : ProjectTreeOperation
    {
        // Make copies of the .gc files but with the .gc stripped off.
        // Delete the .gc file from project and the file system.
        // Add the file with the .gc stripped off to the project.
        public RenameGeneratedCode(Solution2 sln2) {
            base.sln2 = sln2;
        }

        public override void ProcessFolderItem(ProjectItem projectItem) {
            string gcFileFullName = string.Empty;
            string gcFileFullNameRenamed = string.Empty;

            try {
                file = folder.ProjectItems.Item(projectItem.Name);
                if (file != null) {
                    // Get the generated file (.gc) name including full path.
                    gcFileFullName = GetLocalPath(file);
                    // Create a file name that has ".gc" stripped off.
                    gcFileFullNameRenamed = gcFileFullName.Substring(0, gcFileFullName.Length - 3);

                    // Make a copy of the .gc but with the .gc stripped off.
                    File.Copy(gcFileFullName, gcFileFullNameRenamed);

                    // Remove the .gc from the project and the file system.
                    file.Remove();
                    File.Delete(gcFileFullName);

                    // Add the generated file to the project. (automatically adds it to file system)
                    folder.ProjectItems.AddFromFile(gcFileFullNameRenamed);

                    MessageService.WriteMessage(string.Format("GCFile renamed. {0}", gcFileFullNameRenamed));
                }
                else {
                    MessageService.WriteMessage(string.Format("Template: {0} not found", projectItem.Name));
                }
            }
            catch (Exception ex) {
                if (ex.Message == "Value does not fall within the expected range.") {
                    MessageService.WriteMessage(string.Format("Template: {0} not found", projectItem.Name));
                }
                else {
                    throw ex;
                }
            }
        }

        public override bool IsKindT4(ProjectItem projectItem) {
            // Check if .gc file.
            return projectItem.Name.ToLower().EndsWith(".gc");
        }
    }

    #endregion

}
