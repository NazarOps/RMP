using System;
using System.IO;
using RMP.Services;

namespace RMP;

class Program
{
    private static void Main(string[] args)
    {
        LogService logService = new LogService();
        var ui = new SimpleUI(logService);
        ui.Run();
    }
}