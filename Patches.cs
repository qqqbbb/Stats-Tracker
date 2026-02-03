using HarmonyLib;
using Nautilus.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;
using static ErrorMessage;

namespace Stats_Tracker
{
    internal class Patches
    {
        static LiveMixin killedLM = null;
        public static string saveSlot;
        public static TimeSpan timeLastUpdate = TimeSpan.Zero;
        public static HashSet<TechType> roomTypes = new HashSet<TechType> { TechType.BaseRoom, TechType.BaseMapRoom, TechType.BaseMoonpool, TechType.BaseObservatory, TechType.BaseLargeRoom };
        public static Dictionary<Base.CellType, TechType> roomTypeToTechtype = new Dictionary<Base.CellType, TechType>
        {
            { Base.CellType.Room,  TechType.BaseRoom },
            {  Base.CellType.MapRoom, TechType.BaseMapRoom },
            {  Base.CellType.MapRoomRotated, TechType.BaseMapRoom },
            {  Base.CellType.Moonpool, TechType.BaseMoonpool },
            {  Base.CellType.MoonpoolRotated, TechType.BaseMoonpool },
            {  Base.CellType.Observatory, TechType.BaseObservatory },
            {  Base.CellType.LargeRoom, TechType.BaseLargeRoom },
            {  Base.CellType.LargeRoomRotated, TechType.BaseLargeRoom },
        };
        public static HashSet<TechType> corridorTypes = new HashSet<TechType> { TechType.BaseCorridorI, TechType.BaseCorridorL, TechType.BaseCorridorT, TechType.BaseCorridorX, TechType.BaseCorridorGlassI, TechType.BaseCorridorGlassL, TechType.BaseCorridor, TechType.BaseCorridorGlass };
        public static HashSet<TechType> basePowerSourceTypes = new HashSet<TechType> { TechType.SolarPanel, TechType.ThermalPlant, TechType.BaseNuclearReactor, TechType.BaseBioReactor };

        //public static HashSet<TechType> faunaVanilla = new HashSet<TechType> { TechType.Shocker, TechType.Biter, TechType.Blighter, TechType.BoneShark, TechType.Crabsnake, TechType.CrabSquid, TechType.Crash, TechType.LavaLizard, TechType.Mesmer, TechType.SpineEel, TechType.Sandshark, TechType.Stalker, TechType.Warper, TechType.Bladderfish, TechType.Boomerang, TechType.GhostRayRed, TechType.Cutefish, TechType.Eyeye, TechType.GarryFish, TechType.Gasopod, TechType.GhostRayBlue, TechType.HoleFish, TechType.Hoopfish, TechType.Hoverfish, TechType.Jellyray, TechType.LavaBoomerang, TechType.Oculus, TechType.Peeper, TechType.RabbitRay, TechType.LavaEyeye, TechType.Reginald, TechType.Skyray, TechType.Spadefish, TechType.Spinefish, TechType.BlueAmoeba, TechType.LargeFloater, TechType.Bleeder, TechType.Shuttlebug, TechType.CaveCrawler, TechType.Floater, TechType.LavaLarva, TechType.Rockgrub, TechType.Jumper };
        public static HashSet<TechType> leviathans = new HashSet<TechType>();
        public static HashSet<TechType> creatures = new HashSet<TechType>();
        //public static HashSet<TechType> leviathansVanilla = new HashSet<TechType>
        //{TechType.GhostLeviathan, TechType.GhostLeviathanJuvenile, TechType.ReaperLeviathan, TechType.Reefback, TechType.SeaDragon, TechType.SeaEmperorJuvenile, TechType.SeaTreader };
        public static HashSet<TechType> coral = new HashSet<TechType> { TechType.PurpleBrainCoral, TechType.CoralShellPlate, TechType.BrownTubes, TechType.BigCoralTubes, TechType.BlueCoralTubes, TechType.RedTipRockThings, TechType.GenericJeweledDisk, TechType.BlueJeweledDisk, TechType.GreenJeweledDisk, TechType.RedJeweledDisk, TechType.PurpleJeweledDisk, TechType.TreeMushroom };
        public static HashSet<TechType> flora = new HashSet<TechType> { TechType.MelonPlant, TechType.AcidMushroom, TechType.BloodRoot, TechType.BloodVine, TechType.BluePalm, TechType.SmallKoosh, TechType.MediumKoosh, TechType.LargeKoosh, TechType.HugeKoosh, TechType.BulboTree, TechType.PurpleBranches, TechType.PurpleVegetablePlant, TechType.Creepvine, TechType.WhiteMushroom, TechType.EyesPlant, TechType.FernPalm, TechType.RedRollPlant, TechType.GabeSFeather, TechType.JellyPlant, TechType.RedGreenTentacle, TechType.OrangePetalsPlant, TechType.OrangeMushroom, TechType.SnakeMushroom, TechType.HangingFruitTree, TechType.MembrainTree, TechType.PurpleVasePlant, TechType.PinkMushroom, TechType.SmallFan, TechType.SmallFanCluster, TechType.RedBush, TechType.RedConePlant, TechType.RedBasketPlant, TechType.SeaCrown, TechType.PurpleRattle, TechType.ShellGrass, TechType.SpottedLeavesPlant, TechType.CrashHome, TechType.SpikePlant, TechType.PurpleFan, TechType.PurpleStalk, TechType.PinkFlower, TechType.PurpleTentacle, TechType.BloodGrass, TechType.RedGrass, TechType.RedSeaweed, TechType.BlueBarnacle, TechType.BlueBarnacleCluster, TechType.BlueLostRiverLilly, TechType.BlueTipLostRiverPlant, TechType.HangingStinger, TechType.CoveTree, TechType.BlueCluster, TechType.GreenReeds, TechType.BarnacleSuckers, TechType.BallClusters };
        public static HashSet<TechType> constructorBuilt = new HashSet<TechType>();
        public static TechType currentVehicleTT;
        static Dictionary<TechType, float> itemMass = new Dictionary<TechType, float>();
        const int leviathanMinHealth = 2000;
        static bool playerTakesDamage = false;
        public static bool teleporting = false;
        static TechType hatchedTT = TechType.None;


