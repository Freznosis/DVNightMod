using System;
using System.Reflection;
using Harmony12;
using UnityEngine;
using UnityModManagerNet;

namespace DVNightMod
{
	public static class Main
	{
		private static bool Load(UnityModManager.ModEntry modEntry)
		{
			HarmonyInstance.Create(modEntry.Info.Id).PatchAll(Assembly.GetExecutingAssembly());
			Main.Enabled = modEntry.Enabled;
			Main.ModEntry = modEntry;
			
			//Create sky manager instance.
			Debug.Log((object) "Creating NightSkyManager");

			var nsmgr = new GameObject("[NightSkyManagerInstance]");
			nsmgr.transform.SetSiblingIndex(0);
			NightSkyManagerInstance = nsmgr.AddComponent<NightSkyManager>();
			NightSkyManagerInstance.Setup();

			return true;
		}

		public static NightSkyManager NightSkyManagerInstance;
		public static UnityModManager.ModEntry ModEntry;
		public static bool Enabled;
	}
}
