using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static ErrorMessage;

namespace Stats_Tracker
{
    internal static class Util
    {
        static Vector2Int islandMinPos = new Vector2Int(239, 771);
        static Vector2Int islandMaxPos = new Vector2Int(491, 1257);
        static readonly Dictionary<string, string> biomeNames = new Dictionary<string, string>()
        {
            { "safeshallows", "ST_safe_shallows"},
            { "kelpforest", "ST_kelp_forest"},
            { "grassyplateaus", "ST_grassy_plateaus"},
            { "underwaterislands", "ST_underwater_islands"},
            { "mushroomforest", "ST_mushroom_forest" },
            { "kooshzone", "ST_koosh_zone"},
            { "grandreef", "ST_grand_reef" },
            { "ilz", "ST_inactive_lava_zone" },
            { "crashzone", "ST_crash_zone" },
            { "void", "ST_void" },
            { "sparsereef", "ST_sparse_reef" },
            { "dunes", "ST_dunes"},
            { "bloodkelp", "ST_blood_kelp"},
            { "mountains", "ST_mountains" },
            { "seatreaderpath", "ST_seatreader_path" },
            { "cragfield", "ST_cragfield" },
            { "lostriver", "ST_lost_river" },
            { "jellyshroomcaves", "ST_jellyshroom_caves" },
            { "floatingisland", "ST_floating_island" },
            { "lavafalls", "ST_active_lava_zone" },
            { "lavalakes", "ST_active_lava_zone" },
        };

        public static string GetBiomeName()
        {
            string name = LargeWorld.main.GetBiome(Player.main.transform.position).ToLower();
            if (name.StartsWith("bloodkelp"))
                return "ST_blood_kelp";
            else if (name.StartsWith("ilz"))
                return "ST_inactive_lava_zone";
            else if (name.StartsWith("lostriver"))
                return "ST_lost_river";
            else if (biomeNames.ContainsKey(name))
            {
                if (name == "mountains" && IsPlayerOnMountainIsland())
                    return "ST_mountain_island";

                return biomeNames[name];
            }
            else
                return "ST_unknown_biome";
        }

        public static bool IsPlayerOnMountainIsland()
        {
            Vector3 pos = Player.main.transform.position;
            if (pos.y > 0 && pos.x < islandMaxPos.x && pos.x > islandMinPos.x && pos.z < islandMaxPos.y && pos.z > islandMinPos.y)
                return true;
            else
                return false;
        }

        public static string GetFriendlyName(GameObject go)
        {
            TechType tt = CraftData.GetTechType(go);
            return Language.main.Get(tt.AsString(false));
        }

        public static string GetRawBiomeName()
        {
            AtmosphereDirector atmosphereDirector = AtmosphereDirector.main;
            if (atmosphereDirector)
            {
                string biomeOverride = atmosphereDirector.GetBiomeOverride();
                if (!string.IsNullOrEmpty(biomeOverride))
                    return biomeOverride;
            }
            LargeWorld largeWorld = LargeWorld.main;
            return largeWorld && Player.main ? largeWorld.GetBiome(Player.main.transform.position) : "<unknown>";
        }

        public static void AddValue(this Dictionary<string, TimeSpan> dic, string key, TimeSpan value)
        {
            dic[key] = dic.ContainsKey(key) ? dic[key] + value : value;
        }

        public static void AddValue(this Dictionary<string, int> dic, string key, int value)
        {
            dic[key] = dic.ContainsKey(key) ? dic[key] + value : value;
        }

        public static void AddValue(this Dictionary<string, float> dic, string key, float value)
        {
            dic[key] = dic.ContainsKey(key) ? dic[key] + value : value;
        }

        public static void AddValue(this Dictionary<TechType, int> dic, TechType key, int value)
        {
            dic[key] = dic.ContainsKey(key) ? dic[key] + value : value;
            //dic[tt] = dic.TryGetValue(tt, out var currentAmount) ? currentAmount + value : value;
        }

        public static void AddValue(this Dictionary<TechType, TimeSpan> dic, TechType key, TimeSpan value)
        {
            dic[key] = dic.ContainsKey(key) ? dic[key] + value : value;
        }

        public static void AddValue(this Dictionary<TechType, float> dic, TechType key, float value)
        {
            dic[key] = dic.ContainsKey(key) ? dic[key] + value : value;
        }

        public static float CelciusToFahrenhiet(float celcius)
        {
            return celcius * 1.8f + 32f;
        }

        public static float MeterToYard(float meter)
        {
            return meter * 1.0936f;
        }

        public static float KiloToPound(float kilos)
        {
            return kilos * 2.20462f;
        }

        public static float literToGallon(float liters)
        { // US gallon
            return liters / 3.785f;
        }


    }
}
