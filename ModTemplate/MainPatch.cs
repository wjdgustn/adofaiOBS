using System;
using System.CodeDom;
using System.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace ModTemplate.MainPatch {
    // public class Text : MonoBehaviour {
    //     public static string Content = "Wa sans!";
    //     
    //     void OnGUI() {
    //         GUIStyle style = new GUIStyle();
    //         style.fontSize = (int) 50.0f;
    //         style.font = RDString.GetFontDataForLanguage(RDString.language).font;
    //         style.normal.textColor = Color.white;
    //
    //         GUI.Label(new Rect(10, -10, Screen.width, Screen.height), Content, style);
    //     }
    // }

    // [HarmonyPatch(typeof(scrCountdown), "ShowGetReady")]
    //
    // internal static class ResetGame {
    //     private static void Prefix(ADOBase __instance) {
    //         Main.Mod.Logger.Log("wa reset");
    //         Main._called = false;
    //     }
    // }
    //
    // [HarmonyPatch(typeof(scrCountdown), "Update")]
    //
    // internal static class StartGame {
    //     private static void Prefix(ADOBase __instance) {
    //         if (Main._called || !scrController.instance.goShown)
    //             return;
    //         Main._called = true;
    //         Main.Mod.Logger.Log("started game");
    //         __instance.conductor.song.pitch = (float) 0.5;
    //     }
    // }
    //
    // [HarmonyPatch(typeof(ADOBase), "isMobile", MethodType.Getter)]
    //
    // internal static class ForceMobile {
    //     private static bool Prefix(ref bool __result) {
    //         __result = true;
    //         return false;
    //     }
    // }
    //
    // [HarmonyPatch(typeof(PauseMenu), "ChangeLevel")]
    //
    // internal static class MiniSpeed {
    //     private static void Prefix(bool next) {
    //         if (!GCS.speedTrialMode) return;
    //         if (!Input.GetKey(KeyCode.LeftShift)) return;
    //         GCS.nextSpeedRun += next ? -0.09f : 0.09f;
    //     }
    // }
    //
    // [HarmonyPatch(typeof(scrController), "Update")]
    //
    // internal static class ChangeTitle {
    //     private static void Prefix(scrController __instance) {
    //         var str = RDString.Get("levelSelect.multiplier").Replace("[multiplier]", GCS.currentSpeedRun.ToString("0.00"));
    //         __instance.txtCaption.text = __instance.caption + " (" + str + ")";
    //     }
    // }
    //
    // [HarmonyPatch(typeof(PauseMenu), "UpdateLevelDescriptionAndReload")]
    //
    // internal static class ChangePauseMenuTitle {
    //     private static void Postfix(PauseMenu __instance) {
    //         if (!GCS.speedTrialMode) return;
    //
    //         var str1 = scrController.instance.caption;
    //         var str2 = RDString.Get("levelSelect.multiplier").Replace("[multiplier]", GCS.nextSpeedRun.ToString("0.00"));
    //         str1 = str1 + " (" + str2 + ")";
    //
    //         __instance.subtitle.text = str1;
    //     }
    // }
}