// MusicPlayback.cs
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Spectre.Console;
using WMPLib;
using RMP.Services;

namespace RMP
{
    public class MusicPlayback
    {
        public int songindex = 0;

        public void PlayMusic()
        {
            WindowsMediaPlayer music = null;
            try
            {
                music = new WindowsMediaPlayer();
                bool keepPlaying = true;

                while (keepPlaying)
                {
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[slowblink]Scanning directory...[/]");
                    Thread.Sleep(800);
                    AnsiConsole.Clear();

                    string musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                    string[] songs = Directory.GetFiles(musicFolder, "*.mp3");

                    if (songs.Length == 0)
                    {
                        AnsiConsole.MarkupLine("[red]No MP3 files found in your Music folder.[/]");
                        Thread.Sleep(1500);
                        return;
                    }

                    songindex = (songindex % songs.Length + songs.Length) % songs.Length;
                    string currentsong = songs[songindex];

                    // set URL (COM)
                    music.URL = currentsong;

                    var meta = MetadataReader.Read(currentsong);
                    string safeName = Markup.Escape(meta.Title ?? Path.GetFileNameWithoutExtension(currentsong));
                    string artist = Markup.Escape("Artist: " + meta.Artist ?? "Okänd artist");
                    string album = Markup.Escape("\nAlbum: " + meta.Album ?? "Okänt album");
                    string yearReleased = (meta.Year > 0) ? meta.Year.ToString() : "Unknown";
                    string songDuration = Markup.Escape("Duration: " + meta.Duration ?? "Unknown");
                   

                    AnsiConsole.MarkupLine($"[blue]Now playing:[/] [rapidblink]{safeName}[/]");
                    AnsiConsole.MarkupLine($"[blue]{artist} — {album} ({yearReleased}) \n{songDuration}[/]");
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
                            AnsiConsole.WriteLine("Use <-- and --> arrow keys to change track");

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
                                    songindex = (songindex + 1) % songs.Length;
                                    break;
                                }

                                if (Console.KeyAvailable)
                                {
                                    var key = Console.ReadKey(true).Key;
                                    switch (key)
                                    {
                                        case ConsoleKey.RightArrow:
                                            task.StopTask();
                                            stopSong = true;
                                            SafeCall(() => music.controls.stop());
                                            Thread.Sleep(100);
                                            songindex = (songindex + 1) % songs.Length;
                                            break;

                                        case ConsoleKey.LeftArrow:
                                            task.StopTask();
                                            stopSong = true;
                                            SafeCall(() => music.controls.stop());
                                            Thread.Sleep(100);
                                            songindex = (songindex - 1 + songs.Length) % songs.Length;
                                            break;

                                        case ConsoleKey.Escape:
                                            task.StopTask();
                                            stopSong = true;
                                            SafeCall(() => music.controls.stop());
                                            keepPlaying = false;
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
