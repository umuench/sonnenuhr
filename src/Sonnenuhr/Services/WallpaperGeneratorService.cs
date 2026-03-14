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
    // ── KONSTANTEN ─────────────────────────────────────────────

    /// <summary>Dauer der Ein-/Ausblendung in Minuten um Sonnenauf-/untergang.</summary>
    private const float FadeDurationMinutes = 30f;

    /// <summary>Minimaler Alphawert bei Nacht (Geisteruhr, 8 % sichtbar).</summary>
    private const float NightAlpha = 0.08f;

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
        using var bitmap   = new Bitmap(_config.ImageWidth, _config.ImageHeight, PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(bitmap);

        ConfigureGraphics(graphics);

        // Lokale Sonnenzeiten für Hintergrund und Alpha-Berechnung
        DateTime localSunrise = solarData.GetLocalSunrise(timeZone);
        DateTime localSunset  = solarData.GetLocalSunset(timeZone);

        DrawBackground(graphics, currentTime, localSunrise, localSunset);

        // Sonnenuhr-Mittelpunkt und Radius bestimmen
        var center = new PointF(_config.ImageWidth / 2f, _config.ImageHeight / 2f);
        float radius = Math.Min(_config.ImageWidth, _config.ImageHeight) * 0.38f;

        // Alpha-Faktor: Einblenden nach Sonnenaufgang, Ausblenden vor Sonnenuntergang
        float clockAlpha = CalculateDaylightAlpha(currentTime, localSunrise, localSunset);

        // Alle Uhr-Elemente auf eine transparente Zwischenfläche zeichnen
        using var clockBitmap   = new Bitmap(_config.ImageWidth, _config.ImageHeight, PixelFormat.Format32bppArgb);
        using var clockGraphics = Graphics.FromImage(clockBitmap);
        ConfigureGraphics(clockGraphics);

        DrawDialPlate(clockGraphics, center, radius);
        DrawHourLines(clockGraphics, center, radius, solarData, location, timeZone);
        DrawGnomon(clockGraphics, center, radius, location.Latitude);
        DrawCurrentTimeIndicator(clockGraphics, center, radius, solarData, location, currentTime, timeZone);

        if (_config.ShowLocationName || _config.ShowSunriseSunset || _config.ShowCurrentTime)
            DrawInfoPanel(clockGraphics, solarData, location, currentTime, timeZone);

        // Zwischenfläche mit berechneter Transparenz auf Hauptbild komponieren
        CompositeBitmapWithAlpha(graphics, clockBitmap, clockAlpha);

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

    /// <summary>
    /// Zeichnet den animierten Himmelshintergrund: Himmelsgradient, Sterne,
    /// Dämmerungsglow und Sonne/Mond auf ihrer Bogenbahn.
    /// </summary>
    private void DrawBackground(Graphics g, DateTime currentTime, DateTime sunrise, DateTime sunset)
    {
        // ── EINGABE ────────────────────────────────────────────
        float w        = _config.ImageWidth;
        float h        = _config.ImageHeight;
        float horizonY = h * 0.72f;  // Horizontlinie bei 72 % der Bildhöhe

        // ── VERARBEITUNG ───────────────────────────────────────

        // 1. Himmelsgradient (Bildkante → Horizont)
        (Color skyTop, Color skyHorizon) = GetSkyColors(currentTime, sunrise, sunset);
        using (var skyBrush = new LinearGradientBrush(
            new PointF(0, 0), new PointF(0, horizonY), skyTop, skyHorizon))
            g.FillRectangle(skyBrush, 0, 0, w, (int)horizonY + 1);

        // 2. Bodenbereich (Horizont → unterer Rand, gedämpfter)
        using (var groundBrush = new LinearGradientBrush(
            new PointF(0, horizonY), new PointF(0, h),
            skyHorizon, DarkenColor(skyHorizon, 0.45f)))
            g.FillRectangle(groundBrush, 0, (int)horizonY, (int)w, (int)(h - horizonY) + 1);

        // 3. Sterne bei Nacht und Dämmerung
        DrawStars(g, currentTime, sunrise, sunset, horizonY);

        // 4. Horizontglow (orange/pink) bei Dämmerung
        DrawHorizonGlow(g, currentTime, sunrise, sunset, horizonY);

        // 5. Sonne (tags) oder Mond (nachts) auf astronomisch berechnetem Bogen
        bool isDaytime = currentTime >= sunrise && currentTime <= sunset;
        if (isDaytime)
        {
            float t      = (float)Math.Clamp(
                (currentTime - sunrise).TotalSeconds / (sunset - sunrise).TotalSeconds, 0.0, 1.0);
            float alpha  = GetCelestialAlpha(currentTime, sunrise, sunset, isDaytime: true);
            DrawSun(g, CalculateCelestialPosition(t, w, horizonY), alpha);
        }
        else
        {
            float moonT  = CalculateMoonT(currentTime, sunrise, sunset);
            float alpha  = GetCelestialAlpha(currentTime, sunrise, sunset, isDaytime: false);
            DrawMoon(g, CalculateCelestialPosition(moonT, w, horizonY), alpha,
                     CalculateMoonPhase(currentTime));
        }

        // ── AUSGABE ────────────────────────────────────────────
    }

    // ── HIMMEL & ATMOSPHÄRE ────────────────────────────────────

    /// <summary>
    /// Liefert den Himmelsgradient (Zenith/Horizont) passend zur Tageszeit.
    /// Stützpunkte: Tiefe Nacht → Astronomische Dämmerung → Nautische Dämmerung →
    /// Bürgerliche Dämmerung → Sonnenauf/-untergang → Goldene Stunde → Tagesmitte.
    /// </summary>
    private static (Color top, Color horizon) GetSkyColors(
        DateTime current, DateTime sunrise, DateTime sunset)
    {
        // ── EINGABE ────────────────────────────────────────────
        double fromSunrise = (current - sunrise).TotalMinutes;
        double toSunset    = (sunset  - current).TotalMinutes;

        // ── VERARBEITUNG ───────────────────────────────────────
        // Farbstützpunkte (top, horizon)
        var night   = (Color.FromArgb(  4,  6, 14), Color.FromArgb(  6,  9, 18));
        var astro   = (Color.FromArgb(  7,  9, 22), Color.FromArgb( 14, 10, 30));
        var naut    = (Color.FromArgb( 10, 13, 32), Color.FromArgb( 28, 12, 46));
        var civil   = (Color.FromArgb( 14, 19, 52), Color.FromArgb( 68, 24, 58));
        var event_  = (Color.FromArgb( 22, 36, 82), Color.FromArgb(215, 92, 18));
        var golden  = (Color.FromArgb( 24, 68,162), Color.FromArgb(218,115, 25));
        var day     = (Color.FromArgb( 16, 82,204), Color.FromArgb( 52,142,232));

        // Tagsüber
        if (fromSunrise >= 0 && toSunset >= 0)
        {
            double mins = Math.Min(fromSunrise, toSunset);
            if (mins >= 60) return day;
            float p = (float)(mins / 60.0);
            return (LerpColor(event_.Item1, golden.Item1, p),
                    LerpColor(event_.Item2, golden.Item2, p < 0.5f
                        ? p : 1f - (p - 0.5f) * 2f + 0.5f * p));
        }

        // Vor Sonnenaufgang oder nach Sonnenuntergang
        double nearMins = Math.Min(Math.Abs(fromSunrise), Math.Abs(toSunset));

        if (nearMins <= 10)  return event_;
        if (nearMins <= 30)  return Lerp2(civil,  event_, (float)((nearMins - 10) / 20.0));
        if (nearMins <= 60)  return Lerp2(naut,   civil,  (float)((nearMins - 30) / 30.0));
        if (nearMins <= 90)  return Lerp2(astro,  naut,   (float)((nearMins - 60) / 30.0));
        if (nearMins <= 120) return Lerp2(night,  astro,  (float)((nearMins - 90) / 30.0));

        // ── AUSGABE ────────────────────────────────────────────
        return night;
    }

    /// <summary>
    /// Zeichnet deterministisch positionierte Sterne im Himmelbereich.
    /// Gleiche Sterne pro Tag; leichtes stündliches Flackern.
    /// </summary>
    private void DrawStars(
        Graphics g, DateTime currentTime, DateTime sunrise, DateTime sunset, float horizonY)
    {
        // ── EINGABE ────────────────────────────────────────────
        double fromSunrise = (currentTime - sunrise).TotalMinutes;
        double toSunset    = (sunset - currentTime).TotalMinutes;
        bool   isDaytime   = fromSunrise >= 0 && toSunset >= 0;

        // Alpha: erscheint 60 min nach Sonnenuntergang voll, verschwindet 60 min nach Aufgang
        float nearMins  = (float)Math.Min(Math.Abs(fromSunrise), Math.Abs(toSunset));
        float starsAlpha;
        if (!isDaytime)
            starsAlpha = nearMins >= 60 ? 1f : nearMins / 60f;
        else if (fromSunrise < 30)
            starsAlpha = 1f - (float)(fromSunrise / 30.0);
        else if (toSunset < 30)
            starsAlpha = 1f - (float)(toSunset / 30.0);
        else
            return;

        if (starsAlpha <= 0f) return;

        // ── VERARBEITUNG ───────────────────────────────────────
        // Tages-Seed → gleiche Sternpositionen pro Tag
        var rng = new Random(currentTime.Date.GetHashCode() ^ 0x3C1A5F2B);

        for (int i = 0; i < 180; i++)
        {
            float x    = (float)rng.NextDouble() * _config.ImageWidth;
            float y    = (float)rng.NextDouble() * horizonY * 0.95f;
            float size = (float)(rng.NextDouble() * 2.1 + 0.4);
            int   base_brightness = rng.Next(140, 255);

            // Stündliches Flackern pro Stern (nicht bei jedem Refresh gleich)
            var   flickerRng   = new Random(i * 137 ^ currentTime.Hour);
            float flicker      = 0.72f + 0.28f * (float)flickerRng.NextDouble();
            int   alpha        = Math.Clamp((int)(starsAlpha * base_brightness * flicker), 0, 255);

            // Leicht bläulicher Weißton für realistisches Sternenlicht
            int r = Math.Clamp(base_brightness - 10, 0, 255);
            int bv = Math.Clamp(base_brightness + 15, 0, 255);

            using var starBrush = new SolidBrush(Color.FromArgb(alpha, r, r, bv));
            g.FillEllipse(starBrush, x - size / 2f, y - size / 2f, size, size);
        }

        // ── AUSGABE ────────────────────────────────────────────
    }

    /// <summary>
    /// Zeichnet den orangefarbenen Horizontglow bei Dämmerung (Ost = Aufgang, West = Untergang).
    /// </summary>
    private void DrawHorizonGlow(
        Graphics g, DateTime currentTime, DateTime sunrise, DateTime sunset, float horizonY)
    {
        // ── EINGABE ────────────────────────────────────────────
        double fromSunrise = (currentTime - sunrise).TotalMinutes;
        double toSunset    = (sunset - currentTime).TotalMinutes;

        // ── VERARBEITUNG ───────────────────────────────────────
        // Ost-Glow (Sonnenaufgang, links)
        if (Math.Abs(fromSunrise) < 60)
        {
            float a = 1f - (float)(Math.Abs(fromSunrise) / 60.0);
            DrawEllipticalGlow(g,
                _config.ImageWidth * 0.10f, horizonY,
                500f, 300f, a,
                Color.FromArgb(255, 130, 40));
        }

        // West-Glow (Sonnenuntergang, rechts)
        if (Math.Abs(toSunset) < 60)
        {
            float a = 1f - (float)(Math.Abs(toSunset) / 60.0);
            DrawEllipticalGlow(g,
                _config.ImageWidth * 0.90f, horizonY,
                500f, 300f, a,
                Color.FromArgb(255, 100, 15));
        }

        // ── AUSGABE ────────────────────────────────────────────
    }

    /// <summary>Zeichnet einen radialen Glow-Ellipsen am Horizont via PathGradientBrush.</summary>
    private static void DrawEllipticalGlow(
        Graphics g, float cx, float cy, float rx, float ry, float alpha, Color color)
    {
        // ── VERARBEITUNG ───────────────────────────────────────
        using var path = new GraphicsPath();
        path.AddEllipse(cx - rx, cy - ry, rx * 2, ry * 2);

        using var brush = new PathGradientBrush(path)
        {
            CenterPoint    = new PointF(cx, cy),
            CenterColor    = Color.FromArgb((int)(alpha * 160), color),
            SurroundColors = new[] { Color.FromArgb(0, color) }
        };

        // ── AUSGABE ────────────────────────────────────────────
        g.FillPath(brush, path);
    }

    // ── HIMMELSKÖRPER: SONNE ──────────────────────────────────

    /// <summary>
    /// Zeichnet die Sonne mit mehrschichtigem Glow an der berechneten Position.
    /// </summary>
    private static void DrawSun(Graphics g, PointF pos, float alpha)
    {
        // ── EINGABE ────────────────────────────────────────────
        if (alpha <= 0f) return;
        float sunR = 38f;

        // ── VERARBEITUNG & AUSGABE ─────────────────────────────
        // Äußere Glow-Schichten (von außen nach innen, zunehmend opak)
        (float radius, int baseAlpha)[] glowLayers =
        [
            (140f, 8), (108f, 16), (84f, 28), (66f, 45), (52f, 68)
        ];
        foreach (var (r, ba) in glowLayers)
        {
            using var b = new SolidBrush(Color.FromArgb((int)(alpha * ba), 255, 215, 80));
            g.FillEllipse(b, pos.X - r, pos.Y - r, r * 2, r * 2);
        }

        // Halo (weicher Übergang Gelb→Weiß)
        using var halo = new SolidBrush(Color.FromArgb((int)(alpha * 200), 255, 242, 160));
        g.FillEllipse(halo, pos.X - sunR * 1.35f, pos.Y - sunR * 1.35f, sunR * 2.7f, sunR * 2.7f);

        // Sonnendisc
        using var disc = new SolidBrush(Color.FromArgb((int)(alpha * 255), 255, 252, 210));
        g.FillEllipse(disc, pos.X - sunR, pos.Y - sunR, sunR * 2, sunR * 2);

        // Heller Kern
        float cr = sunR * 0.38f;
        using var core = new SolidBrush(Color.FromArgb((int)(alpha * 255), 255, 255, 245));
        g.FillEllipse(core, pos.X - cr, pos.Y - cr, cr * 2, cr * 2);
    }

    // ── HIMMELSKÖRPER: MOND ───────────────────────────────────

    /// <summary>
    /// Zeichnet den Mond mit Glow und astronomisch korrekter Phasendarstellung.
    /// </summary>
    private static void DrawMoon(Graphics g, PointF pos, float alpha, double phase)
    {
        // ── EINGABE ────────────────────────────────────────────
        if (alpha <= 0f) return;
        float moonR = 26f;

        // ── VERARBEITUNG & AUSGABE ─────────────────────────────
        // Mondglow (bläulich-weiß)
        (float radius, int baseAlpha)[] glowLayers = [(moonR + 52f, 8), (moonR + 34f, 15), (moonR + 18f, 26)];
        foreach (var (r, ba) in glowLayers)
        {
            using var b = new SolidBrush(Color.FromArgb((int)(alpha * ba), 190, 200, 255));
            g.FillEllipse(b, pos.X - r, pos.Y - r, r * 2, r * 2);
        }

        // Mondscheibe
        using var moonDisc = new SolidBrush(Color.FromArgb((int)(alpha * 240), 225, 225, 205));
        g.FillEllipse(moonDisc, pos.X - moonR, pos.Y - moonR, moonR * 2, moonR * 2);

        // Mondphase (Schatten überlagert unbeleuchtete Seite)
        DrawMoonPhase(g, pos, moonR, phase, alpha);
    }

    /// <summary>
    /// Überlagert die Mondscheibe mit einem Schattensegment entsprechend der Mondphase.
    /// Verwendet Clip + LinearGradientBrush für einen weichen Terminator.
    /// </summary>
    private static void DrawMoonPhase(Graphics g, PointF pos, float radius, double phase, float alpha)
    {
        // ── EINGABE ────────────────────────────────────────────
        // phase: 0 = Neumond, 0.5 = Vollmond, 1 = Neumond
        double fullnessFraction = 1.0 - Math.Abs(phase - 0.5) * 2.0; // 0=neu, 1=voll
        if (fullnessFraction > 0.98) return; // Vollmond: kein Schatten

        // ── VERARBEITUNG ───────────────────────────────────────
        // Beleuchteter Anteil: 0 (Neumond) → 1 (Vollmond)
        // Zunehmend (phase 0→0.5): Schatten auf der linken Seite
        // Abnehmend (phase 0.5→1): Schatten auf der rechten Seite
        bool waxing = phase < 0.5;

        // Terminatorlinie: x-Position des Schattenschwerpunkts
        // Bei Vollmond: Schatten komplett außerhalb; bei Neumond: Schatten über ganzer Scheibe
        float shadowCx = waxing
            ? pos.X - radius + radius * 2f * (float)fullnessFraction
            : pos.X + radius - radius * 2f * (float)fullnessFraction;

        // Clip auf Mondkreis begrenzen
        using var clipPath = new GraphicsPath();
        clipPath.AddEllipse(pos.X - radius, pos.Y - radius, radius * 2, radius * 2);
        var prevClip = g.Clip;
        g.SetClip(clipPath, CombineMode.Intersect);

        // Schattenbereich: weicher Übergang (LinearGradient über ~30 px Terminator-Breite)
        float terminatorWidth = radius * 0.35f;
        float gradX1 = shadowCx - terminatorWidth;
        float gradX2 = shadowCx + terminatorWidth;

        Color shadowColor = Color.FromArgb((int)(alpha * 215), 12, 16, 38);
        Color transparent = Color.FromArgb(0, 12, 16, 38);

        using var shadowBrush = new LinearGradientBrush(
            new PointF(gradX1, pos.Y),
            new PointF(gradX2, pos.Y),
            waxing ? shadowColor : transparent,  // zunehmend: Schatten links → Licht rechts
            waxing ? transparent : shadowColor); // abnehmend: Licht links → Schatten rechts

        g.FillRectangle(shadowBrush, pos.X - radius, pos.Y - radius, radius * 2, radius * 2);
        g.Clip = prevClip;

        // ── AUSGABE ────────────────────────────────────────────
    }

    // ── POSITION UND ALPHA DER HIMMELSKÖRPER ──────────────────

    /// <summary>
    /// Berechnet die Bildschirmposition eines Himmelskörpers auf einem Sinusbogen.
    /// t=0 → Horizont links (Osten), t=0.5 → Zenith (oben), t=1 → Horizont rechts (Westen).
    /// </summary>
    private PointF CalculateCelestialPosition(float t, float imageWidth, float horizonY)
    {
        // ── VERARBEITUNG ───────────────────────────────────────
        float topY    = _config.ImageHeight * 0.08f;         // maximale Bogenhöhe oben
        float arcH    = horizonY - topY;
        float x       = imageWidth * (0.07f + 0.86f * t);
        float y       = horizonY - arcH * (float)Math.Sin(t * Math.PI);

        // ── AUSGABE ────────────────────────────────────────────
        return new PointF(x, Math.Max(topY, y));
    }

    /// <summary>
    /// Berechnet den Fortschritt des Mondes durch die Nacht (0 = Sonnenuntergang, 1 = Sonnenaufgang).
    /// </summary>
    private static float CalculateMoonT(DateTime current, DateTime sunrise, DateTime sunset)
    {
        // ── VERARBEITUNG ───────────────────────────────────────
        double dayMinutes   = (sunset - sunrise).TotalMinutes;
        double nightMinutes = 1440.0 - dayMinutes;
        double minutesIntoNight;

        if (current >= sunset)
            minutesIntoNight = (current - sunset).TotalMinutes;
        else
            minutesIntoNight = nightMinutes - (sunrise - current).TotalMinutes;

        // ── AUSGABE ────────────────────────────────────────────
        return (float)Math.Clamp(minutesIntoNight / nightMinutes, 0.0, 1.0);
    }

    /// <summary>
    /// Gibt die Deckkraft der Sonne oder des Mondes zurück.
    /// Nahe dem Horizont (±20 min) werden die Objekte weich ein-/ausgeblendet.
    /// </summary>
    private static float GetCelestialAlpha(
        DateTime current, DateTime sunrise, DateTime sunset, bool isDaytime)
    {
        // ── VERARBEITUNG ───────────────────────────────────────
        const double fadeMinutes = 20.0;

        if (isDaytime)
        {
            double fromSunrise = (current - sunrise).TotalMinutes;
            double toSunset    = (sunset  - current).TotalMinutes;
            double nearEdge    = Math.Min(fromSunrise, toSunset);
            return nearEdge >= fadeMinutes ? 1f : (float)(nearEdge / fadeMinutes);
        }
        else
        {
            double fromSunrise = Math.Abs((current - sunrise).TotalMinutes);
            double toSunset    = Math.Abs((sunset  - current).TotalMinutes);
            double nearEdge    = Math.Min(fromSunrise, toSunset);
            return nearEdge >= fadeMinutes ? 1f : (float)(nearEdge / fadeMinutes);
        }
    }

    /// <summary>
    /// Berechnet die aktuelle Mondphase (0 = Neumond, 0.5 = Vollmond, 1 = Neumond).
    /// Basiert auf dem bekannten Neumond vom 6. Januar 2000.
    /// </summary>
    private static double CalculateMoonPhase(DateTime date)
    {
        // ── VERARBEITUNG ───────────────────────────────────────
        const double synodicMonth  = 29.53058867; // mittlere Länge des Synodischen Monats in Tagen
        var          knownNewMoon  = new DateTime(2000, 1, 6, 18, 14, 0, DateTimeKind.Utc);
        double       daysSinceNew  = (date.ToUniversalTime() - knownNewMoon).TotalDays;
        double       phase         = (daysSinceNew % synodicMonth + synodicMonth) % synodicMonth;

        // ── AUSGABE ────────────────────────────────────────────
        return phase / synodicMonth; // 0 = Neumond, 0.5 = Vollmond
    }

    // ── FARB-HILFSMETHODEN ────────────────────────────────────

    /// <summary>Interpoliert linear zwischen zwei Farben. t=0 → a, t=1 → b.</summary>
    private static Color LerpColor(Color a, Color b, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return Color.FromArgb(
            (int)(a.R + (b.R - a.R) * t),
            (int)(a.G + (b.G - a.G) * t),
            (int)(a.B + (b.B - a.B) * t));
    }

    /// <summary>Interpoliert linear zwischen zwei Farb-Tupeln (top, horizon).</summary>
    private static (Color top, Color horizon) Lerp2(
        (Color top, Color horizon) a,
        (Color top, Color horizon) b,
        float t) =>
        (LerpColor(a.top, b.top, t), LerpColor(a.horizon, b.horizon, t));

    /// <summary>Gibt eine um den angegebenen Faktor abgedunkelte Farbe zurück.</summary>
    private static Color DarkenColor(Color c, float factor)
    {
        factor = 1f - Math.Clamp(factor, 0f, 1f);
        return Color.FromArgb(
            (int)(c.R * factor),
            (int)(c.G * factor),
            (int)(c.B * factor));
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
        DateTime solarNoon = solarData.GetLocalSolarNoon(timeZone);

        using var linePen   = new Pen(_config.SecondaryColor, 1.5f);
        using var labelFont = new Font(_config.FontFamily, _config.FontSizeBase * 0.8f, FontStyle.Regular);
        using var labelBrush = new SolidBrush(_config.SecondaryColor);

        // ── VERARBEITUNG ───────────────────────────────────────
        foreach (var (offset, angleDeg) in hourLines)
        {
            // Winkel: Mittagslinie zeigt nach Norden auf der Karte (−90° im Bildkoordinatensystem)
            // Konvention: 0° = Mittagslinie (Nord), positiv = im Uhrzeigersinn = Osten
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

        DateTime solarNoon    = solarData.GetLocalSolarNoon(timeZone);
        DateTime localSunrise = solarData.GetLocalSunrise(timeZone);
        DateTime localSunset  = solarData.GetLocalSunset(timeZone);

        double? shadowAngle = SundialCalculator.CalculateCurrentShadowAngle(
                                  currentTime, solarNoon, location.Latitude,
                                  sunriseLocal: localSunrise,
                                  sunsetLocal:  localSunset);

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
        using var panelBrush  = new SolidBrush(Color.FromArgb(120, 10, 10, 30));
        using var borderPen   = new Pen(Color.FromArgb(80, _config.PrimaryColor), 1f);
        using var titleFont   = new Font(_config.FontFamily, _config.FontSizeBase * 1.1f, FontStyle.Bold);
        using var infoFont    = new Font(_config.FontFamily, _config.FontSizeBase * 0.85f, FontStyle.Regular);
        using var textBrush   = new SolidBrush(_config.SecondaryColor);
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

        if (lines.Count == 0) return;

        // Schriftgrößen-abhängige Maße ermitteln
        const float padding    = 16f;
        float lineHeight = _config.FontSizeBase * 1.8f;

        // Breiteste Zeile messen → Panel passt sich der Schriftgröße an
        float maxTextWidth = 0f;
        foreach (var (text, accent) in lines)
        {
            var   font  = accent ? titleFont : infoFont;
            float width = g.MeasureString(text, font).Width;
            if (width > maxTextWidth) maxTextWidth = width;
        }

        float panelWidth  = maxTextWidth + padding * 2f;
        float panelHeight = lines.Count * lineHeight + padding;

        // Rechts ausrichten: rechter Rand = 4 % der Bildbreite
        float rightMargin = _config.ImageWidth * 0.04f;
        float panelX      = _config.ImageWidth - rightMargin - panelWidth;
        float panelY      = _config.ImageHeight * 0.06f;

        // Panel-Hintergrund und Rahmen zeichnen
        g.FillRectangle(panelBrush, panelX, panelY, panelWidth, panelHeight);
        g.DrawRectangle(borderPen,  panelX, panelY, panelWidth, panelHeight);

        // Textzeilen einheitlich mit innerem Abstand ausgeben
        for (int i = 0; i < lines.Count; i++)
        {
            var brush = lines[i].accent ? accentBrush : textBrush;
            var font  = lines[i].accent ? titleFont   : infoFont;
            g.DrawString(lines[i].text, font, brush,
                         panelX + padding,
                         panelY + padding / 2f + i * lineHeight);
        }

        // ── AUSGABE ────────────────────────────────────────────
    }

    // ── HILFSMETHODEN ─────────────────────────────────────────

    /// <summary>
    /// Berechnet den Deckkraft-Faktor der Uhr (0 = unsichtbar, 1 = vollständig sichtbar)
    /// auf Basis der aktuellen Zeit relativ zu Sonnenaufgang und -untergang.
    /// </summary>
    /// <remarks>
    /// Zeitstrahl:
    /// ←─ Nacht ─→←─ Fade-In ─→←── voller Tag ──→←─ Fade-Out ─→←─ Nacht ─→
    ///            ↑           ↑                  ↑              ↑
    ///        sunrise-30min  sunrise      sunset-30min       sunset
    /// </remarks>
    private static float CalculateDaylightAlpha(
        DateTime currentTime,
        DateTime sunrise,
        DateTime sunset)
    {
        // ── EINGABE ────────────────────────────────────────────
        var fadeDuration = TimeSpan.FromMinutes(FadeDurationMinutes);

        DateTime fadeInStart  = sunrise - fadeDuration;   // Start Einblenden (30 min vor Aufgang)
        DateTime fadeOutStart = sunset  - fadeDuration;   // Start Ausblenden (30 min vor Untergang)

        // ── VERARBEITUNG ───────────────────────────────────────

        // Tief in der Nacht
        if (currentTime <= fadeInStart || currentTime >= sunset + fadeDuration)
            return NightAlpha;

        // Einblenden: fadeInStart → sunrise (NightAlpha → 1.0)
        if (currentTime < sunrise)
        {
            double progress = (currentTime - fadeInStart).TotalMinutes / FadeDurationMinutes;
            return NightAlpha + (1f - NightAlpha) * (float)Math.Clamp(progress, 0.0, 1.0);
        }

        // Ausblenden: fadeOutStart → sunset (1.0 → NightAlpha)
        if (currentTime >= fadeOutStart)
        {
            double progress = (sunset - currentTime).TotalMinutes / FadeDurationMinutes;
            return NightAlpha + (1f - NightAlpha) * (float)Math.Clamp(progress, 0.0, 1.0);
        }

        // ── AUSGABE ────────────────────────────────────────────
        return 1.0f; // Voller Tag: vollständig sichtbar
    }

    /// <summary>
    /// Komponiert eine Quell-Bitmap mit dem angegebenen Alpha-Faktor
    /// auf die Ziel-Graphics-Fläche (Multiply auf den Alpha-Kanal).
    /// </summary>
    /// <param name="g">Ziel-Graphics.</param>
    /// <param name="source">Quell-Bitmap (32bpp ARGB).</param>
    /// <param name="alpha">Deckkraft-Faktor 0.0–1.0.</param>
    private static void CompositeBitmapWithAlpha(Graphics g, Bitmap source, float alpha)
    {
        // ── EINGABE ────────────────────────────────────────────
        if (alpha <= 0f) return;
        if (alpha >= 1f) { g.DrawImage(source, 0, 0); return; }

        // ── VERARBEITUNG ───────────────────────────────────────
        // ColorMatrix.Matrix33 skaliert den Alpha-Kanal aller Pixel
        var colorMatrix = new ColorMatrix { Matrix33 = alpha };
        using var imageAttribs = new ImageAttributes();
        imageAttribs.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

        var destRect = new Rectangle(0, 0, source.Width, source.Height);

        // ── AUSGABE ────────────────────────────────────────────
        g.DrawImage(source, destRect,
                    0, 0, source.Width, source.Height,
                    GraphicsUnit.Pixel, imageAttribs);
    }

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
