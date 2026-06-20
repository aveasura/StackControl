using System.Collections.Generic;
using Verse;

namespace StackTuner
{
    public static class StackTunerText
    {
        private static readonly Dictionary<string, string> EnglishFallback = new Dictionary<string, string>
        {
            { "StackTuner.SettingsCategory", "StackControl" },
            { "StackTuner.Title", "StackControl" },
            { "StackTuner.Description", "Configure item stack sizes with simple presets or custom limits." },
            {
                "StackTuner.SingleStackHint",
                "StackControl changes normal stackable items, body parts, implants and a small whitelist of safe special items.\nWeapons, armor, apparel, corpses and quality-based items are left untouched."
            },
            {
                "StackTuner.MultiplayerHint",
                "Multiplayer note: all players should use the same mod version and the same settings."
            },
            { "StackTuner.Preset", "Stack size mode" },
            {
                "StackTuner.PresetHint",
                "Preset changes are saved and applied immediately.\nCustom values are applied with the Save and apply button."
            },
            { "StackTuner.PresetValuesTitle", "Preset values" },
            { "StackTuner.PresetValues", "Small: x10\nMedium: x50\nLarge: x100\nHuge: x500.\nCustom - manual." },
            {
                "StackTuner.CustomHint",
                "Custom mode uses exact maximum stack sizes for each category.\nPress Save and apply after editing numbers."
            },
            { "StackTuner.CustomSection", "Custom category limits" },
            { "StackTuner.SaveAndApply", "Save and apply" },
            { "StackTuner.ResetDefaults", "Restore defaults" },
            { "StackTuner.Preset.Small", "Small - x10" },
            { "StackTuner.Preset.Medium", "Medium - x50" },
            { "StackTuner.Preset.Large", "Large - x100" },
            { "StackTuner.Preset.Huge", "Huge - x500" },
            { "StackTuner.Preset.Custom", "Custom - manual" },
            { "StackTuner.Category.Resources", "Resources and materials" },
            { "StackTuner.Category.Silver", "Silver" },
            { "StackTuner.Category.Gold", "Gold" },
            { "StackTuner.Category.Medicine", "Medicine" },
            { "StackTuner.Category.BodyParts", "Body parts and implants" },
            { "StackTuner.Category.SpecialItems", "Safe special items" },
            { "StackTuner.Category.Textiles", "Textiles and leather" },
            { "StackTuner.Category.Drugs", "Drugs" },
            { "StackTuner.Category.Meat", "Raw meat" },
            { "StackTuner.Category.RawFood", "Raw food, except meat" },
            { "StackTuner.Category.Meals", "Meals and ready-to-eat food" },
            { "StackTuner.Category.OtherStackable", "Other stackable items" }
        };

        public static string T(string key)
        {
            if (Translator.CanTranslate(key))
            {
                return key.Translate().ToString();
            }

            string fallback;
            if (EnglishFallback.TryGetValue(key, out fallback))
            {
                return fallback;
            }

            return key;
        }
    }
}