        public static TimeSpan GetTimeSpanPlayed()
        {
            return new TimeSpan(0, 0, Mathf.FloorToInt(DayNightCycle.main.timePassedSinceOrigin * DayNightCycle.gameSecondMultiplier));
        }

        static float GetItemMass(TechType techType)
        {
            ItemsContainer.ItemGroup itemGroup;
            if (!Inventory.main._container._items.TryGetValue(techType, out itemGroup))
                return 0f;

            List<InventoryItem> items = itemGroup.items;
            int index1 = items.Count - 1;
            InventoryItem inventoryItem1 = items[index1];
            Rigidbody rb = inventoryItem1.item.GetComponent<Rigidbody>();
            if (rb)
                return rb.mass;

            return 0f;
        }

        static float GetItemMass(TechType techType, GameObject gameObject)
        {
            if (itemMass.ContainsKey(techType))
                return itemMass[techType];

            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb)
            {
                itemMass[techType] = rb.mass;
                return rb.mass;
            }
            return 0f;
        }

        [HarmonyPatch(typeof(Creature), "Start")]
        class Creature_Start_Patch
        {
            static void Postfix(Creature __instance)
            {
                if (__instance.liveMixin)
                {
                    TechType tt = CraftData.GetTechType(__instance.gameObject);
                    if (__instance.liveMixin && __instance.liveMixin.maxHealth >= leviathanMinHealth)
                        leviathans.Add(tt);
                    else
                        creatures.Add(tt);
                }
            }
        }

        [HarmonyPatch(typeof(PowerSource))]
        class PowerSource_Patch
        {
            [HarmonyPostfix, HarmonyPatch("Start")]
            static void StartPostfix(PowerSource __instance)
            {
                TechType tt = CraftData.GetTechType(__instance.gameObject);
                if (basePowerSourceTypes.Contains(tt))
                {
                    //AddDebug("PowerSource Start " + tt + " power " + (int)__instance.power);
                    UnsavedData.basePowerSources.Add(__instance);
                }
            }
        }

        [HarmonyPatch(typeof(CraftingAnalytics))]
        class CraftingAnalytics_Patch
        {
            //[HarmonyPrefix]
            //[HarmonyPatch("OnConstruct")]
            static void OnConstructPrefix(CraftingAnalytics __instance, TechType techType)
            {
                //AddDebug("CraftingAnalytics OnConstruct " + techType);


            }

            [HarmonyPostfix, HarmonyPatch("OnConstruct")]
            static void OnConstructPostfix(CraftingAnalytics __instance, TechType techType)
            {
                //AddDebug("CraftingAnalytics OnConstruct " + techType);
                if (!Main.config.modEnabled)
                    return;

                if (corridorTypes.Contains(techType) || roomTypes.Contains(techType))
                    return;

                if (!constructorBuilt.Contains(techType))
                    UnsavedData.builderToolBuilt.AddValue(techType, 1);
            }

