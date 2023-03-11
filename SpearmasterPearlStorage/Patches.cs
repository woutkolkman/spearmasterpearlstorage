using System;
using UnityEngine;
using System.Reflection;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using MonoMod.RuntimeDetour;
using System.Collections.Generic;

namespace SpearmasterPearlStorage
{
    class Patches
    {
        /*
         Thanks @oatmealine for the code example on Discord.
         RainWorld Discord: https://discord.gg/rainworld
         Modding Discord: https://discord.gg/bh8Jzwqes6
         */
        public static void Apply()
        {
            //allows spearmaster to put pearl in stomach
            IL.Player.GrabUpdate += PlayerGrabUpdateIL;
        }


        public static void Unapply()
        {
            //TODO
        }


        //allows spearmaster to put pearl in stomach
        static void PlayerGrabUpdateIL(ILContext il)
        {
            Plugin.ME.Logger_p.LogInfo("PlayerGrabUpdateIL start");
            ILCursor c = new ILCursor(il);

            try {
                for (int j = 0; j <= 1; j++)                                                            //skip the first match, because that's where speartail is updated, second match is where swallow is allowed
                    c.GotoNext(MoveType.After,                                                          //in C# code: this.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Spear
                        i => i.MatchLdarg(0),                                                           //ldarg.0
                        i => i.MatchLdfld<Player>("SlugCatClass"),                                      //ldfld     class SlugcatStats/Name Player::SlugCatClass
                        i => i.MatchLdsfld<MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName>("Spear"),   //ldsfld    class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Spear
                        i => i.Match(OpCodes.Call),                                                     //call      bool class ExtEnum`1<class SlugcatStats/Name>::op_Inequality(class ExtEnum`1<!0>, class ExtEnum`1<!0>)
                        i => i.Match(OpCodes.Brfalse_S) || i.Match(OpCodes.Brfalse)                     //brfalse.s 585 (0663) ldloc.2
                    );
            } catch (Exception ex) {
                Plugin.ME.Logger_p.LogInfo("PlayerGrabUpdateIL exception: " + ex.ToString());
            }

            //label to go to if condition succeeds
            ILLabel skipSpearCondition = c.MarkLabel();

            //go to start of sequence
            c.GotoPrev(MoveType.Before,
                i => i.MatchLdarg(0),
                i => i.MatchLdfld<Player>("SlugCatClass"),
                i => i.MatchLdsfld<MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName>("Spear")
            );

            //insert condition
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<Player, bool>>(self => {
                return self.FreeHand() == -1; //allow swallow only if player has no free hands, so spears can get created
            });

            // if it's true, skip spearmaster condition
            c.Emit(OpCodes.Brtrue_S, skipSpearCondition);

            Plugin.ME.Logger_p.LogInfo("PlayerGrabUpdateIL success");
        }
    }
}
