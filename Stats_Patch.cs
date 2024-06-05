using HarmonyLib;
using Nautilus.Handlers;
//using static Stats_Tracker.Patches;
using Nautilus.Utility;
using ProtoBuf.Compiler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static ErrorMessage;


namespace Stats_Tracker
{
    internal class Stats_Patch
    {
        public static string saveSlot;
        public static Dictionary<string, string> myStrings = new Dictionary<string, string>();
        public static Dictionary<string, string> descs = new Dictionary<string, string>();
        static string dayLoc = " " + Language.main.Get("ST_day");
        static string daysLoc = " " + Language.main.Get("ST_days");
        static string hourLoc = " " + Language.main.Get("ST_hour");
        static string hoursLoc = " " + Language.main.Get("ST_hours");
        static string minuteLoc = " " + Language.main.Get("ST_minute");
        static string minutesLoc = " " + Language.main.Get("ST_minutes");
        static string meterLoc = " " + Language.main.Get("ST_meter");
        static string metersLoc = " " + Language.main.Get("ST_meters");
        static string kmLoc = " " + Language.main.Get("ST_kilometer");
        static string kmsLoc = " " + Language.main.Get("ST_kilometers");
        static string kgLoc = " " + Language.main.Get("ST_kilograms");
        static string litersLoc = " " + Language.main.Get("ST_liters");

        private static int GetInt(Dictionary<string, int> configDic, int unsaved)
        {
            int total = 0;
            if (configDic.ContainsKey(saveSlot))
                total += configDic[saveSlot];

            total += unsaved;
            return total;
        }

