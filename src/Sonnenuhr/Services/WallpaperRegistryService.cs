// ============================================================
// Datei:     WallpaperRegistryService.cs
// Projekt:   Sonnenuhr – Standortspezifischer Wallpaper-Generator
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Kapselt die Windows-Registry-Operationen zur Konfiguration des
//   Desktop-Hintergrunds sowie zum optionalen Autostart der Anwendung.
// ============================================================

using Microsoft.Win32;

namespace Sonnenuhr.Services;

/// <summary>
/// Setzt den Windows-Desktop-Hintergrund über die Registry-API
/// und verwaltet den Autostart-Eintrag der Anwendung.
/// </summary>
public static class WallpaperRegistryService
{
    // ── REGISTRY-SCHLÜSSEL ─────────────────────────────────────

    private const string DesktopRegKey  = @"Control Panel\Desktop";
    private const string AutostartRegKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName         = "Sonnenuhr";

    // ─────────────────────────────────────────────────────────────────────────
    // WALLPAPER SETZEN
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Setzt den Windows-Desktop-Hintergrund auf das angegebene Bild.
    /// </summary>
    /// <param name="imagePath">Vollständiger Dateipfad zum Wallpaper-Bild (PNG oder BMP).</param>
    /// <exception cref="ArgumentException">Wenn der Pfad leer oder ungültig ist.</exception>
    /// <exception cref="FileNotFoundException">Wenn die Bilddatei nicht existiert.</exception>
    public static void SetWallpaper(string imagePath)
    {
        // ── EINGABE ────────────────────────────────────────────
        if (string.IsNullOrWhiteSpace(imagePath))
            throw new ArgumentException("Der Bildpfad darf nicht leer sein.", nameof(imagePath));

        if (!File.Exists(imagePath))
            throw new FileNotFoundException("Die Wallpaper-Datei wurde nicht gefunden.", imagePath);

        // ── VERARBEITUNG ───────────────────────────────────────
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(DesktopRegKey, writable: true);
        if (key is not null)
        {
            // Hintergrundstil: 6 = Angepasst/Gefüllt (Fill), 10 = Gestreckt, 0 = Zentriert
            key.SetValue("WallpaperStyle", "6");
            key.SetValue("TileWallpaper",  "0");
        }

        // Windows-API über SystemParametersInfo aufrufen
        NativeMethods.SetDesktopWallpaper(imagePath);

        // ── AUSGABE ────────────────────────────────────────────
        // Kein Rückgabewert – der Desktop wurde aktualisiert.
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AUTOSTART
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Aktiviert oder deaktiviert den automatischen Programmstart mit Windows.
    /// </summary>
    /// <param name="enable"><c>true</c> = Autostart aktivieren; <c>false</c> = deaktivieren.</param>
    public static void SetAutostart(bool enable)
    {
        // ── EINGABE ────────────────────────────────────────────
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(AutostartRegKey, writable: true);
        if (key is null) return;

        // ── VERARBEITUNG ───────────────────────────────────────
        if (enable)
        {
            string exePath = Environment.ProcessPath ?? Application.ExecutablePath;
            key.SetValue(AppName, $"\"{exePath}\" --minimized");
        }
        else
        {
            key.DeleteValue(AppName, throwOnMissingValue: false);
        }

        // ── AUSGABE ────────────────────────────────────────────
        // Kein Rückgabewert – Registry wurde aktualisiert.
    }

    /// <summary>
    /// Prüft, ob der Autostart-Eintrag der Anwendung in der Registry vorhanden ist.
    /// </summary>
    /// <returns><c>true</c>, wenn der Autostart aktiv ist.</returns>
    public static bool IsAutostartEnabled()
    {
        // ── EINGABE ────────────────────────────────────────────
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(AutostartRegKey);

        // ── VERARBEITUNG / AUSGABE ─────────────────────────────
        return key?.GetValue(AppName) is not null;
    }
}

// ── NATIVE METHODEN ────────────────────────────────────────

/// <summary>
/// Kapselt den nativen Windows-API-Aufruf zum Setzen des Desktophintergrunds.
/// </summary>
internal static class NativeMethods
{
    private const int SPI_SETDESKWALLPAPER = 0x0014;
    private const int SPIF_UPDATEINIFILE   = 0x01;
    private const int SPIF_SENDCHANGE      = 0x02;

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    private static extern int SystemParametersInfo(
        int uAction, int uParam, string lpvParam, int fuWinIni);

    /// <summary>
    /// Setzt den Desktophintergrund über den nativen Windows-API-Aufruf.
    /// </summary>
    /// <param name="imagePath">Vollständiger Bildpfad.</param>
    internal static void SetDesktopWallpaper(string imagePath) =>
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath,
                             SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
}
