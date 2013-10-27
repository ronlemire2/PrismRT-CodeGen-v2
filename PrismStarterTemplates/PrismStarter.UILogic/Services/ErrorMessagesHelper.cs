using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace PrismStarter.UILogic.Services
{
    public static class ErrorMessagesHelper
    {
        static ErrorMessagesHelper()
        {
            ResourceLoader = new ResourceLoaderAdapter(new ResourceLoader());    
        }
        public static IResourceLoader ResourceLoader { get; set; }

		public static string CreateAsyncFailedError {
			get { return ResourceLoader.GetString("CreateAsyncFailed"); }
		}

		public static string DeleteAsyncFailedError {
			get { return ResourceLoader.GetString("DeleteAsyncFailed"); }
		}

		public static string ExceptionError {
			get { return ResourceLoader.GetString("Exception"); }
		}

		public static string GetAllAsyncFailedError {
			get { return ResourceLoader.GetString("GetAllAsyncFailed"); }
		}

		public static string GetByIdAsyncFailedError {
			get { return ResourceLoader.GetString("GetByIdAsyncFailed"); }
		}

		public static string HttpRequestExceptionError {
			get { return ResourceLoader.GetString("HttpRequestException"); }
		}

		public static string InvalidFormatError {
			get { return ResourceLoader.GetString("InvalidFormatError"); }
		}

		public static string RegexError {
			get { return ResourceLoader.GetString("RegexError"); }
		}

		public static string RequiredError {
			get { return ResourceLoader.GetString("RequiredError"); }
		}

		public static string UpdateAsyncFailedError {
			get { return ResourceLoader.GetString("UpdateAsyncFailed"); }
		}

    }
}