using UnityEngine;
using UnityModManagerNet;

namespace ModTemplate {
    public class MainSettings : UnityModManager.ModSettings, IDrawable {
        [Draw("")] public string StringTest = "wa sans";
        [Draw("A")] public int A = 0;

        public override void Save(UnityModManager.ModEntry modEntry) {
            UnityModManager.ModSettings.Save(this, modEntry);
        }
        
        public void OnChange() {
            
        }
        
        public void OnGUI(UnityModManager.ModEntry modEntry) {
            GUILayout.Label("와 샌즈");
            StringTest = GUILayout.TextField(StringTest);
            
            Main.Settings.Draw(modEntry);
        }

        public void OnSaveGUI(UnityModManager.ModEntry modEntry) {
            Main.Settings.Save(modEntry);
        }
    }
}