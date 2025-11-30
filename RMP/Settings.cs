using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RMP
{
    public class Settings
    {
        private const string FileName = "settings.json";

        // Global settings loaded from JSON
        public static Settings Current { get; private set; } = StateService.Dejsonize<Settings>(FileName);


        // Settings values
        public float Volume { get; set; } = 1.0f;   // 0–1 float
        public bool Repeat { get; set; } = false;
        public bool SettingsMenu = true;
        public string Theme { get; set; } = "Default";


        // -------- JSON LOAD / SAVE --------
        public static Settings Load()
        {
            return StateService.Dejsonize<Settings>(FileName);
        }

        public static void Save()
        {
            StateService.Jsonize(Current, FileName);
        }
        
        // -------- SETTINGS MENU --------

        public void Adjust()
        {
            while (SettingsMenu)
            {
                var theme = new ThemeChanger();
                var primaryColorName = theme.GetPrimaryColorName();

                AnsiConsole.Clear();
                AnsiConsole.Write(
                    new FigletText("Settings")
                    .Color(theme.GetPrimaryColor())
                );

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose an option:")
                        .AddChoices("Adjust volume", "Change Theme", "Exit")
                        .HighlightStyle(theme.GetPrimaryColor())
                );

                switch (choice)
                {
                    case "Adjust volume":
                        AdjustVolume();
                        break;

                    case "Change Theme":
                        ChangeTheme();
                        break;

                    case "Exit":
                        SettingsMenu = false;
                        break;
                }
            }
        }


        private void AdjustVolume()
        {
            var theme = new ThemeChanger();
            var primaryColorName = theme.GetPrimaryColorName();

            bool adjusting = true;

            while (adjusting)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(
                    new FigletText("VOLUME")
                    .Color(theme.GetPrimaryColor()));

                int volumePercent = (int)(Current.Volume * 100);

                AnsiConsole.Write(
                    new BarChart()
                        .Width(40)
                        .CenterLabel()
                        .AddItem("Volume", volumePercent, theme.GetPrimaryColor()));

                AnsiConsole.MarkupLine($"\n[{primaryColorName}]<-[/] decrease   [{primaryColorName}]->[/] increase");
                AnsiConsole.MarkupLine($"Press [{primaryColorName}]ESC[/] to return");

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.RightArrow:
                        volumePercent = Math.Min(100, volumePercent + 5);
                        Current.Volume = volumePercent / 100f;
                        break;

                    case ConsoleKey.LeftArrow:
                        volumePercent = Math.Max(0, volumePercent - 5);
                        Current.Volume = volumePercent / 100f;
                        break;

                    case ConsoleKey.Escape:
                        adjusting = false;
                        Save();
                        break;
                }
            }
        }

        private void ChangeTheme()
        {
            ThemeChanger themeChanger = new ThemeChanger();
            themeChanger.ChangeTheme();
        }
    }
}
