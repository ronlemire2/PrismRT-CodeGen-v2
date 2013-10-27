using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PrismStarter.UILogic.Models
{
    public class CrudResult
    {
        public CrudStatusCode CrudStatusCode { get; set; }
        public int NumRowsAffected { get; set; }
        public object Content { get; set; }

        public CrudResult(CrudStatusCode crudStatusCode, int numRowsAffected, object content) {
            CrudStatusCode = crudStatusCode;
            NumRowsAffected = numRowsAffected;
            Content = content;
        }
    }

    public enum CrudStatusCode
    {
        Success,
        Failure
    }
}