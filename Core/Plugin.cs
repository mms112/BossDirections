using BepInEx;
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
*/

namespace BossDirections {
	[BepInPlugin("fiote.mods.bossdirections", "BossDirections", "0.1.0")]

	public class BossDirections : BaseUnityPlugin {
		// core stuff
		public static bool debug = true;
		public static List<Offering> offerings;
		public static ConfigJson config;

		private void Awake() {
			Bar();
			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "fiote.mods.bossdirections");
			Debug("Awake");
			LoadConfig();
			LoadOfferings();
			Bar();
		}

		void LoadConfig() {
			Line();
			Debug("LoadConfig()");
			config = new ConfigJson();
			var jsondata = JsonLoader.LoadJsonFile<ConfigJson>("config.json");
			if (jsondata != null) config = jsondata;
			Debug("Config loaded: pinless=" + config.pinless);
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

public class ConfigJson {
	public bool pinless;
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