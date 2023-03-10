using UnityEngine;
using System.Reflection;
using MonoMod.RuntimeDetour;
using System.Collections.Generic;

namespace SpearmasterPearlStorage
{
    class Hooks
    {
        public static void Apply()
        {
            //allows spearmaster to put pearl in stomach
            On.Player.GrabUpdate += PlayerGrabUpdateHook;

            //regurgitate the correct object
            On.Player.Regurgitate += PlayerRegurgitateHook;

            //makes Spearmaster be able to swallow pearls
            On.Player.CanBeSwallowed += PlayerCanBeSwallowedHook;
        }


        public static void Unapply()
        {
            //TODO
        }


        //allows spearmaster to put pearl in stomach
        static void PlayerGrabUpdateHook(On.Player.orig_GrabUpdate orig, Player self, bool eu)
        {
            bool isSpearmaster = (self.SlugCatClass == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear);
            bool hasFreeHand = self.FreeHand() != -1; //no free hands so spears get created

            //check food in hand
            bool foodInHand = false;
            if (isSpearmaster && self.grasps != null && self.grasps.Length > 0)
                for (int i = 0; i < self.grasps.Length; i++)
                    if (self.grasps[i] != null && self.grasps[i].grabbed is IPlayerEdible)
                        foodInHand = true;

            //Plugin.ME.Logger_p.LogInfo("");

            orig(self, eu);


        }


        //regurgitate the correct object
        static void PlayerRegurgitateHook(On.Player.orig_Regurgitate orig, Player self)
        {
            //by default spearmaster has nothing in stomach, and creates a new SpearMasterPearl when regurgitating
            //this hook allows objectInStomach to be regurgitated
            bool isSpearmaster = (self.SlugCatClass == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear);

            //temporary set to Survivor
            if (isSpearmaster && self.objectInStomach != null)
                self.SlugCatClass = SlugcatStats.Name.White;

            orig(self);

            //reset to Spearmaster
            if (isSpearmaster)
                self.SlugCatClass = MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear;
        }


        //makes Spearmaster be able to swallow pearls
        static bool PlayerCanBeSwallowedHook(On.Player.orig_CanBeSwallowed orig, Player self, PhysicalObject testObj)
        {
            if (self.SlugCatClass == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear && testObj is DataPearl)
                return true;
            return orig(self, testObj);
        }
    }
}