            [HarmonyPostfix, HarmonyPatch("OnCraft")]
            static void OnCraftPostfix(CraftingAnalytics __instance, TechType techType)
            {
                if (!Main.config.modEnabled)
                    return;

                //AddDebug("CraftingAnalytics OnCraft " + techType);
                if (constructorBuilt.Contains(techType))
                {
                    UnsavedData.constructorBuilt.AddValue(techType, 1);
                    constructorBuilt.Remove(techType);
                    return;
                }
                ReadOnlyCollection<Ingredient> ingredients = TechData.GetIngredients(techType);
                if (ingredients == null)
                    return;

                for (int j = 0; j < ingredients.Count; j++)
                {
                    Ingredient ingredient = ingredients[j];
                    TechType ingredientTT = ingredient.techType;
                    //AddDebug("ingredient " + ingredientTT + " " + ingredient.amount);
                    if (creatures.Contains(ingredientTT))
                    {
                        //AddDebug(ingredientTT + " animal killed by player");
                        UnsavedData.animalsKilled.AddValue(ingredientTT, ingredient.amount);
                    }
                    else if (flora.Contains(ingredientTT))
                    {
                        //AddDebug(ingredientTT + " plant killed by player");
                        UnsavedData.plantsKilled.AddValue(ingredientTT, ingredient.amount);
                    }
                }
                string tts = techType.ToString();
                if (tts.StartsWith("Cooked") || tts.StartsWith("Cured"))
                    return;

                //AddDebug(techType + " Craft Amount " + TechData.GetCraftAmount(techType));
                UnsavedData.itemsCrafted.AddValue(techType, TechData.GetCraftAmount(techType));
            }
        }

        [HarmonyPatch(typeof(Player))]
        internal class Player_Patch
        {
            //static string currentBiome;
            static BasicText currentBiome = new BasicText(0, 250);

            private static void SaveTravelStats(Player player)
            {
                Vector3 position = player.transform.position;
                player.maxDepth = Mathf.Max(player.maxDepth, -position.y);
                UnsavedData.maxDepth = (int)player.maxDepth;
                //if (biomeName != "ST_unknown_biome")
                //    UnsavedData.biomesFound.Add(biomeName);

                if (player.lastPosition != Vector3.zero) // first run after game loads it's Vector3.zero
                    player.distanceTraveled += Vector3.Distance(position, player.lastPosition);

                int distanceTraveledSinceLastUpdate = Mathf.RoundToInt((player.lastPosition - position).magnitude);
                if (player.lastPosition == Vector3.zero)
                    distanceTraveledSinceLastUpdate = 0;

                player.lastPosition = position;
                if (distanceTraveledSinceLastUpdate == 0)
                    return;

                UnsavedData.distanceTraveled += distanceTraveledSinceLastUpdate;

                if (player.motorMode == Player.MotorMode.Seaglide)
                {
                    UnsavedData.distanceTraveledSeaglide += distanceTraveledSinceLastUpdate;
                }
                else if (player.IsUnderwaterForSwimming())
                {
                    UnsavedData.distanceTraveledSwim += distanceTraveledSinceLastUpdate;
                }
                else if (player.currentMountedVehicle && currentVehicleTT != TechType.None)
                {
                    UnsavedData.distanceTraveledVehicle.AddValue(currentVehicleTT, distanceTraveledSinceLastUpdate);
                }
                else if (player.mode == Player.Mode.Piloting && player.currentSub)
                {
                    UnsavedData.distanceTraveledVehicle.AddValue(TechType.Cyclops, distanceTraveledSinceLastUpdate);
                }
                else if (player.motorMode == Player.MotorMode.Walk || player.motorMode == Player.MotorMode.Run)
                {
                    UnsavedData.distanceTraveledWalk += distanceTraveledSinceLastUpdate;
                }
            }

            private static void SaveTimeStats(Player player)
            {
                string biomeName = Util.GetBiomeName();
                TimeSpan timeSinceLastUpdate = GetTimeSpanPlayed() - timeLastUpdate;
                //AddDebug("lastBiome " + lastBiome);
                if (biomeName != "ST_unknown_biome")
                {
                    UnsavedData.timeBiomes.AddValue(biomeName, timeSinceLastUpdate);
                }
                if (player.IsUnderwaterForSwimming())
                {
                    UnsavedData.timeSwam += timeSinceLastUpdate;
                }
                else if (player.mode == Player.Mode.Sitting)
                {
                    UnsavedData.timeSat += timeSinceLastUpdate;
                }
                else if (player.motorMode == Player.MotorMode.Walk || player.motorMode == Player.MotorMode.Run)
                { // MotorMode.Run when swimming on surface
                    UnsavedData.timeWalked += timeSinceLastUpdate;
                }
                else if (player.currentMountedVehicle && currentVehicleTT != TechType.None)
                {
                    //AddDebug("SAVE Vehicle time " + currentVehicleTT);
                    UnsavedData.timeVehicles.AddValue(currentVehicleTT, timeSinceLastUpdate);
                }

                if (player.currentEscapePod)
                {
                    UnsavedData.timeEscapePod += timeSinceLastUpdate;
                }
                else if (player.currentSub)
                {
                    if (player.currentSub.isCyclops)
                        UnsavedData.timeVehicles.AddValue(TechType.Cyclops, timeSinceLastUpdate);
                    else
                        UnsavedData.timeBase += timeSinceLastUpdate;
                }
                else if (player.precursorOutOfWater || PrecursorMoonPoolTrigger.inMoonpool)
                {
                    UnsavedData.timePrecursor += timeSinceLastUpdate;
                }
                //AddDebug("timeSwam " + UnsavedData.timeSwam[saveSlot]);
                timeLastUpdate = GetTimeSpanPlayed();
            }

