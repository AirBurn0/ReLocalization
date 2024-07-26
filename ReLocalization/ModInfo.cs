using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ReLocalization.Patches;

namespace ReLocalization
{
	[BepInPlugin(GUID, MODNAME, VERSION)]
	[BepInProcess("Potion Craft.exe")]
	public class ModInfo : BaseUnityPlugin
	{
		private static new ManualLogSource Logger;
		public const string GUID = "AirBurn.ReLocalization";
		public const string MODNAME = "ReLocalization";
		public const string VERSION = "1.1.0.0";

		public void Awake()
		{
			Logger = base.Logger;
			GlobalConfigs.LazyLoad = Config.Bind(
				"General",
				"LazyLoad",
				false,
				"Should all localization be lazy-loaded or loaded on startup."
			)
			.Value;
			GlobalConfigs.LogLoad = Config.Bind(
				"General",
				"LogLoad",
				false,
				"Show logs about the presence and absence of localization files. Disable LazyLoad to debug more easly."
			)
			.Value;
			GlobalConfigs.LocalizatorMode = Config.Bind(
				"General",
				"LocalizatorMode",
				false,
				"Localization keys will be displayed instead of localized text."
			)
			.Value;
			Localization.AddLocalizationFor(this);
			Harmony.CreateAndPatchAll(typeof(LocalizationPatch));
			Logger.LogInfo($"{MODNAME} succesfully loaded");
		}

		public static void Log(object message, LogLevel level = LogLevel.Info)
		{
			Logger.Log(level, message);
		}
	}
}
