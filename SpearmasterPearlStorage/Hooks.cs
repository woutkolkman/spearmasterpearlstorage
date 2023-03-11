using System;
using UnityEngine;
using System.Reflection;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using MonoMod.RuntimeDetour;
using System.Collections.Generic;

namespace SpearmasterPearlStorage
{
    class Hooks
    {
        public static void Apply()
        {
            //regurgitate the correct object
            On.Player.Regurgitate += PlayerRegurgitateHook;

            //makes Spearmaster be able to only swallow pearls
            On.Player.CanBeSwallowed += PlayerCanBeSwallowedHook;
        }


        public static void Unapply()
        {
            //TODO
        }


        //regurgitate the correct object
        static void PlayerRegurgitateHook(On.Player.orig_Regurgitate orig, Player self)
        {
            //by default spearmaster has nothing in stomach, and creates a new SpearMasterPearl object when regurgitating
            //this hook allows objectInStomach to be regurgitated
            bool isSpearmaster = (self.SlugCatClass == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear);

            //temporarily save objectInStomach (might be null)
            AbstractPhysicalObject temp = self.objectInStomach;

            //non-ideal solution, temporary set to Survivor if not stunned by Pebbles
            if (isSpearmaster && self.objectInStomach != null && !self.Stunned)
                self.SlugCatClass = SlugcatStats.Name.White;

            orig(self);

            //restore objectInStomach if pearl is extracted by Pebbles
            if (isSpearmaster && self.Stunned)
                self.objectInStomach = temp;

            //reset to Spearmaster
            if (isSpearmaster)
                self.SlugCatClass = MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear;
        }


        //makes Spearmaster be able to only swallow pearls
        static bool PlayerCanBeSwallowedHook(On.Player.orig_CanBeSwallowed orig, Player self, PhysicalObject testObj)
        {
            if (self.SlugCatClass == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear && testObj is DataPearl)
                return true;
            return orig(self, testObj);
        }
    }
}