            private static void SaveTempStats(Player player)
            {
                float temp = float.NaN;
                if (player.currentMountedVehicle || player.currentSub)
                {
                    if (player.currentMountedVehicle)
                    {
                        temp = player.currentMountedVehicle.GetTemperature();
                        //AddDebug("Vehicle temp " + (int)temp);
                    }
                    else if (player.currentSub)
                    {
                        temp = player.currentSub.GetTemperature();
                        //AddDebug("currentSub temp " + (int)temp);
                    }
                    if (temp < UnsavedData.minVehicleTemp)
                        UnsavedData.minVehicleTemp = Mathf.RoundToInt(temp);

                    if (temp > UnsavedData.maxVehicleTemp)
                        UnsavedData.maxVehicleTemp = Mathf.RoundToInt(temp);

                    return;
                }
                if (WaterTemperatureSimulation.main == null)
                    return;

                temp = WaterTemperatureSimulation.main.GetTemperature(player.transform.position);
                //AddDebug("player temp " + (int)temp);
                if (temp < UnsavedData.minTemp)
                    UnsavedData.minTemp = Mathf.RoundToInt(temp);

                if (temp > UnsavedData.maxTemp)
                    UnsavedData.maxTemp = Mathf.RoundToInt(temp);
            }

            [HarmonyPrefix, HarmonyPatch("TrackTravelStats")]
            public static bool TrackTravelStatsPrefix(Player __instance)
            {
                if (!Main.config.modEnabled)
                    return true;

                if (Main.setupDone == false || teleporting)
                    return false;

                if (__instance.isNewBorn && GetTimeSpanPlayed() == TimeSpan.Zero)
                {// intro
                    return false;
                }
                //AddDebug("TrackTravelStats cinematicModeActive " + __instance.cinematicModeActive);
                //AddDebug("lastPosition " + Player.main.lastPosition);
                SaveTravelStats(__instance);
                SaveTimeStats(__instance);
                SaveTempStats(__instance);
                return false;
            }

            [HarmonyPostfix, HarmonyPatch("OnKill")]
            public static void OnKillPostfix(Player __instance)
            {
                if (!Main.config.modEnabled)
                    return;

                if (GameModeUtils.IsPermadeath())
                {
                    Main.config.permaDeaths++;
                    Main.config.Save();
                }
                else
                    UnsavedData.playerDeaths++;
            }

            [HarmonyPostfix, HarmonyPatch("Update")]
            public static void UpdatePostfix(Player __instance)
            {
                ShowBiomeName();
            }

            private static void ShowBiomeName()
            {
                if (Main.config.biomeName == false || Main.setupDone == false)
                    return;

                string biomeName = Language.main.Get(Util.GetBiomeName());
                //AddDebug("biomeName " + biomeName);
                if (currentBiome.GetText() == biomeName)
                    return;

                currentBiome.ShowMessage(biomeName, 5);
                //currentBiome.SetColor(Color.green);
            }
        }

        [HarmonyPatch(typeof(PrecursorTeleporter))]
        class PrecursorTeleporter_Patch
        {
            [HarmonyPrefix, HarmonyPatch("BeginTeleportPlayer")]
            public static void BeginTeleportPlayerPrefix(PrecursorTeleporter __instance, GameObject teleportObject)
            {
                //AddDebug("BeginTeleportPlayer");
                teleporting = true;
                Player.main.lastPosition = Vector3.zero;
            }
            [HarmonyPostfix, HarmonyPatch("OnEndTeleportPlayer")]
            public static void OnEndTeleportPlayerPostfix(PrecursorTeleporter __instance)
            {
                //AddDebug("OnEndTeleportPlayer");
                teleporting = false;
            }
        }

