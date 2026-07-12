using System.ComponentModel;
using System.Diagnostics;

namespace CleanRoomArcade.Wrapper;

internal static class PreviewHost
{
    internal static void Run(string player, nint host)
    {
        if (host == 0 || !NativeMethods.IsWindow(host))
        {
            MessageBox.Show("The preview window handle is missing or invalid.", "Arcade Screensaver", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!NativeMethods.GetClientRect(host, out var bounds))
        {
            MessageBox.Show(new Win32Exception().Message, "Unable to read preview window", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        var arguments = $"--preview-child -screen-fullscreen 0 -screen-width {bounds.Width} -screen-height {bounds.Height} -popupwindow";
        using var process = Program.StartPlayer(player, arguments);
        if (process == null) return;
        try
        {
            var child = WaitForWindow(process, TimeSpan.FromSeconds(10));
            if (child == 0) throw new TimeoutException("The Unity preview window did not appear within 10 seconds.");
            NativeMethods.SetParent(child, host);
            var style = NativeMethods.GetStyle(child);
            style = (style | NativeMethods.WsChild) & ~NativeMethods.WsPopup & ~NativeMethods.WsCaption & ~NativeMethods.WsThickFrame;
            NativeMethods.SetStyle(child, style);
            Resize(child, host);

            var previousWidth = 0;
            var previousHeight = 0;
            while (!process.HasExited && NativeMethods.IsWindow(host))
            {
                Application.DoEvents();
                if (NativeMethods.GetClientRect(host, out bounds) && (bounds.Width != previousWidth || bounds.Height != previousHeight))
                {
                    Resize(child, host);
                    previousWidth = bounds.Width;
                    previousHeight = bounds.Height;
                }
                Thread.Sleep(50);
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, "Arcade preview failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!process.HasExited)
            {
                process.CloseMainWindow();
                if (!process.WaitForExit(750)) process.Kill(true);
            }
        }
    }

    private static nint WaitForWindow(Process process, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline && !process.HasExited)
        {
            process.Refresh();
            if (process.MainWindowHandle != 0) return process.MainWindowHandle;
            Thread.Sleep(50);
        }
        return 0;
    }

    private static void Resize(nint child, nint host)
    {
        if (!NativeMethods.GetClientRect(host, out var bounds)) return;
        NativeMethods.SetWindowPos(child, 0, 0, 0, bounds.Width, bounds.Height, NativeMethods.SwpNoActivate | NativeMethods.SwpShowWindow);
    }
}
