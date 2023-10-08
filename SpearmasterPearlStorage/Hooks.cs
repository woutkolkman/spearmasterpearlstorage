using UnityEngine;
using RWCustom;

namespace SpearmasterPearlStorage
{
    class Hooks
    {
        public static void Apply()
        {
            //initialize options
            On.RainWorld.OnModsInit += RainWorldOnModsInitHook;

            //regurgitate the correct object
            On.Player.Regurgitate += PlayerRegurgitateHook;

            //makes Spearmaster be able to only swallow pearls
            On.Player.CanBeSwallowed += PlayerCanBeSwallowedHook;

            //overwrite parts of Spearmaster swallow animation
            On.PlayerGraphics.Update += PlayerGraphicsUpdateHook;

            //for stun option
            On.Player.SwallowObject += PlayerSwallowObjectHook;
        }


        public static void Unapply()
        {
            //TODO
        }


        //initialize options
        static void RainWorldOnModsInitHook(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            MachineConnector.SetRegisteredOI(Plugin.ME.GUID, new Options());
        }


        //regurgitate the correct object
        static void PlayerRegurgitateHook(On.Player.orig_Regurgitate orig, Player self)
        {
            //by default spearmaster has nothing in stomach, and creates a new SpearMasterPearl object when regurgitating
            //this hook allows objectInStomach to be regurgitated
            bool isSpearmaster = self.SlugCatClass == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear;

            //temporarily save objectInStomach (might be null)
            AbstractPhysicalObject temp = self.objectInStomach;

            //non-ideal solution, temporary set to Survivor if not stunned by Pebbles
            if (isSpearmaster && self.objectInStomach != null && !self.Stunned)
                self.SlugCatClass = SlugcatStats.Name.White;

            orig(self);

            if (!isSpearmaster)
                return;

            //place pearl in front of scar if not extracted by Pebbles
            if (!self.Stunned && temp?.realizedObject?.firstChunk != null && self.bodyChunks?.Length >= 2)
                temp.realizedObject.firstChunk.HardSetPosition(Vector2.Lerp(self.bodyChunks[0].pos, self.bodyChunks[1].pos, 0.5f));

            //restore objectInStomach if pearl is extracted by Pebbles
            if (self.Stunned)
                self.objectInStomach = temp;

            //reset to Spearmaster
            self.SlugCatClass = MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear;

            //stun option when not stunned by Pebbles
            if (Options.stun?.Value == true && !self.Stunned)
                self.Stun(40);
        }


        //makes Spearmaster be able to only swallow pearls
        static bool PlayerCanBeSwallowedHook(On.Player.orig_CanBeSwallowed orig, Player self, PhysicalObject testObj)
        {
            bool ret = orig(self, testObj);

            if (self.SlugCatClass != MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear)
                return ret;

            //check if pearl is removed when requirePearlRemoval option is true
            bool campaignAllowsSwallow = (
                Options.afterScarFade.Value && 
                (self.room?.game?.GetStorySession == null || //not campaign allows swallow
                (self.room?.game?.GetStorySession?.saveState?.miscWorldSaveData != null && 
                self.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad > 0))
            );
            campaignAllowsSwallow |= (
                self.graphicsModule is PlayerGraphics && 
                (self.graphicsModule as PlayerGraphics).bodyPearl != null && 
                (self.graphicsModule as PlayerGraphics).bodyPearl.scarVisible
            );
            if (Options.requirePearlRemoval.Value && !campaignAllowsSwallow)
                return ret;

            //Spearmaster may swallow any pearl
            ret |= (testObj is DataPearl);

            //Spearmaster may swallow any regular item
            ret |= (Options.anyRegularObject.Value && (
                testObj is Rock || testObj is DataPearl || testObj is FlareBomb || 
                testObj is Lantern || testObj is FirecrackerPlant || 
                (testObj is VultureGrub && !(testObj as VultureGrub).dead) || 
                (testObj is Hazer && !(testObj as Hazer).dead && !(testObj as Hazer).hasSprayed) || 
                testObj is FlyLure || testObj is ScavengerBomb || testObj is PuffBall || 
                testObj is SporePlant || testObj is BubbleGrass || testObj is SSOracleSwarmer || 
                testObj is NSHSwarmer || testObj is OverseerCarcass || 
                (ModManager.MSC && testObj is MoreSlugcats.FireEgg) || 
                (ModManager.MSC && testObj is MoreSlugcats.SingularityBomb && !(testObj as MoreSlugcats.SingularityBomb).activateSingularity && !(testObj as MoreSlugcats.SingularityBomb).activateSucktion)
            ));
            return ret;
        }


        //overwrite parts of Spearmaster swallow animation
        static void PlayerGraphicsUpdateHook(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
        {
            orig(self);

            //hands
            if (self.player.SlugCatClass == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear && 
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


        //for stun option
        static void PlayerSwallowObjectHook(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            bool wasNull = self.objectInStomach == null;

            orig(self, grasp);

            if (self.SlugCatClass != MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear)
                return;

            if (Options.stun?.Value == true && wasNull && self.objectInStomach != null)
                self.Stun(40);
        }
    }
}
