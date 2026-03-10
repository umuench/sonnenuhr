// ============================================================
// Datei:     Program.cs
// Projekt:   Sonnenuhr – Standortspezifischer Wallpaper-Generator
// Autor:     Uwe Markus Münch
// Firma:     Breihof IT GmbH
// Datum:     2026-07-01
// Beschreibung:
//   Einstiegspunkt der Anwendung. Konfiguriert Windows Forms und
//   startet das Hauptfenster. Unterstützt --minimized-Startparameter
//   für den Autostart-Betrieb.
// ============================================================

using Sonnenuhr.Forms;

namespace Sonnenuhr;

/// <summary>
/// Statische Programmklasse mit dem Anwendungs-Einstiegspunkt.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Einstiegspunkt der Anwendung.
    /// </summary>
    /// <param name="args">Kommandozeilenargumente (z. B. <c>--minimized</c>).</param>
    [STAThread]
    private static void Main(string[] args)
    {
        // ── EINGABE ────────────────────────────────────────────
        bool startMinimized = args.Contains("--minimized", StringComparer.OrdinalIgnoreCase);

        // ── VERARBEITUNG ───────────────────────────────────────
        ApplicationConfiguration.Initialize();

        // Hohe DPI-Auflösung aktivieren
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

        // Nicht behandelte Ausnahmen protokollieren
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) =>
            MessageBox.Show(
                $"Ein unerwarteter Fehler ist aufgetreten:\n\n{e.Exception.Message}",
                "Sonnenuhr – Fehler",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

        // ── AUSGABE ────────────────────────────────────────────
        Application.Run(new MainForm(startMinimized));
    }
}