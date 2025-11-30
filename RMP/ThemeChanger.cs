using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

namespace RMP
{
    public class ThemeChanger
    {
        public void ChangeTheme()
        {
            var theme = new ThemeChanger();
            var primaryColorName = theme.GetPrimaryColorName();
            var primaryColor = theme.GetPrimaryColor();

            AnsiConsole.Clear();

            new FigletText("THEME CHANGER");
            var themes = new List<string> { "Dark", "Purple", "Blue", "Green", "Yellow", "Red" };

            var selectedTheme = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Select a theme:")
            .AddChoices(themes)
            .HighlightStyle(new Style(primaryColor)));

            Settings.Current.Theme = selectedTheme;
            Settings.Save();
        }

        public Color GetPrimaryColor()
        {
            return Settings.Current.Theme switch
            {
                "Dark" => Color.Grey,
                "Purple" => Color.Purple,
                "Blue" => Color.Blue,
                "Green" => Color.Green,
                "Yellow" => Color.Yellow,
                "Red" => Color.Red,
                _ => Color.Cyan1 // Default
            };
        }

        public string GetPrimaryColorName()
        {
            return GetPrimaryColor().ToString();
        }
    }
}
