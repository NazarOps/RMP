namespace RMP;

public class MusicScanner
{
    private readonly string _musicFolderPath;
    private readonly HashSet<string> _supportedExtensions;

    public List<string> MusicFiles { get; private set; }

    public int Count => MusicFiles.Count;
    public MusicScanner(string? musicFolderPath = null)
    {
        _musicFolderPath = musicFolderPath ?? Path.Combine(Directory.GetCurrentDirectory(), "music");
        _supportedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".mp3",
            ".flac",
            ".wav",
            ".m4a"
        };
        MusicFiles = new List<string>();

        EnsureMusicFolderExists();
    }

    public List<string> Scan()
    {
        MusicFiles.Clear();

        if (!Directory.Exists(_musicFolderPath))
        {
            return MusicFiles;
        }

        try
        {
            var allFiles = Directory.GetFiles(_musicFolderPath, "*.*", SearchOption.AllDirectories);

            foreach (var file in allFiles)
            {
                var extension = Path.GetExtension(file);
                if (_supportedExtensions.Contains(extension))
                {
                    MusicFiles.Add(file);
                }
            }
        }
        catch (UnauthorizedAccessException)
        {

        }
        catch (DirectoryNotFoundException)
        {

        }

        return MusicFiles;
    }


    public string GetMusicFolderPath() => _musicFolderPath;

    private void EnsureMusicFolderExists()
    {
        if (!Directory.Exists(_musicFolderPath))
        {
            Directory.CreateDirectory(_musicFolderPath);
        }
    }
}
