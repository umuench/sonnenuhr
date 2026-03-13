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

using Sonnenuhr.Models;

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

    /// <summary>Breitengradbereich für neutrale Äquatorbehandlung.</summary>
    private const double EquatorThresholdDegrees = 2.0;

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

    // ─────────────────────────────────────────────────────────────────────────
    // ORIENTIERUNG
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Löst den gewünschten Orientierungsmodus in eine konkrete Darstellung auf.
    /// </summary>
    public static SundialOrientationMode ResolveOrientationMode(
        double latitudeDegrees,
        SundialOrientationMode selectedMode)
    {
        // ── EINGABE ────────────────────────────────────────────
        if (selectedMode != SundialOrientationMode.AutomaticByLocation)
            return selectedMode;

        // ── VERARBEITUNG ───────────────────────────────────────
        if (latitudeDegrees > EquatorThresholdDegrees)
            return SundialOrientationMode.NorthUp;

        if (latitudeDegrees < -EquatorThresholdDegrees)
            return SundialOrientationMode.SouthUp;

        // Neutrale Äquatornähe: stabiler Standard
        // ── AUSGABE ────────────────────────────────────────────
        return SundialOrientationMode.NorthUp;
    }

    /// <summary>
    /// Liefert eine verständliche Begründung für die aktuelle Orientierung.
    /// </summary>
    public static string GetOrientationReasonText(
        double latitudeDegrees,
        SundialOrientationMode selectedMode)
    {
        // ── EINGABE ────────────────────────────────────────────
        SundialOrientationMode resolved = ResolveOrientationMode(latitudeDegrees, selectedMode);

        // ── VERARBEITUNG & AUSGABE ─────────────────────────────
        if (selectedMode == SundialOrientationMode.NorthUp)
            return "Manuell gewählt: Nord oben.";

        if (selectedMode == SundialOrientationMode.SouthUp)
            return "Manuell gewählt: Süd oben.";

        if (Math.Abs(latitudeDegrees) <= EquatorThresholdDegrees)
            return "Automatik: Äquatornähe erkannt; stabiler Standard Nord oben.";

        return resolved == SundialOrientationMode.NorthUp
            ? "Automatik: Standort liegt auf der Nordhalbkugel."
            : "Automatik: Standort liegt auf der Südhalbkugel.";
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SONNENPOSITION
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Berechnet Sonnenhöhe und Sonnenazimut für einen lokalen Zeitpunkt.
    /// </summary>
    /// <remarks>
    /// Verwendet NOAA-Näherungsformeln für:
    /// - Gleichung der Zeit
    /// - Sonnendeklination
    /// - Stundenwinkel
    /// Daraus werden Sonnenhöhe und Azimut für den Standort abgeleitet.
    /// </remarks>
    public static SolarPosition CalculateSolarPosition(
        DateTime currentLocalTime,
        Location location,
        TimeZoneInfo timeZone)
    {
        // ── EINGABE ────────────────────────────────────────────
        return CalculateSolarPosition(
            currentLocalTime,
            location.Latitude,
            location.Longitude,
            timeZone);
    }

    /// <summary>
    /// Berechnet Sonnenhöhe und Sonnenazimut für einen lokalen Zeitpunkt.
    /// </summary>
    public static SolarPosition CalculateSolarPosition(
        DateTime currentLocalTime,
        double latitudeDegrees,
        double longitudeDegrees,
        TimeZoneInfo timeZone)
    {
        // ── EINGABE ────────────────────────────────────────────
        double latRad = DegreesToRadians(latitudeDegrees);
        int dayOfYear = currentLocalTime.DayOfYear;
        double hour = currentLocalTime.Hour
                    + currentLocalTime.Minute / 60.0
                    + currentLocalTime.Second / 3600.0;

        // ── VERARBEITUNG ───────────────────────────────────────
        // Fractional year (NOAA)
        double gamma = 2.0 * Math.PI / 365.0 * (dayOfYear - 1 + (hour - 12.0) / 24.0);

        // Gleichung der Zeit in Minuten
        double equationOfTime = 229.18 * (
            0.000075
            + 0.001868 * Math.Cos(gamma)
            - 0.032077 * Math.Sin(gamma)
            - 0.014615 * Math.Cos(2 * gamma)
            - 0.040849 * Math.Sin(2 * gamma));

        // Sonnendeklination in Bogenmaß
        double declination = 0.006918
                           - 0.399912 * Math.Cos(gamma)
                           + 0.070257 * Math.Sin(gamma)
                           - 0.006758 * Math.Cos(2 * gamma)
                           + 0.000907 * Math.Sin(2 * gamma)
                           - 0.002697 * Math.Cos(3 * gamma)
                           + 0.001480 * Math.Sin(3 * gamma);

        // Wahre Sonnenzeit in Minuten
        double utcOffsetMinutes = timeZone.GetUtcOffset(currentLocalTime).TotalMinutes;
        double trueSolarMinutes = hour * 60.0 + equationOfTime + 4.0 * longitudeDegrees - utcOffsetMinutes;
        trueSolarMinutes = (trueSolarMinutes % 1440.0 + 1440.0) % 1440.0;

        // Stundenwinkel in Grad / Bogenmaß
        double hourAngleDegrees = trueSolarMinutes / 4.0 - 180.0;
        if (hourAngleDegrees < -180.0)
            hourAngleDegrees += 360.0;
        double hourAngle = DegreesToRadians(hourAngleDegrees);

        // Sonnenhöhe
        double cosZenith = Math.Sin(latRad) * Math.Sin(declination)
                         + Math.Cos(latRad) * Math.Cos(declination) * Math.Cos(hourAngle);
        cosZenith = Math.Clamp(cosZenith, -1.0, 1.0);

        double zenith = Math.Acos(cosZenith);
        double altitudeDegrees = 90.0 - RadiansToDegrees(zenith);

        // Sonnenazimut: 0° = Nord, 90° = Ost, 180° = Süd, 270° = West
        double azimuth = Math.Atan2(
            Math.Sin(hourAngle),
            Math.Cos(hourAngle) * Math.Sin(latRad) - Math.Tan(declination) * Math.Cos(latRad));
        double azimuthDegrees = (RadiansToDegrees(azimuth) + 180.0 + 360.0) % 360.0;

        // ── AUSGABE ────────────────────────────────────────────
        return new SolarPosition(altitudeDegrees, azimuthDegrees, altitudeDegrees > 0.0);
    }

    /// <summary>
    /// Berechnet die relative Schattenlänge (Schatten/Objekthöhe) aus der Sonnenhöhe.
    /// </summary>
    /// <returns>
    /// Faktor der Schattenlänge oder <c>null</c>, wenn die Sonne nicht über dem Horizont steht.
    /// </returns>
    public static double? CalculateShadowLengthFactor(double solarAltitudeDegrees)
    {
        // ── EINGABE ────────────────────────────────────────────
        if (solarAltitudeDegrees <= 0.0)
            return null;

        // ── VERARBEITUNG ───────────────────────────────────────
        double altitudeRad = DegreesToRadians(solarAltitudeDegrees);
        double tanAltitude = Math.Tan(altitudeRad);
        if (tanAltitude <= 0.0001)
            return null;

        // ── AUSGABE ────────────────────────────────────────────
        return 1.0 / tanAltitude;
    }
}
