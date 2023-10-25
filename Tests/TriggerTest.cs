using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using War3Net.Build.Info;

namespace Tests
{
    [TestClass]
    public class TriggerTest
    {
        static ScriptLanguage language = ScriptLanguage.Jass;
        static string name = "TestProject";
        static string projectPath;
        static War3Project project;
        static string directory = System.IO.Directory.GetCurrentDirectory();

        static ExplorerElementTrigger element1, element2, element3;


        [ClassInitialize]
        public static void Init(TestContext context)
        {
            Console.WriteLine("-----------");
            Console.WriteLine("RUNNING TRIGGER TESTS");
            Console.WriteLine("-----------");
            Console.WriteLine("");
        }

        [ClassCleanup]
        public static void TestClassCleanup()
        {
        }

        [TestInitialize]
        public void BeforeEach()
        {
            if (Directory.Exists(directory + @"/" + name))
                Directory.Delete(directory + @"/" + name, true);
            if(File.Exists(directory + @"/" + name + ".json"))
                File.Delete(directory + @"/" + name + ".json");

            ControllerProject controllerProject = new ControllerProject();
            projectPath = controllerProject.CreateProject(language, name, directory);
            project = controllerProject.LoadProject(projectPath);
            controllerProject.SetEnableFileEvents(false); // TODO: Not ideal for testing, but necessary with current architecture.

            string fullPath = ControllerTrigger.Create();
            controllerProject.OnCreateElement(fullPath); // Force OnCreate 'event'.
            element1 = Triggers.GetLastCreated();

            fullPath = ControllerTrigger.Create();
            controllerProject.OnCreateElement(fullPath);
            element2 = Triggers.GetLastCreated();

            fullPath = ControllerTrigger.Create();
            controllerProject.OnCreateElement(fullPath);
            element3 = Triggers.GetLastCreated();
        }

        [TestCleanup]
        public void AfterEach()
        {
            ControllerProject controller = new ControllerProject();
            controller.CloseProject();
        }


        [TestMethod]
        public void OnCreateTrigger()
        {
            ControllerProject controllerProject = new ControllerProject();
            string fullPath = ControllerTrigger.Create();
            controllerProject.OnCreateElement(fullPath);

            var element = Triggers.GetLastCreated();
            string expectedName = Path.GetFileNameWithoutExtension(fullPath);
            string actualName = element.GetName();

            Assert.AreEqual(expectedName, actualName);
        }

        [TestMethod]
        public void OnPasteTrigger()
        {
            ControllerProject controllerProject = new ControllerProject();
            controllerProject.CopyExplorerElement(element1);
            var element = controllerProject.PasteExplorerElement(element3);

            int expectedTriggerCount = 4;
            int actualTriggerCount = Triggers.Count();

            var expectedParameters = ControllerTrigger.GetParametersFromTrigger(element1);
            var actualParameters = ControllerTrigger.GetParametersFromTrigger(element as ExplorerElementTrigger);
            int expectedParamCount = expectedParameters.Count;
            int actualParamCount = actualParameters.Count;

            Assert.AreEqual(element, Triggers.GetLastCreated());
            Assert.AreEqual(expectedTriggerCount, actualTriggerCount);
            Assert.AreEqual(expectedParamCount, actualParamCount);
        }

        [TestMethod]
        public void OnPrepareExplorerTrigger()
        {
            ControllerProject controllerProject = new ControllerProject();
            LocalVariable localVariable = new LocalVariable();
            ControllerVariable.CreateLocalVariable(element1.trigger, localVariable, element1.trigger.LocalVariables, 0);
            ControllerVariable.CreateLocalVariable(element1.trigger, localVariable, element1.trigger.LocalVariables, 1);
            ControllerVariable.CreateLocalVariable(element1.trigger, localVariable, element1.trigger.LocalVariables, 2);
            controllerProject.CopyExplorerElement(element1);
            var pasted = (ExplorerElementTrigger) controllerProject.PasteExplorerElement(element1);

            for (int i = 0; i < pasted.trigger.LocalVariables.Count; i++)
            {
                var copiedLv = (LocalVariable) element1.trigger.LocalVariables[i];
                var pastedLv = (LocalVariable)pasted.trigger.LocalVariables[i];
                long notEqualId = copiedLv.variable.Id;
                long actualId = pastedLv.variable.Id;

                Assert.AreNotEqual(notEqualId, actualId);
            }
        }
    }
}
