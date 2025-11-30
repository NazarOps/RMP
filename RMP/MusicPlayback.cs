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
        private LogService _logService { get; set; }
        public MusicPlayback(LogService logService) { _logService = logService; }

        public void PlayMusic()
        {
            var theme = new ThemeChanger();

            WindowsMediaPlayer music = null;
            try
            {
                music = new WindowsMediaPlayer();
                try
                {
                    int vol = (int)Math.Clamp(Settings.Current.Volume * 100f, 0f, 100f);
                    SafeCall(() => music.settings.volume = vol);
                }
                catch { /* ignore volume set errors */ }

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

                    var primaryColorName = theme.GetPrimaryColorName();

                    var meta = MetadataReader.Read(currentsong);
                    string titleText = meta.Title ?? Path.GetFileNameWithoutExtension(currentsong);
                    string safeTitle = Markup.Escape(titleText);

                    string artistText = meta.Artist ?? "Okänd artist";
                    string safeArtist = Markup.Escape(artistText);

                    string albumText = meta.Album ?? "Okänt album";
                    string safeAlbum = Markup.Escape(albumText);

                    string yearReleased = (meta.Year > 0) ? meta.Year.ToString() : "Unknown";
                    string safeYear = Markup.Escape(yearReleased);

                    string durationText = meta.Duration ?? "Unknown";
                    string safeDuration = Markup.Escape(durationText);

                    // Use markup tags for colors and embed escaped content inside
                    AnsiConsole.MarkupLine($"[{primaryColorName}]Now playing:[/][rapidblink] {safeTitle}[/]");
                    AnsiConsole.MarkupLine($"[{primaryColorName}]Artist:[/] {safeArtist}");
                    AnsiConsole.MarkupLine($"[{primaryColorName}]Album:[/] {safeAlbum} ({safeYear})");
                    AnsiConsole.MarkupLine($"[{primaryColorName}]Duration:[/] {safeDuration}");
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
                                CompletedStyle = new Style(theme.GetPrimaryColor())
                            },
                            new PercentageColumn(),
                            new SpinnerColumn(Spinner.Known.Dots2) { Style = new Style(theme.GetPrimaryColor()) }
                        })
                        .Start(ctx =>
                        {
                            music.controls.play();
                            _logService.LogInfo($"{safeTitle}");
                            var task = ctx.AddTask($"[bold]{safeTitle}[/]", maxValue: duration);
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
                    catch(Exception e) 
                    {
                        _logService.LogError(e.Message);
                    }
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
