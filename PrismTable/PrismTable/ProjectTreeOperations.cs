using EnvDTE;
using EnvDTE80;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.Smo;

namespace PrismTable
{
    // TemplateMethod DesignPattern
    // Abstract TraverseProjectTree navigates from solution->projects->folders->projectitems
    //    which is common to all derived classes.
    // Derived classes implement the ProjectItem processing in the ProcessFolderItem method.
    public abstract class ProjectTreeOperation
    {
        protected Solution2 sln2;
        protected Project project;
        protected List<string> folderList;
        protected ProjectItem folder;
        protected ProjectItem file;

        #region Common Functionality

        protected ProjectTreeOperation(Solution2 sln2)
        {
            this.sln2 = sln2;
        }

        protected ProjectTreeOperation()
        {
        }

        public void TraverseAllProjects()
        {
            string currentProjectName = string.Empty;
            string currentFilePath = string.Empty;

            foreach (Project prj in sln2.Projects)
            {
                project = prj;
                currentProjectName = project.Name;
                MessageService.WriteMessage(string.Empty);
                MessageService.WriteMessage("===================================================");
                MessageService.WriteMessage(string.Format("Project: {0}", prj.Name));
                MessageService.WriteMessage("===================================================");
                TraverseOneProject(project);
            }
        }

        private void TraverseOneProject(Project project)
        {
            foreach (ProjectItem projectItem in project.ProjectItems)
            {
                if (projectItem.Kind == EnvDTEConstants.vsProjectItemKindPhysicalFolder)
                {
                    folderList = new List<string>();
                    TraverseProjectFolders(projectItem);
                }
                else
                {
                    if (projectItem.Kind == EnvDTEConstants.vsProjectItemKindPhysicalFile && IsKindT4(projectItem))
                    {
                        ProcessFolderItem(projectItem);
                        MessageService.WriteMessage(string.Empty);
                    }
                }
            }
        }

        private void TraverseProjectFolders(ProjectItem projectItem)
        {
            if (projectItem.Kind == EnvDTEConstants.vsProjectItemKindPhysicalFolder)
            {
                folderList.Add(projectItem.Name);
                folder = projectItem;
                foreach (ProjectItem subFolder in projectItem.ProjectItems)
                {
                    // recurse folders
                    TraverseProjectFolders(subFolder);
                }
            }
            else
            {
                TraverseFolderItems(projectItem);
                return;
            }
        }

        private void TraverseFolderItems(ProjectItem projectItem)
        {
            if (projectItem.Kind == EnvDTEConstants.vsProjectItemKindPhysicalFile && IsKindT4(projectItem))
            {
                MessageService.WriteMessage(string.Format("TemplateFile: {0}", projectItem.Name));

                ProcessFolderItem(projectItem);
                MessageService.WriteMessage(string.Empty);

                return;
            }
            if (projectItem.ProjectItems.Count == 0)
            {
                return;
            }
            else
            {
                foreach (ProjectItem subProjectItem in projectItem.ProjectItems)
                {
                    // recurse folderitems
                    TraverseFolderItems(subProjectItem);
                }
            }
        }

        protected string GetLocalPath(ProjectItem fileItem)
        {
            string localPath = string.Empty;
            foreach (EnvDTE.Property property in fileItem.Properties)
            {
                if (property.Name.ToLower() == "localpath")
                {
                    localPath = property.Value.ToString();
                    break;
                }
            }
            return localPath;
        }

        protected string GetPathToT4Child(ProjectItem templateItem)
        {
            string pathToT4Child = string.Empty;

            foreach (ProjectItem childItem in templateItem.ProjectItems)
            {
                if (childItem.ProjectItems.Count == 0)
                {
                    pathToT4Child = GetLocalPath(childItem);
                    break;
                }
                else
                {
                    // should not get here because templateItem should have only 1 child
                    throw new Exception("Template File has more that 1 child.");
                }
            }

            return pathToT4Child;
        }

        protected void DeleteT4Child(ProjectItem templateItem)
        {
            foreach (ProjectItem childItem in templateItem.ProjectItems)
            {
                if (childItem.ProjectItems.Count == 0)
                {
                    string localPath = GetLocalPath(childItem);
                    File.Delete(localPath);
                    MessageService.WriteMessage(string.Format("ChildFile: {0} deleted", localPath));
                }
                else
                {
                    // should not get here because templateItem should have only 1 child
                    throw new Exception("Template File has more that 1 child.");
                }
            }
        }

