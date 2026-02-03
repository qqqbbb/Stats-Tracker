using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static ErrorMessage;


namespace Stats_Tracker
{
    internal class Stats_Display
    {
        public static string saveSlot;
        public static readonly Dictionary<string, string> myEntries = new Dictionary<string, string> { { "ST_Stats_global_dist", "ST_stats/global" },
        { "ST_Stats_global_time", "ST_stats/global" },
        { "ST_Stats_this_game_time", "ST_stats/this_game" },
        { "ST_Stats_this_game_dist", "ST_stats/this_game" },
        { "ST_Stats_global_food", "ST_stats/global" },
        { "ST_Stats_global_base", "ST_stats/global" },
        { "ST_Stats_this_game_base", "ST_stats/this_game" },
        { "ST_Stats_this_game_crafting", "ST_stats/this_game" },
        { "ST_Stats_global_crafting", "ST_stats/global" },
        { "ST_Stats_global_farming", "ST_stats/global" },
        { "ST_Stats_this_game_farming", "ST_stats/this_game" },
        { "ST_Stats_this_game_kills", "ST_stats/this_game" },
        { "ST_Stats_global_kills", "ST_stats/global" },
        { "ST_Stats_this_game_misc", "ST_stats/this_game" },
        { "ST_Stats_global_misc", "ST_stats/global" },
        { "ST_Stats_global_discover", "ST_stats/global" },
        { "ST_Stats_this_game_discover", "ST_stats/this_game" },
        { "ST_Stats_ency_tech", "ST_stats/this_game/ency" },
        { "ST_Stats_ency_lifeform", "ST_stats/this_game/ency" },
        { "ST_Stats_ency_data", "ST_stats/this_game/ency" },
        { "ST_Stats_ency_misc", "ST_stats/this_game/ency" },
        };

        static HashSet<string> missingEncyEntries = new HashSet<string> { "CoralSample", "LifepodCaptainsQuartersCode", "LifepodCTODialog1", "LifepodCTODialog2", "LifepodCTOLog1", "LifepodCTOLog2", "DeepPDABase3", "Aurora_RingRoom_Terminal1", "AuroraEngineeringLog", "InnerBiomeWreckLore8", "OuterBiomeWreckLore7", "InnerBiomeWreckLore6", "OuterBiomeWreckLore8", "SeaEmperorLeviathan", "IslandsPDAPaal", "Aurora_RingRoom_Terminal2", "DataCoil", "RaysAdvanced", "SeaEmperorEggHatchingEnzymes" };

        public static void CreateEntry(string key, string path)
        {
            //Main.logger.LogInfo($"CreateEntry {key} {path}");
            PDAEncyclopedia.EntryData entryData = new PDAEncyclopedia.EntryData()
            {
                path = path,
                key = key,
                kind = PDAEncyclopedia.EntryData.Kind.Encyclopedia,
                unlocked = true,
                nodes = path.Split('/')
            };
            PDAHandler.AddEncyclopediaEntry(entryData);
            //PDAEncyclopedia.mapping[key] = entryData;
            //PDAEncyclopedia.Add(key, false);
        }

        public static void CreateMyEntries()
        {
            if (GameModeUtils.RequiresSurvival())
                myEntries["ST_Stats_this_game_food"] = "ST_stats/this_game";

            foreach (var kv in myEntries)
                CreateEntry(kv.Key, kv.Value);
        }

        public static void AddMyEntries()
        {
            foreach (var kv in myEntries)
                PDAEncyclopedia.Add(kv.Key, false);
        }

        [HarmonyPatch(typeof(uGUI_EncyclopediaTab), "DisplayEntry")]
        internal class uGUI_EncyclopediaTab_DisplayEntry_Patch
        {
            public static bool Prefix(uGUI_EncyclopediaTab __instance, string key)
            {
                //AddDebug($"DisplayEntry {key} {myEntries.ContainsKey(key)}");
                if (myEntries.ContainsKey(key) == false)
                    return true;

                __instance.textBuilder.Clear();
                __instance.SetTitle(null);
                __instance.SetImage(null);
                __instance.SetAudio(null);
                //int paragraphSpacing = 0;
                //__instance.textBuilder.Append(GetText(key));
                __instance.SetText(GetText(key, __instance.textBuilder));
                //__instance.message.SetText(GetText(key));
                __instance.message.paragraphSpacing = 0;
                //__instance.SetProgress(-1f);
                return false;
            }

            private static StringBuilder GetText(string key, StringBuilder sb)
            {
                switch (key)
                {
                    case "ST_Stats_global_time":
                        GetTimeStats(sb, true);
                        break;
                    case "ST_Stats_this_game_time":
                        GetTimeStats(sb, false);
                        break;
                    case "ST_Stats_global_dist":
                        GetTravelStats(sb, true);
                        break;
                    case "ST_Stats_this_game_dist":
                        GetTravelStats(sb, false);
                        break;
                    case "ST_Stats_global_food":
                        GetFoodStats(sb, true);
                        break;
                    case "ST_Stats_this_game_food":
                        GetFoodStats(sb, false);
                        break;
                    case "ST_Stats_global_base":
                        GetBaseStats(sb, true);
                        break;
                    case "ST_Stats_this_game_base":
                        GetBaseStats(sb, false);
                        break;
                    case "ST_Stats_global_crafting":
                        GetCraftingStats(sb, true);
                        break;
                    case "ST_Stats_this_game_crafting":
                        GetCraftingStats(sb, false);
                        break;
                    case "ST_Stats_global_farming":
                        GetFarmingStats(sb, true);
                        break;
                    case "ST_Stats_this_game_farming":
                        GetFarmingStats(sb, false);
                        break;
                    case "ST_Stats_global_kills":
                        GetKilledStats(sb, true);
                        break;
                    case "ST_Stats_this_game_kills":
                        GetKilledStats(sb, false);
                        break;
                    case "ST_Stats_global_misc":
                        GetMiscStats(sb, true);
                        break;
                    case "ST_Stats_this_game_misc":
                        GetMiscStats(sb, false);
                        break;
                    case "ST_Stats_global_discover":
                        GetDiscoverStats(sb, true);
                        break;
                    case "ST_Stats_this_game_discover":
                        GetDiscoverStats(sb, false);
                        break;
                    case "ST_Stats_ency_lifeform":
                        GetEncyLifeformStats(sb);
                        break;
                    case "ST_Stats_ency_tech":
                        GetEncyTechStats(sb);
                        break;
                    case "ST_Stats_ency_data":
                        GetEncyDataStats(sb);
                        break;
                    case "ST_Stats_ency_misc":
                        GetEncyMiscStats(sb);
                        break;
                }
                return sb;
            }
        }

