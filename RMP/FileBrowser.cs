using RMP.Services;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WMPLib;

namespace RMP
{
    public class FileBrowser
    {
        public LogService _logService { get; set; }
        public FileBrowser(LogService logService) { _logService = logService; }

        public void ShowBrowse()
        {
            WindowsMediaPlayer player = null;

            var theme = new ThemeChanger();
            var primaryColorName = theme.GetPrimaryColorName();
            var primaryColor = theme.GetPrimaryColor();

            bool Browser = true;
            try
            {
                while (Browser)
                {
                    // Load songs
                    var musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                    var songFiles = Directory.GetFiles(musicFolder, "*.mp3");

                    if (songFiles.Length == 0)
                    {
                        AnsiConsole.MarkupLine("[red]No MP3 files found in Music folder![/]");
                        return;
                    }

                    // Prepare table-like choices
                    var choices = new List<string>();
                    var songMap = new Dictionary<string, string>();

                    foreach (var file in songFiles)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(file);

                        string artist = "Unknown";
                        string title = fileName;

                        // Parse filenames like "Artist - Song"
                        if (fileName.Contains(" - "))
                        {
                            var parts = fileName.Split(" - ", 2, StringSplitOptions.TrimEntries);
                            artist = parts[0];
                            title = parts[1];
                        }

                        // Format: fixed-width columns
                        string row = $"{artist,-30} | {title}";

                        string escaped = Markup.Escape(row);

                        choices.Add(escaped);
                        songMap[escaped] = file;
                    }

                    AnsiConsole.Clear();

                    const string BackOption = "Go back to menu";
                    choices.Insert(0, BackOption);

                    // Display interactive "table"
                    var selectedEscaped = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title($"Select a song:")
                            .PageSize(15)
                            .MoreChoicesText("(Use ↑↓ to navigate)")
                            .HighlightStyle(new Style(primaryColor))
                            .AddChoices(choices)
                    );

                    if (selectedEscaped == BackOption)
                    {
                        Browser = false;
                        break;
                    }


                    if (selectedEscaped == "Go back to menu")
                    {
                        Browser = false;
                    }

                    // Get real path
                    string selectedPath = songMap[selectedEscaped];

                    player = new WindowsMediaPlayer();
                    player.URL = selectedPath;

                    try
                    {
                        int vol = (int)Math.Clamp(Settings.Current.Volume * 100f, 0f, 100f);
                        SafeCall(() => player.settings.volume = vol);
                    }
                    catch(Exception ex) 
                    {
                        _logService.LogWarning(ex.Message);
                    }

                    AnsiConsole.MarkupLine($"[{primaryColorName}]Now playing:[/] [rapidblink]{selectedEscaped}[/]");
                    AnsiConsole.MarkupLine($"[grey]Full path:[/] {Markup.Escape(selectedPath)}");
                    AnsiConsole.Markup($"\nUse [{primaryColorName}]↑↓[/] to change volume");

                    int waitCount = 0;
                    double duration = 0;
                    while (waitCount < 30)
                    {
                        duration = SafeGetDouble(() => player.currentMedia?.duration ?? 0);
                        if (duration > 0) break;
                        Thread.Sleep(100);
                        waitCount++;
                    }

                    if (duration <= 0) duration = 100; // fallback

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
                        new SpinnerColumn(Spinner.Known.Dots2) { Style = new Style(theme.GetPrimaryColor()) }
                        })
                        .Start(ctx =>
                        {
                            player.controls.play();
                            var task = ctx.AddTask($"[bold]{selectedEscaped}[/]", maxValue: duration);


                            // Continue?
                            AnsiConsole.Markup($"\nPress [{primaryColorName}]ENTER[/] to pick another song");


                            while (!ctx.IsFinished)
                            {
                                double position = SafeGetDouble(() => player.controls.currentPosition);

                                if (position < 0) position = 0;
                                if (position > duration) position = duration;

                                task.Value = position;

                                if (position >= duration || SafeGetInt(() => (int)player.playState) == (int)WMPPlayState.wmppsStopped)
                                {
                                    Thread.Sleep(100);
                                    break;
                                }

                                if (Console.KeyAvailable)
                                {
                                    var key = Console.ReadKey(true).Key;
                                    switch (key)
                                    {
                                        case ConsoleKey.Enter:
                                            player.controls.stop();
                                            break;

                                        case ConsoleKey.UpArrow:
                                            player.settings.volume = Math.Min(100, player.settings.volume + 5);
                                            player.settings.volume = player.settings.volume;
                                            break;

                                        case ConsoleKey.DownArrow:
                                            player.settings.volume = Math.Max(0, player.settings.volume - 5);
                                            player.settings.volume = player.settings.volume;
                                            break;

                                    }
                                }
                            }
                        });
                }
            }
            finally
            {

                if (player != null)
                {
                    try
                    {
                        player.controls.stop();
                        Marshal.ReleaseComObject(player);
                    }
                    catch (Exception ex)
                    { 
                        _logService.LogError($"FileBrowser releasing player failed {ex}"); 
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

