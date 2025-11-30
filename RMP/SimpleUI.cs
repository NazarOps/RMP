using RMP.Services;
using Spectre.Console;
using System.Text;
using WMPLib;

namespace RMP;

public class SimpleUI
{
    public LogService LogService { get; set; }
    public SimpleUI(LogService logService) { LogService = logService; }

    private int songindex = 0;
    public void Run()
    {
        Console.OutputEncoding = Encoding.UTF8; // for SpectreConsole spinners support
        bool menu = true;

        var theme = new ThemeChanger();
        var primaryColorName = theme.GetPrimaryColorName();
        
        AnsiConsole.Progress()
            .Columns(new ProgressColumn[]
    {
        new TaskDescriptionColumn(),
        new ProgressBarColumn
        {
            FinishedStyle = new Style(theme.GetPrimaryColor()),
            RemainingStyle = new Style(Color.White),
            CompletedStyle = new Style(theme.GetPrimaryColor())
        },
        new SpinnerColumn
        {
            Style = new Style(theme.GetPrimaryColor())
        }
    })

    .Start(ctx =>
    {
        // Define tasks
        var task1 = ctx.AddTask($"[{primaryColorName}]Loading Runtime Music Player[/]");

        while (!ctx.IsFinished)
        {
            task1.Increment(1.5);
            Thread.Sleep(100);
        }
    });

        // Splash screen
        Console.Clear();

        AnsiConsole.MarkupLine($"[{primaryColorName}]  _____             _   _                  __  __           _        _____  _                       [/]");
        Thread.Sleep(200);

        AnsiConsole.MarkupLine($"[{primaryColorName}] |  __ \\           | | (_)                |  \\/  |         (_)      |  __ \\| |                     [/]");
        Thread.Sleep(200);

        AnsiConsole.MarkupLine($"[{primaryColorName}] | |__) |   _ _ __ | |_ _ _ __ ___   ___  | \\  / |_   _ ___ _  ___  | |__) | | __ _ _   _  ___ _ __ [/]");
        Thread.Sleep(200);

        AnsiConsole.MarkupLine($"[{primaryColorName}] |  _  / | | | '_ \\| __| | '_ ` _ \\ / _ \\ | |\\/| | | | / __| |/ __| |  ___/| |/ _` | | | |/ _ \\ '__|[/]");
        Thread.Sleep(200);

        AnsiConsole.MarkupLine($"[{primaryColorName}] | | \\ \\ |_| | | | | |_| | | | | | |  __/ | |  | | |_| \\__ \\ | (__  | |    | | (_| | |_| |  __/ |   [/]");
        Thread.Sleep(200);

        AnsiConsole.MarkupLine($"[{primaryColorName}] |_|  \\_\\__,_|_| |_|\\__|_|_| |_| |_|\\___| |_|  |_|\\__,_|___/_|\\___| |_|    |_|\\__,_|\\__, |\\___|_|   [/]");
        Thread.Sleep(600);

        AnsiConsole.MarkupLine($"[{primaryColorName}]                                                                                     __/ |          [/]");
        Thread.Sleep(600);

        AnsiConsole.MarkupLine($"[{primaryColorName}]                                                                                    |___/           [/]");
        Thread.Sleep(900);

        while (menu)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("RMP")
                .Color(theme.GetPrimaryColor())
            );

            AnsiConsole.MarkupLine("What would you like to do?");

            var menuItems = new[] { "Play", "Search", "Browse", "Settings", "Exit" };

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .AddChoices(menuItems)
                .HighlightStyle(new Style(theme.GetPrimaryColor()))
                .PageSize(menuItems.Length)
            );

            switch (choice)
            {
                case "Play":
                    MusicPlayback musicPlayback = new MusicPlayback(LogService);
                    musicPlayback.PlayMusic();
                    break;

                case "Search":
                    linq linq = new linq(LogService);
                    linq.Search();
                    break;

                case "Browse":
                    FileBrowser fileBrowser = new FileBrowser(LogService);
                    fileBrowser.ShowBrowse();
                    break;

                case "Settings":
                    Settings settings = new Settings();
                    settings.Adjust();
                    break;

                case "Exit":
                    break;
            }

            if (choice == "Exit")
            {
                Console.Clear();
                AnsiConsole.MarkupLine("Credits: ");
                AnsiConsole.MarkupLine("Made by the Runtime Rebels Team");
                menu = false;
                Thread.Sleep(500);

            }
        }
    }
}