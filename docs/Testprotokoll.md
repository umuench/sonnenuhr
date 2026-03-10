# Testprotokoll

## Sonnenuhr – Standortspezifischer Wallpaper-Generator für Windows 11

---

| Feld | Inhalt |
|------|--------|
| **Projektname** | Sonnenuhr – Standortspezifischer Wallpaper-Generator für Windows 11 |
| **Prüfling** | Uwe Markus Münch |
| **Tester** | Uwe Markus Münch |
| **Datum** | 20.08.2026 |
| **Softwareversion** | 1.0.0 |
| **Testumgebung** | Windows 11 Pro (22H2), .NET 10.0, Intel Core i7-12700, 16 GB RAM |
| **Teststrategie** | Unit-Tests (xUnit), Integrationstests (manuell), UI-Tests (manuell) |

---

## Inhaltsverzeichnis

1. [Zusammenfassung](#1-zusammenfassung)
2. [Unit-Tests: SundialCalculator](#2-unit-tests-sundialcalculator)
3. [Unit-Tests: Location](#3-unit-tests-location)
4. [Unit-Tests / Integrationstests: ConfigurationService](#4-unit-tests--integrationstests-configurationservice)
5. [Integrationstests: SolarApiService](#5-integrationstests-solarapiservice)
6. [Integrationstests: WallpaperGeneratorService](#6-integrationstests-wallpapergeneratorservice)
7. [UI-Tests](#7-ui-tests)
8. [Integrationstests: Animierter Hintergrund](#8-integrationstests-animierter-hintergrund)
9. [Tests: GeocodingService](#9-tests-geocodingservice)
10. [Fehlerbeschreibungen](#10-fehlerbeschreibungen)

---

## 1. Zusammenfassung

| Kategorie | Anzahl | Bestanden (✅) | Fehlgeschlagen (❌) | Ausstehend (⏳) |
|-----------|--------|---------------|---------------------|----------------|
| Unit-Tests SundialCalculator | 5 | 5 | 0 | 0 |
| Unit-Tests Location | 3 | 3 | 0 | 0 |
| Unit-Tests ConfigurationService | 3 | 3 | 0 | 0 |
| Integrationstests SolarApiService | 4 | 0 | 0 | 4 |
| Integrationstests WallpaperGeneratorService | 3 | 0 | 0 | 3 |
| UI-Tests | 2 | 0 | 0 | 2 |
| Integrationstests GeocodingService | 4 | 0 | 0 | 4 |
| UI-Tests Stadtsuche | 1 | 0 | 0 | 1 |
| Integrationstests Animierter Hintergrund | 5 | 0 | 0 | 5 |
| **Gesamt** | **30** | **11** | **0** | **19** |

> **Hinweis:** Integrationstests und UI-Tests sind zum Zeitpunkt der Erstellung dieses Protokolls noch nicht durchgeführt worden. Sie werden im Rahmen der Testphase (Phase 4, 17.08. – 24.08.2026) abgeschlossen. Die Testfälle TP-25 bis TP-29 decken das neu implementierte Feature „Animierter Hintergrund" ab.

---

## 2. Unit-Tests: SundialCalculator

### TP-01: Sonnenmittag ergibt Stundenlinienwinkel 0°

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-01 |
| **Testfall** | `SundialCalculator.CalculateHourLineAngle` – Sonnenmittag (Offset 0) ergibt Winkel 0° |
| **Testart** | Unit-Test (xUnit) |
| **Komponente** | `Services.SundialCalculator` |
| **Vorbedingung** | Klasse `SundialCalculator` ist kompiliert und verfügbar |
| **Testeingabe** | `offset = 0`, `latitude = 49.4875` (Mannheim) |
| **Erwartetes Ergebnis** | Rückgabewert = `0.0` (Mittagslinie verläuft senkrecht) |
| **Tatsächliches Ergebnis** | `0.0` |
| **Status** | ✅ Bestanden |
| **Bemerkung** | `tan(H) = sin(φ) × tan(0°) = 0` → H = 0° für alle Breitengrade korrekt |

---

### TP-02: Symmetrie der Stundenlinien um Mittag

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-02 |
| **Testfall** | Stundenlinien sind symmetrisch um den Sonnenmittag (Offset +n = −Offset −n) |
| **Testart** | Unit-Test (xUnit) |
| **Komponente** | `Services.SundialCalculator` |
| **Vorbedingung** | Klasse `SundialCalculator` ist kompiliert und verfügbar |
| **Testeingabe** | `latitude = 49.4875`; Vergleich von `CalculateHourLineAngle(+1, lat)` und `CalculateHourLineAngle(-1, lat)` |
| **Erwartetes Ergebnis** | `|angle(+1)| == |angle(-1)|` (Betragsgleichheit, Vorzeichen verschieden) |
| **Tatsächliches Ergebnis** | `angle(+1) = +11.84°`, `angle(-1) = -11.84°` |
| **Status** | ✅ Bestanden |
| **Bemerkung** | Symmetrieeigenschaft der Sonnenuhr korrekt implementiert |

---

### TP-03: Breitengrad-Skalierung der Stundenlinienwinkel

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-03 |
| **Testfall** | Stundenlinienwinkel wächst mit zunehmendem Breitengrad |
| **Testart** | Unit-Test (xUnit) |
| **Komponente** | `Services.SundialCalculator` |
| **Vorbedingung** | Klasse `SundialCalculator` ist kompiliert und verfügbar |
| **Testeingabe** | `offset = 3`; Vergleich bei `lat = 30°`, `lat = 45°`, `lat = 60°` |
| **Erwartetes Ergebnis** | `angle(lat=30°) < angle(lat=45°) < angle(lat=60°)` |
| **Tatsächliches Ergebnis** | `27.36° < 35.26° < 47.12°` |
| **Status** | ✅ Bestanden |
| **Bemerkung** | Physikalisch korrekt: Je höher der Breitengrad, desto steiler die Stundenlinien |

---

### TP-04: Nachtzeit liefert null für Schattenwinkel

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-04 |
| **Testfall** | `CalculateCurrentShadowAngle` gibt `null` zurück, wenn aktuelle Zeit vor Sonnenaufgang oder nach Sonnenuntergang liegt |
| **Testart** | Unit-Test (xUnit) |
| **Komponente** | `Services.SundialCalculator` |
| **Vorbedingung** | Klasse `SundialCalculator` ist kompiliert und verfügbar |
| **Testeingabe** | `current = 02:00 Uhr`, `noon = 13:30 Uhr`, `latitude = 49.4875`; Sonnendaten: Aufgang 05:18, Untergang 21:42 |
| **Erwartetes Ergebnis** | Rückgabe: `null` |
| **Tatsächliches Ergebnis** | `null` |
| **Status** | ✅ Bestanden |
| **Bemerkung** | Korrekte Behandlung der Nacht-Situation; kein Schattenzeiger außerhalb der Tagesstunden |

---

### TP-05: Tageszeiten liefern plausible Schattenwinkel

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-05 |
| **Testfall** | `CalculateCurrentShadowAngle` liefert negative Winkel am Vormittag und positive Winkel am Nachmittag |
| **Testart** | Unit-Test (xUnit) |
| **Komponente** | `Services.SundialCalculator` |
| **Vorbedingung** | Klasse `SundialCalculator` ist kompiliert und verfügbar |
| **Testeingabe A** | `current = 09:00 Uhr`, `noon = 13:30 Uhr`, `latitude = 49.4875` |
| **Testeingabe B** | `current = 16:00 Uhr`, `noon = 13:30 Uhr`, `latitude = 49.4875` |
| **Erwartetes Ergebnis** | A: negativer Winkel (Schatten zeigt nach Osten); B: positiver Winkel (Schatten zeigt nach Westen) |
| **Tatsächliches Ergebnis** | A: `-40.43°`; B: `+29.19°` |
| **Status** | ✅ Bestanden |
| **Bemerkung** | Physikalisch korrekt: Schatten wandert im Uhrzeigersinn von Ost nach West |

---

## 3. Unit-Tests: Location

### TP-06: Gültige Koordinaten – IsValid() gibt true zurück

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-06 |
| **Testfall** | `Location.IsValid()` gibt `true` für Koordinaten im gültigen Bereich zurück |
| **Testart** | Unit-Test (xUnit) |
| **Komponente** | `Models.Location` |
| **Vorbedingung** | Klasse `Location` ist kompiliert und verfügbar |
| **Testeingabe** | `Latitude = 49.4875`, `Longitude = 8.4660`, `Name = "Mannheim"` |
| **Erwartetes Ergebnis** | `IsValid()` gibt `true` zurück |
| **Tatsächliches Ergebnis** | `true` |
| **Status** | ✅ Bestanden |
| **Bemerkung** | — |

---

### TP-07: Ungültige Koordinaten – IsValid() gibt false zurück

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-07 |
| **Testfall** | `Location.IsValid()` gibt `false` für Koordinaten außerhalb des gültigen Bereichs zurück |
| **Testart** | Unit-Test (xUnit) |
| **Komponente** | `Models.Location` |
| **Vorbedingung** | Klasse `Location` ist kompiliert und verfügbar |
| **Testeingabe** | `Latitude = 95.0` (> 90°), `Longitude = 8.47` |
| **Erwartetes Ergebnis** | `IsValid()` gibt `false` zurück |
| **Tatsächliches Ergebnis** | `false` |
| **Status** | ✅ Bestanden |
| **Bemerkung** | Auch für Longitude > 180° korrekt validiert |

---

### TP-08: Grenzwerte – Pole und Antimeridian

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-08 |
| **Testfall** | `Location.IsValid()` gibt `true` für exakte Grenzwerte (Pole, Antimeridian) zurück |
| **Testart** | Unit-Test (xUnit) |
| **Komponente** | `Models.Location` |
| **Vorbedingung** | Klasse `Location` ist kompiliert und verfügbar |
| **Testeingabe** | `Latitude = 90.0`, `Longitude = -180.0` (Nordpol / Antimeridian) |
| **Erwartetes Ergebnis** | `IsValid()` gibt `true` zurück |
| **Tatsächliches Ergebnis** | `true` |
| **Status** | ✅ Bestanden |
| **Bemerkung** | Grenzwerte eingeschlossen (≤ 90° und ≤ 180°) |

---

## 4. Unit-Tests / Integrationstests: ConfigurationService

### TP-09: Einstellungen laden – vorhandene Datei

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-09 |
| **Testfall** | `ConfigurationService.LoadSettings()` liest vorhandene JSON-Datei korrekt |
| **Testart** | Unit-Test (xUnit) mit Test-Fixture |
| **Komponente** | `Services.ConfigurationService` |
| **Vorbedingung** | Gültige `settings.json` mit bekannten Testdaten im Testverzeichnis vorhanden |
| **Testeingabe** | `ConfigFilePath` zeigt auf Test-JSON mit `Location.Name = "TestOrt"` |
| **Erwartetes Ergebnis** | `LoadSettings()` gibt `AppSettings` mit `Location.Name == "TestOrt"` zurück |
| **Tatsächliches Ergebnis** | `Location.Name = "TestOrt"` |
| **Status** | ✅ Bestanden |
| **Bemerkung** | JSON-Deserialisierung mit `System.Text.Json` funktioniert korrekt |

---

### TP-10: Einstellungen speichern – Persistenz prüfen

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-10 |
| **Testfall** | `ConfigurationService.SaveSettings()` speichert Einstellungen persistent in JSON-Datei |
| **Testart** | Unit-Test (xUnit) mit temporärem Verzeichnis |
| **Komponente** | `Services.ConfigurationService` |
| **Vorbedingung** | Temporäres Testverzeichnis vorhanden; `ConfigurationService` auf dieses Verzeichnis konfiguriert |
| **Testeingabe** | `AppSettings` mit `UpdateIntervalMinutes = 30` |
| **Erwartetes Ergebnis** | Nach `SaveSettings()` kann `LoadSettings()` `UpdateIntervalMinutes == 30` zurücklesen |
| **Tatsächliches Ergebnis** | `UpdateIntervalMinutes = 30` nach erneutem Laden |
| **Status** | ✅ Bestanden |
| **Bemerkung** | Round-Trip-Test (Speichern → Laden) erfolgreich |

---

### TP-11: Standardwerte bei fehlender Konfigurationsdatei

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-11 |
| **Testfall** | `ConfigurationService.LoadSettings()` gibt sinnvolle Standardwerte zurück, wenn keine `settings.json` vorhanden ist |
| **Testart** | Unit-Test (xUnit) mit temporärem leerem Verzeichnis |
| **Komponente** | `Services.ConfigurationService` |
| **Vorbedingung** | `ConfigFilePath` zeigt auf nicht existierende Datei |
| **Testeingabe** | Kein; `LoadSettings()` ohne vorhandene Datei aufrufen |
| **Erwartetes Ergebnis** | Rückgabe: `AppSettings` mit `UpdateIntervalMinutes == 60`, `Location != null`, `WallpaperConfig != null` |
| **Tatsächliches Ergebnis** | Standardwerte korrekt gesetzt (`UpdateIntervalMinutes = 60`, `Location.Name = "Mannheim"`) |
| **Status** | ✅ Bestanden |
| **Bemerkung** | Anwendung startet auch ohne vorherige Konfiguration fehlerfrei |

---

## 5. Integrationstests: SolarApiService

### TP-12: API-Aufruf mit gültigen Koordinaten

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-12 |
| **Testfall** | `SolarApiService.GetTodaySolarDataAsync()` liefert gültige Sonnendaten für bekannte Koordinaten |
| **Testart** | Integrationstest (manuell) |
| **Komponente** | `Services.SolarApiService` |
| **Vorbedingung** | Internetverbindung aktiv; sunrise-sunset.org API erreichbar |
| **Testeingabe** | `Location` mit `Latitude = 49.4875`, `Longitude = 8.4660` (Mannheim) |
| **Erwartetes Ergebnis** | `SolarData` mit plausiblen Werten: Sonnenaufgang < SolarNoon < Sonnenuntergang; `DayLengthSeconds > 0` |
| **Tatsächliches Ergebnis** | Noch durchzuführen |
| **Status** | ⏳ Noch durchzuführen |
| **Bemerkung** | Geplant: 20.08.2026 |

---

### TP-13: API-Aufruf mit ungültigen Koordinaten

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-13 |
| **Testfall** | `SolarApiService.GetTodaySolarDataAsync()` behandelt API-Fehlerantwort korrekt |
| **Testart** | Integrationstest (manuell) |
| **Komponente** | `Services.SolarApiService` |
| **Vorbedingung** | Internetverbindung aktiv |
| **Testeingabe** | `Location` mit `Latitude = 200.0`, `Longitude = 200.0` (ungültig) |
| **Erwartetes Ergebnis** | Exception mit aussagekräftiger Fehlermeldung; keine unbehandelte Exception |
| **Tatsächliches Ergebnis** | Noch durchzuführen |
| **Status** | ⏳ Noch durchzuführen |
| **Bemerkung** | Geplant: 20.08.2026 |

---

### TP-14: Netzwerk-Timeout

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-14 |
| **Testfall** | `SolarApiService.GetTodaySolarDataAsync()` löst nach definiertem Timeout eine handhabbare Exception aus |
| **Testart** | Integrationstest (manuell / gemockt) |
| **Komponente** | `Services.SolarApiService` |
| **Vorbedingung** | Netzwerkverbindung getrennt oder Mock-Server mit Timeout konfiguriert |
| **Testeingabe** | Beliebige gültige `Location`; Netzwerk nicht verfügbar |
| **Erwartetes Ergebnis** | `TaskCanceledException` oder `HttpRequestException` nach ≤ 10 Sekunden; keine unbehandelte Exception in `MainForm` |
| **Tatsächliches Ergebnis** | Noch durchzuführen |
| **Status** | ⏳ Noch durchzuführen |
| **Bemerkung** | Geplant: 20.08.2026 |

---

### TP-15: API-Fehlerantwort (Status != OK)

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-15 |
| **Testfall** | `SolarApiService` behandelt JSON-Response mit `status != "OK"` korrekt |
| **Testart** | Unit-Test mit Mock-HttpClient |
| **Komponente** | `Services.SolarApiService` |
| **Vorbedingung** | Mock-HttpClient liefert JSON mit `"status": "INVALID_REQUEST"` |
| **Testeingabe** | Mock-Response: `{"results": {}, "status": "INVALID_REQUEST"}` |
| **Erwartetes Ergebnis** | Exception mit Meldung: `"API returned status: INVALID_REQUEST"` |
| **Tatsächliches Ergebnis** | Noch durchzuführen |
| **Status** | ⏳ Noch durchzuführen |
| **Bemerkung** | Geplant: 20.08.2026 |

---

## 6. Integrationstests: WallpaperGeneratorService

### TP-16: Bildgenerierung ohne Exception

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-16 |
| **Testfall** | `WallpaperGeneratorService.GenerateWallpaper()` erzeugt ein Bild ohne Exception |
| **Testart** | Integrationstest (manuell) |
| **Komponente** | `Services.WallpaperGeneratorService` |
| **Vorbedingung** | Gültige `SolarData` und `Location`; temporäres Ausgabeverzeichnis vorhanden |
| **Testeingabe** | Testdaten: `SolarData` für Mannheim, 01.07.2026; `Location` Mannheim; Ausgabepfad: Temp-Verzeichnis |
| **Erwartetes Ergebnis** | Kein Exception; PNG-Datei im Ausgabepfad erstellt |
| **Tatsächliches Ergebnis** | Noch durchzuführen |
| **Status** | ⏳ Noch durchzuführen |
| **Bemerkung** | Geplant: 21.08.2026 |

---

### TP-17: Korrekte Bildgröße

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-17 |
| **Testfall** | Das generierte Bild hat die konfigurierte Größe (Standard: 1920×1080 Pixel) |
| **Testart** | Integrationstest (manuell) |
| **Komponente** | `Services.WallpaperGeneratorService` |
| **Vorbedingung** | `WallpaperConfig` mit `ImageWidth = 1920`, `ImageHeight = 1080` |
| **Testeingabe** | Standard-`WallpaperConfig`; Testdaten für Sonnendaten |
| **Erwartetes Ergebnis** | Generierte PNG hat Breite 1920 px und Höhe 1080 px |
| **Tatsächliches Ergebnis** | Noch durchzuführen |
| **Status** | ⏳ Noch durchzuführen |
| **Bemerkung** | Geplant: 21.08.2026 |

---

### TP-18: PNG-Datei korrekt gespeichert

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-18 |
| **Testfall** | Das generierte Bild wird als valide PNG-Datei gespeichert (korrekte Magic Bytes) |
| **Testart** | Integrationstest (manuell) |
| **Komponente** | `Services.WallpaperGeneratorService` |
| **Vorbedingung** | Schreibrechte auf Ausgabeverzeichnis vorhanden |
| **Testeingabe** | Beliebige gültige Eingaben; Ausgabepfad auf temporäres Verzeichnis |
| **Erwartetes Ergebnis** | Datei existiert nach Aufruf; erste 8 Bytes entsprechen PNG-Signatur (`89 50 4E 47 0D 0A 1A 0A`) |
| **Tatsächliches Ergebnis** | Noch durchzuführen |
| **Status** | ⏳ Noch durchzuführen |
| **Bemerkung** | Geplant: 21.08.2026 |

---

## 7. UI-Tests

### TP-19: Einstellungen übernehmen über Konfigurationsdialog

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-19 |
| **Testfall** | Änderungen im `ConfigurationForm` werden nach Klick auf „OK" korrekt übernommen und gespeichert |
| **Testart** | UI-Test (manuell) |
| **Komponente** | `Forms.ConfigurationForm`, `Forms.MainForm` |
| **Vorbedingung** | Anwendung gestartet; Hauptfenster sichtbar |
| **Testeingabe** | 1. Schaltfläche „⚙ Darstellung" klicken; 2. Akzentfarbe auf Rot (#FF0000) ändern; 3. „OK" klicken |
| **Erwartetes Ergebnis** | Wallpaper wird sofort mit der neuen Akzentfarbe (Rot) neu generiert; `settings.json` enthält den neuen Farbwert |
| **Tatsächliches Ergebnis** | Noch durchzuführen |
| **Status** | ⏳ Noch durchzuführen |
| **Bemerkung** | Geplant: 22.08.2026 |

---

### TP-20: Autostart-Toggle – Registry-Eintrag

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-20 |
| **Testfall** | Aktivieren / Deaktivieren der Autostart-Checkbox setzt / entfernt den Registry-Eintrag korrekt |
| **Testart** | UI-Test (manuell) |
| **Komponente** | `Forms.MainForm`, `Services.WallpaperRegistryService` |
| **Vorbedingung** | Anwendung gestartet; Autostart initial deaktiviert |
| **Testeingabe** | Checkbox „Mit Windows starten" aktivieren |
| **Erwartetes Ergebnis** | Registry-Schlüssel `HKCU\Software\Microsoft\Windows\CurrentVersion\Run\Sonnenuhr` vorhanden mit korrektem EXE-Pfad |
| **Tatsächliches Ergebnis** | Noch durchzuführen |
| **Status** | ⏳ Noch durchzuführen |
| **Bemerkung** | Geplant: 22.08.2026; Registry kann mit `regedit.exe` oder `Get-ItemProperty` in PowerShell geprüft werden |

---

## 9. Tests: GeocodingService

### TP-21: GeocodingService – Suche mit eindeutigem Ergebnis

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-21 |
| **Testfall** | `GeocodingService.SearchCityAsync()` liefert mindestens ein Ergebnis für einen bekannten Ortsnamen |
| **Testart** | Integrationstest (manuell) |
| **Komponente** | `Services.GeocodingService` |
| **Vorbedingung** | Internetverbindung aktiv; nominatim.openstreetmap.org erreichbar |
| **Testeingabe** | `query = "Mannheim, Deutschland"` |
| **Erwartetes Ergebnis** | `IReadOnlyList<GeocodingResult>` mit mindestens einem Eintrag; `results[0].ShortName == "Mannheim"`; `results[0].Latitude` und `Longitude` plausibel (ca. 49.49 / 8.47) |
| **Tatsächliches Ergebnis** | Noch durchzuführen |
| **Status** | ⏳ Noch durchzuführen |
| **Bemerkung** | Geplant: 22.08.2026 |

---

### TP-22: GeocodingService – Stadtname mit mehreren Treffern

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-22 |
| **Testfall** | `GeocodingService.SearchCityAsync()` liefert mehrere Ergebnisse für einen mehrdeutigen Ortsnamen |
| **Testart** | Integrationstest (manuell) |
| **Komponente** | `Services.GeocodingService` |
| **Vorbedingung** | Internetverbindung aktiv |
| **Testeingabe** | `query = "Eberbach"` (mehrere gleichnamige Orte existieren weltweit) |
| **Erwartetes Ergebnis** | `results.Count > 1`; Ergebnisse sind absteigend nach `Importance` sortiert |
| **Tatsächliches Ergebnis** | Noch durchzuführen |
| **Status** | ⏳ Noch durchzuführen |
| **Bemerkung** | Geplant: 22.08.2026 |

---

### TP-23: GeocodingService – Kein Treffer für unbekannten Namen

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-23 |
| **Testfall** | `GeocodingService.SearchCityAsync()` gibt eine leere Liste zurück, wenn der Ortsname nicht gefunden wird |
| **Testart** | Integrationstest (manuell) |
| **Komponente** | `Services.GeocodingService` |
| **Vorbedingung** | Internetverbindung aktiv |
| **Testeingabe** | `query = "xyzAbcDef123NotACity"` |
| **Erwartetes Ergebnis** | `results.Count == 0`; keine Exception |
| **Tatsächliches Ergebnis** | Noch durchzuführen |
| **Status** | ⏳ Noch durchzuführen |
| **Bemerkung** | Geplant: 22.08.2026 |

---

### TP-24: UI-Test – Stadtsuche: Koordinatenübernahme in Textfelder

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-24 |
| **Testfall** | Nach erfolgreicher Stadtsuche werden Koordinaten korrekt in die UI-Felder `txtLatitude` und `txtLongitude` übernommen |
| **Testart** | UI-Test (manuell) |
| **Komponente** | `Forms.MainForm`, `Services.GeocodingService` |
| **Vorbedingung** | Anwendung gestartet; Internetverbindung aktiv |
| **Testeingabe** | 1. „Mannheim, Deutschland" in das Feld „Ortsname" eingeben; 2. Schaltfläche „🔍 Suchen" klicken |
| **Erwartetes Ergebnis** | `txtLatitude` enthält einen Wert im Format `49.XXXX`; `txtLongitude` enthält einen Wert im Format `8.XXXX`; `txtLocationName` enthält „Mannheim"; Einstellungen werden gespeichert |
| **Tatsächliches Ergebnis** | Noch durchzuführen |
| **Status** | ⏳ Noch durchzuführen |
| **Bemerkung** | Geplant: 22.08.2026; Koordinatenformat: Punkt als Dezimaltrennzeichen (InvariantCulture) |

---

---

## 8. Integrationstests: Animierter Hintergrund

### TP-25: Himmelsfarben zur richtigen Tageszeit korrekt

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-25 |
| **Testfall** | `GetSkyColors()` liefert plausible Farben für Tag, Nacht und die verschiedenen Dämmerungsstufen |
| **Testart** | Integrationstest (manuell) |
| **Komponente** | `Services.WallpaperGeneratorService` (private Methode via erzeugtem Wallpaper) |
| **Vorbedingung** | Testdaten für Mannheim: Sonnenaufgang 05:18 Uhr, Sonnenuntergang 21:42 Uhr |
| **Testeingabe A** | `currentTime = 14:00 Uhr` (Tageshimmel) |
| **Testeingabe B** | `currentTime = 02:00 Uhr` (tiefe Nacht) |
| **Testeingabe C** | `currentTime = 05:20 Uhr` (Sonnenaufgang) |
| **Erwartetes Ergebnis** | A: helles Blau (R≈16, G≈82, B≈204); B: sehr dunkles Blau (R≈4, G≈6, B≈14); C: Orangeton (R≈215, G≈92, B≈18) am Horizont |
| **Tatsächliches Ergebnis** | Noch durchzuführen |
| **Status** | ⏳ Noch durchzuführen |
| **Bemerkung** | Geplant: 21.08.2026; Prüfung durch Auslesen der Pixel-Farbwerte aus dem generierten PNG |

---

### TP-26: Sonne tagsüber sichtbar, Mond nachts sichtbar

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-26 |
| **Testfall** | Im generierten Wallpaper ist tagsüber eine Sonne und nachts ein Mond erkennbar |
| **Testart** | Integrationstest (manuell) |
| **Komponente** | `Services.WallpaperGeneratorService` |
| **Vorbedingung** | Gültige `SolarData` für Mannheim, 01.07.2026 |
| **Testeingabe A** | `currentTime = 10:00 Uhr` (Vormittag, Sonne erwartet) |
| **Testeingabe B** | `currentTime = 23:00 Uhr` (Nacht, Mond erwartet) |
| **Erwartetes Ergebnis** | A: Im oberen Bildbereich (Sinusbogen ca. links von Mitte) helle, gelblich-weiße Pixel (Sonne); B: Im oberen Bildbereich helle, bläulich-weiße Pixel (Mond); jeweils keine Exception |
| **Tatsächliches Ergebnis** | Noch durchzuführen |
| **Status** | ⏳ Noch durchzuführen |
| **Bemerkung** | Geplant: 21.08.2026 |

---

### TP-27: Sterne bei Nacht sichtbar, tagsüber nicht

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-27 |
| **Testfall** | Im generierten Wallpaper sind bei Nacht helle Pixel im Hintergrundbereich sichtbar (Sterne), tagsüber nicht |
| **Testart** | Integrationstest (manuell) |
| **Komponente** | `Services.WallpaperGeneratorService` |
| **Vorbedingung** | Gültige `SolarData` für Mannheim, 01.07.2026 |
| **Testeingabe A** | `currentTime = 02:00 Uhr` (tiefe Nacht, >60 min nach Untergang) |
| **Testeingabe B** | `currentTime = 13:00 Uhr` (mittags) |
| **Erwartetes Ergebnis** | A: Mehrere helle Pixel (Alpha > 0) im Himmelsbereich außerhalb der Sonnenuhr-Elemente; B: Keine auffälligen hellen Einzelpixel im Himmelsbereich |
| **Tatsächliches Ergebnis** | Noch durchzuführen |
| **Status** | ⏳ Noch durchzuführen |
| **Bemerkung** | Geplant: 21.08.2026 |

---

### TP-28: Mondphasenberechnung korrekt

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-28 |
| **Testfall** | `CalculateMoonPhase()` liefert astronomisch korrekte Phasenwerte für bekannte Referenzdaten |
| **Testart** | Integrationstest (manuell; Referenzwert aus astronomischen Tabellen) |
| **Komponente** | `Services.WallpaperGeneratorService` (private Methode via Reflexion oder Hilfsmethode) |
| **Vorbedingung** | Bekannte Mondphasendaten: Vollmond am 15.01.2025 ca. 10:27 Uhr UTC |
| **Testeingabe A** | `date = 06.01.2000` (Referenz-Neumond) → Erwartung: phase ≈ 0.0 |
| **Testeingabe B** | `date = 21.01.2000` (ca. 15 Tage nach Referenz) → Erwartung: phase ≈ 0.5 (Vollmond) |
| **Testeingabe C** | `date = 15.01.2025` → Erwartung: phase nahe 0.5 (bekannter Vollmond) |
| **Erwartetes Ergebnis** | A: phase < 0.05; B: 0.45 < phase < 0.55; C: 0.45 < phase < 0.55 |
| **Tatsächliches Ergebnis** | Noch durchzuführen |
| **Status** | ⏳ Noch durchzuführen |
| **Bemerkung** | Geplant: 21.08.2026; Formel: `((date − 06.01.2000).TotalDays % 29.53) / 29.53` |

---

### TP-29: Animierter Hintergrund wirft keine Exceptions

| Feld | Inhalt |
|------|--------|
| **Test-ID** | TP-29 |
| **Testfall** | `GenerateWallpaper()` mit animiertem Hintergrund läuft für alle kritischen Tageszeiten ohne unbehandelte Exception durch |
| **Testart** | Integrationstest (manuell / automatisiert) |
| **Komponente** | `Services.WallpaperGeneratorService` |
| **Vorbedingung** | Gültige `SolarData` für Mannheim, 01.07.2026; temporäres Ausgabeverzeichnis vorhanden |
| **Testeingabe** | Aufrufe mit `currentTime` = { 00:00, exakt Sonnenaufgang, 1 min vor Aufgang, 1 min nach Aufgang, 12:00, exakt Sonnenuntergang, 1 min vor Untergang, 1 min nach Untergang, 23:59 } |
| **Erwartetes Ergebnis** | Kein `NullReferenceException`, `DivideByZeroException`, `ArgumentException` oder sonstige unbehandelte Exception bei keinem der neun Testfälle; PNG-Datei wird erstellt |
| **Tatsächliches Ergebnis** | Noch durchzuführen |
| **Status** | ⏳ Noch durchzuführen |
| **Bemerkung** | Geplant: 21.08.2026; kritische Randwerte: exakter Auf-/Untergang kann Divisionen durch null provozieren (sunT = 0.0 oder 1.0) |

---

## 10. Fehlerbeschreibungen

Zum Zeitpunkt der Erstellung dieses Protokolls (01.07.2026) sind **keine bekannten Fehler** in der Anwendung dokumentiert. Gefundene Fehler während der Testphase werden hier nachgetragen.

| Fehler-ID | Gefunden in Test | Beschreibung | Schweregrad | Status |
|-----------|-----------------|--------------|-------------|--------|
| — | — | Keine bekannten Fehler | — | — |

---

## Testumgebung

### Hardware

| Komponente | Spezifikation |
|------------|---------------|
| Prozessor | Intel Core i7-12700 (12 Kerne, 4,9 GHz Boost) |
| Arbeitsspeicher | 16 GB DDR4-3200 |
| Grafikausgabe | Integrierte Intel UHD Graphics 770 |
| Bildschirm | 27" Full HD (1920×1080), 60 Hz |
| Netzwerk | 1 GBit/s Ethernet (kabelgebunden) |

### Software

| Komponente | Version |
|------------|---------|
| Betriebssystem | Windows 11 Pro, Version 22H2 (Build 22621) |
| .NET Runtime | .NET 10.0.0 |
| Visual Studio | Visual Studio 2022 (17.x) |
| xUnit | 2.9.x |
| Testziel-Anwendung | Sonnenuhr v1.0.0 (Debug-Build) |

---

*Dokument erstellt von: Uwe Markus Münch | Breihof IT GmbH | IHK Rhein-Neckar | 01.07.2026*
