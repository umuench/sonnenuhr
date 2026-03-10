// ============================================================
// Datei:     SundialCalculator.cs
// Projekt:   Sonnenuhr – Standortspezifischer Wallpaper-Generator
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Berechnet die Stundenlinien-Winkel einer horizontalen Sonnenuhr
//   sowie den aktuellen Gnomon-Schatten auf Basis des geografischen
//   Breitengrades. Verwendet astronomische Standardformeln.
// ============================================================

namespace Sonnenuhr.Services;

/// <summary>
/// Stellt Berechnungslogik für horizontale Sonnenuhren bereit.
/// </summary>
/// <remarks>
/// Formel für den Stundenlinien-Winkel einer horizontalen Sonnenuhr:
/// <code>tan(H) = sin(φ) · tan(t)</code>
/// wobei φ = geografischer Breitengrad und t = Stunden-Winkel (15° je Stunde).
/// </remarks>
public static class SundialCalculator
{
    // ── KONSTANTEN ─────────────────────────────────────────────

    /// <summary>Grad-Winkel pro Stunde (360° / 24h = 15°/h).</summary>
    private const double DegreesPerHour = 15.0;

    // ─────────────────────────────────────────────────────────────────────────
    // STUNDENLINIEN-BERECHNUNG
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Berechnet den Stundenlinien-Winkel für eine horizontale Sonnenuhr.
    /// </summary>
    /// <param name="hourOffsetFromNoon">
    /// Stunden-Offset von der Sonnenmittagszeit (negativ = vor Mittag).
    /// Gültige Werte: −6 bis +6 für typische Tageslicht-Stunden.
    /// </param>
    /// <param name="latitudeDegrees">Geografischer Breitengrad in Dezimalgrad.</param>
    /// <returns>
    /// Stundenlinien-Winkel in Grad, gemessen von der Mittagslinie (Süd-Richtung).
    /// Negative Werte = westlich (Vormittag), positive Werte = östlich (Nachmittag).
    /// </returns>
    public static double CalculateHourLineAngle(int hourOffsetFromNoon, double latitudeDegrees)
    {
        // ── EINGABE ────────────────────────────────────────────
        double latRad = DegreesToRadians(latitudeDegrees);
        double hourAngleDeg = hourOffsetFromNoon * DegreesPerHour;
        double hourAngleRad = DegreesToRadians(hourAngleDeg);

        // ── VERARBEITUNG ───────────────────────────────────────
        // Horizontale-Sonnenuhr-Formel: H = atan(sin(φ) · tan(t))
        double tanHourAngle = Math.Tan(hourAngleRad);
        double sinLatitude  = Math.Sin(latRad);
        double hourLineRad  = Math.Atan(sinLatitude * tanHourAngle);

        // ── AUSGABE ────────────────────────────────────────────
        return RadiansToDegrees(hourLineRad);
    }

    /// <summary>
    /// Berechnet alle Stundenlinien-Winkel für eine vollständige Sonnenuhr
    /// von 06:00 bis 18:00 Uhr Sonnenzeit.
    /// </summary>
    /// <param name="latitudeDegrees">Geografischer Breitengrad in Dezimalgrad.</param>
    /// <returns>
    /// Dictionary mit Stunden-Offset (−6 bis +6) als Schlüssel
    /// und Stundenlinien-Winkel in Grad als Wert.
    /// </returns>
    public static Dictionary<int, double> CalculateAllHourLines(double latitudeDegrees)
    {
        // ── EINGABE ────────────────────────────────────────────
        var hourLines = new Dictionary<int, double>();

        // ── VERARBEITUNG ───────────────────────────────────────
        for (int offset = -6; offset <= 6; offset++)
        {
            if (offset == 0)
            {
                // Mittagslinie zeigt genau nach Süden (0°)
                hourLines[0] = 0.0;
                continue;
            }

            hourLines[offset] = CalculateHourLineAngle(offset, latitudeDegrees);
        }

        // ── AUSGABE ────────────────────────────────────────────
        return hourLines;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AKTUELLER SONNENSTAND
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Berechnet den aktuellen Schatten-Winkel des Gnomons auf Basis der
    /// Sonnenmittagszeit und der aktuellen Ortszeit.
    /// </summary>
    /// <param name="currentLocalTime">Aktuelle Ortszeit.</param>
    /// <param name="solarNoonLocal">Wahre Sonnenmittagszeit (lokal).</param>
    /// <param name="latitudeDegrees">Geografischer Breitengrad in Dezimalgrad.</param>
    /// <returns>
    /// Schatten-Winkel in Grad relativ zur Mittagslinie.
    /// <c>null</c>, wenn die Sonne nicht über dem Horizont steht.
    /// </returns>
    public static double? CalculateCurrentShadowAngle(
        DateTime currentLocalTime,
        DateTime solarNoonLocal,
        double   latitudeDegrees)
    {
        // ── EINGABE ────────────────────────────────────────────
        TimeSpan timeDifference = currentLocalTime - solarNoonLocal;
        double hourOffset = timeDifference.TotalHours;

        // ── VERARBEITUNG ───────────────────────────────────────
        // Sonnenuhr gilt nur für Stunden zwischen −6 und +6 (6–18 Uhr Sonnenzeit)
        if (Math.Abs(hourOffset) > 6.0)
            return null;

        double hourAngleDeg = hourOffset * DegreesPerHour;
        double hourAngleRad = DegreesToRadians(hourAngleDeg);
        double latRad       = DegreesToRadians(latitudeDegrees);

        double shadowAngleRad = Math.Atan(Math.Sin(latRad) * Math.Tan(hourAngleRad));

        // ── AUSGABE ────────────────────────────────────────────
        return RadiansToDegrees(shadowAngleRad);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // HILFSMETHODEN
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Konvertiert Grad in Bogenmaß.</summary>
    public static double DegreesToRadians(double degrees) =>
        degrees * (Math.PI / 180.0);

    /// <summary>Konvertiert Bogenmaß in Grad.</summary>
    public static double RadiansToDegrees(double radians) =>
        radians * (180.0 / Math.PI);

    /// <summary>
    /// Berechnet die Stunden-Beschriftung aus dem Offset von der Sonnenmittags-
    /// zeit und einer Basisuhrzeit für Mittag.
    /// </summary>
    /// <param name="hourOffsetFromNoon">Stunden-Offset von Mittag.</param>
    /// <param name="solarNoon">Wahre Sonnenmittagszeit.</param>
    /// <returns>Formatierte Uhrzeitangabe (z. B. „09:00").</returns>
    public static string GetHourLabel(int hourOffsetFromNoon, DateTime solarNoon)
    {
        // ── EINGABE ────────────────────────────────────────────
        // ── VERARBEITUNG ───────────────────────────────────────
        DateTime labelTime = solarNoon.AddHours(hourOffsetFromNoon);

        // ── AUSGABE ────────────────────────────────────────────
        return labelTime.ToString("HH:mm");
    }
}
