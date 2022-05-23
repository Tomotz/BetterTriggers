﻿using BetterTriggers.Containers;
using BetterTriggers.WorldEdit;
using Model;
using Model.Data;
using Model.EditorData;
using Model.SaveableData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using War3Net.Build.Audio;
using War3Net.Build.Info;
using War3Net.Build.Providers;
using War3Net.Common.Extensions;

namespace BetterTriggers.Controllers
{
    public class ControllerScriptGenerator
    {
        List<ExplorerElementVariable> variables = new List<ExplorerElementVariable>();
        List<ExplorerElementScript> scripts = new List<ExplorerElementScript>();
        List<ExplorerElementTrigger> triggers = new List<ExplorerElementTrigger>();
        Dictionary<string, Value> generatedVarNames = new Dictionary<string, Value>();
        CultureInfo enUS = new CultureInfo("en-US");
        string separator = $"//***************************************************************************{System.Environment.NewLine}";

        List<string> initialization_triggers = new List<string>();
        int nameNumber = 0;
        string newline = System.Environment.NewLine;


        public void GenerateScript(string outputPath)
        {
            var inMemoryFiles = ContainerProject.projectFiles;

            SortTriggerElements(inMemoryFiles[0]); // root node.
            StringBuilder script = Generate();

            var scriptFileToInput = $"{System.IO.Directory.GetCurrentDirectory()}/Resources/vJass.j";
            File.WriteAllText(scriptFileToInput, script.ToString());

            string JassHelper = $"{System.IO.Directory.GetCurrentDirectory()}/Resources/JassHelper/jasshelper.exe";
            string CommonJ = "\"" + System.IO.Directory.GetCurrentDirectory() + "/Resources/JassHelper/common.j\"";
            string BlizzardJ = "\"" + System.IO.Directory.GetCurrentDirectory() + "/Resources/JassHelper/Blizzard.j\"";
            string fileOutput = "\"" + System.IO.Directory.GetCurrentDirectory() + "/Resources/JassHelper/output.j\"";
            Process p = Process.Start($"{JassHelper}", $"--scriptonly {CommonJ} {BlizzardJ} \"{scriptFileToInput}\" \"{outputPath}\"");
            p.WaitForExit();
        }

        private void SortTriggerElements(IExplorerElement parent)
        {
            string script = string.Empty;

            // Gather all explorer elements
            List<IExplorerElement> children = new List<IExplorerElement>();
            if (parent is ExplorerElementRoot)
            {
                var root = (ExplorerElementRoot)parent;
                children = root.explorerElements;
            }
            else if (parent is ExplorerElementFolder)
            {
                var root = (ExplorerElementFolder)parent;
                children = root.explorerElements;
            }

            for (int i = 0; i < children.Count; i++)
            {
                var element = (IExplorerElement)children[i];

                if (Directory.Exists(element.GetPath()))
                    SortTriggerElements(element);
                else if (element is ExplorerElementTrigger)
                {
                    triggers.Add(element as ExplorerElementTrigger);
                }
                else if (element is ExplorerElementScript)
                {
                    scripts.Add(element as ExplorerElementScript);
                }
                else if (element is ExplorerElementVariable)
                {
                    var variable = (ExplorerElementVariable)element;
                    variable.variable.Name = Path.GetFileNameWithoutExtension(element.GetPath()); // hack
                    variables.Add(element as ExplorerElementVariable);
                }
            }
        }

