
using HarmonyLib;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using System.Reflection;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ErrorMessage;

namespace Stats_Tracker
{
    [QModCore]
    public class Main
    {
        public static Config config = new Config();
        public static bool setupDone = false;
        //internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        public static void Log(string str, QModManager.Utility.Logger.Level lvl = QModManager.Utility.Logger.Level.Debug)
        {
            QModManager.Utility.Logger.Log(lvl, str);
        }

        //[HarmonyPatch(typeof(Player), "Start")]
        internal class Player_Start_Patch
        { 
            public static void Postfix(Player __instance )
            {
                //AddDebug("Player Start IsPermadeath " + GameModeUtils.IsPermadeath());
                //AddDebug("Player Start SpawnsInitialItems " + GameModeUtils.SpawnsInitialItems());
                //AddDebug("Player Start " + SaveLoadManager.main.currentSlot);
                //config.playerDeaths[SaveLoadManager.main.currentSlot] = 0;
                //config.Save();
                //AddDebug("playerDeaths " + config.playerDeaths[SaveLoadManager.main.currentSlot]);
            }
        }

        public static void Setup()
        {
            Stats_Patch.timeLastUpdate = Stats_Patch.GetTimePlayed();
            string saveSlot = SaveLoadManager.main.currentSlot;
            Stats_Patch.saveSlot = saveSlot;

            if (!config.distanceTraveled.ContainsKey(saveSlot))
                PrepareSaveSlot(saveSlot);

            config.Save();
            Stats_Patch.ModCompat();
            foreach (var s in Stats_Patch.myStrings)
                PDAEncyclopedia.Add(s.Key, false);
        }

        public static void PrepareSaveSlot(string saveSlot)
        {
            //AddDebug("PrepareSaveSlot  " + saveSlot);
            //Log("PrepareSaveSlot  " + saveSlot);
            config.playerDeaths[saveSlot] = 0;
            config.timePlayed[saveSlot] = TimeSpan.Zero;
            config.healthLost[saveSlot] = 0;
            config.foodEaten[saveSlot] = new Dictionary<string, float>();
            config.waterDrunk[saveSlot] = 0;
            config.distanceTraveled[saveSlot] = 0;
            config.maxDepth[saveSlot] = 0;
            config.distanceTraveledSwim[saveSlot] = 0;
            config.distanceTraveledWalk[saveSlot] = 0;
            config.distanceTraveledSeaglide[saveSlot] = 0;
            config.distanceTraveledSeamoth[saveSlot] = 0;
            config.distanceTraveledExosuit[saveSlot] = 0;
            config.distanceTraveledSub[saveSlot] = 0;
            config.seamothsBuilt[saveSlot] = 0;
            config.exosuitsBuilt[saveSlot] = 0;
            config.cyclopsBuilt[saveSlot] = 0;
            config.seamothsLost[saveSlot] = 0;
            config.exosuitsLost[saveSlot] = 0;
            config.cyclopsLost[saveSlot] = 0;
            config.timeSlept[saveSlot] = TimeSpan.Zero;
            config.timeSwam[saveSlot] = TimeSpan.Zero;
            config.timeWalked[saveSlot] = TimeSpan.Zero;
            config.timeSeamoth[saveSlot] = TimeSpan.Zero;
            config.timeExosuit[saveSlot] = TimeSpan.Zero;
            config.timeCyclops[saveSlot] = TimeSpan.Zero;
            config.timeBase[saveSlot] = TimeSpan.Zero;
            config.timeEscapePod[saveSlot] = TimeSpan.Zero;
            config.baseRoomsBuilt[saveSlot] = new Dictionary<string, int>();
            config.baseCorridorsBuilt[saveSlot] = 0;
            config.basePower[saveSlot] = 0;
            config.objectsScanned[saveSlot] = 0;
            config.blueprintsUnlocked[saveSlot] = 0;
            config.blueprintsFromDatabox[saveSlot] = 0;
            config.floraFound[saveSlot] = new HashSet<string>();
            config.faunaFound[saveSlot] = new HashSet<string>();
            config.leviathanFound[saveSlot] = new HashSet<string>();
            config.coralFound[saveSlot] = new HashSet<string>();
            config.animalsKilled[saveSlot] = new Dictionary<string, int>();
            config.plantsKilled[saveSlot] = new Dictionary<string, int>();
            config.coralKilled[saveSlot] = new Dictionary<string, int>();
            config.leviathansKilled[saveSlot] = new Dictionary<string, int>();
            config.plantsRaised[saveSlot] = new Dictionary<string, int>();
            config.eggsHatched[saveSlot] = new Dictionary<string, int>();
            config.itemsCrafted[saveSlot] = new Dictionary<string, int>();
            config.craftingResourcesUsed[saveSlot] = new Dictionary<string, float>();
            config.craftingResourcesUsed_[saveSlot] = new Dictionary<string, int>();
            config.biomesFound[saveSlot] = new HashSet<string>();
            config.kooshFound[saveSlot] = false;
            config.jeweledDiskFound[saveSlot] = false;
            config.ghostLevFound[saveSlot] = false;
            config.storedBase[saveSlot] = new Dictionary<string, int>();
            config.storedEscapePod[saveSlot] = new Dictionary<string, int>();
            config.storedSub[saveSlot] = new Dictionary<string, int>();
            config.storedOutside[saveSlot] = new Dictionary<string, int>();
            config.medkitsUsed[saveSlot] = 0;

        }

