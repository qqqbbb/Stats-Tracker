
using HarmonyLib;
using QModManager.API.ModLoading;
using System.Reflection;
using System;
using SMLHelper.V2.Handlers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using System.Text;
using static ErrorMessage;

namespace Stats_Tracker
{
    class Testing
    {
        //[HarmonyPatch(typeof(Player), "Update")]
        class Player_Update_Patch
        {
            static void Postfix(Player __instance)
            {
                //AddDebug("currentSub " + Player.main.currentSub);
                //AddDebug("timePassedAsFloat " + DayNightCycle.main.timePassedAsFloat);
                //float movementSpeed = (float)System.Math.Round(__instance.movementSpeed * 10f) / 10f;
                if (Input.GetKey(KeyCode.B))
                {
                    //AddDebug("currentSlot " + Main.config.escapePodSmokeOut[SaveLoadManager.main.currentSlot]);
                    //if (Player.main.IsInBase())
                    //    AddDebug("IsInBase");
                    //else if (Player.main.IsInSubmarine())
                    //    AddDebug("IsInSubmarine");
                    //else if (Player.main.inExosuit)
                    //    AddDebug("GetInMechMode");
                    //else if (Player.main.inSeamoth)
                    //    AddDebug("inSeamoth");
                    int x = Mathf.RoundToInt(Player.main.transform.position.x);
                    int y = Mathf.RoundToInt(Player.main.transform.position.y);
                    int z = Mathf.RoundToInt(Player.main.transform.position.z);
                    AddDebug(x + " " + y + " " + z);
                    AddDebug("" + Player.main.GetBiomeString());
                    //Inventory.main.container.Resize(8,8);   GetPlayerBiome()
                    //HandReticle.main.SetInteractText(nameof(startingFood) + " " + dict[i]);
                }

                if (Input.GetKey(KeyCode.C))
                {
                    Survival survival = Player.main.GetComponent<Survival>();

                    if (Input.GetKey(KeyCode.LeftShift))
                        survival.water++;
                    else
                        survival.food++;
                }

                if (Input.GetKey(KeyCode.X))
                {
                    Survival survival = Player.main.GetComponent<Survival>();
                    if (Input.GetKey(KeyCode.LeftShift))
                        //survival.water--;
                        __instance.liveMixin.health--;
                    else
                        //survival.food--;
                        __instance.liveMixin.health++;
                }

                if (Input.GetKey(KeyCode.Z))
                {
                    //AddDebug(" " + LargeWorld.main.GetBiome(Player.main.transform.position));
                    //AddDebug(GameModeUtils.currentGameMode.ToString());
                    Targeting.GetTarget(Player.main.gameObject, 5f, out GameObject target, out float targetDist);
                    //if (GameModeUtils.currentGameMode == GameModeOption.Creative)
                    //    AddDebug("CREATIVE !!!");

                    //if (Main.guiHand.activeTarget)
                    //{
                    //    VFXSurface[] vFXSurfaces = __instance.GetAllComponentsInChildren<VFXSurface>();
                    //    if (vFXSurfaces.Length == 0)
                    //        AddDebug(" " + Main.guiHand.activeTarget.name + " no VFXSurface");
                    //    else
                    //        AddDebug(" " + Main.guiHand.activeTarget.name);

                    //    AddDebug("TechType " + CraftData.GetTechType(Main.guiHand.activeTarget));
                    //}
                    if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                    {
                    }
                    else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                    {
                    }

                    //else

                    //Inventory.main.DropHeldItem(true);
                    //Player.main.liveMixin.TakeDamage(99);
                    //Pickupable held = Inventory.main.GetHeld();
                    //AddDebug("isUnderwaterForSwimming " + Player.main.isUnderwaterForSwimming.value);
                    //AddDebug("isUnderwater " + Player.main.isUnderwater.value);
                    //LaserCutObject laserCutObject = 
                    //Inventory.main.quickSlots.Select(1);

                }
            }
        }

    }
}
