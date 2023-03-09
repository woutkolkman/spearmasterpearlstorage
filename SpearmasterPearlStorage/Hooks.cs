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

            //changes return value CanBeSwallowed for Spearmaster to TRUE
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

            if (isSpearmaster && self.grasps != null && self.grasps.Length > 0)
            {
                //check food in hand
                bool foodInHand = false;
                for (int i = 0; i < self.grasps.Length; i++)
                    if (self.grasps[i] != null && self.grasps[i].grabbed is IPlayerEdible)
                        foodInHand = true;

                //datapearl must be first to be swallowed
                if (self.grasps.Length >= 2 && !(self.grasps[0]?.grabbed is DataPearl) && self.grasps[1]?.grabbed is DataPearl)
                    if (self.switchHandsProcess == 0f && self.switchHandsCounter == 0)
                        self.switchHandsCounter = 15;

                //get in/out of stomach
                if (!hasFreeHand && !foodInHand && (self.grasps[0]?.grabbed is DataPearl || self.objectInStomach != null))
                    self.SlugCatClass = SlugcatStats.Name.White; //temporary set to Survivor
            }

            orig(self, eu);

            //reset to Spearmaster
            if (isSpearmaster)
                self.SlugCatClass = MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear;
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


        //changes return value CanBeSwallowed for Spearmaster to TRUE
        static bool PlayerCanBeSwallowedHook(On.Player.orig_CanBeSwallowed orig, Player self, PhysicalObject testObj)
        {
            bool isSpearmaster = (self.SlugCatClass == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear);

            //temporary set to Survivor
            if (isSpearmaster)
                self.SlugCatClass = SlugcatStats.Name.White;

            bool ret = orig(self, testObj);

            //reset to Spearmaster
            if (isSpearmaster)
                self.SlugCatClass = MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear;

            return ret;
        }
    }
}
