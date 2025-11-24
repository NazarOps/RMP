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
    public class Browser
    {
        public void ShowBrowse()
        {
            WindowsMediaPlayer player = null;
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
                            .Title("[Blue]Select a song:[/]")
                            .PageSize(15)
                            .MoreChoicesText("[grey](Use ↑↓ to navigate)[/]")
                            .HighlightStyle("blue bold")
                            .AddChoices(choices)
                    );

                    if (selectedEscaped == BackOption)
                    {
                        Browser = false;
                        break;
                    }

            
                    if(selectedEscaped == "Go back to menu")
                    {
                        Browser = false;
                    }

                    // Get real path
                    string selectedPath = songMap[selectedEscaped];

                    player = new WindowsMediaPlayer();
                    player.URL = selectedPath;

                    AnsiConsole.MarkupLine($"[blue]Now playing:[/] [rapidblink]{selectedEscaped}[/]");
                    AnsiConsole.MarkupLine($"[grey]Full path:[/] {Markup.Escape(selectedPath)}");

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
                            CompletedStyle = new Style(Color.Blue)
                        },
                        new PercentageColumn(),
                        new SpinnerColumn(Spinner.Known.Dots2) { Style = new Style(Color.Blue) }
                        })
                        .Start(ctx =>
                        {
                            player.controls.play();
                            var task = ctx.AddTask($"[bold]{selectedEscaped}[/]", maxValue: duration);
                            

                            // Continue?
                            AnsiConsole.Markup("\nPress [blue]ENTER[/] to pick another song");

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

