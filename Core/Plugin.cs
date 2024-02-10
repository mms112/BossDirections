using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

/*
 * [0.0.1]
 * First release.
 * 
 * [0.0.2]
 * Adding more logs to show which offererings were loaded.
 * Removing deer hide from the default offerings.
 * Removing drake thophies from the default offerings.
 * Adding a check for wood in the fireplace to avoid spamming debugs.
 * Showing what offerings are loaded when trying to burn anything.
 * 
 * [0.0.3]
 * Fixing missing LitJson reference.
 * 
 * [0.1.0]
 * Adding pinless config/mode (default: true).
 * 
 * [0.2.0]
 * Moving patch to player.usehotbaritem so it happens before usefultrophies.
 * Moving pinless config to an actualy config file instead of using a json (offerings are still on a json).

 * [0.2.1]
 * Checking for fireplaces before offering.

 * [0.2.2]
 * Fixing return values on useHotBarItem.

 * [0.2.3]
 * Fixing return values on useHotBarItem.
*/

namespace BossDirections {
	[BepInPlugin("fiote.mods.bossdirections", "BossDirections", "0.2.3")]

	public class BossDirections : BaseUnityPlugin {
		// core stuff
		public static bool debug = true;
		public static ConfigFile configFile;
		public static ConfigEntry<bool> configPinless;
		public static List<Offering> offerings;

		private void Awake() {
			Bar();
			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "fiote.mods.bossdirections");
			Debug("Awake");
			LoadOfferings();
			SetupConfig();
			Bar();
		}

		#region SETUP

		void SetupConfig() {
			Line();
			Debug("SetupConfig()");
			configFile = Config;
			Config.Bind("General", "NexusID", 2692, "NexusMods ID for updates.");			
			configPinless = Config.Bind("General", "Pinless", true, "Keep the offerings camera-only, without pinning on your map.");
		}

		void LoadOfferings() {
			Line();
			Debug("LoadOfferings()");
			offerings = new List<Offering>();
			var jsondata = JsonLoader.LoadJsonFile<OfferingsJson>("offerings.json");
			if (jsondata != null) offerings = jsondata.offerings;

			foreach (var offering in offerings) {
				Debug("Offering loaded: " + offering.name);
			}

			if (offerings.Count == 0) {
				Error("No offerings loaded. Are you sure you have BossDirections/offerings.json on your plugins folder?");
			}
		}



		#endregion
		#region METHODS
		public static bool TryOffer(ItemDrop.ItemData item) {
			var iname = item.m_shared.m_name.ToLower();
			Log($"Fireplace_UseItem_Patch Prefix {iname} stack={item.m_stack}");

			var neededAmount = 0;
			var bossLocation = "";
			var bossName = "";
			var addname = false;
			var talks = new List<string>();

			// checking all offerings
			offerings.ForEach(offering => {
				// checking items for each offering
				foreach (var offitem in offering.items) {
					// checking if the item is right
					if ("$item_" + offitem.Key.ToLower() == iname) {
						// setting all the values needed for this to work
						neededAmount = offitem.Value;
						bossLocation = offering.location;
						bossName = offering.name;
						talks = offering.quotes;
						addname = offering.addname;
					}
				}
			});

			Log($"neededAmount={neededAmount} bossCode={bossLocation} bossName={bossName} talks={talks.Count}");

			// if we did not find a location, stop here
			if (bossLocation == "") return false;

			var player = Player.m_localPlayer;

			// if the player doesnt have the needed amount
			if (item.m_stack < neededAmount) {
				// let they know
				player.Message(MessageHud.MessageType.Center, "$msg_toofew " + iname);
				// return true because this was an offering item
				return true;
			}

			// getting a random boss quote
			var rnd = new System.Random();
			var randomIndex = rnd.Next(talks.Count);
			var bossTalk = talks[randomIndex];
			if (addname) bossTalk = $"[{bossName}]: " + bossTalk;

			// removing the item from the player's inventory
			player.GetInventory().RemoveItem(item.m_shared.m_name, neededAmount);

			// showing the boss quote
			player.Message(MessageHud.MessageType.Center, bossTalk);

			// getting the player location
			var point = player.gameObject.transform.position;

			if (configPinless.Value) {
				Log("Discovering location in Pinless mode...");
				ZoneSystem.LocationInstance closest;
				ZoneSystem.instance.FindClosestLocation(bossLocation, point, out closest);
				Player.m_localPlayer.SetLookDir(closest.m_position - Player.m_localPlayer.transform.position, 3.5f);
			} else {
				Log("Discovering location in normal mode...");
				Game.instance.DiscoverClosestLocation(bossLocation, point, bossName, (int)Minimap.PinType.Boss, false, false);
			}

			Log("Offering done!");
			return true;
		}

		#endregion

		#region LOGGING

		public static void Bar() {
			Debug("=============================================================");
		}

		public static void Line() {
			Debug("-------------------------------------------------------------");
		}

		public static void Debug(string message) {
			if (debug) Log(message);
		}

		public static void Log(string message) {
			UnityEngine.Debug.Log($"[BossDirections] {message}");
		}

		public static void Error(string message) {
			UnityEngine.Debug.LogError($"[BossDirections] {message}");
		}

		#endregion
	}
}

public class OfferingsJson {
	public List<Offering> offerings;
}

public class Offering {
	public string location, name;
	public bool addname;
	public List<string> quotes;
	public Dictionary<string, int> items;
}