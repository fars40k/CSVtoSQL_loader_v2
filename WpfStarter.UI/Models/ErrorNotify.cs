using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStarter.UI.Models
{
    public class ErrorNotify
    {
        public static string AppErrorCurrrent { get; set; }
        private static Action<string> OnAppError;


        /// <summary>
        /// Accepts delegate and saves it as path to publish error strings
        /// </summary>
        public static void SetUINotifyMethod(Action<string> action)
        {
            ErrorNotify.OnAppError = action;
        }

        /// <summary>
        /// Publish parametr string as new error
        /// </summary>
        public static void NewError(string newError)
        {
            ErrorNotify.AppErrorCurrrent = newError;
            if (OnAppError != null)
            {
                OnAppError.Invoke(newError);
            }
        }

        /// <summary>
        /// Sending empty error string
        /// </summary>
        public static void ClearError()
        {
            ErrorNotify.AppErrorCurrrent = "";
            if (OnAppError != null)
            {
                OnAppError.Invoke("");
            }
        }
    }
}
