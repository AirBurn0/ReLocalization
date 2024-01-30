# ReLocalization for Potion Craft

[![Code license (CC BY-NC 4.0)](https://img.shields.io/badge/License-CC%20BY--NC%204.0-blue.svg?style=flat-square)](https://creativecommons.org/licenses/by-nc/4.0)

Public repository for ReLocalization (Potion Craft Mod / BepInEx 5 plugin).

## What is it for?

Hierarchy of default localization system in Potion Craft looks like this:
```
           ┌──────┐      ┌─────┐
           │locale│  ->  │value│
┌───┐      ├──────┤      ├─────┤ 
│key│  ->  │locale│  ->  │value│
└───┘      ├──────┤      ├─────┤ 
           │locale│  ->  │value│
           └──────┘      └─────┘
```
__And it really pisses me off__. If someone wants to add new words to the localization, correct the translation, or simply add a new language - they hill have to suffer and rummage through an endless number of game files, reading tons of information noise, in attempts to write simple __'key:value'__ state.

But what if the structure was like this:
```
              ┌───┐      ┌─────┐
              │key│  ->  │value│
┌──────┐      ├───┤      ├─────┤ 
│locale│  ->  │key│  ->  │value│
└──────┘      ├───┤      ├─────┤ 
              │key│  ->  │value│
              └───┘      └─────┘
```
Then it would be enough to simply find or create a localization file with the desired language, for example __'en.yml'__, and add/change the __'key:value'__.
It will make life easier for translators who want to translate - not suffer (who ever wants???)

This mod actually does just that.

## How to work?

* For translators: Simply find __'..\\Potion Craft\\BepInEx\\plugins\\Localization\\{GUID}'__ folder, where GUID is desired Mod's ID (some word, string), and create __'*.yml'__ file with lang code name like that: 'en.yml'. List of language codes supported by Potion Craft: 
    * ru - Russian (Potion Craft author's native language),
    * en - English (Default localization language),
    * de - German,
    * fr - French,
    * it - Italian,
    * es - Spanish,
    * ptBR - Portuguese,
    * pl - Polish,
    * tr - Turkish,
    * zh - Simplified Chinese,
    * ja - Japanese,
    * ko - Korean,
    * th - Thai,
    * cs - Czech.
* For modders: just add mod's dll to project and write following line of code in any suitable place (__Awake()__ method in your __BaseUnityPlugin__):
    ```
    Localization.AddLocalizationFor(this);
    ```
Don't forget to create mod localization files and folder.
As example - __'..\\Potion Craft\\BepInEx\\plugins\\Localization\\AirBurn.ReLocalization\\en.yml'__ would provide localization for this mod. If it had it (however, you can still write __something__ there and for sure, __funny things__ will happen).

Basically there is no guides. If some more complex examples needed - consider searching for some mod that uses this and reading it's repo.

## [Crucible](https://github.com/RoboPhred/potioncraft-crucible) (Potion Craft) exists, you know?

Yeah, I know.

~~Developers aren't Russians - won’t use.~~

[Crucible](https://github.com/RoboPhred/potioncraft-crucible) is really cool and very comfortable zero-code modding framework (myself tested), but that doesn't really apply to localization issue.
Crucible Dev's decided not to reinvent the wheel and use essentially the same approach as Potion Craft uses (btw understandable), and I have already explained my attitude to default localization system.

## Are you the smartest one here?

Unfortunately - no. I started studying Unity quite recently, and the code is most likely complete crap. So perhaps what I did is even worse (in terms of performance) than what already exists. Mod was written in about 3 hours and I didn’t really tested it. There are most likely quite a few bugs, and I would be glad if they were at least reported.

Perhaps it’s even more convenient for development or something like that to use default Potion Craft's localization system or even Crucible, but I don’t know many developers who speak >10 languages, and I’m also not familiar with >10 developers from different countries each speaks at least one of game's different localization language. So I personally will need a helping hand of non-developer translators, otherwise Google Translator is certainly cool, but... who knows - they knows. And the less they have to suffer, the greater the chance that volunteers will appear someday.

And one more benefit for me - no need to fumble around in the code - just upload the file and that's all.