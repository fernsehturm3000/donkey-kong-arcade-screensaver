using System.Text.Json;

namespace CleanRoomArcade.Wrapper;

internal sealed class ConfigWindow : Form
{
    private readonly CheckBox crt = new() { Text = "CRT scanlines and vignette", AutoSize = true };
    private readonly TrackBar shake = new() { Minimum = 0, Maximum = 100, TickFrequency = 10, Width = 260 };
    private readonly Label shakeValue = new() { AutoSize = true };
    private readonly CheckBox shortMode = new() { Text = "Short stages (developer test mode)", AutoSize = true };
    private readonly Label status = new() { AutoSize = true, ForeColor = Color.DarkRed };
    private static string SettingsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CleanRoomArcadeScreensaver", "settings.json");

    internal ConfigWindow()
    {
        Text = "Construction Climb Screensaver Settings";
        ClientSize = new Size(390, 260);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        var title = new Label { Text = "Construction Climb", Font = new Font(Font, FontStyle.Bold), AutoSize = true };
        var save = new Button { Text = "Save", AutoSize = true };
        var cancel = new Button { Text = "Cancel", AutoSize = true, DialogResult = DialogResult.Cancel };
        var test = new Button { Text = "Test window", AutoSize = true };
        shake.ValueChanged += (_, _) => shakeValue.Text = $"Shake intensity: {shake.Value}";
        save.Click += (_, _) => SaveAndClose();
        test.Click += (_, _) => TestWindow();
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(18)
        };
        panel.Controls.Add(title);
        panel.Controls.Add(crt);
        panel.Controls.Add(shakeValue);
        panel.Controls.Add(shake);
        panel.Controls.Add(shortMode);
        panel.Controls.Add(new FlowLayoutPanel { AutoSize = true, Controls = { save, cancel, test } });
        panel.Controls.Add(status);
        Controls.Add(panel);
        CancelButton = cancel;
        LoadSettings();
    }

    private void LoadSettings()
    {
        var model = WrapperSettings.Load(SettingsPath);
        crt.Checked = model.crtEnabled;
        shake.Value = Math.Clamp(model.shakeIntensity, 0, 100);
        shortMode.Checked = model.shortStageMode;
    }

    private void SaveAndClose()
    {
        try
        {
            WrapperSettings.Save(SettingsPath, new WrapperSettings { crtEnabled = crt.Checked, shakeIntensity = shake.Value, shortStageMode = shortMode.Checked });
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception exception) { status.Text = exception.Message; }
    }

    private void TestWindow()
    {
        var player = Path.Combine(AppContext.BaseDirectory, Program.PlayerExecutableName);
        if (!File.Exists(player))
        {
            status.Text = $"Build or copy {Program.PlayerExecutableName} beside the wrapper first.";
            return;
        }
        Program.StartPlayer(player, "--preview-child -screen-fullscreen 0 -screen-width 448 -screen-height 512");
    }

    internal sealed class WrapperSettings
    {
        public bool crtEnabled { get; set; } = true;
        public int shakeIntensity { get; set; } = 70;
        public bool shortStageMode { get; set; }

        internal static WrapperSettings Load(string path)
        {
            try { return File.Exists(path) ? JsonSerializer.Deserialize<WrapperSettings>(File.ReadAllText(path)) ?? new WrapperSettings() : new WrapperSettings(); }
            catch { return new WrapperSettings(); }
        }

        internal static void Save(string path, WrapperSettings model)
        {
            var directory = Path.GetDirectoryName(path) ?? throw new IOException("Settings directory is unavailable.");
            Directory.CreateDirectory(directory);
            var temporary = path + ".tmp";
            File.WriteAllText(temporary, JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true }));
            File.Move(temporary, path, true);
        }
    }
}
