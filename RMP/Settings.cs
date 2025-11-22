using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMP
{
    public class Settings
    {
        public int Volume { get; set; } = 50;
        public string MusicFolder { get; set; } =
            Path.Combine(Environment.CurrentDirectory, "Music");

        public string LogFolder { get; set; } =
            Path.Combine(Environment.CurrentDirectory, "logs");

        public string LastPlayedSong { get; set; } = "";
        public double LastPosition { get; set; } = 0;
    }
}
