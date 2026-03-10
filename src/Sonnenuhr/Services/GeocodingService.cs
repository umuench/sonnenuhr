// ============================================================
// Datei:     GeocodingService.cs
// Projekt:   Sonnenuhr – Standortspezifischer Wallpaper-Generator
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Service für die Stadtsuche mittels der kostenlosen
//   OpenStreetMap Nominatim API (kein API-Key erforderlich).
//   Liefert eine sortierte Liste von GeocodingResult-Objekten
//   mit Koordinaten und Anzeigenamen.
// ============================================================

using System.Text.Json;
using Sonnenuhr.Models;

namespace Sonnenuhr.Services;

/// <summary>
/// Sucht Städte über die OpenStreetMap Nominatim API und gibt eine
/// Liste von <see cref="GeocodingResult"/>-Objekten zurück.
/// </summary>
/// <remarks>
/// Die Nominatim API erfordert gemäß Nutzungsbedingungen einen aussagekräftigen
/// <c>User-Agent</c>-Header und darf nicht mit hoher Frequenz abgefragt werden.
/// </remarks>
public sealed class GeocodingService : IDisposable
{
    // ── KONSTANTEN ─────────────────────────────────────────────

    private const string BaseUrl      = "https://nominatim.openstreetmap.org/search";
    private const string UserAgent    = "Sonnenuhr/1.0 (github.com/umuench/sonnenuhr; uwe.muench@breihof-it.de)";
    private const int    MaxResults   = 10;
    private const int    TimeoutSec   = 15;

    // ── FELDER ─────────────────────────────────────────────────

    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // ── KONSTRUKTOR ────────────────────────────────────────────

    /// <summary>Initialisiert den Service und konfiguriert den HTTP-Client.</summary>
    public GeocodingService()
    {
        // ── VERARBEITUNG ───────────────────────────────────────
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(TimeoutSec)
        };
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        // Accept-Language bevorzugt deutsche Ortsnamen
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("de,en;q=0.8");
    }

    // ── ÖFFENTLICHE METHODEN ───────────────────────────────────

    /// <summary>
    /// Sucht nach Städten, Gemeinden oder Orten anhand des übergebenen Namens.
    /// Die Ergebnisse sind nach Relevanz (<see cref="GeocodingResult.Importance"/>)
    /// absteigend sortiert.
    /// </summary>
    /// <param name="query">Suchbegriff (Stadtname, auch mit Länderzusatz wie „Eberbach, Deutschland").</param>
    /// <param name="cancellationToken">Optionales Abbruch-Token.</param>
    /// <returns>
    /// Geordnete schreibgeschützte Liste von <see cref="GeocodingResult"/>-Objekten.
    /// Gibt eine leere Liste zurück, wenn keine Treffer gefunden wurden oder der
    /// Suchbegriff leer war.
    /// </returns>
    /// <exception cref="HttpRequestException">Bei einem Netzwerkfehler.</exception>
    public async Task<IReadOnlyList<GeocodingResult>> SearchCityAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        // ── EINGABE ────────────────────────────────────────────
        if (string.IsNullOrWhiteSpace(query))
            return [];

        // ── VERARBEITUNG ───────────────────────────────────────
        // format=jsonv2: enthält zusätzlich Felder wie „type", „category", „importance"
        // featuretype=settlement: fokussiert auf bewohnte Orte (city, town, village)
        // addressdetails=0: spart Nutzdaten, da wir nur display_name + Koordinaten brauchen
        string url = string.Concat(
            BaseUrl,
            "?q=", Uri.EscapeDataString(query.Trim()),
            "&format=jsonv2",
            "&limit=", MaxResults,
            "&featuretype=settlement",
            "&addressdetails=0");

        string json = await _httpClient.GetStringAsync(url, cancellationToken);

        var results = JsonSerializer.Deserialize<List<GeocodingResult>>(json, _jsonOptions)
                      ?? [];

        // Nach Wichtigkeit absteigend sortieren (populärere Städte zuerst)
        results.Sort((a, b) => b.Importance.CompareTo(a.Importance));

        // ── AUSGABE ────────────────────────────────────────────
        return results.AsReadOnly();
    }

    // ── DISPOSE ────────────────────────────────────────────────

    /// <inheritdoc/>
    public void Dispose() => _httpClient.Dispose();
}