        [HarmonyPatch(typeof(Survival))]
        class Survival_Patch
        {
            [HarmonyPostfix, HarmonyPatch("Use")]
            public static void UsePostfix(Survival __instance, GameObject useObj, bool __result)
            {
                if (!Main.config.modEnabled || !__result)
                    return;

                TechType tt = CraftData.GetTechType(useObj);
                if (tt == TechType.FirstAidKit)
                {
                    //AddDebug("medkit used");
                    UnsavedData.medkitsUsed++;
                }
            }
            [HarmonyPostfix, HarmonyPatch("Eat")]
            public static void EatPostfix(Survival __instance, GameObject useObj, bool __result)
            {
                if (!Main.config.modEnabled || __result == false)
                    return;

                TechType tt = CraftData.GetTechType(useObj);
                if (creatures.Contains(tt))
                {
                    //AddDebug(tt + " animal killed by player");
                    LiveMixin lm = useObj.GetComponent<LiveMixin>();
                    if (lm && lm.IsAlive())
                        UnsavedData.animalsKilled.AddValue(tt, 1);
                }
                else if (flora.Contains(tt))
                {
                    //AddDebug(tt + " animal killed by player");
                    LiveMixin lm = useObj.GetComponent<LiveMixin>();
                    if (lm && lm.IsAlive())
                        UnsavedData.plantsKilled.AddValue(tt, 1);
                }
                float mass = GetItemMass(tt, useObj);
                if (mass == 0)
                    return;

                Eatable eatable = useObj.GetComponent<Eatable>();
                if (eatable == null)
                    return;

                float foodValue = eatable.GetFoodValue();
                float waterValue = eatable.GetWaterValue();
                if (foodValue >= waterValue)
                    UnsavedData.foodEaten.AddValue(tt, mass);
                else if (waterValue > 0 && foodValue == 0)
                    UnsavedData.waterDrunk += mass;
            }
        }

        [HarmonyPatch(typeof(LaunchRocket), "StartEndCinematic")]
        internal class LaunchRocket_StartEndCinematic_Patch
        {
            public static void Postfix(LaunchRocket __instance)
            {
                if (!Main.config.modEnabled)
                    return;

                Main.config.gamesWon++;
                Main.config.Save();
            }
        }

        [HarmonyPatch(typeof(GrowingPlant), "SpawnGrownModel")]
        internal class GrowingPlant_SpawnGrownModel_Patch
        {
            public static void Postfix(GrowingPlant __instance)
            {
                if (!Main.config.modEnabled)
                    return;

                TechType tt = __instance.seed.plantTechType;
                if (tt == TechType.None)
                    return;

                //string name = __instance.seed.plantTechType.AsString();
                //AddDebug("GrownPlant Awake " + tt);
                UnsavedData.plantsGrown.AddValue(tt, 1);
            }
        }

        [HarmonyPatch(typeof(DamageSystem), "CalculateDamage")]
        class DamageSystem_CalculateDamage_Patch
        {
            public static void Postfix(DamageSystem __instance, float damage, DamageType type, GameObject target, GameObject dealer, ref float __result)
            {
                if (!Main.config.modEnabled)
                    return;

                if (playerTakesDamage && __result > 0)
                {
                    float currentHealth = Player.main.GetComponent<LiveMixin>().health;
                    float d = __result > currentHealth ? currentHealth : __result;
                    //AddDebug("Player takes damage " + d);
                    UnsavedData.healthLost += Mathf.RoundToInt(d);
                    playerTakesDamage = false;
                }
            }
        }
        [HarmonyPatch(typeof(LiveMixin))]
        internal class LiveMixin_Patch
        {
            public static bool WasKilledByPlayer(LiveMixin liveMixin, GameObject killer)
            {
                //TechType tt = CraftData.GetTechType(__instance.gameObject);
                //AddDebug("Kill " + tt);
                if (killer && killedLM && liveMixin == killedLM)
                {
                    if (killer == Player.main.gameObject || killer == Player.main.currentMountedVehicle?.gameObject || killer == Player.main.currentSub?.gameObject)
                        return true;
                }
                return false;
            }

            [HarmonyPrefix, HarmonyPatch("Kill")]
            public static void KillPrefix(LiveMixin __instance)
            {
                //TechType tt = CraftData.GetTechType(__instance.gameObject);
                //AddDebug("Kill " + tt);
                killedLM = __instance;
            }

