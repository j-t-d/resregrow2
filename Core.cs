using System;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace resregrow2
{
    class Resregrow2CoreSystem : ModSystem
    {
        public const string ConfigFilename = "resregrow2.json";
        public const float MinMultiplier = 0f;
        public const float MaxMultiplier = 100f;

        public static ILogger Logger;
        public static Config Config;

        private ICoreServerAPI sapi;

        public override void StartServerSide(ICoreServerAPI api)
        {
            sapi = api;
            Logger = Mod.Logger;
            Config = api.LoadModConfig<Config>(ConfigFilename) ?? new Config();
            Config.ResinChanceMultiplier = Math.Clamp(Config.ResinChanceMultiplier, MinMultiplier, MaxMultiplier);
            api.StoreModConfig(Config, ConfigFilename);
            Logger.Notification($"[resregrow2] ResinChanceMultiplier = {Config.ResinChanceMultiplier}");

            var harmony = new Harmony(Mod.Info.ModID);
            harmony.PatchAll();
            Logger.Notification("[resregrow2] Harmony patches applied");

            RegisterCommands(api);

            base.StartServerSide(api);
        }

        private void RegisterCommands(ICoreServerAPI api)
        {
            var parsers = api.ChatCommands.Parsers;
            api.ChatCommands.Create("resregrow2")
                .WithDescription("Configure Resin Regrow 2")
                .RequiresPrivilege(Privilege.controlserver)
                .HandleWith(_ => TextCommandResult.Success(
                    $"Resin chance multiplier: {Config.ResinChanceMultiplier} (use '/resregrow2 chance [{MinMultiplier}-{MaxMultiplier}]' to change)"))
                .BeginSubCommand("chance")
                    .WithDescription($"Set resin chance multiplier [{MinMultiplier}-{MaxMultiplier}]")
                    .WithArgs(parsers.Float("multiplier"))
                    .HandleWith(args =>
                    {
                        var requested = (float)args[0];
                        var clamped = Math.Clamp(requested, MinMultiplier, MaxMultiplier);
                        Config.ResinChanceMultiplier = clamped;
                        sapi.StoreModConfig(Config, ConfigFilename);
                        var msg = clamped == requested
                            ? $"Resin chance multiplier set to {clamped}"
                            : $"Resin chance multiplier clamped to {clamped} (allowed range: {MinMultiplier}-{MaxMultiplier})";
                        return TextCommandResult.Success(msg);
                    })
                .EndSubCommand();
        }

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Server;
        }
    }
}