        private static float GetFloat(Dictionary<string, float> configDic, float unsaved)
        {
            float total = 0;
            if (configDic.ContainsKey(saveSlot))
                total += configDic[saveSlot];

            total += unsaved;
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

        public static TimeSpan GetSumOfDicValues(Dictionary<string, TimeSpan> dic)
        {
            TimeSpan total = TimeSpan.Zero;
            foreach (var kv in dic)
                total += kv.Value;

            return total;
        }

        public static void AppendTimeSpan(StringBuilder sb, TimeSpan time, string s, bool indent = false)
        {
            if (time.Days == 0 && time.Hours == 0 && time.Minutes == 0)
                return;

            if (indent)
                sb.Append("     " + Language.main.Get(s));
            else
                sb.Append(Language.main.Get(s));

            string day = time.Days == 1 ? dayLoc : daysLoc;
            if (time.Days > 0)
                sb.Append(time.Days + day);

            if (time.Days > 0 && (time.Hours > 0 || time.Minutes > 0))
                sb.Append(", ");

            string hour = time.Hours == 1 ? hourLoc : hoursLoc;
            if (time.Hours > 0)
                sb.Append(time.Hours + hour);

            if (time.Hours > 0 && time.Minutes > 0)
                sb.Append(", ");

            string minute = time.Minutes == 1 ? minuteLoc : minutesLoc;
            if (time.Minutes > 0)
                sb.AppendLine(time.Minutes + minute);
            else
                sb.AppendLine();
        }

        private static float GetFoodKilosEaten()
        {
            float total = 0;
            if (Main.config.foodEaten.ContainsKey(saveSlot))
                total += Main.config.foodEaten[saveSlot].Values.Sum();

            total += UnsavedData.foodEaten.Values.Sum();
            return total;
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
            PDAHandler.AddEncyclopediaEntry(entry);
            //mapping[key] = entry;
            myStrings[key] = desc;
            descs["EncyDesc_" + key] = desc;
            LanguageHandler.SetLanguageLine("Ency_" + key, name);
            LanguageHandler.SetLanguageLine("EncyDesc_" + key, desc);
            //PDAEncyclopedia.Add(entryData.encyclopedia, false);
        }

        public static string GetTraveledString(int meters)
        {
            int km = meters / 1000;
            int m = meters % 1000;
            StringBuilder sb = new StringBuilder();
            if (km == 1)
                sb.Append(km + kmLoc);
            else if (km > 1)
                sb.Append(km + kmsLoc);

            if (km > 0 && m > 0)
                sb.Append(", ");

            if (m == 1)
                sb.Append(m + meterLoc);
            else if (m > 1)
                sb.Append(m + metersLoc);
            //sb.Append(m + metersLoc);
            //AddDebug(" GetTraveledString " + sb.ToString());
            return sb.ToString();
        }

        public static void AddEntries()
        {
            AddPDAentry("ST_StatsGlobal", Language.main.Get("ST_global_statistics"), "", "ST_stats");
            AddPDAentry("ST_StatsThisGame", Language.main.Get("ST_current_game_statistics"), "", "ST_stats");
            //LanguageHandler.SetLanguageLine("EncyPath_ST_Stats", "ST_statistics");
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

        private static void GetTimeStats(StringBuilder sb)
        {
            AppendTimeSpan(sb, Patches.GetTimePlayed(), "ST_time_since_landing");
            TimeSpan timeOnFeet = Main.config.timeWalked.ContainsKey(saveSlot) ? Main.config.timeWalked[saveSlot] + UnsavedData.timeWalked : UnsavedData.timeWalked;
            //AddDebug("timeOnFeet " + timeOnFeet);
            AppendTimeSpan(sb, timeOnFeet, "ST_time_on_feet");
            TimeSpan timeSwam = Main.config.timeSwam.ContainsKey(saveSlot) ? Main.config.timeSwam[saveSlot] + UnsavedData.timeSwam : UnsavedData.timeSwam;
            AppendTimeSpan(sb, timeSwam, "ST_time_swimming");
            TimeSpan timeSlept = Main.config.timeSlept.ContainsKey(saveSlot) ? Main.config.timeSlept[saveSlot] + UnsavedData.timeSlept : UnsavedData.timeSlept;
            AppendTimeSpan(sb, timeSlept, "ST_time_sleeping");
            TimeSpan timeEscapePod = Main.config.timeEscapePod.ContainsKey(saveSlot) ? Main.config.timeEscapePod[saveSlot] + UnsavedData.timeEscapePod : UnsavedData.timeEscapePod;
            AppendTimeSpan(sb, timeEscapePod, "ST_time_escape_pod");
            TimeSpan timeBase = Main.config.timeBase.ContainsKey(saveSlot) ? Main.config.timeBase[saveSlot] + UnsavedData.timeBase : UnsavedData.timeBase;
            AppendTimeSpan(sb, timeBase, "ST_time_base");
            int sbLength = sb.Length;
            AppendTimeDic(sb, GetDic(Main.config.timeVehicles, UnsavedData.timeVehicles), "ST_time_vehicles");
            if (sbLength == sb.Length)
                sb.AppendLine();
        }

        private static void GetTimeGlobalStats(StringBuilder sb)
        {
            //GetTimeSinceLandingGlobal(sb);
            AppendTimeSpan(sb, GetSumOfDicValues(Main.config.timePlayed), "ST_time_since_landing");
            AppendTimeSpan(sb, GetSumOfDicValues(Main.config.timeWalked), "ST_time_on_feet");
            AppendTimeSpan(sb, GetSumOfDicValues(Main.config.timeSwam), "ST_time_swimming");
            AppendTimeSpan(sb, GetSumOfDicValues(Main.config.timeSlept), "ST_time_sleeping");
            AppendTimeSpan(sb, GetSumOfDicValues(Main.config.timeEscapePod), "ST_time_escape_pod");
            AppendTimeSpan(sb, GetSumOfDicValues(Main.config.timeBase), "ST_time_base");
            int sbLength = sb.Length;
            AppendTimeDic(sb, GetDicGlobal(Main.config.timeVehicles), "ST_time_vehicles");
            if (sbLength == sb.Length)
                sb.AppendLine();
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

        private static void GetTravelStats(StringBuilder sb)
        {
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

            Dictionary<string, int> traveledVehicles = MergeDics(Main.config.distanceTraveledVehicle, UnsavedData.distanceTraveledVehicle);
            if (traveledVehicles.Count > 0)
                sb.AppendLine(Language.main.Get("ST_distance_vehicles") + traveledVehicles.Values.Sum());

            foreach (var kv in traveledVehicles)
                AppendTravelLine(sb, Language.main.Get(kv.Key) + ": ", kv.Value, true);

            sb.AppendLine();
        }

        private static void GetTravelGlobalStats(StringBuilder sb)
        {
            AppendTravelLine(sb, "ST_distance_traveled", Main.config.distanceTraveled.Values.Sum());
            AppendTravelLine(sb, "ST_distance_swam", Main.config.distanceTraveledSwim.Values.Sum());
            AppendTravelLine(sb, "ST_distance_walked", Main.config.distanceTraveledWalk.Values.Sum());
            AppendTravelLine(sb, "ST_distance_seaglide", Main.config.distanceTraveledSeaglide.Values.Sum());
            AppendTravelLine(sb, "ST_max_depth", Main.config.maxDepth.Values.Max());

            Dictionary<string, int> traveledVehicles = GetDicGlobal(Main.config.distanceTraveledVehicle);
            if (traveledVehicles.Count > 0)
                sb.AppendLine(Language.main.Get("ST_distance_vehicles") + traveledVehicles.Values.Sum());

            foreach (var kv in traveledVehicles)
                AppendTravelLine(sb, Language.main.Get(kv.Key) + ": ", kv.Value, true);

            sb.AppendLine();
        }

        private static void GetKilledStats(StringBuilder sb)
        {
            AppendDic(sb, MergeDics(Main.config.plantsKilled, UnsavedData.plantsKilled), "ST_plants_killed");
            AppendDic(sb, MergeDics(Main.config.animalsKilled, UnsavedData.animalsKilled), "ST_animals_killed");
            AppendDic(sb, MergeDics(Main.config.coralKilled, UnsavedData.coralKilled), "ST_corals_killed");
            AppendDic(sb, MergeDics(Main.config.leviathansKilled, UnsavedData.leviathansKilled), "ST_leviathans_killed");
        }

        private static void GetKilledGlobalStats(StringBuilder sb)
        {
            AppendDic(sb, GetDicGlobal(Main.config.plantsKilled), "ST_plants_killed");
            AppendDic(sb, GetDicGlobal(Main.config.animalsKilled), "ST_animals_killed");
            AppendDic(sb, GetDicGlobal(Main.config.coralKilled), "ST_corals_killed");
            AppendDic(sb, GetDicGlobal(Main.config.leviathansKilled), "ST_leviathans_killed");
        }

        private static void GetHealthStats(StringBuilder sb)
        {
            if (GameModeUtils.currentGameMode == GameModeOption.Creative)
                return;

            int deaths = GetInt(Main.config.playerDeaths, UnsavedData.playerDeaths);
            if (deaths > 0 && !GameModeUtils.IsPermadeath() && GameModeUtils.RequiresOxygen())
                sb.AppendLine(Language.main.Get("ST_deaths") + deaths);

            int healthLost = GetInt(Main.config.healthLost, UnsavedData.healthLost);
            int medkitsUsed = GetInt(Main.config.medkitsUsed, UnsavedData.medkitsUsed);
            if (healthLost > 0)
                sb.AppendLine(Language.main.Get("ST_health_lost") + healthLost);

            if (medkitsUsed > 0)
                sb.AppendLine(Language.main.Get("ST_med_kits_used") + medkitsUsed);

            int minTemp = GetMinTemp();
            int maxTemp = GetMaxTemp();
            int minVehicleTemp = GetMinVehicleTemp();
            int maxVehicleTemp = GetMaxVehicleTemp();
            if (minTemp != int.MaxValue)
                sb.AppendLine(Language.main.Get("ST_min_temp") + Language.main.GetFormat<float>("ThermalPlantCelsius", minTemp));

            if (maxTemp != int.MinValue)
                sb.AppendLine(Language.main.Get("ST_max_temp") + Language.main.GetFormat<float>("ThermalPlantCelsius", maxTemp));

            if (minVehicleTemp != int.MaxValue)
                sb.AppendLine(Language.main.Get("ST_min_vehicle_temp") + Language.main.GetFormat("ThermalPlantCelsius", minVehicleTemp));

            if (maxVehicleTemp != int.MinValue)
                sb.AppendLine(Language.main.Get("ST_max_vehicle_temp") + Language.main.GetFormat("ThermalPlantCelsius", maxVehicleTemp));
            //if (deaths > 0 || medkitsUsed > 0 || healthLost > 0)
            sb.AppendLine();
        }

        private static int GetMinTemp()
        {
            if (Main.config.minTemp.ContainsKey(saveSlot))
            {
                int tempSaved = Main.config.minTemp[saveSlot];
                if (tempSaved < UnsavedData.minTemp)
                    return tempSaved;
            }
            return UnsavedData.minTemp;
        }

        private static int GetMaxTemp()
        {
            if (Main.config.maxTemp.ContainsKey(saveSlot))
            {
                int tempSaved = Main.config.maxTemp[saveSlot];
                if (tempSaved > UnsavedData.maxTemp)
                    return tempSaved;
            }
            return UnsavedData.maxTemp;
        }

        private static int GetMinVehicleTemp()
        {
            if (Main.config.minVehicleTemp.ContainsKey(saveSlot))
            {
                int tempSaved = Main.config.minVehicleTemp[saveSlot];
                if (tempSaved < UnsavedData.minVehicleTemp)
                    return tempSaved;
            }
            return UnsavedData.minVehicleTemp;
        }

        private static int GetMaxVehicleTemp()
        {
            if (Main.config.maxVehicleTemp.ContainsKey(saveSlot))
            {
                int tempSaved = Main.config.maxVehicleTemp[saveSlot];
                if (tempSaved > UnsavedData.maxVehicleTemp)
                    return tempSaved;
            }
            return UnsavedData.maxVehicleTemp;
        }

        private static void GetHealthGlobalStats(StringBuilder sb)
        {
            int healthLost = Main.config.healthLost.Values.Sum();
            int medkitsUsed = Main.config.medkitsUsed.Values.Sum();
            int deaths = Main.config.playerDeaths.Values.Sum() + Main.config.permaDeaths;
            int minTemp = Main.config.minTemp.Values.Min();
            int maxTemp = Main.config.maxTemp.Values.Max();
            int minVehicleTemp = Main.config.minVehicleTemp.Values.Min();
            int maxVehicleTemp = Main.config.maxVehicleTemp.Values.Max();
            int sbLength = sb.Length;

            if (deaths > 0)
                sb.AppendLine(Language.main.Get("ST_deaths") + deaths);

            if (healthLost > 0)
                sb.AppendLine(Language.main.Get("ST_health_lost") + healthLost);

            if (medkitsUsed > 0)
                sb.AppendLine(Language.main.Get("ST_med_kits_used") + medkitsUsed);

            if (minTemp != int.MaxValue)
                sb.AppendLine(Language.main.Get("ST_min_temp") + Language.main.GetFormat<float>("ThermalPlantCelsius", minTemp));

            if (maxTemp != int.MinValue)
                sb.AppendLine(Language.main.Get("ST_max_temp") + Language.main.GetFormat<float>("ThermalPlantCelsius", maxTemp));

            if (minVehicleTemp != int.MaxValue)
                sb.AppendLine(Language.main.Get("ST_min_vehicle_temp") + Language.main.GetFormat("ThermalPlantCelsius", minVehicleTemp));

            if (maxVehicleTemp != int.MinValue)
                sb.AppendLine(Language.main.Get("ST_max_vehicle_temp") + Language.main.GetFormat("ThermalPlantCelsius", maxVehicleTemp));

            if (sb.Length != sbLength)
                sb.AppendLine();
        }

        private static void GetFoodStats(StringBuilder sb)
        {
            if (!GameModeUtils.RequiresSurvival())
                return;

            float waterTotal = GetFloat(Main.config.waterDrunk, UnsavedData.waterDrunk);
            decimal waterTotalD = Math.Round((decimal)waterTotal, 1);
            if (waterTotalD > 0)
                sb.AppendLine(Language.main.Get("ST_water_drunk") + waterTotalD + litersLoc);

            float foodTotal = GetFoodKilosEaten();
            decimal foodTotalD = Math.Round((decimal)foodTotal, 1);
            if (foodTotalD > 0)
                sb.AppendLine(Language.main.Get("ST_food_eaten") + foodTotalD + kgLoc);

            foreach (var kv in GetDic(Main.config.foodEaten, UnsavedData.foodEaten))
                sb.AppendLine("     " + Language.main.Get(kv.Key) + " " + kv.Value + kgLoc);

            if (waterTotalD > 0 || foodTotalD > 0)
                sb.AppendLine();
        }

        private static void GetFoodGlobalStats(StringBuilder sb)
        {
            float waterTotal = Main.config.waterDrunk.Values.Sum();
            decimal waterTotalD = Math.Round((decimal)waterTotal, 1);
            if (waterTotalD > 0)
                sb.AppendLine(Language.main.Get("ST_water_drunk") + waterTotalD + litersLoc);

            float foodTotal = GetFloatGlobal(Main.config.foodEaten);
            decimal foodTotalD = Math.Round((decimal)foodTotal, 1);

            if (foodTotalD > 0)
                sb.AppendLine(Language.main.Get("ST_food_eaten") + foodTotalD + kgLoc);

            foreach (var kv in GetDicGlobal(Main.config.foodEaten))
                sb.AppendLine("     " + Language.main.Get(kv.Key) + " " + kv.Value + kgLoc);

            if (waterTotalD > 0 || foodTotalD > 0)
                sb.AppendLine();
        }

        private static void GetBaseStats(StringBuilder sb)
        {
            Dictionary<TechType, int> roomsBuilt = UnsavedData.GetRoomsDic();
            int corridorsBuilt = UnsavedData.GetCorridorsBuilt();
            //AddDebug("sb length " + sb.Length);
            //AddDebug("roomsBuilt " + roomsBuilt);
            //AddDebug("corridorsBuilt " + corridorsBuilt);
            AppendDic(sb, GetBasePowerDic(), "ST_base_power_generated");
            if (corridorsBuilt > 0)
                sb.AppendLine(Language.main.Get("ST_base_corridors_built") + corridorsBuilt);

            AppendDic(sb, roomsBuilt, "ST_base_rooms_built");
            if (roomsBuilt.Count == 0 && corridorsBuilt > 0)
                sb.AppendLine();
        }

        private static Dictionary<TechType, int> GetBasePowerDic()
        {
            Dictionary<TechType, int> basePower = new Dictionary<TechType, int>();
            foreach (PowerSource ps in UnsavedData.basePowerSources)
            {
                if (ps == null || (int)ps.power < 1)
                    continue;
                //AddDebug("GetBasePowerDic " + ps.name + " " + (int)ps.power);
                TechType tt = CraftData.GetTechType(ps.gameObject);
                if (Patches.basePowerSourceTypes.Contains(tt))
                    basePower.AddValue(tt, (int)ps.power);
            }
            return basePower;
        }

        private static void GetBaseGlobalStats(StringBuilder sb)
        {
            Dictionary<string, int> roomsBuilt = GetDicGlobal(Main.config.baseRoomsBuilt);
            int corridorsBuilt = Main.config.baseCorridorsBuilt.Values.Sum();
            //AddDebug("roomsBuilt " + roomsBuilt.Count);
            //AddDebug("corridorsBuilt " + corridorsBuilt);
            AppendDic(sb, GetDicGlobal(Main.config.basePower), "ST_base_power_generated");
            if (corridorsBuilt > 0)
                sb.AppendLine(Language.main.Get("ST_base_corridors_built") + corridorsBuilt);

            AppendDic(sb, roomsBuilt, "ST_base_rooms_built");
            if (roomsBuilt.Count == 0 && corridorsBuilt > 0)
                sb.AppendLine();
        }

        private static void GetUnlockedStats(StringBuilder sb)
        {
            int numObjectsScanned = GetInt(Main.config.objectsScanned, UnsavedData.objectsScanned);
            if (numObjectsScanned > 0)
                sb.AppendLine(Language.main.Get("ST_objects_scanned") + numObjectsScanned);

            int sbLength = sb.Length;
            AppendSet(sb, MergeSets(Main.config.blueprintsUnlocked, UnsavedData.blueprintsUnlocked), "ST_scanned_blueprints");
            AppendSet(sb, MergeSets(Main.config.blueprintsFromDatabox, UnsavedData.blueprintsFromDatabox), "ST_databox_blueprints");

            if (numObjectsScanned > 0 && sbLength == sb.Length)
                sb.AppendLine();
        }

        private static void GetUnlockedGlobalStats(StringBuilder sb)
        {
            int numObjectsScanned = Main.config.objectsScanned.Values.Sum();
            if (numObjectsScanned > 0)
                sb.AppendLine(Language.main.Get("ST_objects_scanned") + numObjectsScanned);

            int sbLength = sb.Length;
            AppendSet(sb, GetSetGlobal(Main.config.blueprintsUnlocked), "ST_scanned_blueprints");
            AppendSet(sb, GetSetGlobal(Main.config.blueprintsFromDatabox), "ST_databox_blueprints");

            if (numObjectsScanned > 0 && sbLength == sb.Length)
                sb.AppendLine();
        }

        private static void AppendDic(StringBuilder sb, Dictionary<string, int> dic, string title)
        {
            int ValuesSum = dic.Values.Sum();
            if (dic.Count == 0 || ValuesSum == 0)
                return;

            sb.AppendLine(Language.main.Get(title) + ValuesSum);
            foreach (var kv in dic)
            {
                if (kv.Value > 0)
                    sb.AppendLine("     " + Language.main.Get(kv.Key) + ": " + kv.Value);
            }
            sb.AppendLine();
        }

        private static void AppendDic(StringBuilder sb, Dictionary<TechType, int> dic, string title)
        {
            int ValuesSum = dic.Values.Sum();
            if (dic.Count == 0 || ValuesSum == 0)
                return;

            sb.AppendLine(Language.main.Get(title) + ValuesSum);
            foreach (var kv in dic)
            {
                if (kv.Value > 0)
                    sb.AppendLine("     " + Language.main.Get(kv.Key) + ": " + kv.Value);
            }
            sb.AppendLine();
        }

        private static void AppendTimeDic(StringBuilder sb, Dictionary<string, TimeSpan> dic, string title)
        {
            if (dic.Count == 0)
                return;

            sb.AppendLine(Language.main.Get(title));
            foreach (var kv in dic)
                AppendTimeSpan(sb, kv.Value, Language.main.Get(kv.Key) + ": ", true);
            //sb.AppendLine("     " + Language.main.Get(kv.Key) + " " + kv.Value);

            sb.AppendLine();
        }

        private static void AppendSet(StringBuilder sb, HashSet<string> set, string title)
        {
            if (set.Count == 0)
                return;

            sb.AppendLine(Language.main.Get(title));
            foreach (var s in set)
                sb.AppendLine("     " + Language.main.Get(s));

            sb.AppendLine();
        }

        private static void GetDiscoverStats(StringBuilder sb)
        {
            AppendSet(sb, MergeSets(Main.config.faunaFound, UnsavedData.faunaFound), "ST_fauna_discovered");
            AppendSet(sb, MergeSets(Main.config.floraFound, UnsavedData.floraFound), "ST_flora_discovered");
            AppendSet(sb, MergeSets(Main.config.coralFound, UnsavedData.coralFound), "ST_corals_discovered");
            AppendSet(sb, MergeSets(Main.config.leviathanFound, UnsavedData.leviathanFound), "ST_leviathans_discovered");
        }
        private static void GetGlobalDiscoverStats(StringBuilder sb)
        {
            AppendSet(sb, GetSetGlobal(Main.config.faunaFound), "ST_fauna_discovered");
            AppendSet(sb, GetSetGlobal(Main.config.floraFound), "ST_flora_discovered");
            AppendSet(sb, GetSetGlobal(Main.config.coralFound), "ST_corals_discovered");
            AppendSet(sb, GetSetGlobal(Main.config.leviathanFound), "ST_leviathans_discovered");
        }

        private static string GetCurrentStats()
        {
            //AddDebug("GetCurrentStats");
            //string biomeName = Player.main.GetBiomeString();
            string biomeName = Language.main.Get(Util.GetBiomeName());
            StringBuilder sb = new StringBuilder(Language.main.Get("ST_current_biome") + biomeName + "\n");
            sb.AppendLine();
            GetTimeStats(sb);
            GetTravelStats(sb);
            GetHealthStats(sb);
            GetFoodStats(sb);
            GetBaseStats(sb);
            AppendDic(sb, MergeDics(Main.config.constructorBuilt, UnsavedData.constructorBuilt), "ST_constructor_built");
            AppendDic(sb, MergeDics(Main.config.vehiclesLost, UnsavedData.vehiclesLost), "ST_vehicles_lost");
            AppendDic(sb, MergeDics(Main.config.builderToolBuilt, UnsavedData.builderToolBuilt), "ST_builder_tool_built");
            GetUnlockedStats(sb);
            AppendTimeDic(sb, MergeDics(Main.config.timeBiomes, UnsavedData.timeBiomes), "ST_time_biomes");
            AppendDic(sb, MergeDics(Main.config.itemsCrafted, UnsavedData.itemsCrafted), "ST_items_crafted");
            AppendDic(sb, MergeDics(Main.config.plantsGrown, UnsavedData.plantsGrown), "ST_plants_grown");
            AppendDic(sb, MergeDics(Main.config.eggsHatched, UnsavedData.eggsHatched), "ST_hatched_eggs");
            GetDiscoverStats(sb);
            GetKilledStats(sb);
            return sb.ToString();
        }

        private static string GetGlobalStats()
        {
            StringBuilder sb = new StringBuilder();
            GetTimeGlobalStats(sb);
            GetTravelGlobalStats(sb);
            GetHealthGlobalStats(sb);
            GetFoodGlobalStats(sb);
            GetBaseGlobalStats(sb);
            AppendDic(sb, GetDicGlobal(Main.config.constructorBuilt), "ST_constructor_built");
            AppendDic(sb, GetDicGlobal(Main.config.vehiclesLost), "ST_vehicles_lost");
            AppendDic(sb, GetDicGlobal(Main.config.builderToolBuilt), "ST_builder_tool_built");
            GetUnlockedGlobalStats(sb);
            AppendTimeDic(sb, GetDicGlobal(Main.config.timeBiomes), "ST_time_biomes");
            AppendDic(sb, GetDicGlobal(Main.config.itemsCrafted), "ST_items_crafted");
            AppendDic(sb, GetDicGlobal(Main.config.plantsGrown), "ST_plants_grown");
            AppendDic(sb, GetDicGlobal(Main.config.eggsHatched), "ST_hatched_eggs");
            GetGlobalDiscoverStats(sb);
            GetKilledGlobalStats(sb);
            return sb.ToString();
        }

        [HarmonyPatch(typeof(Language), "TryGet")]
        internal class Language_TryGet_Patch
        {
            public static void Postfix(Language __instance, string key, ref string result)
            {
                if (!Main.setupDone || descs == null || key == null || !descs.ContainsKey(key))
                    return;
                //AddDebug("TryGet " + key);
                if (key == "EncyDesc_ST_StatsThisGame")
                {
                    result = GetCurrentStats();
                }
                else if (key == "EncyDesc_ST_StatsGlobal")
                {
                    result = GetGlobalStats();
                }
            }
        }

        [HarmonyPatch(typeof(uGUI_EncyclopediaTab))]
        internal class uGUI_EncyclopediaTab_Patch
        {
            static CraftNode lastEncNode;

            [HarmonyPostfix]
            [HarmonyPatch("Open")]
            public static void OpenPostfix(uGUI_EncyclopediaTab __instance)
            {
                if (lastEncNode != null && myStrings.ContainsKey(lastEncNode.id))
                { // update stats 
                  //AddDebug("update tab");
                    __instance.activeEntry = null;
                    __instance.Activate(lastEncNode);
                }
            }
            [HarmonyPostfix]
            [HarmonyPatch("Activate")]
            public static void ActivatePostfix(uGUI_EncyclopediaTab __instance, CraftNode node)
            {
                lastEncNode = node;
                //if (strings.ContainsKey(node.id))
                //{
                //    AddDebug("Activate " + __instance.activeEntry.key);
                //}
            }
        }


    }

}