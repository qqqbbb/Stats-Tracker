using BepInEx;
using BepInEx.Configuration;
using Nautilus.Commands;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ErrorMessage;

namespace Stats_Tracker
{
    internal class ConfigMenu
    {
        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> fahrenhiet;
        public static ConfigEntry<bool> miles;
        public static ConfigEntry<bool> pounds;
        public static ConfigEntry<bool> biomeName;

        public static void Bind()
        {
            modEnabled = Main.configMenu.Bind("", Language.main.Get("ST_mod_enabled"), true);
            biomeName = Main.configMenu.Bind("", Language.main.Get("ST_biome_name"), false);

            fahrenhiet = Main.configMenu.Bind("", "Show temperature in Fahrenhiet", false);
            miles = Main.configMenu.Bind("", "Show distance in miles and yards", false);
            pounds = Main.configMenu.Bind("", "Show weight in pounds", false);

        }


    }
}