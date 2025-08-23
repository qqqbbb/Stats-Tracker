using BepInEx;
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
    public class Config : ConfigFile
    {
        [Toggle("Mod enabled", Tooltip = "Only saved stats will be shown and no new data will be saved when the mod is disabled")]
        public bool modEnabled = true;

        [Toggle("Show temperature in Fahrenhiet")]
        public bool fahrenhiet = false;

        [Toggle("Show distance in miles and yards")]
        public bool miles = false;

        [Toggle("Show weight in pounds")]
        public bool pounds = false;

        public Dictionary<string, TimeSpan> timePlayed = new Dictionary<string, TimeSpan>();
        public Dictionary<string, TimeSpan> timeEscapePod = new Dictionary<string, TimeSpan>();
        public Dictionary<string, TimeSpan> timeSwam = new Dictionary<string, TimeSpan>();
        public Dictionary<string, TimeSpan> timeWalked = new Dictionary<string, TimeSpan>();
        public Dictionary<string, Dictionary<string, TimeSpan>> timeVehicles = new Dictionary<string, Dictionary<string, TimeSpan>>();
        public Dictionary<string, TimeSpan> timeBase = new Dictionary<string, TimeSpan>();
        public Dictionary<string, TimeSpan> timeSlept = new Dictionary<string, TimeSpan>();
        public Dictionary<string, int> playerDeaths = new Dictionary<string, int>();
        public int gamesWon = 0;
        public Dictionary<string, int> healthLost = new Dictionary<string, int>();
        public Dictionary<string, int> medkitsUsed = new Dictionary<string, int>();
        public Dictionary<string, Dictionary<string, float>> foodEaten = new Dictionary<string, Dictionary<string, float>>();
        public Dictionary<string, float> waterDrunk = new Dictionary<string, float>();
        public Dictionary<string, int> distanceTraveled = new Dictionary<string, int>();
        public Dictionary<string, int> maxDepth = new Dictionary<string, int>();
        public Dictionary<string, int> distanceTraveledSwim = new Dictionary<string, int>();
        public Dictionary<string, int> distanceTraveledWalk = new Dictionary<string, int>();
        public Dictionary<string, int> distanceTraveledSeaglide = new Dictionary<string, int>();
        public Dictionary<string, Dictionary<string, int>> distanceTraveledVehicle = new Dictionary<string, Dictionary<string, int>>();

        public Dictionary<string, Dictionary<string, int>> vehiclesLost = new Dictionary<string, Dictionary<string, int>>();
        public Dictionary<string, Dictionary<string, int>> itemsCrafted = new Dictionary<string, Dictionary<string, int>>();
        public Dictionary<string, Dictionary<string, int>> baseRoomsBuilt = new Dictionary<string, Dictionary<string, int>>();
        public Dictionary<string, int> baseCorridorsBuilt = new Dictionary<string, int>();
        public Dictionary<string, Dictionary<string, int>> basePower = new Dictionary<string, Dictionary<string, int>>();
        public Dictionary<string, int> objectsScanned = new Dictionary<string, int>();
        public Dictionary<string, HashSet<string>> blueprintsFromDatabox = new Dictionary<string, HashSet<string>>();
        public Dictionary<string, HashSet<string>> blueprintsUnlocked = new Dictionary<string, HashSet<string>>();
        public Dictionary<string, HashSet<string>> floraFound = new Dictionary<string, HashSet<string>>();
        public Dictionary<string, HashSet<string>> faunaFound = new Dictionary<string, HashSet<string>>();
        public Dictionary<string, HashSet<string>> coralFound = new Dictionary<string, HashSet<string>>();
        public Dictionary<string, HashSet<string>> leviathanFound = new Dictionary<string, HashSet<string>>();
        public Dictionary<string, Dictionary<string, int>> animalsKilled = new Dictionary<string, Dictionary<string, int>>();
        public Dictionary<string, Dictionary<string, int>> plantsKilled = new Dictionary<string, Dictionary<string, int>>();
        public Dictionary<string, Dictionary<string, int>> coralKilled = new Dictionary<string, Dictionary<string, int>>();
        public Dictionary<string, Dictionary<string, int>> leviathansKilled = new Dictionary<string, Dictionary<string, int>>();
        public Dictionary<string, Dictionary<string, int>> plantsGrown = new Dictionary<string, Dictionary<string, int>>();
        public Dictionary<string, Dictionary<string, int>> eggsHatched = new Dictionary<string, Dictionary<string, int>>();
        public Dictionary<string, Dictionary<string, int>> pickedUpItems = new Dictionary<string, Dictionary<string, int>>();
        public Dictionary<string, Dictionary<string, int>> builderToolBuilt = new Dictionary<string, Dictionary<string, int>>();
        public Dictionary<string, Dictionary<string, int>> constructorBuilt = new Dictionary<string, Dictionary<string, int>>();
        public Dictionary<string, Dictionary<string, TimeSpan>> timeBiomes = new Dictionary<string, Dictionary<string, TimeSpan>>();
        public Dictionary<string, int> minTemp = new Dictionary<string, int>();
        public Dictionary<string, int> maxTemp = new Dictionary<string, int>();
        public Dictionary<string, int> minVehicleTemp = new Dictionary<string, int>();
        public Dictionary<string, int> maxVehicleTemp = new Dictionary<string, int>();
        public int permaDeaths = 0;
    }
}