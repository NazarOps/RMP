using Spectre.Console;
using System.Text;
using WMPLib;

namespace RMP;

public class SimpleUI
{
    private int songindex = 0;
    public void Run()
    {
        Console.OutputEncoding = Encoding.UTF8; // for SpectreConsole spinners support
        bool menu = true;

        
        AnsiConsole.Progress()
            .Columns(new ProgressColumn[]
    {
        new TaskDescriptionColumn(),
        new ProgressBarColumn
        {
            FinishedStyle = new Style(Color.Blue),
            RemainingStyle = new Style(Color.White),
            CompletedStyle = new Style(Color.Blue)
        },
        new SpinnerColumn
        {
            Style = new Style(Color.Blue)
        }
    })

    .Start(ctx =>
    {
        // Define tasks
        var task1 = ctx.AddTask("[blue]Loading Runtime Music Player[/]");

        while (!ctx.IsFinished)
        {
            task1.Increment(1.5);
            Thread.Sleep(100);
        }
    });

        // Splash screen
        Console.Clear();

        AnsiConsole.MarkupLine("[blue]  _____             _   _                  __  __           _        _____  _                       [/]");
        Thread.Sleep(200);

        AnsiConsole.MarkupLine("[blue] |  __ \\           | | (_)                |  \\/  |         (_)      |  __ \\| |                     [/]");
        Thread.Sleep(200);

        AnsiConsole.MarkupLine("[deepskyblue1] | |__) |   _ _ __ | |_ _ _ __ ___   ___  | \\  / |_   _ ___ _  ___  | |__) | | __ _ _   _  ___ _ __ [/]");
        Thread.Sleep(200);

        AnsiConsole.MarkupLine("[deepskyblue1] |  _  / | | | '_ \\| __| | '_ ` _ \\ / _ \\ | |\\/| | | | / __| |/ __| |  ___/| |/ _` | | | |/ _ \\ '__|[/]");
        Thread.Sleep(200);

        AnsiConsole.MarkupLine("[deepskyblue1] | | \\ \\ |_| | | | | |_| | | | | | |  __/ | |  | | |_| \\__ \\ | (__  | |    | | (_| | |_| |  __/ |   [/]");
        Thread.Sleep(200);

        AnsiConsole.MarkupLine("[blue] |_|  \\_\\__,_|_| |_|\\__|_|_| |_| |_|\\___| |_|  |_|\\__,_|___/_|\\___| |_|    |_|\\__,_|\\__, |\\___|_|   [/]");
        Thread.Sleep(600);

        AnsiConsole.MarkupLine("[blue]                                                                                     __/ |          [/]");
        Thread.Sleep(600);

        AnsiConsole.MarkupLine("[blue]                                                                                    |___/           [/]");
        Thread.Sleep(900);

        while (menu)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("RMP")
                .Color(Color.Blue)
            );

            AnsiConsole.MarkupLine("What would you like to do?");

            var menuItems = new[] { "Play", "Search", "Browse", "Player Controls", "Exit" };

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .AddChoices(menuItems)
                .HighlightStyle(new Style(Color.Blue))
                .PageSize(menuItems.Length)
            );

            switch (choice)
            {
                case "Play":
                    MusicPlayback musicPlayback = new MusicPlayback();
                    musicPlayback.PlayMusic();
                    break;

                case "Search":
                    linq linq = new linq();
                    linq.Search();
                    break;

                case "Browse":
                    ShowBrowse();
                    break;

                case "Player Controls":
                    ShowPlayerControls();
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

    private void ShowBrowse()
    {
        AnsiConsole.Clear();
        var category = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Browse by:")
                .AddChoices(new[] { "Artists", "Albums", "Genres", "Back" }));

        if (category != "Back")
        {
            AnsiConsole.MarkupLine($"[yellow]Browsing {category}...[/]");
            AnsiConsole.MarkupLine("[dim]Press any key to return...[/]");
            Console.ReadKey();
        }
    }

    private void ShowPlayerControls()
    {
        AnsiConsole.Clear();
        var control = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Player Controls:")
                .AddChoices(new[] { "Play", "Pause", "Next", "Previous", "Back" }));

        if (control != "Back")
        {
            AnsiConsole.MarkupLine($"[green]{control} pressed[/]");
            AnsiConsole.MarkupLine("[dim]Press any key to return...[/]");
            Console.ReadKey();
        }
    }


}