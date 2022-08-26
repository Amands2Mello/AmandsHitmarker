using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace AmandsHitmarker
{
    [BepInPlugin("com.Amanda.Hitmarker", "Hitmarker", "1.1.0")]
    public class AHitmarkerPlugin : BaseUnityPlugin
    {
        public static GameObject Hook;
        public static ConfigEntry<Vector2> Thickness { get; set; }
        public static ConfigEntry<float> CenterOffset { get; set; }
        public static ConfigEntry<float> OffsetSpeed { get; set; }
        public static ConfigEntry<float> OpacitySpeed { get; set; }
        public static ConfigEntry<string> Shape { get; set; }
        public static ConfigEntry<string> HeadshotShape { get; set; }
        public static ConfigEntry<Vector2> BleedSize { get; set; }
        public static ConfigEntry<bool> EnableSounds { get; set; }
        public static ConfigEntry<string> HitmarkerSound { get; set; }
        public static ConfigEntry<string> HeadshotHitmarkerSound { get; set; }
        public static ConfigEntry<string> KillHitmarkerSound { get; set; }
        public static ConfigEntry<Color> HitmarkerColor { get; set; }
        public static ConfigEntry<Color> ArmorColor { get; set; }
        public static ConfigEntry<Color> BearColor { get; set; }
        public static ConfigEntry<Color> UsecColor { get; set; }
        public static ConfigEntry<Color> ScavColor { get; set; }
        public static ConfigEntry<Color> ThrowWeaponColor { get; set; }
        public static ConfigEntry<Color> BossColor { get; set; }
        public static ConfigEntry<Color> BleedColor { get; set; }
        public static ConfigEntry<bool> HitmarkerDebug { get; set; }
        public static ConfigEntry<bool> HeadshotHitmarkerDebug { get; set; }
        public static ConfigEntry<bool> BleedHitmarkerDebug { get; set; }
        public static ConfigEntry<bool> ArmorHitmarkerDebug { get; set; }
        public static ConfigEntry<bool> UsecHitmarkerDebug { get; set; }
        public static ConfigEntry<bool> BearHitmarkerDebug { get; set; }
        public static ConfigEntry<bool> ScavHitmarkerDebug { get; set; }
        public static ConfigEntry<bool> ThrowWeaponHitmarkerDebug { get; set; }
        public static ConfigEntry<bool> BossHitmarkerDebug { get; set; }
        public static ConfigEntry<bool> HitmarkerSoundDebug { get; set; }
        public static ConfigEntry<bool> HeadshotHitmarkerSoundDebug { get; set; }
        public static ConfigEntry<bool> KillHitmarkerSoundDebug { get; set; }
        public static ConfigEntry<bool> ReloadFiles { get; set; }
        public static ConfigEntry<bool> StaticHitmarkerOnly { get; set; }
        public static ConfigEntry<Vector2> StaticSizeDelta { get; set; }
        public static ConfigEntry<float> StaticSizeDeltaSpeed { get; set; }
        public static ConfigEntry<float> StaticOpacity { get; set; }
        private void Awake()
        {
            Debug.LogError("AmandsHitmarker Awake()");
            Hook = new GameObject();
            Hook.AddComponent<AmandsHitmarkerClass>();
            DontDestroyOnLoad(Hook);
        }
        private void Start()
        {
            Thickness = Config.Bind<Vector2>("AmandsHitmarker", "Thickness", new Vector2(40.0f, 40.0f), new ConfigDescription("Individual image size", null, new ConfigurationManagerAttributes { Order = 210 }));
            CenterOffset = Config.Bind<float>("AmandsHitmarker", "CenterOffset", 10.0f, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 200 }));
            OffsetSpeed = Config.Bind<float>("AmandsHitmarker", "OffsetSpeed", 0.05f, new ConfigDescription("Animation size increase", null, new ConfigurationManagerAttributes { Order = 190 }));
            OpacitySpeed = Config.Bind<float>("AmandsHitmarker", "OpacitySpeed", 0.05f, new ConfigDescription("Animation opacity decrease", null, new ConfigurationManagerAttributes { Order = 180 }));
            Shape = Config.Bind<string>("AmandsHitmarker", "Shape", "Hitmarker.png", new ConfigDescription("Supported File PNG", null, new ConfigurationManagerAttributes { Order = 170 }));
            HeadshotShape = Config.Bind<string>("AmandsHitmarker", "HeadshotShape", "HeadshotHitmarker.png", new ConfigDescription("Supported File PNG", null, new ConfigurationManagerAttributes { Order = 160 }));
            BleedSize = Config.Bind<Vector2>("AmandsHitmarker", "BleedSize", new Vector2(128.0f, 128.0f), new ConfigDescription("Bleed kill glow image size", null, new ConfigurationManagerAttributes { Order = 150 }));
            EnableSounds = Config.Bind<bool>("AmandsHitmarker", "EnableSounds", true, new ConfigDescription("Supported Files WAV OGG", null, new ConfigurationManagerAttributes { Order = 140 }));
            HitmarkerSound = Config.Bind<string>("AmandsHitmarker", "HitmarkerSound", "Hitmarker.wav", new ConfigDescription("Supported Files WAV OGG", null, new ConfigurationManagerAttributes { Order = 130 }));
            HeadshotHitmarkerSound = Config.Bind<string>("AmandsHitmarker", "HeadshotHitmarkerSound", "HeadshotHitmarker.wav", new ConfigDescription("Supported Files WAV OGG", null, new ConfigurationManagerAttributes { Order = 120 }));
            KillHitmarkerSound = Config.Bind<string>("AmandsHitmarker", "KillHitmarkerSound", "KillHitmarker.wav", new ConfigDescription("Supported Files WAV OGG", null, new ConfigurationManagerAttributes { Order = 110 }));

            HitmarkerColor = Config.Bind<Color>("Colors", "HitmarkerColor", new Color(1.0f, 1.0f, 1.0f, 1.0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 180 }));
            ArmorColor = Config.Bind<Color>("Colors", "ArmorColor", new Color(0.2826087f, 0.6086956f, 1.0f, 1.0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 170 }));
            BearColor = Config.Bind<Color>("Colors", "BearColor", new Color(1.0f, 0.0f, 0.0f, 1.0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 160 }));
            UsecColor = Config.Bind<Color>("Colors", "UsecColor", new Color(1.0f, 0.0f, 0.1f, 1.0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 150 }));
            ScavColor = Config.Bind<Color>("Colors", "ScavColor", new Color(1.0f, 0.3043478f, 0.0f, 1.0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 140 }));
            ThrowWeaponColor = Config.Bind<Color>("Colors", "ThrowWeaponColor", new Color(0.4130435f, 0.0f, 1.0f, 1.0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 130 }));
            BossColor = Config.Bind<Color>("Colors", "BossColor", new Color(1.0f, 0.8043478f, 0.0f, 1.0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 120 }));
            BleedColor = Config.Bind<Color>("Colors", "BleedColor", new Color(1.0f, 0.0f, 0.0f, 0.69f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 110 }));

            HitmarkerDebug = Config.Bind<bool>("Debug", "HitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 230, IsAdvanced = true }));
            HeadshotHitmarkerDebug = Config.Bind<bool>("Debug", "HeadshotHitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 220, IsAdvanced = true }));
            BleedHitmarkerDebug = Config.Bind<bool>("Debug", "BleedHitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 210, IsAdvanced = true }));
            ArmorHitmarkerDebug = Config.Bind<bool>("Debug", "ArmorHitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 200, IsAdvanced = true }));
            BearHitmarkerDebug = Config.Bind<bool>("Debug", "BearHitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 190, IsAdvanced = true }));
            UsecHitmarkerDebug = Config.Bind<bool>("Debug", "UsecHitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 180, IsAdvanced = true }));
            ScavHitmarkerDebug = Config.Bind<bool>("Debug", "ScavHitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 170, IsAdvanced = true }));
            ThrowWeaponHitmarkerDebug = Config.Bind<bool>("Debug", "ThrowWeaponHitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 160, IsAdvanced = true }));
            BossHitmarkerDebug = Config.Bind<bool>("Debug", "BossHitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 150, IsAdvanced = true }));
            HitmarkerSoundDebug = Config.Bind<bool>("Debug", "HitmarkerSoundDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 140, IsAdvanced = true }));
            HeadshotHitmarkerSoundDebug = Config.Bind<bool>("Debug", "HeadshotHitmarkerSoundDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 130, IsAdvanced = true }));
            KillHitmarkerSoundDebug = Config.Bind<bool>("Debug", "KillHitmarkerSoundDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 120, IsAdvanced = true }));
            ReloadFiles = Config.Bind<bool>("Debug", "ReloadFiles", false, new ConfigDescription("Currently disabled", null, new ConfigurationManagerAttributes { Order = 110, IsAdvanced = true }));

            StaticHitmarkerOnly = Config.Bind<bool>("Static", "StaticHitmarkerOnly", false, new ConfigDescription("Use only the static hitmarker image", null, new ConfigurationManagerAttributes { Order = 140, IsAdvanced = true }));
            StaticSizeDelta = Config.Bind<Vector2>("Static", "StaticSizeDelta", new Vector2(40.0f, 40.0f), new ConfigDescription("Static hitmarker size", null, new ConfigurationManagerAttributes { Order = 130, IsAdvanced = true }));
            StaticSizeDeltaSpeed = Config.Bind<float>("Static", "StaticSizeDeltaSpeed", 0.01f, new ConfigDescription("Animation size increase", null, new ConfigurationManagerAttributes { Order = 120, IsAdvanced = true }));
            StaticOpacity = Config.Bind<float>("Static", "StaticOpacity", 1.0f, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 110, IsAdvanced = true }));
        }
    }
}
