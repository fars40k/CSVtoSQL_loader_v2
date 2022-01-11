using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfStarter.UI.Models
{
    public static class ErrorNotify
    {
        public static AppError AppErrorCurrrent { get; set; }
        private static Action<AppError> OnAppError;

        /// <summary>
        /// Принимает ошибку и передает по ссылке
        /// </summary>
        /// <param name="newErr"></param>
        public static void NewError(AppError newErr)
        {
            ErrorNotify.AppErrorCurrrent = newErr;
            if (OnAppError != null)
            {
                OnAppError.Invoke(newErr);
            }
        }

        /// <summary>
        /// Принимает ссылку на метод для предачи ошибки
        /// </summary>
        /// <param name="action"></param>
        public static void SetUINotifyMethod(Action<AppError> action)
        {
            ErrorNotify.OnAppError = action;
        }

        /// <summary>
        /// Передает пустую ошибку для очистки окна
        /// </summary>
        public static void ClearError()
        {
            OnAppError.Invoke(new AppError("Empty", ""));
        }
    }
}
