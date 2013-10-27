using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismTable
{
    public static class MessageService
    {
        static MessageService()
        {

        }

        public static void WriteMessage(string message)
        {
            CodeGenMessage = message;
        }

        #region MessageGenerated Event

        public delegate void MessageGeneratedEventHandler(object sender, MessageGeneratedEventArgs args);
        public static event MessageGeneratedEventHandler MessageGenerated;
        private static string _codeGenMessage;
        public static string CodeGenMessage
        {
            set
            {
                _codeGenMessage = value;
                OnMessageGenerated();
            }
            get
            {
                return _codeGenMessage;
            }
        }
        public static void OnMessageGenerated()
        {
            if (MessageGenerated != null)
            {
                MessageGenerated(null, new MessageGeneratedEventArgs(CodeGenMessage));
            }
        }
        public class MessageGeneratedEventArgs : EventArgs
        {
            private string _codeGenMessage;
            public MessageGeneratedEventArgs(string codeGenMessage)
            {
                _codeGenMessage = codeGenMessage;
            }
            public string GetCodeGenMessage { get { return _codeGenMessage; } }

        }

        #endregion
    }
}
