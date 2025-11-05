namespace RMP;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("RMP - Music Player");
        Console.WriteLine("==================\n");

        // Initialize the music scanner
        var scanner = new MusicScanner();

        Console.WriteLine($"Music folder: {scanner.GetMusicFolderPath()}");
        Console.WriteLine("Scanning for music files...\n");

        // Scan for music files
        scanner.Scan();

        // Display results
        Console.WriteLine($"Found {scanner.Count} music file(s) in your library.\n");

        if (scanner.Count > 0)
        {
            Console.WriteLine("Music files:");
            foreach (var file in scanner.MusicFiles)
            {
                Console.WriteLine($"  - {Path.GetFileName(file)}");
            }
        }
        else
        {
            Console.WriteLine("No music files found. Add some music files to the 'music' folder!");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
