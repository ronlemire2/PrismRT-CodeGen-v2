using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PrismTable
{
    public class Settings
    {
        // AppSettings
        public string DbContextAssembly;
        public string DbContextClass;
        public string DELL15z;
        public string EntityRulesTableName;
        public string EntityStringsTableName;
        public string EntityNameParameterPlural;
        public string EntityNameParameter;
        public string EntityNamePlural;
        public string EntityName;
        public bool RemoveEntity;
        public string PrismTableDatabaseName;
        public string PrismTableEntities;
        public string PrismTable;
        public string TableSolutionName;
        public string TableSolutionPath;
        public string DeleteTableSolutionPath;
        public string TargetSolutionName;
        public string TargetSolutionPath;
        public System.Type VisualStudioVersion;
        public int ShortSleep;
        public int MediumSleep;
        public int LongSleep;
        public List<string> TableProjectNames;
        public List<string> TableProjectPaths;
        public List<string> TargetProjectNames;
        public List<string> TargetProjectPaths;
        public List<string> ZipFileNames;

        public Settings()
        {
            GetAppSettings();
        }

        public void GetAppSettings()
        {
            //http://stackoverflow.com/questions/3925308/is-there-a-config-type-file-for-visual-studio-add-in
            string pluginAssemblyPath = Assembly.GetExecutingAssembly().Location;
            System.Configuration.Configuration configuration = System.Configuration.ConfigurationManager.OpenExeConfiguration(pluginAssemblyPath);

            DbContextAssembly = configuration.AppSettings.Settings["DbContextAssembly"].Value;
            DbContextClass = configuration.AppSettings.Settings["DbContextClass"].Value;
            DELL15z = configuration.AppSettings.Settings["DELL15z"].Value;
            EntityRulesTableName = configuration.AppSettings.Settings["EntityRulesTableName"].Value;
            EntityStringsTableName = configuration.AppSettings.Settings["EntityStringsTableName"].Value;
            EntityNameParameterPlural = configuration.AppSettings.Settings["EntityNameParameterPlural"].Value;
            EntityNameParameter = configuration.AppSettings.Settings["EntityNameParameter"].Value;
            EntityNamePlural = configuration.AppSettings.Settings["EntityNamePlural"].Value;
            EntityName = configuration.AppSettings.Settings["EntityName"].Value;
            RemoveEntity = bool.Parse(configuration.AppSettings.Settings["RemoveEntity"].Value);
            LongSleep = int.Parse(configuration.AppSettings.Settings["LongSleep"].Value);
            MediumSleep = int.Parse(configuration.AppSettings.Settings["MediumSleep"].Value);
            PrismTableDatabaseName = configuration.AppSettings.Settings["PrismTableDatabaseName"].Value;
            PrismTableEntities = configuration.AppSettings.Settings["PrismTableEntities"].Value;
            PrismTable = configuration.AppSettings.Settings["PrismTable"].Value;
            ShortSleep = int.Parse(configuration.AppSettings.Settings["ShortSleep"].Value);
            TableSolutionName = configuration.AppSettings.Settings["TableSolutionName"].Value;
            TableSolutionPath = configuration.AppSettings.Settings["TableSolutionPath"].Value;
            DeleteTableSolutionPath = configuration.AppSettings.Settings["DeleteTableSolutionPath"].Value;
            TableProjectNames = new List<string>(configuration.AppSettings.Settings["TableProjectNames"].Value.Split(new char[] { ';' }));
            TableProjectPaths = new List<string>(configuration.AppSettings.Settings["TableProjectPaths"].Value.Split(new char[] { ';' }));
            TargetSolutionName = configuration.AppSettings.Settings["TargetSolutionName"].Value;
            TargetSolutionPath = configuration.AppSettings.Settings["TargetSolutionPath"].Value;
            TargetProjectNames = new List<string>(configuration.AppSettings.Settings["TargetProjectNames"].Value.Split(new char[] { ';' }));
            TargetProjectPaths = new List<string>(configuration.AppSettings.Settings["TargetProjectPaths"].Value.Split(new char[] { ';' }));
            VisualStudioVersion = System.Type.GetTypeFromProgID(configuration.AppSettings.Settings["VisualStudioVersion"].Value);
            ZipFileNames = new List<string>(configuration.AppSettings.Settings["ZipFileNames"].Value.Split(new char[] { ';' }));
        }
    }
}
