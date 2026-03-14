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

    /// <summary>Maximaler Stundenwinkel-Offset (±6h = ±90°, Singularität von tan).</summary>
    private const double MaxHourOffset = 6.0;

    /// <summary>Toleranz für Gleitkomma-Vergleich nahe der ±6h-Grenze.</summary>
    private const double Epsilon = 0.01;

    /// <summary>Maximaler Stundenlinien-Offset (±8h deckt lange Sommertage in Mitteleuropa ab).</summary>
    private const int MaxHourLineOffset = 8;

    // ─────────────────────────────────────────────────────────────────────────
    // STUNDENLINIEN-BERECHNUNG
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Berechnet den Stundenlinien-Winkel für eine horizontale Sonnenuhr.
    /// </summary>
    /// <param name="hourOffsetFromNoon">
    /// Stunden-Offset von der Sonnenmittagszeit (negativ = vor Mittag).
    /// Gültige Werte: −8 bis +8 für lange Sommertage in hohen Breitengraden.
    /// </param>
    /// <param name="latitudeDegrees">Geografischer Breitengrad in Dezimalgrad.</param>
    /// <returns>
    /// Stundenlinien-Winkel in Grad, gemessen von der Mittagslinie (Nord-Richtung).
    /// Auf einer horizontalen Sonnenuhr zeigt die Mittagslinie nach Norden,
    /// da der Schatten bei Sonnenhöchststand (Sonne im Süden) nach Norden fällt.
    /// Negative Werte = westlich (Vormittag), positive Werte = östlich (Nachmittag).
    /// Bei |Offset| &gt; 6 überschreitet der Schatten die Ost-West-Linie (±90°).
    /// </returns>
    public static double CalculateHourLineAngle(int hourOffsetFromNoon, double latitudeDegrees)
    {
        // ── EINGABE ────────────────────────────────────────────
        double latRad = DegreesToRadians(latitudeDegrees);
        double hourAngleDeg = hourOffsetFromNoon * DegreesPerHour;
        double hourAngleRad = DegreesToRadians(hourAngleDeg);

        // ── VERARBEITUNG ───────────────────────────────────────
        // Sonderfall: ±6h → ±90° (tan-Singularität)
        if (Math.Abs(hourOffsetFromNoon) == 6)
            return hourOffsetFromNoon > 0 ? 90.0 : -90.0;

        // Horizontale-Sonnenuhr-Formel: H = atan(sin(φ) · tan(t))
        double tanHourAngle = Math.Tan(hourAngleRad);
        double sinLatitude  = Math.Sin(latRad);
        double hourLineDeg  = RadiansToDegrees(Math.Atan(sinLatitude * tanHourAngle));

        // Quadrantenkorrektur für |Offset| > 6: Schatten jenseits der Ost-West-Linie
        if (Math.Abs(hourOffsetFromNoon) > 6)
            hourLineDeg += hourOffsetFromNoon > 0 ? 180.0 : -180.0;

        // ── AUSGABE ────────────────────────────────────────────
        return hourLineDeg;
    }

    /// <summary>
    /// Berechnet alle Stundenlinien-Winkel für eine vollständige Sonnenuhr
    /// von 04:00 bis 20:00 Uhr Sonnenzeit (±8h, für lange Sommertage).
    /// </summary>
    /// <param name="latitudeDegrees">Geografischer Breitengrad in Dezimalgrad.</param>
    /// <returns>
    /// Dictionary mit Stunden-Offset (−8 bis +8) als Schlüssel
    /// und Stundenlinien-Winkel in Grad als Wert.
    /// </returns>
    public static Dictionary<int, double> CalculateAllHourLines(double latitudeDegrees)
    {
        // ── EINGABE ────────────────────────────────────────────
        var hourLines = new Dictionary<int, double>();

        // ── VERARBEITUNG ───────────────────────────────────────
        for (int offset = -MaxHourLineOffset; offset <= MaxHourLineOffset; offset++)
        {
            if (offset == 0)
            {
                // Mittagslinie zeigt genau nach Norden (0° = Bezugsrichtung)
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
    /// <param name="sunriseLocal">Lokale Sonnenaufgangszeit (optional, für physikalisch korrekte Prüfung).</param>
    /// <param name="sunsetLocal">Lokale Sonnenuntergangszeit (optional, für physikalisch korrekte Prüfung).</param>
    /// <returns>
    /// Schatten-Winkel in Grad relativ zur Mittagslinie (Nord-Richtung).
    /// <c>null</c>, wenn die Sonne nicht über dem Horizont steht.
    /// </returns>
    public static double? CalculateCurrentShadowAngle(
        DateTime  currentLocalTime,
        DateTime  solarNoonLocal,
        double    latitudeDegrees,
        DateTime? sunriseLocal = null,
        DateTime? sunsetLocal  = null)
    {
        // ── EINGABE ────────────────────────────────────────────
        TimeSpan timeDifference = currentLocalTime - solarNoonLocal;
        double hourOffset = timeDifference.TotalHours;

        // ── VERARBEITUNG ───────────────────────────────────────

        // Physikalische Prüfung: Kein Schatten ohne Sonne über dem Horizont.
        // Sunrise/Sunset haben Vorrang vor dem theoretischen ±6h-Limit.
        if (sunriseLocal.HasValue && currentLocalTime < sunriseLocal.Value)
            return null;
        if (sunsetLocal.HasValue && currentLocalTime > sunsetLocal.Value)
            return null;

        // Begrenzung auf ±12h (ein halber Sonnentag – darüber hinaus ist physikalisch unmöglich)
        if (Math.Abs(hourOffset) > 12.0)
            return null;

        double hourAngleDeg = hourOffset * DegreesPerHour;
        double hourAngleRad = DegreesToRadians(hourAngleDeg);
        double latRad       = DegreesToRadians(latitudeDegrees);

        // Sonderbehandlung nahe der tan-Singularität bei ±90° (±6h)
        if (Math.Abs(hourOffset - MaxHourOffset) < Epsilon ||
            Math.Abs(hourOffset + MaxHourOffset) < Epsilon)
            return hourOffset > 0 ? 90.0 : -90.0;

        // Horizontale-Sonnenuhr-Formel: H = atan(sin(φ) · tan(t))
        // atan liefert nur Werte in (−90°, +90°).
        // Für |t| > 90° (>6h von Mittag) ist der Schatten jenseits der Ost-West-Linie
        // und benötigt eine Quadrantenkorrektur von ±180°.
        double shadowAngleRad = Math.Atan(Math.Sin(latRad) * Math.Tan(hourAngleRad));
        double shadowAngleDeg = RadiansToDegrees(shadowAngleRad);

        if (Math.Abs(hourOffset) > MaxHourOffset)
        {
            // Schatten hat die Ost-West-Linie überschritten → in den südlichen Halbraum korrigieren
            shadowAngleDeg += hourOffset > 0 ? 180.0 : -180.0;
        }

        // ── AUSGABE ────────────────────────────────────────────
        return shadowAngleDeg;
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
