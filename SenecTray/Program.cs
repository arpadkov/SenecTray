namespace SenecTray
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var Window = new MainWindow();
            //Window.Visible = false;
            Application.Run(Window);
            // Window.Start();
            // Application.Run(new MainWindow());
        }
    }
}