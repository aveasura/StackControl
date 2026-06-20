using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace StackTuner
{
    public class StackTunerMod : Mod
    {
        public static StackTunerSettings Settings;

        private Vector2 scrollPosition;
        private bool customBuffersInitialized;

        private string customResourcesBuffer;
        private string customSilverBuffer;
        private string customGoldBuffer;
        private string customMedicineBuffer;
        private string customBodyPartsBuffer;
        private string customSpecialItemsBuffer;
        private string customTextilesBuffer;
        private string customDrugsBuffer;
        private string customMeatBuffer;
        private string customRawFoodBuffer;
        private string customMealsBuffer;
        private string customOtherStackableBuffer;

        public StackTunerMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<StackTunerSettings>();
            Settings.Clamp();
            ResetCustomBuffersFromSettings();
        }

        public override string SettingsCategory()
        {
            return StackTunerText.T("StackTuner.SettingsCategory");
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.Clamp();
            EnsureCustomBuffersInitialized();

            Text.Font = GameFont.Small;

            float viewHeight = Settings.UsesCustomLimits() ? 745f : 430f;
            Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, viewHeight);
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            DrawIntro(listing);
            listing.GapLine();
            DrawPresetRow(listing);
            listing.Gap(6f);
            DrawWrappedLabel(listing, StackTunerText.T("StackTuner.PresetHint"));

            if (Settings.UsesCustomLimits())
            {
                listing.GapLine();
                DrawWrappedLabel(listing, StackTunerText.T("StackTuner.CustomHint"));
                listing.Gap(6f);
                DrawSectionHeader(listing, StackTunerText.T("StackTuner.CustomSection"));
                listing.Gap(4f);

                DrawIntRow(listing, StackTunerText.T("StackTuner.Category.Resources"),
                    ref Settings.customResourcesLimit, ref customResourcesBuffer);
                DrawIntRow(listing, StackTunerText.T("StackTuner.Category.Silver"), ref Settings.customSilverLimit,
                    ref customSilverBuffer);
                DrawIntRow(listing, StackTunerText.T("StackTuner.Category.Gold"), ref Settings.customGoldLimit,
                    ref customGoldBuffer);
                DrawIntRow(listing, StackTunerText.T("StackTuner.Category.Medicine"), ref Settings.customMedicineLimit,
                    ref customMedicineBuffer);
                DrawIntRow(listing, StackTunerText.T("StackTuner.Category.BodyParts"),
                    ref Settings.customBodyPartsLimit, ref customBodyPartsBuffer);
                DrawIntRow(listing, StackTunerText.T("StackTuner.Category.SpecialItems"),
                    ref Settings.customSpecialItemsLimit, ref customSpecialItemsBuffer);
                DrawIntRow(listing, StackTunerText.T("StackTuner.Category.Textiles"), ref Settings.customTextilesLimit,
                    ref customTextilesBuffer);
                DrawIntRow(listing, StackTunerText.T("StackTuner.Category.Drugs"), ref Settings.customDrugsLimit,
                    ref customDrugsBuffer);
                DrawIntRow(listing, StackTunerText.T("StackTuner.Category.Meat"), ref Settings.customMeatLimit,
                    ref customMeatBuffer);
                DrawIntRow(listing, StackTunerText.T("StackTuner.Category.RawFood"), ref Settings.customRawFoodLimit,
                    ref customRawFoodBuffer);
                DrawIntRow(listing, StackTunerText.T("StackTuner.Category.Meals"), ref Settings.customMealsLimit,
                    ref customMealsBuffer);
                DrawIntRow(listing, StackTunerText.T("StackTuner.Category.OtherStackable"),
                    ref Settings.customOtherStackableLimit, ref customOtherStackableBuffer);

                listing.GapLine();
                DrawCustomButtons(listing);
            }
            else
            {
                listing.GapLine();
                DrawPresetInfo(listing);
                listing.GapLine();
                DrawResetButton(listing);
            }

            listing.End();
            Widgets.EndScrollView();

            Settings.Clamp();
            Settings.Write();
        }

        private static void DrawIntro(Listing_Standard listing)
        {
            DrawWrappedLabel(listing, StackTunerText.T("StackTuner.Description"));
            listing.Gap(4f);
            DrawWrappedLabel(listing, StackTunerText.T("StackTuner.SingleStackHint"));
            listing.Gap(4f);
            DrawWrappedLabel(listing, StackTunerText.T("StackTuner.MultiplayerHint"));
        }


        private static void DrawWrappedLabel(Listing_Standard listing, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                listing.Gap(2f);
                return;
            }

            Text.Font = GameFont.Small;
            Rect rect = listing.GetRect(Text.CalcHeight(text, listing.ColumnWidth));
            Widgets.Label(rect, text);
        }

        private static void DrawPresetInfo(Listing_Standard listing)
        {
            DrawSectionHeader(listing, StackTunerText.T("StackTuner.PresetValuesTitle"));
            listing.Gap(4f);
            DrawWrappedLabel(listing, StackTunerText.T("StackTuner.PresetValues"));
        }

        private static void DrawSectionHeader(Listing_Standard listing, string label)
        {
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            Rect rect = listing.GetRect(28f);
            Widgets.Label(rect, label);
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private static void DrawPresetRow(Listing_Standard listing)
        {
            Rect row = listing.GetRect(32f);
            Rect labelRect = new Rect(row.x, row.y + 6f, row.width - 190f, row.height);
            Rect buttonRect = new Rect(row.xMax - 180f, row.y, 180f, row.height);

            Widgets.Label(labelRect, StackTunerText.T("StackTuner.Preset"));

            if (Widgets.ButtonText(buttonRect, PresetLabel(Settings.preset)))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>
                {
                    new FloatMenuOption(PresetLabel(StackTunerPreset.Small),
                        delegate { SetPreset(StackTunerPreset.Small); }),
                    new FloatMenuOption(PresetLabel(StackTunerPreset.Medium),
                        delegate { SetPreset(StackTunerPreset.Medium); }),
                    new FloatMenuOption(PresetLabel(StackTunerPreset.Large),
                        delegate { SetPreset(StackTunerPreset.Large); }),
                    new FloatMenuOption(PresetLabel(StackTunerPreset.Huge),
                        delegate { SetPreset(StackTunerPreset.Huge); }),
                    new FloatMenuOption(PresetLabel(StackTunerPreset.Custom),
                        delegate { SetPreset(StackTunerPreset.Custom); })
                };

                Find.WindowStack.Add(new FloatMenu(options));
            }
        }

        private static void SetPreset(StackTunerPreset value)
        {
            Settings.preset = value;
            Settings.Clamp();
            Settings.Write();
            StackLimitApplier.Apply(logResult: true);
        }

        private static string PresetLabel(StackTunerPreset preset)
        {
            switch (preset)
            {
                case StackTunerPreset.Small:
                    return StackTunerText.T("StackTuner.Preset.Small");
                case StackTunerPreset.Medium:
                    return StackTunerText.T("StackTuner.Preset.Medium");
                case StackTunerPreset.Large:
                    return StackTunerText.T("StackTuner.Preset.Large");
                case StackTunerPreset.Huge:
                    return StackTunerText.T("StackTuner.Preset.Huge");
                case StackTunerPreset.Custom:
                    return StackTunerText.T("StackTuner.Preset.Custom");
                default:
                    return StackTunerText.T("StackTuner.Preset.Medium");
            }
        }

        private static void DrawIntRow(Listing_Standard listing, string label, ref int value, ref string buffer)
        {
            Rect row = listing.GetRect(30f);
            Rect labelRect = new Rect(row.x, row.y + 5f, row.width - 150f, row.height);
            Rect fieldRect = new Rect(row.xMax - 140f, row.y, 140f, row.height);

            Widgets.Label(labelRect, label);
            buffer = Widgets.TextField(fieldRect, buffer);

            int parsed;
            if (int.TryParse(buffer, out parsed))
            {
                value = StackTunerSettings.ClampCustomLimit(parsed);
            }
        }

        private void DrawCustomButtons(Listing_Standard listing)
        {
            Rect buttonsRect = listing.GetRect(32f);
            float halfWidth = (buttonsRect.width - 12f) / 2f;
            Rect applyRect = new Rect(buttonsRect.x, buttonsRect.y, halfWidth, buttonsRect.height);
            Rect resetRect = new Rect(buttonsRect.x + halfWidth + 12f, buttonsRect.y, halfWidth, buttonsRect.height);

            if (Widgets.ButtonText(applyRect, StackTunerText.T("StackTuner.SaveAndApply")))
            {
                Settings.Clamp();
                ResetCustomBuffersFromSettings();
                Settings.Write();
                StackLimitApplier.Apply(logResult: true);
            }

            if (Widgets.ButtonText(resetRect, StackTunerText.T("StackTuner.ResetDefaults")))
            {
                Settings.ResetToDefaults();
                Settings.Write();
                ResetCustomBuffersFromSettings();
                StackLimitApplier.Apply(logResult: true);
            }
        }

        private void DrawResetButton(Listing_Standard listing)
        {
            Rect buttonRect = listing.GetRect(32f);
            if (Widgets.ButtonText(buttonRect, StackTunerText.T("StackTuner.ResetDefaults")))
            {
                Settings.ResetToDefaults();
                Settings.Write();
                ResetCustomBuffersFromSettings();
                StackLimitApplier.Apply(logResult: true);
            }
        }

        private void EnsureCustomBuffersInitialized()
        {
            if (!customBuffersInitialized)
            {
                ResetCustomBuffersFromSettings();
            }
        }

        private void ResetCustomBuffersFromSettings()
        {
            customResourcesBuffer = Settings.customResourcesLimit.ToString();
            customSilverBuffer = Settings.customSilverLimit.ToString();
            customGoldBuffer = Settings.customGoldLimit.ToString();
            customMedicineBuffer = Settings.customMedicineLimit.ToString();
            customBodyPartsBuffer = Settings.customBodyPartsLimit.ToString();
            customSpecialItemsBuffer = Settings.customSpecialItemsLimit.ToString();
            customTextilesBuffer = Settings.customTextilesLimit.ToString();
            customDrugsBuffer = Settings.customDrugsLimit.ToString();
            customMeatBuffer = Settings.customMeatLimit.ToString();
            customRawFoodBuffer = Settings.customRawFoodLimit.ToString();
            customMealsBuffer = Settings.customMealsLimit.ToString();
            customOtherStackableBuffer = Settings.customOtherStackableLimit.ToString();
            customBuffersInitialized = true;
        }
    }
}