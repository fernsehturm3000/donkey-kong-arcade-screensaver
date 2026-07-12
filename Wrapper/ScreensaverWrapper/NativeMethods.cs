using System.Runtime.InteropServices;

namespace CleanRoomArcade.Wrapper;

internal static class NativeMethods
{
    internal const int GwlStyle = -16;
    internal const nint WsChild = 0x40000000;
    internal const nint WsPopup = unchecked((int)0x80000000);
    internal const nint WsCaption = 0x00C00000;
    internal const nint WsThickFrame = 0x00040000;
    internal const uint SwpNoActivate = 0x0010;
    internal const uint SwpShowWindow = 0x0040;

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
        internal int Width => Math.Max(1, Right - Left);
        internal int Height => Math.Max(1, Bottom - Top);
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsWindow(nint window);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetClientRect(nint window, out Rect rectangle);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern nint SetParent(nint child, nint newParent);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
    private static extern nint GetWindowLongPtr64(nint window, int index);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
    private static extern int GetWindowLong32(nint window, int index);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
    private static extern nint SetWindowLongPtr64(nint window, int index, nint value);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
    private static extern int SetWindowLong32(nint window, int index, int value);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetWindowPos(nint window, nint insertAfter, int x, int y, int width, int height, uint flags);

    internal static nint GetStyle(nint window) => IntPtr.Size == 8 ? GetWindowLongPtr64(window, GwlStyle) : GetWindowLong32(window, GwlStyle);
    internal static void SetStyle(nint window, nint style)
    {
        if (IntPtr.Size == 8) SetWindowLongPtr64(window, GwlStyle, style);
        else SetWindowLong32(window, GwlStyle, style.ToInt32());
    }
}
