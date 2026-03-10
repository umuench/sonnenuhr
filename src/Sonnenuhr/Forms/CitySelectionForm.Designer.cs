// ============================================================
// Datei:     CitySelectionForm.Designer.cs
// Projekt:   Sonnenuhr – Standortspezifischer Wallpaper-Generator
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Automatisch erstellter Designer-Code für den Stadtauswahl-Dialog.
// ============================================================

namespace Sonnenuhr.Forms;

partial class CitySelectionForm
{
    private System.ComponentModel.IContainer? components = null;

    // ── STEUERELEMENTE ─────────────────────────────────────────
    private System.Windows.Forms.Label   lblInfo    = null!;
    private System.Windows.Forms.ListBox lstResults = null!;
    private System.Windows.Forms.Label   lblCoords  = null!;
    private System.Windows.Forms.Button  btnOk      = null!;
    private System.Windows.Forms.Button  btnCancel  = null!;

    private void InitializeComponent()
    {
        SuspendLayout();

        // ── FORM ───────────────────────────────────────────────
        Text            = "☀  Sonnenuhr – Stadtauswahl";
        Size            = new System.Drawing.Size(600, 430);
        MinimumSize     = new System.Drawing.Size(600, 430);
        MaximumSize     = new System.Drawing.Size(600, 430);
        StartPosition   = FormStartPosition.CenterParent;
        BackColor       = System.Drawing.Color.FromArgb(26, 26, 46);
        ForeColor       = System.Drawing.Color.FromArgb(226, 185, 111);
        Font            = new System.Drawing.Font("Segoe UI", 9.5f);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        MinimizeBox     = false;
        AcceptButton    = btnOk;
        CancelButton    = btnCancel;

        // ── INFO-LABEL ─────────────────────────────────────────
        lblInfo = new System.Windows.Forms.Label
        {
            Location  = new System.Drawing.Point(15, 15),
            Size      = new System.Drawing.Size(560, 36),
            ForeColor = System.Drawing.Color.FromArgb(226, 185, 111),
            Font      = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Bold),
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        };

        // ── ERGEBNIS-LISTE ─────────────────────────────────────
        lstResults = new System.Windows.Forms.ListBox
        {
            Location         = new System.Drawing.Point(15, 58),
            Size             = new System.Drawing.Size(560, 270),
            BackColor        = System.Drawing.Color.FromArgb(15, 15, 40),
            ForeColor        = System.Drawing.Color.FromArgb(220, 220, 220),
            BorderStyle      = System.Windows.Forms.BorderStyle.FixedSingle,
            Font             = new System.Drawing.Font("Segoe UI", 9.5f),
            SelectionMode    = System.Windows.Forms.SelectionMode.One,
            IntegralHeight   = false,
            ScrollAlwaysVisible = false
        };
        lstResults.SelectedIndexChanged += lstResults_SelectedIndexChanged;
        lstResults.DoubleClick          += lstResults_DoubleClick;

        // ── KOORDINATEN-ANZEIGE ────────────────────────────────
        lblCoords = new System.Windows.Forms.Label
        {
            Location  = new System.Drawing.Point(15, 338),
            Size      = new System.Drawing.Size(560, 20),
            ForeColor = System.Drawing.Color.FromArgb(150, 200, 150),
            Font      = new System.Drawing.Font("Segoe UI", 8.5f, System.Drawing.FontStyle.Italic),
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        };

        // ── SCHALTFLÄCHEN ──────────────────────────────────────
        btnOk = new System.Windows.Forms.Button
        {
            Text      = "✓  Auswählen",
            Location  = new System.Drawing.Point(350, 360),
            Size      = new System.Drawing.Size(105, 32),
            BackColor = System.Drawing.Color.FromArgb(40, 80, 40),
            ForeColor = System.Drawing.Color.FromArgb(150, 230, 150),
            FlatStyle = System.Windows.Forms.FlatStyle.Flat,
            Font      = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Bold),
            Cursor    = System.Windows.Forms.Cursors.Hand
        };
        btnOk.Click += btnOk_Click;

        btnCancel = new System.Windows.Forms.Button
        {
            Text      = "✕  Abbrechen",
            Location  = new System.Drawing.Point(465, 360),
            Size      = new System.Drawing.Size(110, 32),
            BackColor = System.Drawing.Color.FromArgb(60, 30, 30),
            ForeColor = System.Drawing.Color.FromArgb(230, 150, 150),
            FlatStyle = System.Windows.Forms.FlatStyle.Flat,
            Font      = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Bold),
            Cursor    = System.Windows.Forms.Cursors.Hand
        };
        btnCancel.Click += btnCancel_Click;

        // ── FORM-ZUSAMMENSTELLUNG ──────────────────────────────
        Controls.AddRange(new System.Windows.Forms.Control[]
        {
            lblInfo, lstResults, lblCoords, btnOk, btnCancel
        });

        ResumeLayout(false);
        PerformLayout();
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
}
