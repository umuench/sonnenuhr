// ============================================================
// Datei:     Location.cs
// Projekt:   Sonnenuhr – Standortspezifischer Wallpaper-Generator
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Datenmodell zur Repräsentation eines geografischen Standorts.
//   Enthält Breiten- und Längengrad sowie einen Anzeigenamen.
// ============================================================

namespace Sonnenuhr.Models;

/// <summary>
/// Repräsentiert einen geografischen Standort mit Koordinaten und Anzeigenamen.
/// </summary>
public class Location
{
    // ── EINGABE-DATEN ──────────────────────────────────────────

    /// <summary>Geografischer Breitengrad in Dezimalgrad (−90 bis +90).</summary>
    public double Latitude { get; set; }

    /// <summary>Geografischer Längengrad in Dezimalgrad (−180 bis +180).</summary>
    public double Longitude { get; set; }

    /// <summary>Lesbarer Name des Standorts (z. B. „Mannheim").</summary>
    public string Name { get; set; } = string.Empty;

    // ── VERARBEITUNGS-HILFSMETHODEN ────────────────────────────

    /// <summary>
    /// Prüft, ob die gespeicherten Koordinaten gültig sind.
    /// </summary>
    /// <returns><c>true</c>, wenn Breiten- und Längengrad im erlaubten Bereich liegen.</returns>
    public bool IsValid() =>
        Latitude is >= -90.0 and <= 90.0 &&
        Longitude is >= -180.0 and <= 180.0;

    // ── AUSGABE ────────────────────────────────────────────────

    /// <summary>
    /// Gibt eine lesbare Darstellung des Standorts zurück.
    /// </summary>
    public override string ToString() =>
        string.IsNullOrWhiteSpace(Name)
            ? $"{Latitude:F4}°, {Longitude:F4}°"
            : $"{Name} ({Latitude:F4}°, {Longitude:F4}°)";
}
