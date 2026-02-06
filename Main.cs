
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Handlers;
using Nautilus.Options;
using Nautilus.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UWE;
using static ErrorMessage;

namespace Stats_Tracker
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main : BaseUnityPlugin
    {
        public const string
            MODNAME = "Stats Tracker",
            GUID = "qqqbbb.subnautica.statsTracker",
            VERSION = "5.5.2";

        public static ManualLogSource logger;
        public static OptionsMenu options;
        public static ConfigFile configMenu;
        public static ConfigMain configMain;
        static string configMenuPath = Paths.ConfigPath + Path.DirectorySeparatorChar + MODNAME + Path.DirectorySeparatorChar + "configMenu.cfg";
        public static bool setupDone = false;

        private void Awake()
        {
            logger = Logger;
            logger.LogInfo("Language " + Language.main.currentLanguage);
            //config = OptionsPanelHandler.RegisterModOptions<Config>();

        }

        private void StartLoadingSetup()
        {
            //AddDebug("StartLoadingSetup " + SaveLoadManager.main.currentSlot);
            //Logger.LogInfo("StartLoadingSetup " + SaveLoadManager.main.currentSlot);
            Stats_Display.saveSlot = SaveLoadManager.main.currentSlot;
            Patches.saveSlot = SaveLoadManager.main.currentSlot;
            FixBornCreatures();
            Stats_Display.CreateMyEntries();
        }

        public static void FinishLoadingSetup()
        {
            //AddDebug(" FinishLoadingSetup");
            Patches.timeLastUpdate = Patches.GetTimeSpanPlayed();
            Stats_Display.AddMyEntries();
            setupDone = true;
            logger.LogInfo($"{MODNAME} {VERSION} FinishLoadingSetup done");
        }

        public static void DeleteSaveSlotData(string saveSlot)
        {
            //AddDebug("DeleteSaveSlotData  " + saveSlot);
            //logger.LogInfo("DeleteSaveSlotData " + saveSlot);
            if (configMain.timePlayed.ContainsKey(saveSlot) == false)
                return;

            configMain.playerDeaths.Remove(saveSlot);
            configMain.timePlayed.Remove(saveSlot);
            configMain.healthLost.Remove(saveSlot);
            configMain.foodEaten.Remove(saveSlot);
            configMain.waterDrunk.Remove(saveSlot);
            configMain.distanceTraveled.Remove(saveSlot);
            configMain.maxDepth.Remove(saveSlot);
            configMain.distanceTraveledSwim.Remove(saveSlot);
            configMain.distanceTraveledWalk.Remove(saveSlot);
            configMain.distanceTraveledSeaglide.Remove(saveSlot);
            configMain.distanceTraveledVehicle.Remove(saveSlot);
            configMain.builderToolBuilt.Remove(saveSlot);
            configMain.constructorBuilt.Remove(saveSlot);
            configMain.vehiclesLost.Remove(saveSlot);
            configMain.timeSlept.Remove(saveSlot);
            configMain.timeSwam.Remove(saveSlot);
            configMain.timeWalked.Remove(saveSlot);
            configMain.timeSat.Remove(saveSlot);
            configMain.timeVehicles.Remove(saveSlot);
            configMain.timeBase.Remove(saveSlot);
            configMain.timePrecursor.Remove(saveSlot);
            configMain.timeEscapePod.Remove(saveSlot);
            configMain.baseRoomsBuilt.Remove(saveSlot);
            configMain.baseCorridorsBuilt.Remove(saveSlot);
            configMain.basePower.Remove(saveSlot);
            configMain.objectsScanned.Remove(saveSlot);
            configMain.blueprintsUnlocked.Remove(saveSlot);
            configMain.blueprintsFromDatabox.Remove(saveSlot);
            configMain.floraFound.Remove(saveSlot);
            configMain.faunaFound.Remove(saveSlot);
            configMain.leviathanFound.Remove(saveSlot);
            configMain.coralFound.Remove(saveSlot);
            configMain.animalsKilled.Remove(saveSlot);
            configMain.plantsKilled.Remove(saveSlot);
            configMain.coralKilled.Remove(saveSlot);
            configMain.leviathansKilled.Remove(saveSlot);
            configMain.plantsGrown.Remove(saveSlot);
            configMain.eggsHatched.Remove(saveSlot);
            configMain.creaturesBred.Remove(saveSlot);
            configMain.itemsCrafted.Remove(saveSlot);
            configMain.timeBiomes.Remove(saveSlot);
            configMain.medkitsUsed.Remove(saveSlot);
            configMain.pickedUpItems.Remove(saveSlot);
            configMain.minTemp.Remove(saveSlot);
            configMain.minVehicleTemp.Remove(saveSlot);
            configMain.maxTemp.Remove(saveSlot);
            configMain.maxVehicleTemp.Remove(saveSlot);
            configMain.Save();
        }

        static void SaveData()
        {
            //logger.LogInfo("SaveData " + SaveLoadManager.main.currentSlot);
            UnsavedData.SaveData(SaveLoadManager.main.currentSlot);
            UnsavedData.ResetData();
        }

        [HarmonyPatch(typeof(SaveLoadManager), "ClearSlotAsync")]
        internal class SaveLoadManager_ClearSlotAsync_Patch
        {
            public static void Postfix(SaveLoadManager __instance, string slotName)
            { // runs when starting new game
              //AddDebug("ClearSlotAsync " + slotName + " WaitScreen.IsWaiting " + WaitScreen.IsWaiting);
              //logger.LogInfo("ClearSlotAsync" + slotName + " WaitScreen.IsWaiting " + WaitScreen.IsWaiting);
                DeleteSaveSlotData(slotName);
            }
        }

        public static void CleanUp()
        {
            //AddDebug("CleanUp ");
            //logger.LogInfo("CleanUp " + SaveLoadManager.main.currentSlot);
            setupDone = false;
            Patches.timeLastUpdate = TimeSpan.Zero;
            Patches.teleporting = false;
            UnsavedData.ResetData();
            UnsavedData.basePowerSources.Clear();
            UnsavedData.bases.Clear();
        }

        private void Start()
        {
            configMenu = new ConfigFile(configMenuPath, false);
            ConfigMenu.Bind();
            WaitScreenHandler.RegisterEarlyLoadTask(MODNAME, task => StartLoadingSetup());
            SaveUtils.RegisterOnQuitEvent(CleanUp);
            LanguageHandler.RegisterLocalizationFolder();
            WaitScreenHandler.RegisterLateLoadTask(MODNAME, task => FinishLoadingSetup());
            AddTechTypesToClassTechTable();
            Harmony harmony = new Harmony(GUID);
            harmony.PatchAll();
            options = new OptionsMenu();
            OptionsPanelHandler.RegisterModOptions(options);
            configMain = new ConfigMain();
            configMain.Load();
            logger.LogInfo($"{MODNAME} {VERSION} Start done");
        }

        [HarmonyPatch(typeof(SaveLoadManager), "SaveToDeepStorageAsync", new Type[0])]
        internal class SaveLoadManager_SaveToDeepStorageAsync_Patch
        {
            public static void Postfix(SaveLoadManager __instance)
            { // runs after nautilus SaveEvent
                //AddDebug("SaveToDeepStorageAsync");
                SaveData();
            }
        }

        private static void AddTechTypesToClassTechTable()
        {
            CraftData.PreparePrefabIDCache();
            CraftData.entClassTechTable["769f9f44-30f6-46ed-aaf6-fbba358e1676"] = TechType.BaseBioReactor;
            CraftData.entClassTechTable["864f7780-a4c3-4bf2-b9c7-f4296388b70f"] = TechType.BaseNuclearReactor;
        }

        private static void FixBornCreatures()
        {
            if (configMain.bornCreaturesFixed)
                return;

            HashSet<string> creatures = new HashSet<string>();
            foreach (var kv1 in configMain.eggsHatched)
            {
                foreach (var kv2 in kv1.Value)
                    creatures.Add(kv2.Key);
            }
            foreach (string tt in creatures)
            {
                foreach (var kv_ in configMain.creaturesBred)
                {
                    if (kv_.Value.ContainsKey(tt))
                    {
                        //AddDebug("FixBornCreatures Remove " + tt);
                        kv_.Value.Remove(tt);
                    }
                }
            }
            configMain.bornCreaturesFixed = true;
            configMenu.Save();
        }


    }
}