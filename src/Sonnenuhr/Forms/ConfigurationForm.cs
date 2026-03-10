// ============================================================
// Datei:     ConfigurationForm.cs
// Projekt:   Sonnenuhr – Standortspezifischer Wallpaper-Generator
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Konfigurationsdialog für die visuelle Gestaltung des Wallpapers.
//   Ermöglicht die Anpassung von Farben, Schriften und Anzeigeoptionen.
// ============================================================

using System.Drawing;
using Sonnenuhr.Models;

namespace Sonnenuhr.Forms;

/// <summary>
/// Modaler Dialog zur Konfiguration der Wallpaper-Darstellung.
/// </summary>
public class ConfigurationForm : Form
{
    // ── FELDER ─────────────────────────────────────────────────

    private AppSettings _settings;

    // ── STEUERELEMENTE ─────────────────────────────────────────
    private readonly System.Windows.Forms.Button        btnBackground  = new();
    private readonly System.Windows.Forms.Button        btnPrimary     = new();
    private readonly System.Windows.Forms.Button        btnAccent      = new();
    private readonly System.Windows.Forms.Panel         pnlBackground  = new();
    private readonly System.Windows.Forms.Panel         pnlPrimary     = new();
    private readonly System.Windows.Forms.Panel         pnlAccent      = new();
    private readonly System.Windows.Forms.ComboBox      cmbFont        = new();
    private readonly System.Windows.Forms.NumericUpDown numFontSize    = new();
    private readonly System.Windows.Forms.CheckBox      chkCurrentTime = new();
    private readonly System.Windows.Forms.CheckBox      chkLocation    = new();
    private readonly System.Windows.Forms.CheckBox      chkSunTimes    = new();
    private readonly System.Windows.Forms.CheckBox      chkTwilight    = new();
    private readonly System.Windows.Forms.CheckBox      chkTimeMarker  = new();
    private readonly System.Windows.Forms.CheckBox      chkRoman       = new();
    private readonly System.Windows.Forms.Button        btnOk          = new();
    private readonly System.Windows.Forms.Button        btnCancel      = new();

    // ── EIGENSCHAFTEN ──────────────────────────────────────────

    /// <summary>Gibt die bearbeiteten Einstellungen zurück (nach OK).</summary>
    public AppSettings Settings => _settings;

    // ── KONSTRUKTOR ────────────────────────────────────────────

    /// <summary>
    /// Öffnet den Konfigurationsdialog mit den aktuellen Einstellungen.
    /// </summary>
    /// <param name="settings">Aktuelle Anwendungseinstellungen.</param>
    public ConfigurationForm(AppSettings settings)
    {
        // ── EINGABE ────────────────────────────────────────────
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        // ── VERARBEITUNG ───────────────────────────────────────
        InitializeLayout();
        LoadSettingsIntoUi();

        // ── AUSGABE ────────────────────────────────────────────
        // Dialog wird von Windows Forms gerendert.
    }

    // ── LAYOUT ────────────────────────────────────────────────

