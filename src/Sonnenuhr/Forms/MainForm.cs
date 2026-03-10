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
    private readonly GeocodingService        _geocodingService;
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
        _configService    = new ConfigurationService();
        _solarApiService  = new SolarApiService();
        _geocodingService = new GeocodingService();
        _settings         = _configService.LoadSettings();

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
            _geocodingService.Dispose();
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

        // Textfeld-Eingaben vor dem API-Aufruf in Settings übernehmen (InvariantCulture für Punkt-Notation)
        ReadCoordinatesFromUi();

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
        // InvariantCulture sicherstellt Punkt als Dezimaltrennzeichen (nicht lokales Komma)
        txtLatitude.Text    = _settings.Location.Latitude.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);
        txtLongitude.Text   = _settings.Location.Longitude.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);
        txtLocationName.Text = _settings.Location.Name;
        numInterval.Value   = _settings.UpdateIntervalMinutes;
        chkAutostart.Checked = _settings.StartWithWindows;

        // ── AUSGABE ────────────────────────────────────────────
        lblLocationDisplay.Text = _settings.Location.ToString();
    }

    /// <summary>
    /// Liest Koordinaten und Standortname aus den Textfeldern und schreibt sie
    /// in <see cref="_settings"/>. Erwartet Punkt als Dezimaltrennzeichen.
    /// Ungültige Eingaben werden stillschweigend ignoriert.
    /// </summary>
    private void ReadCoordinatesFromUi()
    {
        // ── EINGABE ────────────────────────────────────────────
        var culture = System.Globalization.CultureInfo.InvariantCulture;

        // ── VERARBEITUNG ───────────────────────────────────────
        if (double.TryParse(txtLatitude.Text,  System.Globalization.NumberStyles.Float, culture, out double lat))
            _settings.Location.Latitude  = lat;

        if (double.TryParse(txtLongitude.Text, System.Globalization.NumberStyles.Float, culture, out double lon))
            _settings.Location.Longitude = lon;

        if (!string.IsNullOrWhiteSpace(txtLocationName.Text))
            _settings.Location.Name = txtLocationName.Text.Trim();

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
        DateTime solarNoon = data.GetLocalSolarNoon(tz);

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

    /// <summary>
    /// Sucht Städte nach dem eingegebenen Namen über den GeocodingService.
    /// Bei einem Treffer werden Koordinaten direkt übernommen;
    /// bei mehreren Treffern öffnet sich ein Auswahldialog.
    /// </summary>
    private async void btnCitySearch_Click(object sender, EventArgs e)
    {
        // ── EINGABE ────────────────────────────────────────────
        string query = txtLocationName.Text.Trim();
        if (string.IsNullOrWhiteSpace(query))
        {
            MessageBox.Show(
                "Bitte einen Stadtnamen eingeben.",
                "Stadtsuche",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        // ── VERARBEITUNG ───────────────────────────────────────
        btnCitySearch.Enabled = false;
        SetStatus($"Suche nach \"{query}\" ...", isWorking: true);

        try
        {
            var results = await _geocodingService.SearchCityAsync(query);

            GeocodingResult? selected = results.Count switch
            {
                0 => null,
                1 => results[0],
                _ => ChooseFromMultipleResults(results, query)
            };

            if (selected is null)
            {
                if (results.Count == 0)
                    MessageBox.Show(
                        $"Keine Orte für \"{query}\" gefunden.\n\nTipp: Versuchen Sie einen Länderzusatz, z. B. \"Eberbach, Deutschland\".",
                        "Stadtsuche – kein Ergebnis",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                SetStatus("Bereit.", isWorking: false);
                return;
            }

            // ── AUSGABE ────────────────────────────────────────
            ApplyCityResult(selected);
            SetStatus($"Standort übernommen: {selected.ShortName}", isWorking: false);
        }
        catch (OperationCanceledException)
        {
            SetStatus("Stadtsuche abgebrochen.", isWorking: false);
        }
        catch (HttpRequestException ex)
        {
            MessageBox.Show(
                $"Netzwerkfehler bei der Stadtsuche:\n{ex.Message}",
                "Stadtsuche – Fehler",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            SetStatus("Fehler bei der Stadtsuche.", isWorking: false);
        }
        finally
        {
            btnCitySearch.Enabled = true;
        }
    }

    /// <summary>
    /// Öffnet den <see cref="CitySelectionForm"/> und gibt das gewählte Ergebnis zurück.
    /// </summary>
    private GeocodingResult? ChooseFromMultipleResults(
        IReadOnlyList<GeocodingResult> results, string query)
    {
        using var selectionForm = new CitySelectionForm(results, query);
        return selectionForm.ShowDialog(this) == DialogResult.OK
            ? selectionForm.SelectedResult
            : null;
    }

    /// <summary>
    /// Überträgt den gewählten Geocoding-Treffer in die UI-Felder und in die Einstellungen.
    /// </summary>
    private void ApplyCityResult(GeocodingResult result)
    {
        // ── EINGABE ────────────────────────────────────────────
        var culture = System.Globalization.CultureInfo.InvariantCulture;

        // ── VERARBEITUNG ───────────────────────────────────────
        txtLocationName.Text = result.ShortName;
        txtLatitude.Text     = result.Latitude.ToString("F4",  culture);
        txtLongitude.Text    = result.Longitude.ToString("F4", culture);

        _settings.Location.Name      = result.ShortName;
        _settings.Location.Latitude  = result.Latitude;
        _settings.Location.Longitude = result.Longitude;

        _configService.SaveSettings(_settings);

        // ── AUSGABE ────────────────────────────────────────────
        lblLocationDisplay.Text = _settings.Location.ToString();
    }

    // ── SYSTEM-TRAY ────────────────────────────────────────────

    /// <summary>Erstellt das System-Tray-Icon mit Kontextmenü.</summary>
    private NotifyIcon CreateTrayIcon()
    {
        // ── EINGABE ────────────────────────────────────────────
        // Icon aus eingebetteter Ressource laden; Fallback auf Systemicon
        Icon appIcon = LoadEmbeddedIcon() ?? SystemIcons.Application;

        // ── VERARBEITUNG ───────────────────────────────────────
        var menu = new ContextMenuStrip();
        menu.Items.Add("Öffnen",        null, (_, _) => { Show(); WindowState = FormWindowState.Normal; });
        menu.Items.Add("Jetzt aktualisieren", null, async (_, _) => await RefreshWallpaperAsync());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Beenden",       null, (_, _) => { _trayIcon.Visible = false; Application.Exit(); });

        var icon = new NotifyIcon
        {
            Text             = "Sonnenuhr – Wallpaper-Generator",
            Icon             = appIcon,
            ContextMenuStrip = menu,
            Visible          = true
        };
        icon.DoubleClick += (_, _) => { Show(); WindowState = FormWindowState.Normal; };

        // Formular-Icon ebenfalls setzen
        Icon = appIcon;

        // ── AUSGABE ────────────────────────────────────────────
        return icon;
    }

    /// <summary>
    /// Lädt das Anwendungsicon aus den eingebetteten Ressourcen.
    /// </summary>
    /// <returns>Geladenes <see cref="Icon"/> oder <c>null</c> bei Fehler.</returns>
    private static Icon? LoadEmbeddedIcon()
    {
        // ── EINGABE ────────────────────────────────────────────
        string resourceName = "Sonnenuhr.Resources.sonnenuhr.ico";

        // ── VERARBEITUNG ───────────────────────────────────────
        try
        {
            using var stream = System.Reflection.Assembly.GetExecutingAssembly()
                                     .GetManifestResourceStream(resourceName);
            if (stream is null) return null;

            // ── AUSGABE ────────────────────────────────────────
            return new Icon(stream);
        }
        catch
        {
            return null;
        }
    }
}
