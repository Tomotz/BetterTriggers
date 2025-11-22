using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using BetterTriggers;
using BetterTriggers.MapBuilder;

namespace BetterTriggers.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            // Initialize BetterTriggers core
            try
            {
                Init.Initialize(isTest: false);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to initialize BetterTriggers: {ex.Message}");
                return 1;
            }

            var rootCommand = new RootCommand("BetterTriggers CLI - Build Warcraft III maps from trigger projects");

            var buildCommand = new Command("build", "Build a map from a .w3x file")
            {
                CreateMapFileArgument(),
                CreateMapNameArgument(),
                CreateOutputDirArgument(),
                CreateLuaDirArgument(),
                CreateProtectedOption(),
                CreateProjectDirOption()
            };

            buildCommand.SetHandler(BuildCommandHandler);

            rootCommand.AddCommand(buildCommand);

            // Also support legacy positional arguments for backward compatibility
            // Format: BetterTriggers.CLI.exe <map_file> <map_name> <output_dir> <lua_dir> [y/n] [project_dir]
            if (args.Length >= 4 && !args[0].StartsWith("-"))
            {
                return HandleLegacyArguments(args);
            }

            return rootCommand.Invoke(args);
        }

        private static Argument<string> CreateMapFileArgument()
        {
            var arg = new Argument<string>(
                name: "map-file",
                description: "Path to the .w3x map file to build from");
            arg.AddAlias("map");
            return arg;
        }

        private static Argument<string> CreateMapNameArgument()
        {
            return new Argument<string>(
                name: "map-name",
                description: "Name for the output map file (without extension)");
        }

        private static Argument<string> CreateOutputDirArgument()
        {
            return new Argument<string>(
                name: "output-dir",
                description: "Directory where the built map will be saved");
        }

        private static Argument<string> CreateLuaDirArgument()
        {
            return new Argument<string>(
                name: "lua-dir",
                description: "Directory containing Lua source files to include");
        }

        private static Option<bool> CreateProtectedOption()
        {
            var option = new Option<bool>(
                name: "--protected",
                description: "Enable map protection (obfuscation, compression)",
                getDefaultValue: () => true);
            option.AddAlias("-p");
            return option;
        }

        private static Option<string?> CreateProjectDirOption()
        {
            var option = new Option<string?>(
                name: "--project-dir",
                description: "Base directory for the BetterTriggers project (defaults to Documents/Warcraft III/BetterTriggers)",
                getDefaultValue: () => null);
            option.AddAlias("-d");
            return option;
        }

        private static void BuildCommandHandler(InvocationContext context)
        {
            string mapFile = context.ParseResult.GetValueForArgument(CreateMapFileArgument());
            string mapName = context.ParseResult.GetValueForArgument(CreateMapNameArgument());
            string outputDir = context.ParseResult.GetValueForArgument(CreateOutputDirArgument());
            string luaDir = context.ParseResult.GetValueForArgument(CreateLuaDirArgument());
            bool isProtected = context.ParseResult.GetValueForOption(CreateProtectedOption());
            string? projectDir = context.ParseResult.GetValueForOption(CreateProjectDirOption());

            int exitCode = BuildMap(mapFile, mapName, outputDir, luaDir, isProtected, projectDir);
            context.ExitCode = exitCode;
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
