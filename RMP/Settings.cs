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
                AnsiConsole.Clear();
                AnsiConsole.Write(
                    new FigletText("Settings")
                    .Color(Color.Blue)
                );

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose an option:")
                        .AddChoices("Adjust volume", "Change Theme", "Exit")
                        .HighlightStyle(new Style(Color.Blue))
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
            bool adjusting = true;

            while (adjusting)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(
                    new FigletText("VOLUME")
                    .Color(Color.Blue));

                int volumePercent = (int)(Current.Volume * 100);

                AnsiConsole.Write(
                    new BarChart()
                        .Width(40)
                        .CenterLabel()
                        .AddItem("Volume", volumePercent, Color.BlueViolet));

                AnsiConsole.MarkupLine("\n[blue]<-[/] decrease   [blue]->[/] increase");
                AnsiConsole.MarkupLine("Press [blue]ESC[/] to return");

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
            // here you can change the color of the text and more
        }
    }
}
