using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using War3Net.Build.Info;
using War3Net.Build;
using War3Net.IO.Mpq;
using JassObfuscator;
using War3Net.IO.Compression;
using BetterTriggers.WorldEdit.GameDataReader;
using System.Windows;
using System.Text.RegularExpressions;
using System.Collections;
using ICSharpCode.Decompiler.Metadata;
using BetterTriggers.WorldEdit;

namespace BetterTriggers.TestMap
{
    public enum BuildMapStatusCode
    {
        Ok,
        Exception,
        CouldNotWriteToFile,
        ScriptError,
    }

    public class BuildMapStatus
    {
        public BuildMapStatusCode Status;
        public string Message;

        public BuildMapStatus(BuildMapStatusCode status, string message)
        {
            this.Status = status;
            this.Message = message;
        }
    }

    public class Builder
    {
        private ScriptLanguage _language;

        public Builder()
        {
            War3Project project = Project.CurrentProject.war3project;
            _language = project.Language == "lua" ? ScriptLanguage.Lua : ScriptLanguage.Jass;
        }

        public (bool, string) GenerateScript()
        {
            War3Project project = Project.CurrentProject.war3project;
            if (project == null)
                return (false, null);

            ScriptGenerator scriptGenerator = new ScriptGenerator(_language);
            bool success = scriptGenerator.GenerateScript();

            return (success, scriptGenerator.GeneratedScript);
        }

        //private string remove_comments(string luaScript)
        //{
        //    // Split the script into individual lines
        //    string[] lines = luaScript.Split('\n');
        //    string[] outLines = [];
        //    bool is_in_multiline_comment = false;
        //    bool is_in_multiline_string = false;
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        string line = lines[i];
        //        if (is_in_multiline_comment || is_in_multiline_string)
        //        {
        //            int index = line.IndexOf("]]");
        //            if (index == -1)
        //            {
        //                outLines.Append("");
        //                continue;
        //            }
        //            if (is_in_multiline_comment)
        //            {

        //                line = line.Substring(index + 2);
        //            }
        //            is_in_multiline_comment = false;
        //            is_in_multiline_string = false;
        //        }
        //        int commentIdx = line.IndexOf("--");
        //        int multStrIdx = line.IndexOf("[[");
        //        if (commentIdx == -1 && multStrIdx == -1)
        //        {
        //            // No comments in the line. Just append it
        //            outLines.Append(line);
        //            continue;
        //        }
        //        string lineStart = multStrIdx == -1 && "" || line.Substring(0, multStrIdx);
        //        string StringlessLine

        //        // Skip lines containing " or '. We miss lines with strings and comments, but I don't care enough to fix this
        //        if (line.Contains("\"") || line.Contains("'"))
        //        {
        //            outLines.Append(line);
        //            continue;
        //        }

        //        // Remove single-line comments
        //        line = Regex.Replace(line, @"--[^\r\n]*", string.Empty);

        //        // Update the line back into the array
        //        lines[i] = line;
        //    }

        //    // Rejoin the lines into a single string
        //    return string.Join("\n", lines);
        //}
            //// Regular expression to match Lua single-line comments starting with --
            //string pattern = @"--[^\r\n]*";

            //// Replace comments with empty space, preserving line breaks
            //return Regex.Replace(orig, pattern, string.Empty, RegexOptions.Multiline);

            //// Pattern for Lua multiline comments: --[[ ... ]]
            //string multilinePattern = @"--\[\[(.*?)\]\]";

            //// Pattern for Lua single-line comments: --
            //string singleLinePattern = @"--[^\r\n]*";

            //// Remove multiline comments while preserving line breaks
            //luaScript = Regex.Replace(luaScript, multilinePattern, match =>
            //{
            //    // Preserve new lines within multiline comments
            //    string comment = match.Groups[1].Value;
            //    return new string('\n', comment.Split('\n').Length - 1);
            //}, RegexOptions.Singleline);

