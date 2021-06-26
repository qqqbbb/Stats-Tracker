using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using System.Collections.Generic;
using System;

namespace Stats_Tracker
{
    //[Menu("Custom Spawner Settings")]
    public class Config : ConfigFile
    {
        // public Config(string fileName = "config", string subfolder = null) : base(fileName, subfolder)
        // {
        // }
        public Dictionary<string, TimeSpan> timePlayed = new Dictionary<string, TimeSpan>();
        public Dictionary<string, TimeSpan> timeEscapePod = new Dictionary<string, TimeSpan>();
        public Dictionary<string, TimeSpan> timeSwam = new Dictionary<string, TimeSpan>();
        public Dictionary<string, TimeSpan> timeWalked = new Dictionary<string, TimeSpan>();
        public Dictionary<string, TimeSpan> timeSeamoth = new Dictionary<string, TimeSpan>();
        public Dictionary<string, TimeSpan> timeExosuit = new Dictionary<string, TimeSpan>();
        public Dictionary<string, TimeSpan> timeCyclops = new Dictionary<string, TimeSpan>();
        public Dictionary<string, TimeSpan> timeBase = new Dictionary<string, TimeSpan>();
        public Dictionary<string, TimeSpan> timeSlept = new Dictionary<string, TimeSpan>();
        public Dictionary<string, int> playerDeaths = new Dictionary<string, int>();
        public int gamesWon = 0;
        public Dictionary<string, int> healthLost = new Dictionary<string, int>();
        public Dictionary<string, float> foodEaten = new Dictionary<string, float>();
        public Dictionary<string, float> waterDrunk = new Dictionary<string, float>();
        public Dictionary<string, int> distanceTraveled = new Dictionary<string, int>();
        public Dictionary<string, int> maxDepth = new Dictionary<string, int>();
        public Dictionary<string, int> distanceTraveledSwim = new Dictionary<string, int>();
        public Dictionary<string, int> distanceTraveledWalk = new Dictionary<string, int>();
        public Dictionary<string, int> distanceTraveledSeaglide = new Dictionary<string, int>();
        public Dictionary<string, int> distanceTraveledSeamoth = new Dictionary<string, int>();
        public Dictionary<string, int> distanceTraveledExosuit = new Dictionary<string, int>();
        public Dictionary<string, int> distanceTraveledSub = new Dictionary<string, int>();
        public Dictionary<string, int> seamothsBuilt = new Dictionary<string, int>();
        public Dictionary<string, int> exosuitsBuilt = new Dictionary<string, int>();
        public Dictionary<string, int> cyclopsBuilt = new Dictionary<string, int>();
        public Dictionary<string, int> seamothsLost = new Dictionary<string, int>();
        public Dictionary<string, int> exosuitsLost = new Dictionary<string, int>();
        public Dictionary<string, int> cyclopsLost = new Dictionary<string, int>();
        public Dictionary<string, int> itemsCrafted = new Dictionary<string, int>();
        public Dictionary<string, HashSet<TechType>> diffItemsCrafted = new Dictionary<string, HashSet<TechType>>();
        public Dictionary<string, int> baseRoomsBuilt = new Dictionary<string, int>();
        public Dictionary<string, int> baseCorridorsBuilt = new Dictionary<string, int>();
        public Dictionary<string, int> basePower = new Dictionary<string, int>();
        public Dictionary<string, int> objectsScanned = new Dictionary<string, int>();
        public Dictionary<string, int> blueprintsUnlocked = new Dictionary<string, int>();
        public Dictionary<string, int> blueprintsFromDatabox = new Dictionary<string, int>();
        public Dictionary<string, int> floraFound = new Dictionary<string, int>();
        public Dictionary<string, int> faunaFound = new Dictionary<string, int>();
        public Dictionary<string, int> coralFound = new Dictionary<string, int>();
        public Dictionary<string, int> leviathanFound = new Dictionary<string, int>();
        public Dictionary<string, int> animalsKilled = new Dictionary<string, int>();
        public Dictionary<string, int> plantsKilled = new Dictionary<string, int>();
        public Dictionary<string, int> coralKilled = new Dictionary<string, int>();
        public Dictionary<string, int> leviathansKilled = new Dictionary<string, int>();
        public Dictionary<string, int> ghostsKilled = new Dictionary<string, int>();
        public Dictionary<string, int> repersKilled = new Dictionary<string, int>();
        public Dictionary<string, int> reefbacksKilled = new Dictionary<string, int>();
        public Dictionary<string, int> seaDragonsKilled = new Dictionary<string, int>();
        public Dictionary<string, int> seaEmperorsKilled = new Dictionary<string, int>();
        public Dictionary<string, int> seaTreadersKilled = new Dictionary<string, int>();
        public Dictionary<string, int> gulpersKilled = new Dictionary<string, int>();
        public Dictionary<string, int> plantsRaised = new Dictionary<string, int>();
        public Dictionary<string, int> eggsHatched = new Dictionary<string, int>();
        public Dictionary<string, HashSet<TechType>> diffEggsHatched = new Dictionary<string, HashSet<TechType>>();
        public Dictionary<string, float> craftingResourcesUsed = new Dictionary<string, float>();
        public Dictionary<string, HashSet<string>> biomesFound = new Dictionary<string, HashSet<string>>();
    }
}