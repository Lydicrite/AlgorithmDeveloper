using System.Diagnostics;
using System.Runtime;

namespace AlgorithmDeveloper
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Повышаем приоритет процесса для обеспечения максимальной отзывчивости макросов
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            // Настраиваем режим GC для интерактивного приложения
            GCSettings.LatencyMode = GCLatencyMode.Interactive;

            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}