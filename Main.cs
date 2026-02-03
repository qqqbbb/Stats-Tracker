
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ErrorMessage;

namespace Stats_Tracker
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main : BaseUnityPlugin
    {
        public const string
            MODNAME = "Stats Tracker",
            GUID = "qqqbbb.subnautica.statsTracker",
            VERSION = "5.3.1";

        public static ManualLogSource logger;
        public static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        public static bool setupDone = false;

        private void Awake()
        {
            logger = Logger;
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
            if (config.timePlayed.ContainsKey(saveSlot) == false)
                return;

            config.playerDeaths.Remove(saveSlot);
            config.timePlayed.Remove(saveSlot);
            config.healthLost.Remove(saveSlot);
            config.foodEaten.Remove(saveSlot);
            config.waterDrunk.Remove(saveSlot);
            config.distanceTraveled.Remove(saveSlot);
            config.maxDepth.Remove(saveSlot);
            config.distanceTraveledSwim.Remove(saveSlot);
            config.distanceTraveledWalk.Remove(saveSlot);
            config.distanceTraveledSeaglide.Remove(saveSlot);
            config.distanceTraveledVehicle.Remove(saveSlot);
            config.builderToolBuilt.Remove(saveSlot);
            config.constructorBuilt.Remove(saveSlot);
            config.vehiclesLost.Remove(saveSlot);
            config.timeSlept.Remove(saveSlot);
            config.timeSwam.Remove(saveSlot);
            config.timeWalked.Remove(saveSlot);
            config.timeSat.Remove(saveSlot);
            config.timeVehicles.Remove(saveSlot);
            config.timeBase.Remove(saveSlot);
            config.timePrecursor.Remove(saveSlot);
            config.timeEscapePod.Remove(saveSlot);
            config.baseRoomsBuilt.Remove(saveSlot);
            config.baseCorridorsBuilt.Remove(saveSlot);
            config.basePower.Remove(saveSlot);
            config.objectsScanned.Remove(saveSlot);
            config.blueprintsUnlocked.Remove(saveSlot);
            config.blueprintsFromDatabox.Remove(saveSlot);
            config.floraFound.Remove(saveSlot);
            config.faunaFound.Remove(saveSlot);
            config.leviathanFound.Remove(saveSlot);
            config.coralFound.Remove(saveSlot);
            config.animalsKilled.Remove(saveSlot);
            config.plantsKilled.Remove(saveSlot);
            config.coralKilled.Remove(saveSlot);
            config.leviathansKilled.Remove(saveSlot);
            config.plantsGrown.Remove(saveSlot);
            config.eggsHatched.Remove(saveSlot);
            config.creaturesBred.Remove(saveSlot);
            config.itemsCrafted.Remove(saveSlot);
            config.timeBiomes.Remove(saveSlot);
            config.medkitsUsed.Remove(saveSlot);
            config.pickedUpItems.Remove(saveSlot);
            config.minTemp.Remove(saveSlot);
            config.minVehicleTemp.Remove(saveSlot);
            config.maxTemp.Remove(saveSlot);
            config.maxVehicleTemp.Remove(saveSlot);
            config.Save();
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
            WaitScreenHandler.RegisterEarlyLoadTask(MODNAME, task => StartLoadingSetup());
            SaveUtils.RegisterOnQuitEvent(CleanUp);
            LanguageHandler.RegisterLocalizationFolder();
            WaitScreenHandler.RegisterLateLoadTask(MODNAME, task => FinishLoadingSetup());
            AddTechTypesToClassTechTable();
            Harmony harmony = new Harmony(GUID);
            harmony.PatchAll();
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
            if (config.bornCreaturesFixed)
                return;

            HashSet<string> creatures = new HashSet<string>();
            foreach (var kv1 in config.eggsHatched)
            {
                foreach (var kv2 in kv1.Value)
                    creatures.Add(kv2.Key);
            }
            foreach (string tt in creatures)
            {
                foreach (var kv_ in config.creaturesBred)
                {
                    if (kv_.Value.ContainsKey(tt))
                    {
                        //AddDebug("FixBornCreatures Remove " + tt);
                        kv_.Value.Remove(tt);
                    }
                }
            }
            config.bornCreaturesFixed = true;
            config.Save();
        }


    }
}