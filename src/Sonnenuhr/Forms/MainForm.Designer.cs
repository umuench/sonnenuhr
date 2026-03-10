// ============================================================
// Datei:     MainForm.Designer.cs
// Projekt:   Sonnenuhr – Standortspezifischer Wallpaper-Generator
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Automatisch erstellter Designer-Code für das Hauptformular.
//   Definiert alle UI-Steuerelemente und deren Layout.
// ============================================================

namespace Sonnenuhr.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer? components = null;

    // ── STEUERELEMENTE ─────────────────────────────────────────
    private System.Windows.Forms.Label         lblTitle            = null!;
    private System.Windows.Forms.GroupBox      grpLocation         = null!;
    private System.Windows.Forms.Label         lblLatitudeText     = null!;
    private System.Windows.Forms.Label         lblLongitudeText    = null!;
    private System.Windows.Forms.Label         lblNameText         = null!;
    private System.Windows.Forms.TextBox       txtLatitude         = null!;
    private System.Windows.Forms.TextBox       txtLongitude        = null!;
    private System.Windows.Forms.TextBox       txtLocationName     = null!;
    private System.Windows.Forms.Label         lblLocationDisplay  = null!;
    private System.Windows.Forms.Button        btnCitySearch       = null!;
    private System.Windows.Forms.GroupBox      grpSolarData        = null!;
    private System.Windows.Forms.Label         lblSunrise          = null!;
    private System.Windows.Forms.Label         lblSunset           = null!;
    private System.Windows.Forms.Label         lblSolarNoon        = null!;
    private System.Windows.Forms.Label         lblDayLength        = null!;
    private System.Windows.Forms.GroupBox      grpOptions          = null!;
    private System.Windows.Forms.Label         lblIntervalText     = null!;
    private System.Windows.Forms.NumericUpDown numInterval         = null!;
    private System.Windows.Forms.Label         lblMinText          = null!;
    private System.Windows.Forms.CheckBox      chkAutostart        = null!;
    private System.Windows.Forms.Button        btnRefresh          = null!;
    private System.Windows.Forms.Button        btnSettings         = null!;
    private System.Windows.Forms.StatusStrip   statusStrip         = null!;
    private System.Windows.Forms.ToolStripStatusLabel statusLabel  = null!;
    private System.Windows.Forms.ToolStripProgressBar progressBar  = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();

        // ── FORM ───────────────────────────────────────────────
        Text            = "☀  Sonnenuhr – Wallpaper-Generator";
        Size            = new System.Drawing.Size(520, 560);
        MinimumSize     = new System.Drawing.Size(520, 560);
        MaximumSize     = new System.Drawing.Size(520, 560);
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = System.Drawing.Color.FromArgb(26, 26, 46);
        ForeColor       = System.Drawing.Color.FromArgb(226, 185, 111);
        Font            = new System.Drawing.Font("Segoe UI", 9.5f);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;

        // ── TITEL ──────────────────────────────────────────────
        lblTitle = new System.Windows.Forms.Label
        {
            Text      = "☀  Sonnenuhr  Wallpaper-Generator",
            Font      = new System.Drawing.Font("Segoe UI", 15f, System.Drawing.FontStyle.Bold),
            ForeColor = System.Drawing.Color.FromArgb(226, 185, 111),
            Location  = new System.Drawing.Point(20, 15),
            Size      = new System.Drawing.Size(460, 36),
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        };

        // ── STANDORT-GRUPPE ────────────────────────────────────
        grpLocation = CreateGroupBox("Standort-Konfiguration", 20, 60, 470, 130);

        lblNameText    = CreateLabel("Standortname:",   10, 22, 110);
        lblLatitudeText = CreateLabel("Breitengrad:",   10, 52, 110);
        lblLongitudeText = CreateLabel("Längengrad:",   10, 82, 110);

        txtLocationName = new System.Windows.Forms.TextBox { Location = new System.Drawing.Point(125, 19), Size = new System.Drawing.Size(155, 23) };
        txtLatitude     = new System.Windows.Forms.TextBox { Location = new System.Drawing.Point(125, 49), Size = new System.Drawing.Size(120, 23) };
        txtLongitude    = new System.Windows.Forms.TextBox { Location = new System.Drawing.Point(125, 79), Size = new System.Drawing.Size(120, 23) };

        btnCitySearch = new System.Windows.Forms.Button
        {
            Text      = "🔍  Suchen",
            Location  = new System.Drawing.Point(287, 16),
            Size      = new System.Drawing.Size(165, 28),
            BackColor = System.Drawing.Color.FromArgb(30, 50, 80),
            ForeColor = System.Drawing.Color.FromArgb(180, 210, 255),
            FlatStyle = System.Windows.Forms.FlatStyle.Flat,
            Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Regular),
            Cursor    = System.Windows.Forms.Cursors.Hand
        };
        btnCitySearch.Click += btnCitySearch_Click;

        lblLocationDisplay = new System.Windows.Forms.Label
        {
            Location  = new System.Drawing.Point(10, 108),
            Size      = new System.Drawing.Size(440, 20),
            ForeColor = System.Drawing.Color.FromArgb(200, 200, 200),
            Font      = new System.Drawing.Font("Segoe UI", 8.5f, System.Drawing.FontStyle.Italic)
        };

        StyleTextBox(txtLocationName);
        StyleTextBox(txtLatitude);
        StyleTextBox(txtLongitude);

        grpLocation.Controls.AddRange(new System.Windows.Forms.Control[]
            { lblNameText, lblLatitudeText, lblLongitudeText,
              txtLocationName, btnCitySearch,
              txtLatitude, txtLongitude, lblLocationDisplay });

        // ── SONNENDATEN-GRUPPE ─────────────────────────────────
        grpSolarData = CreateGroupBox("Aktuelle Sonnenzeitdaten", 20, 200, 470, 120);

        lblSunrise   = CreateInfoLabel("Sonnenaufgang:   –:–", 10, 22, 440);
        lblSunset    = CreateInfoLabel("Sonnenuntergang: –:–", 10, 45, 440);
        lblSolarNoon = CreateInfoLabel("Sonnenmittag:    –:–", 10, 68, 440);
        lblDayLength = CreateInfoLabel("Tageslänge:      –:–", 10, 91, 440);

        grpSolarData.Controls.AddRange(new System.Windows.Forms.Control[]
            { lblSunrise, lblSunset, lblSolarNoon, lblDayLength });

        // ── OPTIONEN-GRUPPE ────────────────────────────────────
        grpOptions = CreateGroupBox("Einstellungen", 20, 330, 470, 90);

        lblIntervalText = CreateLabel("Aktualisierung alle:", 10, 25, 155);
        numInterval = new System.Windows.Forms.NumericUpDown
        {
            Location = new System.Drawing.Point(170, 22),
            Size     = new System.Drawing.Size(70, 23),
            Minimum  = 1, Maximum = 1440, Value = 60,
            BackColor = System.Drawing.Color.FromArgb(15, 15, 40),
            ForeColor = System.Drawing.Color.FromArgb(226, 185, 111)
        };
        numInterval.ValueChanged += numInterval_ValueChanged;

        lblMinText = CreateLabel("Minuten", 248, 25, 70);
        chkAutostart = new System.Windows.Forms.CheckBox
        {
            Text      = "Automatisch mit Windows starten",
            Location  = new System.Drawing.Point(10, 55),
            Size      = new System.Drawing.Size(320, 22),
            ForeColor = System.Drawing.Color.FromArgb(200, 200, 200)
        };
        chkAutostart.CheckedChanged += chkAutostart_CheckedChanged;

        grpOptions.Controls.AddRange(new System.Windows.Forms.Control[]
            { lblIntervalText, numInterval, lblMinText, chkAutostart });

        // ── SCHALTFLÄCHEN ──────────────────────────────────────
        btnRefresh = CreateButton("⟳  Jetzt aktualisieren", 20, 435);
        btnRefresh.Click += btnRefresh_Click;

        btnSettings = CreateButton("⚙  Darstellung", 270, 435);
        btnSettings.Click += btnSettings_Click;

        // ── STATUSLEISTE ───────────────────────────────────────
        statusStrip = new System.Windows.Forms.StatusStrip { BackColor = System.Drawing.Color.FromArgb(10, 10, 30) };
        statusLabel = new System.Windows.Forms.ToolStripStatusLabel
        {
            Text      = "Bereit.",
            ForeColor = System.Drawing.Color.FromArgb(200, 200, 200),
            Spring    = true,
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        };
        progressBar = new System.Windows.Forms.ToolStripProgressBar
        {
            Style   = System.Windows.Forms.ProgressBarStyle.Marquee,
            Width   = 100,
            Visible = false
        };
        statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { statusLabel, progressBar });

        // ── FORM-ZUSAMMENSTELLUNG ──────────────────────────────
        Controls.AddRange(new System.Windows.Forms.Control[]
        {
            lblTitle, grpLocation, grpSolarData, grpOptions,
            btnRefresh, btnSettings, statusStrip
        });

        ResumeLayout(false);
        PerformLayout();
    }

    // ── UI-HILFSFABRIKEN ───────────────────────────────────────

    private static System.Windows.Forms.GroupBox CreateGroupBox(string text, int x, int y, int w, int h)
    {
        return new System.Windows.Forms.GroupBox
        {
            Text      = text,
            Location  = new System.Drawing.Point(x, y),
            Size      = new System.Drawing.Size(w, h),
            ForeColor = System.Drawing.Color.FromArgb(226, 185, 111),
            Font      = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Bold)
        };
    }

    private static System.Windows.Forms.Label CreateLabel(string text, int x, int y, int w)
    {
        return new System.Windows.Forms.Label
        {
            Text      = text,
            Location  = new System.Drawing.Point(x, y),
            Size      = new System.Drawing.Size(w, 22),
            ForeColor = System.Drawing.Color.FromArgb(200, 200, 200)
        };
    }

    private static System.Windows.Forms.Label CreateInfoLabel(string text, int x, int y, int w)
    {
        return new System.Windows.Forms.Label
        {
            Text      = text,
            Location  = new System.Drawing.Point(x, y),
            Size      = new System.Drawing.Size(w, 20),
            ForeColor = System.Drawing.Color.FromArgb(200, 220, 200),
            Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Regular)
        };
    }

    private static void StyleTextBox(System.Windows.Forms.TextBox tb)
    {
        tb.BackColor  = System.Drawing.Color.FromArgb(15, 15, 40);
        tb.ForeColor  = System.Drawing.Color.FromArgb(226, 185, 111);
        tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
    }

    private static System.Windows.Forms.Button CreateButton(string text, int x, int y)
    {
        return new System.Windows.Forms.Button
        {
            Text      = text,
            Location  = new System.Drawing.Point(x, y),
            Size      = new System.Drawing.Size(200, 38),
            BackColor = System.Drawing.Color.FromArgb(40, 40, 80),
            ForeColor = System.Drawing.Color.FromArgb(226, 185, 111),
            FlatStyle = System.Windows.Forms.FlatStyle.Flat,
            Font      = new System.Drawing.Font("Segoe UI", 10f, System.Drawing.FontStyle.Bold),
            Cursor    = System.Windows.Forms.Cursors.Hand
        };
    }
}
