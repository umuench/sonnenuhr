// ============================================================
// Datei:     WallpaperConfig.cs
// Projekt:   Sonnenuhr – Standortspezifischer Wallpaper-Generator
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Konfigurationsmodell für die visuelle Gestaltung des generierten
//   Sonnenuhr-Wallpapers (Farben, Schriften, Abmessungen).
// ============================================================

namespace Sonnenuhr.Models;

/// <summary>
/// Enthält alle visuellen Parameter zur Steuerung der Wallpaper-Generierung.
/// </summary>
public class WallpaperConfig
{
    // ── EINGABE-DATEN ──────────────────────────────────────────

    /// <summary>Bildbreite in Pixel (Standard: 1920).</summary>
    public int ImageWidth { get; set; } = 1920;

    /// <summary>Bildhöhe in Pixel (Standard: 1080).</summary>
    public int ImageHeight { get; set; } = 1080;

    /// <summary>Hintergrundfarbe als ARGB-Ganzzahl.</summary>
    public int BackgroundColorArgb { get; set; } = unchecked((int)0xFF1A1A2E);

    /// <summary>Primärfarbe (Ziffernblatt, Hauptelemente) als ARGB-Ganzzahl.</summary>
    public int PrimaryColorArgb { get; set; } = unchecked((int)0xFFE2B96F);

    /// <summary>Sekundärfarbe (Hilfslinien, Beschriftungen) als ARGB-Ganzzahl.</summary>
    public int SecondaryColorArgb { get; set; } = unchecked((int)0xFFC8C8C8);

    /// <summary>Akzentfarbe (aktueller Zeitzeiger, Hervorhebungen) als ARGB-Ganzzahl.</summary>
    public int AccentColorArgb { get; set; } = unchecked((int)0xFFFF6B35);

    /// <summary>Schriftfamilie für Beschriftungen (Standard: „Segoe UI").</summary>
    public string FontFamily { get; set; } = "Segoe UI";

    /// <summary>Basisschriftgröße in Punkt.</summary>
    public float FontSizeBase { get; set; } = 14.0f;

    /// <summary>Zeigt die aktuelle Uhrzeit auf dem Wallpaper an.</summary>
    public bool ShowCurrentTime { get; set; } = true;

    /// <summary>Zeigt den Standortnamen auf dem Wallpaper an.</summary>
    public bool ShowLocationName { get; set; } = true;

    /// <summary>Zeigt Sonnenaufgangs- und Sonnenuntergangszeiten an.</summary>
    public bool ShowSunriseSunset { get; set; } = true;

    /// <summary>Zeigt astronomische Dämmerungslinien an.</summary>
    public bool ShowTwilightLines { get; set; } = true;

    /// <summary>Zeigt den aktuellen Stundenzeiger der Sonnenuhr an.</summary>
    public bool ShowCurrentHourMarker { get; set; } = true;

    /// <summary>Zeigt römische Ziffern statt arabischer Ziffern an.</summary>
    public bool UseRomanNumerals { get; set; } = false;

    /// <summary>
    /// Legt fest, wie die Himmelsrichtungen auf der Sonnenuhr dargestellt werden.
    /// </summary>
    public SundialOrientationMode OrientationMode { get; set; } =
        SundialOrientationMode.AutomaticByLocation;

    /// <summary>
    /// Aktiviert die perspektivische 3D-Darstellung der Sonnenuhrplatte.
    /// </summary>
    public bool EnablePerspectiveDial { get; set; } = true;

    // ── VERARBEITUNGS-HILFSMETHODEN ────────────────────────────

    /// <summary>Gibt die Hintergrundfarbe als <see cref="Color"/> zurück.</summary>
    public System.Drawing.Color BackgroundColor =>
        System.Drawing.Color.FromArgb(BackgroundColorArgb);

    /// <summary>Gibt die Primärfarbe als <see cref="Color"/> zurück.</summary>
    public System.Drawing.Color PrimaryColor =>
        System.Drawing.Color.FromArgb(PrimaryColorArgb);

    /// <summary>Gibt die Sekundärfarbe als <see cref="Color"/> zurück.</summary>
    public System.Drawing.Color SecondaryColor =>
        System.Drawing.Color.FromArgb(SecondaryColorArgb);

    /// <summary>Gibt die Akzentfarbe als <see cref="Color"/> zurück.</summary>
    public System.Drawing.Color AccentColor =>
        System.Drawing.Color.FromArgb(AccentColorArgb);

    // ── AUSGABE ────────────────────────────────────────────────

    /// <summary>Gibt eine lesbare Zusammenfassung der Konfiguration zurück.</summary>
    public override string ToString() =>
        $"{ImageWidth}×{ImageHeight}px | Schrift: {FontFamily} {FontSizeBase}pt";
}