        private static void GetFarmingStats(StringBuilder sb, bool global)
        {
            if (global)
            {
                AppendDic(sb, GetDicGlobal(Main.config.plantsGrown), "ST_plants_grown");
                AppendDic(sb, GetDicGlobal(Main.config.eggsHatched), "ST_hatched_eggs");
                AppendDic(sb, GetDicGlobal(Main.config.creaturesBred), "ST_creatures_bred");
                return;
            }
            GetModDisabledString(sb);
            AppendDic(sb, MergeDics(Main.config.plantsGrown, UnsavedData.plantsGrown), "ST_plants_grown");
            AppendDic(sb, MergeDics(Main.config.eggsHatched, UnsavedData.eggsHatched), "ST_hatched_eggs");
            AppendDic(sb, MergeDics(Main.config.creaturesBred, UnsavedData.creaturesBred), "ST_creatures_bred");
        }

        private static void GetCraftingStats(StringBuilder sb, bool global)
        {
            if (global)
            {
                AppendDic(sb, GetDicGlobal(Main.config.constructorBuilt), "ST_constructor_built");
                AppendDic(sb, GetDicGlobal(Main.config.builderToolBuilt), "ST_builder_tool_built");
                AppendDic(sb, GetDicGlobal(Main.config.itemsCrafted), "ST_items_crafted");
                return;
            }
            GetModDisabledString(sb);
            AppendDic(sb, MergeDics(Main.config.constructorBuilt, UnsavedData.constructorBuilt), "ST_constructor_built");
            AppendDic(sb, MergeDics(Main.config.builderToolBuilt, UnsavedData.builderToolBuilt), "ST_builder_tool_built");
            AppendDic(sb, MergeDics(Main.config.itemsCrafted, UnsavedData.itemsCrafted), "ST_items_crafted");
        }

        private static void GetKilledStats(StringBuilder sb, bool global)
        {
            if (global)
            {
                AppendDic(sb, GetDicGlobal(Main.config.plantsKilled), "ST_plants_killed");
                AppendDic(sb, GetDicGlobal(Main.config.animalsKilled), "ST_animals_killed");
                AppendDic(sb, GetDicGlobal(Main.config.coralKilled), "ST_corals_killed");
                AppendDic(sb, GetDicGlobal(Main.config.leviathansKilled), "ST_leviathans_killed");
                return;
            }
            GetModDisabledString(sb);
            AppendDic(sb, MergeDics(Main.config.plantsKilled, UnsavedData.plantsKilled), "ST_plants_killed");
            AppendDic(sb, MergeDics(Main.config.animalsKilled, UnsavedData.animalsKilled), "ST_animals_killed");
            AppendDic(sb, MergeDics(Main.config.coralKilled, UnsavedData.coralKilled), "ST_corals_killed");
            AppendDic(sb, MergeDics(Main.config.leviathansKilled, UnsavedData.leviathansKilled), "ST_leviathans_killed");
        }

        private static void GetTravelStats(StringBuilder sb, bool global)
        {
            string tempSuffix = GetTempSuffix();
            if (global)
            {
                AppendTravelLine(sb, "ST_distance_traveled", Main.config.distanceTraveled.Values.Sum());
                AppendTravelLine(sb, "ST_distance_swam", Main.config.distanceTraveledSwim.Values.Sum());
                AppendTravelLine(sb, "ST_distance_walked", Main.config.distanceTraveledWalk.Values.Sum());
                AppendTravelLine(sb, "ST_distance_seaglide", Main.config.distanceTraveledSeaglide.Values.Sum());
                AppendTravelLine(sb, "ST_max_depth", Main.config.maxDepth.Values.Max());
                Dictionary<string, int> traveledVehicles_ = GetDicGlobal(Main.config.distanceTraveledVehicle);
                sb.AppendLine();
                int minTemp_ = Main.config.minTemp.Values.Min();
                int maxTemp_ = Main.config.maxTemp.Values.Max();
                int minVehicleTemp_ = Main.config.minVehicleTemp.Values.Min();
                int maxVehicleTemp_ = Main.config.maxVehicleTemp.Values.Max();

                if (Main.config.fahrenhiet)
                {
                    maxTemp_ = Mathf.RoundToInt(Util.CelciusToFahrenhiet(maxTemp_));
                    minTemp_ = Mathf.RoundToInt(Util.CelciusToFahrenhiet(minTemp_));
                    maxVehicleTemp_ = Mathf.RoundToInt(Util.CelciusToFahrenhiet(maxVehicleTemp_));
                    minVehicleTemp_ = Mathf.RoundToInt(Util.CelciusToFahrenhiet(minVehicleTemp_));
                }
                if (minTemp_ != int.MaxValue)
                    sb.AppendLine(Language.main.Get("ST_min_temp") + minTemp_ + tempSuffix);

                if (maxTemp_ != int.MinValue)
                    sb.AppendLine(Language.main.Get("ST_max_temp") + maxTemp_ + tempSuffix);

                if (minVehicleTemp_ != int.MaxValue)
                    sb.AppendLine(Language.main.Get("ST_min_vehicle_temp") + minVehicleTemp_ + tempSuffix);

                if (maxVehicleTemp_ != int.MinValue)
                    sb.AppendLine(Language.main.Get("ST_max_vehicle_temp") + maxVehicleTemp_ + tempSuffix);

                sb.AppendLine();
                AppendVehicleTravel(sb, traveledVehicles_);
                return;
            }
            GetModDisabledString(sb);
            GetCurrentBiomeString(sb);
            int distanceTraveled = Main.config.distanceTraveled.ContainsKey(saveSlot) ? Main.config.distanceTraveled[saveSlot] + UnsavedData.distanceTraveled : UnsavedData.distanceTraveled;
            AppendTravelLine(sb, "ST_distance_traveled", distanceTraveled);
            int distanceTraveledSwim = Main.config.distanceTraveledSwim.ContainsKey(saveSlot) ? Main.config.distanceTraveledSwim[saveSlot] + UnsavedData.distanceTraveledSwim : UnsavedData.distanceTraveledSwim;
            AppendTravelLine(sb, "ST_distance_swam", distanceTraveledSwim);
            int distanceTraveledWalk = Main.config.distanceTraveledWalk.ContainsKey(saveSlot) ? Main.config.distanceTraveledWalk[saveSlot] + UnsavedData.distanceTraveledWalk : UnsavedData.distanceTraveledWalk;
            AppendTravelLine(sb, "ST_distance_walked", distanceTraveledWalk);
            int distanceTraveledSeaglide = Main.config.distanceTraveledSeaglide.ContainsKey(saveSlot) ? Main.config.distanceTraveledSeaglide[saveSlot] + UnsavedData.distanceTraveledSeaglide : UnsavedData.distanceTraveledSeaglide;
            AppendTravelLine(sb, "ST_distance_seaglide", distanceTraveledSeaglide);
            int maxDepth = Main.config.maxDepth.ContainsKey(saveSlot) && Main.config.maxDepth[saveSlot] > UnsavedData.maxDepth ? Main.config.maxDepth[saveSlot] : UnsavedData.maxDepth;
            AppendTravelLine(sb, "ST_max_depth", maxDepth);

            sb.AppendLine();
            int minTemp = GetMinTemp();
            int maxTemp = GetMaxTemp();
            int minVehicleTemp = GetMinVehicleTemp();
            int maxVehicleTemp = GetMaxVehicleTemp();

            if (minTemp != int.MaxValue)
                sb.AppendLine(Language.main.Get("ST_min_temp") + minTemp + tempSuffix);

            if (maxTemp != int.MinValue)
                sb.AppendLine(Language.main.Get("ST_max_temp") + maxTemp + tempSuffix);

            if (minVehicleTemp != int.MaxValue)
                sb.AppendLine(Language.main.Get("ST_min_vehicle_temp") + minVehicleTemp + tempSuffix);

            if (maxVehicleTemp != int.MinValue)
                sb.AppendLine(Language.main.Get("ST_max_vehicle_temp") + maxVehicleTemp + tempSuffix);

            sb.AppendLine();
            Dictionary<string, int> traveledVehicles = MergeDics(Main.config.distanceTraveledVehicle, UnsavedData.distanceTraveledVehicle);
            AppendVehicleTravel(sb, traveledVehicles);
        }

