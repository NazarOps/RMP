using Spectre.Console;

namespace RMP;

public class SimpleUI
{
    public void Run()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("RMP").Color(Color.Blue));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .AddChoices(new[] {
                        "Search",
                        "Browse",
                        "Player Controls",
                        "Exit"
                    }));

            switch (choice)
            {
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
                    return;
            }
        }
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
