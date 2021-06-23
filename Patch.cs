using HarmonyLib;
using QModManager.Utility;
using System;
//using SMLHelper.V2.Assets;
//using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using SMLHelper.V2.Handlers;
using System.Text;
using LitJson;
using static ErrorMessage;

namespace Stats_Tracker
{
    internal class Stats_Patch
    {
        public static string saveSlot;
        public static CraftNode lastEncNode;
        //public static bool drivingSub;
        public static Dictionary<string, PDAEncyclopedia.EntryData> mapping;
        public static TechType[] roomTypes = new TechType[] { TechType.BaseRoom, TechType.BaseMapRoom, TechType.BaseMoonpool, TechType.BaseObservatory };
        public static TechType[] corridorTypes = new TechType[] { TechType.BaseCorridorI, TechType.BaseCorridorL, TechType.BaseCorridorT, TechType.BaseCorridorX, TechType.BaseCorridorGlassI, TechType.BaseCorridorGlassL, TechType.BaseCorridor, TechType.BaseCorridorGlass };
        public static List<TechType> fauna = new List<TechType> { TechType.Shocker, TechType.Biter, TechType.Blighter, TechType.BoneShark, TechType.Crabsnake, TechType.CrabSquid, TechType.Crash, TechType.LavaLizard, TechType.Mesmer, TechType.SpineEel, TechType.Sandshark, TechType.Stalker, TechType.Warper, TechType.Bladderfish, TechType.Boomerang, TechType.GhostRayRed, TechType.Cutefish, TechType.Eyeye, TechType.GarryFish, TechType.Gasopod, TechType.GhostRayBlue, TechType.HoleFish, TechType.Hoopfish, TechType.Hoverfish, TechType.Jellyray, TechType.LavaBoomerang, TechType.Oculus, TechType.Peeper, TechType.RabbitRay, TechType.LavaEyeye, TechType.Reginald, TechType.Skyray, TechType.Spadefish, TechType.Spinefish, TechType.BlueAmoeba, TechType.LargeFloater, TechType.Bleeder, TechType.Shuttlebug, TechType.CaveCrawler, TechType.Floater, TechType.LavaLarva, TechType.Rockgrub, TechType.Jumper};
        public static List<TechType> leviathans = new List<TechType>
        {TechType.GhostLeviathan, TechType.GhostLeviathanJuvenile, TechType.ReaperLeviathan, TechType.Reefback, TechType.SeaDragon, TechType.SeaEmperorJuvenile, TechType.SeaTreader };
        public static string[] moddedCreatureTechtypes = new string[] {"StellarThalassacean", "JasperThalassacean", "GrandGlider", "Axetail", "AmberClownPincher", "CitrineClownPincher", "EmeraldClownPincher", "RubyClownPincher", "SapphireClownPincher", "GulperLeviathan", "RibbonRay", "Twisteel", "Filtorb", "JellySpinner", "Trianglefish" };
        public static List<TechType> coral = new List<TechType> { TechType.PurpleBrainCoral, TechType.CoralShellPlate, TechType.BrownTubes, TechType.BigCoralTubes, TechType.BlueCoralTubes, TechType.RedTipRockThings, TechType.GenericJeweledDisk, TechType.BlueJeweledDisk, TechType.GreenJeweledDisk, TechType.RedJeweledDisk, TechType.PurpleJeweledDisk, TechType.TreeMushroom};
        public static List<TechType> flora = new List<TechType> { TechType.AcidMushroom, TechType.BloodRoot, TechType.BloodVine, TechType.BluePalm, TechType.SmallKoosh, TechType.MediumKoosh, TechType.LargeKoosh, TechType.HugeKoosh, TechType.BulboTree, TechType.PurpleBranches, TechType.PurpleVegetablePlant, TechType.Creepvine, TechType.WhiteMushroom, TechType.EyesPlant, TechType.FernPalm, TechType.RedRollPlant, TechType.GabeSFeather, TechType.JellyPlant, TechType.RedGreenTentacle, TechType.OrangePetalsPlant, TechType.OrangeMushroom, TechType.SnakeMushroom, TechType.HangingFruitTree, TechType.MembrainTree, TechType.PurpleVasePlant, TechType.PinkMushroom, TechType.SmallFan, TechType.SmallFanCluster, TechType.RedBush, TechType.RedConePlant, TechType.RedBasketPlant, TechType.SeaCrown, TechType.PurpleRattle, TechType.ShellGrass, TechType.SpottedLeavesPlant, TechType.CrashHome, TechType.SpikePlant, TechType.PurpleFan, TechType.PurpleStalk, TechType.PinkFlower, TechType.PurpleTentacle, TechType.BloodGrass, TechType.RedGrass, TechType.RedSeaweed, TechType.BlueBarnacle, TechType.BlueBarnacleCluster, TechType.BlueLostRiverLilly, TechType.BlueTipLostRiverPlant, TechType.HangingStinger, TechType.CoveTree, TechType.BlueCluster, TechType.GreenReeds, TechType.BarnacleSuckers, TechType.BallClusters};
        static bool removingItemsForRecipe = false;
        static TimeSpan bedTimeStart = TimeSpan.Zero;
        public static TimeSpan timeLastUpdate = TimeSpan.Zero;
        public static Dictionary<string, string> myStrings = new Dictionary<string, string>();
        public static Dictionary<string, string> descs = new Dictionary<string, string>();
        public static HashSet<PowerRelay> powerRelays = new HashSet<PowerRelay>();
       static string timePlayed { get { return "Time since crashlanding: " + GetTimePlayed().Days + " days, " + GetTimePlayed().Hours + " hours, " + GetTimePlayed().Minutes + " minutes"; } }
        static TimeSpan timeSleptTotal
        {
            get
            {
                TimeSpan total = TimeSpan.Zero;
                foreach (var kv in Main.config.timeSlept)
                    total += kv.Value;

                return total;
            }
        }
        static TimeSpan timeSwamTotal
        {
            get
            {
                TimeSpan total = TimeSpan.Zero;
                foreach (var kv in Main.config.timeSwam)
                    total += kv.Value;

                return total;
            }
        }
        static TimeSpan timeWalkedTotal
        {
            get
            {
                TimeSpan total = TimeSpan.Zero;
                foreach (var kv in Main.config.timeWalked)
                    total += kv.Value;

                return total;
            }
        }
        static TimeSpan timeEscapePodTotal
        {
            get
            {
                TimeSpan total = TimeSpan.Zero;
                foreach (var kv in Main.config.timeEscapePod)
                    total += kv.Value;

                return total;
            }
        }
        static TimeSpan timeBaseTotal
        {
            get
            {
                TimeSpan total = TimeSpan.Zero;
                foreach (var kv in Main.config.timeBase)
                    total += kv.Value;

                return total;
            }
        }
        static TimeSpan timeCyclopsTotal
        {
            get
            {
                TimeSpan total = TimeSpan.Zero;
                foreach (var kv in Main.config.timeCyclops)
                    total += kv.Value;

                return total;
            }
        }
        static TimeSpan timeExosuitTotal
        {
            get
            {
                TimeSpan total = TimeSpan.Zero;
                foreach (var kv in Main.config.timeExosuit)
                    total += kv.Value;

                return total;
            }
        }
        static TimeSpan timeSeamothTotal
        {
            get
            {
                TimeSpan total = TimeSpan.Zero;
                foreach (var kv in Main.config.timeSeamoth)
                    total += kv.Value;

                return total;
            }
        }
        static TechType gulperTT;
        static string timePlayedTotal { get {
                TimeSpan total = TimeSpan.Zero;
                foreach (var item in Main.config.timePlayed)
                { 
                    if (item.Key != saveSlot)
                        total += item.Value;
                }
                total += GetTimePlayed();
                return "Time since crashlanding: " + total.Days + " days, " + total.Hours + " hours, " + total.Minutes + " minutes"; } }
        static int deathsTotal
        {
            get
            {
                int total = 0;
                foreach (var item in Main.config.playerDeaths)
                        total += item.Value;

                return total;
            }
        }
        static int healthLostTotal
        {
            get
            {
                int total = 0;
                foreach (var item in Main.config.healthLost)
                    total += item.Value;

                return total;
            }
        }
        static float foodEatenTotal
        {
            get
            {
                float total = 0;
                foreach (var item in Main.config.foodEaten)
                    total += item.Value;

                return total;
            }
        }
        static float waterDrunkTotal
        {
            get
            {
                float total = 0;
                foreach (var item in Main.config.waterDrunk)
                    total += item.Value;

                return total;
            }
        }
        static int distanceTraveledTotal
        {
            get
            {
                int total = 0;
                foreach (var item in Main.config.distanceTraveled)
                    total += item.Value;

                return total;
            }
        }
        static int distanceTraveledSwimTotal
        {
            get
            {
                int total = 0;
                foreach (var item in Main.config.distanceTraveledSwim)
                    total += item.Value;

                return total;
            }
        }
        static int distanceTraveledWalkTotal
        {
            get
            {
                int total = 0;
                foreach (var item in Main.config.distanceTraveledWalk)
                    total += item.Value;

                return total;
            }
        }
        static int distanceTraveledSeaglideTotal
        {
            get
            {
                int total = 0;
                foreach (var item in Main.config.distanceTraveledSeaglide)
                    total += item.Value;

                return total;
            }
        }
        static int distanceTraveledSeamothTotal
        {
            get
            {
                int total = 0;
                foreach (var item in Main.config.distanceTraveledSeamoth)
                    total += item.Value;

                return total;
            }
        }
        static int distanceTraveledExosuitTotal
        {
            get
            {
                int total = 0;
                foreach (var item in Main.config.distanceTraveledExosuit)
                    total += item.Value;

                return total;
            }
        }
        static int distanceTraveledSubTotal
        {
            get
            {
                int total = 0;
                foreach (var item in Main.config.distanceTraveledSub)
                    total += item.Value;

                return total;
            }
        }
        static int maxDepthGlobal
        {
            get
            {
                int max = 0;
                foreach (var item in Main.config.maxDepth)
                {
                    if (item.Value > max)
                        max = item.Value;
                }
                return max;
            }
        }
        static TravelMode travelMode;
        static int seamothsBuiltTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.seamothsBuilt)
                    total += kv.Value;

