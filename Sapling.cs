using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Vintagestory.GameContent;

namespace resregrow2
{
    [HarmonyPatch(typeof(BlockEntitySapling), "CheckGrow")]
    public static class Sapling
    {
        public static float GetMultiplier()
        {
            return Resregrow2CoreSystem.Config?.ResinChanceMultiplier ?? 1.0f;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var log = Resregrow2CoreSystem.Logger;
            log?.Notification("[resregrow2] Transpiler running on BlockEntitySapling.CheckGrow");

            var getMultiplier = typeof(Sapling).GetMethod(nameof(GetMultiplier));
            var prevIndex = -1;
            var matches = 0;
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (prevIndex != -1)
                {
                    if (codes[i].opcode == OpCodes.Stfld)
                    {
                        var fieldInfo = codes[i].operand as FieldInfo;
                        if (fieldInfo != null && fieldInfo.Name == "otherBlockChance" && codes[prevIndex].opcode == OpCodes.Ldc_R4)
                        {
                            log?.Notification($"[resregrow2] Patching otherBlockChance at index {i} (was {codes[prevIndex].operand}, now calls GetMultiplier)");
                            codes[prevIndex] = new CodeInstruction(OpCodes.Call, getMultiplier);
                            matches++;
                        }
                    }
                }
                prevIndex = i;
            }

            log?.Notification($"[resregrow2] Transpiler done, {matches} match(es) patched");
            return codes.AsEnumerable();
        }
    }
}
