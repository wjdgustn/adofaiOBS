using System;
using System.Threading.Tasks;
using HarmonyLib;
using MonsterLove.StateMachine;
using OBSWebsocketDotNet;
using UnityEngine.SceneManagement;

namespace adofaiOBS.MainPatch {

    [HarmonyPatch(typeof(StateBehaviour), "ChangeState", typeof(Enum))]

    internal static class AutoRecord {
        private static async void Prefix(Enum newState) {
            Main.Mod.Logger.Log(newState.ToString());

            var state = (States) newState;
            Main.state = state;

            if (((state == States.Checkpoint || state == States.Countdown) && !RDC.auto)
                || (state == States.Start && SceneManager.GetActiveScene().name == "scnEditor" && RDC.auto)) {
                if (Main.Settings.DontRecordStartFromMiddle && GCS.checkpointNum > 0) return;
                if (Main.Settings.DontRecordAutoPlay && RDC.auto) return;
                if (Main.isRecording) {
                    Main.StopRecording();
                    // await Task.Delay(TimeSpan.FromSeconds(1));
                }
                Main.StartRecording();
            }

            if (state == (Main.Settings.FailCountdownImmediately ? States.Fail : States.Fail2)) {
                if (GCS.checkpointNum > 0 && Main.Settings.KeepRecordingOnCheckpointFailure) return;
                
                await Task.Delay(TimeSpan.FromSeconds(Main.Settings.FailWaitTime));
                if (Main.state != States.PlayerControl 
                    && Main.state != States.Countdown) Main.StopRecording(true);
            }
        }
    }

    [HarmonyPatch(typeof(scrController), "OnLandOnPortal")]

    internal static class DetectWon {
        private static async void Prefix() {
            if (scrController.isGameWorld) {
                Main.Mod.Logger.Log("checking clear");
                if (ADOBase.isScnGame && GCS.customLevelIndex < GCS.customLevelPaths.Length - 1 
                    && Main.Settings.KeepRecordingOnTutorialClear) return;
                
                Main.Mod.Logger.Log("clear detected");

                await Task.Delay(TimeSpan.FromSeconds(Main.Settings.ClearWaitTime));
                var currentState = scrController.instance.currentState;
                if (currentState != States.PlayerControl 
                    && currentState != States.Countdown) Main.StopRecording();
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
            if (!((OBSWebsocket) Main.obs).IsConnected) return;
            if (!Main.isRecording) return;

            if (SceneManager.GetActiveScene().name == "scnEditor" && !ADOBase.isScnGame) return;

            if (__instance.paused) ((OBSWebsocket) Main.obs).PauseRecording();
            else ((OBSWebsocket) Main.obs).ResumeRecording();
        }
    }

    [HarmonyPatch(typeof(scrController), "QuitToMainMenu")]

    internal static class DetectQuit {
        private static void Prefix() {
            Main.StopRecording();
        }
    }

    [HarmonyPatch(typeof(scnEditor), "Update")]

    internal static class CheckRecording {
        private static void Postfix() {
            if (!Main.Settings.CheckRecordingInGame) return;

            if (Main.Settings.DontRecordStartFromMiddle && GCS.checkpointNum > 0) return;
            if (Main.Settings.DontRecordAutoPlay && RDC.auto) return;
            if (Main.state != States.PlayerControl || scnEditor.instance.inStrictlyEditingMode) return;
            if (Main.isRecording) return;
            if (scrController.instance.currentSeqID == ADOBase.lm.listFloors.Count - 1) return;

            Main.Mod.Logger.Log("Recording force started!!!");
            Main.StartRecording();
        }
    }

    // [HarmonyPatch(typeof(scnEditor), "Awake")]
    //
    // internal static class AddEvent {
    //     private static bool EventAdded;
    //     
    //     private static void Prefix() {
    //         if (EventAdded) return;
    //
    //         var dontDeleteVideoEvent = new LevelEventInfo();
    //         dontDeleteVideoEvent.name = "DontDeleteVideo";
    //         dontDeleteVideoEvent.type = (LevelEventType) 100;
    //         dontDeleteVideoEvent.categories = new List<LevelEventCategory>();
    //         dontDeleteVideoEvent.executionTime = LevelEventExecutionTime.Special;
    //         
    //         dontDeleteVideoEvent.categories.Add(LevelEventCategory.Gameplay);
    //
    //         GCS.levelEventsInfo.Add(dontDeleteVideoEvent.name, dontDeleteVideoEvent);
    //
    //         var dontDeleteVideoTexture = new Texture2D(2, 2);
    //         var reader = new BinaryReader(new FileStream(Path.Combine(Main.Mod.Path, "DontDeleteVideo.png"), FileMode.Open));
    //         dontDeleteVideoTexture.LoadImage(reader.ReadBytes((int) reader.BaseStream.Length));
    //         var dontDeleteVideoIcon = Sprite.Create(dontDeleteVideoTexture, new Rect(0, 0, dontDeleteVideoTexture.width, dontDeleteVideoTexture.height), new Vector2(0.5f, 0.5f));
    //         
    //         GCS.levelEventIcons.Add((LevelEventType) 100, dontDeleteVideoIcon);
    //         
    //         EventAdded = true;
    //     }
    // }
}