// ============================================================
// Datei:     SolarApiService.cs
// Projekt:   Sonnenuhr – Standortspezifischer Wallpaper-Generator
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Kapselt die HTTP-Kommunikation mit der Sunrise-Sunset-REST-API
//   (https://api.sunrise-sunset.org). Liefert astronomische Sonnenstand-
//   daten für einen gegebenen Standort und ein gegebenes Datum.
// ============================================================

using System.Text.Json;
using Sonnenuhr.Models;

namespace Sonnenuhr.Services;

/// <summary>
/// Ruft astronomische Sonnenstandsdaten von der Sunrise-Sunset-API ab.
/// </summary>
public class SolarApiService : IDisposable
{
    // ── FELDER ─────────────────────────────────────────────────

    private readonly HttpClient _httpClient;
    private bool _disposed;

    /// <summary>Basis-URL der Sunrise-Sunset-API.</summary>
    private const string ApiBaseUrl = "https://api.sunrise-sunset.org/json";

    // ── KONSTRUKTOR ────────────────────────────────────────────

    /// <summary>
    /// Initialisiert den Service mit einem frischen <see cref="HttpClient"/>.
    /// </summary>
    public SolarApiService()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Sonnenuhr/1.0 (Breihof IT GmbH)");
    }

    // ── ÖFFENTLICHE METHODEN ───────────────────────────────────

    /// <summary>
    /// Fragt die Sonnenzeitdaten für einen Standort und ein Datum ab.
    /// </summary>
    /// <param name="location">Geografischer Standort (Breiten- und Längengrad).</param>
    /// <param name="date">Datum, für das die Sonnendaten ermittelt werden sollen.</param>
    /// <param name="cancellationToken">Abbruch-Token.</param>
    /// <returns>
    /// <see cref="SolarData"/>-Objekt mit den Sonnenzeitdaten oder
    /// <c>null</c>, wenn die API keine gültigen Daten liefert.
    /// </returns>
    /// <exception cref="HttpRequestException">Netzwerkfehler oder HTTP-Fehlerantwort.</exception>
    /// <exception cref="TaskCanceledException">Anfrage wurde abgebrochen oder Timeout.</exception>
    public async Task<SolarData?> GetSolarDataAsync(
        Location          location,
        DateTime          date,
        CancellationToken cancellationToken = default)
    {
        // ── EINGABE ────────────────────────────────────────────
        if (!location.IsValid())
            throw new ArgumentException("Der Standort enthält ungültige Koordinaten.", nameof(location));

        string dateParam = date.ToString("yyyy-MM-dd");
        string requestUrl = BuildRequestUrl(location.Latitude, location.Longitude, dateParam);

        // ── VERARBEITUNG ───────────────────────────────────────
        HttpResponseMessage response = await _httpClient.GetAsync(requestUrl, cancellationToken)
                                                        .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        string jsonContent = await response.Content.ReadAsStringAsync(cancellationToken)
                                                    .ConfigureAwait(false);

        var apiResponse = JsonSerializer.Deserialize<SunriseSunsetApiResponse>(jsonContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (apiResponse?.Status != "OK" || apiResponse.Results is null)
            return null;

        // ── AUSGABE ────────────────────────────────────────────
        // System.Text.Json setzt DateTimeKind auf Local oder Unspecified für ISO-8601-Zeitstempel.
        // TimeZoneInfo.ConvertTimeFromUtc() erfordert zwingend DateTimeKind.Utc → explizit korrigieren.
        return NormalizeToUtc(apiResponse.Results);
    }

    /// <summary>
    /// Fragt die Sonnenzeitdaten für das heutige Datum ab.
    /// </summary>
    /// <param name="location">Geografischer Standort.</param>
    /// <param name="cancellationToken">Abbruch-Token.</param>
    /// <returns>Heutige <see cref="SolarData"/> oder <c>null</c>.</returns>
    public Task<SolarData?> GetTodaySolarDataAsync(
        Location          location,
        CancellationToken cancellationToken = default) =>
        GetSolarDataAsync(location, DateTime.UtcNow.Date, cancellationToken);

    // ── PRIVATE HILFSMETHODEN ──────────────────────────────────

    /// <summary>
    /// Setzt das <see cref="DateTimeKind"/> aller Zeitfelder in <paramref name="data"/>
    /// explizit auf <see cref="DateTimeKind.Utc"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="System.Text.Json"/> deserialisiert ISO-8601-Zeitstempel mit dem Offset "+00:00"
    /// als <see cref="DateTimeKind.Local"/> statt <see cref="DateTimeKind.Utc"/>.
    /// <see cref="TimeZoneInfo.ConvertTimeFromUtc"/> wirft sonst eine
    /// <see cref="ArgumentException"/> ("The supplied DateTime did not have the Kind
    /// property set to Utc").
    /// </remarks>
    private static SolarData NormalizeToUtc(SolarData data)
    {
        // ── EINGABE ────────────────────────────────────────────
        // ── VERARBEITUNG ───────────────────────────────────────
        data.Sunrise            = DateTime.SpecifyKind(data.Sunrise,            DateTimeKind.Utc);
        data.Sunset             = DateTime.SpecifyKind(data.Sunset,             DateTimeKind.Utc);
        data.SolarNoon          = DateTime.SpecifyKind(data.SolarNoon,          DateTimeKind.Utc);
        data.CivilTwilightBegin = DateTime.SpecifyKind(data.CivilTwilightBegin, DateTimeKind.Utc);
        data.CivilTwilightEnd   = DateTime.SpecifyKind(data.CivilTwilightEnd,   DateTimeKind.Utc);

        // ── AUSGABE ────────────────────────────────────────────
        return data;
    }

    /// <summary>
    /// Erstellt die vollständige API-Request-URL mit allen Parametern.
    /// </summary>
    /// <param name="latitude">Breitengrad.</param>
    /// <param name="longitude">Längengrad.</param>
    /// <param name="date">Datum im Format yyyy-MM-dd.</param>
    /// <returns>Vollständige Request-URL als Zeichenkette.</returns>
    private static string BuildRequestUrl(double latitude, double longitude, string date)
    {
        // ── EINGABE ────────────────────────────────────────────
        // ── VERARBEITUNG ───────────────────────────────────────
        // formatted=0 liefert ISO 8601 Zeitstempel statt Strings
        string url = $"{ApiBaseUrl}?lat={latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)}" +
                     $"&lng={longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)}" +
                     $"&date={date}&formatted=0";

        // ── AUSGABE ────────────────────────────────────────────
        return url;
    }

    // ── IDisposable ────────────────────────────────────────────

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