        private static void GetTimeStats(StringBuilder sb, bool global)
        {
            if (global)
            {
                AppendTimeSpan(sb, GetSumOfDicValues(Main.config.timePlayed), "ST_time_since_landing");
                AppendTimeSpan(sb, GetSumOfDicValues(Main.config.timeWalked), "ST_time_on_feet");
                AppendTimeSpan(sb, GetSumOfDicValues(Main.config.timeSwam), "ST_time_swimming");
                AppendTimeSpan(sb, GetSumOfDicValues(Main.config.timeSat), "ST_time_sat");
                AppendTimeSpan(sb, GetSumOfDicValues(Main.config.timeSlept), "ST_time_sleeping");
                AppendTimeSpan(sb, GetSumOfDicValues(Main.config.timeEscapePod), "ST_time_escape_pod");
                AppendTimeSpan(sb, GetSumOfDicValues(Main.config.timeBase), "ST_time_base");
                AppendTimeSpan(sb, GetSumOfDicValues(Main.config.timePrecursor), "ST_time_precursor");
                TimeSpan vehiclesTotalTime = TimeSpan.Zero;
                foreach (var d in Main.config.timeVehicles)
                    vehiclesTotalTime += GetSumOfDicValues(d.Value);

                string title = AppendTimeSpan(vehiclesTotalTime, Language.main.Get("ST_time_vehicles"));
                sb.AppendLine();
                AppendTimeDic(sb, GetDicGlobal(Main.config.timeBiomes), "ST_time_biomes");
                AppendTimeDic(sb, GetDicGlobal(Main.config.timeVehicles), title);
                return;
            }
            GetModDisabledString(sb);
            AppendTimeSpan(sb, GetTimeSpanPlayed(), "ST_time_since_landing");
            TimeSpan timeOnFeet = Main.config.timeWalked.ContainsKey(saveSlot) ? Main.config.timeWalked[saveSlot] + UnsavedData.timeWalked : UnsavedData.timeWalked;
            AppendTimeSpan(sb, timeOnFeet, "ST_time_on_feet");
            TimeSpan timeSwam = Main.config.timeSwam.ContainsKey(saveSlot) ? Main.config.timeSwam[saveSlot] + UnsavedData.timeSwam : UnsavedData.timeSwam;
            AppendTimeSpan(sb, timeSwam, "ST_time_swimming");
            TimeSpan timeSat = Main.config.timeSat.ContainsKey(saveSlot) ? Main.config.timeSat[saveSlot] + UnsavedData.timeSat : UnsavedData.timeSat;
            AppendTimeSpan(sb, timeSat, "ST_time_sat");
            TimeSpan timeSlept = Main.config.timeSlept.ContainsKey(saveSlot) ? Main.config.timeSlept[saveSlot] + UnsavedData.timeSlept : UnsavedData.timeSlept;
            AppendTimeSpan(sb, timeSlept, "ST_time_sleeping");
            TimeSpan timeEscapePod = Main.config.timeEscapePod.ContainsKey(saveSlot) ? Main.config.timeEscapePod[saveSlot] + UnsavedData.timeEscapePod : UnsavedData.timeEscapePod;
            AppendTimeSpan(sb, timeEscapePod, "ST_time_escape_pod");

            TimeSpan timeBase = Main.config.timeBase.ContainsKey(saveSlot) ? Main.config.timeBase[saveSlot] + UnsavedData.timeBase : UnsavedData.timeBase;
            AppendTimeSpan(sb, timeBase, "ST_time_base");
            TimeSpan timePrecursor = Main.config.timePrecursor.ContainsKey(saveSlot) ? Main.config.timePrecursor[saveSlot] + UnsavedData.timePrecursor : UnsavedData.timePrecursor;
            AppendTimeSpan(sb, timePrecursor, "ST_time_precursor");
            sb.AppendLine();
            AppendTimeDic(sb, MergeDics(Main.config.timeBiomes, UnsavedData.timeBiomes), "ST_time_biomes");

            Dictionary<string, TimeSpan> vehiclesTime = MergeDics(Main.config.timeVehicles, UnsavedData.timeVehicles);
            if (vehiclesTime.Count > 0)
            {
                sb.AppendLine();
                TimeSpan vehiclesTotalTime = TimeSpan.Zero;
                foreach (var d in vehiclesTime)
                    vehiclesTotalTime += d.Value;

                string title = AppendTimeSpan(vehiclesTotalTime, Language.main.Get("ST_time_vehicles"));
                AppendTimeDic(sb, vehiclesTime, title);
            }
        }