    private void InitializeLayout()
    {
        Text          = "⚙  Darstellungs-Einstellungen";
        Size          = new System.Drawing.Size(420, 460);
        MinimumSize   = Size;
        MaximumSize   = Size;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox   = false;
        MinimizeBox   = false;
        BackColor     = System.Drawing.Color.FromArgb(26, 26, 46);
        ForeColor     = System.Drawing.Color.FromArgb(226, 185, 111);
        Font          = new System.Drawing.Font("Segoe UI", 9.5f);

        int y = 15;

        // Farb-Abschnitt
        AddSectionLabel("Farbschema", ref y);
        AddColorRow("Hintergrundfarbe:",  btnBackground, pnlBackground, ref y);
        AddColorRow("Primärfarbe:",        btnPrimary,    pnlPrimary,   ref y);
        AddColorRow("Akzentfarbe:",        btnAccent,     pnlAccent,    ref y);

        // Schrift-Abschnitt
        y += 8;
        AddSectionLabel("Typografie", ref y);

        var lblFont = AddLabel("Schriftart:", 20, y, 120);
        cmbFont.Location = new System.Drawing.Point(145, y);
        cmbFont.Size     = new System.Drawing.Size(180, 23);
        cmbFont.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        cmbFont.BackColor = System.Drawing.Color.FromArgb(15, 15, 40);
        cmbFont.ForeColor = System.Drawing.Color.FromArgb(226, 185, 111);
        foreach (var fam in new[] { "Segoe UI", "Arial", "Calibri", "Times New Roman", "Consolas" })
            cmbFont.Items.Add(fam);
        Controls.Add(cmbFont);
        y += 30;

        var lblSize = AddLabel("Schriftgröße:", 20, y, 120);
        numFontSize.Location = new System.Drawing.Point(145, y);
        numFontSize.Size     = new System.Drawing.Size(70, 23);
        numFontSize.Minimum  = 8; numFontSize.Maximum = 24; numFontSize.DecimalPlaces = 1;
        numFontSize.BackColor = System.Drawing.Color.FromArgb(15, 15, 40);
        numFontSize.ForeColor = System.Drawing.Color.FromArgb(226, 185, 111);
        Controls.Add(numFontSize);
        y += 35;

        // Anzeigeoptionen
        AddSectionLabel("Anzeigeoptionen", ref y);
        AddCheckBox(chkCurrentTime, "Aktuelle Uhrzeit anzeigen",        ref y);
        AddCheckBox(chkLocation,    "Standortname anzeigen",            ref y);
        AddCheckBox(chkSunTimes,    "Sonnenauf/-untergang anzeigen",     ref y);
        AddCheckBox(chkTwilight,    "Dämmerungslinien anzeigen",         ref y);
        AddCheckBox(chkTimeMarker,  "Aktuellen Stundenzeiger anzeigen",  ref y);
        AddCheckBox(chkRoman,       "Römische Ziffern verwenden",        ref y);

        // Schaltflächen – readonly-Felder nur konfigurieren, nicht neu erstellen
        y = ClientSize.Height - 52;
        btnOk.Text         = "✓  Übernehmen";
        btnOk.Location     = new System.Drawing.Point(20, y);
        btnOk.Size         = new System.Drawing.Size(170, 36);
        btnOk.BackColor    = System.Drawing.Color.FromArgb(40, 40, 80);
        btnOk.ForeColor    = System.Drawing.Color.FromArgb(226, 185, 111);
        btnOk.FlatStyle    = System.Windows.Forms.FlatStyle.Flat;
        btnOk.Font         = new System.Drawing.Font("Segoe UI", 10f, System.Drawing.FontStyle.Bold);
        btnOk.DialogResult = DialogResult.OK;
        btnOk.Click       += BtnOk_Click;

        btnCancel.Text         = "✕  Abbrechen";
        btnCancel.Location     = new System.Drawing.Point(210, y);
        btnCancel.Size         = new System.Drawing.Size(170, 36);
        btnCancel.BackColor    = System.Drawing.Color.FromArgb(40, 40, 80);
        btnCancel.ForeColor    = System.Drawing.Color.FromArgb(200, 120, 120);
        btnCancel.FlatStyle    = System.Windows.Forms.FlatStyle.Flat;
        btnCancel.Font         = new System.Drawing.Font("Segoe UI", 10f);
        btnCancel.DialogResult = DialogResult.Cancel;

        Controls.AddRange(new System.Windows.Forms.Control[] { btnOk, btnCancel });
        AcceptButton = btnOk;
        CancelButton = btnCancel;
    }

    // ── EVENT-HANDLER ──────────────────────────────────────────

    private void BtnOk_Click(object? sender, EventArgs e)
    {
        // ── EINGABE ────────────────────────────────────────────
        // ── VERARBEITUNG ───────────────────────────────────────
        _settings.WallpaperConfig.BackgroundColorArgb = pnlBackground.BackColor.ToArgb();
        _settings.WallpaperConfig.PrimaryColorArgb    = pnlPrimary.BackColor.ToArgb();
        _settings.WallpaperConfig.AccentColorArgb     = pnlAccent.BackColor.ToArgb();
        _settings.WallpaperConfig.FontFamily          = cmbFont.SelectedItem?.ToString() ?? "Segoe UI";
        _settings.WallpaperConfig.FontSizeBase        = (float)numFontSize.Value;
        _settings.WallpaperConfig.ShowCurrentTime     = chkCurrentTime.Checked;
        _settings.WallpaperConfig.ShowLocationName    = chkLocation.Checked;
        _settings.WallpaperConfig.ShowSunriseSunset   = chkSunTimes.Checked;
        _settings.WallpaperConfig.ShowTwilightLines   = chkTwilight.Checked;
        _settings.WallpaperConfig.ShowCurrentHourMarker = chkTimeMarker.Checked;
        _settings.WallpaperConfig.UseRomanNumerals    = chkRoman.Checked;

        // ── AUSGABE ────────────────────────────────────────────
        DialogResult = DialogResult.OK;
    }

