// ============================================================
// Datei:     WallpaperGeneratorService.cs
// Projekt:   Sonnenuhr – Standortspezifischer Wallpaper-Generator
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Generiert das Sonnenuhr-Wallpaper-Bild mittels System.Drawing.
//   Zeichnet Ziffernblatt, Stundenlinien, Gnomon und Informationstexte.
// ============================================================

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Sonnenuhr.Models;

namespace Sonnenuhr.Services;

/// <summary>
/// Erstellt das standortspezifische Sonnenuhr-Wallpaper als Bitmap-Datei.
/// </summary>
public class WallpaperGeneratorService
{
    // ── FELDER ─────────────────────────────────────────────────

    private readonly WallpaperConfig _config;

    // ── KONSTRUKTOR ────────────────────────────────────────────

    /// <summary>
    /// Initialisiert den Generator mit der angegebenen Wallpaper-Konfiguration.
    /// </summary>
    /// <param name="config">Visuelle Konfiguration des Wallpapers.</param>
    public WallpaperGeneratorService(WallpaperConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    // ── ÖFFENTLICHE METHODEN ───────────────────────────────────

    /// <summary>
    /// Generiert das Sonnenuhr-Wallpaper und speichert es als PNG-Datei.
    /// </summary>
    /// <param name="solarData">Astronomische Sonnenstandsdaten.</param>
    /// <param name="location">Standort für Beschriftung und Berechnung.</param>
    /// <param name="outputPath">Speicherpfad der PNG-Datei.</param>
    /// <param name="currentTime">Aktuelle Ortszeit (für Schattenzeiger).</param>
    /// <param name="timeZone">Zeitzone des Standorts.</param>
    public void GenerateWallpaper(
        SolarData    solarData,
        Location     location,
        string       outputPath,
        DateTime     currentTime,
        TimeZoneInfo timeZone)
    {
        // ── EINGABE ────────────────────────────────────────────
        ArgumentNullException.ThrowIfNull(solarData);
        ArgumentNullException.ThrowIfNull(location);
        if (string.IsNullOrWhiteSpace(outputPath))
            throw new ArgumentException("Ausgabepfad darf nicht leer sein.", nameof(outputPath));

        // ── VERARBEITUNG ───────────────────────────────────────
        using var bitmap  = new Bitmap(_config.ImageWidth, _config.ImageHeight, PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(bitmap);

        ConfigureGraphics(graphics);
        DrawBackground(graphics);

        // Sonnenuhr-Mittelpunkt und Radius bestimmen
        var center = new PointF(_config.ImageWidth / 2f, _config.ImageHeight / 2f);
        float radius = Math.Min(_config.ImageWidth, _config.ImageHeight) * 0.38f;

        // Einzelne Zeichenschichten von unten nach oben aufbauen
        DrawDialPlate(graphics, center, radius);
        DrawHourLines(graphics, center, radius, solarData, location, timeZone);
        DrawGnomon(graphics, center, radius, location.Latitude);
        DrawCurrentTimeIndicator(graphics, center, radius, solarData, location, currentTime, timeZone);

        if (_config.ShowLocationName || _config.ShowSunriseSunset || _config.ShowCurrentTime)
            DrawInfoPanel(graphics, solarData, location, currentTime, timeZone);

        // ── AUSGABE ────────────────────────────────────────────
        string? directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        bitmap.Save(outputPath, ImageFormat.Png);
    }

    // ── PRIVATE ZEICHENMETHODEN ────────────────────────────────

    /// <summary>Konfiguriert Antialiasing und Text-Rendering für hohe Qualität.</summary>
    private static void ConfigureGraphics(Graphics g)
    {
        g.SmoothingMode      = SmoothingMode.AntiAlias;
        g.InterpolationMode  = InterpolationMode.HighQualityBicubic;
        g.TextRenderingHint  = TextRenderingHint.ClearTypeGridFit;
        g.PixelOffsetMode    = PixelOffsetMode.HighQuality;
    }

    /// <summary>Füllt den Hintergrund mit der konfigurierten Hintergrundfarbe.</summary>
    private void DrawBackground(Graphics g)
    {
        // ── EINGABE ────────────────────────────────────────────
        // ── VERARBEITUNG ───────────────────────────────────────
        using var bgBrush = new SolidBrush(_config.BackgroundColor);
        g.FillRectangle(bgBrush, 0, 0, _config.ImageWidth, _config.ImageHeight);

        // Subtiler radialer Verlauf für Tiefenwirkung
        using var gradBrush = new PathGradientBrush(
            new[] { new PointF(_config.ImageWidth / 2f, _config.ImageHeight / 2f) })
        {
            SurroundColors = new[] { Color.FromArgb(60, 255, 255, 255) },
            CenterColor    = Color.FromArgb(0, 0, 0, 0)
        };

        // ── AUSGABE ────────────────────────────────────────────
        // (Zeichnung auf Graphics-Objekt – kein Rückgabewert)
    }

    /// <summary>Zeichnet das kreisförmige Ziffernblatt der Sonnenuhr.</summary>
    private void DrawDialPlate(Graphics g, PointF center, float radius)
    {
        // ── EINGABE ────────────────────────────────────────────
        var rect = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);

        // ── VERARBEITUNG ───────────────────────────────────────
        // Äußerer Kreis (Ziffernblatt)
        using var dialBrush = new SolidBrush(Color.FromArgb(30, _config.PrimaryColor.R,
                                                                  _config.PrimaryColor.G,
                                                                  _config.PrimaryColor.B));
        g.FillEllipse(dialBrush, rect);

        using var outerPen = new Pen(_config.PrimaryColor, 3f);
        g.DrawEllipse(outerPen, rect);

        // Innerer Dekorkreis
        float innerRadius = radius * 0.92f;
        var innerRect = new RectangleF(center.X - innerRadius, center.Y - innerRadius,
                                       innerRadius * 2, innerRadius * 2);
        using var innerPen = new Pen(Color.FromArgb(80, _config.PrimaryColor), 1f);
        g.DrawEllipse(innerPen, innerRect);

        // ── AUSGABE ────────────────────────────────────────────
    }

    /// <summary>Zeichnet alle Stundenlinien mit Beschriftungen auf das Ziffernblatt.</summary>
    private void DrawHourLines(
        Graphics     g,
        PointF       center,
        float        radius,
        SolarData    solarData,
        Location     location,
        TimeZoneInfo timeZone)
    {
        // ── EINGABE ────────────────────────────────────────────
        var hourLines    = SundialCalculator.CalculateAllHourLines(location.Latitude);
        DateTime solarNoon = TimeZoneInfo.ConvertTimeFromUtc(solarData.SolarNoon, timeZone);

        using var linePen   = new Pen(_config.SecondaryColor, 1.5f);
        using var labelFont = new Font(_config.FontFamily, _config.FontSizeBase * 0.8f, FontStyle.Regular);
        using var labelBrush = new SolidBrush(_config.SecondaryColor);

        // ── VERARBEITUNG ───────────────────────────────────────
        foreach (var (offset, angleDeg) in hourLines)
        {
            // Winkel: Mittagslinie zeigt nach Norden auf der Karte (270° im Bildkoordinatensystem)
            // Konvention: 0° = Süden (unten), positiv = im Uhrzeigersinn = Osten
            double lineAngleRad = SundialCalculator.DegreesToRadians(angleDeg - 90.0);

            float x2 = center.X + radius * 0.88f * (float)Math.Cos(lineAngleRad);
            float y2 = center.Y + radius * 0.88f * (float)Math.Sin(lineAngleRad);

            g.DrawLine(linePen, center, new PointF(x2, y2));

            // Stunden-Beschriftung außen platzieren
            float labelX = center.X + radius * 0.95f * (float)Math.Cos(lineAngleRad);
            float labelY = center.Y + radius * 0.95f * (float)Math.Sin(lineAngleRad);

            string label = _config.UseRomanNumerals
                ? ToRoman(solarNoon.AddHours(offset).Hour)
                : SundialCalculator.GetHourLabel(offset, solarNoon);

            var labelSize   = g.MeasureString(label, labelFont);
            var labelOrigin = new PointF(labelX - labelSize.Width / 2f, labelY - labelSize.Height / 2f);
            g.DrawString(label, labelFont, labelBrush, labelOrigin);
        }

        // ── AUSGABE ────────────────────────────────────────────
    }

    /// <summary>Zeichnet den Gnomon (Schattenwerfer) der Sonnenuhr.</summary>
    private void DrawGnomon(Graphics g, PointF center, float radius, double latitudeDegrees)
    {
        // ── EINGABE ────────────────────────────────────────────
        float gnomonHeight = radius * 0.35f;

        // ── VERARBEITUNG ───────────────────────────────────────
        // Gnomon zeigt in Richtung Norden (nach oben im Bild = 270°)
        double gnomonAngleRad = SundialCalculator.DegreesToRadians(-90.0);
        float tipX = center.X + gnomonHeight * (float)Math.Cos(gnomonAngleRad);
        float tipY = center.Y + gnomonHeight * (float)Math.Sin(gnomonAngleRad);

        using var gnomonPen = new Pen(_config.PrimaryColor, 3f) { EndCap = LineCap.ArrowAnchor };
        g.DrawLine(gnomonPen, center, new PointF(tipX, tipY));

        // Mittelpunkt-Kreis (Gnomon-Fußpunkt)
        float dotRadius = radius * 0.025f;
        using var dotBrush = new SolidBrush(_config.PrimaryColor);
        g.FillEllipse(dotBrush,
            center.X - dotRadius, center.Y - dotRadius,
            dotRadius * 2, dotRadius * 2);

        // Breitengrad-Beschriftung am Gnomon
        using var gnomonFont  = new Font(_config.FontFamily, _config.FontSizeBase * 0.7f, FontStyle.Italic);
        using var gnomonBrush = new SolidBrush(Color.FromArgb(180, _config.PrimaryColor));
        g.DrawString($"φ = {latitudeDegrees:F2}°", gnomonFont, gnomonBrush,
                     center.X + 8, center.Y - gnomonHeight * 0.6f);

        // ── AUSGABE ────────────────────────────────────────────
    }

    /// <summary>Zeichnet den aktuellen Schattenzeigerposition (wenn Sonne über Horizont).</summary>
    private void DrawCurrentTimeIndicator(
        Graphics     g,
        PointF       center,
        float        radius,
        SolarData    solarData,
        Location     location,
        DateTime     currentTime,
        TimeZoneInfo timeZone)
    {
        // ── EINGABE ────────────────────────────────────────────
        if (!_config.ShowCurrentHourMarker) return;

        DateTime solarNoon = TimeZoneInfo.ConvertTimeFromUtc(solarData.SolarNoon, timeZone);
        double? shadowAngle = SundialCalculator.CalculateCurrentShadowAngle(
                                  currentTime, solarNoon, location.Latitude);

        if (shadowAngle is null) return;

        // ── VERARBEITUNG ───────────────────────────────────────
        double lineAngleRad = SundialCalculator.DegreesToRadians(shadowAngle.Value - 90.0);
        float x2 = center.X + radius * 0.80f * (float)Math.Cos(lineAngleRad);
        float y2 = center.Y + radius * 0.80f * (float)Math.Sin(lineAngleRad);

        using var shadowPen = new Pen(_config.AccentColor, 4f)
        {
            EndCap    = LineCap.ArrowAnchor,
            DashStyle = DashStyle.Solid
        };

        // Leuchtender Schatten-Effekt (Glow)
        using var glowPen = new Pen(Color.FromArgb(60, _config.AccentColor), 12f);
        g.DrawLine(glowPen, center, new PointF(x2, y2));
        g.DrawLine(shadowPen, center, new PointF(x2, y2));

        // ── AUSGABE ────────────────────────────────────────────
    }

    /// <summary>Zeichnet das Informationspanel mit Standort- und Sonnenzeitdaten.</summary>
    private void DrawInfoPanel(
        Graphics     g,
        SolarData    solarData,
        Location     location,
        DateTime     currentTime,
        TimeZoneInfo timeZone)
    {
        // ── EINGABE ────────────────────────────────────────────
        float panelX = _config.ImageWidth * 0.04f;
        float panelY = _config.ImageHeight * 0.06f;

        using var panelBrush = new SolidBrush(Color.FromArgb(120, 10, 10, 30));
        using var borderPen  = new Pen(Color.FromArgb(80, _config.PrimaryColor), 1f);
        using var titleFont  = new Font(_config.FontFamily, _config.FontSizeBase * 1.1f, FontStyle.Bold);
        using var infoFont   = new Font(_config.FontFamily, _config.FontSizeBase * 0.85f, FontStyle.Regular);
        using var textBrush  = new SolidBrush(_config.SecondaryColor);
        using var accentBrush = new SolidBrush(_config.AccentColor);

        // ── VERARBEITUNG ───────────────────────────────────────
        var lines = new List<(string text, bool accent)>();

        if (_config.ShowLocationName)
            lines.Add(($"☀  {location}", true));

        if (_config.ShowCurrentTime)
            lines.Add(($"Uhrzeit:     {currentTime:HH:mm}", false));

        if (_config.ShowSunriseSunset)
        {
            DateTime sunrise = solarData.GetLocalSunrise(timeZone);
            DateTime sunset  = solarData.GetLocalSunset(timeZone);
            lines.Add(($"Sonnenaufgang:  {sunrise:HH:mm}", false));
            lines.Add(($"Sonnenuntergang: {sunset:HH:mm}", false));
            lines.Add(($"Tageslänge:   {solarData.DayLength:hh\\:mm} Std.", false));
        }

        // Panel-Hintergrund zeichnen
        float panelWidth  = 340f;
        float lineHeight  = _config.FontSizeBase * 1.8f;
        float panelHeight = lines.Count * lineHeight + 20f;

        g.FillRectangle(panelBrush,
            panelX - 10, panelY - 10, panelWidth, panelHeight);
        g.DrawRectangle(borderPen,
            panelX - 10, panelY - 10, panelWidth, panelHeight);

        // Textzeilen ausgeben
        for (int i = 0; i < lines.Count; i++)
        {
            var brush = lines[i].accent ? accentBrush : textBrush;
            var font  = lines[i].accent ? titleFont : infoFont;
            g.DrawString(lines[i].text, font, brush, panelX, panelY + i * lineHeight);
        }

        // ── AUSGABE ────────────────────────────────────────────
    }

    // ── HILFSMETHODEN ─────────────────────────────────────────

    /// <summary>Konvertiert eine Ganzzahl in eine römische Zifferndarstellung (I–XII).</summary>
    private static string ToRoman(int number)
    {
        // ── EINGABE ────────────────────────────────────────────
        number = ((number % 12) + 12) % 12;
        if (number == 0) number = 12;

        // ── VERARBEITUNG & AUSGABE ─────────────────────────────
        return number switch
        {
            1 => "I",   2 => "II",   3 => "III", 4 => "IV",
            5 => "V",   6 => "VI",   7 => "VII", 8 => "VIII",
            9 => "IX",  10 => "X",   11 => "XI", 12 => "XII",
            _ => number.ToString()
        };
    }
}
