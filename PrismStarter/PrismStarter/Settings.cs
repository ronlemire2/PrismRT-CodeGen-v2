using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PrismStarter
{
    public class Settings
    {
        // AppSettings
        public string DbContextAssembly;
        public string DbContextClass;
        public string DELL15z;
        public string EntityStringsTableName;
        public string PrismStarterDatabaseName;
        public string PrismStarterEntities;
        public string PrismStarter;
        public string TargetSolutionName;
        public string TargetSolutionPath;
        public System.Type VisualStudioVersion;
        public int ShortSleep;
        public int MediumSleep;
        public int LongSleep;
        public List<string> ProjectNames;
        public List<string> ProjectPaths;
        public List<string> ZipFileNames;

        public Settings() {
            GetAppSettings();
        }

        public void GetAppSettings() {

            //http://stackoverflow.com/questions/3925308/is-there-a-config-type-file-for-visual-studio-add-in
            string pluginAssemblyPath = Assembly.GetExecutingAssembly().Location;
            System.Configuration.Configuration configuration = System.Configuration.ConfigurationManager.OpenExeConfiguration(pluginAssemblyPath);

            DbContextAssembly = configuration.AppSettings.Settings["DbContextAssembly"].Value;
            DbContextClass = configuration.AppSettings.Settings["DbContextClass"].Value;
            DELL15z = configuration.AppSettings.Settings["DELL15z"].Value;
            EntityStringsTableName = configuration.AppSettings.Settings["EntityStringsTableName"].Value;
            PrismStarterDatabaseName = configuration.AppSettings.Settings["PrismStarterDatabaseName"].Value;
            PrismStarterEntities = configuration.AppSettings.Settings["PrismStarterEntities"].Value;
            PrismStarter = configuration.AppSettings.Settings["PrismStarter"].Value;
            TargetSolutionName = configuration.AppSettings.Settings["TargetSolutionName"].Value;
            TargetSolutionPath = configuration.AppSettings.Settings["TargetSolutionPath"].Value;
            VisualStudioVersion = System.Type.GetTypeFromProgID(configuration.AppSettings.Settings["VisualStudioVersion"].Value);
            ShortSleep = int.Parse(configuration.AppSettings.Settings["ShortSleep"].Value);
            MediumSleep = int.Parse(configuration.AppSettings.Settings["MediumSleep"].Value);
            LongSleep = int.Parse(configuration.AppSettings.Settings["LongSleep"].Value);
            ProjectNames = new List<string>(configuration.AppSettings.Settings["ProjectNames"].Value.Split(new char[] { ';' }));
            ProjectPaths = new List<string>(configuration.AppSettings.Settings["ProjectPaths"].Value.Split(new char[] { ';' }));
            ZipFileNames = new List<string>(configuration.AppSettings.Settings["ZipFileNames"].Value.Split(new char[] { ';' }));
        }
    }
}