            [HarmonyPrefix, HarmonyPatch("TakeDamage")]
            public static void TakeDamagePrefix(LiveMixin __instance, float originalDamage, Vector3 position, DamageType type, GameObject dealer)
            {
                if (__instance.TryGetComponent(out Player player))
                {
                    //AddDebug($"invincible {__instance.invincible} shielded {__instance.shielded} currentSub {player.currentSub} IsInvisible() {GameModeUtils.IsInvisible()}");
                    if (__instance.invincible || __instance.shielded || player.currentSub || player.currentEscapePod || __instance.health <= 0 || GameModeUtils.IsInvisible())
                        return;

                    playerTakesDamage = true;
                }
            }
            [HarmonyPostfix, HarmonyPatch("TakeDamage")]
            public static void TakeDamagePostfix(LiveMixin __instance, float originalDamage, Vector3 position, DamageType type, GameObject dealer)
            {
                if (!Main.config.modEnabled)
                    return;

                if (WasKilledByPlayer(__instance, dealer))
                {
                    killedLM = null;
                    TechType tt = CraftData.GetTechType(__instance.gameObject);
                    if (tt == TechType.None)
                        return;
                    //AddDebug(tt + " killed by player");
                    //string name = tt.AsString();
                    if (creatures.Contains(tt))
                    {
                        //AddDebug(tt + " animal killed by player");
                        UnsavedData.animalsKilled.AddValue(tt, 1);
                    }
                    else if (flora.Contains(tt))
                    {
                        //AddDebug(tt + " plant killed by player");
                        UnsavedData.plantsKilled.AddValue(tt, 1);
                    }
                    else if (coral.Contains(tt))
                    {
                        //AddDebug(tt + " coral killed by player");
                        UnsavedData.coralKilled.AddValue(tt, 1);
                    }
                    else if (leviathans.Contains(tt))
                    {
                        //AddDebug(tt + " leviathan killed by player");
                        UnsavedData.leviathansKilled.AddValue(tt, 1);
                    }
                }
            }
        }

        //[HarmonyPatch(typeof(Inventory))]
        internal class Inventory_Patch
        {
            //[HarmonyPostfix]
            //[HarmonyPatch("Pickup")]
            static void PickupPostfix(Inventory __instance, Pickupable pickupable, bool __result)
            {
                if (__result && !Player.main.pda.isInUse)
                {
                    //AddDebug("Inventory Pickup " + pickupable.GetTechType() + " " + __result);
                    TechType tt = pickupable.GetTechType();
                    UnsavedData.pickedUpItems.AddValue(tt, 1);
                }
            }
        }

        [HarmonyPatch(typeof(BlueprintHandTarget), "UnlockBlueprint")]
        internal class BlueprintHandTarget_UnlockBlueprint_Patch
        {
            public static void Prefix(BlueprintHandTarget __instance)
            {
                if (!Main.config.modEnabled || string.IsNullOrEmpty(saveSlot) || __instance.used)
                    return;

                if (!KnownTech.Contains(__instance.unlockTechType))
                {
                    //AddDebug("unlock  " + __instance.unlockTechType);
                    UnsavedData.blueprintsFromDatabox.Add(__instance.unlockTechType);
                }
            }
        }

        [HarmonyPatch(typeof(ScannerTool), "Scan")]
        internal class ScannerTool_Scan_Patch
        {
            public static void Postfix(ScannerTool __instance, PDAScanner.Result __result)
            {
                if (!Main.config.modEnabled)
                    return;

                if (__result == PDAScanner.Result.None || __result == PDAScanner.Result.Scan || __result == PDAScanner.Result.Known) { }
                else
                {
                    //AddDebug("result " + __result + " IsFragment " + fragment);
                    UnsavedData.objectsScanned++;
                    TechType tt = PDAScanner.scanTarget.techType;
                    //string name = tt.AsString();
                    if (creatures.Contains(tt))
                    {
                        //AddDebug("scanned creature");
                        UnsavedData.faunaFound.Add(tt);
                    }
                    else if (flora.Contains(tt))
                    {
                        //AddDebug("scanned flora"); 
                        if (tt == TechType.SmallKoosh || tt == TechType.MediumKoosh || tt == TechType.LargeKoosh || tt == TechType.HugeKoosh)
                            UnsavedData.floraFound.Add(TechType.MediumKoosh);
                        else
                            UnsavedData.floraFound.Add(tt);
                    }
                    else if (coral.Contains(tt))
                    {
                        //AddDebug("scanned coral");
                        UnsavedData.coralFound.Add(tt);
                    }
                    else if (leviathans.Contains(tt))
                    {
                        //AddDebug("scanned leviathan");
                        if (tt == TechType.GhostLeviathan || tt == TechType.GhostLeviathanJuvenile)
                            UnsavedData.leviathanFound.Add(TechType.GhostLeviathan);
                        else
                            UnsavedData.leviathanFound.Add(tt);
                    }

                }

            }
        }

