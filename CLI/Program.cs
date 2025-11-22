using System;
using System.CommandLine;
using System.IO;
using BetterTriggers;
using BetterTriggers.WorldEdit.GameDataReader;

namespace BetterTriggers.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand("BetterTriggers CLI - Build Warcraft III maps from trigger projects");

            var mapFileArg = new Argument<string>(
                name: "map-file",
                description: "Path to the .w3x map file to build from");

            var mapNameArg = new Argument<string>(
                name: "map-name",
                description: "Name for the output map file (without extension)");

            var outputDirArg = new Argument<string>(
                name: "output-dir",
                description: "Directory where the built map will be saved");

            var luaDirArg = new Argument<string>(
                name: "lua-dir",
                description: "Directory containing Lua source files to include");

            var noProtectedOpt = new Option<bool>(
                name: "--no-protected",
                description: "Disable map protection (obfuscation, compression). By default, maps are protected.",
                getDefaultValue: () => false);

            var projectDirOpt = new Option<string?>(
                name: "--project-dir",
                description: "Base directory for the BetterTriggers project (defaults to Documents/Warcraft III/BetterTriggers)",
                getDefaultValue: () => null);
            projectDirOpt.AddAlias("-d");

            rootCommand.AddArgument(mapFileArg);
            rootCommand.AddArgument(mapNameArg);
            rootCommand.AddArgument(outputDirArg);
            rootCommand.AddArgument(luaDirArg);
            rootCommand.AddOption(noProtectedOpt);
            rootCommand.AddOption(projectDirOpt);

            rootCommand.SetHandler(
                (string mapFile, string mapName, string outputDir, string luaDir, bool noProtected, string? projectDir) =>
                {
                    int exitCode = BuildMap(mapFile, mapName, outputDir, luaDir, !noProtected, projectDir);
                    Environment.Exit(exitCode);
                },
                mapFileArg, mapNameArg, outputDirArg, luaDirArg, noProtectedOpt, projectDirOpt);

            return rootCommand.Invoke(args);
        }

        private static int BuildMap(string mapFile, string mapName, string outputDir, string luaDir, bool isProtected, string? projectDir)
        {
            // Initialize BetterTriggers core
            // Load Warcraft III game data storage (CASC or MPQ)
            var (isStorageValid, error) = WarcraftStorageReader.Load();
            if (!isStorageValid)
            {
                Console.Error.WriteLine("Failed to load Warcraft III game data.");
                if (!string.IsNullOrEmpty(error))
                {
                    Console.Error.WriteLine(error);
                }
                Console.Error.WriteLine();
                Console.Error.WriteLine("Please open the Better Triggers GUI to configure the Warcraft III installation path in the settings, then try again.");
                return 1;
            }

            // Initialize BetterTriggers data (trigger definitions, types, locale, etc.)
            Init.Initialize(isTest: false);

            Console.WriteLine("BetterTriggers CLI - Building map...");
            Console.WriteLine($"  Map file: {mapFile}");
            Console.WriteLine($"  Output name: {mapName}");
            Console.WriteLine($"  Output directory: {outputDir}");
            Console.WriteLine($"  Lua directory: {luaDir}");
            Console.WriteLine($"  Protected: {isProtected}");
            if (projectDir != null)
            {
                Console.WriteLine($"  Project directory: {projectDir}");
            }
            Console.WriteLine();

            var options = new MapBuildService.BuildOptions
            {
                MapFilePath = mapFile,
                OutputMapName = mapName,
                OutputDirectory = outputDir,
                LuaSourceDirectory = luaDir,
                IsProtected = isProtected,
                ProjectDirectory = projectDir
            };

            var result = MapBuildService.BuildFromMap(options);

            if (result.Success)
            {
                Console.WriteLine("✓ Build completed successfully!");
                Console.WriteLine($"  Project: {result.ProjectPath}");
                Console.WriteLine($"  Output: {result.OutputMapPath}");
                return 0;
            }
            else
            {
                Console.Error.WriteLine("✗ Build failed!");
                Console.Error.WriteLine($"  Error: {result.ErrorMessage}");
                if (result.ProjectPath != null)
                {
                    Console.Error.WriteLine($"  Project: {result.ProjectPath}");
                }
                return 1;
            }
        }
    }
}
