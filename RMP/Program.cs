using System;
using System.IO;
using RMP.Services;

namespace RMP;

class Program
{
    private static void Main(string[] args)
    {
        var ui = new SimpleUI();
        ui.Run();
    }
}