// ============================================================
// Datei:     GeocodingResult.cs
// Projekt:   Sonnenuhr – Standortspezifischer Wallpaper-Generator
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Datenmodell für einen einzelnen Treffer der OpenStreetMap
//   Nominatim Geocoding-API (Format: jsonv2).
// ============================================================

using System.Globalization;
using System.Text.Json.Serialization;

namespace Sonnenuhr.Models;

/// <summary>
/// Repräsentiert einen einzelnen Suchergebnis-Eintrag der Nominatim-API.
/// </summary>
/// <remarks>
/// Die Felder <see cref="LatitudeText"/> und <see cref="LongitudeText"/>
/// enthalten Koordinaten als Zeichenkette mit Punkt als Dezimaltrennzeichen
/// (US-Englisch). Die berechneten Properties <see cref="Latitude"/> und
/// <see cref="Longitude"/> stellen die geparsten <c>double</c>-Werte bereit.
/// </remarks>
public sealed class GeocodingResult
{
    // ── EINGABE (JSON-Deserialisierung) ────────────────────────

    /// <summary>Vollständige Adressanzeige (z. B. „Eberbach, Rhein-Neckar-Kreis, …").</summary>
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Breitengrad als Zeichenkette (Punkt als Dezimaltrennzeichen).</summary>
    [JsonPropertyName("lat")]
    public string LatitudeText { get; set; } = string.Empty;

    /// <summary>Längengrad als Zeichenkette (Punkt als Dezimaltrennzeichen).</summary>
    [JsonPropertyName("lon")]
    public string LongitudeText { get; set; } = string.Empty;

    /// <summary>OSM-Typ des Ortes (z. B. „city", „town", „village", „administrative").</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>OSM-Kategorie (z. B. „place", „boundary").</summary>
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    /// <summary>Relevanz-Score der API (0–1, je höher desto bedeutender).</summary>
    [JsonPropertyName("importance")]
    public double Importance { get; set; }

    // ── VERARBEITUNG ───────────────────────────────────────────

    /// <summary>
    /// Breitengrad als <c>double</c>-Wert, geparst aus <see cref="LatitudeText"/>.
    /// </summary>
    public double Latitude =>
        double.TryParse(LatitudeText, NumberStyles.Float, CultureInfo.InvariantCulture, out double v)
            ? v
            : 0.0;

    /// <summary>
    /// Längengrad als <c>double</c>-Wert, geparst aus <see cref="LongitudeText"/>.
    /// </summary>
    public double Longitude =>
        double.TryParse(LongitudeText, NumberStyles.Float, CultureInfo.InvariantCulture, out double v)
            ? v
            : 0.0;

    /// <summary>
    /// Kurzname des Ortes: erster Teil des <see cref="DisplayName"/> vor dem ersten Komma.
    /// </summary>
    public string ShortName =>
        DisplayName.Contains(',')
            ? DisplayName[..DisplayName.IndexOf(',')].Trim()
            : DisplayName.Trim();

    // ── AUSGABE ────────────────────────────────────────────────

    /// <summary>
    /// Gibt den vollständigen Anzeigenamen zurück (geeignet für ListBox-Darstellung).
    /// </summary>
    public override string ToString() => DisplayName;
}
