# Pflichtenheft

## Sonnenuhr – Standortspezifischer Wallpaper-Generator für Windows 11

---

| Feld               | Inhalt                                      |
|--------------------|---------------------------------------------|
| **Projektname**    | Sonnenuhr – Standortspezifischer Wallpaper-Generator für Windows 11 |
| **Prüfling**       | Uwe Markus Münch                            |
| **Ausbildungsbetrieb** | Breihof IT GmbH                        |
| **IHK**            | IHK Rhein-Neckar                            |
| **Projektzeitraum**| 01.07.2026 – 31.08.2026                     |
| **Dokumentversion**| 1.0                                         |
| **Stand**          | 01.07.2026                                  |

---

## Inhaltsverzeichnis

1. [Einleitung](#1-einleitung)
2. [Ist-Analyse](#2-ist-analyse)
3. [Soll-Konzept und Anforderungen](#3-soll-konzept-und-anforderungen)
4. [Schnittstellen](#4-schnittstellen)
5. [Qualitätsanforderungen](#5-qualitätsanforderungen)
6. [Technische Rahmenbedingungen](#6-technische-rahmenbedingungen)
7. [Liefergegenstände](#7-liefergegenstände)
8. [Projektphasen und Zeitplanung](#8-projektphasen-und-zeitplanung)

---

## 1. Einleitung

### 1.1 Auftraggeber

**Breihof IT GmbH**  
Abteilung: Interne Softwareentwicklung  
Verantwortliche Person: Geschäftsführung / Ausbildungsleitung

Die Breihof IT GmbH ist ein mittelständisches IT-Unternehmen mit Sitz im Rhein-Neckar-Raum. Das Unternehmen bietet Dienstleistungen im Bereich Systemintegration, Softwareentwicklung und IT-Beratung für Geschäftskunden an. Im Rahmen der betrieblichen Ausbildung zum Fachinformatiker für Anwendungsentwicklung werden regelmäßig Abschlussprojekte realisiert, die einen praktischen Mehrwert für das Unternehmen oder dessen Mitarbeiter darstellen.

### 1.2 Auftragnehmer

**Uwe Markus Münch**  
Auszubildender: Fachinformatiker für Anwendungsentwicklung  
IHK Rhein-Neckar  
Ausbildungsjahr: 3. Lehrjahr

### 1.3 Vertragsgrundlage

Das vorliegende Pflichtenheft wurde auf Grundlage des internen Projektauftrags der Breihof IT GmbH erstellt und dient als verbindliche Spezifikation für die Durchführung und Abnahme des IHK-Abschlussprojekts. Es konkretisiert die im Lastenheft formulierten Anforderungen des Auftraggebers und legt fest, **wie** und **womit** diese Anforderungen umgesetzt werden.

Das Pflichtenheft bildet die Grundlage für:
- Die Bewertung des Projekts durch die IHK Rhein-Neckar
- Die interne Abnahme durch den Ausbildungsbetrieb
- Die Dokumentation des Entwicklungsprozesses

---

## 2. Ist-Analyse

### 2.1 Ausgangssituation

Mitarbeiterinnen und Mitarbeiter der Breihof IT GmbH nutzen Windows 11 als primäres Betriebssystem auf ihren Arbeitsplatzrechnern. Der Desktop-Hintergrund (Wallpaper) dient in vielen Fällen lediglich als statisches Bild ohne informativen Mehrwert.

Im Bereich der klassischen Zeitmessung existiert das Konzept der **Sonnenuhr** (lat. *Horologium solare*) seit der Antike. Eine Sonnenuhr nutzt den Stand der Sonne und den Schattenwurf eines Gnomons (Zeigers), um die Tageszeit abzulesen. Die genaue Ausrichtung der Stundenlinien einer Sonnenuhr ist dabei **standortabhängig** und wird durch den geografischen Breitengrad bestimmt.

Derzeit existiert keine standardmäßig integrierte oder leicht verfügbare Windows-Anwendung, die:
- Automatisch eine standortspezifische Sonnenuhr als Desktop-Wallpaper generiert
- Die tatsächlichen astronomischen Daten (Sonnenaufgang, Sonnenmittag, Sonnenuntergang) für den jeweiligen Standort und das aktuelle Datum berücksichtigt
- Sich automatisch im Tagesverlauf aktualisiert
- Konfigurierbar und anpassbar ist

### 2.2 Bestehende Probleme und Defizite

| Problem | Beschreibung |
|---------|--------------|
| **Statische Wallpaper** | Standard-Wallpaper bieten keine dynamischen oder informativen Inhalte; sie müssen manuell aktualisiert werden. |
| **Fehlende Standortindividualität** | Verfügbare Sonnenuhr-Apps berücksichtigen nicht den genauen geografischen Standort des Benutzers. |
| **Keine Windows-11-Integration** | Vorhandene Sonnenuhr-Software ist meist für andere Plattformen konzipiert oder bietet keine nahtlose Windows-Integration (Autostart, Registry, Systemtray). |
| **Manuelle Zeitdatenpflege** | Anwendungen, die Sonnenaufgangs- und Sonnenuntergangsdaten benötigen, erfordern häufig manuelle Eingaben oder bieten keine automatische API-Anbindung. |
| **Fehlende Konfigurierbarkeit** | Bestehende Lösungen bieten kaum Möglichkeiten zur individuellen Anpassung von Farben, Schriften und Anzeigeoptionen. |

### 2.3 Handlungsbedarf

Aus der beschriebenen Ausgangssituation ergibt sich der Bedarf nach einer Softwarelösung, die:
1. Automatisch astronomische Daten für einen konfigurierbaren Standort abruft
2. Auf Basis dieser Daten eine geometrisch korrekte Sonnenuhr berechnet und zeichnet
3. Das Ergebnis als Desktop-Wallpaper unter Windows 11 setzt
4. Sich in definierten Zeitintervallen selbst aktualisiert
5. Eine benutzerfreundliche Konfigurationsoberfläche bereitstellt

---

## 3. Soll-Konzept und Anforderungen

### 3.1 Zielsetzung

Das Projekt **Sonnenuhr** hat das Ziel, eine Windows-Desktopanwendung zu entwickeln, die auf Basis des geografischen Standorts des Benutzers und der tagesaktuellen astronomischen Sonnendaten eine stilisierte Sonnenuhr als Desktop-Wallpaper generiert und automatisch setzt.

Die Anwendung soll nahtlos in Windows 11 integriert sein, eine ansprechende Benutzeroberfläche bieten und vollständig konfigurierbar sein.

### 3.2 Muss-Kriterien

Die folgenden Anforderungen **müssen** zwingend erfüllt sein, damit das Projekt als abgeschlossen gilt:

| Nr. | Anforderung | Beschreibung |
|-----|-------------|--------------|
| M-01 | **Standortkonfiguration** | Der Benutzer muss Breitengrad und Längengrad seines Standorts manuell eingeben können. Zusätzlich muss ein Ortsname gespeichert werden. |
| M-02 | **REST-API-Integration** | Die Anwendung muss die öffentliche REST-API von `sunrise-sunset.org` verwenden, um tagesaktuelle Sonnendaten (Sonnenaufgang, Sonnenuntergang, Sonnenmittag, Tageslänge, bürgerliche Dämmerung) abzurufen. |
| M-03 | **Sonnenuhr-Berechnung** | Die Stundenlinienwinkel der Sonnenuhr müssen mathematisch korrekt auf Basis des geografischen Breitengrads berechnet werden. |
| M-04 | **Wallpaper-Generierung** | Die Anwendung muss ein Bild (PNG-Format) generieren, das eine stilisierte Sonnenuhr mit Stundenlinien, Gnomon-Symbol und aktueller Schattenposition darstellt. |
| M-05 | **Wallpaper setzen** | Das generierte Bild muss automatisch als Desktop-Hintergrund unter Windows 11 gesetzt werden (via Windows Registry und `SystemParametersInfo`). |
| M-06 | **Automatische Aktualisierung** | Die Anwendung muss einen konfigurierbaren Timer besitzen, der in definierten Intervallen (standardmäßig 60 Minuten) die Sonnendaten abruft und das Wallpaper neu generiert. |
| M-07 | **Konfigurationsdialog** | Es muss ein Konfigurationsdialog existieren, über den der Benutzer Farben, Schriftarten und Anzeigeoptionen anpassen kann. |
| M-08 | **JSON-Konfigurationsspeicherung** | Alle Benutzereinstellungen müssen persistent in einer JSON-Datei unter `%APPDATA%\Sonnenuhr\settings.json` gespeichert werden. |
| M-09 | **Autostart-Funktion** | Die Anwendung muss optional beim Windows-Start automatisch gestartet werden können (via Windows Registry, `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`). |
| M-10 | **Fehlerbehandlung** | Bei Netzwerkfehlern, ungültigen API-Antworten oder anderen Laufzeitfehlern muss die Anwendung eine verständliche Fehlermeldung anzeigen und stabil bleiben (keine unbehandelten Exceptions). |
| M-11 | **Bildauflösung** | Das generierte Wallpaper muss in der konfigurierten Auflösung (Standard: 1920×1080 Pixel) erstellt werden. |
| M-12 | **Zeitzone** | Die Anwendung muss Sonnenzeiten korrekt von UTC in die lokale Zeitzone des Benutzers umrechnen. |

### 3.3 Kann-Kriterien

Die folgenden Anforderungen **können** umgesetzt werden, sofern Zeit und Ressourcen es erlauben:

| Nr. | Anforderung | Beschreibung |
|-----|-------------|--------------|
| K-01 | **System-Tray-Integration** | Die Anwendung kann in den Windows-Systemtray minimiert werden, sodass sie im Hintergrund läuft ohne die Taskleiste zu belasten. |
| K-02 | **Römische Ziffern** | Die Stundenmarkierungen auf der Sonnenuhr können optional als römische Ziffern dargestellt werden. |
| K-03 | **Dämmerungslinien** | Auf der Sonnenuhr können optionale Linien für Sonnenaufgang und Sonnenuntergang sowie die bürgerliche Dämmerung eingezeichnet werden. |
| K-04 | **Aktueller Stundenzeiger** | Die aktuelle Stunde kann auf dem Wallpaper optisch hervorgehoben werden. |
| K-05 | **Mehrere Farbthemen** | Es können voreingestellte Farbthemen (z.B. „Dunkel", „Hell", „Klassisch") zur Auswahl angeboten werden. |
| K-06 | **Standortname im Wallpaper** | Der konfigurierte Ortsname kann als Text im generierten Wallpaper eingeblendet werden. |
| K-07 | **Anzeige von Uhrzeit** | Die aktuelle Uhrzeit kann optional im Wallpaper angezeigt werden. |
| K-08 | **Stadtsuche via Geocoding-API** | Statt manueller Koordinateneingabe kann der Benutzer einen Stadtnamen suchen. Die Koordinaten werden über die OpenStreetMap Nominatim API automatisch ermittelt. Bei mehreren gleichnamigen Orten öffnet sich ein Auswahldialog. |

### 3.4 Abgrenzungskriterien

Die folgenden Punkte sind **ausdrücklich nicht** Gegenstand dieses Projekts:

| Nr. | Abgrenzung |
|-----|------------|
| A-01 | Die Anwendung unterstützt **keine** anderen Betriebssysteme als Windows 11 (kein macOS, kein Linux, kein Windows 10 oder älter). |
| A-02 | Es erfolgt **keine** automatische GPS-Standortermittlung. Die manuelle Koordinateneingabe wird durch eine **Stadtsuche-Funktion** (Nominatim API) ergänzt, die die Eingabe vereinfacht. |
| A-03 | Die Anwendung bietet **keine** eigene Wetterdaten-Integration oder Wettervorhersagefunktion. |
| A-04 | Es wird **kein** Mehrbenutzer-Betrieb unterstützt; die Konfiguration ist benutzerspezifisch (pro Windows-Benutzerprofil). |
| A-05 | Die Anwendung bietet **keine** Datenbankintegration; die Datenspeicherung erfolgt ausschließlich über JSON-Dateien. |
| A-06 | Es werden **keine** Benachrichtigungen (Push-Notifications) für Sonnenauf- oder -untergang implementiert. |
| A-07 | Die Anwendung unterstützt **keine** Mehrmonitor-Konfigurationen mit unterschiedlichen Wallpapern pro Monitor. |
| A-08 | Es wird **kein** Web-Interface oder Remote-Zugriff bereitgestellt. |

---

## 4. Schnittstellen

### 4.1 Externe Schnittstelle: Sunrise-Sunset API

| Eigenschaft | Beschreibung |
|-------------|--------------|
| **Anbieter** | sunrise-sunset.org |
| **Basis-URL** | `https://api.sunrise-sunset.org/json` |
| **Protokoll** | HTTPS (REST) |
| **Methode** | HTTP GET |
| **Authentifizierung** | Keine (öffentliche API, kostenlos) |
| **Anfrage-Parameter** | `lat` (Breitengrad), `lng` (Längengrad), `date` (Datum, ISO 8601), `formatted=0` (UTC-Timestamps) |
| **Antwortformat** | JSON |
| **Rate-Limiting** | Keine offiziellen Limits; verantwortungsvoller Umgang vorgesehen |

**Beispiel-Request:**
```
GET https://api.sunrise-sunset.org/json?lat=49.4875&lng=8.4660&date=today&formatted=0
```

**Beispiel-Response (gekürzt):**
```json
{
  "results": {
    "sunrise": "2026-07-01T03:18:00+00:00",
    "sunset": "2026-07-01T19:42:00+00:00",
    "solar_noon": "2026-07-01T11:30:00+00:00",
    "day_length": 59040,
    "civil_twilight_begin": "2026-07-01T02:45:00+00:00",
    "civil_twilight_end": "2026-07-01T20:15:00+00:00"
  },
  "status": "OK"
}
```

### 4.2 Externe Schnittstelle: OpenStreetMap Nominatim API

| Eigenschaft | Beschreibung |
|-------------|--------------|
| **Anbieter** | OpenStreetMap / Nominatim-Projekt |
| **Basis-URL** | `https://nominatim.openstreetmap.org/search` |
| **Protokoll** | HTTPS (REST) |
| **Methode** | HTTP GET |
| **Authentifizierung** | Keine; Pflicht: aussagekräftiger `User-Agent`-Header gemäß Nutzungsbedingungen |
| **Anfrage-Parameter** | `q` (Suchbegriff), `format=jsonv2`, `limit=10`, `featuretype=settlement` |
| **Antwortformat** | JSON-Array mit GeocodingResult-Objekten |
| **Rate-Limiting** | Max. 1 Anfrage/Sekunde gemäß Nutzungsbedingungen; in der Anwendung durch Benutzerinteraktion natürlich begrenzt |

**Beispiel-Request:**
```
GET https://nominatim.openstreetmap.org/search?q=Eberbach%2C+Deutschland&format=jsonv2&limit=10&featuretype=settlement
```

**Beispiel-Response (gekürzt):**
```json
[
  {
    "display_name": "Eberbach, Rhein-Neckar-Kreis, Baden-Württemberg, Deutschland",
    "lat": "49.46788",
    "lon": "8.99278",
    "type": "town",
    "importance": 0.5324
  }
]
```

### 4.3 Systemschnittstelle: Windows Registry

| Schlüssel | Pfad | Zweck |
|-----------|------|-------|
| **Wallpaper** | `HKCU\Control Panel\Desktop\Wallpaper` | Pfad zum aktuellen Wallpaper-Bild |
| **Autostart** | `HKCU\Software\Microsoft\Windows\CurrentVersion\Run\Sonnenuhr` | Autostart-Eintrag der Anwendung |

Die Windows-API-Funktion `SystemParametersInfo(SPI_SETDESKWALLPAPER, ...)` wird verwendet, um den Desktop-Hintergrund nach dem Setzen des Registry-Eintrags zu aktualisieren.

### 4.4 Dateisystemschnittstelle

| Pfad | Inhalt | Format |
|------|--------|--------|
| `%APPDATA%\Sonnenuhr\settings.json` | Benutzereinstellungen (Standort, Wallpaper-Konfiguration, Timer-Intervall) | JSON |
| `%APPDATA%\Sonnenuhr\wallpaper.png` | Zuletzt generiertes Wallpaper-Bild | PNG |
| `%APPDATA%\Sonnenuhr\sonnenuhr.log` | Anwendungslog für Fehlerdiagnose | Textdatei |

### 4.5 Interne Komponentenschnittstellen

Die Kommunikation zwischen den internen Komponenten erfolgt über klar definierte C#-Interfaces und Methodenaufrufe. Details sind dem Klassendiagramm zu entnehmen.

---

## 5. Qualitätsanforderungen

### 5.1 Performance

| Anforderung | Zielwert |
|-------------|----------|
| Wallpaper-Generierungszeit | ≤ 2 Sekunden auf Referenzhardware (Intel Core i5, 8 GB RAM) |
| API-Antwortzeit (Netzwerk) | ≤ 5 Sekunden (Timeout nach 10 Sekunden) |
| Anwendungsstart (Kaltstart) | ≤ 3 Sekunden |
| Speicherverbrauch (RAM) | ≤ 150 MB im Normalbetrieb |

### 5.2 Usability (Gebrauchstauglichkeit)

- Die Benutzeroberfläche muss intuitiv und ohne Schulung bedienbar sein.
- Alle Felder müssen mit aussagekräftigen Tooltips versehen sein.
- Fehlermeldungen müssen in verständlicher Sprache formuliert sein und Lösungshinweise enthalten.
- Die Anwendung muss auf Bildschirmen mit einer Auflösung von mindestens 1024×768 Pixeln vollständig dargestellt werden können.
- Die Schriftgröße muss den Windows-Systemeinstellungen für DPI-Skalierung entsprechen.

### 5.3 Stabilität und Zuverlässigkeit

- Die Anwendung darf bei Netzwerkfehlern nicht abstürzen.
- Alle Exceptions müssen behandelt und protokolliert werden.
- Bei einem fehlerhaften API-Response muss das zuletzt generierte Wallpaper beibehalten werden.
- Die Anwendung muss nach einer Netzwerkunterbrechung automatisch beim nächsten Timer-Tick erneut versuchen, die API aufzurufen.

### 5.4 Wartbarkeit

- Der Quellcode muss nach dem Prinzip der Separation of Concerns strukturiert sein (Services, Models, Forms).
- Alle öffentlichen Methoden und Klassen müssen mit XML-Dokumentationskommentaren versehen sein.
- Unit-Tests müssen für die Kernkomponenten `SundialCalculator` und `Location` vorhanden sein.

### 5.5 Sicherheit

- API-Aufrufe erfolgen ausschließlich über HTTPS.
- Es werden keine persönlichen Daten an Dritte übermittelt (die API-Aufrufe enthalten nur geografische Koordinaten).
- Es werden keine Passwörter oder Authentifizierungstoken gespeichert.

---

## 6. Technische Rahmenbedingungen

### 6.1 Zielbetriebssystem

| Eigenschaft | Wert |
|-------------|------|
| Betriebssystem | Windows 11 (22H2 oder neuer) |
| Architektur | x64 |
| .NET Runtime | .NET 10.0 (Windows) |
| Mindest-RAM | 4 GB |
| Festplattenspeicher | 50 MB (inkl. Laufzeitabhängigkeiten) |
| Netzwerk | Internetverbindung für API-Abrufe erforderlich |

### 6.2 Entwicklungsumgebung

| Eigenschaft | Wert |
|-------------|------|
| IDE | Microsoft Visual Studio 2022 (Community Edition) |
| Programmiersprache | C# 13 (.NET 10) |
| Versionskontrolle | Git / GitHub |
| Paketverwaltung | NuGet |
| Build-System | MSBuild (.csproj) |

### 6.3 Verwendete Frameworks und Bibliotheken

| Framework / Bibliothek | Version | Zweck |
|------------------------|---------|-------|
| .NET 10 Windows Forms | 10.0 | UI-Framework |
| System.Drawing.Common | 10.0 | Bildgenerierung (GDI+) |
| System.Net.Http (HttpClient) | Integriert | REST-API-Aufrufe |
| System.Text.Json | Integriert | JSON-Serialisierung / -Deserialisierung |
| Microsoft.Win32 (Registry) | Integriert | Windows Registry-Zugriff |
| xUnit | 2.x | Unit-Testing |

### 6.4 Architekturprinzipien

Die Anwendung wird nach dem Prinzip der **Schichtenarchitektur** entwickelt:

1. **Präsentationsschicht (Forms):** `MainForm`, `ConfigurationForm`
2. **Serviceschicht (Services):** `SolarApiService`, `WallpaperGeneratorService`, `WallpaperRegistryService`, `ConfigurationService`, `SundialCalculator`
3. **Datenmodellschicht (Models):** `Location`, `SolarData`, `WallpaperConfig`, `AppSettings`

---

## 7. Liefergegenstände

| Nr. | Liefergegenstand | Format | Beschreibung |
|-----|-----------------|--------|--------------|
| L-01 | Lauffähige Anwendung | `.exe` + Abhängigkeiten | Kompiliertes Windows-Programm, lauffähig unter Windows 11 mit .NET 10 Runtime |
| L-02 | Quellcode | C#-Projektmappe (.sln) | Vollständiger, kommentierter Quellcode, eingecheckt in Git-Repository |
| L-03 | Pflichtenheft | Markdown / PDF | Dieses Dokument |
| L-04 | Klassendiagramm | Markdown / PNG | UML-Klassendiagramm aller Klassen und ihrer Beziehungen |
| L-05 | Aktivitätsdiagramm | Markdown / PNG | Ablaufdiagramme der Hauptprozesse |
| L-06 | Sequenzdiagramm | Markdown / PNG | Sequenzdiagramme der wichtigsten Anwendungsszenarien |
| L-07 | ER-Diagramm | Markdown / PNG | Logisches Datenstrukturdiagramm |
| L-08 | Mockups / Wireframes | Markdown | UI-Entwürfe für alle Dialogfenster |
| L-09 | Testprotokoll | Markdown / PDF | Dokumentation aller durchgeführten Tests mit Ergebnissen |
| L-10 | Benutzerhandbuch | Markdown / PDF | Bedienungsanleitung für Endbenutzer |
| L-11 | Installationsanleitung | Markdown / PDF | Schritt-für-Schritt-Installationsanleitung |
| L-12 | Soll-Ist-Vergleich | Markdown / PDF | Abgleich der geplanten mit den tatsächlich erreichten Zielen |
| L-13 | Projektdokumentation | PDF | Vollständige IHK-Projektdokumentation (Zusammenfassung) |

---

## 8. Projektphasen und Zeitplanung

### 8.1 Übersicht Projektphasen

Der Gesamtaufwand beträgt **80 Stunden** und verteilt sich auf folgende Phasen:

| Phase | Bezeichnung | Geplante Stunden | Zeitraum |
|-------|-------------|-----------------|----------|
| 1 | Analyse und Planung | 10 Std. | 01.07. – 07.07.2026 |
| 2 | Entwurf (Design) | 12 Std. | 08.07. – 17.07.2026 |
| 3 | Implementierung | 38 Std. | 18.07. – 14.08.2026 |
| 4 | Test und Qualitätssicherung | 10 Std. | 17.08. – 24.08.2026 |
| 5 | Dokumentation und Abschluss | 10 Std. | 25.08. – 31.08.2026 |
| **Gesamt** | | **80 Std.** | |

### 8.2 Detaillierter Ablaufplan

#### Phase 1: Analyse und Planung (10 Stunden)

| Aufgabe | Stunden |
|---------|---------|
| Anforderungsanalyse, Gespräche mit Ausbildungsleitung | 2 Std. |
| Recherche: Sonnenuhr-Mathematik, Stundenlinienberechnung | 2 Std. |
| Recherche: sunrise-sunset.org API, JSON-Format | 1 Std. |
| Erstellung Lastenheft und Pflichtenheft | 3 Std. |
| Zeitplanung, Meilensteine definieren | 1 Std. |
| Technische Machbarkeitsprüfung | 1 Std. |

#### Phase 2: Entwurf (12 Stunden)

| Aufgabe | Stunden |
|---------|---------|
| Architekturentwurf (Schichtenmodell, Klassenstruktur) | 3 Std. |
| UML-Klassendiagramm erstellen | 2 Std. |
| Aktivitäts- und Sequenzdiagramme erstellen | 2 Std. |
| UI-Mockups und Wireframes erstellen | 2 Std. |
| Datenbankmodell / JSON-Struktur entwerfen | 1 Std. |
| ER-Diagramm erstellen | 1 Std. |
| Review des Entwurfs mit Ausbilder | 1 Std. |

#### Phase 3: Implementierung (38 Stunden)

| Aufgabe | Stunden |
|---------|---------|
| Projektstruktur anlegen, NuGet-Pakete einrichten | 1 Std. |
| Datenmodelle implementieren (`Models`) | 2 Std. |
| `ConfigurationService` (JSON laden/speichern) | 3 Std. |
| `SolarApiService` (HTTP-Client, API-Integration) | 4 Std. |
| `SundialCalculator` (Stundenlinien-Mathematik) | 5 Std. |
| `WallpaperGeneratorService` (GDI+ Bildgenerierung) | 10 Std. |
| `WallpaperRegistryService` (Registry, SystemParametersInfo) | 3 Std. |
| `MainForm` (Hauptfenster, Timer, Koordination) | 5 Std. |
| `ConfigurationForm` (Konfigurationsdialog) | 3 Std. |
| System-Tray-Integration | 2 Std. |

#### Phase 4: Test und Qualitätssicherung (10 Stunden)

| Aufgabe | Stunden |
|---------|---------|
| Unit-Tests für `SundialCalculator` schreiben und ausführen | 3 Std. |
| Unit-Tests für `Location` und `ConfigurationService` | 2 Std. |
| Integrationstests (API, Wallpaper-Setzung) | 2 Std. |
| UI-Tests (manuelle Durchführung) | 2 Std. |
| Testprotokoll ausfüllen, Fehler beheben | 1 Std. |

#### Phase 5: Dokumentation und Abschluss (10 Stunden)

| Aufgabe | Stunden |
|---------|---------|
| Benutzerhandbuch erstellen | 2 Std. |
| Installationsanleitung erstellen | 1 Std. |
| Soll-Ist-Vergleich erstellen | 1 Std. |
| Gesamtdokumentation zusammenführen (IHK-Format) | 3 Std. |
| Abschlusspräsentation vorbereiten | 2 Std. |
| Projektabnahme mit Ausbilder | 1 Std. |

### 8.3 Meilensteine

| Meilenstein | Datum | Kriterium |
|-------------|-------|-----------|
| M1: Entwurf abgeschlossen | 17.07.2026 | Alle UML-Diagramme und Mockups fertig |
| M2: Kern-Services implementiert | 01.08.2026 | API-Aufruf, Berechnung und Bildgenerierung funktionieren |
| M3: Anwendung lauffähig | 14.08.2026 | Vollständige Anwendung ohne bekannte kritische Fehler |
| M4: Tests abgeschlossen | 24.08.2026 | Alle Testfälle dokumentiert und bestanden |
| M5: Projektabschluss | 31.08.2026 | Vollständige Dokumentation, Abgabe bei IHK |

---

*Dokument erstellt von: Uwe Markus Münch | Breihof IT GmbH | IHK Rhein-Neckar | 01.07.2026*