            //// Remove single-line comments
            //luaScript = Regex.Replace(luaScript, singleLinePattern, string.Empty, RegexOptions.Multiline);

        //    return luaScript;
        //}

        public string archivePath;
        /// <summary>
        /// Builds an MPQ archive.
        /// Throws <see cref="Exception"/> and <see cref="ContainsBTDataException"/> on errors.
        /// </summary>
        public BuildMapStatus BuildMap(string destinationDir = null, bool includeMPQSettings = false, bool isTest = false, string mapName = "")
        {
            EditorSettings settings = EditorSettings.Load();
            (bool wasVerified, string script) = GenerateScript();
            if (!wasVerified)
            {
                return new BuildMapStatus(BuildMapStatusCode.ScriptError, "Could not compile script.");
            }

            if (includeMPQSettings)
            {
                if (settings.Export_Obfuscate)
                {
                    if (_language == ScriptLanguage.Jass)
                    {
                        string commonJ = ScriptGenerator.PathCommonJ;
                        string blizzardJ = ScriptGenerator.PathBlizzardJ;
                        script = Obfuscator.Obfuscate(script, commonJ, blizzardJ);
                    } else
                    {
                        //script = remove_comments(script);
                    }
                }
            }

            string mapDir = Project.CurrentProject.GetFullMapPath();
            var map = Map.Open(mapDir);

            /// We overwrite the loaded doodads with those modified by BT.
            /// The reason being the <see cref="ScriptGenerator"/> will modify the loaded destructibles' state flag,
            /// when a reference to the destructible has been made in GUI.
            /// This is important for the creation of destructibles.
            if (map.Doodads != null)
            {
                int count = map.Doodads.Doodads.Count;
                for (int i = 0; i < count; i++)
                {
                    map.Doodads.Doodads.RemoveAt(0);
                }
                map.Doodads.Doodads.AddRange(Destructibles.GetAllDoodads());
            }


            map.Info.ScriptLanguage = _language;
            map.Script = script;
            if (mapName != "")
                map.TriggerStrings.Strings[0].Value = mapName;

            if (settings.Export_IncludeTriggerData && isTest == false)
            {
                var bt2we = new BT2WE(map);
                bt2we.Convert();
            }


            // We need to add all arbitrary files into to the builder.
            MapBuilder builder = new MapBuilder(map);
            if (Directory.Exists(mapDir))
                builder.AddFiles(mapDir, "*", SearchOption.AllDirectories);
            else
            {
                var mpqArchive = MpqArchive.Open(mapDir, true);
                builder.AddFiles(mpqArchive);
            }

            // MPQ protection
            if (includeMPQSettings && isTest == false)
            {
                if (settings.Export_RemoveTriggerData)
                {
                    map.Triggers = null;
                }
            }

            ushort blockSize = 3;
            if (settings.Export_Compress && isTest == false)
                blockSize = 8;
            if (settings.Export_Compress && settings.Export_Compress_Advanced && isTest == false)
                blockSize = settings.Export_Compress_BlockSize;

            var archiveCreateOptions = new MpqArchiveCreateOptions
            {
                ListFileCreateMode = MpqFileCreateMode.Overwrite,
                AttributesCreateMode = MpqFileCreateMode.Prune,
                BlockSize = blockSize,
            };

            string src = Path.GetDirectoryName(Project.CurrentProject.src);
            if (destinationDir == null)
                archivePath = Path.Combine(src, Path.Combine("dist", Path.GetFileName(mapDir)));
            else
            {
                archivePath = Path.Combine(destinationDir, settings.CopyLocation + ".w3x");
            }

            bool didWrite = false;
            int attemptLimit = 10;
            Exception err = new Exception();
            while (attemptLimit > 0 && !didWrite)
            {
                try
                {
                    builder.Build(archivePath, archiveCreateOptions);
                    didWrite = true;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(5);
                    attemptLimit--;
                    if (attemptLimit <= 0)
                    {
                        err = ex;
                    }
                }
            }
            if (!didWrite)
            {
                return new BuildMapStatus(BuildMapStatusCode.CouldNotWriteToFile, err.Message);
            }

            if (includeMPQSettings && isTest == false)
            {
                if (settings.Export_RemoveListfile)
                {
                    string tempFile = Path.Combine(GetProgramTempPath(), "tempfile.w3x");
                    if (File.Exists(tempFile))
                        File.Delete(tempFile);

                    IEnumerable<MpqFile> mpqFiles = null;
                    using (var stream = new FileStream(archivePath, FileMode.Open))
                    {
                        File.Copy(archivePath, tempFile);
                        using (var stream2 = new FileStream(tempFile, FileMode.Open))
                        {
                            var archive = MpqArchive.Open(stream2, false);
                            mpqFiles = archive.GetMpqFiles();
                        }
                    }
                    File.Delete(archivePath);
                    using (var s = new FileStream(archivePath, FileMode.Create))
                    {
                        MpqArchive.Create(s, mpqFiles, archiveCreateOptions);
                    }
                }
            }

            return new BuildMapStatus(BuildMapStatusCode.Ok, string.Empty);
        }

