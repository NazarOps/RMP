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
        public Style SearchHighlightStyle { get; set; }

        public void Search()
        {


            while (search)
            {
                AnsiConsole.Clear();
                string musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                string[] songs = Directory.GetFiles(musicFolder, "*.mp3");

                var searchTerm = AnsiConsole.Ask<string>("Enter search term:");
                AnsiConsole.MarkupLine("Press [red]ESC[/] to go back to main menu");
                AnsiConsole.MarkupLine($"[blue]Searching for: {searchTerm}[/]\n");

                var results = songs
            .Where(s => Path.GetFileNameWithoutExtension(s)
            .Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();

                if (results.Count == 0)
                {
                    AnsiConsole.MarkupLine("[red]No results found.[/]");
                }
                else
                {
                    foreach (var song in results)
                    {
                        AnsiConsole.MarkupLine($"Found: {Path.GetFileName(song)}");

                    }

                    Thread.Sleep(100);
                   
                        var songToPlay = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Choose a song to play")
                    
                    .AddChoices(results)
                    );

                        WindowsMediaPlayer music = null;

                        try
                        {
                            Console.Clear();
                            music = new WindowsMediaPlayer();
                            music.URL = songToPlay;
                            bool keepplaying = true;

                            while (keepplaying)
                            {
                                var meta = MetadataReader.Read(songToPlay);
                                string safeName = Markup.Escape(meta.Title ?? Path.GetFileNameWithoutExtension(searchTerm));
                                string artist = Markup.Escape("Artist: " + meta.Artist ?? "Okänd artist");
                                string album = Markup.Escape("\nAlbum: " + meta.Album ?? "Okänt album");
                                string yearReleased = (meta.Year > 0) ? meta.Year.ToString() : "Unknown";
                                string songDuration = Markup.Escape("Duration: " + meta.Duration ?? "Unknown");


                                AnsiConsole.MarkupLine($"[blue]Now playing:[/] [rapidblink]{safeName}[/]");
                                AnsiConsole.MarkupLine($"[grey]{artist} — {album} ({yearReleased}) \n{songDuration}[/]");
                                AnsiConsole.MarkupLine("[blue]Press ESC to go back to menu[/]");

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
                                CompletedStyle = new Style(Color.Blue)
                            },
                            new PercentageColumn(),
                            new SpinnerColumn(Spinner.Known.Dots2) { Style = new Style(Color.Blue) }
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
                                catch { }
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
                
