using HarmonyLib;
using Photon.Realtime;
using pworld.Scripts;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using UnityEngine;

namespace WeAreHigh.Patches
{
	[HarmonyPatch(typeof(PlayerController))]
	internal class PlayerControllerPatch
	{

		[HarmonyPatch("Start")]
		[HarmonyPostfix]
		private static void StartPatch() {
			WeAreHigh.Plugin.log.LogInfo((object)"StartPatch1 running.");
		}

		[HarmonyPatch("Start")]
		[HarmonyPostfix]
		private static void StartPatch(ref float ___maxStamina, ref float ___staminaRegRate, ref float ___sprintMultiplier)
		{
			WeAreHigh.Plugin.log.LogInfo((object)"StartPatch2 running.");
			if(!Player.localPlayer) {
				WeAreHigh.Plugin.log.LogInfo((object)"warn1 player not ready.");
			}
			___maxStamina = WeAreHigh.Plugin.maxStamina.Value;
			___staminaRegRate = WeAreHigh.Plugin.staminaMult.Value;
			___sprintMultiplier = WeAreHigh.Plugin.sprintMult.Value;
			Player.localPlayer.data.currentStamina = WeAreHigh.Plugin.maxStamina.Value;
			if (!Player.localPlayer.isActiveAndEnabled || !Player.localPlayer.data.isGrounded
				|| Player.localPlayer.data.isHangingUpsideDown || Player.localPlayer.data.dead
				|| Player.localPlayer.data.fallTime > 0f || !Player.localPlayer.data.physicsAreReady) {
				WeAreHigh.Plugin.log.LogInfo((object)"warn2 player not ready.");
				return;
			}
		}
		[HarmonyPatch("Update")]
		[HarmonyPostfix]
		private static void UpdatePatch(ref PlayerController __instance)
		{
			WeAreHigh.Plugin.log.LogInfo((object)"UpdatePatch running.");
			// this is an ugly hack to retrieve a private field from PlayerController
			FieldInfo playerField = typeof(PlayerController).GetField("player", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
			Player player = (Player)playerField.GetValue(__instance);

			if (player == null) {
				WeAreHigh.Plugin.log.LogError((object)"error couldnt get player object.");
				return;
			}
			if (!player.isActiveAndEnabled || !player.data.isGrounded || player.data.isHangingUpsideDown
				|| player.data.dead || player.data.fallTime > 0f || !player.data.physicsAreReady) {
				WeAreHigh.Plugin.log.LogInfo((object)"warn3 player not ready.");
				return;
			}

		//Player.PlayerData data = Player.localPlayer.data;

			Player.PlayerData data = player.data;
			if (data.dead) return;

			// UPDATE ALL player fields in PlayerController and PlayerData
			__instance.maxStamina			= WeAreHigh.Plugin.maxStamina.Value;
			__instance.movementForce		= WeAreHigh.Plugin.baseMovement.Value;
			__instance.sprintMultiplier		= WeAreHigh.Plugin.sprintMult.Value;
			__instance.staminaRegRate		= WeAreHigh.Plugin.staminaMult.Value;
			

			data.maxOxygen					= WeAreHigh.Plugin.maxOxygen.Value;
			
			// how tf do we set max health, we can't?
			//data.maxHealth					= WeAreHigh.Plugin.maxHealth.Value;

			// this is ugly
			if (data.sinceSprint > 1f) {
				if (WeAreHigh.Plugin.staminaMult.Value > 1f) {
					WeAreHigh.Plugin.log.LogInfo("currentStamina 1: " + data.currentStamina.ToString());
					data.currentStamina = Mathf.MoveTowards(data.currentStamina, WeAreHigh.Plugin.maxStamina.Value, (WeAreHigh.Plugin.staminaMult.Value/100 - 1f) * Time.deltaTime);
					WeAreHigh.Plugin.log.LogInfo("currentStamina 2: " + data.currentStamina.ToString());
				} else if (WeAreHigh.Plugin.staminaMult.Value > 0f) {
					WeAreHigh.Plugin.log.LogInfo("currentStamina 1: " + data.currentStamina.ToString());
					data.currentStamina = Mathf.MoveTowards(data.currentStamina, 0f, (1f - WeAreHigh.Plugin.staminaMult.Value/100) * Time.deltaTime);
					WeAreHigh.Plugin.log.LogInfo("currentStamina 2: " + data.currentStamina.ToString());
				}
			}
			if (WeAreHigh.Plugin.infiniteHealth.Value == true) {
				data.health = 100f;
				data.remainingOxygen = data.maxOxygen;
				data.usingOxygen = false;
			}
			if (WeAreHigh.Plugin.infiniteOxygen.Value == true) {
				data.remainingOxygen = data.maxOxygen;
				data.usingOxygen = false;
			} else {
				// let's override the game's default depletion rate
				WeAreHigh.Plugin.log.LogInfo("DeltaTime: " + Time.deltaTime.ToString());
				data.remainingOxygen  -= Mathf.Clamp01(Time.deltaTime * (WeAreHigh.Plugin.oxygenMult.Value / 100));
			}
			if (WeAreHigh.Plugin.infiniteStamina.Value == true) {
				data.currentStamina = WeAreHigh.Plugin.maxStamina.Value;
			}

			Player.localPlayer.data = data;
			player.data = data;
		}
	}
}
