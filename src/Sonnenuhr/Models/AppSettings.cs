// ============================================================
// Datei:     AppSettings.cs
// Projekt:   Sonnenuhr – Standortspezifischer Wallpaper-Generator
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Hauptkonfigurationsmodell der Anwendung. Aggregiert Standort-
//   und Wallpaper-Einstellungen und wird als JSON persistiert.
// ============================================================

namespace Sonnenuhr.Models;

/// <summary>
/// Aggregiert alle persistierbaren Anwendungseinstellungen.
/// Wird als JSON in <c>%APPDATA%\Sonnenuhr\settings.json</c> gespeichert.
/// </summary>
public class AppSettings
{
    // ── EINGABE-DATEN ──────────────────────────────────────────

    /// <summary>Aktuell konfigurierter Standort des Benutzers.</summary>
    public Location Location { get; set; } = new()
    {
        Name      = "Mannheim",
        Latitude  = 49.4875,
        Longitude = 8.4660
    };

    /// <summary>Visuelle Konfiguration des Wallpapers.</summary>
    public WallpaperConfig WallpaperConfig { get; set; } = new();

    /// <summary>Intervall in Minuten für die automatische Wallpaper-Aktualisierung.</summary>
    public int UpdateIntervalMinutes { get; set; } = 60;

    /// <summary>Startet den Wallpaper-Updater automatisch mit Windows.</summary>
    public bool StartWithWindows { get; set; } = false;

    /// <summary>Zeitzone-ID des Standorts (z. B. „W. Europe Standard Time").</summary>
    public string TimeZoneId { get; set; } = "W. Europe Standard Time";

    /// <summary>Zeitstempel der letzten API-Abfrage (UTC).</summary>
    public DateTime? LastApiCallUtc { get; set; }

    // ── VERARBEITUNGS-HILFSMETHODEN ────────────────────────────

    /// <summary>
    /// Gibt die konfigurierte Zeitzone als <see cref="TimeZoneInfo"/> zurück.
    /// Bei unbekannter ID wird UTC zurückgegeben.
    /// </summary>
    public TimeZoneInfo GetTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.Utc;
        }
    }

    // ── AUSGABE ────────────────────────────────────────────────

    /// <summary>Gibt eine lesbare Zusammenfassung der Einstellungen zurück.</summary>
    public override string ToString() =>
        $"Standort: {Location} | Update: alle {UpdateIntervalMinutes} min | Zone: {TimeZoneId}";
}
