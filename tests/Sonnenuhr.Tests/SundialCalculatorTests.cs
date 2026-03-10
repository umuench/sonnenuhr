// ============================================================
// Datei:     SundialCalculatorTests.cs
// Projekt:   Sonnenuhr.Tests
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Unit-Tests für die SundialCalculator-Klasse.
//   Verifiziert astronomische Berechnungsformeln der Sonnenuhr.
// ============================================================

using Sonnenuhr.Services;

namespace Sonnenuhr.Tests;

/// <summary>
/// Unit-Tests für <see cref="SundialCalculator"/>.
/// </summary>
public class SundialCalculatorTests
{
    // ── KONSTANTEN ─────────────────────────────────────────────

    /// <summary>Toleranz für Gleitkommavergleiche (in Grad).</summary>
    private const double AngleTolerance = 0.001;

    /// <summary>Breitengrad Mannheim für Standardtests.</summary>
    private const double MannheimLatitude = 49.4875;

    // ─────────────────────────────────────────────────────────────────────────
    // STUNDENLINIEN-WINKEL
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void CalculateHourLineAngle_NoonOffset_ReturnsZero()
    {
        // ── EINGABE ────────────────────────────────────────────
        // Mittags-Stundenlinie (Offset 0) muss exakt 0° ergeben
        // ── VERARBEITUNG ───────────────────────────────────────
        double angle = SundialCalculator.CalculateHourLineAngle(0, MannheimLatitude);
        // ── AUSGABE ────────────────────────────────────────────
        Assert.Equal(0.0, angle, AngleTolerance);
    }

    [Theory]
    [InlineData( 1,  49.4875)]   // 1 Stunde nach Mittag → positiver Winkel
    [InlineData(-1,  49.4875)]   // 1 Stunde vor Mittag  → negativer Winkel
    public void CalculateHourLineAngle_SymmetricAroundNoon(int offset, double lat)
    {
        // ── EINGABE ────────────────────────────────────────────
        // ── VERARBEITUNG ───────────────────────────────────────
        double positiveAngle = SundialCalculator.CalculateHourLineAngle( offset, lat);
        double negativeAngle = SundialCalculator.CalculateHourLineAngle(-offset, lat);
        // ── AUSGABE ────────────────────────────────────────────
        // Stundenlinien sind symmetrisch zur Mittagslinie
        Assert.Equal(Math.Abs(positiveAngle), Math.Abs(negativeAngle), AngleTolerance);
        Assert.NotEqual(0.0, positiveAngle);
    }