                return total;
            }
        }
        static int seamothsLostTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.seamothsLost)
                    total += kv.Value;

                return total;
            }
        }
        static int exosuitsBuiltTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.exosuitsBuilt)
                    total += kv.Value;

                return total;
            }
        }
        static int exosuitsLostTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.exosuitsLost)
                    total += kv.Value;

                return total;
            }
        }
        static int cyclopsBuiltTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.cyclopsBuilt)
                    total += kv.Value;

                return total;
            }
        }
        static int cyclopsLostTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.cyclopsLost)
                    total += kv.Value;

                return total;
            }
        }
        static int itemsCraftedTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.itemsCrafted)
                    total += kv.Value;

                return total;
            }
        }
        static float craftingResourcesTotal
        {
            get
            {
                float total = 0;
                foreach (var kv in Main.config.craftingResourcesUsed)
                    total += kv.Value;

                return total;
            }
        }
        static int diffItemsCraftedTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.diffItemsCrafted)
                {
                    if (total < kv.Value.Count)
                        total = kv.Value.Count;
                }
                return total;
            }
        }
        static int baseRoomsBuiltTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.baseRoomsBuilt)
                    total += kv.Value;

                return total;
            }
        }
        static int baseCorridorsBuiltTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.baseCorridorsBuilt)
                    total += kv.Value;

                return total;
            }
        }
        public static int basePower
        {
            get
            {
                int total = 0;
                foreach (PowerRelay pr in powerRelays)
                {
                    if (pr)
                        total += (int)pr.GetMaxPower();
                }
                return total /2;
            }
        }
        static int basePowerTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.basePower)
                {
                    if (kv.Key != saveSlot)
                        total += kv.Value;
                }
                total += basePower;

                return total;
            }
        }
        static int objectsScannedTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.objectsScanned)
                {
                    if (kv.Key != saveSlot)
                        total += kv.Value;
                }
                total += Main.config.objectsScanned[saveSlot];

                return total;
            }
        }
        static int blueprintsUnlockedTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.blueprintsUnlocked)
                {
                    if (kv.Key != saveSlot)
                        total += kv.Value;
                }
                total += Main.config.blueprintsUnlocked[saveSlot];

                return total;
            }
        }
        static int blueprintsFromDataboxesTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.blueprintsFromDatabox)
                {
                    if (kv.Key != saveSlot)
                        total += kv.Value;
                }
                total += Main.config.blueprintsFromDatabox[saveSlot];

                return total;
            }
        }
        static int faunaFoundTotal
        {
            get
            {
                int max = 0;
                foreach (var kv in Main.config.faunaFound)
                {
                    if (max < kv.Value)
                        max = kv.Value;
                }
                return max;
            }
        }
        static int floraFoundTotal
        {
            get
            {
                int max = 0;
                foreach (var kv in Main.config.floraFound)
                {
                    if (max < kv.Value)
                        max = kv.Value;
                }
                return max;
            }
        }
        static int coralFoundTotal
        {
            get
            {
                int max = 0;
                foreach (var kv in Main.config.coralFound)
                {
                    if (max < kv.Value)
                        max = kv.Value;
                }
                return max;
            }
        }
        static int leviathanFoundTotal
        {
            get
            {
                int max = 0;
                foreach (var kv in Main.config.leviathanFound)
                {
                    if (max < kv.Value)
                        max = kv.Value;
                }
                return max;
            }
        }
        static int plantsKilledTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.plantsKilled)
                {
                    if (kv.Key != saveSlot)
                        total += kv.Value;
                }
                total += Main.config.plantsKilled[saveSlot];

                return total;
            }
        }
        static int animalsKilledTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.animalsKilled)
                {
                    if (kv.Key != saveSlot)
                        total += kv.Value;
                }
                total += Main.config.animalsKilled[saveSlot];

                return total;
            }
        }
        static int coralsKilledTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.coralKilled)
                {
                    if (kv.Key != saveSlot)
                        total += kv.Value;
                }
                total += Main.config.coralKilled[saveSlot];

                return total;
            }
        }
        static int leviathansKilledTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.leviathansKilled)
                {
                    if (kv.Key != saveSlot)
                        total += kv.Value;
                }
                total += Main.config.leviathansKilled[saveSlot];

                return total;
            }
        }
        static int ghostsKilledTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.ghostsKilled)
                {
                    if (kv.Key != saveSlot)
                        total += kv.Value;
                }
                total += Main.config.ghostsKilled[saveSlot];

                return total;
            }
        }
        static int reapersKilledTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.repersKilled)
                {
                    if (kv.Key != saveSlot)
                        total += kv.Value;
                }
                total += Main.config.repersKilled[saveSlot];
                return total;
            }
        }
        static int reefbacksKilledTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.reefbacksKilled)
                {
                    if (kv.Key != saveSlot)
                        total += kv.Value;
                }
                total += Main.config.reefbacksKilled[saveSlot];
                return total;
            }
        }
        static int seaDragonsKilledTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.seaDragonsKilled)
                {
                    if (kv.Key != saveSlot)
                        total += kv.Value;
                }
                total += Main.config.seaDragonsKilled[saveSlot];
                return total;
            }
        }
        static int seaEmperorsKilledTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.seaEmperorsKilled)
                {
                    if (kv.Key != saveSlot)
                        total += kv.Value;
                }
                total += Main.config.seaEmperorsKilled[saveSlot];
                return total;
            }
        }
        static int seaTreadersKilledTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.seaTreadersKilled)
                {
                    if (kv.Key != saveSlot)
                        total += kv.Value;
                }
                total += Main.config.seaTreadersKilled[saveSlot];
                return total;
            }
        }
        static int plantsRaisedTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.plantsRaised)
                {
                    if (kv.Key != saveSlot)
                        total += kv.Value;
                }
                total += Main.config.plantsRaised[saveSlot];
                return total;
            }
        }
        static int eggsHatchedTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.eggsHatched)
                {
                    if (kv.Key != saveSlot)
                        total += kv.Value;
                }
                total += Main.config.eggsHatched[saveSlot];
                return total;
            }
        }
        static int diffEggsHatchedTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.diffEggsHatched)
                {
                    if (total < kv.Value.Count)
                        total = kv.Value.Count;
                }
                return total;
            }
        }


        public static void AddPDAentry(string key, string name, string desc, string path)
        {
            //newPediaEntries.Add(key);
            string[] nodes = path.Split('/');
            PDAEncyclopedia.EntryData entry = new PDAEncyclopedia.EntryData()
            {
                //path = path,
                key = key,
                nodes = nodes
            };
            PDAEncyclopediaHandler.AddCustomEntry(entry);
            //mapping[key] = entry;
            myStrings[key] = desc;
            descs["EncyDesc_" + key] = desc;
            LanguageHandler.SetLanguageLine("Ency_" + key, name);
            LanguageHandler.SetLanguageLine("EncyDesc_" + key, desc);
            //PDAEncyclopedia.Add(entryData.encyclopedia, false);
        }

        public static TimeSpan GetTimePlayed()
        {
            DateTime gameDateTime = DayNightCycle.ToGameDateTime(DayNightCycle.main.timePassedAsFloat);
            return gameDateTime - DayNightCycle.dateOrigin;
        }

        public static string GetTraveledString(int meters)
        {
            //AddDebug(" metersToKM " + meters);
            int km = 0;
            string s = ""; 
            for (int i = meters; i > 1000; i-=1000)
            {
                km++;
                meters -= 1000;
            }
            //for (int i = 0; i < length; i++)
            if (km > 0)
                s += km + " km, ";

            s += meters + " meters";
            //AddDebug(" metersToKM " + km + " " + meters);
            return s;
        }

        public static void AddEntries()
        {
            //LanguageHandler.SetLanguageLine("EncyPath_", "Statistics");
            //AddPDAentry("Stats", "Statistics", "", "Stats");
            AddPDAentry("StatsGlobal", "Global statistics", "", "Stats");
            AddPDAentry("StatsThisGame", "Current game statistics", "_qwe_", "Stats");
            LanguageHandler.SetLanguageLine("EncyPath_Stats", "Statistics");
        }

        public static void ModCompat()
        {
            foreach (string tt in moddedCreatureTechtypes)
            {
                TechType newTT = TechType.None;
                TechTypeExtensions.FromString(tt, out newTT, false);
                if (newTT != TechType.None)
                {
                    if (tt == "GulperLeviathan")
                    {
                        gulperTT = newTT;
                        leviathans.Add(newTT);
                    }
                    else
                        fauna.Add(newTT);
                }
            }
        }

        [HarmonyPatch(typeof(Language), "TryGet")]
        internal class Language_TryGet_Patch
        {
            public static void Postfix(Language __instance, string key, ref string result)
            {
                if (descs == null || key == null || !descs.ContainsKey(key))
                    return;
                //AddDebug("TryGet " + key);
                if (key == "EncyDesc_StatsGlobal")
                {
                    result = timePlayedTotal;
                    if (Main.config.gamesWon > 0)
                        result += "\nGames completed " + Main.config.gamesWon;
                    result += "\nDeaths: " + deathsTotal;
                    result += "\nHealth lost: " + healthLostTotal;

                    result += "\n\nTime spent on feet: " + timeWalkedTotal.Days + " days, " + timeWalkedTotal.Hours + " hours, " + timeWalkedTotal.Minutes + " minutes.";
                    result += "\nTime spent swimming: " + timeSwamTotal.Days + " days, " + timeSwamTotal.Hours + " hours, " + timeSwamTotal.Minutes + " minutes.";
                    result += "\nTime spent sleeping: " + timeSleptTotal.Days + " days, " + timeSleptTotal.Hours + " hours, " + timeSleptTotal.Minutes + " minutes.";
                    result += "\nTime spent in your life pod: " + timeEscapePodTotal.Days + " days, " + timeEscapePodTotal.Hours + " hours, " + timeEscapePodTotal.Minutes + " minutes.";
                    result += "\nTime spent in your base: " + timeBaseTotal.Days + " days, " + timeBaseTotal.Hours + " hours, " + timeBaseTotal.Minutes + " minutes.";
                    result += "\nTime spent in seamoth: " + timeSeamothTotal.Days + " days, " + timeSeamothTotal.Hours + " hours, " + timeSeamothTotal.Minutes + " minutes.";
                    result += "\nTime spent in prawn suit: " + timeExosuitTotal.Days + " days, " + timeExosuitTotal.Hours + " hours, " + timeExosuitTotal.Minutes + " minutes.";
                    result += "\nTime spent in cyclops: " + timeCyclopsTotal.Days + " days, " + timeCyclopsTotal.Hours + " hours, " + timeCyclopsTotal.Minutes + " minutes.";

                    result += "\n\nFood eaten: " + foodEatenTotal + " kg.";
                    result += "\nWater drunk: " + waterDrunkTotal + " liters.";

                    result += "\n\nDistance traveled: " + GetTraveledString(distanceTraveledTotal);
                    result += "\nDistance traveled by foot: " + distanceTraveledWalkTotal + " meters.";
                    result += "\nDistance traveled by swimming: " + distanceTraveledSwimTotal + " meters.";
                    result += "\nDistance traveled by seaglide: " + distanceTraveledSeaglideTotal + " meters.";
                    result += "\nDistance traveled in seamoth: " + distanceTraveledSeamothTotal + " meters.";
                    result += "\nDistance traveled in prawn suit: " + distanceTraveledExosuitTotal + " meters.";
                    result += "\nDistance traveled in cyclops: " + distanceTraveledSubTotal + " meters.";
                    result += "\nMax depth reached: " + maxDepthGlobal + " meters.";

                    result += "\n\nSeamoths constructed: " + seamothsBuiltTotal;
                    result += "\nSeamoths lost: " + seamothsLostTotal;
                    result += "\nPrawn suits constructed: " + exosuitsBuiltTotal;
                    result += "\nPrawn suits lost: " + exosuitsLostTotal;
                    result += "\nCyclopes constructed: " + cyclopsBuiltTotal;
                    result += "\nCyclopes lost: " + cyclopsLostTotal;

                    result += "\n\nItems crafted: " + itemsCraftedTotal;
                    result += "\nDifferent item types crafted: " + diffItemsCraftedTotal;
                    result += "\nResources used for crafting and constructing: " + craftingResourcesTotal.ToString("0.0") + " kg";

                    result += "\n\nBase rooms built: " + baseRoomsBuiltTotal;
                    result += "\nBase corridor segments built: " + baseCorridorsBuiltTotal;
                    result += "\nTotal power generated for your bases: " + basePowerTotal;

                    result += "\n\nThings scanned: " + objectsScannedTotal;
                    result += "\nBlueprints unlocked by scanning: " + blueprintsUnlockedTotal;
                    result += "\nBlueprints found in databoxes: " + blueprintsFromDataboxesTotal;
                    result += "\nFauna species discovered: " + faunaFoundTotal;
                    result += "\nFlora species discovered: " + floraFoundTotal;
                    result += "\nCoral species discovered: " + coralFoundTotal;
                    result += "\nLeviathan species discovered: " + leviathanFoundTotal;

                    result += "\n\nPlants killed: " + plantsKilledTotal;
                    result += "\nAnimals killed: " + animalsKilledTotal;
                    result += "\nCorals killed: " + coralsKilledTotal;
                    result += "\nLeviathans killed: " + leviathansKilledTotal;
                    if (ghostsKilledTotal > 0)
                        result += "\nGhost leviathans killed: " + ghostsKilledTotal;
                    if (reapersKilledTotal > 0)
                        result += "\nReaper leviathans killed: " + reapersKilledTotal;
                    if (reefbacksKilledTotal > 0)
                        result += "\nReefback leviathans killed: " + reefbacksKilledTotal;
                    if (seaDragonsKilledTotal > 0)
                        result += "\nSea dragon leviathans killed: " + seaDragonsKilledTotal;
                    if (seaEmperorsKilledTotal > 0)
                        result += "\nSea emperor leviathans killed: " + seaEmperorsKilledTotal;
                    if (seaTreadersKilledTotal > 0)
                        result += "\nSea treader leviathans killed: " + seaTreadersKilledTotal;

                    result += "\n\nPlants raised: " + plantsRaisedTotal;
                    result += "\nEggs hatched in AC: " + eggsHatchedTotal;
                    result += "\nDifferent species hatched in AC: " + diffEggsHatchedTotal;

                }
                else if (key == "EncyDesc_StatsThisGame")
                {
                     result = timePlayed;
                    if (GameModeUtils.IsPermadeath() || GameModeUtils.SpawnsInitialItems())
                    {
                        if (Main.config.playerDeaths[saveSlot] > 0)
                            result += "\nDeaths: " + Main.config.playerDeaths[saveSlot];
                    }
                    result += "\nHealth lost: " + Main.config.healthLost[saveSlot];

                    result += "\n\nTime spent on feet: " + Main.config.timeWalked[saveSlot].Days + " days, " + Main.config.timeWalked[saveSlot].Hours + " hours, " + Main.config.timeWalked[saveSlot].Minutes + " minutes.";
                    result += "\nTime spent swimming: " + timeSwamTotal.Days + " days, " + timeSwamTotal.Hours + " hours, " + timeSwamTotal.Minutes + " minutes.";
                    result += "\nTime spent sleeping: " + Main.config.timeSlept[saveSlot].Days + " days, " + Main.config.timeSlept[saveSlot].Hours + " hours, " + Main.config.timeSlept[saveSlot].Minutes + " minutes.";
                    result += "\nTime spent in your life pod: " + Main.config.timeEscapePod[saveSlot].Days + " days, " + Main.config.timeEscapePod[saveSlot].Hours + " hours, " + Main.config.timeEscapePod[saveSlot].Minutes + " minutes.";
                    if (Main.config.baseRoomsBuilt[saveSlot] > 0 || Main.config.baseCorridorsBuilt[saveSlot] > 0)
                        result += "\nTime spent in your base: " + Main.config.timeBase[saveSlot].Days + " days, " + Main.config.timeBase[saveSlot].Hours + " hours, " + Main.config.timeBase[saveSlot].Minutes + " minutes.";
                    if (Main.config.seamothsBuilt[saveSlot] > 0)
                        result += "\nTime spent in seamoth: " + Main.config.timeSeamoth[saveSlot].Days + " days, " + Main.config.timeSeamoth[saveSlot].Hours + " hours, " + Main.config.timeSeamoth[saveSlot].Minutes + " minutes.";
                    if (Main.config.exosuitsBuilt[saveSlot] > 0)
                        result += "\nTime spent in prawn suit: " + Main.config.timeExosuit[saveSlot].Days + " days, " + Main.config.timeExosuit[saveSlot].Hours + " hours, " + Main.config.timeExosuit[saveSlot].Minutes + " minutes.";
                    if (Main.config.cyclopsBuilt[saveSlot] > 0)
                        result += "\nTime spent in cyclops: " + Main.config.timeCyclops[saveSlot].Days + " days, " + Main.config.timeCyclops[saveSlot].Hours + " hours, " + Main.config.timeCyclops[saveSlot].Minutes + " minutes.";

                    if (GameModeUtils.RequiresSurvival())
                    {
                        result += "\n\nFood eaten: " + Main.config.foodEaten[saveSlot] + " kg.";
                        result += "\nWater drunk: " + Main.config.waterDrunk[saveSlot] + " liters.";
                    }
                    result += "\n\nDistance traveled: " + Main.config.distanceTraveled[saveSlot] + " meters.";
                    result += "\nDistance traveled by swimming: " + Main.config.distanceTraveledSwim[saveSlot] + " meters.";
                    result += "\nDistance traveled by foot: " + Main.config.distanceTraveledWalk[saveSlot] + " meters.";
                    result += "\nDistance traveled by seaglide: " + Main.config.distanceTraveledSeaglide[saveSlot] + " meters.";
                    if (Main.config.seamothsBuilt[saveSlot] > 0)
                        result += "\nDistance traveled in seamoth: " + Main.config.distanceTraveledSeamoth[saveSlot] + " meters.";
                    if (Main.config.exosuitsBuilt[saveSlot] > 0)
                        result += "\nDistance traveled in prawn suit: " + Main.config.distanceTraveledExosuit[saveSlot] + " meters.";
                    if (Main.config.cyclopsBuilt[saveSlot] > 0)
                        result += "\nDistance traveled in cyclops: " + Main.config.distanceTraveledSub[saveSlot] + " meters.";
                    result += "\nMax depth reached: " + Main.config.maxDepth[saveSlot] + " meters.";

                    if (Main.config.seamothsBuilt[saveSlot] > 0)
                        result += "\n\nSeamoths constructed: " + Main.config.seamothsBuilt[saveSlot];
                    if (Main.config.exosuitsBuilt[saveSlot] > 0)
                        result += "\nPrawn suits constructed: " + Main.config.exosuitsBuilt[saveSlot];
                    if (Main.config.cyclopsBuilt[saveSlot] > 0)
                        result += "\nCyclopes constructed: " + Main.config.cyclopsBuilt[saveSlot];
                    if (Main.config.seamothsLost[saveSlot] > 0)
                        result += "\nSeamoths lost: " + Main.config.seamothsLost[saveSlot];
                    if (Main.config.exosuitsLost[saveSlot] > 0)
                        result += "\nPrawn suits lost: " + Main.config.exosuitsLost[saveSlot];
                    if (Main.config.cyclopsLost[saveSlot] > 0)
                        result += "\nCyclopes lost: " + Main.config.cyclopsLost[saveSlot];

                    result += "\n\nItems crafted: " + Main.config.itemsCrafted[saveSlot];
                    result += "\nDifferent item types crafted: " + Main.config.diffItemsCrafted[saveSlot].Count;
                    result += "\nResources used for crafting and constructing: " + Main.config.craftingResourcesUsed[saveSlot].ToString("0.0") + " kg";

                    result += "\n";
                    if (Main.config.baseRoomsBuilt[saveSlot] > 0)
                        result += "\nBase rooms built: " + Main.config.baseRoomsBuilt[saveSlot];
                    if (Main.config.baseCorridorsBuilt[saveSlot] > 0)
                        result += "\nBase corridor segments built: " + Main.config.baseCorridorsBuilt[saveSlot];
                    if (Main.config.baseRoomsBuilt[saveSlot] > 0 || Main.config.baseCorridorsBuilt[saveSlot] > 0)
                        result += "\nTotal power generated for your bases: " + basePower;

                    result += "\n";
                    if (Main.config.objectsScanned[saveSlot] > 0)
                        result += "\nThings scanned: " + Main.config.objectsScanned[saveSlot];
                    if (Main.config.blueprintsUnlocked[saveSlot] > 0)
                        result += "\nBlueprints unlocked by scanning: " + Main.config.blueprintsUnlocked[saveSlot];
                    if (Main.config.blueprintsFromDatabox[saveSlot] > 0)
                        result += "\nBlueprints found in databoxes: " + Main.config.blueprintsFromDatabox[saveSlot];
                    if (Main.config.faunaFound[saveSlot] > 0)
                        result += "\nFauna species discovered: " + Main.config.faunaFound[saveSlot];
                    if (Main.config.floraFound[saveSlot] > 0)
                        result += "\nFlora species discovered: " + Main.config.floraFound[saveSlot];
                    if (Main.config.coralFound[saveSlot] > 0)
                        result += "\nCoral species discovered: " + Main.config.coralFound[saveSlot];
                    if (Main.config.leviathanFound[saveSlot] > 0)
                        result += "\nLeviathan species discovered: " + Main.config.leviathanFound[saveSlot];

                    result += "\n";
                    if (Main.config.plantsKilled[saveSlot] > 0)
                        result += "\nPlants killed: " + Main.config.plantsKilled[saveSlot];
                    if (Main.config.animalsKilled[saveSlot] > 0)
                        result += "\nAnimals killed: " + Main.config.animalsKilled[saveSlot];
                    if (Main.config.coralKilled[saveSlot] > 0)
                        result += "\nCorals killed: " + Main.config.coralKilled[saveSlot];
                    if (Main.config.leviathansKilled[saveSlot] > 0)
                    {
                        result += "\nLeviathans killed: " + Main.config.leviathansKilled[saveSlot];
                        if (Main.config.ghostsKilled[saveSlot] > 0)
                            result += "\nGhost leviathans killed: " + Main.config.ghostsKilled[saveSlot];
                        if (Main.config.repersKilled[saveSlot] > 0)
                            result += "\nReaper leviathans killed: " + Main.config.repersKilled[saveSlot];
                        if (Main.config.reefbacksKilled[saveSlot] > 0)
                            result += "\nReefback leviathans killed: " + Main.config.reefbacksKilled[saveSlot];
                        if (Main.config.seaDragonsKilled[saveSlot] > 0)
                            result += "\nSea dragon leviathans killed: " + Main.config.seaDragonsKilled[saveSlot];
                        if (Main.config.seaEmperorsKilled[saveSlot] > 0)
                            result += "\nSea emperor leviathans killed: " + Main.config.seaEmperorsKilled[saveSlot];
                        if (Main.config.seaTreadersKilled[saveSlot] > 0)
                            result += "\nSea treader leviathans killed: " + Main.config.seaTreadersKilled[saveSlot];
                    }
                    result += "\n";
                    if (Main.config.plantsRaised[saveSlot] > 0)
                        result += "\nPlants raised: " + Main.config.plantsRaised[saveSlot];
                    if (Main.config.eggsHatched[saveSlot] > 0)
                    {
                        result += "\nEggs hatched in AC: " + Main.config.eggsHatched[saveSlot];
                        result += "\nDifferent species hatched in AC: " + Main.config.diffEggsHatched[saveSlot].Count;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Player), "OnKill")]
        internal class Player_OnKill_Patch
        {
            public static void Postfix(Player __instance)
            {
                Main.config.playerDeaths[saveSlot] += 1;
            }
        }

        [HarmonyPatch(typeof(DamageSystem), "CalculateDamage")]
        class DamageSystem_CalculateDamage_Patch
        {
            public static void Postfix(DamageSystem __instance, float damage, DamageType type, GameObject target, GameObject dealer, ref float __result)
            {
                if (__result > 0f && target == Player.mainObject)
                {
                    //AddDebug("Player takes damage");
                    Main.config.healthLost[saveSlot] += Mathf.RoundToInt(__result);
                }
            }
        }

        [HarmonyPatch(typeof(Survival), "Eat")]
        class Survival_Eat_Patch
        {
            public static void Postfix(Survival __instance, GameObject useObj, bool __result)
            {
                if (__result)
                {
                    Eatable eatable = useObj.GetComponent<Eatable>();
                    Rigidbody rb = useObj.GetComponent<Rigidbody>();
                    if (eatable && rb)
                    {
                        float foodValue = eatable.GetFoodValue();
                        float waterValue = eatable.GetWaterValue();
                        if (foodValue > waterValue)
                            Main.config.foodEaten[saveSlot] += rb.mass;
                        else if (waterValue > foodValue)
                            Main.config.waterDrunk[saveSlot] += rb.mass;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(LaunchRocket), "StartEndCinematic")]
        internal class LaunchRocket_StartEndCinematic_Patch
        {
            public static void Prefix(LaunchRocket __instance)
            {
                Main.config.gamesWon++;
                Main.config.Save();
            }
        }

        [HarmonyPatch(typeof(Player), "TrackTravelStats")]
        internal class Player_TrackTravelStats_Patch
        {
            public static bool Prefix(Player __instance)
            { 
                Vector3 position = __instance.transform.position;

                __instance.maxDepth = Mathf.Max(__instance.maxDepth, -position.y);
                Main.config.maxDepth[saveSlot] = (int)__instance.maxDepth;

                __instance.distanceTraveled += Vector3.Distance(position, __instance.lastPosition);
                int distanceTraveled = Mathf.RoundToInt((__instance.lastPosition - position).magnitude);
                Main.config.distanceTraveled[saveSlot] += distanceTraveled;

                if (travelMode == TravelMode.Swim && __instance.motorMode == Player.MotorMode.Dive)
                    Main.config.distanceTraveledSwim[saveSlot] += distanceTraveled;
                else if (travelMode == TravelMode.Seaglide && __instance.motorMode == Player.MotorMode.Seaglide)
                    Main.config.distanceTraveledSeaglide[saveSlot] += distanceTraveled;
                else if (travelMode == TravelMode.Seamoth && __instance.inSeamoth)
                    Main.config.distanceTraveledSeamoth[saveSlot] += distanceTraveled;
                else if (travelMode == TravelMode.Exosuit && __instance.inExosuit)
                    Main.config.distanceTraveledExosuit[saveSlot] += distanceTraveled;
                else if (travelMode == TravelMode.Sub && Player.main.mode == Player.Mode.Piloting)
                    Main.config.distanceTraveledSub[saveSlot] += distanceTraveled;
                // MotorMode.Run not used
                else if (travelMode == TravelMode.Walk && __instance.motorMode == Player.MotorMode.Walk) 
                    Main.config.distanceTraveledWalk[saveSlot] += distanceTraveled;

                __instance.lastPosition = position;

                if (__instance.motorMode == Player.MotorMode.Dive)
                    travelMode = TravelMode.Swim;
                else if (__instance.motorMode == Player.MotorMode.Seaglide)
                    travelMode = TravelMode.Seaglide;
                else if (__instance.inSeamoth)
                    travelMode = TravelMode.Seamoth;
                else if (__instance.inExosuit)
                    travelMode = TravelMode.Exosuit;
                else if (__instance.mode == Player.Mode.Piloting)
                    travelMode = TravelMode.Sub;
                else if (__instance.motorMode == Player.MotorMode.Walk)
                    travelMode = TravelMode.Walk;

                if (__instance.motorMode == Player.MotorMode.Walk)
                    Main.config.timeWalked[saveSlot] += GetTimePlayed() - timeLastUpdate;
                if (__instance.IsSwimming())
                    Main.config.timeSwam[saveSlot] += GetTimePlayed() - timeLastUpdate;
                else if (Player.main.currentEscapePod)
                    Main.config.timeEscapePod[saveSlot] += GetTimePlayed() - timeLastUpdate;
                else if (Player.main.IsInSubmarine())
                    Main.config.timeCyclops[saveSlot] += GetTimePlayed() - timeLastUpdate;
                else if (__instance.inSeamoth)
                    Main.config.timeSeamoth[saveSlot] += GetTimePlayed() - timeLastUpdate;
                else if (__instance.inExosuit)
                    Main.config.timeExosuit[saveSlot] += GetTimePlayed() - timeLastUpdate;
                else if (Player.main.IsInBase())
                    Main.config.timeBase[saveSlot] += GetTimePlayed() - timeLastUpdate;


                //AddDebug("timeSwam " + Main.config.timeSwam[saveSlot]);
                timeLastUpdate = GetTimePlayed();
                return false;
            }
        }

        [HarmonyPatch(typeof(CreatureEgg), "Hatch")]
        internal class CreatureEgg_Hatch_Patch
        {
            public static void Prefix(CreatureEgg __instance)
            {
                //AddDebug("Hatch  " + __instance.hatchingCreature);
                Main.config.eggsHatched[saveSlot]++;
                Main.config.diffEggsHatched[saveSlot].Add(__instance.hatchingCreature);
            }
        }

        [HarmonyPatch(typeof(GrowingPlant), "SpawnGrownModel")]
        internal class Plantable_SpawnGrownModel_Patch
        {
            public static void Prefix(GrowingPlant __instance)
            {
                //AddDebug("SpawnGrownModel  " + __instance.name);
                Main.config.plantsRaised[saveSlot]++;
            }
        }

        [HarmonyPatch(typeof(LiveMixin), "TakeDamage")]
        internal class LiveMixin_TakeDamage_Patch
        {
            static bool wasAlive = false;
            public static void Prefix(LiveMixin __instance)
            {
                wasAlive = __instance.health > 0f;
            }
            public static void Postfix(LiveMixin __instance, float originalDamage, Vector3 position, DamageType type , GameObject dealer)
            {
                if (wasAlive && __instance.health <= 0f)
                {
                    if (dealer == Player.main.gameObject || dealer == Player.main.currentMountedVehicle?.gameObject)
                    {
                        TechType tt = CraftData.GetTechType(__instance.gameObject);
                        //AddDebug(tt + " killed by player");
                        if (fauna.Contains(tt))
                            //AddDebug(tt + " animal killed by player");
                            Main.config.animalsKilled[saveSlot]++;
                        else if (flora.Contains(tt))
                            //AddDebug(tt + " plant killed by player");
                            Main.config.plantsKilled[saveSlot]++;
                        else if (coral.Contains(tt))
                            //AddDebug(tt + " coral killed by player");
                            Main.config.coralKilled[saveSlot]++;
                        else if (leviathans.Contains(tt))
                        {
                            //AddDebug(tt + " leviathan killed by player");
                            Main.config.leviathansKilled[saveSlot]++;
                            if (tt == TechType.GhostLeviathan || tt == TechType.GhostLeviathanJuvenile)
                                Main.config.ghostsKilled[saveSlot]++;
                            else if(tt == TechType.ReaperLeviathan)
                                Main.config.repersKilled[saveSlot]++;
                            else if (tt == TechType.Reefback)
                                Main.config.reefbacksKilled[saveSlot]++;
                            else if (tt == TechType.SeaDragon)
                                Main.config.seaDragonsKilled[saveSlot]++;
                            else if (tt == TechType.SeaEmperorJuvenile)
                                Main.config.seaEmperorsKilled[saveSlot]++;
                            else if (tt == TechType.SeaTreader)
                                Main.config.seaTreadersKilled[saveSlot]++;
                            else if(tt == gulperTT)
                                Main.config.gulpersKilled[saveSlot]++;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Inventory), "ConsumeResourcesForRecipe")]
        internal class Inventory_ConsumeResourcesForRecipe_Patch
        { // runs when you have enough ingredients
            public static void Prefix(DamageOnPickup __instance, TechType techType)
            {
                ITechData techData = CraftData.Get(techType);
                if (techData == null)
                    return;
                //int index = 0;
                removingItemsForRecipe = true;
                //for (int ingredientCount = techData.ingredientCount; index < ingredientCount; ++index)
                //{
                //    IIngredient ingredient = techData.GetIngredient(index);
                    //AddDebug("ConsumeResourcesForRecipe  " + ingredient.techType + " " + ingredient.amount);
                    //AddDebug(ingredient.techType + " killed by player");
                    //if (fauna.Contains(ingredient.techType))
                        //AddDebug(ingredient.techType + " animal killed by player");
                    //    Main.config.animalsKilled[saveSlot]++;
                    //else if (flora.Contains(ingredient.techType))
                        //AddDebug(ingredient.techType + " plant killed by player");
                        //Main.config.plantsKilled[saveSlot]++;
                //}
            }
            public static void Postfix(DamageOnPickup __instance, TechType techType)
            {
                removingItemsForRecipe = false;
            }
        }

        [HarmonyPatch(typeof(Inventory), "OnRemoveItem")]
        internal class Inventory_OnRemoveItem_Patch
        { 
            public static void Prefix(InventoryItem item)
            {
                if (removingItemsForRecipe)
                {
                    LiveMixin liveMixin = item.item.GetComponent<LiveMixin>();
                    Rigidbody rb = item.item.GetComponent<Rigidbody>();
                    if (rb)
                        Main.config.craftingResourcesUsed[saveSlot] += rb.mass;

                    bool alive = liveMixin && liveMixin.IsAlive();
                    if (alive || liveMixin == null)
                    {
                        TechType tt = CraftData.GetTechType(item.item.gameObject);
                        if (fauna.Contains(tt))
                        {
                            //AddDebug(tt + " animal killed by player");
                            Main.config.animalsKilled[saveSlot]++;
                        }
                        else if (flora.Contains(tt))
                        {
                            //AddDebug(tt + " plant killed by player");
                            Main.config.plantsKilled[saveSlot]++;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(BlueprintHandTarget), "UnlockBlueprint")]
        internal class BlueprintHandTarget_UnlockBlueprint_Patch
        {
            public static void Prefix(BlueprintHandTarget __instance)
            {
                if (__instance.used)
                    return;

                if (!KnownTech.Contains(__instance.unlockTechType))
                {
                    //AddDebug("unlock  " + __instance.unlockTechType);
                    Main.config.blueprintsFromDatabox[saveSlot]++;
                }
            }
        }

        [HarmonyPatch(typeof(ScannerTool), "Scan")]
        internal class ScannerTool_Scan_Patch
        {
            public static void Postfix(ScannerTool __instance, PDAScanner.Result __result)
            {
                //TechType techType = PDAScanner.scanTarget.techType;

                if (__result == PDAScanner.Result.None || __result == PDAScanner.Result.Scan || __result == PDAScanner.Result.Known) { }
                else
                {
                    //AddDebug("result " + __result + " IsFragment " + fragment);
                    Main.config.objectsScanned[saveSlot] ++;
                    TechType scanTargetTT = PDAScanner.scanTarget.techType;

                    if (fauna.Contains(scanTargetTT))
                    {
                        //AddDebug("scanned creature");
                        Main.config.faunaFound[saveSlot]++;
                    }
                    else if (flora.Contains(scanTargetTT))
                    {
                        //AddDebug("scanned flora");
                        Main.config.floraFound[saveSlot]++;
                        if (scanTargetTT == TechType.SmallKoosh || scanTargetTT == TechType.MediumKoosh || scanTargetTT == TechType.LargeKoosh || scanTargetTT == TechType.HugeKoosh)
                        {
                            flora.Remove(TechType.SmallKoosh);
                            flora.Remove(TechType.MediumKoosh);
                            flora.Remove(TechType.LargeKoosh);
                            flora.Remove(TechType.HugeKoosh);
                        }
                    }
                    else if (coral.Contains(scanTargetTT))
                    {
                        //AddDebug("scanned coral");
                        Main.config.coralFound[saveSlot]++;
                        if (scanTargetTT == TechType.BlueJeweledDisk || scanTargetTT == TechType.GreenJeweledDisk || scanTargetTT == TechType.PurpleJeweledDisk || scanTargetTT == TechType.RedJeweledDisk || scanTargetTT == TechType.GenericJeweledDisk)
                        {
                            coral.Remove(TechType.BlueJeweledDisk);
                            coral.Remove(TechType.GreenJeweledDisk);
                            coral.Remove(TechType.PurpleJeweledDisk);
                            coral.Remove(TechType.RedJeweledDisk);
                            coral.Remove(TechType.GenericJeweledDisk);
                        }
                    }
                    else if (leviathans.Contains(scanTargetTT))
                    {
                        //AddDebug("scanned leviathan");
                        Main.config.leviathanFound[saveSlot]++;
                        if (scanTargetTT == TechType.GhostLeviathan || scanTargetTT == TechType.GhostLeviathanJuvenile)
                        {
                            leviathans.Remove(TechType.GhostLeviathan);
                            leviathans.Remove(TechType.GhostLeviathanJuvenile);
                        }
                    }

                }

            }
        }

        [HarmonyPatch(typeof(PDAScanner), "Unlock")]
        internal class PDAScanner_Unlock_Patch
        {
            public static void Prefix(PDAScanner.EntryData entryData, bool unlockBlueprint, bool unlockEncyclopedia)
            {
                if (entryData == null)
                    return;

                //AddDebug("unlock  " + entryData.key);
                if (unlockBlueprint && entryData.blueprint != TechType.None)
                {
                    //AddDebug("unlock Blueprint ");
                    Main.config.blueprintsUnlocked[saveSlot] ++;
                }
                //if (!PDAEncyclopedia.ContainsEntry(entryData.encyclopedia))
                //{
                //    AddDebug("unlock Encyclopedia " + entryData.encyclopedia);
                //}
                //if (!string.IsNullOrEmpty( entryData.encyclopedia))
                //    AddDebug("unlock Encyclopedia ");
            }
        }

        [HarmonyPatch(typeof(uGUI_EncyclopediaTab), "Activate")]
        internal class uGUI_EncyclopediaTab_Activate_Patch
        {
            public static void Postfix(uGUI_EncyclopediaTab __instance, CraftNode node)
            {
                lastEncNode = node;
                //if (strings.ContainsKey(node.id))
                //{
                //    AddDebug("Activate " + __instance.activeEntry.key);
                //}
            }
        }

        [HarmonyPatch(typeof(Constructable), "Construct")]
        internal class Constructable_Construct_Patch
        {
            public static void Postfix(Constructable __instance)
            {
                if (__instance.constructedAmount >= 1f)
                {
                    //AddDebug(" constructed " + __instance.techType);
                    if (roomTypes.Contains(__instance.techType))
                        Main.config.baseRoomsBuilt[saveSlot]++;
                    else if (corridorTypes.Contains(__instance.techType))
                        Main.config.baseCorridorsBuilt[saveSlot]++;
                }
            }
        }

        [HarmonyPatch(typeof(Constructable), "Deconstruct")]
        internal class Constructable_Deconstruct_Patch
        {
            public static void Postfix(Constructable __instance)
            {
                if (__instance.constructedAmount <= 0f)
                {
                    //AddDebug(" deconstructed " + __instance.techType);
                    if (roomTypes.Contains(__instance.techType))
                        Main.config.baseRoomsBuilt[saveSlot]--;
                    else if (corridorTypes.Contains(__instance.techType))
                        Main.config.baseCorridorsBuilt[saveSlot]--;
                }
            }
        }

        [HarmonyPatch(typeof(PowerRelay), "Start")]
        internal class PowerRelay_Start_Patch
        {
            public static void Postfix(PowerRelay __instance)
            {
                //AddDebug(" PowerRelay Start " + __instance.GetMaxPower());
                if (__instance.GetComponent<EscapePod>())
                { }
                else if(__instance.GetComponent<SubControl>())
                { }
                else
                    powerRelays.Add(__instance);
            }
        }

        [HarmonyPatch(typeof(uGUI_EncyclopediaTab), "Open")]
        internal class uGUI_EncyclopediaTab_Close_Patch
        {
            public static void Prefix(uGUI_EncyclopediaTab __instance)
            {
                if (lastEncNode != null && myStrings.ContainsKey(lastEncNode.id))
                { // update stats 
                    //AddDebug("update tab");
                    __instance.activeEntry = null;
                    __instance.Activate(lastEncNode);
                }
            }
        }

        [HarmonyPatch(typeof(Vehicle), "OnKill")]
        internal class Vehicle_OnKill_Patch
        {
            public static void Postfix(Vehicle __instance)
            {
                if (__instance is SeaMoth)
                {
                    //AddDebug("SeaMoth lost" );
                    Main.config.seamothsLost[saveSlot]++;
                }
                else if (__instance is Exosuit)
                {
                    //AddDebug("Exosuit lost");
                    Main.config.exosuitsLost[saveSlot]++;
                }
            }
        }

        [HarmonyPatch(typeof(SubRoot), "OnKill")]
        internal class SubRoot_OnKill_Patch
        {
            public static void Postfix(SubRoot __instance)
            {
                    //AddDebug("Sub lost");
                Main.config.cyclopsLost[saveSlot]++;
            }
        }

        [HarmonyPatch(typeof(Constructor), "OnConstructionDone")]
        internal class Constructor_OnConstructionDone_Patch
        {
            public static void Prefix(Constructor __instance, GameObject constructedObject)
            {
                TechType tt = CraftData.GetTechType(constructedObject);
                //AddDebug("built tt " + tt);
                //AddDebug("built " + constructedObject.name);
                //if (tt == TechType.Seamoth)
                if (constructedObject.GetComponent<SeaMoth>())
                {
                    Main.config.seamothsBuilt[saveSlot] ++;
                }
                //else if (tt == TechType.Exosuit)
                else if (constructedObject.GetComponent<Exosuit>())
                {
                    Main.config.exosuitsBuilt[saveSlot] ++;
                }
                else if (constructedObject.GetComponent<SubRoot>())
                { // tt is none
                    Main.config.cyclopsBuilt[saveSlot] ++;
                }
            }
        }

        [HarmonyPatch(typeof(GhostCrafter), "Craft")]
        internal class GhostCrafter_Craft_Patch
        {
            public static void Postfix(GhostCrafter __instance, TechType techType)
            {
                Main.config.itemsCrafted[saveSlot]++;
                Main.config.diffItemsCrafted[saveSlot].Add(techType);
                //AddDebug("Craft " + techType);
                //AddDebug("diffItemsCrafted " + Main.config.diffItemsCrafted[saveSlot].Count);
            }
        }

        [HarmonyPatch(typeof(Bed), "EnterInUseMode")]
        internal class Bed_EnterInUseMode_Patch
        {
            public static void Postfix(Bed __instance)
            {
                bedTimeStart = GetTimePlayed();
                //AddDebug("EnterInUseMode " );
            }
        }

        [HarmonyPatch(typeof(Bed), "ExitInUseMode")]
        internal class Bed_ExitInUseMode_Patch
        {
            public static void Postfix(Bed __instance)
            {
                Main.config.timeSlept[saveSlot] += GetTimePlayed() - bedTimeStart;
                //AddDebug("ExitInUseMode " );
            }
        }

        //[HarmonyPatch(typeof(PDAEncyclopedia), "Add", new Type[] { typeof(string), typeof(PDAEncyclopedia.Entry), typeof(bool) })]
        internal class PDAEncyclopedia_Add_Patch
        {
            public static void Postfix(string key, PDAEncyclopedia.Entry entry)
            {
                //uGUI_ListEntry uGuiListEntry = item as uGUI_ListEntry;
                AddDebug("Add " + key);
                Main.Log("Add " + key);
            }
        }

        //[HarmonyPatch(typeof(PDAEncyclopedia), "OnAdd")]
        internal class PDAEncyclopedia_OnAdd_Patch
        {
            public static void Postfix(CraftNode node, bool verbose)
            {
                //uGUI_ListEntry uGuiListEntry = item as uGUI_ListEntry;
                AddDebug("Add " + node.id);
            }
        }

        //[HarmonyPatch(typeof(PDAEncyclopedia), "Initialize")]
        internal class PDAEncyclopedia_Initialize_Patch
        {
            public static void Postfix(PDAData pdaData)
            {
                mapping = PDAEncyclopedia.mapping;
                //Log("mapping count " + mapping.Count);
                //foreach (var item in mapping)
                //{
                //Log(item.Key + " " + item.Value);
                //}
            }
        }

        //[HarmonyPatch(typeof(PDAScanner), "NotifyRemove")]
        internal class PDAScanner_NotifyRemove_Patch
        {
            public static void Postfix(PDAScanner.Entry entry)
            {
                if (PDAScanner.onRemove == null)
                    return;

                AddDebug("NotifyRemove " + entry.techType);
            }
        }

        public enum TravelMode
        {
            Walk,
            Swim,
            Seaglide,
            Seamoth,
            Exosuit,
            Sub
        }
    }
}