using HarmonyLib;
//using QModManager.Utility;
using System;
//using SMLHelper.V2.Assets;
//using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using SMLHelper.V2.Handlers;
using static ErrorMessage;
// gulper lev spawn 1169, 903; 1400, 1281; -72, 867; -174, 1070; -49, 1184; -265, 1118; -717, -1088; -573, 1311; -970, -509
namespace Stats_Tracker
{
    internal class Stats_Patch
    {
        static bool craftedItem = false;
        static LiveMixin killedLM = null;
        public static string saveSlot;
        public static CraftNode lastEncNode;
        public static Dictionary<string, PDAEncyclopedia.EntryData> mapping;
        public static TechType[] roomTypes = new TechType[] { TechType.BaseRoom, TechType.BaseMapRoom, TechType.BaseMoonpool, TechType.BaseObservatory };
        public static TechType[] corridorTypes = new TechType[] { TechType.BaseCorridorI, TechType.BaseCorridorL, TechType.BaseCorridorT, TechType.BaseCorridorX, TechType.BaseCorridorGlassI, TechType.BaseCorridorGlassL, TechType.BaseCorridor, TechType.BaseCorridorGlass };
        public static List<TechType> fauna = new List<TechType> { TechType.Shocker, TechType.Biter, TechType.Blighter, TechType.BoneShark, TechType.Crabsnake, TechType.CrabSquid, TechType.Crash, TechType.LavaLizard, TechType.Mesmer, TechType.SpineEel, TechType.Sandshark, TechType.Stalker, TechType.Warper, TechType.Bladderfish, TechType.Boomerang, TechType.GhostRayRed, TechType.Cutefish, TechType.Eyeye, TechType.GarryFish, TechType.Gasopod, TechType.GhostRayBlue, TechType.HoleFish, TechType.Hoopfish, TechType.Hoverfish, TechType.Jellyray, TechType.LavaBoomerang, TechType.Oculus, TechType.Peeper, TechType.RabbitRay, TechType.LavaEyeye, TechType.Reginald, TechType.Skyray, TechType.Spadefish, TechType.Spinefish, TechType.BlueAmoeba, TechType.LargeFloater, TechType.Bleeder, TechType.Shuttlebug, TechType.CaveCrawler, TechType.Floater, TechType.LavaLarva, TechType.Rockgrub, TechType.Jumper};
        public static List<TechType> leviathans = new List<TechType>
        {TechType.GhostLeviathan, TechType.GhostLeviathanJuvenile, TechType.ReaperLeviathan, TechType.Reefback, TechType.SeaDragon, TechType.SeaEmperorJuvenile, TechType.SeaTreader };
        public static string[] moddedCreatureTechtypes = new string[] { "StellarThalassacean", "JasperThalassacean", "GrandGlider", "Axetail", "AmberClownPincher", "CitrineClownPincher", "EmeraldClownPincher", "RubyClownPincher", "SapphireClownPincher", "GulperLeviathan", "RibbonRay", "Twisteel", "Filtorb", "JellySpinner", "TriangleFish" };
        public static List<TechType> coral = new List<TechType> { TechType.PurpleBrainCoral, TechType.CoralShellPlate, TechType.BrownTubes, TechType.BigCoralTubes, TechType.BlueCoralTubes, TechType.RedTipRockThings, TechType.GenericJeweledDisk, TechType.BlueJeweledDisk, TechType.GreenJeweledDisk, TechType.RedJeweledDisk, TechType.PurpleJeweledDisk, TechType.TreeMushroom};
        public static List<TechType> flora = new List<TechType> { TechType.MelonPlant, TechType.AcidMushroom, TechType.BloodRoot, TechType.BloodVine, TechType.BluePalm, TechType.SmallKoosh, TechType.MediumKoosh, TechType.LargeKoosh, TechType.HugeKoosh, TechType.BulboTree, TechType.PurpleBranches, TechType.PurpleVegetablePlant, TechType.Creepvine, TechType.WhiteMushroom, TechType.EyesPlant, TechType.FernPalm, TechType.RedRollPlant, TechType.GabeSFeather, TechType.JellyPlant, TechType.RedGreenTentacle, TechType.OrangePetalsPlant, TechType.OrangeMushroom, TechType.SnakeMushroom, TechType.HangingFruitTree, TechType.MembrainTree, TechType.PurpleVasePlant, TechType.PinkMushroom, TechType.SmallFan, TechType.SmallFanCluster, TechType.RedBush, TechType.RedConePlant, TechType.RedBasketPlant, TechType.SeaCrown, TechType.PurpleRattle, TechType.ShellGrass, TechType.SpottedLeavesPlant, TechType.CrashHome, TechType.SpikePlant, TechType.PurpleFan, TechType.PurpleStalk, TechType.PinkFlower, TechType.PurpleTentacle, TechType.BloodGrass, TechType.RedGrass, TechType.RedSeaweed, TechType.BlueBarnacle, TechType.BlueBarnacleCluster, TechType.BlueLostRiverLilly, TechType.BlueTipLostRiverPlant, TechType.HangingStinger, TechType.CoveTree, TechType.BlueCluster, TechType.GreenReeds, TechType.BarnacleSuckers, TechType.BallClusters};
        static bool removingItemsForRecipe = false;
        static TimeSpan bedTimeStart = TimeSpan.Zero;
        public static TimeSpan timeLastUpdate = TimeSpan.Zero;
        public static Dictionary<string, string> myStrings = new Dictionary<string, string>();
        public static Dictionary<string, string> descs = new Dictionary<string, string>();
        public static HashSet<PowerRelay> powerRelays = new HashSet<PowerRelay>();
       static string timePlayed { get { return "Time since crashlanding: " + GetTimePlayed().Days + " days, " + GetTimePlayed().Hours + " hours, " + GetTimePlayed().Minutes + " minutes"; } }
        static string timePlayedTotal
        {
            get
            {
                TimeSpan total = TimeSpan.Zero;
                foreach (var item in Main.config.timePlayed)
                {
                    //if (item.Key != saveSlot)
                        total += item.Value;
                }
                total += GetTimePlayed();
                return "Time since crashlanding: " + total.Days + " days, " + total.Hours + " hours, " + total.Minutes + " minutes";
            }
        }
        static TravelMode travelMode;
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
                return total;
            }
        }
        static int basePowerTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.basePower)
                {
                    //if (kv.Key != saveSlot)
                    total += kv.Value;
                }
                total += basePower;

                return total;
            }
        }
        static float craftingResourcesUsedTotal
        {
            get
            {
                float total = 0;
                foreach (var kv in Main.config.craftingResourcesUsedTotal)
                    total += kv.Value;

                return total;
            }
        }
        static float craftingResourcesUsed
        {
            get
            {
                float total = 0;
                foreach (var crafted in Main.config.craftingResourcesUsed[saveSlot])
                    total += crafted.Value;

                return total;
            }
        }
        static int plantsRaised
        {
            get
            {
                int total = 0;
                foreach (var r in Main.config.plantsRaised[saveSlot])
                    total += r.Value;

                return total;
            }
        }
        static int plantsRaisedTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.plantsRaisedTotal)
                    total += kv.Value;

                return total;
            }
        }
        static int eggsHatched
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.eggsHatched[saveSlot])
                    total += kv.Value;

                return total;
            }
        }
        static int eggsHatchedTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.eggsHatchedTotal)
                    total += kv.Value;

                return total;
            }
        }
        static int itemsCrafted
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.itemsCrafted[saveSlot])
                    total += kv.Value;

                return total;
            }
        }
        static int itemsCraftedTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.itemsCraftedTotal)
                    total += kv.Value;

                return total;
            }
        }
        static int animalsKilled
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.animalsKilled[saveSlot])
                    total += kv.Value;

                return total;
            }
        }
        static int animalsKilledTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.animalsKilledTotal)
                    total += kv.Value;

                return total;
            }
        }
        static int leviathansKilled
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.leviathansKilled[saveSlot])
                    total += kv.Value;

                return total;
            }
        }
        static int leviathansKilledTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.leviathansKilledTotal)
                    total += kv.Value;

                return total;
            }
        }
        static int plantsKilled
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.plantsKilled[saveSlot])
                    total += kv.Value;

                return total;
            }
        }
        static int plantsKilledTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.plantsKilledTotal)
                    total += kv.Value;

                return total;
            }
        }
        static int coralKilled
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.coralKilled[saveSlot])
                    total += kv.Value;

                return total;
            }
        }
        static int coralKilledTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.coralKilledTotal)
                    total += kv.Value;

                return total;
            }
        }
        static float foodEaten
        {
            get
            {
                float total = 0;
                foreach (var kv in Main.config.foodEaten[saveSlot])
                    total += kv.Value;

                return total;
            }
        }
        static float foodEatenTotal
        {
            get
            {
                float total = 0;
                foreach (var kv in Main.config.foodEatenTotal)
                    total += kv.Value;

                return total;
            }
        }
        static int baseRoomsBuilt
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.baseRoomsBuilt[saveSlot])
                    total += kv.Value;

                return total;
            }
        }
        static int baseRoomsBuiltTotal
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.baseRoomsBuiltTotal)
                    total += kv.Value;

                return total;
            }
        }
        static int storedEscapePod
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.storedEscapePod[saveSlot])
                    total += kv.Value;

                return total;
            }
        }
        static int storedOutside
        {
            get
            {
                int total = 0;
                foreach (var kv in Main.config.storedOutside[saveSlot])
                    total += kv.Value;

                return total;
            }
        }
        private readonly Dictionary<string, string> biomeNames = new Dictionary<string, string>()
        {
            { "BloodKelp", "Blood Kelp Zone"},
            { "CragField", "Crag Field" },
            { "crashZone", "Crash Zone" },
            { "deepGrandReef", "Deep Grand Reef" },
            { "dunes", "Sand Dunes"},
            { "GrassyPlateaus", "Grassy Plateaus"},
            { "LavaLakes", "Lava Lakes" },
            { "LavaFalls", "Lava Lakes" },
            { "ALZChamber", "Lava Lakes" },
            { "grandReef", "Grand Reef" },
            { "LostRiver_Junction", "Lost River" },
            { "LostRiver_BonesField", "Lost River" },
            { "LostRiver_TreeCove", "Lost River" },
            { "FloatingIsland", "Floating Island" },
            { "JellyshroomCaves", "Jellyshroom Caves" },
            { "kelpForest", "Kelp Forest"},
            { "kooshZone", "Bulb Zone"},
            { "mountains", "Mountains" },
            { "mushroomForest", "Mushroom Forest" },
            { "safeShallows", "Safe Shallows"},
            { "seaTreaderPath", "Sea Treader's Path" },
            { "SparseReef", "Sparse Reef" },
            { "underwaterIslands", "Underwater Islands"},
            { "void", "Ecological Dead Zone" },
            { "ILZChamber", "Inactive Lava Zone" },
            { "ilz", "Inactive Lava Zone" },
        };

        public static string GetCraftingResourcesUsedTotal(string str)
        {
            if (Main.config.craftingResourcesUsedTotal.ContainsKey(str))
                return Language.main.Get(str) + " " + Main.config.craftingResourcesUsedTotal_[str] + ", " + Main.config.craftingResourcesUsedTotal[str].ToString("0.0") + " kg";
            else
                return Language.main.Get(str) + " " + Main.config.craftingResourcesUsedTotal_[str];
        }

        public static string GetCraftingResourcesUsed(string str)
        {
            if (Main.config.craftingResourcesUsed[saveSlot].ContainsKey(str))
                return Language.main.Get(str) + " " + Main.config.craftingResourcesUsed_[saveSlot][str] + ", " + Main.config.craftingResourcesUsed[saveSlot][str].ToString("0.0") + " kg";
            else
                return Language.main.Get(str) + " " + Main.config.craftingResourcesUsed_[saveSlot][str];
        }

        public static string GetBiomeName(string name)
        {
            name = name.ToLower();
            if (name.Contains("bloodkelp"))
                return "Blood Kelp Zone";
            else if (name.Contains("cragfield"))
                return "Crag Field";
            else if (name.Contains("crashzone"))
                return "Crash Zone";
            else if (name.Contains("grandreef"))
                return "Grand Reef";
            else if (name.Contains("dunes"))
                return "Sand Dunes";
            else if (name.Contains("grassyplateaus"))
                return "Grassy Plateaus";
            else if (name.Contains("lostriver"))
                return "Lost River";
            else if (name.Contains("floatingisland"))
                return "Floating Island";
            else if (name.Contains("jellyshroomcaves"))
                return "Jellyshroom Caves";
            else if (name.Contains("kelpforest"))
                return "Kelp Forest";
            else if (name.Contains("kooshzone"))
                return "Bulb Zone";
            else if (name.Contains("mountains"))
                return "Mountains";
            else if (name.Contains("mushroomforest"))
                return "Mushroom Forest";
            else if (name.Contains("safeshallows"))
                return "Safe Shallows";
            else if (name.Contains("seatreaderpath"))
                return "Sea Treader's Path";
            else if (name.Contains("sparsereef"))
                return "Sparse Reef";
            else if (name.Contains("underwaterislands"))
                return "Underwater Islands";
            else if (name.Contains("void"))
                return "Ecological Dead Zone";
            else if (name.Contains("ilz"))
                return "Inactive Lava Zone";
            else if (name.Contains("lava") || name.Contains("alz") || name.Contains("prison"))
                return "Lava Lakes";

            return "unknown";
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
            //DateTime gameDateTime = DayNightCycle.ToGameDateTime(DayNightCycle.main.timePassedAsFloat);
            //return gameDateTime - DayNightCycle.dateOrigin;
            return new TimeSpan(0, 0, Mathf.FloorToInt(DayNightCycle.main.timePassedSinceOrigin * 72f));
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
            //AddPDAentry("Stats", "Statistics", "", "Stats");
            AddPDAentry("StatsGlobal", "Global statistics", "", "Stats");
            AddPDAentry("StatsThisGame", "Current game statistics", "", "Stats");
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
                        //AddDebug("GulperLeviathan");
                        //Main.Log("GulperLeviathan " + newTT);
                        //AddDebug(Language.main.Get(newTT.AsString()));
                        leviathans.Add(newTT);
                    }
                    else
                    {
                        //Main.Log("mod TT " + newTT);
                        fauna.Add(newTT);
                    }

                }
            }
        }

        public static void GetStartingLoot()
        {
            //AddDebug("LootSpawner Start  " + LootSpawner.main.escapePodTechTypes.Count);
            foreach (TechType tt in LootSpawner.main.escapePodTechTypes)
            {
                //AddDebug("Start Loot " + tt);
                string name = tt.AsString();
                if (Main.config.storedEscapePod[saveSlot].ContainsKey(name))
                    Main.config.storedEscapePod[saveSlot][name]++;
                else
                    Main.config.storedEscapePod[saveSlot][name] = 1;

                if (Main.config.storedEscapePodTotal.ContainsKey(name))
                    Main.config.storedEscapePodTotal[name]++;
                else
                    Main.config.storedEscapePodTotal[name] = 1;
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
                if (key == "EncyDesc_StatsThisGame")
                {
                    //string biomeName = Player.main.GetBiomeString();
                    string biomeName = GetBiomeName(LargeWorld.main.GetBiome(Player.main.transform.position));
                    result = "Current biome: " + biomeName;
                    result += "\n" + timePlayed;
                    if (!GameModeUtils.IsPermadeath() && GameModeUtils.RequiresOxygen())
                    {
                        if (Main.config.playerDeaths[saveSlot] > 0)
                            result += "\nDeaths: " + Main.config.playerDeaths[saveSlot];
                    }

                    result += "\n\nTime spent on feet: " + Main.config.timeWalked[saveSlot].Days + " days, " + Main.config.timeWalked[saveSlot].Hours + " hours, " + Main.config.timeWalked[saveSlot].Minutes + " minutes.";
                    result += "\nTime spent swimming: " + Main.config.timeSwam[saveSlot].Days + " days, " + Main.config.timeSwam[saveSlot].Hours + " hours, " + Main.config.timeSwam[saveSlot].Minutes + " minutes.";
                    result += "\nTime spent sleeping: " + Main.config.timeSlept[saveSlot].Days + " days, " + Main.config.timeSlept[saveSlot].Hours + " hours, " + Main.config.timeSlept[saveSlot].Minutes + " minutes.";
                    result += "\nTime spent in your life pod: " + Main.config.timeEscapePod[saveSlot].Days + " days, " + Main.config.timeEscapePod[saveSlot].Hours + " hours, " + Main.config.timeEscapePod[saveSlot].Minutes + " minutes.";
                    if (baseRoomsBuilt > 0 || Main.config.baseCorridorsBuilt[saveSlot] > 0)
                        result += "\nTime spent in your base: " + Main.config.timeBase[saveSlot].Days + " days, " + Main.config.timeBase[saveSlot].Hours + " hours, " + Main.config.timeBase[saveSlot].Minutes + " minutes.";
                    if (Main.config.seamothsBuilt[saveSlot] > 0)
                        result += "\nTime spent in seamoth: " + Main.config.timeSeamoth[saveSlot].Days + " days, " + Main.config.timeSeamoth[saveSlot].Hours + " hours, " + Main.config.timeSeamoth[saveSlot].Minutes + " minutes.";
                    if (Main.config.exosuitsBuilt[saveSlot] > 0)
                        result += "\nTime spent in prawn suit: " + Main.config.timeExosuit[saveSlot].Days + " days, " + Main.config.timeExosuit[saveSlot].Hours + " hours, " + Main.config.timeExosuit[saveSlot].Minutes + " minutes.";
                    if (Main.config.cyclopsBuilt[saveSlot] > 0)
                        result += "\nTime spent in cyclops: " + Main.config.timeCyclops[saveSlot].Days + " days, " + Main.config.timeCyclops[saveSlot].Hours + " hours, " + Main.config.timeCyclops[saveSlot].Minutes + " minutes.";

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

                    if (GameModeUtils.currentGameMode != GameModeOption.Creative)
                    {
                        if (Main.config.healthLost[saveSlot] > 0)
                            result += "\n\nHealth lost: " + Main.config.healthLost[saveSlot];
                        if (Main.config.medkitsUsed[saveSlot] > 0)
                            result += "\nFirst aid kits used: " + Main.config.medkitsUsed[saveSlot];
                    }

                    if (GameModeUtils.RequiresSurvival())
                    {
                        result += "\n\nWater drunk: " + Main.config.waterDrunk[saveSlot] + " liters.";
                        result += "\nFood eaten: " + foodEaten + " kg.";
                        foreach (var kv in Main.config.foodEaten[saveSlot])
                            result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value + " kg.";
                    }

                    if (baseRoomsBuilt > 0 || Main.config.baseCorridorsBuilt[saveSlot] > 0)
                        result += "\n\nTotal power generated for your bases: " + basePower;
                    if (Main.config.baseCorridorsBuilt[saveSlot] > 0)
                        result += "\nBase corridor segments built: " + Main.config.baseCorridorsBuilt[saveSlot];
                    if (baseRoomsBuilt > 0)
                        result += "\nBase rooms built: " + baseRoomsBuilt;
                    foreach (var kv in Main.config.baseRoomsBuilt[saveSlot])
                        result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;

                    if (Main.config.objectsScanned[saveSlot] > 0 || Main.config.blueprintsFromDatabox[saveSlot] > 0)
                        result += "\n";
                    if (Main.config.objectsScanned[saveSlot] > 0)
                        result += "\nObjects scanned: " + Main.config.objectsScanned[saveSlot];
                    if (Main.config.blueprintsUnlocked[saveSlot] > 0)
                        result += "\nBlueprints unlocked by scanning: " + Main.config.blueprintsUnlocked[saveSlot];
                    if (Main.config.blueprintsFromDatabox[saveSlot] > 0)
                        result += "\nBlueprints found in databoxes: " + Main.config.blueprintsFromDatabox[saveSlot];

                    if (plantsKilled > 0)
                        result += "\n\nPlants killed: " + plantsKilled;
                    foreach (var kv in Main.config.plantsKilled[saveSlot])
                        result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;

                    if (animalsKilled > 0)
                        result += "\n\nAnimals killed: " + animalsKilled;
                    foreach (var kv in Main.config.animalsKilled[saveSlot])
                        result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;

                    if (coralKilled > 0)
                        result += "\n\nCorals killed: " + coralKilled;
                    foreach (var kv in Main.config.coralKilled[saveSlot])
                        result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;

                    if (leviathansKilled > 0)
                        result += "\n\nLeviathans killed: " + leviathansKilled;
                    foreach (var kv in Main.config.leviathansKilled[saveSlot])
                        result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;

                    result += "\n\nItems crafted: " + itemsCrafted;
                    foreach (var kv in Main.config.itemsCrafted[saveSlot])
                        result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;

                    result += "\n\nResources used for crafting and constructing: " + craftingResourcesUsed.ToString("0.0") + " kg";
                    foreach (var kv in Main.config.craftingResourcesUsed_[saveSlot])
                        result += "\n      " + GetCraftingResourcesUsed(kv.Key);

                    if (plantsRaised > 0)
                        result += "\n\nPlants raised: " + plantsRaised;
                    foreach (var kv in Main.config.plantsRaised[saveSlot])
                        result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;

                    if (eggsHatched > 0)
                    {
                        result += "\n\nEggs hatched in AC: " + eggsHatched;
                        foreach (var kv in Main.config.eggsHatched[saveSlot])
                            result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;
                    }

                    if (storedEscapePod > 0)
                    {
                        result += "\n\nThings stored in your life pod: ";
                        foreach (var kv in Main.config.storedEscapePod[saveSlot])
                        {
                            if (kv.Value > 0)
                                result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;
                        }
                    }

                    if (baseRoomsBuilt > 0 || Main.config.baseCorridorsBuilt[saveSlot] > 0)
                    {
                        result += "\n\nThings stored in your bases: ";
                        foreach (var kv in Main.config.storedBase[saveSlot])
                        {
                            if (kv.Value > 0)
                                result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;
                        }
                    }

                    if (Main.config.cyclopsBuilt[saveSlot] > 0)
                    {
                        result += "\n\nThings stored in cyclops: ";
                        foreach (var kv in Main.config.storedSub[saveSlot])
                        {
                            if (kv.Value > 0)
                                result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;
                        }
                    }

                    if (storedOutside > 0)
                    {
                        result += "\n\nThings stored outside your bases: ";
                        foreach (var kv in Main.config.storedOutside[saveSlot])
                        {
                            if (kv.Value > 0)
                                result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;
                        }
                    }

                    result += "\n\nBiomes discovered:";
                    foreach (string biome in Main.config.biomesFound[saveSlot])
                        result += "\n      " + biome;

                    if (Main.config.faunaFound[saveSlot].Count > 0)
                        result += "\n\nFauna species discovered: ";
                    foreach (string str in Main.config.faunaFound[saveSlot])
                        result += "\n      " + Language.main.Get(str);

                    if (Main.config.floraFound[saveSlot].Count > 0)
                        result += "\n\nFlora species discovered: ";
                    foreach (string str in Main.config.floraFound[saveSlot])
                            result += "\n      " + Language.main.Get(str);

                    if (Main.config.coralFound[saveSlot].Count > 0)
                        result += "\n\nCoral species discovered: ";
                    foreach (string str in Main.config.coralFound[saveSlot])
                            result += "\n      " + Language.main.Get(str);

                    if (Main.config.leviathanFound[saveSlot].Count > 0)
                        result += "\n\nLeviathan species discovered: ";
                    foreach (string str in Main.config.leviathanFound[saveSlot])
                            result += "\n      " + Language.main.Get(str);
                }
                else if (key == "EncyDesc_StatsGlobal")
                {
                    result = timePlayedTotal;
                    if (Main.config.gamesWon > 0)
                        result += "\nGames completed " + Main.config.gamesWon;
                    result += "\nDeaths: " + Main.config.playerDeathsTotal;

                    result += "\n\nTime spent on feet: " + Main.config.timeWalkedTotal.Days + " days, " + Main.config.timeWalkedTotal.Hours + " hours, " + Main.config.timeWalkedTotal.Minutes + " minutes.";
                    result += "\nTime spent swimming: " + Main.config.timeSwamTotal.Days + " days, " + Main.config.timeSwamTotal.Hours + " hours, " + Main.config.timeSwamTotal.Minutes + " minutes.";
                    result += "\nTime spent sleeping: " + Main.config.timeSleptTotal.Days + " days, " + Main.config.timeSleptTotal.Hours + " hours, " + Main.config.timeSleptTotal.Minutes + " minutes.";
                    result += "\nTime spent in your life pod: " + Main.config.timeEscapePodTotal.Days + " days, " + Main.config.timeEscapePodTotal.Hours + " hours, " + Main.config.timeEscapePodTotal.Minutes + " minutes.";
                    result += "\nTime spent in your base: " + Main.config.timeBaseTotal.Days + " days, " + Main.config.timeBaseTotal.Hours + " hours, " + Main.config.timeBaseTotal.Minutes + " minutes.";
                    result += "\nTime spent in seamoth: " + Main.config.timeSeamothTotal.Days + " days, " + Main.config.timeSeamothTotal.Hours + " hours, " + Main.config.timeSeamothTotal.Minutes + " minutes.";
                    result += "\nTime spent in prawn suit: " + Main.config.timeExosuitTotal.Days + " days, " + Main.config.timeExosuitTotal.Hours + " hours, " + Main.config.timeExosuitTotal.Minutes + " minutes.";
                    result += "\nTime spent in cyclops: " + Main.config.timeCyclopsTotal.Days + " days, " + Main.config.timeCyclopsTotal.Hours + " hours, " + Main.config.timeCyclopsTotal.Minutes + " minutes.";

                    result += "\n\nDistance traveled: " + GetTraveledString(Main.config.distanceTraveledTotal);
                    result += "\nDistance traveled by foot: " + Main.config.distanceTraveledWalkTotal + " meters.";
                    result += "\nDistance traveled by swimming: " + Main.config.distanceTraveledSwimTotal + " meters.";
                    result += "\nDistance traveled by seaglide: " + Main.config.distanceTraveledSeaglideTotal + " meters.";
                    result += "\nDistance traveled in seamoth: " + Main.config.distanceTraveledSeamothTotal + " meters.";
                    result += "\nDistance traveled in prawn suit: " + Main.config.distanceTraveledExosuitTotal + " meters.";
                    result += "\nDistance traveled in cyclops: " + Main.config.distanceTraveledSubTotal + " meters.";
                    result += "\nMax depth reached: " + Main.config.maxDepthGlobal + " meters.";

                    result += "\n\nSeamoths constructed: " + Main.config.seamothsBuiltTotal;
                    result += "\nSeamoths lost: " + Main.config.seamothsLostTotal;
                    result += "\nPrawn suits constructed: " + Main.config.exosuitsBuiltTotal;
                    result += "\nPrawn suits lost: " + Main.config.exosuitsLostTotal;
                    result += "\nCyclopes constructed: " + Main.config.cyclopsBuiltTotal;
                    result += "\nCyclopes lost: " + Main.config.cyclopsLostTotal;

                    result += "\nHealth lost: " + Main.config.healthLostTotal;
                    result += "\nFirst aid kits used: " + Main.config.medkitsUsedTotal;

                    result += "\n\nWater drunk: " + Main.config.waterDrunkTotal + " liters.";
                    result += "\nFood eaten: " + foodEatenTotal + " kg.";
                    foreach (var kv in Main.config.foodEatenTotal)
                        result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value + " kg.";

                    result += "\n\nTotal power generated for your bases: " + basePowerTotal;
                    result += "\nBase corridor segments built: " + Main.config.baseCorridorsBuiltTotal;
                    result += "\nBase rooms built: " + baseRoomsBuiltTotal;
                    foreach (var kv in Main.config.baseRoomsBuiltTotal)
                        result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;

                    result += "\n\nObjects scanned: " + Main.config.objectsScannedTotal;
                    result += "\nBlueprints unlocked by scanning: " + Main.config.blueprintsUnlockedTotal;
                    result += "\nBlueprints found in databoxes: " + Main.config.blueprintsFromDataboxTotal;

                    result += "\n\nPlants killed: " + plantsKilledTotal;
                    foreach (var kv in Main.config.plantsKilledTotal)
                        result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;

                    result += "\n\nAnimals killed: " + animalsKilledTotal;
                    foreach (var kv in Main.config.animalsKilledTotal)
                        result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;

                    result += "\n\nCorals killed: " + coralKilledTotal;
                    foreach (var kv in Main.config.coralKilledTotal)
                        result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;

                    result += "\n\nLeviathans killed: " + leviathansKilledTotal;
                    foreach (var kv in Main.config.leviathansKilledTotal)
                        result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;

                    result += "\n\nItems crafted: " + itemsCraftedTotal;
                    foreach (var kv in Main.config.itemsCraftedTotal)
                        result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;

                    result += "\n\nResources used for crafting and constructing: " +  craftingResourcesUsedTotal.ToString("0.0") + " kg";
                    foreach (var kv in Main.config.craftingResourcesUsedTotal_)
                        result += "\n      " + GetCraftingResourcesUsedTotal(kv.Key);
             
                    result += "\n\nPlants raised: " + plantsRaisedTotal;
                    foreach (var kv in Main.config.plantsRaisedTotal)
                        result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;

                    result += "\n\nEggs hatched in AC: " + eggsHatchedTotal;
                    foreach (var kv in Main.config.eggsHatchedTotal)
                        result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;

                    result += "\n\nThings stored in your life pods: ";
                    foreach (var kv in Main.config.storedEscapePodTotal)
                    {
                        if (kv.Value > 0)
                            result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;
                    }

                    result += "\n\nThings stored in your bases: ";
                    foreach (var kv in Main.config.storedBaseTotal)
                    {
                        if (kv.Value > 0)
                            result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;
                    }

                    result += "\n\nThings stored in cyclops: ";
                    foreach (var kv in Main.config.storedSubTotal)
                    {
                        if (kv.Value > 0)
                            result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;
                    }

                    result += "\n\nThings stored outside your bases: ";
                    foreach (var kv in Main.config.storedOutsideTotal)
                    {
                        if (kv.Value > 0)
                            result += "\n      " + Language.main.Get(kv.Key) + " " + kv.Value;
                    }

                    result += "\n\nBiomes discovered:";
                    foreach (string biome in Main.config.biomesFoundGlobal)
                        result += "\n      " + biome;

                    result += "\n\nFauna species discovered: ";
                    foreach (string str in Main.config.faunaFoundTotal)
                        result += "\n      " + Language.main.Get(str);

                    result += "\n\nFlora species discovered: ";
                    foreach (string str in Main.config.floraFoundTotal)
                        result += "\n      " + Language.main.Get(str);

                    result += "\n\nCoral species discovered: ";
                    foreach (string str in Main.config.coralFoundTotal)
                        result += "\n      " + Language.main.Get(str);

                    result += "\n\nLeviathan species discovered: ";
                    foreach (string str in Main.config.leviathanFoundTotal)
                        result += "\n      " + Language.main.Get(str);
                }
            }
        }

        [HarmonyPatch(typeof(Player))]
        internal class Player_Patch
        {
            [HarmonyPrefix]
            [HarmonyPatch("TrackTravelStats")]
            public static bool TrackTravelStatsPrefix(Player __instance)
            {
                Vector3 position = __instance.transform.position;
                if (string.IsNullOrEmpty(saveSlot))
                    return false;

                __instance.maxDepth = Mathf.Max(__instance.maxDepth, -position.y);
                Main.config.maxDepth[saveSlot] = (int)__instance.maxDepth;
                Main.config.maxDepthGlobal = (int)__instance.maxDepth;
                string biomeName = GetBiomeName(LargeWorld.main.GetBiome(Player.main.transform.position));
                if (biomeName == "unknown")
                { }
                else
                {
                    Main.config.biomesFound[saveSlot].Add(biomeName);
                    Main.config.biomesFoundGlobal.Add(biomeName);
                }
                if (__instance.lastPosition != Vector3.zero) // first run after game loads it's Vector3.zero
                    __instance.distanceTraveled += Vector3.Distance(position, __instance.lastPosition);

                int distanceTraveled = Mathf.RoundToInt((__instance.lastPosition - position).magnitude);
                if (__instance.lastPosition == Vector3.zero)
                    distanceTraveled = 0;
                //Main.Log("lastPosition " + __instance.lastPosition);
                //Main.Log("position " + position);
                Main.config.distanceTraveled[saveSlot] += distanceTraveled;
                Main.config.distanceTraveledTotal += distanceTraveled;

                if (travelMode == TravelMode.Swim && Player.main.IsUnderwaterForSwimming())
                {
                    Main.config.distanceTraveledSwim[saveSlot] += distanceTraveled;
                    Main.config.distanceTraveledSwimTotal += distanceTraveled;
                }
                else if (travelMode == TravelMode.Seaglide && __instance.motorMode == Player.MotorMode.Seaglide)
                {
                    Main.config.distanceTraveledSeaglide[saveSlot] += distanceTraveled;
                    Main.config.distanceTraveledSeaglideTotal += distanceTraveled;
                }
                else if (travelMode == TravelMode.Seamoth && __instance.inSeamoth)
                {
                    Main.config.distanceTraveledSeamoth[saveSlot] += distanceTraveled;
                    Main.config.distanceTraveledSeamothTotal += distanceTraveled;
                }
                else if (travelMode == TravelMode.Exosuit && __instance.inExosuit)
                {
                    Main.config.distanceTraveledExosuit[saveSlot] += distanceTraveled;
                    Main.config.distanceTraveledExosuitTotal += distanceTraveled;
                }
                else if (travelMode == TravelMode.Sub && Player.main.mode == Player.Mode.Piloting)
                {
                    Main.config.distanceTraveledSub[saveSlot] += distanceTraveled;
                    Main.config.distanceTraveledSubTotal += distanceTraveled;
                }
                // MotorMode.Run not used
                else if (travelMode == TravelMode.Walk && __instance.motorMode == Player.MotorMode.Walk)
                {
                    Main.config.distanceTraveledWalk[saveSlot] += distanceTraveled;
                    Main.config.distanceTraveledWalkTotal += distanceTraveled;
                }
                __instance.lastPosition = position;

                if (Player.main.IsUnderwaterForSwimming())
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

                TimeSpan ts = GetTimePlayed() - timeLastUpdate;

                if (__instance.motorMode == Player.MotorMode.Walk)
                {
                    Main.config.timeWalked[saveSlot] += ts;
                    Main.config.timeWalkedTotal += ts;
                }
                else if (__instance.IsUnderwaterForSwimming())
                {
                    Main.config.timeSwam[saveSlot] += ts;
                    Main.config.timeSwamTotal += ts;
                }
                else if (Player.main.currentEscapePod)
                {
                    Main.config.timeEscapePod[saveSlot] += ts;
                    Main.config.timeEscapePodTotal += ts;
                }
                else if (__instance.inSeamoth)
                {
                    Main.config.timeSeamoth[saveSlot] += ts;
                    Main.config.timeSeamothTotal += ts;
                }
                else if (__instance.inExosuit)
                {
                    Main.config.timeExosuit[saveSlot] += ts;
                    Main.config.timeExosuitTotal += ts;
                }
                else if (Player.main.currentSub)
                {
                    if (Player.main.currentSub.isCyclops)
                    {
                        Main.config.timeCyclops[saveSlot] += ts;
                        Main.config.timeCyclopsTotal += ts;
                    }
                    else
                    {
                        Main.config.timeBase[saveSlot] += ts;
                        Main.config.timeBaseTotal += ts;
                    }
                }
                //AddDebug("timeSwam " + Main.config.timeSwam[saveSlot]);
                timeLastUpdate = GetTimePlayed();
                //Main.config.timePlayedTotal = DayNightCycle.main.timePassedSinceOrigin;
                return false;
            }
            [HarmonyPatch("OnKill")]
            [HarmonyPostfix]
            public static void OnKillPostfix(Player __instance)
            {
                Main.config.playerDeathsTotal++;
                if(!GameModeUtils.IsPermadeath())
					Main.config.playerDeaths[saveSlot] ++;
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
                    int dam = Mathf.RoundToInt(__result);
                    Main.config.healthLost[saveSlot] += dam;
                    Main.config.healthLostTotal += dam;
                }
            }
        }

        [HarmonyPatch(typeof(Survival))]
        class Survival_Patch
        {
            [HarmonyPatch("Use")]
            [HarmonyPostfix]
            public static void UsePostfix(Survival __instance, GameObject useObj, bool __result)
            {
                if (!__result)
                    return;

                TechType tt = CraftData.GetTechType(useObj);
                if (tt == TechType.FirstAidKit)
                {
                    //AddDebug("medkit used");
                    Main.config.medkitsUsed[saveSlot]++;
                    Main.config.medkitsUsedTotal++;
                }
            }
            [HarmonyPatch("Eat")]
            [HarmonyPostfix]
            public static void EatPostfix(Survival __instance, GameObject useObj, bool __result)
            {
                if (__result)
                {
                    Eatable eatable = useObj.GetComponent<Eatable>();
                    Rigidbody rb = useObj.GetComponent<Rigidbody>();
                    if (eatable && rb)
                    {
                        float foodValue = eatable.GetFoodValue();
                        float waterValue = eatable.GetWaterValue();
                        string name = CraftData.GetTechType(useObj).AsString();
                        if (foodValue >= waterValue)
                        {
                            if (Main.config.foodEaten[saveSlot].ContainsKey(name))
                                Main.config.foodEaten[saveSlot][name] += rb.mass;
                            else
                                Main.config.foodEaten[saveSlot][name] = rb.mass;

                            if (Main.config.foodEatenTotal.ContainsKey(name))
                                Main.config.foodEatenTotal[name] += rb.mass;
                            else
                                Main.config.foodEatenTotal[name] = rb.mass;
                        }
                        else if (waterValue > foodValue)
                        {
                            Main.config.waterDrunk[saveSlot] += rb.mass;
                            Main.config.waterDrunkTotal += rb.mass;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(LaunchRocket), "StartEndCinematic")]
        internal class LaunchRocket_StartEndCinematic_Patch
        {
            public static void Postfix(LaunchRocket __instance)
            {
                Main.config.gamesWon++;
                Main.config.Save();
            }
        }

        [HarmonyPatch(typeof(CreatureEgg), "Hatch")]
        internal class CreatureEgg_Hatch_Patch
        {
            public static void Postfix(CreatureEgg __instance)
            {
                //AddDebug("Hatch  " + __instance.hatchingCreature);
                if (__instance.hatchingCreature == TechType.None)
                    return;

                string name = __instance.hatchingCreature.AsString();
                if (Main.config.eggsHatchedTotal.ContainsKey(name))
                    Main.config.eggsHatchedTotal[name]++;
                else
                    Main.config.eggsHatchedTotal[name] = 1;

                if (Main.config.eggsHatched[saveSlot].ContainsKey(name))
                    Main.config.eggsHatched[saveSlot][name]++;
                else
                    Main.config.eggsHatched[saveSlot][name] = 1;
            }
        }

        [HarmonyPatch(typeof(GrowingPlant), "SpawnGrownModel")]
        internal class GrowingPlant_SpawnGrownModel_Patch
        {
            public static void Postfix(GrowingPlant __instance)
            {
                if (__instance.seed.plantTechType == TechType.None)
                    return;

                string name = __instance.seed.plantTechType.AsString();
                //AddDebug("GrownPlant Awake " + tt);

                if (Main.config.plantsRaisedTotal.ContainsKey(name))
                    Main.config.plantsRaisedTotal[name]++;
                else
                    Main.config.plantsRaisedTotal[name] = 1;

                if (Main.config.plantsRaised[saveSlot].ContainsKey(name))
                    Main.config.plantsRaised[saveSlot][name]++;
                else
                    Main.config.plantsRaised[saveSlot][name] = 1;
            }
        }

        [HarmonyPatch(typeof(LiveMixin))]
        internal class LiveMixin_Patch
        {
            [HarmonyPatch("Kill")]
            [HarmonyPrefix]
            public static void KillPrefix(LiveMixin __instance)
            {
                //TechType tt = CraftData.GetTechType(__instance.gameObject);
                //AddDebug("Kill " + tt);
                killedLM = __instance;
            }
            [HarmonyPatch("TakeDamage")]
            [HarmonyPostfix]
            public static void TakeDamagePostfix(LiveMixin __instance, float originalDamage, Vector3 position, DamageType type, GameObject dealer)
            {
                if (dealer && killedLM && __instance == killedLM)
                {
                    //AddDebug(__instance.name + " killed by " + dealer.name);
                    if (dealer == Player.main.gameObject || dealer == Player.main.currentMountedVehicle?.gameObject || dealer == Player.main.currentSub?.gameObject)
                    {
                        TechType tt = CraftData.GetTechType(__instance.gameObject);
                        if (tt == TechType.None)
                            return;
                        //AddDebug(tt + " killed by player");
                        string name = tt.AsString();
                        if (fauna.Contains(tt))
                        {
                            //AddDebug(tt + " animal killed by player");
                            if (Main.config.animalsKilledTotal.ContainsKey(name))
                                Main.config.animalsKilledTotal[name]++;
                            else
                                Main.config.animalsKilledTotal[name] = 1;

                            if (Main.config.animalsKilled[saveSlot].ContainsKey(name))
                                Main.config.animalsKilled[saveSlot][name]++;
                            else
                                Main.config.animalsKilled[saveSlot][name] = 1;
                        }
                        else if (flora.Contains(tt))
                        {
                            //AddDebug(tt + " plant killed by player");
                            if (Main.config.plantsKilledTotal.ContainsKey(name))
                                Main.config.plantsKilledTotal[name]++;
                            else
                                Main.config.plantsKilledTotal[name] = 1;

                            if (Main.config.plantsKilled[saveSlot].ContainsKey(name))
                                Main.config.plantsKilled[saveSlot][name]++;
                            else
                                Main.config.plantsKilled[saveSlot][name] = 1;
                        }
                        else if (coral.Contains(tt))
                        {
                            //AddDebug(tt + " coral killed by player");
                            if (Main.config.coralKilledTotal.ContainsKey(name))
                                Main.config.coralKilledTotal[name]++;
                            else
                                Main.config.coralKilledTotal[name] = 1;

                            if (Main.config.coralKilled[saveSlot].ContainsKey(name))
                                Main.config.coralKilled[saveSlot][name]++;
                            else
                                Main.config.coralKilled[saveSlot][name] = 1;
                        }
                        else if (leviathans.Contains(tt))
                        {
                            //AddDebug(tt + " leviathan killed by player");
                            if (Main.config.leviathansKilled[saveSlot].ContainsKey(name))
                                Main.config.leviathansKilled[saveSlot][name]++;
                            else
                                Main.config.leviathansKilled[saveSlot][name] = 1;

                            if (Main.config.leviathansKilledTotal.ContainsKey(name))
                                Main.config.leviathansKilledTotal[name]++;
                            else
                                Main.config.leviathansKilledTotal[name] = 1;
                        }
                        killedLM = null;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Inventory))]
        internal class Inventory_Patch
        { // runs when you have enough ingredients
          // static TechType tt = TechType.None;
            [HarmonyPatch("ConsumeResourcesForRecipe")]
            [HarmonyPrefix]
            public static void ConsumeResourcesForRecipePrefix(DamageOnPickup __instance, TechType techType)
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
            [HarmonyPostfix]
            [HarmonyPatch("ConsumeResourcesForRecipe")]
            public static void Postfix(DamageOnPickup __instance, TechType techType)
            {
                removingItemsForRecipe = false;
            }
            [HarmonyPostfix]
            [HarmonyPatch("OnRemoveItem")]
            public static void OnRemoveItemPostfix(InventoryItem item)
            {
                if (string.IsNullOrEmpty(saveSlot) || !removingItemsForRecipe)
                    return;

                TechType tt = item.item.GetTechType();
                if (tt == TechType.None)
                    return;

                LiveMixin liveMixin = item.item.GetComponent<LiveMixin>();
                Rigidbody rb = item.item.GetComponent<Rigidbody>();
                bool alive = liveMixin && liveMixin.IsAlive();
                string name = tt.AsString();

                if (alive || liveMixin == null)
                {
                    //TechType tt = CraftData.GetTechType(item.item.gameObject);

                    if (fauna.Contains(tt))
                    {
                        //AddDebug(tt + " animal killed by player");
                        if (Main.config.animalsKilledTotal.ContainsKey(name))
                            Main.config.animalsKilledTotal[name]++;
                        else
                            Main.config.animalsKilledTotal[name] = 1;

                        if (Main.config.animalsKilled[saveSlot].ContainsKey(name))
                            Main.config.animalsKilled[saveSlot][name]++;
                        else
                            Main.config.animalsKilled[saveSlot][name] = 1;
                    }
                    else if (flora.Contains(tt))
                    {
                        //AddDebug(tt + " plant killed by player");
                        if (Main.config.plantsKilledTotal.ContainsKey(name))
                            Main.config.plantsKilledTotal[name]++;
                        else
                            Main.config.plantsKilledTotal[name] = 1;

                        if (Main.config.plantsKilled[saveSlot].ContainsKey(name))
                            Main.config.plantsKilled[saveSlot][name]++;
                        else
                            Main.config.plantsKilled[saveSlot][name] = 1;
                    }
                }

                if (item.item.GetComponent<Eatable>())
                    return;

                if (Main.config.craftingResourcesUsed_[saveSlot].ContainsKey(name))
                    Main.config.craftingResourcesUsed_[saveSlot][name]++;
                else
                    Main.config.craftingResourcesUsed_[saveSlot][name] = 1;

                if (Main.config.craftingResourcesUsedTotal_.ContainsKey(name))
                    Main.config.craftingResourcesUsedTotal_[name]++;
                else
                    Main.config.craftingResourcesUsedTotal_[name] = 1;

                if (rb)
                {
                    if (Main.config.craftingResourcesUsed[saveSlot].ContainsKey(name))
                        Main.config.craftingResourcesUsed[saveSlot][name] += rb.mass;
                    else
                        Main.config.craftingResourcesUsed[saveSlot][name] = rb.mass;

                    if (Main.config.craftingResourcesUsedTotal.ContainsKey(name))
                        Main.config.craftingResourcesUsedTotal[name] += rb.mass;
                    else
                        Main.config.craftingResourcesUsedTotal[name] = rb.mass;
                }
            }
        }

        [HarmonyPatch(typeof(ItemsContainer))]
        internal class ItemsContainer_Patch
        {
            [HarmonyPatch("NotifyAddItem")]
            [HarmonyPostfix]
            public static void NotifyAddItemPostfix(ItemsContainer __instance, InventoryItem item)
            {
                if (!Main.setupDone || Inventory.main.usedStorage.Count == 0 || Inventory.main._container == __instance || __instance.tr.parent.GetComponent<Trashcan>())
                    return;
                //AddDebug("NotifyAddItem " + __instance.tr.name);
                TechType tt = item.item.GetTechType();
                string name = tt.AsString();
                Rigidbody rb = item.item.GetComponent<Rigidbody>();
                if (tt == TechType.None || rb == null)
                    return;

                if (Player.main.currentSub)
                {
                    if (Player.main.currentSub.isCyclops)
                    {
                        //AddDebug("NotifyAddItem IsInBase " + tt);
                        if (Main.config.storedSub[saveSlot].ContainsKey(name))
                            Main.config.storedSub[saveSlot][name]++;
                        else
                            Main.config.storedSub[saveSlot][name] = 1;

                        if (Main.config.storedSubTotal.ContainsKey(name))
                            Main.config.storedSubTotal[name]++;
                        else
                            Main.config.storedSubTotal[name] = 1;
                    }
                    else
                    {
                        if (Main.config.storedBase[saveSlot].ContainsKey(name))
                            Main.config.storedBase[saveSlot][name]++;
                        else
                            Main.config.storedBase[saveSlot][name] = 1;

                        if (Main.config.storedBaseTotal.ContainsKey(name))
                            Main.config.storedBaseTotal[name]++;
                        else
                            Main.config.storedBaseTotal[name] = 1;
                    }
                }
                else if (Player.main._currentEscapePod)
                {
                    //AddDebug("NotifyAddItem EscapePod " + tt);
                    if (Main.config.storedEscapePod[saveSlot].ContainsKey(name))
                        Main.config.storedEscapePod[saveSlot][name]++;
                    else
                        Main.config.storedEscapePod[saveSlot][name] = 1;

                    if (Main.config.storedEscapePodTotal.ContainsKey(name))
                        Main.config.storedEscapePodTotal[name]++;
                    else
                        Main.config.storedEscapePodTotal[name] = 1;
                }
                else
                {
                    if (Main.config.storedOutside[saveSlot].ContainsKey(name))
                        Main.config.storedOutside[saveSlot][name]++;
                    else
                        Main.config.storedOutside[saveSlot][name] = 1;

                    if (Main.config.storedOutsideTotal.ContainsKey(name))
                        Main.config.storedOutsideTotal[name]++;
                    else
                        Main.config.storedOutsideTotal[name] = 1;
                }
            }
            [HarmonyPatch("NotifyRemoveItem")]
            [HarmonyPostfix]
            public static void NotifyRemoveItemPostfix(ItemsContainer __instance, InventoryItem item)
            {
                if (!Main.setupDone || Inventory.main.usedStorage.Count == 0 || Inventory.main._container == __instance || __instance.tr.parent.GetComponent<Trashcan>())
                    return;

                //AddDebug("NotifyRemoveItem " + __instance.tr.name);
                //AddDebug("NotifyRemoveItem " + saveSlot);
                TechType tt = item.item.GetTechType();
                string name = tt.AsString();
                Rigidbody rb = item.item.GetComponent<Rigidbody>();
                if (tt == TechType.None || rb == null)
                    return;
                if (Player.main.currentSub)
                {
                    if (Player.main.currentSub.isCyclops)
                    {
                        //AddDebug("NotifyRemoveItem isCyclops " + tt); 
                        if (Main.config.storedSub[saveSlot].ContainsKey(name) && Main.config.storedSub[saveSlot][name] > 0)
                            Main.config.storedSub[saveSlot][name]--;

                        if (Main.config.storedSubTotal.ContainsKey(name) && Main.config.storedSubTotal[name] > 0)
                            Main.config.storedSubTotal[name]--;
                    }
                    else
                    {
                        //AddDebug("NotifyRemoveItem IsInBase " + tt);
                        if (Main.config.storedBase[saveSlot].ContainsKey(name) && Main.config.storedBase[saveSlot][name] > 0)
                            Main.config.storedBase[saveSlot][name]--;

                        if (Main.config.storedBaseTotal.ContainsKey(name) && Main.config.storedBaseTotal[name] > 0)
                            Main.config.storedBaseTotal[name]--;
                    }
                }
                else if (Player.main._currentEscapePod)
                {
                    //AddDebug("NotifyRemoveItem EscapePod " + tt);
                    if (Main.config.storedEscapePod[saveSlot].ContainsKey(name) && Main.config.storedEscapePod[saveSlot][name] > 0)
                        Main.config.storedEscapePod[saveSlot][name]--;

                    if (Main.config.storedEscapePodTotal.ContainsKey(name) && Main.config.storedEscapePodTotal[name] > 0)
                        Main.config.storedEscapePodTotal[name]--;
                }
                else
                {
                    //AddDebug("NotifyRemoveItem " + tt);
                    if (Main.config.storedOutside[saveSlot].ContainsKey(name) && Main.config.storedOutside[saveSlot][name] > 0)
                        Main.config.storedOutside[saveSlot][name]--;

                    if (Main.config.storedOutsideTotal.ContainsKey(name) && Main.config.storedOutsideTotal[name] > 0)
                        Main.config.storedOutsideTotal[name]--;
                }
            }
        }

        [HarmonyPatch(typeof(BlueprintHandTarget), "UnlockBlueprint")]
        internal class BlueprintHandTarget_UnlockBlueprint_Patch
        {
            public static void Prefix(BlueprintHandTarget __instance)
            {
                if (string.IsNullOrEmpty(saveSlot) || __instance.used)
                    return;

                if (!KnownTech.Contains(__instance.unlockTechType))
                {
                    //AddDebug("unlock  " + __instance.unlockTechType);
                    Main.config.blueprintsFromDatabox[saveSlot]++;
                    Main.config.blueprintsFromDataboxTotal++;
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
                    Main.config.objectsScannedTotal++;
                    TechType tt = PDAScanner.scanTarget.techType;
                    string name = tt.AsString();

                    if (fauna.Contains(tt))
                    {
                        //AddDebug("scanned creature");
                        Main.config.faunaFound[saveSlot].Add(name);
                        Main.config.faunaFoundTotal.Add(name);
                    }
                    else if (flora.Contains(tt))
                    {
                        //AddDebug("scanned flora"); 
                        if (tt == TechType.SmallKoosh || tt == TechType.MediumKoosh || tt == TechType.LargeKoosh || tt == TechType.HugeKoosh)
                        {
                            if (Main.config.kooshFound[saveSlot])
                                return;
                            Main.config.kooshFound[saveSlot] = true;
                        }
                        Main.config.floraFound[saveSlot].Add(name);
                        Main.config.floraFoundTotal.Add(name);
                    }
                    else if (coral.Contains(tt))
                    {
                        //AddDebug("scanned coral");
                        if (tt == TechType.BlueJeweledDisk || tt == TechType.GreenJeweledDisk || tt == TechType.PurpleJeweledDisk || tt == TechType.RedJeweledDisk || tt == TechType.GenericJeweledDisk)
                        {
                            if (Main.config.jeweledDiskFound[saveSlot])
                                return;
                            Main.config.jeweledDiskFound[saveSlot] = true;
                        }
                        Main.config.coralFound[saveSlot].Add(name);
                        Main.config.coralFoundTotal.Add(name);
                    }
                    else if (leviathans.Contains(tt))
                    {
                        //AddDebug("scanned leviathan");
                        if (tt == TechType.GhostLeviathan || tt == TechType.GhostLeviathanJuvenile)
                        {
                            if (Main.config.ghostLevFound[saveSlot])
                                return;
                            Main.config.ghostLevFound[saveSlot] = true;
                        }
                        Main.config.leviathanFound[saveSlot].Add(name);
                        Main.config.leviathanFoundTotal.Add(name);
                    }

                }

            }
        }

        [HarmonyPatch(typeof(PDAScanner), "Unlock")]
        internal class PDAScanner_Unlock_Patch
        {
            public static void Postfix(PDAScanner.EntryData entryData, bool unlockBlueprint, bool unlockEncyclopedia, bool verbose)
            {
                if (entryData == null || !verbose)
                    return;

                //AddDebug("unlock  " + entryData.key);
                if (unlockBlueprint && entryData.blueprint != TechType.None)
                { // scanning bladderfish unlocks bladderfish blueprint
                    if (fauna.Contains(entryData.blueprint) || flora.Contains(entryData.blueprint) || coral.Contains(entryData.blueprint) || leviathans.Contains(entryData.blueprint))
                        return;
                    //AddDebug("unlock Blueprint " + entryData.key);
                    Main.config.blueprintsUnlocked[saveSlot]++;
                    Main.config.blueprintsUnlockedTotal++;
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

        [HarmonyPatch(typeof(Constructable))]
        internal class Constructable_Patch
        {
            [HarmonyPatch("Construct")]
            [HarmonyPostfix]
            public static void ConstructPostfix(Constructable __instance)
            {
                if (__instance.constructedAmount >= 1f)
                {
                    //AddDebug(" constructed " + __instance.techType);
                    if (__instance.techType == TechType.None)
                        return;

                    foreach (TechType tt in __instance.resourceMap)
                    {
                        //AddDebug("resourceMap  " + tt);
                        GameObject prefab = CraftData.GetPrefabForTechType(tt);
                        //if (prefab.GetComponent<Eatable>())
                        //    return;
                        if (prefab)
                        {
                            string name = tt.AsString();
                            if (Main.config.craftingResourcesUsed_[saveSlot].ContainsKey(name))
                                Main.config.craftingResourcesUsed_[saveSlot][name]++;
                            else
                                Main.config.craftingResourcesUsed_[saveSlot][name] = 1;

                            if (Main.config.craftingResourcesUsedTotal_.ContainsKey(name))
                                Main.config.craftingResourcesUsedTotal_[name]++;
                            else
                                Main.config.craftingResourcesUsedTotal_[name] = 1;

                            Rigidbody rb = prefab.GetComponent<Rigidbody>();
                            if (rb)
                            {
                                if (Main.config.craftingResourcesUsed[saveSlot].ContainsKey(name))
                                    Main.config.craftingResourcesUsed[saveSlot][name] += rb.mass;
                                else
                                    Main.config.craftingResourcesUsed[saveSlot][name] = rb.mass;

                                if (Main.config.craftingResourcesUsedTotal.ContainsKey(name))
                                    Main.config.craftingResourcesUsedTotal[name] += rb.mass;
                                else
                                    Main.config.craftingResourcesUsedTotal[name] = rb.mass;
                            }
                        }
                    }

                    if (roomTypes.Contains(__instance.techType))
                    {
                        string name = __instance.techType.AsString();
                        if (Main.config.baseRoomsBuilt[saveSlot].ContainsKey(name))
                            Main.config.baseRoomsBuilt[saveSlot][name]++;
                        else
                            Main.config.baseRoomsBuilt[saveSlot][name] = 1;

                        if (Main.config.baseRoomsBuiltTotal.ContainsKey(name))
                            Main.config.baseRoomsBuiltTotal[name]++;
                        else
                            Main.config.baseRoomsBuiltTotal[name] = 1;
                    }
                    else if (corridorTypes.Contains(__instance.techType))
                    {
                        Main.config.baseCorridorsBuilt[saveSlot]++;
                        Main.config.baseCorridorsBuiltTotal++;
                    }
                }
            }
            [HarmonyPatch("Deconstruct")]
            [HarmonyPostfix]
            public static void DeconstructPostfix(Constructable __instance)
            {
                if (__instance.constructedAmount <= 0f)
                {
                    //AddDebug(" deconstructed " + __instance.techType);
                    if (roomTypes.Contains(__instance.techType))
                    {
                        string name = __instance.techType.AsString();
                        if (Main.config.baseRoomsBuilt[saveSlot].ContainsKey(name) && Main.config.baseRoomsBuilt[saveSlot][name] > 0)
                            Main.config.baseRoomsBuilt[saveSlot][name]--;

                        if (Main.config.baseRoomsBuiltTotal.ContainsKey(name) && Main.config.baseRoomsBuiltTotal[name] > 0)
                            Main.config.baseRoomsBuiltTotal[name]--;
                    }
                    else if (corridorTypes.Contains(__instance.techType))
                    {
                        if (Main.config.baseCorridorsBuilt[saveSlot] > 0)
                            Main.config.baseCorridorsBuilt[saveSlot]--;
                        if (Main.config.baseCorridorsBuiltTotal > 0)
                            Main.config.baseCorridorsBuiltTotal--;
                    }
                }
            }

        }

        [HarmonyPatch(typeof(SubRoot))]
        class SubRoot_Patch
        {
            [HarmonyPatch("Start")]
            [HarmonyPostfix]
            static void StartPostfix(SubRoot __instance)
            {
                if (!__instance.isCyclops && __instance.powerRelay)
                    powerRelays.Add(__instance.powerRelay);
            }
            [HarmonyPatch("OnKill")]
            [HarmonyPostfix]
            public static void OnKillPostfix(SubRoot __instance)
            { // assuming player uses 1 sub
              //AddDebug("Sub lost");
                Main.config.cyclopsLost[saveSlot]++;
                Main.config.cyclopsLostTotal++;

                foreach (var kv in Main.config.storedSub[saveSlot])
                {
                    if (Main.config.storedSubTotal.ContainsKey(kv.Key))
                        Main.config.storedSubTotal[kv.Key] -= kv.Value;

                    if (Main.config.storedOutside[saveSlot].ContainsKey(kv.Key))
                        Main.config.storedOutside[saveSlot][kv.Key] += kv.Value;
                    else
                        Main.config.storedOutside[saveSlot][kv.Key] = 1;

                    if (Main.config.storedOutsideTotal.ContainsKey(kv.Key))
                        Main.config.storedOutsideTotal[kv.Key] += kv.Value;
                    else
                        Main.config.storedOutsideTotal[kv.Key] = 1;
                }
                Main.config.storedSub[saveSlot] = new Dictionary<string, int>();
            }
        }

        [HarmonyPatch(typeof(uGUI_EncyclopediaTab), "Open")]
        internal class uGUI_EncyclopediaTab_Close_Patch
        {
            public static void Postfix(uGUI_EncyclopediaTab __instance)
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
                    Main.config.seamothsLostTotal++;
                }
                else if (__instance is Exosuit)
                {
                    //AddDebug("Exosuit lost");
                    Main.config.exosuitsLost[saveSlot]++;
                    Main.config.exosuitsLostTotal++;
                }
            }
        }

        [HarmonyPatch(typeof(Constructor), "OnConstructionDone")]
        internal class Constructor_OnConstructionDone_Patch
        {
            public static void Postfix(Constructor __instance, GameObject constructedObject)
            {
                //TechType tt = CraftData.GetTechType(constructedObject);
                //AddDebug("built tt " + tt);
                //AddDebug("built " + constructedObject.name);
                //if (tt == TechType.Seamoth)
                if (constructedObject.GetComponent<SeaMoth>())
                {
                    Main.config.seamothsBuilt[saveSlot] ++;
                    Main.config.seamothsBuiltTotal++;
                }
                //else if (tt == TechType.Exosuit)
                else if (constructedObject.GetComponent<Exosuit>())
                {
                    Main.config.exosuitsBuilt[saveSlot] ++;
                    Main.config.exosuitsBuiltTotal++;
                }
                else if (constructedObject.GetComponent<SubRoot>())
                { // tt is none
                    Main.config.cyclopsBuilt[saveSlot] ++;
                    Main.config.cyclopsBuiltTotal++;
                }
            }
        }

        [HarmonyPatch(typeof(CrafterLogic))]
        internal class GhostCrafter_TryPickupSingle_Patch
        {
            [HarmonyPatch("TryPickupSingle")]
            [HarmonyPrefix]
            public static void TryPickupSinglePrefix(CrafterLogic __instance, TechType techType)
            {
                //AddDebug("TryPickupSingle " + techType);
                craftedItem = true;
            }
            [HarmonyPatch("NotifyCraftEnd")]
            [HarmonyPostfix]
            public static void NotifyCraftEndPostfix(CrafterLogic __instance, GameObject target, TechType techType)
            {
                //GameObject prefab = CraftData.GetPrefabForTechType(techType);
                if (!craftedItem)
                    return;
                craftedItem = false;
                if (target && target.GetComponent<Eatable>())
                    return;
                //AddDebug("NotifyCraftEnd " + techType);
                string name = techType.AsString();

                if (Main.config.itemsCrafted[saveSlot].ContainsKey(name))
                    Main.config.itemsCrafted[saveSlot][name]++;
                else
                    Main.config.itemsCrafted[saveSlot][name] = 1;

                if (Main.config.itemsCraftedTotal.ContainsKey(name))
                    Main.config.itemsCraftedTotal[name]++;
                else
                    Main.config.itemsCraftedTotal[name] = 1;
            }
        }

        [HarmonyPatch(typeof(Bed))]
        internal class Bed_Patch
        {
            [HarmonyPatch("EnterInUseMode")]
            [HarmonyPostfix]
            public static void EnterInUseModePostfix(Bed __instance)
            {
                bedTimeStart = GetTimePlayed();
                //AddDebug("EnterInUseMode " );
            }
            [HarmonyPatch("ExitInUseMode")]
            [HarmonyPostfix]
            public static void ExitInUseModePostfix(Bed __instance)
            {
                TimeSpan ts = GetTimePlayed() - bedTimeStart;
                Main.config.timeSlept[saveSlot] += ts;
                Main.config.timeSleptTotal += ts;
                //AddDebug("ExitInUseMode " );
            }
        }

        //[HarmonyPatch(typeof(PDAEncyclopedia), "Add", new Type[] { typeof(string), typeof(PDAEncyclopedia.Entry), typeof(bool) })]
        internal class PDAEncyclopedia_Add_Patch
        {
            public static void Postfix(string key, PDAEncyclopedia.Entry entry)
            {
                //uGUI_ListEntry uGuiListEntry = item as uGUI_ListEntry;
                //AddDebug("Add " + key);
                //Main.Log("Add " + key);
            }
        }

        //[HarmonyPatch(typeof(PDAEncyclopedia), "OnAdd")]
        internal class PDAEncyclopedia_OnAdd_Patch
        {
            public static void Postfix(CraftNode node, bool verbose)
            {
                //uGUI_ListEntry uGuiListEntry = item as uGUI_ListEntry;
                //AddDebug("Add " + node.id);
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

                //AddDebug("NotifyRemove " + entry.techType);
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