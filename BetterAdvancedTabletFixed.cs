using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

#nullable disable
namespace BetterAdvancedTabletFixed
{
    [BepInPlugin("BetterAdvancedTabletFixed", "Better Advanced Tablet Fixed", "2.1.1")]
    public class BetterAdvancedTabletPlugin : BaseUnityPlugin
    {
        private ConfigEntry<int> configTabletSlots;
        private ConfigEntry<bool> configDebugMode;
        public static int TabletSlots;
        public static bool DebugMode;

        public static void ModLog(string text)
        {
            Debug.Log((object)("Better Advanced Tablet Fixed: " + text));
        }

        private void Awake()
        {
            this.HandleConfig();
            this.Patch();
        }

        private void Patch()
        {
            Debug.Log((object)"Plugin Better Advanced Tablet Fixed 2.1.1 is loaded!");
            new Harmony("BetterAdvancedTabletFixed").PatchAll();
            Debug.Log((object)"Better Advanced Tablet Patching complete!");
        }

        public void OnLoad() => this.Patch();

        public void OnUnload() => Debug.Log((object)"Better Advanced Tablet bye!");

        private void HandleConfig()
        {
            this.configTabletSlots = this.Config.Bind<int>("General", "AdvancedTabletSlots", 2, "Number of slots to add on the Advanced Tablet.\nVanilla has 2 already. You can add up to 6 extra slots for a total of 8 slots.\nCAUTION! Removing slots on a already created world with more slots will crash the game.");
            BetterAdvancedTabletPlugin.TabletSlots = this.configTabletSlots.Value;
            this.configDebugMode = this.Config.Bind<bool>("Debug", "DebugMode", false, "Turns debug mode");
            BetterAdvancedTabletPlugin.DebugMode = this.configDebugMode.Value;
        }
    }
}
