using System;
using System.Collections;
using System.IO;
using System.Linq;
using Harmony12;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
using LightType = UnityEngine.LightType;

namespace DVNightMod
{

    public class NightSkyManager : MonoBehaviour
    {

        public Material nightSkyMaterial;
        public Shader nightSkyShader;
        
        public void Setup()
        {
            //Load skybox assetbundle
            string bundlePath = Path.Combine(Main.ModEntry.Path, "nightskymode");
            Debug.Log($"Loading nightskymode.assetbundle from path '{bundlePath}");

            var assetBundle = AssetBundle.LoadFromFile(bundlePath);
            
            if (assetBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }

            nightSkyMaterial = assetBundle.LoadAsset<Material>("Cold Night");

            if (nightSkyMaterial != null)
            {
                Main.ModEntry.Logger.Log($"Successfully loaded asset bundle. Material name is {nightSkyMaterial.name} with shader {nightSkyMaterial.shader.name}");

                nightSkyShader = nightSkyMaterial.shader;
            }
            
            assetBundle.Unload(false);
        }

        public void ReplaceSky()
        {
            if (nightSkyMaterial != null)
            {
                RenderSettings.skybox = nightSkyMaterial;
                RenderSettings.ambientMode = AmbientMode.Flat;
                RenderSettings.ambientLight = new Color32(12, 18, 30, 255);

                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color32(9, 15, 24, 255);
                RenderSettings.fogMode = FogMode.Linear;
                RenderSettings.fogStartDistance = 1;
                RenderSettings.fogEndDistance = 500;

                //Find direction light and change settings
                var directionalLight = FindObjectsOfType<Light>().FirstOrDefault(x => x.type == LightType.Directional);

                if (directionalLight != null)
                {
                    directionalLight.color = new Color32(61,42,100, 255);
                    directionalLight.intensity = 0.59f;
                    directionalLight.transform.rotation = Quaternion.Euler(163.442f, 0.583f, 0.024f);

                    RenderSettings.sun = directionalLight;
                }
                
                DynamicGI.UpdateEnvironment();
            }
        }
    }
    
    [HarmonyPatch(typeof(PlayerInstantiator), "Awake")]
    static class PlayerInstantiator_Awake_Patch
    {
        static void Postfix()
        {
            if (!Main.Enabled)
                return;
            
            try
            {
               //Probably shouldnt patch here but im lazy.
               Main.NightSkyManagerInstance.ReplaceSky();
            }
            catch(Exception e)
            {
                Main.ModEntry.Logger.Error(e.ToString());
            }
        }
    }
}