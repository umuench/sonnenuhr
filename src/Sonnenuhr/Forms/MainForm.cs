// ============================================================
// Datei:     MainForm.cs
// Projekt:   Sonnenuhr – Standortspezifischer Wallpaper-Generator
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Hauptformular der Anwendung. Koordiniert die Wallpaper-Generierung,
//   den Timer für automatische Aktualisierungen und den System-Tray.
// ============================================================

using Sonnenuhr.Models;
using Sonnenuhr.Services;

namespace Sonnenuhr.Forms;

/// <summary>
/// Hauptfenster der Sonnenuhr-Anwendung.
/// </summary>
public partial class MainForm : Form
{
    // ── FELDER ─────────────────────────────────────────────────

    private readonly ConfigurationService    _configService;
    private readonly SolarApiService         _solarApiService;
    private AppSettings                      _settings;
    private SolarData?                       _currentSolarData;
    private readonly System.Windows.Forms.Timer _updateTimer;
    private readonly NotifyIcon              _trayIcon;
    private bool                             _isGenerating;

    // ── KONSTRUKTOR ────────────────────────────────────────────

    /// <summary>
    /// Initialisiert das Hauptformular und lädt gespeicherte Einstellungen.
    /// </summary>
    /// <param name="startMinimized">Wenn <c>true</c>, wird das Fenster minimiert gestartet.</param>
    public MainForm(bool startMinimized = false)
    {
        // ── EINGABE ────────────────────────────────────────────
        InitializeComponent();

        // ── VERARBEITUNG ───────────────────────────────────────
        _configService   = new ConfigurationService();
        _solarApiService = new SolarApiService();
        _settings        = _configService.LoadSettings();

        // Aktualisierungs-Timer konfigurieren
        _updateTimer = new System.Windows.Forms.Timer
        {
            Interval = _settings.UpdateIntervalMinutes * 60 * 1000
        };
        _updateTimer.Tick += async (_, _) => await RefreshWallpaperAsync();

        // System-Tray-Icon einrichten
        _trayIcon = CreateTrayIcon();

        ApplySettingsToUi();

        if (startMinimized)
        {
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
        }

        // ── AUSGABE ────────────────────────────────────────────
        // Formular wird von Windows Forms gerendert.
    }

    // ── FORM-LEBENSZYKLUS ──────────────────────────────────────

