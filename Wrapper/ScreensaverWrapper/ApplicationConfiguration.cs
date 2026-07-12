namespace CleanRoomArcade.Wrapper;

internal static class ApplicationConfiguration
{
    internal static void Initialize()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
    }
}