        private static void GetHealthStats(StringBuilder sb, bool global)
        {
            if (global)
            {
                int healthLost = Main.config.healthLost.Values.Sum();
                int medkitsUsed = Main.config.medkitsUsed.Values.Sum();
                int deaths = Main.config.playerDeaths.Values.Sum() + Main.config.permaDeaths;

                if (deaths > 0)
                    sb.AppendLine(Language.main.Get("ST_deaths") + deaths);

                if (healthLost > 0)
                    sb.AppendLine(Language.main.Get("ST_health_lost") + healthLost);

                if (medkitsUsed > 0)
                    sb.AppendLine(Language.main.Get("ST_med_kits_used") + medkitsUsed);

                return;
            }
            GetModDisabledString(sb);
            int deaths_ = GetInt(Main.config.playerDeaths, UnsavedData.playerDeaths);
            if (deaths_ > 0 && !GameModeUtils.IsPermadeath() && GameModeUtils.RequiresOxygen())
                sb.AppendLine(Language.main.Get("ST_deaths") + deaths_);

            int healthLost_ = GetInt(Main.config.healthLost, UnsavedData.healthLost);
            int medkitsUsed_ = GetInt(Main.config.medkitsUsed, UnsavedData.medkitsUsed);
            if (healthLost_ > 0)
                sb.AppendLine(Language.main.Get("ST_health_lost") + healthLost_);

            if (medkitsUsed_ > 0)
                sb.AppendLine(Language.main.Get("ST_med_kits_used") + medkitsUsed_);
        }

        private static void GetFoodStats(StringBuilder sb, bool global)
        {
            if (global)
            {
                float waterTotal_ = Main.config.waterDrunk.Values.Sum();
                AppendWater(sb, waterTotal_);
                sb.AppendLine();
                float foodTotal_ = GetFloatGlobal(Main.config.foodEaten);
                AppendFood(sb, foodTotal_, GetDicGlobal(Main.config.foodEaten));
                return;
            }
            GetModDisabledString(sb);
            float waterTotal = GetFloat(Main.config.waterDrunk, UnsavedData.waterDrunk);
            AppendWater(sb, waterTotal);
            sb.AppendLine();
            Dictionary<string, float> dic = GetDic(Main.config.foodEaten, UnsavedData.foodEaten);
            float foodTotal = dic.Values.Sum();
            AppendFood(sb, foodTotal, dic);
        }

        private static void GetBaseStats(StringBuilder sb, bool global)
        {
            if (global)
            {
                Dictionary<string, int> roomsBuilt_ = GetDicGlobal(Main.config.baseRoomsBuilt);
                int corridorsBuilt_ = Main.config.baseCorridorsBuilt.Values.Sum();
                if (corridorsBuilt_ > 0)
                    sb.AppendLine(Language.main.Get("ST_base_corridors_built") + corridorsBuilt_);

                sb.AppendLine();
                AppendDic(sb, roomsBuilt_, "ST_base_rooms_built");
                AppendDic(sb, GetDicGlobal(Main.config.basePower), "ST_base_power_generated");
                return;
            }
            GetModDisabledString(sb);
            int corridorsBuilt = UnsavedData.GetCorridorsBuilt();
            //AddDebug("corridorsBuilt " + corridorsBuilt);
            if (corridorsBuilt > 0)
                sb.AppendLine(Language.main.Get("ST_base_corridors_built") + corridorsBuilt);

            //sb.AppendLine();
            //AddDebug("Rooms " + UnsavedData.GetRoomsDic().Count);
            AppendDic(sb, UnsavedData.GetRoomsDic(), "ST_base_rooms_built");
            AppendDic(sb, GetBasePowerDic(), "ST_base_power_generated");
        }

        private static void GetMiscStats(StringBuilder sb, bool global)
        {
            if (global)
            {
                if (Main.config.gamesWon > 0)
                    sb.AppendLine(Language.main.Get("ST_games_won") + " " + Main.config.gamesWon);

                int healthLost = Main.config.healthLost.Values.Sum();
                int medkitsUsed = Main.config.medkitsUsed.Values.Sum();
                int deaths = Main.config.playerDeaths.Values.Sum() + Main.config.permaDeaths;

                if (deaths > 0)
                    sb.AppendLine(Language.main.Get("ST_deaths") + deaths);

                if (healthLost > 0)
                    sb.AppendLine(Language.main.Get("ST_health_lost") + healthLost);

                if (medkitsUsed > 0)
                    sb.AppendLine(Language.main.Get("ST_med_kits_used") + medkitsUsed);

                sb.AppendLine();
                AppendDic(sb, GetDicGlobal(Main.config.vehiclesLost), "ST_vehicles_lost");
                //int numObjectsScanned_ = Main.config.objectsScanned.Values.Sum();
                //if (numObjectsScanned_ > 0)
                //{
                //    sb.AppendLine(Language.main.Get("ST_objects_scanned") + numObjectsScanned_);
                //    sb.AppendLine();
                //}
                AppendSet(sb, GetSetGlobal(Main.config.blueprintsUnlocked), "ST_scanned_blueprints");
                AppendSet(sb, GetSetGlobal(Main.config.blueprintsFromDatabox), "ST_databox_blueprints");
                return;
            }
            GetModDisabledString(sb);
            if (GameModeUtils.currentGameMode != GameModeOption.Creative)
            {
                int deaths_ = GetInt(Main.config.playerDeaths, UnsavedData.playerDeaths);
                if (deaths_ > 0 && !GameModeUtils.IsPermadeath() && GameModeUtils.RequiresOxygen())
                    sb.AppendLine(Language.main.Get("ST_deaths") + deaths_);

                int healthLost_ = GetInt(Main.config.healthLost, UnsavedData.healthLost);
                int medkitsUsed_ = GetInt(Main.config.medkitsUsed, UnsavedData.medkitsUsed);
                if (healthLost_ > 0)
                    sb.AppendLine(Language.main.Get("ST_health_lost") + healthLost_);

                if (medkitsUsed_ > 0)
                    sb.AppendLine(Language.main.Get("ST_med_kits_used") + medkitsUsed_);

                if (deaths_ > 0 || healthLost_ > 0 || medkitsUsed_ > 0)
                    sb.AppendLine();
            }
            AppendDic(sb, MergeDics(Main.config.vehiclesLost, UnsavedData.vehiclesLost), "ST_vehicles_lost");
            //int numObjectsScanned = GetInt(Main.config.objectsScanned, UnsavedData.objectsScanned);
            //if (numObjectsScanned > 0)
            //{
            //    sb.AppendLine(Language.main.Get("ST_objects_scanned") + numObjectsScanned);
            //    sb.AppendLine();
            //}
            AppendSet(sb, MergeSets(Main.config.blueprintsUnlocked, UnsavedData.blueprintsUnlocked), "ST_scanned_blueprints");
            AppendSet(sb, MergeSets(Main.config.blueprintsFromDatabox, UnsavedData.blueprintsFromDatabox), "ST_databox_blueprints");
        }

