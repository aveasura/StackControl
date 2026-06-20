using Verse;

namespace StackTuner
{
    public class StackTunerSettings : ModSettings
    {
        public StackTunerPreset preset = StackTunerPreset.Medium;

        public int customResourcesLimit = 5000;
        public int customSilverLimit = 25000;
        public int customGoldLimit = 25000;
        public int customMedicineLimit = 500;
        public int customBodyPartsLimit = 50;
        public int customSpecialItemsLimit = 50;
        public int customTextilesLimit = 5000;
        public int customDrugsLimit = 500;
        public int customMeatLimit = 5000;
        public int customRawFoodLimit = 5000;
        public int customMealsLimit = 500;
        public int customOtherStackableLimit = 5000;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref preset, "preset", StackTunerPreset.Medium);

            Scribe_Values.Look(ref customResourcesLimit, "customResourcesLimit", 5000);
            Scribe_Values.Look(ref customSilverLimit, "customSilverLimit", 25000);
            Scribe_Values.Look(ref customGoldLimit, "customGoldLimit", 25000);
            Scribe_Values.Look(ref customMedicineLimit, "customMedicineLimit", 500);
            Scribe_Values.Look(ref customBodyPartsLimit, "customBodyPartsLimit", 50);
            Scribe_Values.Look(ref customSpecialItemsLimit, "customSpecialItemsLimit", 50);
            Scribe_Values.Look(ref customTextilesLimit, "customTextilesLimit", 5000);
            Scribe_Values.Look(ref customDrugsLimit, "customDrugsLimit", 500);
            Scribe_Values.Look(ref customMeatLimit, "customMeatLimit", 5000);
            Scribe_Values.Look(ref customRawFoodLimit, "customRawFoodLimit", 5000);
            Scribe_Values.Look(ref customMealsLimit, "customMealsLimit", 500);
            Scribe_Values.Look(ref customOtherStackableLimit, "customOtherStackableLimit", 5000);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Clamp();
            }
        }

        public void ResetToDefaults()
        {
            preset = StackTunerPreset.Medium;

            customResourcesLimit = 5000;
            customSilverLimit = 25000;
            customGoldLimit = 25000;
            customMedicineLimit = 500;
            customBodyPartsLimit = 50;
            customSpecialItemsLimit = 50;
            customTextilesLimit = 5000;
            customDrugsLimit = 500;
            customMeatLimit = 5000;
            customRawFoodLimit = 5000;
            customMealsLimit = 500;
            customOtherStackableLimit = 5000;
        }

        public void Clamp()
        {
            customResourcesLimit = ClampCustomLimit(customResourcesLimit);
            customSilverLimit = ClampCustomLimit(customSilverLimit);
            customGoldLimit = ClampCustomLimit(customGoldLimit);
            customMedicineLimit = ClampCustomLimit(customMedicineLimit);
            customBodyPartsLimit = ClampCustomLimit(customBodyPartsLimit);
            customSpecialItemsLimit = ClampCustomLimit(customSpecialItemsLimit);
            customTextilesLimit = ClampCustomLimit(customTextilesLimit);
            customDrugsLimit = ClampCustomLimit(customDrugsLimit);
            customMeatLimit = ClampCustomLimit(customMeatLimit);
            customRawFoodLimit = ClampCustomLimit(customRawFoodLimit);
            customMealsLimit = ClampCustomLimit(customMealsLimit);
            customOtherStackableLimit = ClampCustomLimit(customOtherStackableLimit);
        }

        public int MultiplierForPreset()
        {
            switch (preset)
            {
                case StackTunerPreset.Small:
                    return 10;
                case StackTunerPreset.Medium:
                    return 50;
                case StackTunerPreset.Large:
                    return 100;
                case StackTunerPreset.Huge:
                    return 500;
                default:
                    return 1;
            }
        }

        public bool UsesCustomLimits()
        {
            return preset == StackTunerPreset.Custom;
        }

        public static int ClampCustomLimit(int value)
        {
            if (value < 2)
            {
                return 2;
            }

            if (value > 1000000)
            {
                return 1000000;
            }

            return value;
        }
    }
}