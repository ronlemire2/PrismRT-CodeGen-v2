using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismTable
{
    public class MyEnvDTE
    {
        private static DTE2 dte2;
        private static Solution2 sln2;

        public static List<string> GetTextEditorProperties()
        {
            System.Type t = Connect.settingsObject.VisualStudioVersion;
            object obj = Activator.CreateInstance(t, true);
            dte2 = (DTE2)obj;
            sln2 = (Solution2)dte2.Solution;

            // http://msdn.microsoft.com/en-us/library/ms165641(v=vs.90).aspx?cs-save-lang=1&cs-lang=csharp#code-snippet-1
            // Nested node doesn't work in C# as done in article with VB. 
            // Tried different separators but have not found the right C# syntax.
            // Other links:
            // http://msdn.microsoft.com/en-us/library/ms165644.aspx
            // http://www.mztools.com/articles/2005/mz2005008.aspx
            // http://dotnet.dzone.com/articles/structure-visual-studio?page=0,1

            EnvDTE.Properties txtEdCS = dte2.get_Properties("TextEditor", "CSharp - Formatting");

            EnvDTE.Property prop = null;
            string msg = null;

            List<string> propList = new List<string>();
            foreach (EnvDTE.Property temp in txtEdCS)
            {
                prop = temp;
                msg = ("Prop Name: " + prop.Name + "  VALUE: " + prop.Value) + "\n";
                propList.Add(msg);
            }

            return propList;

        }
    }
}
