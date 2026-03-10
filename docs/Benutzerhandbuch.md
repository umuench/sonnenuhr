# Benutzerhandbuch

## Sonnenuhr – Standortspezifischer Wallpaper-Generator für Windows 11

---

| Feld | Inhalt |
|------|--------|
| **Produktname** | Sonnenuhr – Standortspezifischer Wallpaper-Generator |
| **Version** | 1.0.0 |
| **Entwickler** | Uwe Markus Münch / Breihof IT GmbH |
| **Stand** | 01.07.2026 |

---

## Inhaltsverzeichnis

1. [Einführung](#1-einführung)
2. [Systemvoraussetzungen](#2-systemvoraussetzungen)
3. [Installation](#3-installation)
4. [Erste Schritte](#4-erste-schritte)
5. [Benutzeroberfläche](#5-benutzeroberfläche)
6. [Animierter Hintergrund](#6-animierter-hintergrund)
7. [Konfiguration](#7-konfiguration)
8. [Automatische Aktualisierung](#8-automatische-aktualisierung)
9. [Fehlerbehebung](#9-fehlerbehebung)
10. [Technische Details](#10-technische-details)
11. [Häufig gestellte Fragen (FAQ)](#11-häufig-gestellte-fragen-faq)

---

## 1. Einführung

### 1.1 Was ist Sonnenuhr?

**Sonnenuhr** ist eine Windows-Desktopanwendung, die Ihren Desktop-Hintergrund in eine animierte, standortspezifische Sonnenuhr verwandelt. Die Anwendung berechnet auf Basis Ihres geografischen Standorts und der tagesaktuellen astronomischen Daten eine geometrisch korrekte Sonnenuhr und setzt diese automatisch als Windows-Wallpaper.

Das Konzept der Sonnenuhr basiert auf der historischen Methode der Zeitmessung durch den Stand der Sonne: Ein senkrecht aufgestellter Stab (Gnomon) wirft einen Schatten, dessen Position die Tageszeit anzeigt. Die genaue Form der Sonnenuhr – insbesondere der Winkel der Stundenlinien – ist dabei standortabhängig und wird durch den geografischen Breitengrad bestimmt.

### 1.2 Was kann Sonnenuhr?

- ☀ **Standortspezifische Sonnenuhr** auf Ihrem Desktop generieren
- 🌅 **Aktuelle Sonnendaten** (Sonnenaufgang, -mittag, -untergang) von einer öffentlichen API abrufen
- 🎨 **Vollständige visuelle Anpassung** (Farben, Schriften, Anzeigeoptionen)
- ⏱ **Automatische Aktualisierung** im konfigurierbaren Intervall
- 🚀 **Autostart** mit Windows
- 📊 **Aktuelle Informationen** direkt auf dem Wallpaper (Uhrzeit, Ortsname, Sonnenzeiten)
- 🔍 **Stadtsuche** mit automatischer Koordinatenübernahme (OpenStreetMap Nominatim)

### 1.3 Für wen ist Sonnenuhr geeignet?

Die Anwendung eignet sich für alle Windows 11-Benutzer, die:
- Ihren Desktop mit einem informativen, ästhetischen Hintergrund gestalten möchten
- Interesse an Astronomie und klassischer Zeitmessung haben
- Ein ruhiges, dunkel gestaltetes Arbeitsumfeld bevorzugen

---

## 2. Systemvoraussetzungen

Stellen Sie sicher, dass Ihr System die folgenden Mindestanforderungen erfüllt, bevor Sie Sonnenuhr installieren:

### 2.1 Pflichtanforderungen

| Komponente | Mindestanforderung | Empfehlung |
|------------|-------------------|------------|
| **Betriebssystem** | Windows 11 (Version 22H2 oder neuer) | Windows 11 23H2 oder neuer |
| **.NET Runtime** | .NET 10.0 Runtime (Windows) | Aktuellste .NET 10-Version |
| **Prozessor** | 1,5 GHz, Dual-Core | 2,5 GHz, Quad-Core oder besser |
| **Arbeitsspeicher** | 2 GB RAM | 8 GB RAM oder mehr |
| **Festplattenspeicher** | 50 MB (für Anwendung und Daten) | 100 MB |
| **Bildschirmauflösung** | 1024×768 | 1920×1080 (Full HD) |
| **Internetverbindung** | Erforderlich für API-Abrufe | Stabile Breitbandverbindung |

> ⚠️ **Wichtig:** Windows 10 und ältere Betriebssysteme werden **nicht** unterstützt.

### 2.2 Optional

- **Systemtray-Unterstützung:** Wird von Windows 11 standardmäßig bereitgestellt
- **Administratorrechte:** Nicht erforderlich; die Anwendung läuft vollständig im Benutzerkontext

---

## 3. Installation

Eine detaillierte Schritt-für-Schritt-Installationsanleitung finden Sie im separaten Dokument **[Installationsanleitung.md](Installationsanleitung.md)**.

### 3.1 Kurzübersicht

1. Laden Sie die aktuelle Version von der GitHub-Releases-Seite herunter
2. Installieren Sie die .NET 10 Runtime (falls noch nicht vorhanden)
3. Entpacken Sie die Sonnenuhr-ZIP-Datei in ein Verzeichnis Ihrer Wahl
4. Starten Sie `Sonnenuhr.exe`

---

## 4. Erste Schritte

### 4.1 Anwendung starten

1. Navigieren Sie zum Installationsverzeichnis (z.B. `C:\Programme\Sonnenuhr\`)
2. Doppelklicken Sie auf `Sonnenuhr.exe`
3. Beim ersten Start erscheint das Hauptfenster mit Standardeinstellungen (Standort: Mannheim)

### 4.2 Standort einrichten

Damit Sonnenuhr eine korrekte Sonnenuhr für Ihren Standort erstellen kann, müssen Sie Ihren Standort konfigurieren:

**Schritt 1:** Geben Sie einen Stadtnamen in das Feld **„Ortsname"** ein (z.B. „Eberbach" oder „Johannesburg") und klicken Sie auf **„🔍 Suchen"**.

Die Anwendung durchsucht die OpenStreetMap-Datenbank und:
- findet **genau einen Ort** → Koordinaten werden sofort übernommen
- findet **mehrere Orte** mit dem gleichen Namen → ein Auswahldialog öffnet sich

> 💡 **Tipp:** Wenn die Suche zu viele irrelevante Ergebnisse liefert, verwenden Sie einen Länderzusatz:  
> `Eberbach, Deutschland` oder `Johannesburg, Südafrika`

**Alternativ – Manuelle Eingabe:**

Wenn Sie die Koordinaten bereits kennen, können Sie Breitengrad und Längengrad auch direkt in die entsprechenden Felder eingeben:
- **Breitengrad:** Dezimalgrad, z.B. `49.4875` (Nord positiv)
- **Längengrad:** Dezimalgrad, z.B. `8.9967` (Ost positiv)

> ⚠️ **Wichtig:** Verwenden Sie immer einen **Punkt** (`.`) als Dezimaltrennzeichen, kein Komma.

**Schritt 2:** Klicken Sie auf **„⟳ Jetzt aktualisieren"**, um sofort ein Wallpaper für den neuen Standort zu generieren.

### 4.3 Erstes Wallpaper generieren

Nach dem Klick auf „Jetzt aktualisieren":

1. Die Statusbar zeigt „Abruf läuft..." an
2. Die Schaltfläche wird während des Abrufs deaktiviert
3. Nach erfolgreichem Abschluss werden die Sonnendaten im Bereich „Aktuelle Sonnenzeitdaten" angezeigt
4. Der Desktop-Hintergrund wird automatisch auf das neue Sonnenuhr-Wallpaper gesetzt
5. Die Statusbar zeigt den Zeitpunkt der letzten Aktualisierung an

### 4.4 Koordinaten für deutsche Großstädte (Referenz)

| Stadt | Breitengrad | Längengrad |
|-------|-------------|------------|
| Berlin | 52.5200 | 13.4050 |
| Hamburg | 53.5753 | 10.0153 |
| München | 48.1351 | 11.5820 |
| Köln | 50.9333 | 6.9500 |
| Frankfurt am Main | 50.1109 | 8.6821 |
| Stuttgart | 48.7758 | 9.1829 |
| Düsseldorf | 51.2217 | 6.7762 |
| Mannheim | 49.4875 | 8.4660 |
| Heidelberg | 49.3988 | 8.6724 |
| Karlsruhe | 49.0069 | 8.4037 |

---

## 5. Benutzeroberfläche

### 5.1 Hauptfenster

Das Hauptfenster ist in drei Bereiche unterteilt:

#### Bereich „Standort-Konfiguration"

| Steuerelement | Beschreibung | Eingabeformat |
|---------------|--------------|---------------|
| **Ortsname** | Frei wählbarer Name für den Standort | Text, max. 100 Zeichen |
| **🔍 Suchen** | Startet die Stadtsuche über OpenStreetMap Nominatim API | — |
| **Breitengrad** | Geografischer Breitengrad | Dezimalzahl, −90° bis +90° |
| **Längengrad** | Geografischer Längengrad | Dezimalzahl, −180° bis +180° |
| **Zeitzone** | Windows-Zeitzone für korrekte Zeitumrechnung | Dropdown-Liste |

#### Bereich „Aktuelle Sonnenzeitdaten"

Dieser Bereich zeigt nach einem erfolgreichen API-Abruf die folgenden Informationen an:

| Anzeige | Beschreibung |
|---------|--------------|
| **Datum** | Das aktuelle Datum |
| **Sonnenaufgang** | Zeitpunkt des Sonnenaufgangs in lokaler Zeit |
| **Sonnenmittag** | Zeitpunkt des astronomischen Sonnenmittags in lokaler Zeit |
| **Sonnenuntergang** | Zeitpunkt des Sonnenuntergangs in lokaler Zeit |
| **Tageslänge** | Dauer des Tages in Stunden und Minuten |
| **Bürgerliche Dämmerung** | Begin und Ende der bürgerlichen Dämmerung in lokaler Zeit |

#### Bereich „Einstellungen"

| Steuerelement | Beschreibung | Standardwert |
|---------------|--------------|--------------|
| **Aktualisierungsintervall** | Zeitabstand zwischen automatischen Aktualisierungen | 60 Minuten |
| **Autostart-Checkbox** | Startet Sonnenuhr automatisch mit Windows | Deaktiviert |

#### Schaltflächen

| Schaltfläche | Funktion |
|--------------|----------|
| **⟳ Jetzt aktualisieren** | Löst sofort einen Abruf der Sonnendaten und eine Wallpaper-Aktualisierung aus |
| **⚙ Darstellung** | Öffnet den Konfigurationsdialog für visuelle Einstellungen |

#### Statusbar

Die Statusbar am unteren Rand des Fensters zeigt permanent den Zeitpunkt der letzten erfolgreichen Aktualisierung an. Bei Fehlern wird eine Fehlermeldung angezeigt.

### 5.2 Systemtray-Icon

Wenn Sie das Hauptfenster minimieren oder schließen, wird die Anwendung in den Windows-Systemtray (Benachrichtigungsbereich der Taskleiste) verschoben. Das Sonnensymbol (☀) erscheint im Systemtray.

**Interaktionen mit dem Tray-Icon:**

| Aktion | Ergebnis |
|--------|----------|
| Doppelklick auf Tray-Icon | Hauptfenster wird wieder angezeigt |
| Rechtsklick → „Öffnen" | Hauptfenster wird angezeigt |
| Rechtsklick → „Jetzt aktualisieren" | Sofortige Wallpaper-Aktualisierung |
| Rechtsklick → „Darstellungsoptionen" | Konfigurationsdialog öffnet sich |
| Rechtsklick → „Beenden" | Anwendung wird vollständig beendet |

---

## 6. Animierter Hintergrund

### 6.1 Überblick

Sonnenuhr generiert einen vollständig animierten Hintergrund, der sich automatisch nach der aktuellen Tageszeit und dem konfigurierten Standort richtet. Der Hintergrund wird bei jeder Wallpaper-Aktualisierung neu berechnet – der Benutzer muss hierfür keine zusätzlichen Einstellungen vornehmen.

> 💡 **Tipp:** Stellen Sie das Aktualisierungsintervall auf 15–30 Minuten (Abschnitt 8.1), um den Übergang von Tag zu Nacht besonders flüssig zu erleben.

### 6.2 Himmelsfarben und Dämmerungsstufen

Die Hintergrundfarbe des Wallpapers interpoliert automatisch zwischen sieben astronomisch definierten Farbzuständen:

| Tagesabschnitt | Typische Uhrzeit (Mitteleuropa Sommer) | Erscheinungsbild |
|---|---|---|
| **Tiefe Nacht** | 0:00 – 03:00 Uhr | Sehr dunkles Dunkelblau |
| **Astronomische Dämmerung** | ca. 90 min vor Aufgang | Dunkles Blauviolett |
| **Nautische Dämmerung** | ca. 60 min vor Aufgang | Tiefes Blau |
| **Bürgerliche Dämmerung** | ca. 30 min vor Aufgang | Blaugrau |
| **Sonnenaufgang / -untergang** | exakt Auf-/Untergangszeit | Warme Orange- und Rottöne |
| **Goldene Stunde** | ca. 30–60 min nach Aufgang | Goldgelb bis warmes Blau |
| **Tageshimmel** | Tagsüber | Klares Himmelblau |

### 6.3 Sterne

Bei Nacht und in den Dämmerungsstunden erscheint ein Sternenhimmel mit 180 deterministisch platzierten Sternen:

- Die Positionen der Sterne sind für jeden Tag identisch (deterministischer Zufallsgenerator, Seed = aktuelles Datum).
- Die Sterne blenden sich sanft ein, sobald die Sonne 60 Minuten nach dem Untergang unter den Horizont gesunken ist.
- Ein subtiles, stündlich wechselndes Flackern einzelner Sterne verstärkt den Eindruck eines lebendigen Nachthimmels.
- Vor Sonnenaufgang blenden die Sterne sich entsprechend aus.

### 6.4 Sonnenauf- und -untergang: Horizontglühen

Innerhalb von ±60 Minuten um den Sonnenauf- oder -untergang erscheint ein orangefarbener Glow am Horizont. Die Intensität des Leuchtens ist proportional zur Nähe des Ereignisses – sie ist am stärksten exakt zum Zeitpunkt des Auf- oder Untergangs.

### 6.5 Sonne und Mond auf der Bogenbahn

Sonne und Mond bewegen sich auf einer Sinusbogen-Trajektorie über den Bildhintergrund:

- Die **Sonne** ist tagsüber sichtbar. Sie erscheint links am Horizont beim Aufgang (8 % der Bildbreite), erreicht den Höhepunkt (Zenit, 8 % der Bildhöhe) zur Mittagszeit und sinkt rechts zum Untergang (92 % der Bildbreite).
- Der **Mond** ist nachts auf derselben Bahn sichtbar. Sein Fortschritt wird von Sonnenuntergang bis Sonnenaufgang berechnet.
- Sonne und Mond blenden sich jeweils in einem Zeitfenster von ±20 Minuten nahe dem Horizont weich ein und aus, um einen fließenden Übergang zwischen Tag und Nacht zu erzeugen.
- Sonne und Mond werden mit mehrschichtigen Glow-Effekten dargestellt (Halo, Leuchtscheibe, Kern).

### 6.6 Mondphasen

Die aktuelle Mondphase wird exakt auf Basis des synodischen Monats (29,53 Tage, Referenz: Neumond 6. Januar 2000) berechnet und visuell dargestellt:

| Mondphase | Erscheinungsbild |
|---|---|
| **Neumond** | Mondscheibe nicht sichtbar |
| **Zunehmender Halbmond** | Rechte Hälfte beleuchtet, linke Hälfte im Schatten |
| **Vollmond** | Vollständig beleuchtete Scheibe |
| **Abnehmender Halbmond** | Linke Hälfte beleuchtet, rechte Hälfte im Schatten |

Der Terminator (die Grenzlinie zwischen beleuchtetem und unbeleuchtetem Teil) wird als sanft gerenderte Kurve dargestellt.

---

## 7. Konfiguration

### 6.1 Farbschema anpassen

Klicken Sie auf **„⚙ Darstellung"** um den Konfigurationsdialog zu öffnen.

Im Abschnitt **„Farbschema"** können Sie vier Farben individuell anpassen:

| Farbe | Verwendung | Standardwert |
|-------|-----------|--------------|
| **Hintergrundfarbe** | Hintergrund des Wallpapers | Dunkles Marineblau (`#1A1A2E`) |
| **Primärfarbe** | Zifferblatt-Umriss, Stundenlinien, Hauptbeschriftungen | Gold (`#FFD700`) |
| **Sekundärfarbe** | Hilfstexte, kleinere Beschriftungen | Weiß (`#FFFFFF`) |
| **Akzentfarbe** | Aktueller Schattenzeiger, hervorgehobene Stunde | Orange (`#FF8C00`) |

**Farbe ändern:**
1. Klicken Sie auf die Farbvorschau-Fläche neben dem Farbnamen
2. Der Windows-Farbauswahldialog öffnet sich
3. Wählen Sie die gewünschte Farbe aus
4. Bestätigen Sie mit „OK"

### 6.2 Schrift anpassen

Im Abschnitt **„Schrift"** können Sie Schriftart und Schriftgröße konfigurieren:

| Einstellung | Beschreibung | Standard |
|-------------|--------------|---------|
| **Schriftart** | Schriftfamilie für alle Texte auf dem Wallpaper | Segoe UI |
| **Schriftgröße** | Basisschriftgröße in Punkt | 14 pt |

> 💡 **Empfehlung:** Für eine gute Lesbarkeit auf einem Full-HD-Wallpaper eignen sich serifenlose Schriften wie „Segoe UI", „Arial" oder „Calibri" besonders gut.

### 6.3 Anzeigeoptionen

Im Abschnitt **„Anzeigeoptionen"** können Sie konfigurieren, welche Informationen auf dem Wallpaper angezeigt werden:

| Option | Beschreibung | Standard |
|--------|--------------|---------|
| **Aktuelle Uhrzeit anzeigen** | Zeigt die aktuelle Systemzeit auf dem Wallpaper | Aktiviert |
| **Ortsname anzeigen** | Zeigt den konfigurierten Ortsnamen auf dem Wallpaper | Aktiviert |
| **Sonnenaufgang / -untergang anzeigen** | Zeigt die Zeiten für Sonnenaufgang und -untergang | Aktiviert |
| **Dämmerungslinien einzeichnen** | Zeichnet Hilfslinien für die bürgerliche Dämmerung | Deaktiviert |
| **Aktuelle Stunde hervorheben** | Hebt die aktuelle Stunde auf dem Zifferblatt hervor | Aktiviert |
| **Römische Ziffern verwenden** | Stellt Stundenmarkierungen als römische Ziffern dar | Deaktiviert |

---

## 8. Automatische Aktualisierung

### 7.1 Timer-Einstellung

Sonnenuhr aktualisiert das Wallpaper automatisch in einem konfigurierbaren Zeitintervall. Das Intervall legen Sie im Bereich „Einstellungen" des Hauptfensters fest.

| Einstellung | Beschreibung | Wertebereich | Standard |
|-------------|--------------|-------------|---------|
| **Aktualisierung alle X Minuten** | Zeitabstand zwischen automatischen Aktualisierungen | 1 – 1440 Minuten | 60 Minuten |

**Empfehlungen für das Intervall:**

| Anwendungsfall | Empfohlenes Intervall |
|----------------|----------------------|
| Normalgebrauch (tägliche Aktualisierung reicht) | 60 – 120 Minuten |
| Live-Schattenzeiger soll aktuell sein | 15 – 30 Minuten |
| Maximale Aktualität | 5 – 15 Minuten |
| Datensparmodus (selten online) | 240 – 1440 Minuten |

> ⚠️ **Hinweis:** Sehr kurze Intervalle (unter 5 Minuten) können die API-Server unnötig belasten. Bitte gehen Sie verantwortungsvoll mit der öffentlichen, kostenlosen API um.

### 7.2 Autostart konfigurieren

Wenn Sie möchten, dass Sonnenuhr automatisch mit Windows gestartet wird:

1. Aktivieren Sie die Checkbox **„Mit Windows starten (Autostart)"** im Hauptfenster
2. Die Anwendung trägt sich sofort in den Windows-Autostart ein
3. Beim nächsten Windows-Start öffnet sich Sonnenuhr automatisch minimiert im Systemtray

**Autostart deaktivieren:**
- Deaktivieren Sie die Checkbox **„Mit Windows starten (Autostart)"**
- Der Registry-Eintrag wird sofort entfernt

**Manuell prüfen:**
Der Autostart-Eintrag befindet sich in der Windows Registry unter:
```
HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run
Eintrag: Sonnenuhr
```

---

## 9. Fehlerbehebung

### 8.1 Häufige Probleme und Lösungen

#### Problem: „Keine Netzwerkverbindung" – Fehlermeldung in Statusbar

**Symptom:** Die Statusbar zeigt „Fehler: Keine Netzwerkverbindung" oder ähnlich.

**Mögliche Ursachen und Lösungen:**

| Ursache | Lösung |
|---------|--------|
| Kein Internet | Internetverbindung prüfen und herstellen |
| Firewall blockiert Sonnenuhr | Windows Defender Firewall: Ausgehende Verbindungen für `Sonnenuhr.exe` erlauben |
| Proxy-Server erforderlich | HTTP-Proxy in Windows-Systemeinstellungen konfigurieren |
| VPN blockiert API-Zugriff | VPN vorübergehend deaktivieren oder API-Domain in Ausnahmeliste eintragen |

#### Problem: „API nicht erreichbar" – Sonnendaten werden nicht geladen

**Symptom:** Trotz Internetverbindung werden keine Sonnendaten geladen.

**Mögliche Ursachen und Lösungen:**

| Ursache | Lösung |
|---------|--------|
| sunrise-sunset.org temporär nicht erreichbar | Einige Minuten warten und erneut versuchen |
| DNS-Auflösung fehlgeschlagen | DNS-Cache leeren: `ipconfig /flushdns` in der Eingabeaufforderung ausführen |
| SSL/TLS-Zertifikatsproblem | Windows-Zertifikatsspeicher aktualisieren; Windows Update ausführen |

#### Problem: Wallpaper wird nicht auf dem Desktop gesetzt

**Symptom:** Das Bild wird generiert (keine Fehlermeldung), aber der Desktop-Hintergrund ändert sich nicht.

**Mögliche Ursachen und Lösungen:**

| Ursache | Lösung |
|---------|--------|
| Windows-Richtlinie verhindert Wallpaper-Änderung | Gruppenrichtlinien prüfen (nur in Domänen-Umgebungen relevant) |
| Slideshow-Modus aktiv | Windows-Einstellungen → Personalisierung → Hintergrund → auf „Bild" setzen |
| Schreibrechte fehlen | Prüfen ob `%APPDATA%\Sonnenuhr\` schreibbar ist |

**Manuelle Prüfung:**
Prüfen Sie ob die Datei `%APPDATA%\Sonnenuhr\wallpaper.png` existiert und das aktuelle Bild enthält.

#### Problem: Sonnenuhr-Anwendung startet nicht

**Symptom:** Beim Doppelklick auf `Sonnenuhr.exe` passiert nichts oder eine Fehlermeldung erscheint.

**Mögliche Ursachen und Lösungen:**

| Ursache | Lösung |
|---------|--------|
| .NET 10 Runtime nicht installiert | .NET 10 Runtime von [dotnet.microsoft.com](https://dotnet.microsoft.com) herunterladen und installieren |
| Beschädigte Installation | Anwendung neu herunterladen und entpacken |
| Antivirensoftware blockiert | Sonnenuhr zur Ausnahmeliste des Antivirusprogramms hinzufügen |

#### Problem: Sonnenuhr-Zifferblatt sieht falsch aus (Linien stimmen nicht)

**Symptom:** Die Stundenlinien erscheinen verzerrt oder unlogisch.

**Lösung:** Überprüfen Sie die eingegebenen Koordinaten. Stellen Sie sicher, dass:
- Breitengrad und Längengrad nicht vertauscht sind
- Das Dezimaltrennzeichen korrekt ist (Punkt, kein Komma)
- Der Breitengrad positiv ist (nördliche Breite für Deutschland)

### 8.2 Protokolldateien

Sonnenuhr erstellt eine Protokolldatei unter:
```
%APPDATA%\Sonnenuhr\sonnenuhr.log
```

Bei anhaltenden Problemen können Sie diese Datei öffnen und nach Fehlermeldungen suchen. Die Datei kann auch dem Support-Team der Breihof IT GmbH übermittelt werden.

---

## 10. Technische Details

### 9.1 Datenspeicherung

Sonnenuhr speichert alle benutzerspezifischen Daten im Windows-Anwendungsdatenverzeichnis. Es werden **keine Daten auf externen Servern oder in der Cloud gespeichert**.

| Datei | Pfad | Inhalt |
|-------|------|--------|
| `settings.json` | `%APPDATA%\Sonnenuhr\settings.json` | Alle Benutzereinstellungen im JSON-Format |
| `wallpaper.png` | `%APPDATA%\Sonnenuhr\wallpaper.png` | Das zuletzt generierte Wallpaper-Bild |
| `sonnenuhr.log` | `%APPDATA%\Sonnenuhr\sonnenuhr.log` | Anwendungsprotokoll (Fehler und Ereignisse) |

Der `%APPDATA%`-Pfad entspricht typischerweise `C:\Users\<IhrBenutzername>\AppData\Roaming\`.

### 9.2 Verwendete API

Sonnenuhr verwendet die öffentliche, kostenlose REST-API von **sunrise-sunset.org** für den Abruf astronomischer Sonnendaten.

| Eigenschaft | Beschreibung |
|-------------|--------------|
| **API-Anbieter** | sunrise-sunset.org |
| **Basis-URL** | `https://api.sunrise-sunset.org/json` |
| **Übertragenes Datum** | Geografische Koordinaten (Breitengrad, Längengrad) und Datum |
| **Nicht übertragen** | Personenbezogene Daten, Benutzername, Gerätedaten |
| **Datenschutz** | Keine Registrierung erforderlich; keine Benutzeridentifikation |

### 9.3 Windows Registry

Sonnenuhr schreibt in folgende Registry-Schlüssel:

| Schlüssel | Zweck |
|-----------|-------|
| `HKCU\Control Panel\Desktop\Wallpaper` | Pfad zum aktuellen Wallpaper-Bild |
| `HKCU\Software\Microsoft\Windows\CurrentVersion\Run\Sonnenuhr` | Autostart-Eintrag (nur wenn Autostart aktiviert) |

Alle Änderungen werden nur im Benutzerkontext (`HKCU`) vorgenommen – Administratorrechte sind nicht erforderlich.

### 9.4 Sonnenuhr-Mathematik

Die Stundenlinienwinkel der Sonnenuhr werden nach der Formel für **horizontale Sonnenuhren** berechnet:

```
H = arctan(sin(φ) × tan(t))
```

- `H` = Stundenlinienwinkel gegenüber der Mittagslinie (in Grad)
- `φ` = geografischer Breitengrad (in Radiant)
- `t` = Stundenwinkel = 15° × (Stunden vom Sonnenmittag)

Diese Formel liefert exakte Stundenlinien für einen flach auf dem Boden liegenden Sonnenuhr-Typ (Horizontalsonnenuhr), wie er in Gärten häufig anzutreffen ist.

---

## 11. Häufig gestellte Fragen (FAQ)

**F: Benötigt Sonnenuhr ständig eine Internetverbindung?**  
A: Nein. Die Internetverbindung wird nur beim Abruf der Sonnendaten benötigt. Zwischen den Aktualisierungen läuft die Anwendung offline. Wenn keine Verbindung verfügbar ist, bleibt das zuletzt generierte Wallpaper bestehen.

**F: Wie funktioniert die Stadtsuche?**  
A: Geben Sie einen Stadtnamen in das Feld „Ortsname" ein und klicken Sie auf „🔍 Suchen". Die Anwendung fragt die kostenlose OpenStreetMap Nominatim API ab und übernimmt die Koordinaten automatisch. Bei mehreren gleichnamigen Orten (z.B. gibt es mehrere Orte namens „Eberbach" weltweit) erscheint ein Auswahldialog. Die Koordinaten können anschließend noch manuell angepasst werden.

**F: Ändert Sonnenuhr meine aktuellen Datenschutzeinstellungen oder Systemdateien?**  
A: Nein. Sonnenuhr schreibt ausschließlich in den Benutzerkontext der Windows Registry (HKCU) und in das Anwendungsdatenverzeichnis (`%APPDATA%`). Systemdateien werden nicht verändert.

**F: Kann ich Sonnenuhr für mehrere Standorte gleichzeitig verwenden?**  
A: In der aktuellen Version (1.0) wird nur ein Standort unterstützt. Jeder Benutzer kann jedoch seinen eigenen Standort konfigurieren.

**F: Wie aktuell sind die Sonnendaten?**  
A: Die Sonnendaten werden in dem von Ihnen konfigurierten Intervall (Standard: 60 Minuten) neu abgerufen. Die astronomischen Daten für Sonnenaufgang und -untergang ändern sich im Tagesverlauf nicht; der Schattenzeiger auf der Sonnenuhr hingegen aktualisiert sich mit jedem Wallpaper-Neugenerierungszyklus.

**F: Welche Auflösung hat das generierte Wallpaper?**  
A: Standard ist 1920×1080 Pixel (Full HD). Die Auflösung kann im Konfigurationsdialog angepasst werden.

**F: Kann ich die Anwendung deinstallieren, ohne Spuren zu hinterlassen?**  
A: Ja. Eine vollständige Deinstallation ist in der [Installationsanleitung](Installationsanleitung.md) beschrieben. Die Anwendung hinterlässt nach vollständiger Deinstallation keine Spuren im System.

---

*Dokument erstellt von: Uwe Markus Münch | Breihof IT GmbH | IHK Rhein-Neckar | 01.07.2026*
