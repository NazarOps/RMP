using RMP.Services;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WMPLib;

namespace RMP
{
    public class linq
    {
        bool search = true;

        public LogService _logService { get; set; }
        public linq(LogService logService) { _logService = logService; }

        public void Search()
        {

            while (search)
            {
                AnsiConsole.Clear();
                var theme = new ThemeChanger();
                var primaryColorName = theme.GetPrimaryColorName();
                var primaryColor = theme.GetPrimaryColor();

                string musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                string[] songs = Directory.GetFiles(musicFolder, "*.mp3");

                var searchTerm = AnsiConsole.Ask<string>("Enter search term:");
                AnsiConsole.MarkupLine($"Press [{primaryColorName}]ESC[/] to go back to main menu");
                AnsiConsole.MarkupLine($"[{primaryColorName}]Searching for: {Markup.Escape(searchTerm)}[/]\n");

                var results = songs
            .Where(s => Path.GetFileNameWithoutExtension(s)
            .Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();

                if (results.Count == 0)
                {
                    AnsiConsole.MarkupLine("[red]No results found.[/]");
                    _logService.LogWarning($"Search for {searchTerm} returned 0 results in {musicFolder}");
                    Thread.Sleep(500);
                }
                else
                {
                    foreach (var song in results)
                    {
                        AnsiConsole.MarkupLine($"Found: {Markup.Escape(Path.GetFileName(song))}");
                    }

                    Thread.Sleep(100);

                    var songToPlay = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Choose a song to play")
                    .AddChoices(results)
                    .HighlightStyle(new Style(primaryColor))
                    );

                    WindowsMediaPlayer music = null;

                    try
                    {
                        Console.Clear();
                        music = new WindowsMediaPlayer();
                        music.URL = songToPlay;

                        try
                        {
                            int vol = (int)Math.Clamp(Settings.Current.Volume * 100f, 0f, 100f);
                            SafeCall(() => music.settings.volume = vol);
                        }
                        catch { /* ignore volume set errors */ }

                        bool keepplaying = true;

                        while (keepplaying)
                        {
                            var meta = MetadataReader.Read(songToPlay);

                            // prepare safe (escaped) values
                            string safeName = Markup.Escape(meta.Title ?? Path.GetFileNameWithoutExtension(songToPlay));
                            string safeArtist = Markup.Escape((meta.Artist ?? "Okänd artist"));
                            string safeAlbum = Markup.Escape((meta.Album ?? "Okänt album"));
                            string yearReleased = (meta.Year > 0) ? meta.Year.ToString() : "Unknown";
                            string safeYear = Markup.Escape(yearReleased);
                            string safeDuration = Markup.Escape((meta.Duration ?? "Unknown"));

                            AnsiConsole.MarkupLine($"[{primaryColorName}]Now playing:[/] [rapidblink]{safeName}[/]");
                            AnsiConsole.MarkupLine($"[{primaryColorName}]Artist:[/] {safeArtist}");
                            AnsiConsole.MarkupLine($"[{primaryColorName}]Album:[/] {safeAlbum} ({safeYear})");
                            AnsiConsole.MarkupLine($"[{primaryColorName}]Duration:[/] {safeDuration}");
                            AnsiConsole.MarkupLine($"\nUse [{primaryColorName}]↑↓[/] to change volume");
                            AnsiConsole.MarkupLine($"[{primaryColorName}]Press ESC to go back to menu[/]");

                            // give the player a short moment to load metadata
                            int waitCount = 0;
                            double duration = 0;
                            while (waitCount < 30)
                            {
                                duration = SafeGetDouble(() => music.currentMedia?.duration ?? 0);
                                if (duration > 0) break;
                                Thread.Sleep(100);
                                waitCount++;
                            }

                            if (duration <= 0) duration = 100; // fallback

                            bool stopSong = false;

                            AnsiConsole.Progress()
                                .AutoRefresh(true)
                                .Columns(new ProgressColumn[]
                                {
                            new TaskDescriptionColumn(),
                            new ProgressBarColumn
                            {
                                CompletedStyle = new Style(primaryColor)
                            },
                            new PercentageColumn(),
                            new SpinnerColumn(Spinner.Known.Dots2) { Style = new Style(primaryColor) }
                                })
                                .Start(ctx =>
                                {
                                    music.controls.play();
                                    var task = ctx.AddTask($"[bold]{safeName}[/]", maxValue: duration);

                                    while (!ctx.IsFinished && !stopSong)
                                    {
                                        double position = SafeGetDouble(() => music.controls.currentPosition);

                                        if (position < 0) position = 0;
                                        if (position > duration) position = duration;

                                        task.Value = position;

                                        if (position >= duration || SafeGetInt(() => (int)music.playState) == (int)WMPPlayState.wmppsStopped)
                                        {
                                            stopSong = true;
                                            Thread.Sleep(100);

                                            break;
                                        }

                                        if (Console.KeyAvailable)
                                        {
                                            var key = Console.ReadKey(true).Key;
                                            switch (key)
                                            {
                                                case ConsoleKey.Escape:
                                                    task.StopTask();
                                                    stopSong = true;
                                                    SafeCall(() => music.controls.stop());
                                                    keepplaying = false;
                                                    search = false;
                                                    break;

                                                case ConsoleKey.UpArrow:
                                                    music.settings.volume = Math.Min(100, music.settings.volume + 5);
                                                    music.settings.volume = music.settings.volume;
                                                    break;

                                                case ConsoleKey.DownArrow:
                                                    music.settings.volume = Math.Max(0, music.settings.volume - 5);
                                                    music.settings.volume = music.settings.volume;
                                                    break;
                                            }
                                        }

                                        Thread.Sleep(250);
                                    }
                                });
                        }

                    }
                    finally
                    {

                        if (music != null)
                        {
                            try
                            {
                                music.controls.stop();
                                Marshal.ReleaseComObject(music);
                            }
                            catch (Exception ex)
                            {
                                _logService.LogInfo($"No results found: {results}");
                            }
                        }
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }

                }
            }
        }

        private static void SafeCall(Action act)
        {
            try { act(); }
            catch (COMException) { }
            catch { }
        }

        private static double SafeGetDouble(Func<double> get)
        {
            try
            {
                return get();
            }
            catch (COMException)
            {
                return 0;
            }
            catch
            {
                return 0;
            }
        }
        private static int SafeGetInt(Func<int> get)
        {
            try { return get(); }
            catch (COMException) { return 0; }
            catch { return 0; }
        }
    }
}