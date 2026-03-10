// ============================================================
// Datei:     LocationTests.cs
// Projekt:   Sonnenuhr.Tests
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Unit-Tests für das Location-Datenmodell.
//   Verifiziert Validierung und ToString-Ausgabe.
// ============================================================

using Sonnenuhr.Models;

namespace Sonnenuhr.Tests;

/// <summary>
/// Unit-Tests für <see cref="Location"/>.
/// </summary>
public class LocationTests
{
    [Theory]
    [InlineData(49.4875,  8.4660, true)]   // Mannheim – gültig
    [InlineData(90.0,    180.0,   true)]   // Maximalwerte – gültig
    [InlineData(-90.0,  -180.0,  true)]    // Minimalwerte – gültig
    [InlineData(91.0,    0.0,    false)]   // Breitengrad zu groß
    [InlineData(0.0,    181.0,   false)]   // Längengrad zu groß
    public void IsValid_ReturnsExpected(double lat, double lon, bool expected)
    {
        // ── EINGABE ────────────────────────────────────────────
        var location = new Location { Latitude = lat, Longitude = lon };
        // ── VERARBEITUNG & AUSGABE ─────────────────────────────
        Assert.Equal(expected, location.IsValid());
    }

    [Fact]
    public void ToString_WithName_ContainsNameAndCoordinates()
    {
        // ── EINGABE ────────────────────────────────────────────
        var location = new Location { Name = "Mannheim", Latitude = 49.4875, Longitude = 8.4660 };
        // ── VERARBEITUNG ───────────────────────────────────────
        string result = location.ToString();
        // ── AUSGABE ────────────────────────────────────────────
        Assert.Contains("Mannheim", result);
        Assert.Contains("49,4875", result.Replace('.', ','));
    }

    [Fact]
    public void ToString_WithoutName_ReturnsCoordinatesOnly()
    {
        // ── EINGABE ────────────────────────────────────────────
        var location = new Location { Name = "", Latitude = 49.0, Longitude = 8.0 };
        // ── VERARBEITUNG ───────────────────────────────────────
        string result = location.ToString();
        // ── AUSGABE ────────────────────────────────────────────
        Assert.DoesNotContain("(", result);
    }
}