        public BuildMapStatus TestMap()
        {
            string destinationDir = Path.GetTempPath();
            var status = BuildMap(destinationDir, isTest: true);
            if (status.Status != BuildMapStatusCode.Ok)
            {
                return status;
            }

            EditorSettings settings = EditorSettings.Load();
            string war3Exe = Path.Combine(settings.war3root, "_retail_/x86_64/Warcraft III.exe");
            if (!File.Exists(war3Exe))
            {
                war3Exe = Path.Combine(settings.war3root, "x86_64/Warcraft III.exe");
            }
            if (!File.Exists(war3Exe))
            {
                war3Exe = Path.Combine(settings.war3root, "Warcraft III.exe");
            }

            int difficulty = settings.Difficulty;
            string windowMode;
            switch (settings.WindowMode)
            {
                case 0:
                    windowMode = "windowed";
                    break;
                case 1:
                    windowMode = "windowedfullscreen";
                    break;
                default:
                    windowMode = "fullscreen";
                    break;
            }
            int hd = settings.HD;
            int teen = settings.Teen;
            string playerProfile = settings.PlayerProfile;
            int fixedseed = settings.FixedRandomSeed == true ? 1 : 0;
            string nowfpause = settings.NoWindowsFocusPause == true ? "-nowfpause " : "";

            string launchArgs = string.Empty;
            if (WarcraftStorageReader.GameVersion >= new Version(1, 32))
            {
                launchArgs += "-launch ";
                launchArgs += $"-windowmode {windowMode} ";
                launchArgs += $"-hd {hd} ";
                launchArgs += $"-teen {teen} ";
                launchArgs += $"{nowfpause}";
                launchArgs += $"-mapdiff {difficulty} ";
                launchArgs += $"-testmapprofile {playerProfile} ";
                launchArgs += $"-fixedseed {fixedseed} ";
            }
            else if (WarcraftStorageReader.GameVersion >= new Version(1, 31))
            {
                launchArgs += $"-windowmode {windowMode} ";
                launchArgs += nowfpause;
            }
            else
            {
                switch (settings.WindowMode)
                {
                    case 0:
                        launchArgs += "-window ";
                        break;
                    case 1:
                        launchArgs += "-fullscreen ";
                        break;
                    default:
                        launchArgs += "-nativefullscr ";
                        break;
                }

                launchArgs += nowfpause;
            }

            Process.Start($"\"{war3Exe}\" {launchArgs} -loadfile \"{archivePath}\"");
            return status;
        }


        private string GetProgramTempPath()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "BetterTriggers");
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            return tempPath;
        }
    }
}
