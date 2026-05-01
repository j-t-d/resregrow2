using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace resregrow2
{
    class Resregrow2CoreSystem : ModSystem
    {
        public static ILogger Logger;

        public override void StartServerSide(ICoreServerAPI api)
        {
            Logger = Mod.Logger;
            Logger.Notification("[resregrow2] StartServerSide called, applying Harmony patches");
            var harmony = new Harmony(Mod.Info.ModID);
            harmony.PatchAll();
            Logger.Notification("[resregrow2] Harmony patches applied");
            base.StartServerSide(api);
        }

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Server;
        }
    }
}
