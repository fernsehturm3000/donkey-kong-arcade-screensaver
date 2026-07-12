using System.Diagnostics;

namespace CleanRoomArcade.Wrapper;

internal enum LaunchMode { Fullscreen, Configuration, Preview }

internal readonly record struct LaunchRequest(LaunchMode Mode, nint PreviewHandle);

internal static class Program
{
    internal const string PlayerExecutableName = "DKArcadePlayer.exe";

    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        var request = Parse(args);
        if (request.Mode == LaunchMode.Configuration)
        {
            Application.Run(new ConfigWindow());
            return;
        }
        var player = Path.Combine(AppContext.BaseDirectory, PlayerExecutableName);
        if (!File.Exists(player))
        {
            MessageBox.Show($"The Unity player was not found:\n{player}\n\nBuild or copy {PlayerExecutableName} beside this screensaver.",
                "Arcade Screensaver", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        if (request.Mode == LaunchMode.Preview)
        {
            PreviewHost.Run(player, request.PreviewHandle);
            return;
        }
        using var process = StartPlayer(player, "/s");
        process?.WaitForExit();
    }

    internal static LaunchRequest Parse(string[] args)
    {
        if (args.Length == 0) return new LaunchRequest(LaunchMode.Configuration, 0);
        var first = args[0].Trim();
        if (first.Equals("/s", StringComparison.OrdinalIgnoreCase)) return new LaunchRequest(LaunchMode.Fullscreen, 0);
        if (first.StartsWith("/p:", StringComparison.OrdinalIgnoreCase) && TryHandle(first[3..], out var attached)) return new LaunchRequest(LaunchMode.Preview, attached);
        if (first.Equals("/p", StringComparison.OrdinalIgnoreCase) && args.Length > 1 && TryHandle(args[1], out attached)) return new LaunchRequest(LaunchMode.Preview, attached);
        return new LaunchRequest(LaunchMode.Configuration, 0);
    }

    private static bool TryHandle(string value, out nint handle)
    {
        value = value.Trim();
        var hex = value.StartsWith("0x", StringComparison.OrdinalIgnoreCase);
        if (hex) value = value[2..];
        if (long.TryParse(value, hex ? System.Globalization.NumberStyles.HexNumber : System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture, out var parsed) && parsed > 0)
        {
            handle = (nint)parsed;
            return true;
        }
        handle = 0;
        return false;
    }

    internal static Process? StartPlayer(string player, string arguments)
    {
        return Process.Start(new ProcessStartInfo(player, arguments)
        {
            UseShellExecute = false,
            WorkingDirectory = AppContext.BaseDirectory
        });
    }
}
