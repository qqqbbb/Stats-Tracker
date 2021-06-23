
using HarmonyLib;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using System.Reflection;
using System;
using System.Collections.Generic;
using static ErrorMessage;

namespace Stats_Tracker
{
    [QModCore]
    public class Main
    {
        public static Config config = new Config();

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

            if (!config.playerDeaths.ContainsKey(saveSlot))
                config.playerDeaths[saveSlot] = 0;

            if (!config.healthLost.ContainsKey(saveSlot))
                config.healthLost[saveSlot] = 0;
            if (!config.foodEaten.ContainsKey(saveSlot))
                config.foodEaten[saveSlot] = 0;
            if (!config.waterDrunk.ContainsKey(saveSlot))
                config.waterDrunk[saveSlot] = 0;
            if (!config.distanceTraveled.ContainsKey(saveSlot))
                config.distanceTraveled[saveSlot] = 0;
            if (!config.maxDepth.ContainsKey(saveSlot))
                config.maxDepth[saveSlot] = 0;
            if (!config.distanceTraveledSwim.ContainsKey(saveSlot))
                config.distanceTraveledSwim[saveSlot] = 0;
            if (!config.distanceTraveledWalk.ContainsKey(saveSlot))
                config.distanceTraveledWalk[saveSlot] = 0;
            if (!config.distanceTraveledSeaglide.ContainsKey(saveSlot))
                config.distanceTraveledSeaglide[saveSlot] = 0;
            if (!config.distanceTraveledSeamoth.ContainsKey(saveSlot))
                config.distanceTraveledSeamoth[saveSlot] = 0;
            if (!config.distanceTraveledExosuit.ContainsKey(saveSlot))
                config.distanceTraveledExosuit[saveSlot] = 0;
            if (!config.distanceTraveledSub.ContainsKey(saveSlot))
                config.distanceTraveledSub[saveSlot] = 0;
            if (!config.seamothsBuilt.ContainsKey(saveSlot))
                config.seamothsBuilt[saveSlot] = 0;
            if (!config.exosuitsBuilt.ContainsKey(saveSlot))
                config.exosuitsBuilt[saveSlot] = 0;
            if (!config.cyclopsBuilt.ContainsKey(saveSlot))
                config.cyclopsBuilt[saveSlot] = 0;
            if (!config.seamothsLost.ContainsKey(saveSlot))
                config.seamothsLost[saveSlot] = 0;
            if (!config.exosuitsLost.ContainsKey(saveSlot))
                config.exosuitsLost[saveSlot] = 0;
            if (!config.cyclopsLost.ContainsKey(saveSlot))
                config.cyclopsLost[saveSlot] = 0;
            if (!config.itemsCrafted.ContainsKey(saveSlot))
                config.itemsCrafted[saveSlot] = 0;
            if (!config.timeSlept.ContainsKey(saveSlot))
                config.timeSlept[saveSlot] = TimeSpan.Zero;
            if (!config.timeSwam.ContainsKey(saveSlot))
                config.timeSwam[saveSlot] = TimeSpan.Zero;
            if (!config.timeWalked.ContainsKey(saveSlot))
                config.timeWalked[saveSlot] = TimeSpan.Zero;
            if (!config.timeSeamoth.ContainsKey(saveSlot))
                config.timeSeamoth[saveSlot] = TimeSpan.Zero;
            if (!config.timeExosuit.ContainsKey(saveSlot))
                config.timeExosuit[saveSlot] = TimeSpan.Zero;
            if (!config.timeCyclops.ContainsKey(saveSlot))
                config.timeCyclops[saveSlot] = TimeSpan.Zero;
            if (!config.timeBase.ContainsKey(saveSlot))
                config.timeBase[saveSlot] = TimeSpan.Zero;
            if (!config.timeEscapePod.ContainsKey(saveSlot))
                config.timeEscapePod[saveSlot] = TimeSpan.Zero;
            if (!config.baseRoomsBuilt.ContainsKey(saveSlot))
                config.baseRoomsBuilt[saveSlot] = 0;
            if (!config.baseCorridorsBuilt.ContainsKey(saveSlot))
                config.baseCorridorsBuilt[saveSlot] = 0;
            if (!config.basePower.ContainsKey(saveSlot))
                config.basePower[saveSlot] = 0;
            if (!config.objectsScanned.ContainsKey(saveSlot))
                config.objectsScanned[saveSlot] = 0;
            if (!config.blueprintsUnlocked.ContainsKey(saveSlot))
                config.blueprintsUnlocked[saveSlot] = 0;
            if (!config.blueprintsFromDatabox.ContainsKey(saveSlot))
                config.blueprintsFromDatabox[saveSlot] = 0;
            if (!config.floraFound.ContainsKey(saveSlot))
                config.floraFound[saveSlot] = 0;
            if (!config.faunaFound.ContainsKey(saveSlot))
                config.faunaFound[saveSlot] = 0;
            if (!config.leviathanFound.ContainsKey(saveSlot))
                config.leviathanFound[saveSlot] = 0;
            if (!config.coralFound.ContainsKey(saveSlot))
                config.coralFound[saveSlot] = 0;
            if (!config.animalsKilled.ContainsKey(saveSlot))
                config.animalsKilled[saveSlot] = 0;
            if (!config.plantsKilled.ContainsKey(saveSlot))
                config.plantsKilled[saveSlot] = 0;
            if (!config.coralKilled.ContainsKey(saveSlot))
                config.coralKilled[saveSlot] = 0;
            if (!config.leviathansKilled.ContainsKey(saveSlot))
                config.leviathansKilled[saveSlot] = 0;
            if (!config.ghostsKilled.ContainsKey(saveSlot))
                config.ghostsKilled[saveSlot] = 0;
            if (!config.repersKilled.ContainsKey(saveSlot))
                config.repersKilled[saveSlot] = 0;
            if (!config.reefbacksKilled.ContainsKey(saveSlot))
                config.reefbacksKilled[saveSlot] = 0;
            if (!config.seaDragonsKilled.ContainsKey(saveSlot))
                config.seaDragonsKilled[saveSlot] = 0;
            if (!config.seaEmperorsKilled.ContainsKey(saveSlot))
                config.seaEmperorsKilled[saveSlot] = 0;
            if (!config.seaTreadersKilled.ContainsKey(saveSlot))
                config.seaTreadersKilled[saveSlot] = 0;
            if (!config.gulpersKilled.ContainsKey(saveSlot))
                config.gulpersKilled[saveSlot] = 0;
            if (!config.plantsRaised.ContainsKey(saveSlot))
                config.plantsRaised[saveSlot] = 0;
            if (!config.eggsHatched.ContainsKey(saveSlot))
                config.eggsHatched[saveSlot] = 0;
            if (!config.diffItemsCrafted.ContainsKey(saveSlot))
                config.diffItemsCrafted[saveSlot] = new HashSet<TechType>();
            if (!config.diffEggsHatched.ContainsKey(saveSlot))
                config.diffEggsHatched[saveSlot] = new HashSet<TechType>();
            if (!config.craftingResourcesUsed.ContainsKey(saveSlot))
                config.craftingResourcesUsed[saveSlot] = 0;