        private static void GetDiscoverStats(StringBuilder sb, bool glpbal)
        {
            if (glpbal)
            {
                AppendSet(sb, GetSetGlobal(Main.config.faunaFound), "ST_fauna_discovered");
                AppendSet(sb, GetSetGlobal(Main.config.floraFound), "ST_flora_discovered");
                AppendSet(sb, GetSetGlobal(Main.config.coralFound), "ST_corals_discovered");
                AppendSet(sb, GetSetGlobal(Main.config.leviathanFound), "ST_leviathans_discovered");
                return;
            }
            GetModDisabledString(sb);
            AppendSet(sb, MergeSets(Main.config.faunaFound, UnsavedData.faunaFound), "ST_fauna_discovered");
            AppendSet(sb, MergeSets(Main.config.floraFound, UnsavedData.floraFound), "ST_flora_discovered");
            AppendSet(sb, MergeSets(Main.config.coralFound, UnsavedData.coralFound), "ST_corals_discovered");
            AppendSet(sb, MergeSets(Main.config.leviathanFound, UnsavedData.leviathanFound), "ST_leviathans_discovered");
        }

        private static void GetEncyMiscStats(StringBuilder sb)
        {
            GetEncyEntries(sb, "PlanetaryGeology", true);
            sb.AppendLine();
            GetEncyEntries(sb, "Advanced", true);
        }

        private static void GetEncyTechStats(StringBuilder sb)
        {
            GetEncyEntries(sb, "Tech");
            GetEncyEntries(sb, "Tech/Equipment", false, true);
            GetEncyEntries(sb, "Tech/Habitats", false, true);
            GetEncyEntries(sb, "Tech/Power", false, true);
            GetEncyEntries(sb, "Tech/Vehicles", false, true);
        }

        private static void GetEncyDataStats(StringBuilder sb)
        {
            if (GetEncyEntries(sb, "DownloadedData", true) == 0)
                return;

            sb.AppendLine();
            if (GetEncyEntries(sb, "DownloadedData/Precursor") > 0)
            {
                GetEncyEntries(sb, "DownloadedData/Precursor/Artifacts", false, true);
                GetEncyEntries(sb, "DownloadedData/Precursor/Scan", false, true);
                GetEncyEntries(sb, "DownloadedData/Precursor/Terminal", false, true);
                sb.AppendLine();
            }
            if (GetEncyEntries(sb, "DownloadedData/AuroraSurvivors") > 0)
                sb.AppendLine();

            if (GetEncyEntries(sb, "DownloadedData/Codes") > 0)
                sb.AppendLine();

            if (GetEncyEntries(sb, "DownloadedData/Degasi") > 0)
                sb.AppendLine();

            if (GetEncyEntries(sb, "DownloadedData/BeforeCrash") > 0)
                sb.AppendLine();

            GetEncyEntries(sb, "DownloadedData/PublicDocs");
        }

        private static void GetEncyLifeformStats(StringBuilder sb)
        {
            if (GetEncyEntries(sb, "Lifeforms", true) == 0)
                return;

            sb.AppendLine();
            if (GetEncyEntries(sb, "Lifeforms/Coral") > 0)
                sb.AppendLine();

            if (GetEncyEntries(sb, "Lifeforms/Flora") > 0)
            {
                GetEncyEntries(sb, "Lifeforms/Flora/Exploitable", false, true);
                GetEncyEntries(sb, "Lifeforms/Flora/Land", false, true);
                GetEncyEntries(sb, "Lifeforms/Flora/Sea", false, true);
                sb.AppendLine();
            }
            if (GetEncyEntries(sb, "Lifeforms/Fauna") > 0)
            {
                GetEncyEntries(sb, "Lifeforms/Fauna/Carnivores", false, true);
                GetEncyEntries(sb, "Lifeforms/Fauna/Deceased", false, true);
                GetEncyEntries(sb, "Lifeforms/Fauna/LargeHerbivores", false, true);
                GetEncyEntries(sb, "Lifeforms/Fauna/SmallHerbivores", false, true);
                GetEncyEntries(sb, "Lifeforms/Fauna/Leviathans", false, true);
                GetEncyEntries(sb, "Lifeforms/Fauna/Scavengers", false, true);
            }
        }

        static int GetEncyEntries(StringBuilder sb, string path, bool showIfNoneUnlocked = false, bool indent = false)
        {
            int count = 0;
            int unlockedCount = 0;
            foreach (var kv in PDAEncyclopedia.mapping)
            {
                if (missingEncyEntries.Contains(kv.Key))
                    continue;

                PDAEncyclopedia.EntryData data = kv.Value;
                if (data.path.StartsWith(path))
                {
                    count++;
                    if (PDAEncyclopedia.entries.ContainsKey(kv.Key))
                        unlockedCount++;
                }
            }
            if (unlockedCount > 0 || showIfNoneUnlocked)
            {
                string indentation = indent ? "    " : "";
                sb.AppendLine($"{indentation}{Language.main.Get("EncyPath_" + path)}: {unlockedCount} {Language.main.Get("ST_out_of")} {count}");
            }
            return unlockedCount;
        }

        static TimeSpan GetTimeSpanPlayed()
        {
            if (Main.config.modEnabled)
                return Patches.GetTimeSpanPlayed();
            else if (Main.config.timePlayed.ContainsKey(saveSlot))
                return Main.config.timePlayed[saveSlot];

            return TimeSpan.Zero;
        }

        private static int GetInt(Dictionary<string, int> configDic, int unsaved)
        {
            int total = unsaved;
            if (configDic.ContainsKey(saveSlot))
                total += configDic[saveSlot];

            return total;
        }

