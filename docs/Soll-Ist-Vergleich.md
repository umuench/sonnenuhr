# Soll-Ist-Vergleich

**Sonnenuhr – Standortspezifischer Wallpaper-Generator**
**IHK-Abschlussprojekt · Fachinformatiker Anwendungsentwicklung**

---

| | |
|---|---|
| **Prüfling** | Uwe Markus Münch (Nr. 10350411) |
| **Ausbildungsbetrieb** | Breihof IT GmbH |
| **IHK-Bezirk** | Rhein-Neckar |
| **Projektzeitraum** | 01.07.2026 – 31.08.2026 |
| **Bearbeiter** | Uwe Markus Münch |
| **Datum** | August 2026 |

---

## 1. Einleitung

Dieses Dokument stellt die im Projektantrag definierten Soll-Ziele den tatsächlich
erzielten Projektergebnissen gegenüber. Ziel ist eine objektive Bewertung des
Projekterfolgs und eine Reflexion über Abweichungen.

---

## 2. Soll-Ist-Vergleich der Projektziele

### 2.1 Funktionale Ziele

| Nr. | Geplantes Ziel (SOLL) | Tatsächliches Ergebnis (IST) | Abweichung | Begründung |
|---|---|---|---|---|
| F-01 | C# Windows Forms Desktop-Anwendung für Windows 11 | ✅ Vollständig implementiert | Keine | Realisiert mit .NET 10, Windows Forms, vollständiger UI |
| F-02 | .NET 10 Framework | ✅ .NET 10.0.103 verwendet | Keine | Aktuellste .NET 10 SDK-Version eingesetzt |
| F-03 | REST-API-Anbindung für astronomische Sonnenstandsdaten | ✅ Vollständig implementiert | Keine | `SolarApiService` mit HttpClient, `api.sunrise-sunset.org` |
| F-04 | Sonnenuhr-Berechnungslogik (Stundenlinien-Winkel) | ✅ Vollständig implementiert | Keine | `SundialCalculator` mit Formel `tan(H) = sin(φ)·tan(t)` |
| F-05 | Grafische Wallpaper-Generierung | ✅ Vollständig implementiert | Keine | `WallpaperGeneratorService` mit System.Drawing, Antialiasing |
| F-06 | Konfigurierbare Benutzeroberfläche (Farben, Schriften) | ✅ Vollständig implementiert | Keine | `ConfigurationForm` mit Farbauswahl, Schriftart, 6 Optionen |
| F-07 | JSON-Konfigurationsdateien für Benutzereinstellungen | ✅ Vollständig implementiert | Keine | `ConfigurationService`, `%APPDATA%\Sonnenuhr\settings.json` |
| F-08 | Windows Registry Integration für Wallpaper-Wechsel | ✅ Vollständig implementiert | Keine | `WallpaperRegistryService` mit `SystemParametersInfo` |
| F-09 | Automatische Wallpaper-Aktualisierung (Timer) | ✅ Vollständig implementiert | Keine | Konfigurierbares Intervall 1–1440 Minuten |
| F-10 | **System-Tray-Integration** | ✅ Implementiert (Mehrwert) | Positiv | Nicht im ursprünglichen Plan – erhöht Benutzerfreundlichkeit erheblich |
| F-11 | Autostart mit Windows | ✅ Vollständig implementiert | Keine | `HKCU\...\Run`-Registry-Eintrag, togglebar in UI |
| F-12 | **Stadtsuche mit automatischer Koordinatenübernahme** | ✅ Implementiert (Mehrwert) | Positiv | Nominatim API (OpenStreetMap) – erleichtert Standortkonfiguration erheblich; kein API-Key erforderlich |

### 2.2 Qualitative Ziele

| Nr. | Geplantes Ziel (SOLL) | Tatsächliches Ergebnis (IST) | Abweichung | Begründung |
|---|---|---|---|---|
| Q-01 | EVA-Prinzip Kommentierung | ✅ Vollständig implementiert | Keine | Alle Methoden mit EINGABE/VERARBEITUNG/AUSGABE-Kommentaren |
| Q-02 | XML-Dokumentationskommentare | ✅ Vollständig implementiert | Keine | Alle public Members mit `<summary>` und `<param>` dokumentiert |
| Q-03 | Nullable Reference Types | ✅ Aktiviert und eingehalten | Keine | `<Nullable>enable</Nullable>` im Projekt |
| Q-04 | Exception Handling | ✅ Vollständig implementiert | Keine | HttpRequestException, ArgumentNullException, JsonException abgefangen |
| Q-05 | Eingabevalidierung | ✅ Vollständig implementiert | Keine | `Location.IsValid()`, `ArgumentNullException.ThrowIfNull()` |
| Q-06 | Ressourcen-Management | ✅ Vollständig implementiert | Keine | IDisposable für HttpClient, `using`-Statements für GDI-Objekte |
| Q-07 | Async/Await Pattern | ✅ Vollständig implementiert | Keine | API-Aufrufe vollständig asynchron |
| Q-08 | Windows 11 Design-Guidelines | ✅ Weitgehend eingehalten | Geringfügig | Dunkles Custom-Theme statt Standard-Windows-UI (bewusste Designentscheidung) |
| Q-09 | Unit-Tests | ✅ 24 Tests, alle bestanden | Keine | SundialCalculatorTests (18), LocationTests (6) |

### 2.3 Technische Ziele