        public abstract void ProcessFolderItem(ProjectItem projectItem);
        public abstract bool IsKindT4(ProjectItem projectItem);

        #endregion
    }

    #region RenameT4Templates

    public class RenameT4Templates : ProjectTreeOperation
    {
        public RenameT4Templates(Solution2 sln2)
        {
            base.sln2 = sln2;
        }

        public override void ProcessFolderItem(ProjectItem projectItem)
        {
            string templateFileFullName = string.Empty;
            string templateFileFullNameRenamed = string.Empty;

            try
            {
                file = folder.ProjectItems.Item(projectItem.Name);
                // Replace the template file (.tt) with a renamed copy.
                if (file != null)
                {
                    // Get the complete template file name including full path.
                    templateFileFullName = GetLocalPath(file);
                    // Replace the word 'Entity' in the template file name with the Domain EntityName.
                    templateFileFullNameRenamed = Utilities.SearchFileNameAndReplace(templateFileFullName, "Entity", Connect.settingsObject.EntityName);

                    // Make the copy.
                    File.Copy(templateFileFullName, templateFileFullNameRenamed);

                    // Delete the file that the template file (.tt) generated from the file system.
                    DeleteT4Child(file);

                    // Remove the template file (.tt) from the project.
                    file.Remove();

                    // Delete the template file (.tt) from the file system.
                    File.Delete(templateFileFullName);

                    // Add the Domain named template file (.tt) to the project. (automatically adds it to file system)
                    folder.ProjectItems.AddFromFile(templateFileFullNameRenamed);
                    
                    MessageService.WriteMessage(string.Format("Template renamed. {0}", templateFileFullNameRenamed));
                }
                else
                {
                    MessageService.WriteMessage(string.Format("Template: {0} not found", projectItem.Name));
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Value does not fall within the expected range.")
                {
                    MessageService.WriteMessage(string.Format("Template: {0} not found", projectItem.Name));
                }
                else
                {
                    throw ex;
                }
            }
        }

        public override bool IsKindT4(ProjectItem projectItem)
        {
            // Check if .tt file that contains the word 'Entity' in its Name.
            return (projectItem.Name.ToLower().IndexOf("entity") != -1) && projectItem.Name.ToLower().EndsWith(".tt");
        }
    }

    #endregion

    #region RemoveT4Templates

    public class RemoveT4Templates : ProjectTreeOperation
    {
        // Delete the T4 (.tt) files from the project and the file system.
        // But before doing that, make a copy of the file that the .tt generated 
        // adding a .gc to the end of its file name.
        public RemoveT4Templates(Solution2 sln2)
        {
            base.sln2 = sln2;
        }

