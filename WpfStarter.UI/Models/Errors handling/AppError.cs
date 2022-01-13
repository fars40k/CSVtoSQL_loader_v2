namespace WpfStarter.UI.Models
{
    /// <summary>
    /// Класс контейнер для ошибки с локализованной строкой
    /// </summary>
    public class AppError
    {
        public string? Code { get; set; }
        public string? Message { get; set; }

        public AppError(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
