// ============================================================
// Datei:     CitySelectionForm.cs
// Projekt:   Sonnenuhr – Standortspezifischer Wallpaper-Generator
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Auswahldialog, der dem Benutzer bei mehreren Treffern einer
//   Stadtsuche eine strukturierte Liste zur Auswahl anzeigt.
//   Gibt das ausgewählte GeocodingResult über SelectedResult zurück.
// ============================================================

using Sonnenuhr.Models;

namespace Sonnenuhr.Forms;

/// <summary>
/// Modaler Dialog zur Auswahl eines Suchergebnisses aus einer Liste
/// von Geocoding-Treffern.
/// </summary>
public sealed partial class CitySelectionForm : Form
{
    // ── EIGENSCHAFTEN ──────────────────────────────────────────

    /// <summary>
    /// Das vom Benutzer gewählte Geocoding-Ergebnis oder <c>null</c>,
    /// wenn der Dialog abgebrochen wurde.
    /// </summary>
    public GeocodingResult? SelectedResult { get; private set; }

    // ── KONSTRUKTOR ────────────────────────────────────────────

    /// <summary>
    /// Initialisiert den Auswahldialog mit der übergebenen Trefferliste.
    /// </summary>
    /// <param name="results">Liste der gefundenen Orte (darf nicht leer sein).</param>
    /// <param name="searchQuery">Der ursprüngliche Suchbegriff (für den Titel).</param>
    public CitySelectionForm(IReadOnlyList<GeocodingResult> results, string searchQuery)
    {
        // ── EINGABE ────────────────────────────────────────────
        ArgumentNullException.ThrowIfNull(results);

        // ── VERARBEITUNG ───────────────────────────────────────
        InitializeComponent();
        PopulateList(results, searchQuery);
    }

    // ── PRIVATE METHODEN ──────────────────────────────────────

    /// <summary>Befüllt die Liste mit den Suchergebnissen.</summary>
    private void PopulateList(IReadOnlyList<GeocodingResult> results, string searchQuery)
    {
        // ── EINGABE ────────────────────────────────────────────
        lblInfo.Text = $"{results.Count} Treffer für \"{searchQuery}\" – bitte einen Ort auswählen:";

        // ── VERARBEITUNG ───────────────────────────────────────
        lstResults.Items.Clear();
        foreach (var result in results)
            lstResults.Items.Add(result);

        // Ersten Eintrag vorauswählen
        if (lstResults.Items.Count > 0)
            lstResults.SelectedIndex = 0;

        // ── AUSGABE ────────────────────────────────────────────
        btnOk.Enabled = lstResults.Items.Count > 0;
    }

    // ── EVENT-HANDLER ──────────────────────────────────────────

    private void btnOk_Click(object sender, EventArgs e)
    {
        // ── EINGABE ────────────────────────────────────────────
        // ── VERARBEITUNG ───────────────────────────────────────
        SelectedResult = lstResults.SelectedItem as GeocodingResult;

        // ── AUSGABE ────────────────────────────────────────────
        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        // ── AUSGABE ────────────────────────────────────────────
        SelectedResult   = null;
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void lstResults_DoubleClick(object sender, EventArgs e)
    {
        // Doppelklick: direkt bestätigen
        if (lstResults.SelectedItem is not null)
            btnOk_Click(sender, e);
    }

    private void lstResults_SelectedIndexChanged(object sender, EventArgs e)
    {
        // ── EINGABE & VERARBEITUNG ─────────────────────────────
        if (lstResults.SelectedItem is not GeocodingResult result)
        {
            lblCoords.Text = string.Empty;
            return;
        }

        // ── AUSGABE ────────────────────────────────────────────
        lblCoords.Text =
            $"Breitengrad: {result.Latitude.ToString("F4", System.Globalization.CultureInfo.InvariantCulture)}  |  " +
            $"Längengrad: {result.Longitude.ToString("F4", System.Globalization.CultureInfo.InvariantCulture)}  " +
            $"({result.Type})";
    }
}
