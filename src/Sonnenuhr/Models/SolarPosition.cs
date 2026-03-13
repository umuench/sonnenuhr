namespace Sonnenuhr.Models;

/// <summary>
/// Enthält die lokal berechnete Sonnenposition für einen Zeitpunkt und Standort.
/// </summary>
public readonly record struct SolarPosition(
    double AltitudeDegrees,
    double AzimuthDegrees,
    bool   IsAboveHorizon)
{
    /// <summary>
    /// Schattenpeilung in Grad bezogen auf geografisch Nord (Uhrzeigersinn).
    /// </summary>
    public double ShadowBearingDegrees =>
        (AzimuthDegrees + 180.0) % 360.0;
}
