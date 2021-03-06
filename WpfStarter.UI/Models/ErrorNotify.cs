using System;

namespace WpfStarter.UI.Models
{
    public class ErrorNotify
    {
        public static string AppErrorCurrrent { get; set; }
        private static Action<string> _OnAppError;

        /// <summary>
        /// Accepts delegate and saves it as path to publish error strings
        /// </summary>
        public static void SetUINotifyMethod(Action<string> action)
        {
            ErrorNotify._OnAppError = action;
        }

        /// <summary>
        /// Publish argument string as new error
        /// </summary>
        public static void NewError(string newError)
        {
            ErrorNotify.AppErrorCurrrent = newError;
            if (_OnAppError != null)
            {
                _OnAppError.Invoke(newError);
            }
        }

        /// <summary>
        /// Sending empty error string
        /// </summary>
        public static void ClearError()
        {
            ErrorNotify.AppErrorCurrrent = "";
            if (_OnAppError != null)
            {
                _OnAppError.Invoke("");
            }
        }
    }
}
