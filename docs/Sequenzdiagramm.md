# Sequenzdiagramm

## Sonnenuhr – Standortspezifischer Wallpaper-Generator für Windows 11

---

| Feld | Inhalt |
|------|--------|
| **Projektname** | Sonnenuhr – Wallpaper-Generator |
| **Prüfling** | Uwe Markus Münch |
| **Stand** | 01.07.2026 |
| **Version** | 1.0 |

---

## Inhaltsverzeichnis

1. [Sequenzdiagramm 1: Wallpaper-Generierung](#1-sequenzdiagramm-wallpaper-generierung)
2. [Sequenzdiagramm 2: Anwendungsstart](#2-sequenzdiagramm-anwendungsstart)
3. [Sequenzdiagramm 3: Konfigurationsänderung](#3-sequenzdiagramm-konfigurationsänderung)
4. [Sequenzdiagramm 4: Stadtsuche](#4-sequenzdiagramm-stadtsuche)
5. [Sequenzdiagramm 5: Animierter Hintergrund](#5-sequenzdiagramm-animierter-hintergrund)

---

## 1. Sequenzdiagramm: Wallpaper-Generierung

### 1.1 Beschreibung

Dieses Sequenzdiagramm zeigt die vollständige Interaktionskette aller beteiligten Komponenten bei der Generierung und dem Setzen eines neuen Desktop-Wallpapers. Das Szenario wird entweder durch einen Timer-Tick (automatisch) oder durch einen manuellen Klick des Benutzers auf „Jetzt aktualisieren" ausgelöst.

Die Abfolge umfasst folgende Hauptschritte:
1. Auslösung durch den Benutzer oder Timer
2. Asynchroner API-Aufruf zur Beschaffung der Sonnendaten
3. Geometrische Berechnung der Sonnenuhr-Elemente
4. Bildgenerierung mit GDI+
5. Systemintegration über Windows Registry

### 1.2 Sequenzdiagramm

```mermaid
sequenceDiagram
    autonumber
    actor Benutzer
    participant MF as MainForm
    participant SAS as SolarApiService
    participant API as SunriseSunsetAPI
    participant SDC as SundialCalculator
    participant WGS as WallpaperGeneratorService
    participant WRS as WallpaperRegistryService

    Benutzer->>MF: Klick auf "Jetzt aktualisieren"
    Note over MF: btnRefresh_Click() aufgerufen<br/>Schaltfläche deaktivieren<br/>Status: "Abruf läuft..."

    MF->>+SAS: GetTodaySolarDataAsync(location)
    Note over SAS: URL aufbauen:<br/>api.sunrise-sunset.org/json?<br/>lat=49.49&lng=8.47&date=today&formatted=0

    SAS->>+API: HTTP GET Request (HTTPS)
    Note over API: Sonnendaten für Datum<br/>und Koordinaten berechnen

    API-->>-SAS: HTTP 200 OK<br/>JSON Response Body

    Note over SAS: JSON deserialisieren<br/>mit System.Text.Json<br/>Status prüfen: "OK"?

    SAS-->>-MF: SolarData (Sunrise, Sunset,<br/>SolarNoon, DayLength,<br/>CivilTwilightBegin/End)

    Note over MF: SolarData in _currentSolarData<br/>speichern<br/>UI-Labels aktualisieren

    MF->>+SDC: CalculateAllHourLines(location.Latitude)
    Note over SDC: Für jeden Stundenoffset -6..+6:<br/>tan(H) = sin(φ) × tan(t)<br/>Winkel in Grad berechnen

    SDC-->>-MF: Dictionary<int, double><br/>(StundenOffset → Winkel in Grad)

    MF->>+SDC: CalculateCurrentShadowAngle(DateTime.Now,<br/>solarData.SolarNoon, location.Latitude)
    Note over SDC: Zeitdifferenz zu Sonnenmittag<br/>in Stundenwinkel umrechnen<br/>Null wenn Nacht (vor Aufgang / nach Untergang)

    SDC-->>-MF: double? shadowAngle<br/>(null wenn Sonne unter Horizont)

    MF->>+WGS: GenerateWallpaper(solarData, location,<br/>outputPath, DateTime.Now, timeZone)
    Note over WGS: Bitmap(1920, 1080) erstellen<br/>Graphics-Objekt erzeugen

    WGS->>+SDC: CalculateCurrentShadowAngle(...)
    SDC-->>-WGS: double? shadowAngle

    Note over WGS: Hintergrund zeichnen (Gradient)<br/>Zifferblatt-Kreis zeichnen<br/>Stundenlinien einzeichnen<br/>Beschriftungen hinzufügen<br/>Gnomon-Symbol zeichnen<br/>Schattenzeiger zeichnen (wenn nicht null)<br/>Optionale Elemente zeichnen<br/>Bitmap.Save(path, ImageFormat.Png)

    WGS-->>-MF: void (PNG gespeichert unter<br/>%APPDATA%\Sonnenuhr\wallpaper.png)

    MF->>+WRS: SetWallpaper(wallpaperFilePath)
    Note over WRS: Registry.SetValue(<br/>HKCU\Control Panel\Desktop\Wallpaper,<br/>wallpaperFilePath)

    WRS->>WRS: SystemParametersInfo(<br/>SPI_SETDESKWALLPAPER, 0,<br/>path, SPIF_UPDATEINIFILE)
    Note over WRS: Windows aktualisiert<br/>sofort den Desktop-Hintergrund

    WRS-->>-MF: void (Wallpaper gesetzt)

    Note over MF: StatusLabel aktualisieren:<br/>"Letztes Update: HH:mm:ss Uhr"<br/>Sonnendaten in Labels anzeigen<br/>Schaltfläche wieder aktivieren

    MF-->>Benutzer: Desktop-Hintergrund aktualisiert ✓
```

### 1.3 Fehlerszenarien

```mermaid
sequenceDiagram
    autonumber
    actor Benutzer
    participant MF as MainForm
    participant SAS as SolarApiService
    participant API as SunriseSunsetAPI

    Benutzer->>MF: Klick auf "Jetzt aktualisieren"
    MF->>+SAS: GetTodaySolarDataAsync(location)
    SAS->>+API: HTTP GET Request (HTTPS)

    alt Netzwerk nicht verfügbar
        API-->>SAS: Timeout nach 10 Sekunden
        Note over SAS: TaskCanceledException<br/>oder HttpRequestException
        SAS-->>-MF: Exception ausgelöst
        Note over MF: Catch-Block im try/catch<br/>Fehler in sonnenuhr.log schreiben
        MF-->>Benutzer: StatusLabel: "Fehler: Netzwerk nicht verfügbar"

    else API antwortet mit Fehler
        API-->>SAS: HTTP 200 OK<br/>Status: "INVALID_REQUEST"
        Note over SAS: Status != "OK" erkannt
        SAS-->>-MF: Exception: "API returned status: INVALID_REQUEST"
        Note over MF: Fehler protokollieren
        MF-->>Benutzer: StatusLabel: "Fehler: Ungültige Koordinaten"
    end

    Note over MF: Letztes gültiges Wallpaper<br/>bleibt als Hintergrund bestehen
```

---

## 2. Sequenzdiagramm: Anwendungsstart

### 2.1 Beschreibung

Dieses Sequenzdiagramm zeigt den vollständigen Startvorgang der Anwendung, vom Aufruf des Einstiegspunkts `Program.Main` bis zum Abschluss der Initialisierung und der ersten Wallpaper-Generierung.

```mermaid
sequenceDiagram
    autonumber
    participant PM as Program.Main
    participant MF as MainForm
    participant CS as ConfigurationService
    participant FS as Dateisystem
    participant SAS as SolarApiService
    participant WGS as WallpaperGeneratorService

    PM->>PM: Application.SetHighDpiMode(HighDpiMode.SystemAware)
    PM->>PM: Application.EnableVisualStyles()
    PM->>PM: Application.SetCompatibleTextRenderingDefault(false)

    Note over PM: Kommandozeilenargumente prüfen:<br/>--minimized Flag vorhanden?

    PM->>+MF: new MainForm(startMinimized: false)
    Note over MF: Konstruktor: Felder initialisieren

    MF->>+CS: new ConfigurationService()
    Note over CS: ConfigFilePath = %APPDATA%\Sonnenuhr\settings.json<br/>Verzeichnis anlegen falls nicht vorhanden

    CS-->>-MF: ConfigurationService instanziiert

    MF->>+CS: LoadSettings()
    CS->>+FS: File.Exists(configFilePath) prüfen

    alt settings.json existiert
        FS-->>CS: true
        CS->>FS: File.ReadAllText(configFilePath)
        FS-->>CS: JSON-String
        CS->>CS: JsonSerializer.Deserialize<AppSettings>(json)
        Note over CS: Validierung: Sind alle Pflichtfelder vorhanden?
    else settings.json existiert nicht
        FS-->>-CS: false
        Note over CS: new AppSettings() mit Standardwerten erstellen:<br/>Location: Mannheim (49.4875, 8.4660)<br/>UpdateIntervalMinutes: 60<br/>StartWithWindows: false
        CS->>FS: Verzeichnis anlegen, JSON speichern
    end

    CS-->>-MF: AppSettings (_settings)

    MF->>+SAS: new SolarApiService()
    Note over SAS: HttpClient mit BaseAddress und Timeout (10s) initialisieren
    SAS-->>-MF: SolarApiService instanziiert

    MF->>+WGS: new WallpaperGeneratorService(_settings.WallpaperConfig)
    WGS-->>-MF: WallpaperGeneratorService instanziiert

    MF->>MF: InitializeComponent()
    Note over MF: UI-Elemente mit Werten aus _settings befüllen:<br/>txtLocationName.Text = _settings.Location.Name<br/>nudLatitude.Value = _settings.Location.Latitude<br/>nudInterval.Value = _settings.UpdateIntervalMinutes

    MF->>MF: Timer konfigurieren und starten
    Note over MF: timer.Interval = _settings.UpdateIntervalMinutes * 60000<br/>timer.Start()

    alt startMinimized = true
        MF->>MF: WindowState = Minimized<br/>ShowInTaskbar = false<br/>notifyIcon.Visible = true
    else startMinimized = false
        MF->>MF: Fenster normal anzeigen
    end

    MF->>MF: RefreshWallpaperAsync()
    Note over MF: Initialer Abruf direkt beim Start<br/>(Nicht auf Timer-Tick warten)

    PM->>PM: Application.Run(mainForm)
    Note over PM: Hauptnachrichtenschleife gestartet<br/>Anwendung läuft
```

---

## 3. Sequenzdiagramm: Konfigurationsänderung

### 3.1 Beschreibung

Dieses Sequenzdiagramm zeigt den Ablauf, wenn der Benutzer die Anwendungskonfiguration über den Konfigurationsdialog ändert.

```mermaid
sequenceDiagram
    autonumber
    actor Benutzer
    participant MF as MainForm
    participant CF as ConfigurationForm
    participant CS as ConfigurationService
    participant WRS as WallpaperRegistryService
    participant WGS as WallpaperGeneratorService

    Benutzer->>MF: Klick auf "⚙ Darstellung"

    MF->>+CF: new ConfigurationForm(_settings.WallpaperConfig)
    Note over CF: Formularfelder mit aktuellen Werten befüllen:<br/>Farbpaletten, Schriftarten, Checkboxen

    CF-->>-MF: ConfigurationForm instanziiert

    MF->>+CF: ShowDialog()
    Note over CF: Modaler Dialog öffnet sich<br/>Benutzer kann Einstellungen ändern

    Benutzer->>CF: Farben / Schriften / Optionen ändern

    alt Benutzer klickt "OK"
        Benutzer->>CF: Klick auf "OK" (btnOk_Click)
        CF->>CF: Eingaben validieren
        Note over CF: Intervall-Wert prüfen (1 ≤ x ≤ 1440 Min.)<br/>Schriftgröße prüfen (6 ≤ x ≤ 72 pt)
        CF->>CF: DialogResult = OK
        CF-->>-MF: DialogResult.OK

        MF->>MF: _settings.WallpaperConfig = cf.Settings.WallpaperConfig
        Note over MF: Neue Einstellungen in _settings übernehmen

        MF->>+CS: SaveSettings(_settings)
        Note over CS: JsonSerializer.Serialize(_settings)<br/>File.WriteAllText(configFilePath, json)
        CS-->>-MF: void (gespeichert)

        alt Autostart geändert
            MF->>+WRS: SetAutostart(_settings.StartWithWindows)
            Note over WRS: Registry-Eintrag setzen oder löschen:<br/>HKCU\Software\Microsoft\Windows\CurrentVersion\Run
            WRS-->>-MF: void
        end

        MF->>MF: Timer-Intervall aktualisieren
        Note over MF: timer.Stop()<br/>timer.Interval = newInterval * 60000<br/>timer.Start()

        MF->>+WGS: _config = _settings.WallpaperConfig
        Note over WGS: Neue Konfiguration übernehmen
        WGS-->>-MF: void

        MF->>MF: RefreshWallpaperAsync()
        Note over MF: Wallpaper sofort neu generieren<br/>Benutzer sieht Änderungen sofort

        MF-->>Benutzer: Wallpaper aktualisiert ✓

    else Benutzer klickt "Abbrechen"
        Benutzer->>CF: Klick auf "Abbrechen"
        CF->>CF: DialogResult = Cancel
        CF-->>MF: DialogResult.Cancel
        Note over MF: Keine Änderungen übernommen<br/>_settings bleibt unverändert
        MF-->>Benutzer: Dialog geschlossen (keine Änderungen)
    end
```

---

## Zusammenfassung der Interaktionspartner

| Komponente | Rolle | Kommunikationsrichtung |
|------------|-------|----------------------|
| **Benutzer** | Auslöser von Aktionen | → MainForm |
| **MainForm** | Orchestrator / Koordinator | ↔ alle Services |
| **SolarApiService** | Datenbeschaffung | → externe API |
| **SunriseSunsetAPI** | Externe Datenquelle | → SolarApiService |
| **SundialCalculator** | Rechenlogik (statisch) | → MainForm, WallpaperGeneratorService |
| **WallpaperGeneratorService** | Bildgenerierung | → Dateisystem |
| **WallpaperRegistryService** | Systemintegration (statisch) | → Windows Registry |
| **ConfigurationService** | Datenpersistenz | → Dateisystem |
| **ConfigurationForm** | Benutzereingabe | ↔ MainForm |

---

## 4. Sequenzdiagramm: Stadtsuche

### 4.1 Beschreibung

Dieses Sequenzdiagramm zeigt den Ablauf der Stadtsuche. Der Benutzer gibt
einen Stadtnamen ein und klickt auf den Suchen-Button. Je nach Anzahl der
gefundenen Treffer werden die Koordinaten direkt übernommen oder ein
Auswahldialog öffnet sich.

```mermaid
sequenceDiagram
    autonumber
    actor Benutzer
    participant MF as MainForm
    participant GS as GeocodingService
    participant NOM as Nominatim API
    participant CSF as CitySelectionForm

    Benutzer->>MF: Stadtname eingeben und "Suchen" klicken
    Note over MF: btnCitySearch_Click()<br/>btnCitySearch.Enabled = false<br/>Status: "Suche nach ..."

    MF->>+GS: SearchCityAsync(query)
    Note over GS: URL aufbauen:<br/>nominatim.openstreetmap.org/search?<br/>q={query}&format=jsonv2&featuretype=settlement

    GS->>+NOM: HTTP GET (User-Agent: Sonnenuhr/1.0)
    Note over NOM: Ortsname-Abfrage verarbeiten<br/>bis zu 10 Ergebnisse

    NOM-->>-GS: HTTP 200 OK<br/>JSON-Array mit GeocodingResult[]

    Note over GS: JSON deserialisieren<br/>Nach Importance absteigend sortieren

    GS-->>-MF: IReadOnlyList<GeocodingResult>

    alt 0 Treffer
        Note over MF: MessageBox: "Keine Orte gefunden.<br/>Tipp: Länderzusatz verwenden"
    else 1 Treffer
        Note over MF: ApplyCityResult(results[0])
        MF->>MF: txtLocationName = ShortName
        MF->>MF: txtLatitude = Latitude (F4, InvariantCulture)
        MF->>MF: txtLongitude = Longitude (F4, InvariantCulture)
        MF->>MF: Settings speichern
        MF-->>Benutzer: Koordinaten übernommen
    else Mehrere Treffer
        MF->>+CSF: new CitySelectionForm(results, query)
        Note over CSF: ListBox mit allen Treffern<br/>aufsteigend nach Importance sortiert

        CSF-->>-MF: Dialog instanziiert

        MF->>+CSF: ShowDialog()
        CSF-->>Benutzer: Auswahldialog zeigen

        Benutzer->>CSF: Treffer auswählen
        Note over CSF: Koordinaten-Vorschau aktualisieren

        alt Benutzer klickt "Auswählen"
            Benutzer->>CSF: Klick auf Auswählen
            CSF->>CSF: SelectedResult = gewählter Eintrag
            CSF->>CSF: DialogResult = OK
            CSF-->>-MF: DialogResult.OK

            MF->>MF: ApplyCityResult(selectionForm.SelectedResult)
            MF-->>Benutzer: Koordinaten übernommen

        else Benutzer klickt "Abbrechen"
            Benutzer->>CSF: Klick auf Abbrechen
            CSF-->>-MF: DialogResult.Cancel
            Note over MF: Keine Änderungen
        end
    end

    Note over MF: btnCitySearch.Enabled = true<br/>Status aktualisieren
```

### 4.2 Fehlerszenario: Netzwerkfehler bei Stadtsuche
    MF->>+GS: SearchCityAsync(query)
    GS->>+NOM: HTTP GET Request

    alt Netzwerk nicht verfügbar
        NOM-->>GS: Timeout nach 15 Sekunden
        GS-->>-MF: HttpRequestException
        Note over MF: Catch-Block: MessageBox mit Fehlermeldung<br/>Status: "Fehler bei der Stadtsuche"
        MF-->>Benutzer: Fehler-Dialog angezeigt
    end

    Note over MF: btnCitySearch.Enabled = true (finally-Block)
```

---

## 5. Sequenzdiagramm: Animierter Hintergrund

### 5.1 Beschreibung

Dieses Sequenzdiagramm zeigt die interne Aufrufkette innerhalb von `WallpaperGeneratorService`, die beim Zeichnen des animierten Hintergrunds durchlaufen wird. Der Einstiegspunkt ist `GenerateWallpaper()`. Vor dem eigentlichen Zeichnen werden die lokalen Sonnenauf- und -untergangszeiten berechnet und als Parameter an `DrawBackground()` übergeben. Je nach Tageszeit werden Sonne oder Mond gezeichnet; Sterne, Horizontglühen und Himmelsfarbe werden immer berechnet.

```mermaid
sequenceDiagram
    autonumber
    participant WGS as WallpaperGeneratorService
    participant DB  as DrawBackground
    participant GSC as GetSkyColors
    participant DS  as DrawStars
    participant DHG as DrawHorizonGlow
    participant CP  as CalculateCelestialPosition
    participant CMP as CalculateMoonPhase
    participant DSun as DrawSun
    participant DMoon as DrawMoon

    Note over WGS: GenerateWallpaper() aufgerufen<br/>Bitmap(1920×1080) erstellen

    WGS->>WGS: localSunrise = solarData.GetLocalSunrise(tz)
    WGS->>WGS: localSunset  = solarData.GetLocalSunset(tz)

    WGS->>+DB: DrawBackground(g, currentTime, localSunrise, localSunset)

    DB->>+GSC: GetSkyColors(currentTime, sunrise, sunset)
    Note over GSC: 7 Farbstützpunkte interpolieren:<br/>Tiefe Nacht → Astron. Dämmerung →<br/>Naut. Dämmerung → Bürg. Dämmerung →<br/>Sonnenauf-/untergang → Goldene Stunde →<br/>Tageshimmel
    GSC-->>-DB: (topColor, horizonColor)

    Note over DB: LinearGradientBrush mit topColor/horizonColor<br/>Hintergrundrechteck füllen

    DB->>+DS: DrawStars(g, currentTime, sunrise, sunset, horizonY)
    Note over DS: 180 Sterne deterministisch (Seed = Datum)<br/>Sanftes Ein-/Ausblenden ±60 min nach Sonnenuntergang<br/>Stündliches Flackern via Pseudo-Zufall
    DS-->>-DB: void

    DB->>+DHG: DrawHorizonGlow(g, currentTime, sunrise, sunset, horizonY)
    Note over DHG: Orangefarbener Ellipsen-Glow<br/>±60 min um Auf-/Untergang<br/>via PathGradientBrush
    DHG-->>-DB: void

    alt Tageszeit (zwischen Sonnenaufgang und -untergang)
        Note over DB: sunT = (currentTime − sunrise) / (sunset − sunrise)
        DB->>+CP: CalculateCelestialPosition(sunT, imageWidth, horizonY)
        Note over CP: Sinusbogen:<br/>t=0 → Horizont links (8 %)<br/>t=0.5 → Zenith (8 % Bildhöhe)<br/>t=1 → Horizont rechts (92 %)
        CP-->>-DB: PointF sunPos

        DB->>DB: GetCelestialAlpha(currentTime, sunrise, sunset, isDaytime: true)
        Note over DB: Sanftes Ein-/Ausblenden ±20 min am Horizont

        DB->>+DSun: DrawSun(g, sunPos, alpha)
        Note over DSun: 5 Glow-Schichten + Halo + Scheibe + Kern<br/>via PathGradientBrush
        DSun-->>-DB: void

    else Nachtzeit (nach Sonnenuntergang oder vor Sonnenaufgang)
        DB->>DB: CalculateMoonT(currentTime, sunrise, sunset)
        Note over DB: moonT: 0 = Sonnenuntergang<br/>1 = Sonnenaufgang (nächster Tag)

        DB->>+CP: CalculateCelestialPosition(moonT, imageWidth, horizonY)
        CP-->>-DB: PointF moonPos

        DB->>DB: GetCelestialAlpha(currentTime, sunrise, sunset, isDaytime: false)

        DB->>+CMP: CalculateMoonPhase(currentTime.Date)
        Note over CMP: Synodischer Monat (29,53 Tage)<br/>Referenz: Neumond 06.01.2000<br/>0 = Neumond, 0.5 = Vollmond
        CMP-->>-DB: double phase

        DB->>+DMoon: DrawMoon(g, moonPos, alpha, phase)
        Note over DMoon: Bläulicher Glow + Mondscheibe<br/>+ DrawMoonPhase() für Terminator<br/>zunehmend = Schatten links<br/>abnehmend = Schatten rechts
        DMoon-->>-DB: void
    end

    DB-->>-WGS: void (Hintergrund gezeichnet)

    Note over WGS: Sonnenuhr-Elemente zeichnen<br/>(Zifferblatt, Stundenlinien, Schattenzeiger…)<br/>Bitmap.Save(path, PNG)
```

### 5.2 Erläuterung der Schlüsselentscheidungen

| Entscheidung | Bedingung | Auswirkung |
|---|---|---|
| **Sonne oder Mond zeichnen?** | `currentTime` zwischen `localSunrise` und `localSunset` | Tag → `DrawSun()`; Nacht → `DrawMoon()` |
| **Sterne anzeigen?** | `currentTime` nahe/nach Sonnenuntergang oder vor Sonnenaufgang | Sanftes Einblenden ab 60 min nach Untergang |
| **Horizontglühen aktiv?** | `|currentTime − sunrise| ≤ 60 min` oder `|currentTime − sunset| ≤ 60 min` | Orangefarbener Glow proportional zur Nähe |
| **Alpha-Überblendung** | ±20 min nahe Horizont | Sonne/Mond werden weich ein-/ausgeblendet |

---

*Dokument erstellt von: Uwe Markus Münch | Breihof IT GmbH | IHK Rhein-Neckar | 01.07.2026*
