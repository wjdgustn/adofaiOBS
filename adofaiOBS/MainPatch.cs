using System;
using System.Threading.Tasks;
using HarmonyLib;
using MonsterLove.StateMachine;
using UnityEngine.SceneManagement;

namespace adofaiOBS.MainPatch {

    [HarmonyPatch(typeof(StateBehaviour), "ChangeState", typeof(Enum))]

    internal static class AutoRecord {
        private static async void Prefix(Enum newState) {
            Main.Mod.Logger.Log(newState.ToString());

            var state = (scrController.States) newState;

            if (state == scrController.States.Checkpoint || state == scrController.States.Countdown
                || (state == scrController.States.Start && SceneManager.GetActiveScene().name == "scnEditor" && RDC.auto)) {
                if (Main.Settings.DontRecordStartFromMiddle && GCS.checkpointNum > 0) return;
                if (Main.isRecording) {
                    Main.StopRecording();
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
                Main.StartRecording();
            }

            if (state == (Main.Settings.FailCountdownImmediately ? scrController.States.Fail : scrController.States.Fail2)) {
                if (GCS.checkpointNum > 0 && Main.Settings.KeepRecordingOnCheckpointFailure) return;
                
                await Task.Delay(TimeSpan.FromSeconds(Main.Settings.FailWaitTime));
                Main.StopRecording(true);
            }
        }
    }

    [HarmonyPatch(typeof(scrController), "OnLandOnPortal")]

    internal static class DetectWon {
        private static async void Prefix() {
            if (scrController.isGameWorld) {
                Main.Mod.Logger.Log("checking clear");
                if (GCS.standaloneLevelMode && GCS.customLevelIndex < GCS.customLevelPaths.Length - 1 
                    && Main.Settings.KeepRecordingOnTutorialClear) return;
                
                Main.Mod.Logger.Log("clear detected");

                await Task.Delay(TimeSpan.FromSeconds(Main.Settings.ClearWaitTime));
                Main.StopRecording();
            }
        }
    }

    [HarmonyPatch(typeof(scnEditor), "SwitchToEditMode")]

    internal static class DetectEditMode {
        private static void Prefix() {
            Main.StopRecording();
        }
    }
    
    [HarmonyPatch(typeof(scrController), "TogglePauseGame")]
    
    internal static class DetectPause {
        private static void Postfix(scrController __instance) {
            if (!Main.Settings.PauseRecordingOnPause) return;
            if (!Main.obs.IsConnected) return;
            if (!Main.isRecording) return;

            if(SceneManager.GetActiveScene().name == "scnEditor" && !GCS.standaloneLevelMode) return;

            if(__instance.paused) Main.obs.PauseRecording();
            else Main.obs.ResumeRecording();
        }
    }

    [HarmonyPatch(typeof(scrController), "QuitToMainMenu")]

    internal static class DetectQuit {
        private static void Prefix() {
            Main.StopRecording();
        }
    }
}