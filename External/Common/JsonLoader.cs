using BepInEx;
using LitJson;
using System.IO;
using System.Reflection;


public static class JsonLoader {
	public static T LoadJsonFile<T>(string filename) where T : class {
		var jsonFileName = GetAssetPath(filename);
		if (!string.IsNullOrEmpty(jsonFileName)) {
			var jsonFile = File.ReadAllText(jsonFileName);
			return JsonMapper.ToObject<T>(jsonFile);
		}

		return null;
	}

	public static string GetAssetPath(string assetName) {
		var assetFileName = Path.Combine(Paths.PluginPath, "BossDirections", assetName);
		if (!File.Exists(assetFileName)) {
			Assembly assembly = typeof(BossDirections.BossDirections).Assembly;
			assetFileName = Path.Combine(Path.GetDirectoryName(assembly.Location), assetName);
			if (!File.Exists(assetFileName)) {
				BossDirections.BossDirections.Error($"File not found: {assetFileName}");
				return null;
			}
		}

		return assetFileName;
	}
}