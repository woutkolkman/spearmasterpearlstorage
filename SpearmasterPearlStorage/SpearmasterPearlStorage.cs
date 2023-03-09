using BepInEx;
using System;
using System.Security.Permissions;
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace SpearmasterPearlStorage
{
    //also edit version in "modinfo.json"
    [BepInPlugin("maxi-mol.spearmasterpearlstorage", "Spearmaster Pearl Storage", "0.1.0")] //(GUID, mod name, mod version)
    public class SpearmasterPearlStorage : BaseUnityPlugin
    {
        //for accessing logger https://rainworldmodding.miraheze.org/wiki/Code_Environments
        private static WeakReference __me; //WeakReference still allows garbage collection
        public SpearmasterPearlStorage() { __me = new WeakReference(this); }
        public static SpearmasterPearlStorage ME => __me?.Target as SpearmasterPearlStorage;
        public BepInEx.Logging.ManualLogSource Logger_p => Logger;

        private static bool IsEnabled = false;


        //called when mod is loaded, subscribe functions to methods of the game
        public void OnEnable()
        {
            if (IsEnabled) return;
            IsEnabled = true;

            Enums.RegisterValues();
            Hooks.Apply();

            SpearmasterPearlStorage.ME.Logger_p.LogInfo("OnEnable called");
        }


        //called when mod is unloaded
        public void OnDisable()
        {
            if (!IsEnabled) return;
            IsEnabled = false;

            Enums.UnregisterValues();
            Hooks.Unapply();

            SpearmasterPearlStorage.ME.Logger_p.LogInfo("OnDisable called");
        }
    }
}
