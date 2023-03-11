using System;
using UnityEngine;
using RWCustom;
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

            //overwrite Spearmaster swallow animation
            On.PlayerGraphics.Update += PlayerGraphicsUpdateHook;
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

            //offset position for regurgitated object (in front of hole)
            if (isSpearmaster && !self.Stunned && self.standing && temp.realizedObject?.firstChunk != null)
                temp.realizedObject.firstChunk.pos += new Vector2(0,-10);

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


        //overwrite Spearmaster swallow animation
        static void PlayerGraphicsUpdateHook(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
        {
            orig(self);

            bool isSpearmaster = (self.player.SlugCatClass == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear);

            //hands
            if (isSpearmaster && 
                self.player.swallowAndRegurgitateCounter > 10 && 
                self.player.objectInStomach == null)
            {
                for (int i = 0; i < self.player.graphicsModule.bodyParts.Length; i++)
                {
                    if (!(self.player.graphicsModule.bodyParts[i] is SlugcatHand))
                        continue;
                    SlugcatHand hand = self.player.graphicsModule.bodyParts[i] as SlugcatHand;

                    //check which limb will swallow
                    int limbToSwallow = -1;
                    int j = 0;
                    while (limbToSwallow < 0 && j < 2) {
                        if (self.player.grasps[j] != null && self.player.CanBeSwallowed(self.player.grasps[j].grabbed))
                            limbToSwallow = j;
                        j++;
                    }

                    //offset hands downwards -10f (originally -4f)
                    if (limbToSwallow == hand.limbNumber) {
                        float progress = Mathf.InverseLerp(10f, 90f, (float)self.player.swallowAndRegurgitateCounter);
                        if (progress > 0.5f)
                            hand.relativeHuntPos = new Vector2(0f, -10f) + Custom.RNV() * 2f * UnityEngine.Random.value * Mathf.InverseLerp(0.5f, 1f, progress);
                    }
                }
            }
        }
    }
}
