using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using YamlDotNet.RepresentationModel;
using static PotionCraft.LocalizationSystem.LocalizationManager;
using static UnityEngine.Scripting.GarbageCollector;

namespace ReLocalization
{
    public static class Localization
    {
        private static readonly Dictionary<string, bool> localizations = new Dictionary<string, bool>();
        private static readonly Dictionary<Locale, LocalizationEntry> localizationData = new Dictionary<Locale, LocalizationEntry>();
        private static readonly string locFolderName = "Localization";
        private static readonly Dictionary<string, string> localizationFolders = new Dictionary<string, string>();

        public static string modLocalizationFolder(string modid) => localizationFolders[modid];

        /// <summary>
        ///     Collection of all plugins/mods that uses ReLocalization localization features;
        /// </summary>
        /// <returns>Collection of plugins/mods GUID's</returns>
        public static ReadOnlyCollection<string> Mods()
        {
            return localizations.Keys.ToList().AsReadOnly();
        }

        /// <summary>
        ///     Adding this plugin/mod in localizaton list.
        ///     This will allow localization keys for plugin/mod to be translated in the future.
        ///     If LazyLoad flag in config set to 'false', also will force-load localization in memory.
        /// </summary>
        /// <param name="mod">The plugin/mod that will be added in localization list.</param>
        public static void AddLocalizationFor(BaseUnityPlugin mod)
        {
            string modid = MetadataHelper.GetMetadata(mod).GUID;
            string modPath = Path.GetDirectoryName(Assembly.GetAssembly(mod.GetType()).Location);
			string localizationsFolder = Path.Combine(modPath, modid, locFolderName);


            localizationFolders.Add(modid, localizationsFolder);
			localizations.Add(modid, false);
			if (!GlobalConfigs.LazyLoad)
				LoadLocalizationFor(modid);
		}

        /// <summary>
        ///     Force-Loads localization in memory for specified plugin/mod.
        ///     This will also add this plugin/mod in localizaton list.
        /// </summary>
        /// <param name="modid">GUID of the plugin/mod whose localization will be force-loaded.</param>
        internal static void LoadLocalizationFor(string modid)
        {
            foreach (Locale locale in Enum.GetValues(typeof(Locale)))
            {
                LoadLocalizationFor(modid, locale);
            }
        }

        /// <summary>
        ///     Fully Loads localization in memory for all plugins/mods specified by <see cref="LocalizationHelper.AddLocalizationFor"/> method.
        ///     Do not use this method without serious reasons.
        /// </summary>
        /// <param name="reload">If set true, all localization will be reloaded from disk.</param>
        public static void LoadAllLocalization(bool reload)
        {
            // BTW, if you still decide to use this method, it means something isn’t working well for you.
            // Consider using github to report/improve that issue!
            foreach (Locale locale in Enum.GetValues(typeof(Locale)))
            {
                LoadLocalization(locale, reload);
            }
        }

        /// <summary>
        ///     Tries to get translated text based on specified localization key for currently selected language.
        /// </summary>
        /// <param name="key">Localization key.</param>
        /// <returns>
        ///     Translated text if it exists for currently selected language;
        ///     Else translated text for default (en) language;
        ///     Else localization key.
        /// </returns>
        public static string GetLocalized(string key)
        {
            return GetLocalized(key, CurrentLocale);
        }

        /// <summary>
        ///     Tries to get translated text based on specified localization key for given language.
        /// </summary>
        /// <param name="key">Localization key.</param>
        /// <param name="locale">Localization language.</param>
        /// <returns>
        ///     Translated text if it exists for given language;
        ///     Else translated text for default (en) language;
        ///     Else localization key.
        /// </returns>
        public static string GetLocalized(string key, Locale locale)
        {
            string value = GetLocalizedSilent(key, locale);
            if (value == null)
            {
                ModInfo.Log((locale == Locale.en ? "D" : $"Neither '{locale}.yml' nor d") + $"efault localization file doesn't contain valid '{key}' key.", BepInEx.Logging.LogLevel.Error);
                return key;
            }
            return value;
        }

        internal static void LoadLocalization(Locale locale, bool force)
        {
            foreach (var mod in localizations.Keys.Where(mod => force || !localizations[mod]).ToArray())
            {
                if (localizations[mod])
                    localizations[mod] = false; // for the case that something fails on reload
                LoadLocalizationFor(mod, locale);
            }
        }

        internal static void LoadLocalizationFor(string modid, Locale locale)
        {
            string path = Path.Combine(modLocalizationFolder(modid), locale + ".yml");
            if (!File.Exists(path))
            {
                if (GlobalConfigs.LogLoad && modid != ModInfo.GUID) // It's okay. I can stand it.
                    ModInfo.Log($"'{locale}.yml' localization file doesn't found for mod '{modid}'. Please consider helping mod author to provide translation.", BepInEx.Logging.LogLevel.Warning);
                return;
            }
            if(GlobalConfigs.LogLoad)
                ModInfo.Log($"'{locale}.yml' found for mod '{modid}'.", BepInEx.Logging.LogLevel.Message);
            localizationData[locale] = ReadLocalizationByPath(path);
            localizations[modid] = true;
        }

        internal static void TryLoadLocale(Locale locale)
        {
            if (!localizationData.ContainsKey(locale))
                LoadLocalization(locale, false);
        }

        internal static string GetLocalizedSilent(string key, Locale locale)
        {
            TryLoadLocale(locale);
            string value = null;
            if (localizationData.TryGetValue(locale, out LocalizationEntry entry))
                value = entry.GetLocalization(key);
            return value ?? ((locale != Locale.en) ? GetLocalizedSilent(key, Locale.en) : null);
        }

        private static LocalizationEntry ReadLocalizationByPath(string path)
        {
            LocalizationEntry entry = new LocalizationEntry();
            using (var stream = new StreamReader(path))
            {
                var yaml = new YamlStream();
                yaml.Load(stream);
                if (yaml.Documents.Count == 0)
                {
                    ModInfo.Log($"Empty localization file on path {path}!", BepInEx.Logging.LogLevel.Error);
                    return entry;
                }
                foreach (var node in yaml.Documents[0].RootNode as YamlMappingNode)
                {
                    entry.AddLocalization((string)node.Key, (string)node.Value);
                }
            }
            return entry;
        }

        internal class LocalizationEntry
        {
            internal readonly Dictionary<string, string> localizationData = new Dictionary<string, string>();

            internal void AddLocalization(string key, string value)
            {
                localizationData[key] = value;
            }

            internal string GetLocalization(string key)
            {
                if (localizationData.TryGetValue(key, out string value))
                    return value;
                return null;
            }

        }

    }

}