        [HarmonyPatch(typeof(PDAScanner), "Unlock")]
        internal class PDAScanner_Unlock_Patch
        {
            public static void Postfix(PDAScanner.EntryData entryData, bool unlockBlueprint, bool unlockEncyclopedia, bool verbose)
            {
                if (!Main.config.modEnabled || entryData == null || !verbose || !unlockBlueprint)
                    return;

                //AddDebug(" scanned " + entryData.key);
                //AddDebug("unlock Blueprint " + entryData.blueprint);
                TechType tt = entryData.blueprint;
                if (tt != TechType.None)
                { // bladderfish unlocks filteredWater blueprint
                    if (creatures.Contains(tt) || flora.Contains(tt) || coral.Contains(tt) || leviathans.Contains(tt))
                        return;

                    UnsavedData.blueprintsUnlocked.Add(tt);
                }
                //if (!PDAEncyclopedia.ContainsEntry(entryData.encyclopedia))
                //{
                //    AddDebug("unlock Encyclopedia " + entryData.encyclopedia);
                //}
                //if (!string.IsNullOrEmpty( entryData.encyclopedia))
                //    AddDebug("unlock Encyclopedia ");
            }
        }

        [HarmonyPatch(typeof(Base), "Start")]
        internal class Base_Start_Patch
        {
            public static void Postfix(Base __instance)
            {
                if (!Main.config.modEnabled || __instance.isGhost)
                    return;

                UnsavedData.bases.Add(__instance);
            }
        }

        [HarmonyPatch(typeof(Constructable))]
        internal class Constructable_Patch
        {
            //[HarmonyPrefix]
            //[HarmonyPatch("Construct")]
            public static void ConstructPrefix(Constructable __instance, bool __result)
            {
                //AddDebug(" Construct Prefix" + __instance.techType + " " + __instance.constructedAmount);
                if (__instance.techType == TechType.None || __instance.constructedAmount != 0)
                    return;

                //foreach (TechType tt in __instance.resourceMap)
                //{
                //    if (!itemMass.ContainsKey(tt))
                //        itemMass[tt] = GetItemMass(tt);
                //}
            }

            //[HarmonyPostfix]
            //[HarmonyPatch("Construct")]
            public static void ConstructPostfix(Constructable __instance, bool __result)
            {
                if (__instance.techType == TechType.None || __instance.constructedAmount < 1)
                    return;

                //AddDebug(" Construct " + __instance.techType);
                //AddDebug(" Construct resourceMap " + __instance.resourceMap.Count);
                //if (GameModeUtils.RequiresIngredients())
                //    SaveResourcesUsedToConstruct(__instance.resourceMap);
            }

            [HarmonyPostfix, HarmonyPatch("DeconstructAsync")]
            public static void DeconstructAsyncPostfix(Constructable __instance)
            {

                if (!Main.config.modEnabled)
                    return;

                if (__instance.constructedAmount <= 0f)
                {
                    //AddDebug(" deconstructed " + __instance.techType + " " + __instance.constructedAmount);
                    HandleDeconstruction(__instance.techType);
                }
            }

            private static void HandleDeconstruction(TechType techType)
            {
                if (UnsavedData.builderToolBuilt.ContainsKey(techType) && UnsavedData.builderToolBuilt[techType] > 0)
                {
                    UnsavedData.builderToolBuilt[techType]--;
                }
                else if (Main.config.builderToolBuilt.ContainsKey(saveSlot))
                {
                    string name = techType.AsString();
                    if (Main.config.builderToolBuilt[saveSlot].ContainsKey(name) && Main.config.builderToolBuilt[saveSlot][name] > 0)
                    {
                        Main.config.builderToolBuilt[saveSlot][name]--;
                        Main.config.Save();
                    }
                }

            }

        }

        [HarmonyPatch(typeof(SubRoot))]
        class SubRoot_Patch
        {
            //[HarmonyPostfix]
            //[HarmonyPatch("Start")]
            static void StartPostfix(SubRoot __instance)
            {
            }
            [HarmonyPostfix, HarmonyPatch("OnKill")]
            public static void OnKillPostfix(SubRoot __instance)
            {
                if (!Main.config.modEnabled)
                    return;
                //AddDebug("Sub lost");
                if (__instance.isCyclops)
                    UnsavedData.vehiclesLost.AddValue(TechType.Cyclops, 1);
            }
        }

