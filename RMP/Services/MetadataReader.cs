using RMP.Interfaces;
using System;
using System.IO;
using TagLib;

namespace RMP.Services
{
    public static class MetadataReader
    {
        public static SongMetadata Read(string filePath, ILogService? log = null)
        {
            try
            {
                var file = TagLib.File.Create(filePath);

                return new SongMetadata
                {
                    FilePath = filePath,
                    Title = file.Tag.Title ?? Path.GetFileNameWithoutExtension(filePath),
                    Artist = file.Tag.FirstPerformer ?? "Okänd artist",
                    Album = file.Tag.Album ?? "Okänt album",
                    Year = (int)file.Tag.Year,
                    Duration = file.Properties.Duration.ToString(@"mm\:ss")
                };
            }
            
            catch (Exception ex)
            {
                log.LogError($"Metadatareader failed for '{filePath}': {ex}");
            }

            {
                return new SongMetadata
                {
                    FilePath = filePath,
                    Title = Path.GetFileNameWithoutExtension(filePath),
                    Artist = "Okänd artist",
                    Album = "Okänt album",
                    Year = 0,
                    Duration = "??:??"
                };
            }
        }
    }
}