            config.Save();
            Stats_Patch.ModCompat();
            foreach (var s in Stats_Patch.myStrings)
                PDAEncyclopedia.Add(s.Key, false);
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
            }
        }

        static void SaveData()
        {
            config.timePlayed[SaveLoadManager.main.currentSlot] = Stats_Patch.GetTimePlayed();
            config.basePower[SaveLoadManager.main.currentSlot] = Stats_Patch.basePower;
            config.Save();
        }

        //[HarmonyPatch(typeof(SaveLoadManager), "ClearSlotAsync")]
        internal class SaveLoadManager_ClearSlotAsync_Patch
        {
            public static void Postfix(SaveLoadManager __instance, string slotName)
            {
                //AddDebug("ClearSlotAsync " + slotName);
                config.playerDeaths.Remove(slotName);
                config.healthLost.Remove(slotName);
                config.foodEaten.Remove(slotName);
                config.waterDrunk.Remove(slotName);
                config.distanceTraveled.Remove(slotName);
                config.maxDepth.Remove(slotName);
                config.distanceTraveledSwim.Remove(slotName);
                config.distanceTraveledWalk.Remove(slotName);
                config.distanceTraveledSeaglide.Remove(slotName);
                config.distanceTraveledSeamoth.Remove(slotName);
                config.distanceTraveledExosuit.Remove(slotName);
                config.distanceTraveledSub.Remove(slotName);

                config.Save();
            }
        }

        public static void CleanUp()
        {
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