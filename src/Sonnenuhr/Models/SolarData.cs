// ============================================================
// Datei:     SolarData.cs
// Projekt:   Sonnenuhr – Standortspezifischer Wallpaper-Generator
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Datenmodell für astronomische Sonnenstandsdaten.
//   Wird durch die REST-API (sunrise-sunset.org) befüllt.
// ============================================================

using System.Text.Json.Serialization;

namespace Sonnenuhr.Models;

/// <summary>
/// Enthält die von der Sunrise-Sunset-API gelieferten Sonnenzeitdaten
/// für einen bestimmten Standort und ein bestimmtes Datum.
/// </summary>
public class SolarData
{
    // ── EINGABE-DATEN (API-Response-Mapping) ───────────────────

    /// <summary>Uhrzeit des Sonnenaufgangs (UTC).</summary>
    [JsonPropertyName("sunrise")]
    public DateTime Sunrise { get; set; }

    /// <summary>Uhrzeit des Sonnenuntergangs (UTC).</summary>
    [JsonPropertyName("sunset")]
    public DateTime Sunset { get; set; }

    /// <summary>Uhrzeit der wahren Sonnenmittagszeit (UTC).</summary>
    [JsonPropertyName("solar_noon")]
    public DateTime SolarNoon { get; set; }

    /// <summary>Tageslänge in Sekunden.</summary>
    [JsonPropertyName("day_length")]
    public int DayLengthSeconds { get; set; }

    /// <summary>Beginn der bürgerlichen Morgendämmerung (UTC).</summary>
    [JsonPropertyName("civil_twilight_begin")]
    public DateTime CivilTwilightBegin { get; set; }

    /// <summary>Ende der bürgerlichen Abenddämmerung (UTC).</summary>
    [JsonPropertyName("civil_twilight_end")]
    public DateTime CivilTwilightEnd { get; set; }

    // ── VERARBEITUNGS-HILFSMETHODEN ────────────────────────────

    /// <summary>
    /// Gibt die Tageslänge als <see cref="TimeSpan"/> zurück.
    /// </summary>
    public TimeSpan DayLength => TimeSpan.FromSeconds(DayLengthSeconds);

    /// <summary>
    /// Konvertiert den UTC-Sonnenaufgang in die angegebene Zeitzone.
    /// </summary>
    /// <param name="timeZone">Zielzeitzone.</param>
    /// <returns>Sonnenaufgang in der lokalen Zeitzone.</returns>
    /// <remarks>
    /// Erzwingt <see cref="DateTimeKind.Utc"/>, da System.Text.Json bei ISO-8601-Strings
    /// mit Offset +00:00 ggf. <see cref="DateTimeKind.Unspecified"/> setzt.
    /// </remarks>
    public DateTime GetLocalSunrise(TimeZoneInfo timeZone) =>
        TimeZoneInfo.ConvertTimeFromUtc(AsUtc(Sunrise), timeZone);

    /// <summary>
    /// Konvertiert den UTC-Sonnenuntergang in die angegebene Zeitzone.
    /// </summary>
    /// <param name="timeZone">Zielzeitzone.</param>
    /// <returns>Sonnenuntergang in der lokalen Zeitzone.</returns>
    public DateTime GetLocalSunset(TimeZoneInfo timeZone) =>
        TimeZoneInfo.ConvertTimeFromUtc(AsUtc(Sunset), timeZone);

    /// <summary>
    /// Konvertiert die UTC-Sonnenmittagszeit in die angegebene Zeitzone.
    /// </summary>
    /// <param name="timeZone">Zielzeitzone.</param>
    /// <returns>Sonnenmittag in der lokalen Zeitzone.</returns>
    public DateTime GetLocalSolarNoon(TimeZoneInfo timeZone) =>
        TimeZoneInfo.ConvertTimeFromUtc(AsUtc(SolarNoon), timeZone);

    /// <summary>
    /// Stellt sicher, dass ein <see cref="DateTime"/> als UTC markiert ist.
    /// Notwendig, da JSON-Deserialisierung <see cref="DateTimeKind.Unspecified"/>
    /// setzen kann, was <see cref="TimeZoneInfo.ConvertTimeFromUtc"/> abbricht.
    /// </summary>
    private static DateTime AsUtc(DateTime dt) =>
        dt.Kind == DateTimeKind.Utc
            ? dt
            : DateTime.SpecifyKind(dt, DateTimeKind.Utc);

    // ── AUSGABE ────────────────────────────────────────────────

    /// <summary>
    /// Gibt eine lesbare Zusammenfassung der Sonnenzeitdaten zurück.
    /// </summary>
    public override string ToString() =>
        $"Aufgang: {Sunrise:HH:mm} UTC | Untergang: {Sunset:HH:mm} UTC | Tageslänge: {DayLength:hh\\:mm}";
}

// ── API-Response-Wrapper ───────────────────────────────────

/// <summary>
/// Wrapper für die JSON-Antwort der Sunrise-Sunset-API.
/// </summary>
internal class SunriseSunsetApiResponse
{
    [JsonPropertyName("results")]
    public SolarData? Results { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
