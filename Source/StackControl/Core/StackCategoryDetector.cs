using System.Collections.Generic;
using Verse;

namespace StackTuner
{
    public static class StackCategoryDetector
    {
        public static int GetCustomLimit(ThingDef thingDef, StackTunerSettings settings)
        {
            // Прямые проверки defName идут первыми для ванильных предметов,
            // у которых категории слишком широкие или неочевидные по названию в игре.
            if (thingDef.defName == "Silver")
            {
                return settings.customSilverLimit;
            }

            if (thingDef.defName == "Gold")
            {
                return settings.customGoldLimit;
            }

            if (thingDef.defName == "ComponentIndustrial" || thingDef.defName == "ComponentSpacer")
            {
                return settings.customResourcesLimit;
            }

            if (thingDef.defName == "Chocolate")
            {
                return settings.customMealsLimit;
            }

            if (IsBodyPart(thingDef))
            {
                return settings.customBodyPartsLimit;
            }

            if (IsSpecialSingleUseItem(thingDef))
            {
                return settings.customSpecialItemsLimit;
            }

            if (IsWithinAny(thingDef, "Medicine"))
            {
                return settings.customMedicineLimit;
            }

            if (IsWithinAny(thingDef, "Textiles", "Leathers"))
            {
                return settings.customTextilesLimit;
            }

            if (IsWithinAny(thingDef, "Drugs"))
            {
                return settings.customDrugsLimit;
            }

            if (IsWithinAny(thingDef, "MeatRaw"))
            {
                return settings.customMeatLimit;
            }

            if (IsWithinAny(thingDef, "PlantFoodRaw", "AnimalProductRaw", "FoodRaw"))
            {
                return settings.customRawFoodLimit;
            }

            if (IsWithinAny(thingDef, "FoodMeals", "Meals"))
            {
                return settings.customMealsLimit;
            }

            if (IsWithinAny(thingDef, "ResourcesRaw", "ResourcesManufactured", "StoneBlocks", "Manufactured",
                    "RawResources", "Resources"))
            {
                return settings.customResourcesLimit;
            }

            return settings.customOtherStackableLimit;
        }

        public static bool IsBodyPart(ThingDef thingDef)
        {
            // Здесь разрешены только явные категории или известные defName частей тела.
            return IsKnownVanillaBodyPartDef(thingDef)
                   || IsWithinAny(thingDef,
                       "BodyParts",
                       "BodyPartsNatural",
                       "BodyPartsArtificial",
                       "BodyPartsProsthetic",
                       "BodyPartsArchotech");
        }


        public static bool IsSpecialSingleUseItem(ThingDef thingDef)
        {
            // Здесь разрешены только известные одноразовые предметы и тренеры из ванили, Royalty и Biotech.
            string defName = thingDef.defName;

            if (StartsWithAny(defName,
                    "Neurotrainer",
                    "Skilltrainer",
                    "Psytrainer"))
            {
                return true;
            }

            switch (defName)
            {
                // Психические копья шока и безумия специально не добавлены в whitelist.
                // Они ведут себя как utility-предметы: пешки надевают весь стак,
                // а использование уничтожает надетый стак целиком.
                // =====================================================================
                
                case "PsychicAmplifier":
                case "PsylinkNeuroformer":
                case "PsychicAnimalPulser":
                case "PsychicSoothePulser":
                case "MechSerumHealer":
                case "MechSerumResurrector":
                    
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsKnownVanillaBodyPartDef(ThingDef thingDef)
        {
            switch (thingDef.defName)
            {
                case "Heart":
                case "Lung":
                case "Kidney":
                case "Liver":
                case "SimpleProstheticArm":
                case "SimpleProstheticLeg":
                case "SimpleProstheticHeart":
                case "BionicArm":
                case "BionicLeg":
                case "BionicEye":
                case "BionicEar":
                case "BionicHeart":
                case "BionicSpine":
                case "BionicStomach":
                case "ArchotechArm":
                case "ArchotechLeg":
                case "ArchotechEye":
                case "PowerClaw":
                case "DrillArm":
                case "FieldHand":
                case "HandTalon":
                case "AestheticShaper":
                case "AestheticNose":
                case "Coagulator":
                case "HealingEnhancer":
                case "Immunoenhancer":
                case "GastroAnalyzer":
                case "LearningAssistant":
                case "Neurocalculator":
                case "CircadianAssistant":
                case "CircadianHalfCycler":
                case "PsychicSensitizer":
                case "PsychicHarmonizer":
                case "PsychicReader":
                case "PsychicSilencer":
                case "Joywire":
                case "Painstopper":
                case "DeathAcidifier":
                case "Mindscrew":
                case "ToughskinGland":
                case "ArmorskinGland":
                case "StoneskinGland":
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsWithinAny(ThingDef thingDef, params string[] categoryDefNames)
        {
            if (thingDef.thingCategories == null)
            {
                return false;
            }

            HashSet<ThingCategoryDef> visited = new HashSet<ThingCategoryDef>();

            for (int i = 0; i < thingDef.thingCategories.Count; i++)
            {
                ThingCategoryDef categoryDef = thingDef.thingCategories[i];
                if (CategoryOrParentMatches(categoryDef, categoryDefNames, visited))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool CategoryOrParentMatches(ThingCategoryDef categoryDef, string[] categoryDefNames,
            HashSet<ThingCategoryDef> visited)
        {
            if (categoryDef == null || visited.Contains(categoryDef))
            {
                return false;
            }

            visited.Add(categoryDef);

            if (CategoryNameMatches(categoryDef, categoryDefNames))
            {
                return true;
            }

            foreach (ThingCategoryDef parentCategory in categoryDef.Parents)
            {
                if (CategoryOrParentMatches(parentCategory, categoryDefNames, visited))
                {
                    return true;
                }
            }

            return false;
        }


        private static bool StartsWithAny(string value, params string[] prefixes)
        {
            for (int i = 0; i < prefixes.Length; i++)
            {
                if (value.StartsWith(prefixes[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool CategoryNameMatches(ThingCategoryDef categoryDef, string[] categoryDefNames)
        {
            for (int i = 0; i < categoryDefNames.Length; i++)
            {
                if (categoryDef.defName == categoryDefNames[i])
                {
                    return true;
                }
            }

            return false;
        }
    }
}