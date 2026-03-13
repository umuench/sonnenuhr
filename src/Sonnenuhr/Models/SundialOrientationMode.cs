namespace Sonnenuhr.Models;

/// <summary>
/// Bestimmt die Ansichtsorientierung der Sonnenuhr.
/// </summary>
public enum SundialOrientationMode
{
    /// <summary>Automatische Ausrichtung anhand des Breitengrads.</summary>
    AutomaticByLocation = 0,

    /// <summary>Nordrichtung wird oben dargestellt.</summary>
    NorthUp = 1,

    /// <summary>Südrichtung wird oben dargestellt.</summary>
    SouthUp = 2
}
