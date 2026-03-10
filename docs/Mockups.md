# Mockups und Wireframes

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

1. [Hauptfenster (MainForm)](#1-hauptfenster-mainform)
2. [Konfigurationsdialog (ConfigurationForm)](#2-konfigurationsdialog-configurationform)
3. [Generiertes Wallpaper (1920×1080)](#3-generiertes-wallpaper-1920x1080)
4. [Systemtray-Kontextmenü](#4-systemtray-kontextmenü)

---

## 1. Hauptfenster (MainForm)

### 1.1 Übersicht

Das Hauptfenster der Sonnenuhr-Anwendung hat eine feste Größe von **520 × 560 Pixeln** und ist nicht in der Größe veränderbar. Es bietet eine übersichtliche Darstellung aller relevanten Informationen und Steuerelemente auf einem einzigen Bildschirm.

Das Fenster ist in drei funktionale Gruppen (GroupBoxen) unterteilt:
- **Standort-Konfiguration:** Eingabe und Anzeige der geografischen Position
- **Aktuelle Sonnenzeitdaten:** Anzeige der von der API abgerufenen astronomischen Daten
- **Einstellungen:** Konfiguration von Aktualisierungsintervall und Autostart

### 1.2 Wireframe

```
┌─────────────────────────────────────────────────────────────┐
│ ☀ Sonnenuhr – Wallpaper-Generator               [_][□][X] │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─── Standort-Konfiguration ──────────────────────────┐   │
│  │                                                      │   │
│  │  Ortsname:    [________________________________]     │   │
│  │                                                      │   │
│  │  Breitengrad: [__________] °N  (z.B. 49.4875)       │   │
│  │                                                      │   │
│  │  Längengrad:  [__________] °E  (z.B. 8.4660)        │   │
│  │                                                      │   │
│  │  Zeitzone:    [▼ W. Europe Standard Time         ]   │   │
│  │                                                      │   │
│  └──────────────────────────────────────────────────────┘   │
│                                                             │
│  ┌─── Aktuelle Sonnenzeitdaten ────────────────────────┐   │
│  │                                                      │   │
│  │  📅 Datum:           Mittwoch, 01.07.2026           │   │
│  │                                                      │   │
│  │  🌅 Sonnenaufgang:   05:18 Uhr  (MESZ)             │   │
│  │                                                      │   │
│  │  🌞 Sonnenmittag:    13:30 Uhr  (MESZ)             │   │
│  │                                                      │   │
│  │  🌇 Sonnenuntergang: 21:42 Uhr  (MESZ)             │   │
│  │                                                      │   │
│  │  ⏱ Tageslänge:       16 Std. 24 Min.              │   │
│  │                                                      │   │
│  │  🌆 Bürg. Dämmerung: 04:45 – 22:15 Uhr (MESZ)     │   │
│  │                                                      │   │
│  └──────────────────────────────────────────────────────┘   │
│                                                             │
│  ┌─── Einstellungen ───────────────────────────────────┐   │
│  │                                                      │   │
│  │  Aktualisierung alle: [▲ 60 ▼] Minuten             │   │
│  │                                                      │   │
│  │  [☑] Mit Windows starten (Autostart)               │   │
│  │                                                      │   │
│  └──────────────────────────────────────────────────────┘   │
│                                                             │
│   ┌─────────────────────────┐  ┌─────────────────────┐     │
│   │  ⟳  Jetzt aktualisieren │  │  ⚙  Darstellung     │     │
│   └─────────────────────────┘  └─────────────────────┘     │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│ ✓ Letztes Update: 01.07.2026 – 09:00:42 Uhr               │
└─────────────────────────────────────────────────────────────┘
  520 px Breite × 560 px Höhe
```

### 1.3 Steuerelemente – Beschreibung

| Steuerelement | Typ | Bindung | Beschreibung |
|---------------|-----|---------|--------------|
| **Ortsname** | TextBox | `_settings.Location.Name` | Benutzerfreundlicher Name des Standorts |
| **Breitengrad** | NumericUpDown | `_settings.Location.Latitude` | Geografischer Breitengrad (−90° bis +90°) |
| **Längengrad** | NumericUpDown | `_settings.Location.Longitude` | Geografischer Längengrad (−180° bis +180°) |
| **Zeitzone** | ComboBox | `_settings.TimeZoneId` | Dropdown mit allen Windows-Zeitzonen |
| **Datum** | Label | Aktuelles Systemdatum | Schreibgeschützte Anzeige |
| **Sonnenaufgang** | Label | `_currentSolarData.Sunrise` | In lokaler Zeit angezeigt |
| **Sonnenmittag** | Label | `_currentSolarData.SolarNoon` | In lokaler Zeit angezeigt |
| **Sonnenuntergang** | Label | `_currentSolarData.Sunset` | In lokaler Zeit angezeigt |
| **Tageslänge** | Label | `_currentSolarData.DayLength` | Als Stunden und Minuten formatiert |
| **Aktualisierungsintervall** | NumericUpDown | `_settings.UpdateIntervalMinutes` | Wertebereich: 1–1440 Minuten |
| **Autostart-Checkbox** | CheckBox | `_settings.StartWithWindows` | Ändert Registry-Eintrag beim Ändern |
| **Jetzt aktualisieren** | Button | `btnRefresh_Click` | Löst sofortige Wallpaper-Aktualisierung aus |
| **Darstellung** | Button | `btnConfig_Click` | Öffnet den Konfigurationsdialog |
| **Statusbar** | StatusLabel | Laufender Betrieb | Zeigt letzten Aktualisierungszeitpunkt |

---

## 2. Konfigurationsdialog (ConfigurationForm)

### 2.1 Übersicht

Der Konfigurationsdialog hat eine feste Größe von **420 × 460 Pixeln** und öffnet sich als modales Fenster über dem Hauptfenster. Er ermöglicht die vollständige Konfiguration aller visuellen Eigenschaften des generierten Wallpapers.

### 2.2 Wireframe

```
┌───────────────────────────────────────────────────┐
│ ⚙ Darstellungsoptionen                  [_][X]   │
├───────────────────────────────────────────────────┤
│                                                   │
│  ┌─── Farbschema ──────────────────────────────┐  │
│  │                                             │  │
│  │  Hintergrund:  [████████] [Farbe wählen...] │  │
│  │                #1A1A2E (Dunkelblau)          │  │
│  │                                             │  │
│  │  Primärfarbe:  [████████] [Farbe wählen...] │  │
│  │                #FFD700 (Gold)               │  │
│  │                                             │  │
│  │  Sekundärfarbe:[████████] [Farbe wählen...] │  │
│  │                #FFFFFF (Weiß)               │  │
│  │                                             │  │
│  │  Akzentfarbe:  [████████] [Farbe wählen...] │  │
│  │                #FF8C00 (Orange)             │  │
│  │                                             │  │
│  └─────────────────────────────────────────────┘  │
│                                                   │
│  ┌─── Schrift ─────────────────────────────────┐  │
│  │                                             │  │
│  │  Schriftart:  [▼ Segoe UI                 ] │  │
│  │                                             │  │
│  │  Schriftgröße:[▲ 14 ▼] pt                 │  │
│  │                                             │  │
│  └─────────────────────────────────────────────┘  │
│                                                   │
│  ┌─── Anzeigeoptionen ─────────────────────────┐  │
│  │                                             │  │
│  │  [☑] Aktuelle Uhrzeit anzeigen             │  │
│  │  [☑] Ortsname anzeigen                     │  │
│  │  [☑] Sonnenaufgang / -untergang anzeigen   │  │
│  │  [☐] Dämmerungslinien einzeichnen          │  │
│  │  [☑] Aktuelle Stunde hervorheben           │  │
│  │  [☐] Römische Ziffern verwenden            │  │
│  │                                             │  │
│  └─────────────────────────────────────────────┘  │
│                                                   │
│   ┌──────────────────┐     ┌──────────────────┐   │
│   │       OK         │     │    Abbrechen     │   │
│   └──────────────────┘     └──────────────────┘   │
│                                                   │
└───────────────────────────────────────────────────┘
  420 px Breite × 460 px Höhe
```

### 2.3 Steuerelemente – Beschreibung

| Steuerelement | Typ | Bindung | Beschreibung |
|---------------|-----|---------|--------------|
| **Hintergrundfarbe (Vorschau)** | Panel | `WallpaperConfig.BackgroundColorArgb` | Zeigt die aktuell gewählte Farbe als Farbfläche an |
| **Hintergrundfarbe (Button)** | Button | Öffnet `ColorDialog` | Öffnet den Windows-Farbauswahldialog |
| **Primärfarbe** | Panel + Button | `WallpaperConfig.PrimaryColorArgb` | Wie Hintergrundfarbe |
| **Sekundärfarbe** | Panel + Button | `WallpaperConfig.SecondaryColorArgb` | Wie Hintergrundfarbe |
| **Akzentfarbe** | Panel + Button | `WallpaperConfig.AccentColorArgb` | Wie Hintergrundfarbe |
| **Schriftart** | ComboBox | `WallpaperConfig.FontFamily` | Dropdown mit installierten Systemschriften |
| **Schriftgröße** | NumericUpDown | `WallpaperConfig.FontSizeBase` | Wertebereich: 6–72 Punkt |
| **Checkboxen** | CheckBox (×6) | `WallpaperConfig.*` | Boolesche Anzeigeoptionen |
| **OK** | Button | `DialogResult.OK` | Übernimmt alle Änderungen |
| **Abbrechen** | Button | `DialogResult.Cancel` | Verwirft alle Änderungen |

---

## 3. Generiertes Wallpaper (1920×1080)

### 3.1 Beschreibung

Das generierte Wallpaper-Bild hat eine Auflösung von **1920 × 1080 Pixeln** (Full HD) und zeigt eine stilisierte astronomische Sonnenuhr auf einem dunklen Hintergrund.

### 3.2 Visuelles Layout

```
┌──────────────────────────────────────────────────────────────────────────────────────┐
│                                                                                      │
│  ┌──────────────────────────────┐                                                    │
│  │ 📍 Mannheim                  │              [Dunkler Gradient-Hintergrund]         │
│  │    49.49°N  8.47°E           │              [Dunkles Marineblau → Schwarz]         │
│  │                              │                                                    │
│  │ 🌅  05:18 MESZ               │                                                    │
│  │ 🌞  13:30 MESZ               │                                                    │
│  │ 🌇  21:42 MESZ               │                                                    │
│  │                              │                                                    │
│  │ ⏰  09:47 Uhr                │                                                    │
│  └──────────────────────────────┘                                                    │
│                                                                                      │
│                                                                                      │
│                          ┌──── Sonnenuhr-Zifferblatt ─────┐                          │
│                          │                                 │                         │
│                          │            12                   │                         │
│                          │         XI /  \ I               │                         │
│                          │        X  /    \  II            │                         │
│                          │        IX ──────── III          │                         │
│                          │        VIII\    / IV            │                         │
│                          │         VII \  / V              │                         │
│                          │             VI                  │                         │
│                          │          ↑ Gnomon               │                         │
│                          │       [Goldener Zeiger]         │                         │
│                          │    [Orange Schattenzeiger       │                         │
│                          │     zeigt aktuelle Stunde]      │                         │
│                          │                                 │                         │
│                          └─────────────────────────────────┘                         │
│                                                                                      │
│                                                                                      │
│  ─────────────────── Sonnenaufgang (05:18) ──────────────────────────────────────── │
│  ════════════════════════════════════════════════════════════════════════════════════│
│  ─────────────────── Sonnenuntergang (21:42) ────────────────────────────────────── │
│                                                                                      │
└──────────────────────────────────────────────────────────────────────────────────────┘
                                   1920 × 1080 Pixel
```

### 3.3 Farbschema (Standard)

| Element | Farbe | Hex-Wert | Beschreibung |
|---------|-------|----------|--------------|
| **Hintergrund** | Dunkles Marineblau | `#1A1A2E` | Tiefer, dunkler Hintergrund für gute Lesbarkeit |
| **Hintergrund-Gradient** | Sehr dunkles Blau | `#16213E` → `#0F3460` | Leichter radialer Gradient von Mitte nach außen |
| **Primärfarbe** | Gold | `#FFD700` | Zifferblatt-Umriss, Stundenlinien, Beschriftungen |
| **Sekundärfarbe** | Weiß | `#FFFFFF` | Hilfstexte, kleinere Beschriftungen |
| **Akzentfarbe** | Orange | `#FF8C00` | Aktueller Schattenzeiger, hervorgehobene Stunde |
| **Gnomon** | Hellgold | `#FFF0A0` | Zeigerdarstellung mit leichtem Glüheffekt |
| **Dämmerungslinien** | Gedämpftes Blau | `#4A6FA5` (50% Alpha) | Markierungslinien für bürgerliche Dämmerung |

### 3.4 Sonnenuhr-Geometrie

Die Sonnenuhr wird als **horizontale Sonnenuhr** berechnet. Die Stundenlinienwinkel werden nach folgender Formel ermittelt:

```
H = arctan(sin(φ) × tan(t))
```

Dabei gilt:
- `φ` = geografischer Breitengrad in Radiant
- `t` = Stundenwinkel (15° × Stundenoffset vom Sonnenmittag)
- `H` = Stundenlinienwinkel zur Mittagslinie

Die Stundenlinien werden für die Stunden 6 bis 18 Uhr (Offset −6 bis +6 vom Sonnenmittag) berechnet und vom Mittelpunkt des Zifferblatts nach außen gezogen.

### 3.5 Layout-Zonen

```
┌─────────────────────────────────────────────────────┐
│ Zone A: Info-Panel (links oben)                     │
│ Position: x=30, y=30, Breite=280, Höhe=200         │
│ Inhalt: Ortsname, Koordinaten, Sonnenzeiten, Uhrzeit│
├─────────────────────────────────────────────────────┤
│ Zone B: Sonnenuhr-Zifferblatt (Mitte)               │
│ Mittelpunkt: x=960, y=540                           │
│ Radius Zifferblatt: 380 px                          │
│ Radius Stundenlinien: 320 px                        │
│ Gnomon-Höhe: 60 px                                  │
├─────────────────────────────────────────────────────┤
│ Zone C: Tageslängen-Balken (unten)                  │
│ Position: y=980 bis y=1060                          │
│ Inhalt: Optionale horizontale Linien für Aufgang,   │
│         Untergang und Dämmerung                     │
└─────────────────────────────────────────────────────┘
```

---

## 4. Systemtray-Kontextmenü

### 4.1 Beschreibung

Wenn die Anwendung in den Systemtray minimiert wird, erscheint ein Sonnensymbol (☀) in der Windows-Taskleiste. Ein Rechtsklick auf dieses Symbol öffnet ein Kontextmenü.

### 4.2 Wireframe

```
┌─────────────────────────────────────┐
│ ☀ Sonnenuhr                        │
├─────────────────────────────────────┤
│  🔍  Öffnen                         │
│  ⟳   Jetzt aktualisieren            │
├─────────────────────────────────────┤
│  ⚙   Darstellungsoptionen...        │
├─────────────────────────────────────┤
│  ✖   Beenden                        │
└─────────────────────────────────────┘
```

### 4.3 Menüeinträge

| Eintrag | Aktion | Shortcut |
|---------|--------|----------|
| **Öffnen** | Hauptfenster wieder in den Vordergrund bringen | Doppelklick auf Tray-Icon |
| **Jetzt aktualisieren** | Sofortigen Wallpaper-Aktualisierungszyklus starten | — |
| **Darstellungsoptionen** | Konfigurationsdialog öffnen | — |
| **Beenden** | Anwendung vollständig beenden | — |

---

## 5. Designprinzipien

Die Benutzeroberfläche der Sonnenuhr-Anwendung folgt folgenden Designprinzipien:

| Prinzip | Umsetzung |
|---------|-----------|
| **Konsistenz** | Einheitliche Schriftarten, Abstände und Farben in allen Dialogen (Windows-Standard: Segoe UI 9pt) |
| **Feedback** | Jede Benutzeraktion erhält sofortiges visuelles Feedback (Statusbar, deaktivierte Schaltflächen während Ladevorgängen) |
| **Einfachheit** | Alle notwendigen Funktionen auf einem einzigen Hauptfenster; keine tiefen Menühierarchien |
| **Fehlervermeidung** | Eingabefelder mit definierten Wertebereichen (NumericUpDown statt TextBox); Validierung vor dem Speichern |
| **Barrierefreiheit** | Tooltips auf allen Steuerelementen; Tastaturnavigation vollständig unterstützt |

---

*Dokument erstellt von: Uwe Markus Münch | Breihof IT GmbH | IHK Rhein-Neckar | 01.07.2026*
