using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using WeAreHigh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeAreHigh
{
	[BepInPlugin("com.dsrdakota.mods", "We Are High", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		public static ManualLogSource log;

		public static ConfigEntry<bool> infiniteHealth;
		public static ConfigEntry<bool> infiniteStamina;
		public static ConfigEntry<bool> infiniteOxygen;

		public static ConfigEntry<float> baseMovement;
		public static ConfigEntry<float> sprintMult;
		public static ConfigEntry<float> oxygenMult;
		public static ConfigEntry<float> staminaMult;

		public static ConfigEntry<float> maxHealth;
		public static ConfigEntry<float> maxOxygen;
		public static ConfigEntry<float> maxStamina;

		private void Awake()
		{

			infiniteHealth	= Config.Bind<bool>("Player", "Infinite health", false, "Be almost a God (but prevent soft-locks).");
			infiniteStamina	= Config.Bind<bool>("Player", "Infinite stamina", false, "Do you want to walk forever?");
			infiniteOxygen	= Config.Bind<bool>("Player", "Infinite oxygen", false, "Bro is a fish.");

			baseMovement	= Config.Bind<float>("Player", "Base movement speed", 17f, "This is the base walk speed.");
			sprintMult		= Config.Bind<float>("Player", "Sprint multiplier", 2.3f, "Sprint multiplier to use.");
			oxygenMult		= Config.Bind<float>("Player", "Oxygen depletion multiplier", 5f, "How fast to deplete oxygen.");
			staminaMult		= Config.Bind<float>("Player", "Stamina depletion multiplier", 2f, "How fast to deplete stamina.");

			maxHealth		= Config.Bind<float>("Player", "Maximum health", 100f, "(Does nothing currently)");
			maxOxygen		= Config.Bind<float>("Player", "Maximum oxygen", 500f, "How long do you to want to stay fishy?");
			maxStamina		= Config.Bind<float>("Player", "Maximum stamina", 20f, "How long do you to run for?");

			var harmony = new Harmony("com.dsrdakota.mods.wearehigh");

			harmony.PatchAll(typeof(Plugin));
			harmony.PatchAll(typeof(Patches.PlayerControllerPatch));

			log = Logger;
			// ASCII Art
			Logger.LogInfo(@"");
		}
	}
}
