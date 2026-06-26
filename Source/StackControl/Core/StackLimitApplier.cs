using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace StackTuner
{
    public static class StackLimitApplier
    {
        private const bool EnableApplyLogs = false;

        private static readonly Dictionary<string, int> OriginalLimits = new Dictionary<string, int>();

        public static void Apply(bool logResult = false)
        {
            StackTunerSettings settings = StackTunerMod.Settings;
            if (settings == null)
            {
                return;
            }

            settings.Clamp();

            bool shouldLog = EnableApplyLogs && logResult;

            int changed = 0;
            int scanned = 0;
            int skippedSingleStack = 0;
            int allowedBodyParts = 0;
            int allowedSpecialItems = 0;

            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (thingDef == null || thingDef.category != ThingCategory.Item)
                {
                    continue;
                }

                if (shouldLog)
                {
                    scanned++;
                }

                int originalLimit = GetOriginalLimit(thingDef);
                bool isBodyPart = StackCategoryDetector.IsBodyPart(thingDef);
                bool isSpecialItem = StackCategoryDetector.IsSpecialSingleUseItem(thingDef);

                if (originalLimit <= 1 && !isBodyPart && !isSpecialItem)
                {
                    if (shouldLog)
                    {
                        skippedSingleStack++;
                    }

                    continue;
                }

                if (shouldLog && originalLimit <= 1 && isBodyPart)
                {
                    allowedBodyParts++;
                }

                if (shouldLog && originalLimit <= 1 && isSpecialItem)
                {
                    allowedSpecialItems++;
                }

                int newLimit = settings.UsesCustomLimits()
                    ? StackCategoryDetector.GetCustomLimit(thingDef, settings)
                    : CalculateMultiplierLimit(originalLimit, settings.MultiplierForPreset());

                newLimit = Mathf.Max(originalLimit, StackTunerSettings.ClampCustomLimit(newLimit));

                if (thingDef.stackLimit != newLimit)
                {
                    thingDef.stackLimit = newLimit;

                    if (shouldLog)
                    {
                        changed++;
                    }
                }
            }

            if (shouldLog)
            {
                Log.Message("[StackTuner] Applied stack limits. Scanned: " + scanned + ", changed: " + changed +
                            ", skipped single-stack items: " + skippedSingleStack + ", allowed body parts: " +
                            allowedBodyParts + ", allowed special items: " + allowedSpecialItems + ".");
            }
        }

        private static int CalculateMultiplierLimit(int originalLimit, int multiplier)
        {
            long newLimit = (long)originalLimit * multiplier;
            if (newLimit > int.MaxValue)
            {
                return int.MaxValue;
            }

            return (int)newLimit;
        }

        private static int GetOriginalLimit(ThingDef thingDef)
        {
            int originalLimit;
            if (OriginalLimits.TryGetValue(thingDef.defName, out originalLimit))
            {
                return originalLimit;
            }

            originalLimit = thingDef.stackLimit;
            OriginalLimits[thingDef.defName] = originalLimit;
            return originalLimit;
        }
    }
}