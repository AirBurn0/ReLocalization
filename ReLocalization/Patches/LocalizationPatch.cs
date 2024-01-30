using System;
using HarmonyLib;
using PotionCraft.LocalizationSystem;
using static PotionCraft.LocalizationSystem.LocalizationManager;

namespace ReLocalization.Patches
{

	[HarmonyPatch(typeof(LocalizationManager))]
	internal class LocalizationPatch
	{
		[HarmonyPatch(nameof(LocalizationManager.GetText), new Type[] {typeof(string), typeof(Locale)})]
		[HarmonyPrefix]
		public static bool GetText(string key, Locale locale, ref string __result)
		{
			if(GlobalConfigs.LocalizatorMode)
				return false;
			string value = Localization.GetLocalizedSilent(key, locale);
			if(value == null)
				return true;
			__result = value;
			return false;
		}
	}

}
