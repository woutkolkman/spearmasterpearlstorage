using UnityEngine;
using System.Reflection;
using MonoMod.RuntimeDetour;
using System.Collections.Generic;

namespace SpearmasterPearlStorage
{
    class Hooks
    {
        static BindingFlags propFlags = BindingFlags.Instance | BindingFlags.Public;
        static BindingFlags myMethodFlags = BindingFlags.Static | BindingFlags.Public;


        public static void Apply()
        {
            //selects room to place GameController type
            //On.Room.Loaded += RoomLoadedHook;

            //Moon OracleGetToPos RuntimeDetour
            /*Hook SLOracleBehaviorHasMarkOracleGetToPosHook = new Hook(
                typeof(SLOracleBehaviorHasMark).GetProperty("OracleGetToPos", propFlags).GetGetMethod(),
                typeof(Hooks).GetMethod("SLOracleBehaviorHasMark_OracleGetToPos_get", myMethodFlags)
            );*/
        }


        public static void Unapply()
        {
            //TODO
        }


        //creates GameController object
        /*static void AbstractPhysicalObjectRealizeHook(On.AbstractPhysicalObject.orig_Realize orig, AbstractPhysicalObject self)
        {
            orig(self);
        }*/
    }
}