        public override void ProcessFolderItem(ProjectItem projectItem)
        {
            string templateFileFullName = string.Empty;

            try
            {
                file = folder.ProjectItems.Item(projectItem.Name);
                // Remove the template file and add its generated file to the project.
                if (file != null)
                {
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
                else
                {
                    MessageService.WriteMessage(string.Format("Template: {0} not found", projectItem.Name));
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Value does not fall within the expected range.")
                {
                    MessageService.WriteMessage(string.Format("Template: {0} not found", projectItem.Name));
                }
                else
                {
                    throw ex;
                }
            }
        }

        public override bool IsKindT4(ProjectItem projectItem)
        {
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
        public RenameGeneratedCode(Solution2 sln2)
        {
            base.sln2 = sln2;
        }

        public override void ProcessFolderItem(ProjectItem projectItem)
        {
            string gcFileFullName = string.Empty;
            string gcFileFullNameRenamed = string.Empty;

            try
            {
                file = folder.ProjectItems.Item(projectItem.Name);
                if (file != null)
                {
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
                else
                {
                    MessageService.WriteMessage(string.Format("Template: {0} not found", projectItem.Name));
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Value does not fall within the expected range.")
                {
                    MessageService.WriteMessage(string.Format("Template: {0} not found", projectItem.Name));
                }
                else
                {
                    throw ex;
                }
            }
        }

        public override bool IsKindT4(ProjectItem projectItem)
        {
            // Check if .gc file.
            return projectItem.Name.ToLower().EndsWith(".gc");
        }
    }

    #endregion

    #region CopyGeneratedTableCode

    public class CopyGeneratedTableCode : ProjectTreeOperation
    {
        public Solution2 targetSolution2;
        private Project targetProject;
        private ProjectItem targetFolder;
        private ProjectItem targetFile;

        public CopyGeneratedTableCode(Solution2 sln2)
        {
            base.sln2 = sln2;
        }

        public override void ProcessFolderItem(ProjectItem projectItem)
        {
            string ttFileFullName = string.Empty;
            string childFileFullName = string.Empty;
            string targetFileFullName = string.Empty;

            try
            {
                file = folder.ProjectItems.Item(projectItem.Name);
                // Add tableProject .tt child to targetProject.
                if (file != null)
                {
                    ttFileFullName = GetLocalPath(file);
                    childFileFullName = GetPathToT4Child(file);

                    targetProject = FindProjectInTargetSolution(targetSolution2, project.UniqueName);
                    targetFolder = FindFolderInTargetProject(targetProject, folder);

                    // Add the Table .cs file to the TargetFolder. (automatically adds it to file system)
                    targetFileFullName = string.Format(@"{0}{1}\{2}", targetProject.FullName.Substring(0, targetProject.FullName.LastIndexOf("\\")+1),
                        targetFolder.Name, childFileFullName.Substring(childFileFullName.LastIndexOf("\\")+1));
                    if (Connect.settingsObject.RemoveEntity == true)
                    {
                        targetFile = targetFolder.ProjectItems.Item(childFileFullName.Substring(childFileFullName.LastIndexOf("\\") + 1));
                        if (targetFile != null)
                        {
                            targetFile.Remove();
                            if (File.Exists(targetFileFullName)) { File.Delete(targetFileFullName); }
                        }
                    }
                    else
                    {
                        if (File.Exists(targetFileFullName)) { File.Delete(targetFileFullName); }
                        File.Copy(childFileFullName, targetFileFullName);
                        targetFolder.ProjectItems.AddFromFile(targetFileFullName);
                    }

                    MessageService.WriteMessage(string.Format("GCFile renamed. {0}", childFileFullName));
                }
                else
                {
                    MessageService.WriteMessage(string.Format("File: {0} not found", projectItem.Name));
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Value does not fall within the expected range.")
                {
                    MessageService.WriteMessage(string.Format("Template: {0} not found", projectItem.Name));
                }
                else
                {
                    throw ex;
                }
            }
        }

        public override bool IsKindT4(ProjectItem projectItem)
        {
            // Check if .tt file.
            return projectItem.Name.ToLower().EndsWith(".tt");
        }

        protected Project FindProjectInTargetSolution(Solution2 targetSolution2, string tableProjectUniqueName)
        {
            Project targetProject = null;

            foreach (Project prj in targetSolution2.Projects)
            {
                if (prj.UniqueName == tableProjectUniqueName)
                {
                    targetProject = prj;
                }
            }

            return targetProject;
        }

        private ProjectItem FindFolderInTargetProject(Project targetProject, ProjectItem tableFolder)
        {
            ProjectItem targetFolder = null;

            foreach (ProjectItem projectItem in targetProject.ProjectItems)
            {
                if (projectItem.Kind == EnvDTEConstants.vsProjectItemKindPhysicalFolder)
                {
                    if (projectItem.Name == tableFolder.Name)
                    {
                        targetFolder = projectItem;
                        break;
                    }
                    CheckSubFolders(projectItem, tableFolder);
                }
                else
                {
                    continue;
                }
            }

            return targetFolder;
        }

        private ProjectItem CheckSubFolders(ProjectItem projectItem, ProjectItem tableFolder)
        {
            if (projectItem.ProjectItems.Count == 0)
            {
                return null;
            }

            foreach (ProjectItem pi in projectItem.ProjectItems)
            {
                if (projectItem.Kind == EnvDTEConstants.vsProjectItemKindPhysicalFolder)
                {
                    if (projectItem.Name == tableFolder.Name)
                    {
                        return projectItem;
                    }
                    CheckSubFolders(pi, tableFolder);
                }
                else
                {
                    continue;
                }                
            }

            return null;
        }
    }

    #endregion

}
