using System;

namespace CleanRoomArcade.Core
{
    public enum ScreensaverMode { Editor, Fullscreen, Configuration, Preview }

    public readonly struct ScreensaverLaunch
    {
        public ScreensaverLaunch(ScreensaverMode mode, IntPtr previewHandle = default)
        {
            Mode = mode;
            PreviewHandle = previewHandle;
        }

        public ScreensaverMode Mode { get; }
        public IntPtr PreviewHandle { get; }
    }

    public static class ScreensaverArguments
    {
        public static ScreensaverLaunch Parse(string[] args)
        {
            if (args == null || args.Length == 0) return new ScreensaverLaunch(ScreensaverMode.Editor);
            for (var index = 0; index < args.Length; index++)
            {
                var value = (args[index] ?? string.Empty).Trim();
                if (value.Equals("/s", StringComparison.OrdinalIgnoreCase))
                    return new ScreensaverLaunch(ScreensaverMode.Fullscreen);
                if (value.Equals("/c", StringComparison.OrdinalIgnoreCase))
                    return new ScreensaverLaunch(ScreensaverMode.Configuration);
                if (value.StartsWith("/p:", StringComparison.OrdinalIgnoreCase) && TryHandle(value.Substring(3), out var colonHandle))
                    return new ScreensaverLaunch(ScreensaverMode.Preview, colonHandle);
                if (value.Equals("/p", StringComparison.OrdinalIgnoreCase) && index + 1 < args.Length && TryHandle(args[index + 1], out var handle))
                    return new ScreensaverLaunch(ScreensaverMode.Preview, handle);
                if (value.Equals("--preview-child", StringComparison.OrdinalIgnoreCase))
                    return new ScreensaverLaunch(ScreensaverMode.Preview);
            }
            return new ScreensaverLaunch(ScreensaverMode.Editor);
        }

        private static bool TryHandle(string value, out IntPtr handle)
        {
            value = (value ?? string.Empty).Trim();
            var style = System.Globalization.NumberStyles.Integer;
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
                style = System.Globalization.NumberStyles.HexNumber;
            }
            if (long.TryParse(value, style, System.Globalization.CultureInfo.InvariantCulture, out var parsed) && parsed > 0)
            {
                handle = new IntPtr(parsed);
                return true;
            }
            handle = IntPtr.Zero;
            return false;
        }
    }
}
