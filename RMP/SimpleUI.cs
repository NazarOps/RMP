using Spectre.Console;
using WMPLib;

namespace RMP;

public class SimpleUI
{
    public void Run()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("RMP")
                .Color(Color.Blue)
            );

            AnsiConsole.MarkupLine("What would you like to do?");

            //var choice = AnsiConsole.Prompt(
            //    new SelectionPrompt<string>()
            //        .AddChoices(
            //            "Play",
            //            "Search",
            //            "Browse",
            //            "Player Controls",
            //            "Exit"
            //        )
            //        .PageSize(5)
            //);

            var menuItems = new[] { "Play", "Search", "Browse", "Player Controls", "Exit" };

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .AddChoices(menuItems)
                .HighlightStyle(new Style(Color.Yellow))
                .PageSize(menuItems.Length)
            );

            switch (choice)
            {
                case "Play":
                    PlayMusic();
                    break;

                case "Search":
                    ShowSearch();
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

            if(choice == "Exit")
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[slowblink]Goodbye, see you later![/]");
                Thread.Sleep(500);
                break;
            }
        }
    }

    private void PlayMusic()
    {
        AnsiConsole.Clear();

        
        AnsiConsole.MarkupLine("[slowblink]Scanning directory...[/]");
        Thread.Sleep(1000);
        Console.Clear();

        string musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        string[] songs = Directory.GetFiles(musicFolder, "*.mp3");

        Random rnd = new Random();
        string randomSong = songs[rnd.Next(songs.Length)];

        WindowsMediaPlayer music = new WindowsMediaPlayer();
        music.URL = randomSong;
        music.controls.play();
        
        string songName = Path.GetFileNameWithoutExtension(music.URL);
        string safeName = Markup.Escape(songName);

        AnsiConsole.MarkupLine($"[blue]Now playing:[/] {safeName}");

        Thread.Sleep(100);
        double duration = 0;
        int waitCount = 0;
        while ((duration = music.currentMedia?.duration ?? 0) <= 0 && waitCount < 50)
        {
            Thread.Sleep(100);
            waitCount++;
        }

        if(duration <= 0)
        {
            AnsiConsole.MarkupLine("[red]Could not get song duration. Displaying simulated progress...[/]");
            duration = 100; 
        }

        AnsiConsole.Progress()
            .Columns(new ProgressColumn[]
            {
            new TaskDescriptionColumn(),
            new ProgressBarColumn(),
            new ElapsedTimeColumn(),
            new SpinnerColumn()
            })

            .Start(ctx =>
            {
                music.controls.play();
                var task = ctx.AddTask($"[bold]{safeName}[/]", maxValue: duration);

                while (!ctx.IsFinished)
                {
                    if(Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);
                        if(key.Key == ConsoleKey.Escape)
                        {
                            music.controls.stop();
                            break;
                        }
                    }

                    double position = music.controls.currentPosition;

                    if(position >= duration || music.playState == WMPPlayState.wmppsStopped)
                    {
                        task.Value = duration;
                        break;
                    }

                    task.Value = position;
                    Thread.Sleep(200);
                }   
            });
        music.controls.stop();
        Console.Clear();
        Run();
    }

    private void ShowSearch()
    {
        AnsiConsole.Clear();
        var searchTerm = AnsiConsole.Ask<string>("Enter search term:");
        AnsiConsole.MarkupLine($"[yellow]Searching for: {searchTerm}[/]");
        AnsiConsole.MarkupLine("[dim]Press any key to return...[/]");
        Console.ReadKey();
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