        private static float GetFloat(Dictionary<string, float> configDic, float unsaved)
        {
            float total = unsaved;
            if (configDic.ContainsKey(saveSlot))
                total += configDic[saveSlot];

            return total;
        }

        private static float GetFloatGlobal(Dictionary<string, Dictionary<string, float>> configDic)
        {
            float total = 0;
            foreach (var kv in configDic)
                total += kv.Value.Values.Sum();

            return total;
        }

        private static HashSet<string> GetSetGlobal(Dictionary<string, HashSet<string>> dic)
        {
            HashSet<string> newSet = new HashSet<string>();
            foreach (var kv in dic)
            {
                foreach (var s in kv.Value)
                    newSet.Add(s);
            }
            return newSet;
        }

        private static HashSet<string> MergeSets(Dictionary<string, HashSet<string>> configDic, HashSet<TechType> set)
        {
            HashSet<string> newSet = new HashSet<string>();
            if (configDic.ContainsKey(saveSlot))
            {
                foreach (var s in configDic[saveSlot])
                    newSet.Add(s);
            }
            foreach (var tt in set)
                newSet.Add(tt.AsString());

            return newSet;
        }

        private static HashSet<string> MergeSets(Dictionary<string, HashSet<string>> configDic, HashSet<string> set)
        {
            HashSet<string> newSet = new HashSet<string>();
            if (configDic.ContainsKey(saveSlot))
            {
                foreach (var s in configDic[saveSlot])
                    newSet.Add(s);
            }
            foreach (var tt in set)
                newSet.Add(tt);

            return newSet;
        }

        public static TimeSpan GetSumOfDicValues(Dictionary<string, TimeSpan> dic)
        {
            TimeSpan total = TimeSpan.Zero;
            foreach (var kv in dic)
                total += kv.Value;

            return total;
        }

        public static void AppendTimeSpan(StringBuilder sb, TimeSpan time, string s, bool indent = false)
        {
            if (time.TotalMinutes < 1)
                return;

            if (indent)
                sb.Append("     " + Language.main.Get(s));
            else
                sb.Append(Language.main.Get(s));

            string day = time.Days == 1 ? Language.main.Get("ST_day") : Language.main.Get("ST_days");
            if (time.Days > 0)
                sb.Append(time.Days + " " + day);

            if (time.Days > 0 && (time.Hours > 0 || time.Minutes > 0))
                sb.Append(" ");

            string hour = time.Hours == 1 ? Language.main.Get("ST_hour") : Language.main.Get("ST_hours");
            if (time.Hours > 0)
                sb.Append(time.Hours + " " + hour);

            if (time.Hours > 0 && time.Minutes > 0)
                sb.Append(" ");

            string minute = time.Minutes == 1 ? Language.main.Get("ST_minute") : Language.main.Get("ST_minutes");
            if (time.Minutes > 0)
                sb.AppendLine(time.Minutes + " " + minute);
            else
                sb.AppendLine();
        }

        public static string AppendTimeSpan(TimeSpan time, string s)
        {
            if (time.TotalMinutes < 1)
                return s;

            StringBuilder sb = new StringBuilder(s);

            string day = time.Days == 1 ? Language.main.Get("ST_day") : Language.main.Get("ST_days");
            if (time.Days > 0)
                sb.Append(time.Days + " " + day);

            if (time.Days > 0 && (time.Hours > 0 || time.Minutes > 0))
                sb.Append(" ");

            string hour = time.Hours == 1 ? Language.main.Get("ST_hour") : Language.main.Get("ST_hours");
            if (time.Hours > 0)
                sb.Append(time.Hours + " " + hour);

            if (time.Hours > 0 && time.Minutes > 0)
                sb.Append(" ");

            string minute = time.Minutes == 1 ? Language.main.Get("ST_minute") : Language.main.Get("ST_minutes");
            if (time.Minutes > 0)
                sb.Append(time.Minutes + " " + minute);

            return sb.ToString();
        }

        public static string GetTraveledString(int meters)
        {
            if (Main.config.miles)
            {
                int yards = Mathf.RoundToInt(Util.MeterToYard(meters));
                int miles = yards / 1760;
                int y = yards % 1760;
                StringBuilder sb = new StringBuilder();
                if (miles == 1)
                    sb.Append(miles + " " + Language.main.Get("ST_mile"));
                else if (miles > 1)
                    sb.Append(miles + " " + Language.main.Get("ST_miles"));

                if (miles > 0 && y > 0)
                    sb.Append(" ");

                if (y == 1)
                    sb.Append(y + " " + Language.main.Get("ST_yard"));
                else if (y > 1)
                    sb.Append(y + " " + Language.main.Get("ST_yards"));

                return sb.ToString();
            }
            else
            {
                int km = meters / 1000;
                int m = meters % 1000;
                StringBuilder sb = new StringBuilder();
                if (km == 1)
                    sb.Append(km + " " + Language.main.Get("ST_kilometer"));
                else if (km > 1)
                    sb.Append(km + " " + Language.main.Get("ST_kilometers"));

                if (km > 0 && m > 0)
                    sb.Append(" ");

                if (m == 1)
                    sb.Append(m + " " + Language.main.Get("ST_meter"));
                else if (m > 1)
                    sb.Append(m + " " + Language.main.Get("ST_meters"));

                //AddDebug(" GetTraveledString " + sb.ToString());
                return sb.ToString();
            }
        }

        private static void AppendVehicleTravel(StringBuilder sb, Dictionary<string, int> traveledVehicles)
        {
            if (traveledVehicles.Count == 0)
                return;

            AppendTravelLine(sb, "ST_distance_vehicles", traveledVehicles.Values.Sum());
            SortedDictionary<string, int> sortedDic = GetTranslatedSortedDic(traveledVehicles);
            foreach (var kv in sortedDic)
                AppendTravelLine(sb, kv.Key + ": ", kv.Value, true);
        }

        private static Dictionary<string, int> MergeDics(Dictionary<string, Dictionary<string, int>> configDic, Dictionary<TechType, int> unsavedDic)
        {
            Dictionary<string, int> newDic;
            if (configDic.ContainsKey(saveSlot))
                newDic = new Dictionary<string, int>(configDic[saveSlot]);
            else
                newDic = new Dictionary<string, int>();

            foreach (var kv in unsavedDic)
                newDic.AddValue(kv.Key.ToString(), kv.Value);

            return newDic;
        }

