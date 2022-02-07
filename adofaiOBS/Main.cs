﻿using System;
using System.Reflection;
using HarmonyLib;
using OBSWebsocketDotNet;
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

        internal static OBSWebsocket obs;

        internal static bool OBSConnected;

        internal static bool isRecording {
            get {
                if(!obs.IsConnected) return false;
                
                var streamStatus = obs.GetStreamingStatus();
                return streamStatus.IsRecording;
            }
        }

        internal static void StartRecording() {
            if (!obs.IsConnected) return;
            if (isRecording) return;
            obs.StartRecording();
        }

        internal static void StopRecording() {
            if (!obs.IsConnected) return;
            if (!isRecording) return;
            obs.StopRecording();
        }

        private static void Load(UnityModManager.ModEntry modEntry) {
            Mod = modEntry;
            Mod.OnToggle = OnToggle;
            Settings = UnityModManager.ModSettings.Load<MainSettings>(modEntry);
            Mod.OnGUI = Settings.OnGUI;
            Mod.OnSaveGUI = Settings.OnSaveGUI;
            
            #if DEBUG
            Mod.OnUnload = Stop;
            #endif
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
            IsEnabled = value;

            if (value) Start();
            else Stop(modEntry);

            return true;
        }

        internal static void ConnectOBS() {
            try {
                obs.Connect(Settings.OBSServer, Settings.Password);
            }
            catch {
                // ignored
            }
        }

        private static void Start() {
            _harmony = new Harmony(Mod.Info.Id);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());

            obs = new OBSWebsocket();

            obs.Connected += onOBSConnect;
            obs.Disconnected += onOBSDisconnect;

            ConnectOBS();
        }

        private static bool Stop(UnityModManager.ModEntry modEntry) {
            _harmony.UnpatchAll(Mod.Info.Id);
            #if RELEASE
            _harmony = null;
            #endif

            if(obs.IsConnected) obs.Disconnect();

            return true;
        }

        private static void onOBSConnect(object sender, EventArgs e) {
            OBSConnected = true;
        }
        
        private static void onOBSDisconnect(object sender, EventArgs e) {
            OBSConnected = false;
        }
    }
}