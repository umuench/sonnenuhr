# Installationsanleitung

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

1. [Systemvoraussetzungen](#1-systemvoraussetzungen)
2. [Vorbereitung: .NET 10 Runtime installieren](#2-vorbereitung-net-10-runtime-installieren)
3. [Download der Sonnenuhr-Anwendung](#3-download-der-sonnenuhr-anwendung)
4. [Installation und Erststart](#4-installation-und-erststart)
5. [Ersteinrichtung und Konfiguration](#5-ersteinrichtung-und-konfiguration)
6. [Autostart konfigurieren](#6-autostart-konfigurieren)
7. [Deinstallation](#7-deinstallation)
8. [Bekannte Einschränkungen](#8-bekannte-einschränkungen)
9. [Support und Kontakt](#9-support-und-kontakt)

---

## 1. Systemvoraussetzungen

Stellen Sie vor der Installation sicher, dass Ihr System alle folgenden Anforderungen erfüllt.

### 1.1 Pflichtanforderungen

| Komponente | Mindestanforderung | Hinweis |
|------------|-------------------|---------|
| **Betriebssystem** | Windows 11, Version 22H2 (Build 22621) oder neuer | Windows 10 wird **nicht** unterstützt |
| **.NET Runtime** | Microsoft .NET 10.0 Runtime (Windows Desktop Runtime) | Muss separat installiert werden, falls nicht vorhanden |
| **Prozessor** | x64-kompatibler Prozessor, ≥ 1,5 GHz | ARM64 wird in dieser Version nicht unterstützt |
| **Arbeitsspeicher** | Mindestens 2 GB RAM frei | Empfohlen: 8 GB oder mehr |
| **Festplatte** | Mindestens 50 MB freier Speicherplatz | Für Anwendung (~10 MB) und generierte Bilder (~5 MB) |
| **Internetverbindung** | Für API-Abrufe erforderlich | Nur beim Abruf von Sonnendaten benötigt |

### 1.2 Benutzerrechte

Die Installation von Sonnenuhr erfordert **keine Administratorrechte**. Die Anwendung wird vollständig im Benutzerprofilverzeichnis installiert und schreibt ausschließlich in benutzerspezifische Bereiche der Windows Registry (`HKCU`).

### 1.3 Windows-Version prüfen

So prüfen Sie Ihre Windows-Version:

1. Drücken Sie `Windows-Taste + R`
2. Geben Sie `winver` ein und drücken Sie `Enter`
3. Das Dialogfenster zeigt die Windows-Version und Build-Nummer an

> ✅ Sonnenuhr benötigt mindestens **Build 22621** (Windows 11 22H2).

---

## 2. Vorbereitung: .NET 10 Runtime installieren

Sonnenuhr benötigt die **Microsoft .NET 10.0 Windows Desktop Runtime**. Falls diese noch nicht auf Ihrem System installiert ist, folgen Sie diesen Schritten:

### 2.1 Prüfen, ob .NET 10 bereits installiert ist

1. Öffnen Sie die **Windows-Eingabeaufforderung** (`Windows-Taste + R` → `cmd` → `Enter`)
2. Geben Sie folgenden Befehl ein:
   ```
   dotnet --list-runtimes
   ```
3. Falls in der Ausgabe eine Zeile mit `Microsoft.WindowsDesktop.App 10.x.x` erscheint, ist .NET 10 bereits installiert. Fahren Sie mit [Schritt 3](#3-download-der-sonnenuhr-anwendung) fort.

### 2.2 .NET 10 Runtime herunterladen und installieren

1. Öffnen Sie Ihren Webbrowser und navigieren Sie zu:
   ```
   https://dotnet.microsoft.com/de-de/download/dotnet/10.0
   ```
2. Klicken Sie im Abschnitt **„.NET Desktop Runtime"** auf die Schaltfläche **„x64"** unter **„Windows"**
3. Laden Sie den Installer (`windowsdesktop-runtime-10.0.x-win-x64.exe`) herunter
4. Starten Sie den Installer durch Doppelklick
5. Folgen Sie den Anweisungen des Installationsassistenten (Standardoptionen sind ausreichend)
6. Starten Sie den Computer nach der Installation **nicht** neu – dies ist nicht erforderlich

### 2.3 Installation prüfen

Wiederholen Sie [Schritt 2.1](#21-prüfen-ob-net-10-bereits-installiert-ist). Die Ausgabe sollte nun eine Zeile der Form enthalten:
```
Microsoft.WindowsDesktop.App 10.0.0 [C:\Programme\dotnet\shared\Microsoft.WindowsDesktop.App\10.0.0]
```

---

## 3. Download der Sonnenuhr-Anwendung

### 3.1 Von GitHub Releases herunterladen

1. Öffnen Sie Ihren Webbrowser und navigieren Sie zur GitHub-Releases-Seite:
   ```
   https://github.com/[organisation]/sonnenuhr/releases/latest
   ```
2. Laden Sie die aktuelle Version herunter:
   - **`Sonnenuhr-v1.0.0-win-x64.zip`** – Vollständiges Release-Paket (empfohlen)
3. Speichern Sie die ZIP-Datei in einem temporären Verzeichnis (z.B. `Downloads`)

### 3.2 Inhalt des Release-Pakets

Die heruntergeladene ZIP-Datei enthält:

```
Sonnenuhr-v1.0.0-win-x64/
├── Sonnenuhr.exe              ← Hauptprogramm
├── Sonnenuhr.dll              ← Anwendungsbibliothek
├── Sonnenuhr.runtimeconfig.json
├── README.md                  ← Kurzanleitung (Englisch)
└── docs/                      ← Vollständige Dokumentation
    ├── Benutzerhandbuch.md
    └── Installationsanleitung.md
```

---

## 4. Installation und Erststart

Sonnenuhr benötigt **keinen Installationsassistenten** – die Anwendung ist portabel und kann aus jedem Verzeichnis heraus gestartet werden.

### 4.1 Anwendung entpacken

**Option A: Empfohlenes Installationsverzeichnis**

1. Erstellen Sie das Verzeichnis `C:\Users\<IhrBenutzername>\AppData\Local\Sonnenuhr\` (oder ein anderes gewünschtes Verzeichnis)
2. Entpacken Sie den Inhalt der ZIP-Datei in dieses Verzeichnis

**Option B: Beliebiges Verzeichnis**

Entpacken Sie die ZIP-Datei in ein beliebiges Verzeichnis, auf das Sie Lese- und Schreibzugriff haben (z.B. `C:\Programme\Sonnenuhr\` – beachten Sie, dass hierfür möglicherweise Administratorrechte benötigt werden).

> 💡 **Empfehlung:** Wählen Sie ein Verzeichnis im Benutzerprofil (z.B. unter `AppData\Local` oder `Dokumente`), um Administratorrechte zu vermeiden.

### 4.2 Anwendung starten

1. Navigieren Sie zum Installationsverzeichnis
2. Doppelklicken Sie auf `Sonnenuhr.exe`

**Beim ersten Start:**
- Windows Defender SmartScreen zeigt möglicherweise eine Warnung an. Klicken Sie auf **„Weitere Informationen"** und dann auf **„Trotzdem ausführen"**.
- Das Hauptfenster der Anwendung öffnet sich mit Standardeinstellungen.

### 4.3 Optionale Desktopverknüpfung erstellen

1. Rechtsklicken Sie auf `Sonnenuhr.exe`
2. Wählen Sie **„Weitere Optionen anzeigen"** → **„Verknüpfung erstellen"**
3. Verschieben Sie die erstellte Verknüpfung auf den Desktop oder in die Taskleiste

---

## 5. Ersteinrichtung und Konfiguration

Nach dem ersten Start führen Sie folgende Schritte durch:

### 5.1 Standort eingeben

1. Öffnen Sie das Hauptfenster (falls noch nicht geöffnet)
2. Geben Sie im Bereich **„Standort-Konfiguration"** ein:
   - **Ortsname:** z.B. `Frankfurt am Main`
   - **Breitengrad:** z.B. `50.1109`
   - **Längengrad:** z.B. `8.6821`
3. Wählen Sie Ihre **Zeitzone** aus dem Dropdown-Menü (für Deutschland: `W. Europe Standard Time`)

> 💡 **Tipp:** Koordinaten ermitteln Sie auf [maps.google.com](https://maps.google.com) durch Rechtsklick auf Ihren Standort.

### 5.2 Aktualisierungsintervall festlegen

Stellen Sie im Bereich **„Einstellungen"** das gewünschte Aktualisierungsintervall ein (Standard: 60 Minuten).

### 5.3 Erstes Wallpaper generieren

Klicken Sie auf **„⟳ Jetzt aktualisieren"**. Die Anwendung:
1. Prüft die Netzwerkverbindung
2. Ruft Sonnendaten von der API ab
3. Berechnet die Sonnenuhr-Geometrie
4. Generiert das Wallpaper-Bild
5. Setzt das Bild als Desktop-Hintergrund

### 5.4 Darstellung anpassen (optional)

Klicken Sie auf **„⚙ Darstellung"** um Farben, Schriften und Anzeigeoptionen anzupassen. Alle Änderungen werden nach dem Klick auf „OK" sofort wirksam.

---

## 6. Autostart konfigurieren

Damit Sonnenuhr automatisch mit Windows startet:

### 6.1 Autostart aktivieren (über Anwendung)

1. Aktivieren Sie im Hauptfenster die Checkbox **„Mit Windows starten (Autostart)"**
2. Die Anwendung schreibt sofort einen Eintrag in die Windows Registry

### 6.2 Autostart-Eintrag manuell prüfen

1. Drücken Sie `Windows-Taste + R`
2. Geben Sie `regedit` ein und bestätigen Sie mit `Enter`
3. Navigieren Sie zu:
   ```
   HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run
   ```
4. Suchen Sie den Eintrag **„Sonnenuhr"** mit dem Wert `"C:\...\Sonnenuhr.exe" --minimized`

### 6.3 Autostart-Eintrag über PowerShell prüfen

Öffnen Sie PowerShell und führen Sie folgenden Befehl aus:
```powershell
Get-ItemProperty "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run" | Select-Object Sonnenuhr
```

---

## 7. Deinstallation

### 7.1 Anwendung beenden

1. Rechtsklicken Sie auf das Sonnenuhr-Symbol im Systemtray (☀)
2. Wählen Sie **„Beenden"**

### 7.2 Autostart-Eintrag entfernen

**Option A: Über die Anwendung (empfohlen)**
- Deaktivieren Sie vor dem Beenden die Checkbox **„Mit Windows starten (Autostart)"**

**Option B: Manuell über Registry**
1. Öffnen Sie den Registrierungseditor (`regedit`)
2. Navigieren Sie zu `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`
3. Löschen Sie den Eintrag **„Sonnenuhr"** (Rechtsklick → Löschen)

**Option C: Über PowerShell**
```powershell
Remove-ItemProperty -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run" -Name "Sonnenuhr" -ErrorAction SilentlyContinue
```

### 7.3 Anwendungsdateien löschen

1. Löschen Sie das Installationsverzeichnis (z.B. `C:\Users\<Name>\AppData\Local\Sonnenuhr\`)
2. Löschen Sie das Anwendungsdatenverzeichnis:
   - Öffnen Sie den Windows Explorer
   - Navigieren Sie zu `%APPDATA%\Sonnenuhr\`
   - Löschen Sie den gesamten Ordner

**Schnell über Ausführen-Dialog:**
1. `Windows-Taste + R` → `%APPDATA%` → Enter
2. Löschen Sie den Ordner `Sonnenuhr`

### 7.4 Desktop-Hintergrund zurücksetzen

Nach der Deinstallation zeigt Windows möglicherweise das zuletzt gesetzte Sonnenuhr-Wallpaper als Hintergrund an (da die Bilddatei gelöscht wurde, erscheint ein schwarzer Hintergrund). So setzen Sie einen neuen Hintergrund:

1. Rechtsklicken Sie auf den Desktop
2. Wählen Sie **„Personalisieren"**
3. Wählen Sie unter **„Hintergrund"** ein neues Bild oder eine Farbe aus

### 7.5 Vollständigkeits-Checkliste für Deinstallation

- [ ] Anwendung beendet (Tray-Icon nicht mehr sichtbar)
- [ ] Autostart-Checkbox deaktiviert oder Registry-Eintrag manuell gelöscht
- [ ] Installationsverzeichnis gelöscht
- [ ] `%APPDATA%\Sonnenuhr\` Verzeichnis gelöscht
- [ ] Desktop-Hintergrund in Windows-Einstellungen zurückgesetzt

---

## 8. Bekannte Einschränkungen

| Einschränkung | Beschreibung | Workaround |
|---------------|--------------|------------|
| **Nur Windows 11** | Die Anwendung unterstützt ausschließlich Windows 11 (22H2+) | Kein Workaround; Betriebssystem aktualisieren |
| **Kein Mehrmonitor-Betrieb** | Das generierte Wallpaper wird auf allen Monitoren identisch gesetzt | Nicht implementiert in v1.0 |
| **Nur x64** | ARM64-Prozessoren (z.B. Qualcomm Snapdragon) werden nicht unterstützt | Nicht implementiert in v1.0 |
| **Manueller Standort** | GPS-Standortermittlung ist nicht implementiert | Koordinaten manuell eingeben |
| **Ein Standort** | Nur ein Standort kann gleichzeitig konfiguriert werden | Mehrere Benutzerkonten anlegen |
| **Keine Offlinedaten** | Ohne Internetverbindung werden keine neuen Sonnendaten abgerufen | Letztes Wallpaper bleibt bestehen |
| **Keine Vorhersage** | Zukünftige Sonnendaten (andere Tage) können nicht abgerufen werden | Nicht implementiert in v1.0 |

---

## 9. Support und Kontakt

Bei Fragen oder Problemen stehen folgende Ressourcen zur Verfügung:

| Ressource | Adresse |
|-----------|---------|
| **GitHub Issues** | `https://github.com/[organisation]/sonnenuhr/issues` |
| **Ausbildungsbetrieb** | Breihof IT GmbH – IT-Support |
| **Benutzerhandbuch** | [Benutzerhandbuch.md](Benutzerhandbuch.md) |

Bitte geben Sie bei Supportanfragen folgende Informationen an:
- Sonnenuhr-Version (in der Titelleiste des Hauptfensters)
- Windows-Version (aus `winver`)
- .NET-Version (aus `dotnet --version`)
- Protokolldatei: `%APPDATA%\Sonnenuhr\sonnenuhr.log`
- Fehlerbeschreibung mit möglichst genauen Reproduktionsschritten

---

*Dokument erstellt von: Uwe Markus Münch | Breihof IT GmbH | IHK Rhein-Neckar | 01.07.2026*
