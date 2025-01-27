using System.Diagnostics;

namespace MultithreadedTicTacToeGui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            RegisterGlobalExceptionHandlers();
        }

        private void RegisterGlobalExceptionHandlers()
        {
            // AppDomain unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                HandleException(e.ExceptionObject as Exception, "AppDomain.CurrentDomain.UnhandledException");
            };

            // TaskScheduler unobserved task exceptions
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                HandleException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved(); // Marks the exception as observed
            };
        }

        private void HandleException(Exception exception, string source)
        {
            if (exception == null) return;

            // Log the exception (you can replace this with a logger or telemetry)
            Debug.WriteLine($"Unhandled Exception: {exception.Message} (Source: {source})");
            Debug.WriteLine($"StackTrace: {exception.StackTrace}");

            // Optionally, display an alert to the user
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Unexpected Error",
                    $"An unexpected error occurred:\n{exception.Message}",
                    "OK");
            });
        }
    }
}
