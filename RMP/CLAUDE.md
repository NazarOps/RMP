# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET 8.0 C# console application named "RMP". The project uses implicit usings and nullable reference types are enabled.

## Build and Run Commands

```bash
# Build the project
dotnet build

# Run the application
dotnet run

# Clean build artifacts
dotnet clean

# Build in Release mode
dotnet build -c Release

# Run in Release mode
dotnet run -c Release
```

## Project Structure

- `Program.cs` - Main entry point containing the `Program` class with `Main` method
- `RMP.csproj` - Project file targeting .NET 8.0
- `bin/` - Build output directory (generated)
- `obj/` - Intermediate build files (generated)

## Development Notes

- Target Framework: .NET 8.0
- Implicit Usings: Enabled (common namespaces are automatically imported)
- Nullable Reference Types: Enabled (requires explicit handling of null values)
- Output Type: Console application (.exe)
