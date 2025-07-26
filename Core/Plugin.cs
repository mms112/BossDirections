using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

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

    public static class Extensions
    {
        private static readonly string[] vendors = { "Vendor_BlackForest", "Hildir_camp", "BogWitch_Camp" };

        private static Offering GetOffering(this ItemDrop.ItemData item, out int numItems)
        {
            // checking all offerings
            foreach (var offering in BossDirections.offerings)
            {
                // checking items for each offering
                foreach (var offitem in offering.items)
                {
                    // checking if the item is right
                    if ("$item_" + offitem.Key.ToLower() == item.m_shared.m_name.ToLower())
                    {
                        numItems = offitem.Value;
                        return offering;
                    }
                }
            }

            numItems = 0;
            return null;
        }

        public static bool TryOffer(this Fireplace target, Humanoid user, ItemDrop.ItemData item)
        {
            ZoneSystem.LocationInstance loc;
            int numItems;
            Offering offering = item.GetOffering(out numItems);

            if (offering == null)
                return false;

            if (offering.boss_location == "")
            {
                BossDirections.TryOffer(offering, user, item, numItems);
                return true;
            }

            for (int i = 0; i < vendors.Length; i++)
            {
                if (ZoneSystem.instance.FindClosestLocation(vendors[i], target.transform.position, out loc))
                {
                    if (Vector3.Distance(target.transform.position, loc.m_position) < loc.m_location.m_exteriorRadius)
                    {
                        //All offers are allowed in vendor location
                        BossDirections.TryOffer(offering, user, item, numItems);
                        return true;
                    }
                }
            }

            user.Message(MessageHud.MessageType.Center, $"Das Opfer für {offering.name} kann hier nicht erbracht werden");
            return true;
        }

        public static bool TryOffer(this Vegvisir target, Humanoid user, ItemDrop.ItemData item)
        {
            int numItems;
            Offering offering = item.GetOffering(out numItems);

            if (offering == null)
                return false;

            if ((offering.boss_location == "") || (target.m_locations[0].m_locationName == offering.boss_location))
            {
                BossDirections.TryOffer(offering, user, item, numItems);
                return true;
            }

            user.Message(MessageHud.MessageType.Center, $"Das Opfer für {offering.name} kann hier nicht erbracht werden");
            return true;
        }

        public static bool TryOffer(this OfferingBowl target, Humanoid user, ItemDrop.ItemData item)
        {
            ZoneSystem.LocationInstance loc;
            int numItems;
            Offering offering = item.GetOffering(out numItems);

            if (offering == null)
                return false;

            if (offering.boss_location == "")
            {
                BossDirections.TryOffer(offering, user, item, numItems);
                return true;
            }

            if (ZoneSystem.instance.FindClosestLocation(offering.boss_location, target.transform.position, out loc))
            {
                if (Vector3.Distance(target.transform.position, loc.m_position) < loc.m_location.m_exteriorRadius)
                {
                    BossDirections.TryOffer(offering, user, item, numItems);
                    return true;
                }
            }

            user.Message(MessageHud.MessageType.Center, $"Das Opfer für {offering.name} kann hier nicht erbracht werden");
            return true;
        }
    }

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
		public static void TryOffer(Offering offering, Humanoid user, ItemDrop.ItemData item, int neededAmount) {
			var iname = item.m_shared.m_name.ToLower();
            var stack = user.GetInventory().CountItems(item.m_shared.m_name);
            Log($"Fireplace_UseItem_Patch Prefix {iname} stack={stack}");
			Log($"neededAmount={neededAmount} bossCode={offering.location} bossName={offering.name} talks={offering.quotes.Count}");

			// if the player doesnt have the needed amount
			if (stack < neededAmount) {
				user.Message(MessageHud.MessageType.Center, "$msg_toofew " + iname);
			}

			var bossTalk = "Der Weg offenbart sich dir...";
			if (offering.addname) bossTalk = $"[{offering.name}]: " + bossTalk;

			// removing the item from the player's inventory
			user.GetInventory().RemoveItem(item.m_shared.m_name, neededAmount);

			// showing the boss quote
			user.Message(MessageHud.MessageType.Center, bossTalk);

			// getting the player location
			var point = user.gameObject.transform.position;

			if (configPinless.Value) {
				Log("Discovering location in Pinless mode...");
				ZoneSystem.LocationInstance closest;
				ZoneSystem.instance.FindClosestLocation(offering.location, point, out closest);
				Player.m_localPlayer.SetLookDir(closest.m_position - Player.m_localPlayer.transform.position, 3.5f);
			} else {
				Log("Discovering location in normal mode...");
				Game.instance.DiscoverClosestLocation(offering.location, point, offering.name, (int)Minimap.PinType.Boss, false, false);
			}

			Log("Offering done!");
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
    public string boss_location;
	public List<string> quotes;
	public Dictionary<string, int> items;
}