        public static void GetBiomeNames()
        {
            if (LargeWorld.main == null || LargeWorld.main.biomeMapLegend == null)
            {
                //AddDebug("LargeWorld.main = null");
                return;
            }
            foreach (KeyValuePair<Int3, BiomeProperties> keyValuePair in LargeWorld.main.biomeMapLegend)
            {
                string name = keyValuePair.Value.name;
                if (!string.IsNullOrEmpty(name))
                    Log(name);
            }
        }

        public static string GetFriendlyName(GameObject go)
        {
            TechType tt = CraftData.GetTechType(go);
            return Language.main.Get(tt.AsString(false));
        }

        [HarmonyPatch(typeof(uGUI_SceneLoading), "End")]
        internal class uGUI_SceneLoading_End_Patch
        { // fires after game loads
            public static void Postfix(uGUI_SceneLoading __instance)
            {
                if (!uGUI.main.hud.active)
                {
                    //AddDebug(" is Loading");
                    return;
                }
                Setup();
                setupDone = true;
            }
        }

        static void SaveData()
        {
            config.timePlayed[SaveLoadManager.main.currentSlot] = Stats_Patch.GetTimePlayed();
            config.basePower[SaveLoadManager.main.currentSlot] = Stats_Patch.basePower;
            config.Save();
        }

        [HarmonyPatch(typeof(EscapePod), "TriggerIntroCinematic")]
        internal class EscapePod_TriggerIntroCinematic_Patch
        {
            public static void Postfix(EscapePod __instance )
            {
                Stats_Patch.GetStartingLoot();
                //AddDebug("Trigger Intro Cinematic " + SaveLoadManager.main.currentSlot);
                //if (!config.distanceTraveled.ContainsKey(SaveLoadManager.main.currentSlot))
                //    PrepareSaveSlot(SaveLoadManager.main.currentSlot);
                //config.Save();
            }
        }

        [HarmonyPatch(typeof(SaveLoadManager), "ClearSlotAsync")]
        internal class SaveLoadManager_ClearSlotAsync_Patch
        {
            public static void Postfix(SaveLoadManager __instance, string slotName)
            {
                //AddDebug("ClearSlotAsync " + slotName);
                PrepareSaveSlot(slotName);
                config.Save();
            }
        }

        public static void CleanUp()
        {
            setupDone = false;
            Stats_Patch.powerRelays = new HashSet<PowerRelay>();
            Stats_Patch.timeLastUpdate = TimeSpan.Zero;
            config.Load();
        }

        [QModPatch]
        public static void Load()
        {
            config.Load();
            Assembly assembly = Assembly.GetExecutingAssembly();
            new Harmony($"qqqbbb_{assembly.GetName().Name}").PatchAll(assembly);
            IngameMenuHandler.RegisterOnSaveEvent(SaveData);
            IngameMenuHandler.RegisterOnQuitEvent(CleanUp);
        }

        [HarmonyPatch(typeof(Player), "Awake")]
        internal class PlayerAwakePatcher
        {
            public static void Prefix()
            {
                Stats_Patch.AddEntries();

            }
        }

    }
}