using UnityEngine;
using UnityModManagerNet;

namespace adofaiOBS {
    public class MainSettings : UnityModManager.ModSettings, IDrawable {
        [Draw("")] public string OBSServer = "ws://localhost:4444";
        [Draw("")] public string Password = "password";
        [Draw("레벨 클리어 후 대기 초(Waiting seconds after level clear)", Min = 0)] public int ClearWaitTime = 3;
        [Draw("레벨 실패 후 대기 초(Waiting seconds after fail level)", Min = 0)] public int FailWaitTime = 2;
        [Draw("일시정지시 녹화 일시정지(Pause Recording on game pause)")] public bool PauseRecordingOnPause = true;
        [Draw("체크포인트에서 실패시 녹화 유지(Keep recording on checkpoint failure)")] public bool KeepRecordingOnCheckpointFailure = false;
        [Draw("중간에 시작 시 녹화하지 않기(Don't record if start from middle)")] public bool DontRecordStartFromMiddle = false;
        [Draw("튜토리얼 클리어 시 녹화 유지(Keep recording on tutorial clear)")] public bool KeepRecordingOnTutorialClear = true;
        [Draw("실패 후 효과 전 즉시 카운트다운(Countdown immediately after fail)")] public bool FailCountdownImmediately = false;
        [Draw("실패 시 녹화 파일 삭제(Delete recording file on fail)")] public bool DeleteRecordingOnFail = false;
        [Draw("게임 중 녹화가 되지 않고 있는지 확인(Check if recording is in game)")] public bool CheckRecordingInGame = true;

        public override void Save(UnityModManager.ModEntry modEntry) {
            UnityModManager.ModSettings.Save(this, modEntry);
        }
        
        public void OnChange() {
            
        }
        
        public void OnGUI(UnityModManager.ModEntry modEntry) {
            GUILayout.Label($"OBS 상태(OBS Status) : {(Main.OBSConnected ? "연결됨(Connected)" : "연결되지 않음(Not Connected)")}");
            
            GUILayout.Label("서버(Server)");
            OBSServer = GUILayout.TextField(OBSServer);
            
            GUILayout.Label("비밀번호(Password)");
            Password = GUILayout.TextField(Password);
            
            Main.Settings.Draw(modEntry);
        }

        public void OnSaveGUI(UnityModManager.ModEntry modEntry) {
            Main.Settings.Save(modEntry);

            Main.ConnectOBS();
        }
    }
}