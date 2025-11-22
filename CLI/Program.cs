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

            // Also support legacy positional arguments for backward compatibility
            // Format: BetterTriggers.CLI.exe <map_file> <map_name> <output_dir> <lua_dir> [y/n] [project_dir]
            if (args.Length >= 4 && !args[0].StartsWith("-") && args[0] != "build")
            {
                return HandleLegacyArguments(args);
            }

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

            var protectedOpt = new Option<bool>(
                name: "--protected",
                description: "Enable map protection (obfuscation, compression)",
                getDefaultValue: () => true);
            protectedOpt.AddAlias("-p");

            var projectDirOpt = new Option<string?>(
                name: "--project-dir",
                description: "Base directory for the BetterTriggers project (defaults to Documents/Warcraft III/BetterTriggers)",
                getDefaultValue: () => null);
            projectDirOpt.AddAlias("-d");

            var buildCommand = new Command("build", "Build a map from a .w3x file")
            {
                mapFileArg,
                mapNameArg,
                outputDirArg,
                luaDirArg,
                protectedOpt,
                projectDirOpt
            };

            buildCommand.SetHandler(
                (string mapFile, string mapName, string outputDir, string luaDir, bool isProtected, string? projectDir) =>
                {
                    int exitCode = BuildMap(mapFile, mapName, outputDir, luaDir, isProtected, projectDir);
                    Environment.Exit(exitCode);
                },
                mapFileArg, mapNameArg, outputDirArg, luaDirArg, protectedOpt, projectDirOpt);

            rootCommand.AddCommand(buildCommand);

            return rootCommand.Invoke(args);
        }

        private static int HandleLegacyArguments(string[] args)
        {
            // Legacy format: <map_file> <map_name> <output_dir> <lua_dir> [y/n] [project_dir]
            string mapFile = args[0];
            string mapName = args[1];
            string outputDir = args[2];
            string luaDir = args[3];
            bool isProtected = true;
            string? projectDir = null;

            if (args.Length >= 5)
            {
                isProtected = args[4].ToLower() == "y";
            }

            if (args.Length >= 6)
            {
                projectDir = args[5];
            }

            return BuildMap(mapFile, mapName, outputDir, luaDir, isProtected, projectDir);
        }

        private static int BuildMap(string mapFile, string mapName, string outputDir, string luaDir, bool isProtected, string? projectDir)
        {
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
