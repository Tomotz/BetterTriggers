using BetterTriggers.Containers;
using BetterTriggers.WorldEdit;
using BetterTriggers.TestMap;
using System;
using System.IO;

namespace BetterTriggers
{
    /// <summary>
    /// Shared service for building maps from BetterTriggers projects.
    /// Can be used by both GUI and CLI.
    /// </summary>
    public class MapBuildService
    {
        public class BuildOptions
        {
            public string MapFilePath { get; set; } = string.Empty;
            public string OutputMapName { get; set; } = string.Empty;
            public string OutputDirectory { get; set; } = string.Empty;
            public string LuaSourceDirectory { get; set; } = string.Empty;
            public bool IsProtected { get; set; } = true;
            public string? ProjectDirectory { get; set; }
        }

        public class BuildResult
        {
            public bool Success { get; set; }
            public string? ErrorMessage { get; set; }
            public string? ProjectPath { get; set; }
            public string? OutputMapPath { get; set; }
        }

        /// <summary>
        /// Builds a map from a .w3x file by converting it to a BetterTriggers project and building.
        /// </summary>
        public static BuildResult BuildFromMap(BuildOptions options)
        {
            // Validate inputs
            if (!File.Exists(options.MapFilePath))
            {
                return new BuildResult
                {
                    Success = false,
                    ErrorMessage = $"Map file not found: {options.MapFilePath}"
                };
            }

            if (!Directory.Exists(options.LuaSourceDirectory))
            {
                return new BuildResult
                {
                    Success = false,
                    ErrorMessage = $"Lua source directory not found: {options.LuaSourceDirectory}"
                };
            }

            // Determine project directory
            string projectDir = options.ProjectDirectory ?? 
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                "Warcraft III", "BetterTriggers");

            if (!Directory.Exists(projectDir))
            {
                Directory.CreateDirectory(projectDir);
            }

            // Create full path for the project (project directory + map name)
            string mapNameWithoutExtension = Path.GetFileNameWithoutExtension(options.MapFilePath);
            string fullProjectPath = Path.Combine(projectDir, mapNameWithoutExtension);

            // Delete existing project directory if it exists (matching GUI behavior)
            if (Directory.Exists(fullProjectPath))
            {
                Directory.Delete(fullProjectPath, true);
            }

            // Convert .w3x to BetterTriggers project
            TriggerConverter converter = new TriggerConverter(options.MapFilePath);
            string projectPath = converter.Convert(fullProjectPath);

            // Load the project
            Project project = Project.Load(projectPath);

            // Copy lua files (this is handled in Project.Load via Environment.GetCommandLineArgs,
            // but we need to do it explicitly here for CLI)
            string projectSrc = Path.Combine(Path.GetDirectoryName(projectPath)!, "src");
            CopyLuaFiles(projectSrc, options.LuaSourceDirectory);

            // Configure build settings
            EditorSettings settings = EditorSettings.Load();
            settings.Export_IncludeTriggerData = !options.IsProtected;
            settings.Export_Compress = options.IsProtected;
            settings.Export_Compress_Advanced = false;
            settings.Export_Compress_BlockSize = 3;
            settings.Export_RemoveListfile = options.IsProtected;
            settings.Export_RemoveTriggerData = options.IsProtected;
            settings.Export_Obfuscate = options.IsProtected;

            // Determine output path
            string outputPath = Path.Combine(options.OutputDirectory, options.OutputMapName);
            settings.CopyLocation = outputPath;

            // Build the map
            Builder builder = new Builder();
            string mapNameColored = "|c00750508" + options.OutputMapName + "|r";
            var status = builder.BuildMap(includeMPQSettings: true, destinationDir: "", mapName: mapNameColored);

            if (status.Status == BuildMapStatusCode.ScriptError)
            {
                return new BuildResult
                {
                    Success = false,
                    ErrorMessage = $"Script error: {status.Message}",
                    ProjectPath = projectPath
                };
            }

            if (status.Status != BuildMapStatusCode.Ok)
            {
                return new BuildResult
                {
                    Success = false,
                    ErrorMessage = $"Build failed: {status.Message}",
                    ProjectPath = projectPath
                };
            }

            return new BuildResult
            {
                Success = true,
                ProjectPath = projectPath,
                OutputMapPath = outputPath + ".w3x"
            };
        }

        private static void CopyLuaFiles(string projectSrc, string luaSrc)
        {
            string dstDir = Path.Combine(projectSrc, "Triggers");

            foreach (string filePath in Directory.EnumerateFiles(luaSrc, "*.*", SearchOption.AllDirectories))
            {
                string fileName = Path.GetFileName(filePath);
                if (filePath.Contains("\\Natives\\"))
                {
                    continue;
                }
                string destinationPath = Path.Combine(dstDir, fileName);
                File.Copy(filePath, destinationPath, overwrite: true);
            }
        }
    }
}
