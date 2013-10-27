using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrismStarter
{
    public static class Utilities
    {
        static Utilities() {

        }

        #region Strings

        public static string RemoveEndBackSlash(string path) {
            return path.Substring(0, path.Length - 1);
        }

        public static string SearchFileNameAndReplace(string fileName, string oldValue, string newValue) {
            return fileName.Replace(oldValue, newValue);
        }

        #endregion

    }
}