        [HarmonyPatch(typeof(Vehicle))]
        internal class Vehicle_Patch
        {
            [HarmonyPostfix, HarmonyPatch("OnKill")]
            public static void OnKillPostfix(Vehicle __instance)
            {
                if (!Main.config.modEnabled)
                    return;

                if (__instance is SeaMoth)
                {
                    //AddDebug("SeaMoth lost" );
                    UnsavedData.vehiclesLost.AddValue(TechType.Seamoth, 1);
                }
                else if (__instance is Exosuit)
                {
                    //AddDebug("Exosuit lost");
                    UnsavedData.vehiclesLost.AddValue(TechType.Exosuit, 1);
                }
                else
                {
                    TechType tt = CraftData.GetTechType(__instance.gameObject);
                    if (tt != TechType.None)
                        UnsavedData.vehiclesLost.AddValue(tt, 1);
                }
            }
            [HarmonyPostfix, HarmonyPatch("EnterVehicle")]
            public static void EnterVehicleostfix(Vehicle __instance)
            {
                currentVehicleTT = CraftData.GetTechType(__instance.gameObject);
                //AddDebug("currentVehicleTT " + currentVehicleTT);
            }
        }

        [HarmonyPatch(typeof(ConstructorInput), "OnCraftingBegin")]
        internal class ConstructorInput_OnCraftingBegin_Patch
        {
            public static void Postfix(ConstructorInput __instance, TechType techType)
            {
                if (!Main.config.modEnabled)
                    return;

                constructorBuilt.Add(techType);
                //AddDebug("Constructor OnCraftingBegin " + techType);
            }
        }

        [HarmonyPatch(typeof(Bed))]
        internal class Bed_Patch
        {
            static TimeSpan bedTimeStart = TimeSpan.Zero;

            [HarmonyPostfix, HarmonyPatch("EnterInUseMode")]
            public static void EnterInUseModePostfix(Bed __instance)
            {
                if (!Main.config.modEnabled)
                    return;

                bedTimeStart = GetTimeSpanPlayed();
                //AddDebug("EnterInUseMode " );
            }
            [HarmonyPostfix, HarmonyPatch("ExitInUseMode")]
            public static void ExitInUseModePostfix(Bed __instance)
            {
                if (!Main.config.modEnabled)
                    return;

                TimeSpan timeSlept = GetTimeSpanPlayed() - bedTimeStart;
                UnsavedData.timeSlept += timeSlept;
                //AddDebug("ExitInUseMode " );
            }
        }

        [HarmonyPatch(typeof(LiveMixin), "TakeDamage")]
        class LiveMixin_TakeDamage_Patch
        {
            static void Postfix(LiveMixin __instance, DamageType type)
            {
                if (!Main.config.modEnabled)
                    return;

                if (__instance.gameObject.tag == "Player" && __instance.damageInfo.damage > 0)
                {
                    if (type == DamageType.Fire || type == DamageType.Heat)
                    {
                        float temp = WaterTemperatureSimulation.main.GetTemperature(__instance.transform.position);
                        //AddDebug("player fire damage " + (int)temp);
                        if (UnsavedData.maxTemp < temp)
                            UnsavedData.maxTemp = Mathf.RoundToInt(temp);
                    }
                    else if (type == DamageType.Cold)
                    {
                        float temp = WaterTemperatureSimulation.main.GetTemperature(__instance.transform.position);
                        //AddDebug("player Cold damage " + (int)temp);
                        if (UnsavedData.minTemp > temp)
                            UnsavedData.minTemp = Mathf.RoundToInt(temp);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(CreatureEgg), "Hatch")]
        internal class CreatureEgg_Hatch_Patch
        {
            public static void Postfix(CreatureEgg __instance)
            {
                if (!Main.config.modEnabled)
                    return;

                TechType tt = __instance.creatureType;
                if (tt == TechType.None)
                    return;

                //AddDebug("CreatureEgg Hatch  " + tt);
                hatchedTT = tt;
                UnsavedData.eggsHatched.AddValue(tt, 1);
            }
        }

        [HarmonyPatch(typeof(WaterParkCreature), "InitializeCreatureBornInWaterPark")]
        class GWaterParkCreature_InitializeCreatureBornInWaterPark_patch
        {
            public static void Postfix(WaterParkCreature __instance)
            {
                if (!Main.config.modEnabled)
                    return;

                if (hatchedTT != TechType.None)
                {
                    //AddDebug("InitializeCreatureBornInWaterPark hatched");
                    hatchedTT = TechType.None;
                    return;
                }
                TechType tt = CraftData.GetTechType(__instance.gameObject);
                if (tt == TechType.None)
                    return;

                UnsavedData.creaturesBred.AddValue(tt, 1);
                //AddDebug("InitializeCreatureBornInWaterPark  " + tt);
            }
        }

    }
}