        private StringBuilder Generate()
        {
            StringBuilder script = new StringBuilder(UInt16.MaxValue);

            // ---- Generate script ---- //

            List<Variable> InitGlobals = new List<Variable>();

            // Global variables
            for (int i = 0; i < variables.Count; i++)
            {
                Variable variable = variables[i].variable;
                InitGlobals.Add(variable);

                script.Append("globals" + newline);
                script.Append(newline);
                script.Append(variable.Type + " " + variable.Name);
                script.Append(newline);
                script.Append("endglobals" + newline);
            }

            // Generated variables
            ControllerTrigger controllerTrigger = new ControllerTrigger();
            var parameters = controllerTrigger.GetParametersAll();
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i] is Value && (
                    parameters[i].returnType == "unit" ||
                    parameters[i].returnType == "destructable" ||
                    parameters[i].returnType == "item" ||
                    parameters[i].returnType == "rect" //||
                                                       //parameters[i].returnType == "camerasetup" // not needed?
                    ))
                {
                    Value value = (Value)parameters[i];

                    generatedVarNames.TryAdd(value.identifier, value);
                }
            }
            script.Append("globals" + newline);

            // Generated map object globals 
            foreach (KeyValuePair<string, Value> kvp in generatedVarNames)
            {
                string varName = kvp.Key;
                string type = kvp.Value.returnType;
                script.Append($"{type} {varName} = null {newline}");
            }

            var regions = Regions.GetAll();
            foreach (var r in regions)
            {
                script.Append($"rect {r.Name.Replace(" ", "_")} = null {newline}");
            }
            var sounds = Sounds.GetSoundsAll();
            foreach (var s in sounds)
            {
                script.Append($"sound {s.Name} = null {newline}");
            }
            var music = Sounds.GetMusicAll();
            foreach (var s in music)
            {
                script.Append($"string {s.Name} = null {newline}");
            }
            var cameras = Cameras.GetAll();
            foreach (var c in cameras)
            {
                script.Append($"camerasetup {c.Name.Replace(" ", "_")} = null {newline}");
            }

            foreach (var trigger in triggers)
            {
                script.Append($"trigger gg_trg_{trigger.GetName().Replace(" ", "_")} {newline}");
            }

            script.Append("endglobals" + newline);
            script.Append(newline);


            // Init global variables
            script.Append("function InitGlobals takes nothing returns nothing" + newline);
            for (int i = 0; i < InitGlobals.Count; i++)
            {
                var variable = InitGlobals[i];
                script.Append("set " + variable.Name + "=" + variable.InitialValue + newline);
            }
            script.Append("endfunction" + newline);


            CreateItemTables(script);
            GenerateUnitItemTables(script);
            CreateSounds(script);

            CreateDestructibles(script);
            CreateItems(script);
            CreateUnits(script);
            CreateRegions(script);
            CreateCameras(script);

            CreateCustomScripts(script);
            GenerateTriggers(script);
            GenerateTriggerInitialization(script);

            GenerateTriggerPlayers(script);
            GenerateCustomTeams(script);
            GenerateAllyPriorities(script);

            GenerateMain(script);
            GenerateMapConfiguration(script);


            // TODO: Feed to jasshelper here?



            return script;
        }



        private void CreateUnits(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"//*{newline}");
            script.Append($"//*  Unit Creation{newline}");
            script.Append($"//*{newline}");
            script.Append(separator);

            script.Append($"function CreateAllUnits takes nothing returns nothing{newline}");
            script.Append($"\tlocal unit u{newline}");
            script.Append($"\tlocal integer unitID{newline}");
            script.Append($"\tlocal trigger t{newline}");
            script.Append($"\tlocal real life{newline}");

            var units = Units.GetAll();
            foreach (var u in units)
            {
                if (u.ToString() == "sloc")
                    continue;

                var id = u.ToString();
                var owner = u.OwnerId.ToString();
                var x = u.Position.X.ToString(enUS);
                var y = u.Position.Y.ToString(enUS);
                var angle = u.Rotation.ToString(enUS);
                var skinId = Int32Extensions.ToRawcode(u.SkinId);

                if (owner == "24")
                    owner = "PLAYER_NEUTRAL_AGGRESSIVE";
                if (owner == "27")
                    owner = "PLAYER_NEUTRAL_PASSIVE";

                Value value;
                if (generatedVarNames.TryGetValue($"{u.ToString()}_{u.CreationNumber}", out value)) // unit with generated variable
                    script.Append($"set {u.ToString()}_{u.CreationNumber} = BlzCreateUnitWithSkin(Player({owner}), '{id}', {x}, {y}, {angle}, '{skinId}'){newline}");
                else
                    script.Append($"call BlzCreateUnitWithSkin(Player({owner}), '{id}', {x}, {y}, {angle}, '{skinId}'){newline}");
            }

            script.Append($"endfunction{newline}");
        }

        private void CreateDestructibles(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"//*{newline}");
            script.Append($"//*  Destructible Objects{newline}");
            script.Append($"//*{newline}");
            script.Append(separator);

            script.Append($"function CreateAllDestructables takes nothing returns nothing{newline}");
            script.Append($"local real life{newline}");

            var dests = Destructibles.GetAll();
            foreach (var d in dests)
            {
                var id = d.ToString();
                var x = d.Position.X.ToString(enUS);
                var y = d.Position.Y.ToString(enUS);
                var angle = d.Rotation.ToString(enUS);
                var scale = d.Scale.X.ToString(enUS);
                var variation = d.Variation;
                var skin = Int32Extensions.ToRawcode(d.SkinId);

                Value value;
                if (generatedVarNames.TryGetValue($"{d.ToString()}_{d.CreationNumber}", out value)) // dest with generated variable
                {
                    script.Append($"set {d.ToString()}_{d.CreationNumber} = BlzCreateDestructableWithSkin('{id}', {x}, {y}, {angle}, {scale}, {variation}, '{skin}'){newline}");
                    if (d.Life < 100)
                    {
                        script.Append($"set life = GetDestructableLife({d.ToString()}_{d.CreationNumber}){newline}");
                        script.Append($"call SetDestructableLife({d.ToString()}_{d.CreationNumber}, {(d.Life * 0.01).ToString(enUS)} * life){newline}");
                    }
                }
            }

            script.Append($"endfunction{newline}");
        }

        private void CreateItems(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"//*{newline}");
            script.Append($"//*  Item Creation{newline}");
            script.Append($"//*{newline}");
            script.Append(separator);

            script.Append($"function CreateAllItems takes nothing returns nothing{newline}");

            var items = Units.GetMapItemsAll();
            foreach (var i in items)
            {
                if (i.ToString() == "sloc")
                    continue;

                var id = i.ToString();
                var x = i.Position.X.ToString(enUS);
                var y = i.Position.Y.ToString(enUS);
                var skinId = Int32Extensions.ToRawcode(i.SkinId);

                script.Append($"call BlzCreateItemWithSkin('{id}', {x}, {y}, '{skinId}'){newline}");
            }

            script.Append($"endfunction{newline}");
        }


        private void CreateRegions(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"//*{newline}");
            script.Append($"//*  Regions{newline}");
            script.Append($"//*{newline}");
            script.Append(separator);

            script.Append($"function CreateRegions takes nothing returns nothing{newline}");
            script.Append($"local weathereffect we{newline}");
            script.Append($"{newline}");

            var regions = Regions.GetAll();
            foreach (var r in regions)
            {
                var id = r.Name.Replace(" ", "_");
                var left = r.Left;
                var bottom = r.Bottom;
                var right = r.Right;
                var top = r.Top;

                script.Append($"set {id} = Rect({left}, {bottom}, {right}, {top}){newline}");
                if (r.WeatherType == War3Net.Build.WeatherType.None)
                    continue;

                script.Append($"set we = AddWeatherEffect({id}, '{Int32Extensions.ToRawcode((int)r.WeatherType)}'){newline}");
                script.Append($"EnableWeatherEffect(we, true){newline}");
            }


            script.Append($"endfunction{newline}");
        }


        private void CreateCameras(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"//*{newline}");
            script.Append($"//*  Cameras{newline}");
            script.Append($"//*{newline}");
            script.Append(separator);

            script.Append($"function CreateCameras takes nothing returns nothing{newline}");
            script.Append($"{newline}");

            var cameras = Cameras.GetAll();
            foreach (var c in cameras)
            {
                var id = c.Name.Replace(" ", "_");

                script.Append($"set {id} = CreateCameraSetup(){newline}");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_ZOFFSET, {c.ZOffset.ToString(enUS)}, 0.0){newline}");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_ROTATION, {c.Rotation.ToString(enUS)}, 0.0){newline}");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_ANGLE_OF_ATTACK, {c.AngleOfAttack.ToString(enUS)}, 0.0){newline}");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_TARGET_DISTANCE, {c.TargetDistance.ToString(enUS)}, 0.0){newline}");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_ROLL, {c.Roll.ToString(enUS)}, 0.0){newline}");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_FIELD_OF_VIEW, {c.FieldOfView.ToString(enUS)}, 0.0){newline}");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_FARZ, {c.FarClippingPlane.ToString(enUS)}, 0.0){newline}");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_NEARZ, {c.NearClippingPlane.ToString(enUS)}, 0.0){newline}");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_LOCAL_PITCH, {c.LocalPitch.ToString(enUS)}, 0.0){newline}");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_LOCAL_YAW, {c.LocalYaw.ToString(enUS)}, 0.0){newline}");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_LOCAL_ROLL, {c.LocalRoll.ToString(enUS)}, 0.0){newline}");
                script.Append($"call CameraSetupSetDestPosition({id}, {c.TargetPosition.X.ToString(enUS)}, {c.TargetPosition.Y.ToString(enUS)}, 0.0){newline}");
                script.Append($"{newline}");
            }

            script.Append($"endfunction{newline}");
        }


        private void CreateSounds(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"//*{newline}");
            script.Append($"//*  Sounds{newline}");
            script.Append($"//*{newline}");
            script.Append(separator);

            script.Append($"function InitSounds takes nothing returns nothing{newline}");
            script.Append($"{newline}");

            var sounds = Sounds.GetSoundsAll();
            var music = Sounds.GetMusicAll();
            foreach (var s in sounds)
            {
                var id = s.Name;
                var path = s.FilePath;
                var looping = s.Flags.HasFlag(SoundFlags.Looping);
                var is3D = s.Flags.HasFlag(SoundFlags.Is3DSound);
                var stopRange = s.Flags.HasFlag(SoundFlags.StopWhenOutOfRange);
                var fadeInRate = s.FadeInRate;
                var fadeOutRate = s.FadeInRate;
                var eax = s.EaxSetting;
                var cutoff = s.DistanceCutoff;
                var pitch = s.Pitch;

                if (cutoff >= UInt32.MaxValue)
                    cutoff = 3000;
                if (pitch >= UInt32.MaxValue)
                    pitch = 1;

                script.Append($"set {id} = CreateSound(\"{path}\", {looping.ToString().ToLower()}, {is3D.ToString().ToLower()}, {stopRange.ToString().ToLower()}, {fadeInRate}, {fadeOutRate}, \"{eax}\"){newline}");
                script.Append($"call SetSoundParamsFromLabel({id}, \"{s.SoundName}\"){newline}");
                script.Append($"call SetSoundDuration({id}, {s.FadeInRate}){newline}"); // TODO: This is not the duration. We should pull from CASC data.
                script.Append($"call SetSoundChannel({id}, {(int)s.Channel}){newline}");
                if (s.MinDistance < 100000 && s.Channel != SoundChannel.UserInterface)
                    script.Append($"call SetSoundDistances({id}, {s.MinDistance}, {s.MaxDistance}){newline}");
                if (cutoff != 3000)
                    script.Append($"call SetSoundDistanceCutoff({id}, {s.DistanceCutoff}){newline}");
                script.Append($"call SetSoundVolume({id}, {s.Volume}){newline}");
                if (pitch != 1)
                    script.Append($"call SetSoundPitch({id}, {pitch}){newline}");
            }
            foreach (var s in music)
            {
                var id = s.Name;
                script.Append($"set {id} = \"{s.SoundName}\"{newline}");
            }

            script.Append($"endfunction{newline}");
        }


        private void CreateItemTables(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"//*{newline}");
            script.Append($"//*  Map Item Tables{newline}");
            script.Append($"//*{newline}");
            script.Append(separator);

            script.Append(newline);

            var itemTables = Info.MapInfo.RandomItemTables;
            foreach (var table in itemTables)
            {
                script.Append($"function ItemTable_{table.Index} takes nothing returns nothing{newline}");
                script.Append(@"
    local widget trigWidget= null
	local unit trigUnit= null
	local integer itemID= 0
	local boolean canDrop= true
	set trigWidget=bj_lastDyingWidget
	if ( trigWidget == null ) then
		set trigUnit=GetTriggerUnit()
	endif
	if ( trigUnit != null ) then
		set canDrop=not IsUnitHidden(trigUnit)
		if ( canDrop and GetChangingUnit() != null ) then
			set canDrop=( GetChangingUnitPrevOwner() == Player(PLAYER_NEUTRAL_AGGRESSIVE) )
		endif
	endif

    if ( canDrop ) then
");

                script.Append($"{newline}");

                foreach (var itemSets in table.ItemSets)
                {
                    script.Append($"\t\tcall RandomDistReset(){newline}");
                    foreach (var item in itemSets.Items)
                    {
                        script.Append($"\t\tcall RandomDistAddItem('{Int32Extensions.ToRawcode(item.ItemId)}', {item.Chance}){newline}");
                    }

                    script.Append(@"
        set itemID=RandomDistChoose()
		if ( trigUnit != null ) then
			call UnitDropItem(trigUnit, itemID)
		else
			call WidgetDropItem(trigWidget, itemID)
		endif
");
                }

                script.Append(@"
    endif
	set bj_lastDyingWidget=null
	call DestroyTrigger(GetTriggeringTrigger())
endfunction
                ");
            }

            script.Append($"{newline}");
        }


        private void GenerateUnitItemTables(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"//*{newline}");
            script.Append($"//*  Unit Item Tables{newline}");
            script.Append($"//*{newline}");
            script.Append(separator);


            var units = Units.GetAll();
            foreach (var u in units)
            {
                if (u.ItemTableSets.Count == 0)
                    continue;

                script.Append($"function UnitItemDrops_{u.CreationNumber} takes nothing returns nothing{newline}");
                script.Append(@"
    local widget trigWidget= null
	local unit trigUnit= null
	local integer itemID= 0
	local boolean canDrop= true
	set trigWidget=bj_lastDyingWidget
	if ( trigWidget == null ) then
		set trigUnit=GetTriggerUnit()
	endif
	if ( trigUnit != null ) then
		set canDrop=not IsUnitHidden(trigUnit)
		if ( canDrop and GetChangingUnit() != null ) then
			set canDrop=( GetChangingUnitPrevOwner() == Player(PLAYER_NEUTRAL_AGGRESSIVE) )
		endif
	endif
	if ( canDrop ) then
		)
");

                script.Append($"{newline}");

                foreach (var itemSet in u.ItemTableSets)
                {
                    script.Append($"\t\tcall RandomDistReset(){newline}");
                    foreach (var item in itemSet.Items)
                    {
                        script.Append($"\t\tcall RandomDistAddItem('{Int32Extensions.ToRawcode(item.ItemId)}', {item.Chance}){newline}");
                    }

                    script.Append(@"
        set itemID=RandomDistChoose()
		if ( trigUnit != null ) then
			call UnitDropItem(trigUnit, itemID)
		else
			call WidgetDropItem(trigWidget, itemID)
		endif
		)
");
                }
                script.Append(@"
    endif
	    set bj_lastDyingWidget=null
	    call DestroyTrigger(GetTriggeringTrigger())
    endfunction
		)
");


                script.Append($"{newline}");
            }

            var destructibles = Destructibles.GetAll();
            foreach (var d in destructibles)
            {
                if (d.ItemTableSets.Count == 0)
                    continue;

                script.Append($"function UnitItemDrops_{d.CreationNumber} takes nothing returns nothing{newline}");
                script.Append(@"
    local widget trigWidget = null
    local unit trigUnit = null
    local integer itemID = 0
    local boolean canDrop = true
    set trigWidget = bj_lastDyingWidget
    if (trigWidget == null) then
      set trigUnit = GetTriggerUnit()
    endif
    if (trigUnit != null) then
      set canDrop = not IsUnitHidden(trigUnit)
        if (canDrop and GetChangingUnit() != null ) then
           set canDrop = (GetChangingUnitPrevOwner() == Player(PLAYER_NEUTRAL_AGGRESSIVE))
        endif
    endif

    if (canDrop) then
		)");

                script.Append($"{newline}");

                foreach (var itemSets in d.ItemTableSets)
                {
                    script.Append($"\t\tcall RandomDistReset(){newline}");
                    foreach (var item in itemSets.Items)
                    {
                        script.Append($"\t\tcall RandomDistAddItem('{Int32Extensions.ToRawcode(item.ItemId)}', {item.Chance}){newline}");
                    }

                    script.Append(@"
        set itemID=RandomDistChoose()
		if ( trigUnit != null ) then
			call UnitDropItem(trigUnit, itemID)
		else
			call WidgetDropItem(trigWidget, itemID)
		endif
		)");
                }

                script.Append(@"(

    endif

    set bj_lastDyingWidget = null

    call DestroyTrigger(GetTriggeringTrigger())
endfunction
        )");

                script.Append($"{newline}");
            }
        }



        private void GenerateTriggerInitialization(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"//*{newline}");
            script.Append($"//*  Triggers{newline}");
            script.Append($"//*{newline}");
            script.Append(separator);

            script.Append($"function InitCustomTriggers takes nothing returns nothing{newline}");

            foreach (var t in triggers)
            {
                if (!t.isEnabled)
                    continue;

                string triggerName = t.GetName().Replace(" ", "_");
                script.Append($"\tcall InitTrig_{triggerName}(){newline}");
            }
            script.Append($"endfunction{newline}");
            script.Append($"{newline}");

            script.Append($"function RunInitializationTriggers takes nothing returns nothing{newline}");
            foreach (var t in initialization_triggers)
            {
                script.Append($"\tcall ConditionalTriggerExecute(\"{t}\"){newline}");
            }
            script.Append($"endfunction{newline}");
        }



        private void GenerateTriggerPlayers(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"//*{newline}");
            script.Append($"//*  Players{newline}");
            script.Append($"//*{newline}");
            script.Append(separator);

            script.Append($"function InitCustomPlayerSlots takes nothing returns nothing{newline}");

            string[] playerType = new string[] { "MAP_CONTROL_USER", "MAP_CONTROL_COMPUTER", "MAP_CONTROL_NEUTRAL", "MAP_CONTROL_RESCUABLE" };
            string[] races = new string[] { "RACE_PREF_RANDOM", "RACE_PREF_HUMAN", "RACE_PREF_ORC", "RACE_PREF_UNDEAD", "RACE_PREF_NIGHTELF" };

            int index = 0;
            var players = Info.MapInfo.Players;
            foreach (var p in players)
            {
                string player = $"Player({p.Id}), ";
                script.Append($"\tcall SetPlayerStartLocation({player + index.ToString()}){newline}");
                if (p.Flags.HasFlag(PlayerFlags.FixedStartPosition) || p.Race == PlayerRace.Selectable)
                    script.Append($"\tcall ForcePlayerStartLocation({player + index.ToString()}){newline}");

                script.Append($"\tcall SetPlayerColor({player} ConvertPlayerColor({index.ToString()})){newline}");
                script.Append($"\tcall SetPlayerRacePreference({player} {races[(int)p.Race]} ){newline}");
                script.Append($"\tcall SetPlayerRaceSelectable({player} true){newline}");
                script.Append($"\tcall SetPlayerController({player} {playerType[(int)p.Controller]} ){newline}");

                if (p.Controller == PlayerController.Rescuable)
                {
                    foreach (var j in players)
                    {
                        if (j.Race == PlayerRace.Human) // why is this here, eejin?
                            script.Append($"\tcall SetPlayerAlliance({player} Player({j.Id}), ALLIANCE_RESCUABLE, true){newline}");
                    }
                }

                script.Append($"{newline}");
                index++;
            }

            script.Append($"endfunction{newline}");
        }



        private void GenerateCustomTeams(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"//*{newline}");
            script.Append($"//*  Custom Teams{newline}");
            script.Append($"//*{newline}");
            script.Append(separator);

            script.Append($"function InitCustomTeams takes nothing returns nothing{newline}");

            int current_force = 0;
            var forces = Info.MapInfo.Forces;
            foreach (var f in forces)
            {
                script.Append($"{newline}");
                script.Append($"\t// Force: {f.Name}{newline}");

                string post_state = string.Empty;
                foreach (var p in Info.MapInfo.Players)
                {
                    // something about player masks here? (HiveWE)
                    script.Append($"\tcall SetPlayerTeam(Player({p.Id}), {current_force}){newline}");

                    if (f.Flags.HasFlag(ForceFlags.AlliedVictory))
                    {
                        script.Append($"\tcall SetPlayerState(Player({p.Id}), PLAYER_STATE_ALLIED_VICTORY, 1){newline}");
                    }

                    foreach (var k in Info.MapInfo.Players)
                    {
                        // something about player masks here? (HiveWE)
                        if (p.Id != k.Id)
                        {
                            if (f.Flags.HasFlag(ForceFlags.Allied))
                                post_state += $"\tcall SetPlayerAllianceStateAllyBJ(Player({p.Id}), Player({k.Id}), true){newline}";
                            if (f.Flags.HasFlag(ForceFlags.ShareVision))
                                post_state += $"\tcall SetPlayerAllianceStateVisionBJ(Player({p.Id}), Player({k.Id}), true){newline}";
                            if (f.Flags.HasFlag(ForceFlags.ShareUnitControl))
                                post_state += $"\tcall SetPlayerAllianceStateControlBJ(Player({p.Id}), Player({k.Id}), true){newline}";
                            if (f.Flags.HasFlag(ForceFlags.ShareAdvancedUnitControl))
                                post_state += $"\tcall SetPlayerAllianceStateFullControlBJ(Player({p.Id}), Player({k.Id}), true){newline}";
                        }
                    }
                }
                if (post_state != string.Empty)
                {
                    script.Append(post_state);
                }

                script.Append($"{newline}");
                current_force++;
            }

            script.Append($"endfunction{newline}");
        }


        private void GenerateAllyPriorities(StringBuilder script)
        {
            script.Append($"function InitAllyPriorities takes nothing returns nothing{newline}");

            Dictionary<int, int> player_to_startloc = new Dictionary<int, int>();

            int current_player = 0;
            foreach (var p in Info.MapInfo.Players)
            {
                player_to_startloc[p.Id] = current_player;
                current_player++;
            }

            current_player = 0;
            foreach (var p in Info.MapInfo.Players)
            {
                string player_text = string.Empty;

                int current_index = 0;
                foreach (var j in Info.MapInfo.Players)
                {
                    if (p.EnemyLowPriorityFlags == 1 && p.Id != j.Id)
                    {
                        player_text += $"\tcall SetStartLocPrio({current_player}, {current_index}, {player_to_startloc[j.Id]}, MAP_LOC_PRIO_LOW){newline}";
                        current_index++;
                    }
                    else if (p.EnemyHighPriorityFlags == 1 && p.Id != j.Id)
                    {
                        player_text += $"\tcall SetStartLocPrio({current_player}, {current_index}, {player_to_startloc[j.Id]}, MAP_LOC_PRIO_HIGH){newline}";
                        current_index++;
                    }
                }

                player_text = $"\tcall SetStartLocPrioCount({current_player}, {current_index}){newline}";
                script.Append(player_text);
                current_player++;
            }

            script.Append($"endfunction{newline}");
        }



        private void GenerateMain(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"//*{newline}");
            script.Append($"//*  Main Initialization{newline}");
            script.Append($"//*{newline}");
            script.Append(separator);

            script.Append($"function main takes nothing returns nothing{newline}");

            string camera_bounds = "\tcall SetCameraBounds(" +
                (Info.MapInfo.CameraBounds.BottomLeft.X - 512f) + " + GetCameraMargin(CAMERA_MARGIN_LEFT), " +
                (Info.MapInfo.CameraBounds.BottomLeft.Y - 256f) + " + GetCameraMargin(CAMERA_MARGIN_BOTTOM), " +

                (Info.MapInfo.CameraBounds.TopRight.X + 512f) + " - GetCameraMargin(CAMERA_MARGIN_RIGHT), " +
                (Info.MapInfo.CameraBounds.TopRight.Y + 256f) + " - GetCameraMargin(CAMERA_MARGIN_TOP), " +

                (Info.MapInfo.CameraBounds.TopLeft.X - 512f) + " + GetCameraMargin(CAMERA_MARGIN_LEFT), " +
                (Info.MapInfo.CameraBounds.TopLeft.Y + 256f) + " - GetCameraMargin(CAMERA_MARGIN_TOP), " +

                (Info.MapInfo.CameraBounds.BottomRight.X + 512f) + " - GetCameraMargin(CAMERA_MARGIN_RIGHT), " +
                (Info.MapInfo.CameraBounds.BottomRight.Y - 256f) + $" + GetCameraMargin(CAMERA_MARGIN_BOTTOM)){newline}";

            script.Append(camera_bounds);

            string terrain_lights = LightEnvironmentProvider.GetTerrainLightEnvironmentModel(Info.MapInfo.LightEnvironment);
            string unit_lights = LightEnvironmentProvider.GetUnitLightEnvironmentModel(Info.MapInfo.LightEnvironment);
            //string terrain_lights = Info.MapInfo.LightEnvironment.ToString();
            //string unit_lights = Info.MapInfo.LightEnvironment.ToString();
            if (terrain_lights == "")
                terrain_lights = LightEnvironmentProvider.GetTerrainLightEnvironmentModel(War3Net.Build.Common.Tileset.LordaeronSummer);
            if (unit_lights == "")
                unit_lights = LightEnvironmentProvider.GetUnitLightEnvironmentModel(War3Net.Build.Common.Tileset.LordaeronSummer);


            script.Append($"\tcall SetDayNightModels(\"" + terrain_lights.Replace(@"\", @"\\") + "\", \"" + unit_lights.Replace(@"\", @"\\") + $"\"){newline}");

            string sound_environment = Info.MapInfo.SoundEnvironment; // TODO: Not working
            script.Append($"\tcall NewSoundEnvironment(\"" + sound_environment + $"\"){newline}");

            string ambient_day = "LordaeronSummerDay"; // TODO: Hardcoded
            script.Append($"\tcall SetAmbientDaySound(\"" + ambient_day + $"\"){newline}");

            string ambient_night = "LordaeronSummerNight"; // TODO: Hardcoded
            script.Append($"\tcall SetAmbientNightSound(\"" + ambient_night + $"\"){newline}");

            script.Append($"\tcall SetMapMusic(\"Music\", true, 0){newline}");
            script.Append($"\tcall InitSounds(){newline}");
            script.Append($"\tcall CreateRegions(){newline}");
            script.Append($"\tcall CreateCameras(){newline}");
            script.Append($"\tcall CreateAllDestructables(){newline}");
            script.Append($"\tcall CreateAllItems(){newline}");
            script.Append($"\tcall CreateAllUnits(){newline}");
            script.Append($"\tcall InitBlizzard(){newline}");

            script.Append($"\tcall InitGlobals(){newline}");
            script.Append($"\tcall InitCustomTriggers(){newline}");
            script.Append($"\tcall RunInitializationTriggers(){newline}");

            script.Append($"endfunction{newline}");
        }



        private void GenerateMapConfiguration(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"//*{newline}");
            script.Append($"//*  Map Configuration{newline}");
            script.Append($"//*{newline}");
            script.Append(separator);

            script.Append($"function config takes nothing returns nothing{newline}");

            script.Append($"\tcall SetMapName(\"{Info.MapInfo.MapName}\"){newline}");
            script.Append($"\tcall SetMapDescription(\"{Info.MapInfo.MapDescription}\"){newline}");
            script.Append($"\tcall SetPlayers({Info.MapInfo.Players.Count}){newline}");
            script.Append($"\tcall SetTeams({Info.MapInfo.Forces.Count}){newline}");
            script.Append($"\tcall SetGamePlacement(MAP_PLACEMENT_TEAMS_TOGETHER){newline}");

            script.Append($"{newline}");

            var units = Units.GetAll();
            foreach (var u in units)
            {
                if (u.ToString() == "sloc")
                    script.Append($"\tcall DefineStartLocation({u.OwnerId}, {u.Position.X * 128f + Info.MapInfo.PlayableMapAreaWidth}, {u.Position.Y * 128f + Info.MapInfo.PlayableMapAreaHeight}){newline}");
            }

            script.Append($"{newline}");

            script.Append($"\tcall InitCustomPlayerSlots(){newline}");
            if (Info.MapInfo.MapFlags.HasFlag(MapFlags.UseCustomForces))
                script.Append($"\tcall InitCustomTeams(){newline}");
            else
            {
                foreach (var p in Info.MapInfo.Players)
                {
                    script.Append($"\tcall SetPlayerSlotAvailable(Player({p.Id}), MAP_CONTROL_USER){newline}");
                }
                script.Append($"\tcall InitGenericPlayerSlots(){newline}");
            }
            script.Append($"\tcall InitAllyPriorities(){newline}");
            script.Append($"endfunction{newline}");
        }



        private void CreateCustomScripts(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"//*{newline}");
            script.Append($"//*  Custom Script Code{newline}");
            script.Append($"//*{newline}");
            script.Append(separator);

            foreach (var s in scripts)
            {
                script.Append(s.script);
                script.Append($"{newline}");
            }
        }



        private void GenerateTriggers(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"//*{newline}");
            script.Append($"//*  Triggers{newline}");
            script.Append($"//*{newline}");
            script.Append(separator);

            foreach (var i in triggers)
            {
                if (!i.isEnabled)
                    continue;

                /* TODO: support trigger to script conversion in editor.
                if (!i.custom_text.empty())
                {
                    trigger_script += i.custom_text + "{newline}";
                }
                else
                */
                //{
                script.Append(ConvertGUIToJass(i, initialization_triggers));
                //}
            }


            // TODO:
            // Search the trigger script for global unit/destructible definitons
            /*
            size_t pos = trigger_script.find("gg_unit", 0);
            while (pos != std::string::npos) {
                std::string type = trigger_script.substr(pos + 8, 4);
                std::string creation_number = trigger_script.substr(pos + 13, 4);
                unit_variables[creation_number] = type;
                pos = trigger_script.find("gg_unit", pos + 17);
            }

            pos = trigger_script.find("gg_dest", 0);
            while (pos != std::string::npos) {
                std::string type = trigger_script.substr(pos + 8, 4);
                std::string creation_number = trigger_script.substr(pos + 13, trigger_script.find_first_not_of("0123456789", pos + 13) - pos - 13);
                destructable_variables[creation_number] = type;
                pos = trigger_script.find("gg_dest", pos + 17);
            }
            */
        }

        private string ConvertGUIToJass(ExplorerElementTrigger t, List<string> initialization_triggers)
        {
            string triggerName = t.GetName().Replace(" ", "_");
            string triggerVarName = "gg_trg_" + triggerName;
            string triggerActionName = "Trig_" + triggerName + "_Actions";

            StringBuilder events = new StringBuilder();
            StringBuilder conditions = new StringBuilder();
            StringBuilder pre_actions = new StringBuilder();
            StringBuilder actions = new StringBuilder();

            events.Append($"function InitTrig_{triggerName} takes nothing returns nothing{newline}");
            events.Append($"\tset {triggerVarName} = CreateTrigger(){newline}");

            actions.Append($"function {triggerActionName} takes nothing returns nothing{newline}");

            foreach (var e in t.trigger.Events)
            {
                if (!e.isEnabled)
                    continue;

                if (e.function.identifier == "MapInitializationEvent")
                {
                    initialization_triggers.Add(triggerVarName);
                    continue;
                }

                events.Append($"\tcall {e.function.identifier}({triggerVarName}, ");
                for (int i = 0; i < e.function.parameters.Count; i++)
                {
                    var p = e.function.parameters[i];

                    if (p.identifier == "VarAsString_Real")
                        events.Append($"\"{ConvertParametersToJass(p)}\"");
                    else
                        events.Append($"{ConvertParametersToJass(p)}");

                    if (i < e.function.parameters.Count - 1)
                        events.Append(", ");

                }
                events.Append($"){newline}");
            }
            foreach (var c in t.trigger.Conditions)
            {
                conditions.Append($"\tif (not{ConvertTriggerElementToJass(c, pre_actions, triggerName, true)})) then{newline}");
                conditions.Append($"\treturn false{newline}");
                conditions.Append($"\tendif{newline}");
            }
            foreach (var a in t.trigger.Actions)
            {
                actions.Append($"\t{ConvertTriggerElementToJass(a, pre_actions, triggerName, false)}{newline}");
            }
            actions.Append($"endfunction{newline}{newline}");

            if (conditions.ToString() != "")
            {
                conditions.Insert(0, $"function Trig_{triggerName}_Conditions takes nothing returns nothing{newline}");
                conditions.Append($"\treturn true{newline}");
                conditions.Append($"endfunction{newline}");

                events.Append($"\tcall TriggerAddCondition({triggerVarName}, Condition(function Trig_{triggerName}_Conditions)){newline}");
            }

            if (!t.isInitiallyOn)
                events.Append($"\tcall DisableTrigger({triggerVarName}){newline}");

            events.Append($"\tcall TriggerAddAction({triggerVarName}, function {triggerActionName}){newline}");
            events.Append($"endfunction{newline}");

            string finalTrigger = $"// Trigger {triggerName}{newline}{separator}{pre_actions.ToString()}{conditions.ToString()}{actions.ToString()}{separator}{events.ToString()}";

            return finalTrigger;
        }



        private string ConvertTriggerElementToJass(TriggerElement t, StringBuilder pre_actions, string triggerName, bool nested)
        {
            StringBuilder script = new StringBuilder();

            if (!t.isEnabled)
                return "";

            Function f = (Function)t.function;

            // Specials
            if (t.function.identifier == "WaitForCondition")
            {
                script.Append($"loop{newline}");
                script.Append($"exitwhen({ConvertParametersToJass(f.parameters[0])}{newline})");
                script.Append($"call TriggerSleepAction(RMaxBJ(bj_WAIT_FOR_COND_MIN_INTERVAL, {ConvertParametersToJass(f.parameters[1])})){newline})");
                script.Append("endloop");
                return script.ToString();
            }

            else if (f.identifier == "ForLoopAMultiple" || f.identifier == "ForLoopBMultiple")
            {
                string loopIndex = f.identifier == "ForLoopAMultiple" ? "bj_forLoopAIndex" : "bj_forLoopBIndex";
                string loopIndexEnd = f.identifier == "ForLoopAMultiple" ? "bj_forLoopAIndexEnd" : "bj_forLoopBIndexEnd";

                script.Append($"set {loopIndex}={ConvertParametersToJass(f.parameters[0])}");
                script.Append($"set {loopIndexEnd}={ConvertParametersToJass(f.parameters[1])}");
                script.Append($"loop{newline}");
                script.Append($"\texitwhen {loopIndex} > {loopIndexEnd}{newline}");

                if (f.identifier == "ForLoopAMultiple")
                {
                    ForLoopAMultiple loopA = (ForLoopAMultiple)f;
                    foreach (var action in loopA.Actions)
                    {
                        script.Append($"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}");
                    }
                }
                else
                {
                    ForLoopBMultiple loopB = (ForLoopBMultiple)f;
                    foreach (var action in loopB.Actions)
                    {
                        script.Append($"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}");
                    }
                }
                script.Append($"\tset {loopIndex}={loopIndex}+1{newline}");
                script.Append($"endloop");
                return script.ToString();
            }

            else if (f.identifier == "ForLoopVarMultiple")
            {
                ForLoopVarMultiple loopVar = (ForLoopVarMultiple)f;
                string variable = loopVar.parameters[0].identifier;

                script.Append($"set {variable} = ");
                script.Append(ConvertParametersToJass(loopVar.parameters[1]) + $"{newline}");
                script.Append("loop{newline}");
                script.Append($"exitwhen {variable} > {ConvertParametersToJass(loopVar.parameters[2])}{newline}");
                foreach (var action in loopVar.Actions)
                {
                    script.Append( $"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}");
                }
                script.Append($"set {variable} = {variable} + 1{newline}");
                script.Append($"endloop{newline}");
                return script.ToString();
            }

            else if (f.identifier == "IfThenElseMultiple")
            {
                IfThenElse ifThenElse = (IfThenElse)f;

                script.Append("if (");
                foreach (var condition in ifThenElse.If)
                {
                    script.Append( $"\t{ConvertTriggerElementToJass(condition, pre_actions, triggerName, true)} ");

                    if (ifThenElse.If.IndexOf(condition) != ifThenElse.If.Count - 1)
                        script.Append( "and ");
                }
                if (ifThenElse.If.Count == 0)
                    script.Append("(true)");

                script.Append( $") then{newline}");
                foreach (var action in ifThenElse.Then)
                {
                    script.Append($"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}");
                }
                script.Append($"\telse{newline}");
                foreach (var action in ifThenElse.Else)
                {
                    script.Append( $"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}");
                }
                script.Append($"\tendif{newline}");

                return script.ToString();
            }

            else if (f.identifier == "ForForceMultiple" || f.identifier == "ForGroupMultiple")
            {
                string function_name = generate_function_name(triggerName);

                // Remove multiple
                script.Append($"call {f.identifier.Substring(0, 8)}({ConvertParametersToJass(f.parameters[0])}, function {function_name}){newline}");

                string pre = string.Empty;
                if (f.identifier == "ForForceMultiple")
                {
                    ForForceMultiple forForce = (ForForceMultiple)f;
                    foreach (var action in forForce.Actions)
                    {
                        pre += $"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}";
                    }
                }
                else
                {
                    ForGroupMultiple forGroup = (ForGroupMultiple)f;
                    foreach (var action in forGroup.Actions)
                    {
                        pre += $"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}";
                    }
                }
                pre_actions.Append($"function {function_name} takes nothing returns nothing{newline}");
                pre_actions.Append(pre);
                pre_actions.Append($"{newline}endfunction{newline}");

                return script.ToString();
            }

            else if (f.identifier == "EnumDestructablesInRectAllMultiple")
            {
                EnumDestructablesInRectAllMultiple enumDest = (EnumDestructablesInRectAllMultiple)f;

                /* TODO: What is this?
                string script_name = trigger_data.data("TriggerActions", "_" + eca.name + "_ScriptName");

                const std::string function_name = generate_function_name(trigger_name);

                // Remove multiple
                output += "call " + script_name + "(" + resolve_parameter(eca.parameters[0], trigger_name, pre_actions, get_type(eca.name, 0)) + ", function " + function_name + "){newline}";
                */

                string function_name = generate_function_name(triggerName);

                // Remove multiple
                script.Append($"call {f.identifier.Substring(0, 26)}({ConvertParametersToJass(f.parameters[0])}), function {function_name}){newline}");

                string pre = string.Empty;
                foreach (var action in enumDest.Actions)
                {
                    pre += $"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}";
                }
                pre_actions.Append($"function {function_name} takes nothing returns nothing{newline}");
                pre_actions.Append(pre);
                pre_actions.Append($"{newline}endfunction{newline}");

                return script.ToString();
            }

            else if (f.identifier == "EnumDestructablesInCircleBJMultiple")
            {
                EnumDestructiblesInCircleBJMultiple enumDest = (EnumDestructiblesInCircleBJMultiple)f;

                /* TODO: What is this?
                string script_name = trigger_data.data("TriggerActions", "_" + eca.name + "_ScriptName");

                const std::string function_name = generate_function_name(trigger_name);

                // Remove multiple
               output += "call " + script_name + "(" + resolve_parameter(eca.parameters[0], trigger_name, pre_actions, get_type(eca.name, 0)) + ", " +
			        resolve_parameter(eca.parameters[1], trigger_name, pre_actions, get_type(eca.name, 1)) + ", function " + function_name + "){newline}";
                */

                string function_name = generate_function_name(triggerName);

                // Remove multiple
                script.Append($"call {f.identifier.Substring(0, 26)}({ConvertParametersToJass(f.parameters[0])}, {ConvertParametersToJass(f.parameters[0])}), function {function_name}){newline}");

                string pre = string.Empty;
                foreach (var action in enumDest.Actions)
                {
                    pre += $"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}";
                }
                pre_actions.Append($"function {function_name} takes nothing returns nothing{newline}");
                pre_actions.Append(pre);
                pre_actions.Append($"{newline}endfunction{newline}");

                return script.ToString();
            }

            else if (f.identifier == "AndMultiple")
            {
                AndMultiple andMultiple = (AndMultiple)f;

                script.Append( "(");
                foreach (var condition in andMultiple.And)
                {
                    script.Append($"\t{ConvertTriggerElementToJass(condition, pre_actions, triggerName, true)} ");

                    if (andMultiple.And.IndexOf(condition) != andMultiple.And.Count - 1)
                        script.Append("and ");
                }
                script.Append( ")");

                return script.ToString();
            }

            else if (f.identifier == "OrMultiple")
            {
                OrMultiple orMultiple = (OrMultiple)f;

                script.Append( "(");
                foreach (var condition in orMultiple.Or)
                {
                    script.Append($"\t{ConvertTriggerElementToJass(condition, pre_actions, triggerName, true)} ");

                    if (orMultiple.Or.IndexOf(condition) != orMultiple.Or.Count - 1)
                        script.Append("or ");
                }
                script.Append(")");

                return script.ToString();
            }

            else if (f.identifier == "ReturnAction")
            {
                return $"return {newline}";
            }

            //script = ConvertEcasToJass(t.function, pre_actions, triggerName, nested);
            script.Append($"call {ConvertParametersToJass(t.function)}");

            return script.ToString();
        }



        private string ConvertParametersToJass(Parameter parameter)
        {
            string output = string.Empty;

            if (parameter is Function)
            {
                Function f = (Function)parameter;
                output += f.identifier + "(";
                for (int i = 0; i < f.parameters.Count; i++)
                {
                    Parameter p = f.parameters[i];
                    output += ConvertParametersToJass(p);
                    if (i != f.parameters.Count - 1)
                        output += ",";
                }
                output += ")";
            }
            else if (parameter is Constant)
            {
                Constant c = (Constant)parameter;
                output += c.identifier;
            }
            else if (parameter is VariableRef)
            {
                VariableRef v = (VariableRef)parameter;
                output += v.identifier;

                ControllerVariable controller = new ControllerVariable();
                Variable variable = controller.GetByReference(v);
                if (variable.IsArray)
                    output += $"[{v.arrayIndexValues[0]}]";
                else if (variable.IsArray && variable.IsTwoDimensions)
                    output += $"[{v.arrayIndexValues[1]}]";
            }
            else if (parameter is Value)
            {
                Value v = (Value)parameter;
                if (v.returnType == "StringExt")
                    output += "\"" + v.identifier + "\"";
                else
                    output += v.identifier;
            }

            return output;
        }



        private string generate_function_name(string triggerName)
        {
            string name = $"Trig_{triggerName}_{nameNumber}";
            nameNumber++;

            return name;
            /*
            auto time = std::chrono::high_resolution_clock::now().time_since_epoch().count();
	        return "Trig_" + trigger_name + "_" + std::to_string(time & 0xFFFFFFFF);
            */
        }
    }
}