    private void LoadSettingsIntoUi()
    {
        // ── EINGABE ────────────────────────────────────────────
        var cfg = _settings.WallpaperConfig;

        // ── VERARBEITUNG & AUSGABE ─────────────────────────────
        pnlBackground.BackColor = cfg.BackgroundColor;
        pnlPrimary.BackColor    = cfg.PrimaryColor;
        pnlAccent.BackColor     = cfg.AccentColor;

        int fontIndex = cmbFont.Items.IndexOf(cfg.FontFamily);
        cmbFont.SelectedIndex = fontIndex >= 0 ? fontIndex : 0;
        numFontSize.Value = (decimal)cfg.FontSizeBase;

        chkCurrentTime.Checked  = cfg.ShowCurrentTime;
        chkLocation.Checked     = cfg.ShowLocationName;
        chkSunTimes.Checked     = cfg.ShowSunriseSunset;
        chkTwilight.Checked     = cfg.ShowTwilightLines;
        chkTimeMarker.Checked   = cfg.ShowCurrentHourMarker;
        chkRoman.Checked        = cfg.UseRomanNumerals;
    }

    // ── UI-HILFSFABRIKEN ───────────────────────────────────────

    private void AddSectionLabel(string text, ref int y)
    {
        var lbl = new System.Windows.Forms.Label
        {
            Text      = text,
            Location  = new System.Drawing.Point(20, y),
            Size      = new System.Drawing.Size(360, 20),
            Font      = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Bold),
            ForeColor = System.Drawing.Color.FromArgb(226, 185, 111)
        };
        Controls.Add(lbl);
        y += 24;
    }

    private System.Windows.Forms.Label AddLabel(string text, int x, int y, int w)
    {
        var lbl = new System.Windows.Forms.Label
        {
            Text      = text,
            Location  = new System.Drawing.Point(x, y),
            Size      = new System.Drawing.Size(w, 22),
            ForeColor = System.Drawing.Color.FromArgb(200, 200, 200)
        };
        Controls.Add(lbl);
        return lbl;
    }

    private void AddColorRow(string labelText, System.Windows.Forms.Button btn, System.Windows.Forms.Panel panel, ref int y)
    {
        AddLabel(labelText, 20, y, 140);

        panel.Location  = new System.Drawing.Point(165, y);
        panel.Size      = new System.Drawing.Size(50, 22);
        panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

        btn.Text      = "Wählen…";
        btn.Location  = new System.Drawing.Point(225, y - 1);
        btn.Size      = new System.Drawing.Size(90, 24);
        btn.BackColor = System.Drawing.Color.FromArgb(40, 40, 80);
        btn.ForeColor = System.Drawing.Color.FromArgb(226, 185, 111);
        btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btn.Tag       = panel;
        btn.Click    += ColorBtn_Click;

        Controls.Add(panel);
        Controls.Add(btn);
        y += 30;
    }

    private void AddCheckBox(System.Windows.Forms.CheckBox cb, string text, ref int y)
    {
        cb.Text      = text;
        cb.Location  = new System.Drawing.Point(20, y);
        cb.Size      = new System.Drawing.Size(360, 22);
        cb.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
        Controls.Add(cb);
        y += 26;
    }

    private void ColorBtn_Click(object? sender, EventArgs e)
    {
        if (sender is System.Windows.Forms.Button btn && btn.Tag is System.Windows.Forms.Panel panel)
        {
            using var dlg = new System.Windows.Forms.ColorDialog { Color = panel.BackColor, FullOpen = true };
            if (dlg.ShowDialog(this) == DialogResult.OK)
                panel.BackColor = dlg.Color;
        }
    }
}