    /// <inheritdoc/>
    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        _updateTimer.Start();
        await RefreshWallpaperAsync();
    }

    /// <inheritdoc/>
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            Hide();
            _trayIcon.ShowBalloonTip(
                2000,
                "Sonnenuhr",
                "Die Anwendung läuft im Hintergrund weiter.",
                ToolTipIcon.Info);
        }
        else
        {
            base.OnFormClosing(e);
        }
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _updateTimer.Dispose();
            _solarApiService.Dispose();
            _trayIcon.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    // ── WALLPAPER-GENERIERUNG ──────────────────────────────────

    /// <summary>
    /// Ruft aktuelle Sonnendaten ab und generiert das Wallpaper neu.
    /// </summary>
    private async Task RefreshWallpaperAsync()
    {
        // ── EINGABE ────────────────────────────────────────────
        if (_isGenerating) return;
        _isGenerating = true;

        SetStatus("Sonnendaten werden abgerufen …", isWorking: true);

        // ── VERARBEITUNG ───────────────────────────────────────
        try
        {
            _currentSolarData = await _solarApiService.GetTodaySolarDataAsync(_settings.Location);

            if (_currentSolarData is null)
            {
                SetStatus("Fehler: API lieferte keine Daten.", isWorking: false);
                return;
            }

            _settings.LastApiCallUtc = DateTime.UtcNow;
            _configService.SaveSettings(_settings);

            SetStatus("Wallpaper wird generiert …", isWorking: true);

            string outputPath = GetWallpaperOutputPath();
            TimeZoneInfo tz   = _settings.GetTimeZone();

            var generator = new WallpaperGeneratorService(_settings.WallpaperConfig);
            generator.GenerateWallpaper(
                _currentSolarData,
                _settings.Location,
                outputPath,
                DateTime.Now,
                tz);

            WallpaperRegistryService.SetWallpaper(outputPath);

            // UI mit neuen Sonnenzeitdaten aktualisieren
            UpdateSolarDataDisplay(_currentSolarData, tz);

            // ── AUSGABE ────────────────────────────────────────
            SetStatus($"Wallpaper aktualisiert: {DateTime.Now:HH:mm:ss}", isWorking: false);
        }
        catch (HttpRequestException ex)
        {
            SetStatus($"Netzwerkfehler: {ex.Message}", isWorking: false);
        }
        catch (Exception ex)
        {
            SetStatus($"Fehler: {ex.Message}", isWorking: false);
        }
        finally
        {
            _isGenerating = false;
        }
    }

    // ── UI-HILFSMETHODEN ──────────────────────────────────────

    /// <summary>
    /// Überträgt die aktuellen Einstellungen in die UI-Steuerelemente.
    /// </summary>
    private void ApplySettingsToUi()
    {
        // ── EINGABE ────────────────────────────────────────────
        // ── VERARBEITUNG ───────────────────────────────────────
        txtLatitude.Text    = _settings.Location.Latitude.ToString("F4");
        txtLongitude.Text   = _settings.Location.Longitude.ToString("F4");
        txtLocationName.Text = _settings.Location.Name;
        numInterval.Value   = _settings.UpdateIntervalMinutes;
        chkAutostart.Checked = _settings.StartWithWindows;

        // ── AUSGABE ────────────────────────────────────────────
        lblLocationDisplay.Text = _settings.Location.ToString();
    }

    /// <summary>
    /// Zeigt die aktuellen Sonnenzeitdaten in der UI an.
    /// </summary>
    private void UpdateSolarDataDisplay(SolarData data, TimeZoneInfo tz)
    {
        // ── EINGABE ────────────────────────────────────────────
        // ── VERARBEITUNG ───────────────────────────────────────
        if (InvokeRequired) { Invoke(() => UpdateSolarDataDisplay(data, tz)); return; }

        DateTime sunrise  = data.GetLocalSunrise(tz);
        DateTime sunset   = data.GetLocalSunset(tz);
        DateTime solarNoon = TimeZoneInfo.ConvertTimeFromUtc(data.SolarNoon, tz);

        // ── AUSGABE ────────────────────────────────────────────
        lblSunrise.Text   = $"Sonnenaufgang:   {sunrise:HH:mm}";
        lblSunset.Text    = $"Sonnenuntergang: {sunset:HH:mm}";
        lblSolarNoon.Text = $"Sonnenmittag:    {solarNoon:HH:mm}";
        lblDayLength.Text = $"Tageslänge:      {data.DayLength:hh\\:mm} Std.";
    }

    /// <summary>Zeigt eine Statusmeldung in der Statusleiste an (Thread-sicher).</summary>
    private void SetStatus(string message, bool isWorking)
    {
        // ── EINGABE ────────────────────────────────────────────
        // ── VERARBEITUNG ───────────────────────────────────────
        if (InvokeRequired) { Invoke(() => SetStatus(message, isWorking)); return; }

        // ── AUSGABE ────────────────────────────────────────────
        statusLabel.Text       = message;
        progressBar.Visible    = isWorking;
        progressBar.Style      = isWorking ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
    }

    /// <summary>Gibt den Ausgabepfad für die generierte Wallpaper-Datei zurück.</summary>
    private static string GetWallpaperOutputPath()
    {
        // ── VERARBEITUNG ───────────────────────────────────────
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string folder  = Path.Combine(appData, "Sonnenuhr", "wallpapers");
        Directory.CreateDirectory(folder);

        // ── AUSGABE ────────────────────────────────────────────
        return Path.Combine(folder, "sonnenuhr_wallpaper.png");
    }

    // ── EVENT-HANDLER ──────────────────────────────────────────

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        await RefreshWallpaperAsync();
    }

    private void btnSettings_Click(object sender, EventArgs e)
    {
        // ── EINGABE ────────────────────────────────────────────
        using var configForm = new ConfigurationForm(_settings);

        // ── VERARBEITUNG ───────────────────────────────────────
        if (configForm.ShowDialog(this) == DialogResult.OK)
        {
            _settings = configForm.Settings;
            _configService.SaveSettings(_settings);
            _updateTimer.Interval = _settings.UpdateIntervalMinutes * 60 * 1000;
            ApplySettingsToUi();
        }

        // ── AUSGABE ────────────────────────────────────────────
    }

    private void chkAutostart_CheckedChanged(object sender, EventArgs e)
    {
        _settings.StartWithWindows = chkAutostart.Checked;
        WallpaperRegistryService.SetAutostart(chkAutostart.Checked);
        _configService.SaveSettings(_settings);
    }

    private void numInterval_ValueChanged(object sender, EventArgs e)
    {
        _settings.UpdateIntervalMinutes = (int)numInterval.Value;
        _updateTimer.Interval = _settings.UpdateIntervalMinutes * 60 * 1000;
        _configService.SaveSettings(_settings);
    }

    // ── SYSTEM-TRAY ────────────────────────────────────────────

    /// <summary>Erstellt das System-Tray-Icon mit Kontextmenü.</summary>
    private NotifyIcon CreateTrayIcon()
    {
        // ── VERARBEITUNG ───────────────────────────────────────
        var menu = new ContextMenuStrip();
        menu.Items.Add("Öffnen",        null, (_, _) => { Show(); WindowState = FormWindowState.Normal; });
        menu.Items.Add("Jetzt aktualisieren", null, async (_, _) => await RefreshWallpaperAsync());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Beenden",       null, (_, _) => { _trayIcon.Visible = false; Application.Exit(); });

        var icon = new NotifyIcon
        {
            Text            = "Sonnenuhr – Wallpaper-Generator",
            Icon            = SystemIcons.Application,
            ContextMenuStrip = menu,
            Visible         = true
        };
        icon.DoubleClick += (_, _) => { Show(); WindowState = FormWindowState.Normal; };

        // ── AUSGABE ────────────────────────────────────────────
        return icon;
    }
}
