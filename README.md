# Sonnenuhr – Standortspezifischer Wallpaper-Generator

<div align="center">

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Platform](https://img.shields.io/badge/Platform-Windows%2011-0078D6?logo=windows)
![Language](https://img.shields.io/badge/Language-C%23-239120?logo=csharp)
![License](https://img.shields.io/badge/License-MIT-green)

**Automatisch generierte, standortspezifische Sonnenuhr-Wallpaper für Windows 11**

*IHK-Abschlussprojekt · Fachinformatiker Anwendungsentwicklung · Breihof IT GmbH*

</div>

---

## 📋 Projektbeschreibung

Die **Sonnenuhr**-Anwendung generiert automatisch personalisierte Desktop-Hintergrundbilder,
die eine horizontale Sonnenuhr mit standortspezifischen astronomischen Daten darstellen.
Auf Basis geografischer Koordinaten werden die Stundenlinien-Winkel präzise berechnet
und die aktuellen Sonnenauf- und Sonnenuntergangszeiten über eine externe REST-API bezogen.

### ✨ Hauptfunktionen

| Funktion | Beschreibung |
|---|---|
| 🌍 **Standortbasiert** | Sonnenuhr-Berechnung für beliebige geografische Koordinaten |
| 🌅 **REST-API** | Sonnenstandsdaten via `api.sunrise-sunset.org` |
| 🎨 **Individualisierbar** | Farbschema, Schriften und Anzeigeelemente konfigurierbar |
| ⏱️ **Automatisch** | Timer-gesteuerte Wallpaper-Aktualisierung |
| 🔔 **System-Tray** | Läuft diskret im Hintergrund |
| 🚀 **Autostart** | Optionaler Programmstart mit Windows |

---

## 🔧 Systemvoraussetzungen

- **Betriebssystem:** Windows 11 (22H2 oder neuer)
- **Runtime:** [.NET 10 Runtime](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Netzwerk:** Internetverbindung für API-Abfragen
- **Speicher:** min. 50 MB freier Festplattenspeicher

---

## 🚀 Schnellstart

### Option A – Release herunterladen

1. Unter [Releases](../../releases) die aktuelle Version herunterladen
2. `Sonnenuhr.exe` starten
3. Standort eingeben und „**⟳ Jetzt aktualisieren**" klicken

### Option B – Aus Quellcode bauen

```bash
# Repository klonen
git clone https://github.com/umuench/sonnenuhr.git
cd sonnenuhr

# Anwendung bauen und starten
dotnet build src/Sonnenuhr/Sonnenuhr.csproj -c Release
dotnet run --project src/Sonnenuhr/Sonnenuhr.csproj
```

---

## 📁 Projektstruktur

```
sonnenuhr/
├── src/
│   └── Sonnenuhr/                  # Hauptprojekt (Windows Forms)
│       ├── Forms/                  # UI-Formulare
│       │   ├── MainForm.cs         # Hauptfenster
│       │   └── ConfigurationForm.cs # Einstellungsdialog
│       ├── Models/                 # Datenmodelle
│       │   ├── Location.cs         # Geografischer Standort
│       │   ├── SolarData.cs        # API-Sonnenstandsdaten
│       │   ├── WallpaperConfig.cs  # Visuelle Konfiguration
│       │   └── AppSettings.cs      # Anwendungseinstellungen
│       ├── Services/               # Geschäftslogik
│       │   ├── SolarApiService.cs  # REST-API-Client
│       │   ├── SundialCalculator.cs # Astronomische Berechnungen
│       │   ├── WallpaperGeneratorService.cs # Bildgenerierung
│       │   ├── WallpaperRegistryService.cs  # Windows-Registry
│       │   └── ConfigurationService.cs      # JSON-Persistenz
│       └── Program.cs              # Einstiegspunkt
├── tests/
│   └── Sonnenuhr.Tests/            # xUnit-Unit-Tests
│       ├── SundialCalculatorTests.cs
│       └── LocationTests.cs
├── docs/                           # Projektdokumentation (IHK)
│   ├── Pflichtenheft.md
│   ├── Klassendiagramm.md
│   ├── Aktivitaetsdiagramm.md
│   ├── Sequenzdiagramm.md
│   ├── ER-Diagramm.md
│   ├── Mockups.md
│   ├── Testprotokoll.md
│   ├── Benutzerhandbuch.md
│   ├── Installationsanleitung.md
│   └── Soll-Ist-Vergleich.md
└── Sonnenuhr.sln
```

---

## 🏗️ Architektur

Die Anwendung folgt einer **geschichteten Architektur**:

```
┌─────────────────────────────────────┐
│           Präsentationsschicht       │
│     (Windows Forms – MainForm,       │
│      ConfigurationForm)              │
├─────────────────────────────────────┤
│           Serviceschicht             │
│  SolarApiService │ SundialCalculator │
│  WallpaperGenerator │ ConfigService  │
│  WallpaperRegistryService            │
├─────────────────────────────────────┤
│           Modellschicht              │
│  Location │ SolarData │ AppSettings  │
│  WallpaperConfig                     │
└─────────────────────────────────────┘
```

### EVA-Prinzip im Code

Alle Methoden verwenden das **Eingabe–Verarbeitung–Ausgabe**-Prinzip:

```csharp
public static double CalculateHourLineAngle(int hourOffsetFromNoon, double latitudeDegrees)
{
    // ── EINGABE ────────────────────────────────────────────
    double latRad       = DegreesToRadians(latitudeDegrees);
    double hourAngleRad = DegreesToRadians(hourOffsetFromNoon * 15.0);

    // ── VERARBEITUNG ───────────────────────────────────────
    // Formel: H = atan(sin(φ) · tan(t))
    double hourLineRad = Math.Atan(Math.Sin(latRad) * Math.Tan(hourAngleRad));

    // ── AUSGABE ────────────────────────────────────────────
    return RadiansToDegrees(hourLineRad);
}
```

---

## 🧪 Tests ausführen

```bash
dotnet test tests/Sonnenuhr.Tests/Sonnenuhr.Tests.csproj --verbosity normal
```

---

## 📚 Dokumentation

Die vollständige IHK-Projektdokumentation befindet sich im Verzeichnis [`docs/`](docs/):

| Dokument | Beschreibung |
|---|---|
| [Pflichtenheft](docs/Pflichtenheft.md) | Anforderungen und Rahmenbedingungen |
| [Klassendiagramm](docs/Klassendiagramm.md) | UML-Klassenstruktur |
| [Aktivitätsdiagramm](docs/Aktivitaetsdiagramm.md) | Programmablauf |
| [Sequenzdiagramm](docs/Sequenzdiagramm.md) | Interaktionsabläufe |
| [ER-Diagramm](docs/ER-Diagramm.md) | Datenstrukturmodell |
| [Mockups](docs/Mockups.md) | UI-Wireframes und Entwürfe |
| [Testprotokoll](docs/Testprotokoll.md) | Testergebnisse |
| [Benutzerhandbuch](docs/Benutzerhandbuch.md) | Anleitung für Endbenutzer |
| [Installationsanleitung](docs/Installationsanleitung.md) | Installationsschritte |
| [Soll-Ist-Vergleich](docs/Soll-Ist-Vergleich.md) | Projektevaluation |

---

## 🔗 Externe Abhängigkeiten

| Komponente | Version | Lizenz | Zweck |
|---|---|---|---|
| [sunrise-sunset.org API](https://sunrise-sunset.org/api) | v2 | CC0 | Astronomische Sonnendaten |
| System.Text.Json | 9.x | MIT | JSON-Serialisierung |

---

## 👤 Autor

**Uwe Markus Münch**  
IHK-Prüfling Nr. 10350411  
Ausbildungsbetrieb: Breihof IT GmbH  
IHK-Bezirk: Rhein-Neckar  

---

## 📄 Lizenz

Dieses Projekt ist unter der [MIT-Lizenz](LICENSE) lizenziert.