    [Fact]
    public void CalculateHourLineAngle_HigherLatitude_ProducesLargerAngle()
    {
        // ── EINGABE ────────────────────────────────────────────
        double latitudeNorth = 60.0; // z. B. Helsinki
        double latitudeSouth = 30.0; // z. B. Kairo
        int hourOffset = 3;

        // ── VERARBEITUNG ───────────────────────────────────────
        double angleNorth = SundialCalculator.CalculateHourLineAngle(hourOffset, latitudeNorth);
        double angleSouth = SundialCalculator.CalculateHourLineAngle(hourOffset, latitudeSouth);

        // ── AUSGABE ────────────────────────────────────────────
        // Bei höherem Breitengrad sind Stundenlinien-Winkel größer
        Assert.True(Math.Abs(angleNorth) > Math.Abs(angleSouth));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ALLE STUNDENLINIEN
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void CalculateAllHourLines_Returns13Entries()
    {
        // ── EINGABE ────────────────────────────────────────────
        // ── VERARBEITUNG ───────────────────────────────────────
        var hourLines = SundialCalculator.CalculateAllHourLines(MannheimLatitude);
        // ── AUSGABE ────────────────────────────────────────────
        Assert.Equal(13, hourLines.Count); // −6 bis +6 inclusive
    }

    [Fact]
    public void CalculateAllHourLines_ContainsOffsets_MinusSix_To_PlusSix()
    {
        // ── EINGABE ────────────────────────────────────────────
        // ── VERARBEITUNG ───────────────────────────────────────
        var hourLines = SundialCalculator.CalculateAllHourLines(MannheimLatitude);
        // ── AUSGABE ────────────────────────────────────────────
        for (int i = -6; i <= 6; i++)
            Assert.True(hourLines.ContainsKey(i), $"Offset {i} fehlt im Dictionary.");
    }

    [Fact]
    public void CalculateAllHourLines_NoonLineIsZero()
    {
        // ── EINGABE ────────────────────────────────────────────
        // ── VERARBEITUNG ───────────────────────────────────────
        var hourLines = SundialCalculator.CalculateAllHourLines(MannheimLatitude);
        // ── AUSGABE ────────────────────────────────────────────
        Assert.Equal(0.0, hourLines[0], AngleTolerance);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AKTUELLER SCHATTEN-WINKEL
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void CalculateCurrentShadowAngle_AtSolarNoon_ReturnsZero()
    {
        // ── EINGABE ────────────────────────────────────────────
        DateTime solarNoon  = new(2026, 7, 1, 13, 14, 0);
        DateTime currentTime = solarNoon; // Genau Sonnenmittag

        // ── VERARBEITUNG ───────────────────────────────────────
        double? angle = SundialCalculator.CalculateCurrentShadowAngle(
            currentTime, solarNoon, MannheimLatitude);

        // ── AUSGABE ────────────────────────────────────────────
        Assert.NotNull(angle);
        Assert.Equal(0.0, angle!.Value, AngleTolerance);
    }

    [Fact]
    public void CalculateCurrentShadowAngle_NightTime_ReturnsNull()
    {
        // ── EINGABE ────────────────────────────────────────────
        DateTime solarNoon   = new(2026, 7, 1, 13, 0, 0);
        DateTime nightTime   = solarNoon.AddHours(8); // 21 Uhr – keine Sonne

        // ── VERARBEITUNG ───────────────────────────────────────
        double? angle = SundialCalculator.CalculateCurrentShadowAngle(
            nightTime, solarNoon, MannheimLatitude);

        // ── AUSGABE ────────────────────────────────────────────
        Assert.Null(angle);
    }

    [Fact]
    public void CalculateCurrentShadowAngle_AfternoonTime_ReturnsPositiveAngle()
    {
        // ── EINGABE ────────────────────────────────────────────
        DateTime solarNoon  = new(2026, 7, 1, 13, 0, 0);
        DateTime afternoon  = solarNoon.AddHours(2); // 15 Uhr (2h nach Mittag)

        // ── VERARBEITUNG ───────────────────────────────────────
        double? angle = SundialCalculator.CalculateCurrentShadowAngle(
            afternoon, solarNoon, MannheimLatitude);

        // ── AUSGABE ────────────────────────────────────────────
        Assert.NotNull(angle);
        Assert.True(angle!.Value > 0, "Nachmittags-Winkel sollte positiv sein.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // KONVERTIERUNGS-HILFSMETHODEN
    // ─────────────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0,   0.0)]
    [InlineData(90,  Math.PI / 2)]
    [InlineData(180, Math.PI)]
    [InlineData(360, Math.PI * 2)]
    public void DegreesToRadians_KnownValues(double degrees, double expectedRadians)
    {
        // ── EINGABE ────────────────────────────────────────────
        // ── VERARBEITUNG & AUSGABE ─────────────────────────────
        Assert.Equal(expectedRadians, SundialCalculator.DegreesToRadians(degrees), 1e-9);
    }

    [Theory]
    [InlineData(0.0,       0)]
    [InlineData(Math.PI,   180)]
    [InlineData(Math.PI/2, 90)]
    public void RadiansToDegrees_KnownValues(double radians, double expectedDegrees)
    {
        // ── EINGABE ────────────────────────────────────────────
        // ── VERARBEITUNG & AUSGABE ─────────────────────────────
        Assert.Equal(expectedDegrees, SundialCalculator.RadiansToDegrees(radians), 1e-9);
    }
}
