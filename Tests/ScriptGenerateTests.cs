using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using War3Net.Build.Info;

namespace Tests
{
    [TestClass]
    public class ScriptGenerateTests
    {
        static War3Project project;
        static string mapDir;
        static string projectFile;
        static bool success;
        static string failedMsg = "Script generate failed. Project folder kept for inspection.";


        [ClassInitialize]
        public static void Init(TestContext context)
        {
            Console.WriteLine("-----------");
            Console.WriteLine("RUNNING SCRIPT GENERATE TESTS");
            Console.WriteLine("-----------");
            Console.WriteLine("");

            Casc.GameVersion = new Version(1, 35, 1); // hack. We need the newest version to load our custom frame definition script.
            BetterTriggers.Init.Initialize(true);


            string[] testMaps = Directory.GetDirectories(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/"));
            foreach(var folder in testMaps)
            {
                if (!folder.EndsWith(".w3x") && !folder.EndsWith(".w3m"))
                    Directory.Delete(folder, true);
            }
        }

        [ClassCleanup]
        public static void Shutdown()
        {
        }

        [TestInitialize]
        public void BeforeEach()
        {
            success = false;
        }

        [TestCleanup]
        public void AfterEach()
        {
            ControllerProject controller = new ControllerProject();
            controller.CloseProject();
            string projectDir = Path.GetDirectoryName(projectFile);
            if (success && Directory.Exists(projectDir))
                Directory.Delete(projectDir, true);
        }


        [TestMethod]
        public void ConvertMap_GenerateScript_DirectStrikeReforged()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Direct_Strike_Reforged_Open_Source.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_DirectStrikeReforged_133()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Direct_Strike_Reforged_Open_Source_1.33.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_DirectStrikeReforged_134()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Direct_Strike_Reforged_Open_Source_1.34.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_Enfo_FFB()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Enfo FFB - v2.42d.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_MZA()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/MZA 2.41.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_AAE_REBORN()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/AAE REBORN V0.5b (beta) UNPROTECTED.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_Skeletal_Annihilation()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Skeletal Annihilation 3.075.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_WoW_Dungeons_Classic()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/WoW Dungeons Classic 3.9.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_Holy_War_Anniversary()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/HolyWarAnniversary1_32a.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_The_Legend_of_Ergl_131()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/The Legend of Ergl_1.31.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_Sheol_131()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Sheol_131.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_empty_131()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/empty_131.w3m");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_empty_132()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/empty_132.w3m");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void GenerateScript_CustomProject_LocalVarMap()
        {
            string projectDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Projects/LocalVarMap/LocalVarMap.json");
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Projects/LocalVarMap/map/Map.w3x");
            ControllerProject controllerProject = new ControllerProject();
            CustomMapData.Init(mapDir, true); // TODO: CustomMapData init should be run by the controller.
            CustomMapData.Load();
            controllerProject.LoadProject(projectDir);
            success = controllerProject.GenerateScript();

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void GenerateScript_CustomProject_FramesMap()
        {
            string projectDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Projects/Frames_Map/Frames_Map.json");
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Projects/Frames_Map/map/Map.w3x");
            ControllerProject controllerProject = new ControllerProject();
            CustomMapData.Init(mapDir, true); // TODO: CustomMapData init should be run by the controller.
            CustomMapData.Load();
            controllerProject.LoadProject(projectDir);
            success = controllerProject.GenerateScript();

            Assert.IsTrue(success, failedMsg);
        }



        void ConvertMap_GenerateScript(string mapDir)
        {
            TriggerConverter triggerConverter = new TriggerConverter();
            projectFile = triggerConverter.Convert(mapDir, Path.Combine(Path.GetDirectoryName(mapDir), Path.GetFileNameWithoutExtension(mapDir)));
            ControllerProject controllerProject = new ControllerProject();

            string projectFileContent = File.ReadAllText(projectFile);
            project = JsonConvert.DeserializeObject<War3Project>(projectFileContent);
            project.War3MapDirectory = mapDir;
            File.WriteAllText(projectFile, JsonConvert.SerializeObject(project));

            project = controllerProject.LoadProject(projectFile);
            CustomMapData.Init(mapDir, true); // TODO: CustomMapData init should be run by the controller.
            CustomMapData.Load();
            success = controllerProject.GenerateScript();
        }
    }
}