        private static Dictionary<string, TimeSpan> MergeDics(Dictionary<string, Dictionary<string, TimeSpan>> configDic, Dictionary<TechType, TimeSpan> unsavedDic)
        {
            Dictionary<string, TimeSpan> newDic;
            if (configDic.ContainsKey(saveSlot))
                newDic = new Dictionary<string, TimeSpan>(configDic[saveSlot]);
            else
                newDic = new Dictionary<string, TimeSpan>();

            foreach (var kv in unsavedDic)
                newDic.AddValue(kv.Key.ToString(), kv.Value);

            return newDic;
        }

        private static Dictionary<string, TimeSpan> GetDic(Dictionary<string, Dictionary<string, TimeSpan>> configDic, Dictionary<TechType, TimeSpan> unsavedDic)
        {
            Dictionary<string, TimeSpan> newDic;
            if (configDic.ContainsKey(saveSlot))
                newDic = new Dictionary<string, TimeSpan>(configDic[saveSlot]);
            else
                newDic = new Dictionary<string, TimeSpan>();

            foreach (var kv in unsavedDic)
                newDic.AddValue(kv.Key.ToString(), kv.Value);

            return newDic;
        }

        private static Dictionary<string, int> GetDicGlobal(Dictionary<string, Dictionary<string, int>> configDic)
        {
            Dictionary<string, int> newDic = new Dictionary<string, int>();
            foreach (var kv1 in configDic)
            {
                foreach (var kv in kv1.Value)
                    newDic.AddValue(kv.Key, kv.Value);
            }
            return newDic;
        }

        private static Dictionary<string, float> GetDicGlobal(Dictionary<string, Dictionary<string, float>> configDic)
        {
            Dictionary<string, float> newDic = new Dictionary<string, float>();
            foreach (var kv1 in configDic)
            {
                foreach (var kv in kv1.Value)
                    newDic.AddValue(kv.Key, kv.Value);
            }
            return newDic;
        }

        private static Dictionary<string, float> GetDic(Dictionary<string, Dictionary<string, float>> configDic, Dictionary<TechType, float> unsavedDic)
        {
            Dictionary<string, float> newDic;
            if (configDic.ContainsKey(saveSlot))
                newDic = new Dictionary<string, float>(configDic[saveSlot]);
            else
                newDic = new Dictionary<string, float>();

            foreach (var kv in unsavedDic)
                newDic.AddValue(kv.Key.ToString(), kv.Value);

            return newDic;
        }

        private static Dictionary<string, TimeSpan> MergeDics(Dictionary<string, Dictionary<string, TimeSpan>> configDic, Dictionary<string, TimeSpan> unsavedDic)
        {
            Dictionary<string, TimeSpan> newDic;
            if (configDic.ContainsKey(saveSlot))
                newDic = new Dictionary<string, TimeSpan>(configDic[saveSlot]);
            else
                newDic = new Dictionary<string, TimeSpan>();

            foreach (var kv in unsavedDic)
                newDic.AddValue(kv.Key.ToString(), kv.Value);

            return newDic;
        }

        private static Dictionary<string, TimeSpan> GetDicGlobal(Dictionary<string, Dictionary<string, TimeSpan>> configDic)
        {
            Dictionary<string, TimeSpan> newDic = new Dictionary<string, TimeSpan>();
            foreach (var kv1 in configDic)
            {
                foreach (var kv in kv1.Value)
                    newDic.AddValue(kv.Key, kv.Value);
            }
            return newDic;
        }

        private static void AppendTravelLine(StringBuilder sb, string s, int dist, bool indent = false)
        {
            if (dist <= 0)
                return;

            if (indent)
                sb.AppendLine("     " + Language.main.Get(s) + GetTraveledString(dist));
            else
                sb.AppendLine(Language.main.Get(s) + GetTraveledString(dist));
        }

        private static string GetTempSuffix()
        {
            if (Main.config.fahrenhiet)
                return "°F";
            else
                return "°C";
        }

        private static int GetMinTemp()
        {
            int temp = UnsavedData.minTemp;
            if (Main.config.minTemp.ContainsKey(saveSlot) && Main.config.minTemp[saveSlot] < temp)
                temp = Main.config.minTemp[saveSlot];

            if (Main.config.fahrenhiet)
                temp = Mathf.RoundToInt(Util.CelciusToFahrenhiet(temp));

            return temp;
        }

        private static int GetMaxTemp()
        {
            int temp = UnsavedData.maxTemp;
            if (Main.config.maxTemp.ContainsKey(saveSlot) && Main.config.maxTemp[saveSlot] > temp)
                temp = Main.config.maxTemp[saveSlot];

            if (Main.config.fahrenhiet)
                temp = Mathf.RoundToInt(Util.CelciusToFahrenhiet(temp));

            return temp;
        }

        private static int GetMinVehicleTemp()
        {
            int temp = UnsavedData.minVehicleTemp;
            if (Main.config.minVehicleTemp.ContainsKey(saveSlot) && Main.config.minVehicleTemp[saveSlot] < temp)
                temp = Main.config.minVehicleTemp[saveSlot];

            if (temp != int.MaxValue && Main.config.fahrenhiet)
                temp = Mathf.RoundToInt(Util.CelciusToFahrenhiet(temp));

            return temp;
        }

        private static int GetMaxVehicleTemp()
        {
            int temp = UnsavedData.maxVehicleTemp;
            if (Main.config.maxVehicleTemp.ContainsKey(saveSlot) && Main.config.maxVehicleTemp[saveSlot] > temp)
                temp = Main.config.maxVehicleTemp[saveSlot];

            if (temp != int.MinValue && Main.config.fahrenhiet)
                temp = Mathf.RoundToInt(Util.CelciusToFahrenhiet(temp));

            return temp;
        }

        private static void AppendWater(StringBuilder sb, float water)
        {
            if (water == 0)
                return;

            if (Main.config.pounds)
                sb.AppendLine(Language.main.Get("ST_water_drunk") + Util.literToGallon(water).ToString("0.0") + " " + Language.main.Get("ST_gallons"));
            else
                sb.AppendLine(Language.main.Get("ST_water_drunk") + water.ToString("0.0") + " " + Language.main.Get("ST_liters"));
        }

