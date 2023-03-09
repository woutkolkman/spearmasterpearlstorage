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
            On.Player.GrabUpdate += PlayerGrabUpdateHook;

            On.Player.Regurgitate += PlayerRegurgitateHook;

            On.Player.CanBeSwallowed += PlayerCanBeSwallowedHook;
        }


        public static void Unapply()
        {
            //TODO
        }


        //allows spearmaster to eat/put items in stomach
        static void PlayerGrabUpdateHook(On.Player.orig_GrabUpdate orig, Player self, bool eu)
        {
            bool isSpearmaster = (self.SlugCatClass == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear);
            bool hasFreeHand = self.FreeHand() != -1;

            //temporary set to Survivor
            if (isSpearmaster && !hasFreeHand)
                self.SlugCatClass = SlugcatStats.Name.White;

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
