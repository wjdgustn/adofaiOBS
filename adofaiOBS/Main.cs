using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using UnityModManagerNet;

namespace adofaiOBS {
    #if DEBUG
    [EnableReloading]
    #endif

    internal static class Main {
        internal static UnityModManager.ModEntry Mod;
        private static Harmony _harmony;
        internal static bool IsEnabled { get; private set; }
        internal static MainSettings Settings { get; private set; }

        internal static object obs;

        internal static bool OBSConnected;

        private static string recordingFile;

        internal static scrController.States state;

        internal static bool isRecording {
            get {
                if(!((OBSWebsocket) obs).IsConnected) return false;
                
                var streamStatus = ((OBSWebsocket) obs).GetStreamingStatus();
                return streamStatus.IsRecording;
            }
        }

        internal static void StartRecording() {
            if (!((OBSWebsocket) obs).IsConnected) return;
            if (isRecording) return;
            ((OBSWebsocket) obs).StartRecording();
        }

        internal static void StopRecording(bool deleteFile = false) {
            if (!((OBSWebsocket) obs).IsConnected) return;
            if (!isRecording) return;

            if (Settings.DeleteRecordingOnFail && deleteFile) recordingFile = ((OBSWebsocket) obs).GetRecordingStatus().RecordingFilename;
            
            ((OBSWebsocket) obs).StopRecording();
        }

        private static void Load(UnityModManager.ModEntry modEntry) {
            LoadAssembly("Mods/adofaiOBS/netstandard.dll");
            LoadAssembly("Mods/adofaiOBS/Newtonsoft.Json.dll");
            LoadAssembly("Mods/adofaiOBS/websocket-sharp.dll");
            LoadAssembly("Mods/adofaiOBS/obs-websocket-dotnet.dll");

            Mod = modEntry;
            Mod.OnToggle = OnToggle;
            Settings = UnityModManager.ModSettings.Load<MainSettings>(modEntry);
            Mod.OnGUI = Settings.OnGUI;
            Mod.OnSaveGUI = Settings.OnSaveGUI;
            
            #if DEBUG
            Mod.OnUnload = Stop;
            #endif
        }
        
        private static void LoadAssembly(string path) {
            using (FileStream stream = new FileStream(path, FileMode.Open)) {
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                AppDomain.CurrentDomain.Load(data);
            }
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
            IsEnabled = value;

            if (value) Start();
            else Stop(modEntry);

            return true;
        }

        internal static void ConnectOBS() {
            try {
                Task.Run(() => ((OBSWebsocket) obs).Connect(Settings.OBSServer, Settings.Password));
            }
            catch {
                // ignored
            }
        }

        private static void Start() {
            _harmony = new Harmony(Mod.Info.Id);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());

            obs = new OBSWebsocket();

            ((OBSWebsocket) obs).Connected += onOBSConnect;
            ((OBSWebsocket) obs).Disconnected += onOBSDisconnect;
            ((OBSWebsocket) obs).RecordingStateChanged += onOBSRecordingStateChanged;

            ConnectOBS();
        }

        private static bool Stop(UnityModManager.ModEntry modEntry) {
            _harmony.UnpatchAll(Mod.Info.Id);
            #if RELEASE
            _harmony = null;
            #endif

            if(((OBSWebsocket) obs).IsConnected) ((OBSWebsocket) obs).Disconnect();

            return true;
        }

        private static void onOBSConnect(object sender, EventArgs e) {
            OBSConnected = true;
        }
        
        private static void onOBSDisconnect(object sender, EventArgs e) {
            OBSConnected = false;
        }
        
        private static void onOBSRecordingStateChanged(OBSWebsocket sender, OutputState state) {
            if (state == OutputState.Stopped) {
                if(Settings.DeleteRecordingOnFail && recordingFile != null) {
                    File.Delete(recordingFile);
                    recordingFile = null;
                }
            }
        }
    }
}