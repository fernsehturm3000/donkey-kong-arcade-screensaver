using System;

namespace CleanRoomArcade.Data
{
    [Serializable]
    public sealed class AppSettings
    {
        public bool crtEnabled = true;
        public int shakeIntensity = 70;
        public bool shortStageMode;

        public static AppSettings Defaults() => new AppSettings();

        public void Sanitize()
        {
            shakeIntensity = Math.Max(0, Math.Min(100, shakeIntensity));
        }
    }
}