        private static void AppendFood(StringBuilder sb, float foodTotal, Dictionary<string, float> dic)
        {
            if (foodTotal == 0)
                return;

            string kgLoc = Main.config.pounds ? " " + Language.main.Get("ST_pounds") : " " + Language.main.Get("ST_kilograms");
            if (Main.config.pounds)
                foodTotal = Util.KiloToPound(foodTotal);

            sb.AppendLine(Language.main.Get("ST_food_eaten") + foodTotal.ToString("0.0") + kgLoc);
            SortedDictionary<string, float> sortedDic = GetTranslatedSortedDic(dic);
            foreach (var kv in sortedDic)
            {
                float kg = kv.Value;
                if (Main.config.pounds)
                    kg = Util.KiloToPound(kg);

                sb.AppendLine("     " + kv.Key + ": " + kg.ToString("0.0") + kgLoc);
            }
        }

        private static Dictionary<TechType, int> GetBasePowerDic()
        {
            Dictionary<TechType, int> basePower = new Dictionary<TechType, int>();
            foreach (PowerSource ps in UnsavedData.basePowerSources)
            {
                if (ps == null)
                    continue;

                int power = Mathf.RoundToInt(ps.power);
                if (power < 1)
                    continue;
                //AddDebug("GetBasePowerDic " + ps.name + " " + (int)ps.power);
                TechType tt = CraftData.GetTechType(ps.gameObject);
                if (Patches.basePowerSourceTypes.Contains(tt))
                    basePower.AddValue(tt, power);
            }
            return basePower;
        }

        private static SortedDictionary<string, int> GetTranslatedSortedDic(Dictionary<string, int> dic)
        {
            SortedDictionary<string, int> sortedDic = new SortedDictionary<string, int>();
            foreach (var kv in dic)
            {
                if (kv.Value > 0)
                    sortedDic.Add(Language.main.Get(kv.Key), kv.Value);
            }
            return sortedDic;
        }

        private static SortedDictionary<string, int> GetTranslatedSortedDic(Dictionary<TechType, int> dic)
        {
            SortedDictionary<string, int> sortedDic = new SortedDictionary<string, int>();
            foreach (var kv in dic)
            {
                if (kv.Value > 0)
                    sortedDic.Add(Language.main.Get(kv.Key), kv.Value);
            }
            return sortedDic;
        }

        private static SortedDictionary<string, float> GetTranslatedSortedDic(Dictionary<string, float> dic)
        {
            SortedDictionary<string, float> sortedDic = new SortedDictionary<string, float>();
            foreach (var kv in dic)
            {
                if (kv.Value > 0)
                    sortedDic.Add(Language.main.Get(kv.Key), kv.Value);
            }
            return sortedDic;
        }

        private static SortedDictionary<string, TimeSpan> GetTranslatedSortedDic(Dictionary<string, TimeSpan> dic)
        {
            SortedDictionary<string, TimeSpan> sortedDic = new SortedDictionary<string, TimeSpan>();
            foreach (var kv in dic)
            {
                if (kv.Value.TotalMinutes >= 1)
                    sortedDic.Add(Language.main.Get(kv.Key), kv.Value);
            }
            return sortedDic;
        }

        private static void AppendDic(StringBuilder sb, Dictionary<string, int> dic, string title)
        {
            int ValuesSum = dic.Values.Sum();
            if (dic.Count == 0 || ValuesSum == 0)
                return;

            sb.AppendLine(Language.main.Get(title) + ValuesSum);
            foreach (var kv in GetTranslatedSortedDic(dic))
                sb.AppendLine("     " + kv.Key + ": " + kv.Value);

            sb.AppendLine();
        }

        private static void AppendDic(StringBuilder sb, Dictionary<TechType, int> dic, string title)
        {
            int ValuesSum = dic.Values.Sum();
            if (dic.Count == 0 || ValuesSum == 0)
                return;

            sb.AppendLine(Language.main.Get(title) + ValuesSum);
            SortedDictionary<string, int> sortedDic = GetTranslatedSortedDic(dic);
            foreach (var kv in sortedDic)
                sb.AppendLine("     " + kv.Key + ": " + kv.Value);

            sb.AppendLine();
        }

        private static void AppendTimeDic(StringBuilder sb, Dictionary<string, TimeSpan> dic, string title)
        {
            if (dic.Count == 0)
                return;

            SortedDictionary<string, TimeSpan> sortedDic = GetTranslatedSortedDic(dic);
            sb.AppendLine(Language.main.Get(title));
            foreach (var kv in sortedDic)
                AppendTimeSpan(sb, kv.Value, kv.Key + ": ", true);

            sb.AppendLine();
        }

        private static void AppendSet(StringBuilder sb, HashSet<string> set, string title)
        {
            if (set.Count == 0)
                return;

            sb.AppendLine(Language.main.Get(title));
            SortedSet<string> sortedSet = GetTranslatedSortedSet(set);
            foreach (var s in sortedSet)
                sb.AppendLine("     " + s);

            sb.AppendLine();
        }

        private static SortedSet<string> GetTranslatedSortedSet(HashSet<string> set)
        {
            SortedSet<string> sortedSet = new SortedSet<string>();
            foreach (var s in set)
                sortedSet.Add(Language.main.Get(s));

            return sortedSet;
        }

        private static StringBuilder GetModDisabledString(StringBuilder sb)
        {
            if (!Main.config.modEnabled)
                sb.Append(Language.main.Get("ST_mod_disabled") + "\n" + "\n");

            return sb;
        }

        private static StringBuilder GetCurrentBiomeString(StringBuilder sb)
        {
            string biomeName = Language.main.Get(Util.GetBiomeName());
            sb.Append(Language.main.Get("ST_current_biome") + biomeName + "\n");
            sb.AppendLine();
            return sb;
        }

        [HarmonyPatch(typeof(uGUI_EncyclopediaTab))]
        internal class uGUI_EncyclopediaTab_Patch
        {
            static CraftNode lastNode;

            [HarmonyPostfix]
            [HarmonyPatch("Open")]
            public static void OpenPostfix(uGUI_EncyclopediaTab __instance)
            {
                //AddDebug("uGUI_EncyclopediaTab Open");
                if (lastNode != null && myEntries.ContainsKey(lastNode.id))
                { // update stats 
                  //AddDebug("update tab");
                    __instance.activeEntry = null;
                    __instance.Activate(lastNode);
                }
            }
            [HarmonyPostfix]
            [HarmonyPatch("Activate")]
            public static void ActivatePostfix(uGUI_EncyclopediaTab __instance, CraftNode node)
            {
                lastNode = node;
                //AddDebug("Activate " + __instance.activeEntry.key);
            }
        }


    }

}