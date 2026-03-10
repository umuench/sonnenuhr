// ============================================================
// Datei:     ConfigurationService.cs
// Projekt:   Sonnenuhr – Standortspezifischer Wallpaper-Generator
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Kapselt das Lesen und Schreiben der JSON-Konfigurationsdateien.
//   Speicherort: %APPDATA%\Sonnenuhr\settings.json
// ============================================================

using System.Text.Json;
using Sonnenuhr.Models;

namespace Sonnenuhr.Services;

/// <summary>
/// Verwaltet das persistente Laden und Speichern der Anwendungseinstellungen
/// als JSON-Datei im Benutzer-AppData-Verzeichnis.
/// </summary>
public class ConfigurationService
{
    // ── FELDER ─────────────────────────────────────────────────

    private readonly string _configFilePath;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented            = true,
        PropertyNamingPolicy     = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition   = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    // ── KONSTRUKTOR ────────────────────────────────────────────

    /// <summary>
    /// Initialisiert den Service und legt den Konfigurationspfad fest.
    /// </summary>
    public ConfigurationService()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string appFolder   = Path.Combine(appDataPath, "Sonnenuhr");
        Directory.CreateDirectory(appFolder);
        _configFilePath = Path.Combine(appFolder, "settings.json");
    }

    // ── ÖFFENTLICHE METHODEN ───────────────────────────────────

    /// <summary>
    /// Lädt die Anwendungseinstellungen aus der JSON-Datei.
    /// Existiert keine Datei, werden Standardeinstellungen zurückgegeben.
    /// </summary>
    /// <returns>Geladene oder neu erstellte <see cref="AppSettings"/>.</returns>
    public AppSettings LoadSettings()
    {
        // ── EINGABE ────────────────────────────────────────────
        if (!File.Exists(_configFilePath))
            return new AppSettings();

        // ── VERARBEITUNG ───────────────────────────────────────
        try
        {
            string jsonContent = File.ReadAllText(_configFilePath);
            var settings = JsonSerializer.Deserialize<AppSettings>(jsonContent, SerializerOptions);

            // ── AUSGABE ────────────────────────────────────────
            return settings ?? new AppSettings();
        }
        catch (JsonException)
        {
            // Fehlerhafte Konfiguration → Standardwerte verwenden
            return new AppSettings();
        }
    }

    /// <summary>
    /// Speichert die Anwendungseinstellungen in die JSON-Datei.
    /// </summary>
    /// <param name="settings">Zu speichernde Einstellungen.</param>
    /// <exception cref="ArgumentNullException">Wenn <paramref name="settings"/> null ist.</exception>
    /// <exception cref="IOException">Schreibfehler beim Persistieren.</exception>
    public void SaveSettings(AppSettings settings)
    {
        // ── EINGABE ────────────────────────────────────────────
        ArgumentNullException.ThrowIfNull(settings);

        // ── VERARBEITUNG ───────────────────────────────────────
        string jsonContent = JsonSerializer.Serialize(settings, SerializerOptions);

        // ── AUSGABE ────────────────────────────────────────────
        File.WriteAllText(_configFilePath, jsonContent);
    }

    /// <summary>
    /// Gibt den vollständigen Pfad der Konfigurationsdatei zurück.
    /// </summary>
    public string GetConfigFilePath() => _configFilePath;
}