| Nr. | Geplantes Ziel (SOLL) | Tatsächliches Ergebnis (IST) | Abweichung | Begründung |
|---|---|---|---|---|
| T-01 | Visual Studio 2022 als IDE | ✅ Verwendet | Keine | Entwicklungsumgebung der Breihof IT GmbH |
| T-02 | C# als Programmiersprache | ✅ C# 14 (.NET 10) | Keine | Modernste C#-Version genutzt |
| T-03 | System.Drawing für Bildgenerierung | ✅ Vollständig implementiert | Keine | Antialiasing, Gradient-Effekte, PathGradientBrush |
| T-04 | Git Versionsverwaltung | ✅ Implementiert | Keine | GitHub-Repository `umuench/sonnenuhr` mit vollständiger History |
| T-05 | Lokales Git-Repository | ✅ Implementiert | Keine | Erweiterung: öffentliches GitHub-Repository mit README |

### 2.4 Dokumentationsziele

| Nr. | Geplantes Dokument (SOLL) | Tatsächliches Ergebnis (IST) | Status |
|---|---|---|---|
| D-01 | Projektdokumentation | ✅ README.md + vollständige Docs | ✅ |
| D-02 | Pflichtenheft | ✅ `docs/Pflichtenheft.md` | ✅ |
| D-03 | ER-Diagramm | ✅ `docs/ER-Diagramm.md` (Mermaid) | ✅ |
| D-04 | Klassendiagramm (UML) | ✅ `docs/Klassendiagramm.md` (Mermaid) | ✅ |
| D-05 | Aktivitätsdiagramm | ✅ `docs/Aktivitaetsdiagramm.md` (Mermaid) | ✅ |
| D-06 | Sequenzdiagramm | ✅ `docs/Sequenzdiagramm.md` (Mermaid) | ✅ |
| D-07 | Mockups / Wireframes | ✅ `docs/Mockups.md` (ASCII-Art) | ✅ |
| D-08 | Testprotokoll | ✅ `docs/Testprotokoll.md` | ✅ |
| D-09 | Benutzerhandbuch | ✅ `docs/Benutzerhandbuch.md` | ✅ |
| D-10 | Installationsanleitung | ✅ `docs/Installationsanleitung.md` | ✅ |
| D-11 | Soll-Ist-Vergleich | ✅ `docs/Soll-Ist-Vergleich.md` | ✅ |

---

## 3. Zeitplan-Vergleich

| Phase | Geplant (Std.) | Tatsächlich (Std.) | Differenz |
|---|---|---|---|
| Analysephase | 10 h | ~9 h | −1 h |
| Entwurfsphase | 12 h | ~11 h | −1 h |
| Implementierungsphase | 30 h | ~31 h | +1 h |
| Testphase | 12 h | ~12 h | 0 h |
| Abschlussphase | 5 h | ~5 h | 0 h |
| Dokumentation | 11 h | ~12 h | +1 h |
| **Gesamt** | **80 h** | **~80 h** | **0 h** |

> **Anmerkung:** Die zusätzliche Stunde in der Implementierungsphase entstand durch die
> ungeplante, aber wertvolle System-Tray-Integration. Diese wurde durch die Zeitersparnis
> in Analyse und Entwurf kompensiert.

---

## 4. Budget-Vergleich

| Position | Geplant | Tatsächlich | Differenz |
|---|---|---|---|
| Entwicklungsstunden | 80 h × 80 €/h = 6.400 € | 80 h × 80 €/h = 6.400 € | 0 € |
| Externe Abhängigkeiten | 0 € | 0 € (OSS/kostenlose API) | 0 € |
| **Gesamtbudget** | **6.400 €** | **6.400 €** | **0 €** |

Das Projektbudget wurde exakt eingehalten.

---

## 5. Fazit und Reflexion

### 5.1 Projekterfolg

Das Projekt **Sonnenuhr – Standortspezifischer Wallpaper-Generator** wurde innerhalb des
geplanten Zeit- und Budgetrahmens von 80 Stunden und 6.400 Euro erfolgreich abgeschlossen.

Alle im Projektantrag definierten **funktionalen, qualitativen und technischen Ziele**
wurden vollständig erreicht. Darüber hinaus wurde die Anwendung durch eine
**System-Tray-Integration** erweitert, die die Benutzerfreundlichkeit erheblich steigert.

### 5.2 Technische Erkenntnisse

Die Implementierung der astronomischen Berechnungsformel für horizontale Sonnenuhren
(`tan(H) = sin(φ) · tan(t)`) war eine fachliche Herausforderung, die gründliche Recherche
erforderte. Die Umsetzung mit .NET 10 und System.Drawing ermöglichte eine ressourcen-
schonende und performante Bildgenerierung.

Die strikte Anwendung des **EVA-Prinzips** (Eingabe–Verarbeitung–Ausgabe) in allen Methoden
verbesserte die Lesbarkeit und Wartbarkeit des Codes erheblich und erleichterte die
Fehlersuche während der Testphase.

### 5.3 Prozessreflexion

Die **agile Vorgehensweise** mit kontinuierlicher Qualitätssicherung durch Unit-Tests
(24 Tests, alle bestanden) hat sich bewährt. Fehler wurden frühzeitig erkannt und
konnten ohne große Rückwirkungen auf den Zeitplan behoben werden.

### 5.4 Verbesserungspotenzial

Für zukünftige Versionen der Anwendung sind folgende Erweiterungen denkbar:

| Erweiterung | Beschreibung | Priorität |
|---|---|---|
| Mehrsprachigkeit | Englische UI-Lokalisierung | Mittel |
| Mehrmonitor-Unterstützung | Individuelle Wallpaper pro Monitor | Hoch |
| Offline-Modus | Fallback auf lokal gecachte Sonnendaten | Mittel |
| Weltzeit-Anzeige | Mehrere Standorte gleichzeitig auf dem Wallpaper | Niedrig |
| MSI-Installer | Professionelles Setup-Paket | Mittel |

---

*© 2026 Breihof IT GmbH – Uwe Markus Münch*
