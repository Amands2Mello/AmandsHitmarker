using BepInEx;
using BepInEx.Configuration;
using EFT;
using EFT.InventoryLogic;
using System.Reflection;
using Aki.Reflection.Patching;
using UnityEngine;
using TMPro;
using HarmonyLib;

namespace AmandsHitmarker
{
    [BepInPlugin("com.Amanda.Hitmarker", "Hitmarker", "2.1.1")]
    public class AHitmarkerPlugin : BaseUnityPlugin
    {
        public static GameObject Hook;
        public static ConfigEntry<bool> EnableHitmarker { get; set; }
        public static ConfigEntry<bool> EnableBleeding { get; set; }
        public static ConfigEntry<EHitmarkerPositionMode> HitmarkerPositionMode { get; set; }
        public static ConfigEntry<EHitmarkerPositionMode> ADSHitmarkerPositionMode { get; set; }
        public static ConfigEntry<Vector2> Thickness { get; set; }
        public static ConfigEntry<float> CenterOffset { get; set; }
        public static ConfigEntry<float> AnimatedTime { get; set; }
        public static ConfigEntry<float> AnimatedAlphaTime { get; set; }
        public static ConfigEntry<float> AnimatedAmplitude { get; set; }
        public static ConfigEntry<string> Shape { get; set; }
        public static ConfigEntry<string> HeadshotShape { get; set; }
        public static ConfigEntry<Vector2> BleedSize { get; set; }

        public static ConfigEntry<bool> EnableSounds { get; set; }
        public static ConfigEntry<float> SoundVolume { get; set; }
        public static ConfigEntry<string> HitmarkerSound { get; set; }
        public static ConfigEntry<string> HeadshotHitmarkerSound { get; set; }
        public static ConfigEntry<string> KillHitmarkerSound { get; set; }
        public static ConfigEntry<string> ArmorSound { get; set; }
        public static ConfigEntry<string> ArmorBreakSound { get; set; }

        public static ConfigEntry<Color> HitmarkerColor { get; set; }
        public static ConfigEntry<Color> ArmorColor { get; set; }
        public static ConfigEntry<Color> BearColor { get; set; }
        public static ConfigEntry<Color> UsecColor { get; set; }
        public static ConfigEntry<Color> ScavColor { get; set; }
        public static ConfigEntry<Color> ThrowWeaponColor { get; set; }
        public static ConfigEntry<Color> FollowerColor { get; set; }
        public static ConfigEntry<Color> BossColor { get; set; }
        public static ConfigEntry<Color> BleedColor { get; set; }
        public static ConfigEntry<Color> PoisonColor { get; set; }

        public static ConfigEntry<bool> HitmarkerDebug { get; set; }
        public static ConfigEntry<bool> HeadshotHitmarkerDebug { get; set; }
        public static ConfigEntry<bool> BleedHitmarkerDebug { get; set; }
        public static ConfigEntry<bool> PoisonHitmarkerDebug { get; set; }
        public static ConfigEntry<bool> ArmorHitmarkerDebug { get; set; }
        public static ConfigEntry<bool> UsecHitmarkerDebug { get; set; }
        public static ConfigEntry<bool> BearHitmarkerDebug { get; set; }
        public static ConfigEntry<bool> ScavHitmarkerDebug { get; set; }
        public static ConfigEntry<bool> ThrowWeaponHitmarkerDebug { get; set; }
        public static ConfigEntry<bool> FollowerHitmarkerDebug { get; set; }
        public static ConfigEntry<bool> BossHitmarkerDebug { get; set; }
        public static ConfigEntry<bool> HitmarkerSoundDebug { get; set; }
        public static ConfigEntry<bool> HeadshotHitmarkerSoundDebug { get; set; }
        public static ConfigEntry<bool> ArmorSoundDebug { get; set; }
        public static ConfigEntry<bool> ArmorBreakSoundDebug { get; set; }
        public static ConfigEntry<bool> KillHitmarkerSoundDebug { get; set; }
        public static ConfigEntry<bool> ReloadFiles { get; set; }
        public static ConfigEntry<bool> ReloadBattleUIScreen { get; set; }
        public static ConfigEntry<bool> StaticHitmarkerOnly { get; set; }
        public static ConfigEntry<Vector2> StaticSizeDelta { get; set; }
        public static ConfigEntry<float> StaticSizeDeltaSpeed { get; set; }
        public static ConfigEntry<float> StaticOpacity { get; set; }

        public static ConfigEntry<bool> EnableKillfeed { get; set; }
        public static ConfigEntry<Color> KillTextColor { get; set; }
        public static ConfigEntry<int> KillFontSize { get; set; }
        public static ConfigEntry<int> KillChildSpacing { get; set; }
        public static ConfigEntry<EKillPreset> KillPreset { get; set; }
        public static ConfigEntry<bool> KillChildDirection { get; set; }
        public static ConfigEntry<Vector2> KillRectPosition { get; set; }
        public static ConfigEntry<Vector2> KillRectPivot { get; set; }
        public static ConfigEntry<TextAlignmentOptions> KillTextAlignment { get; set; }
        public static ConfigEntry<float> KillFontOutline { get; set; }
        public static ConfigEntry<bool> KillFontUpperCase { get; set; }
        public static ConfigEntry<float> KillTime { get; set; }
        public static ConfigEntry<float> KillOpacitySpeed { get; set; }
        public static ConfigEntry<bool> KillUpperText { get; set; }
        public static ConfigEntry<EKillStart> KillStart { get; set; }
        public static ConfigEntry<EKillNameColor> KillNameColor { get; set; }
        public static ConfigEntry<Color> KillNameSingleColor { get; set; }
        public static ConfigEntry<EKillEnd> KillEnd { get; set; }
        public static ConfigEntry<int> KillDistanceThreshold { get; set; }
        private void Awake()
        {
            Debug.LogError("AmandsHitmarker Awake()");
            Hook = new GameObject();
            Hook.AddComponent<AmandsHitmarkerClass>();
            DontDestroyOnLoad(Hook);
        }
        private void Start()
        {
            EnableHitmarker = Config.Bind<bool>("AmandsHitmarker", "EnableHitmarker", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 420 }));
            EnableBleeding = Config.Bind<bool>("AmandsHitmarker", "EnableBleeding", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 410 }));

            HitmarkerPositionMode = Config.Bind<EHitmarkerPositionMode>("AmandsHitmarker", "Position Mode", EHitmarkerPositionMode.ImpactPoint, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 320 }));
            ADSHitmarkerPositionMode = Config.Bind<EHitmarkerPositionMode>("AmandsHitmarker", "ADS Position Mode", EHitmarkerPositionMode.GunDirection, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 310 }));

            Thickness = Config.Bind<Vector2>("AmandsHitmarker", "Thickness", new Vector2(40.0f, 40.0f), new ConfigDescription("Individual image size", null, new ConfigurationManagerAttributes { Order = 210 }));
            CenterOffset = Config.Bind<float>("AmandsHitmarker", "CenterOffset", 15.0f, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 200 }));
            AnimatedTime = Config.Bind<float>("AmandsHitmarker", "AnimatedTime", 0.25f, new ConfigDescription("Animation time duration", new AcceptableValueRange<float>(0.01f, 1.0f), new ConfigurationManagerAttributes { Order = 190 }));
            AnimatedAlphaTime = Config.Bind<float>("AmandsHitmarker", "AnimatedAlphaTime", 0.25f, new ConfigDescription("Alpha animation time duration", new AcceptableValueRange<float>(0.01f, 1.0f), new ConfigurationManagerAttributes { Order = 180 }));
            AnimatedAmplitude = Config.Bind<float>("AmandsHitmarker", "AnimatedAmplitude", 2.5f, new ConfigDescription("Animation size amplitude", new AcceptableValueRange<float>(1.0f, 10.0f), new ConfigurationManagerAttributes { Order = 178 }));
            Shape = Config.Bind<string>("AmandsHitmarker", "Shape", "Hitmarker.png", new ConfigDescription("Supported File PNG", null, new ConfigurationManagerAttributes { Order = 170 }));
            HeadshotShape = Config.Bind<string>("AmandsHitmarker", "HeadshotShape", "HeadshotHitmarker.png", new ConfigDescription("Supported File PNG", null, new ConfigurationManagerAttributes { Order = 160 }));
            BleedSize = Config.Bind<Vector2>("AmandsHitmarker", "BleedSize", new Vector2(128.0f, 128.0f), new ConfigDescription("Bleed kill glow image size", null, new ConfigurationManagerAttributes { Order = 150 }));
            EnableSounds = Config.Bind<bool>("AmandsHitmarker", "EnableSounds", true, new ConfigDescription("Supported Files WAV OGG", null, new ConfigurationManagerAttributes { Order = 140 }));
            SoundVolume = Config.Bind<float>("AmandsHitmarker", "SoundVolume", 1.0f, new ConfigDescription("", new AcceptableValueRange<float>(0.0f,4.0f), new ConfigurationManagerAttributes { Order = 136 }));
            HitmarkerSound = Config.Bind<string>("AmandsHitmarker", "HitmarkerSound", "Hitmarker.wav", new ConfigDescription("Supported Files WAV OGG", null, new ConfigurationManagerAttributes { Order = 130 }));
            HeadshotHitmarkerSound = Config.Bind<string>("AmandsHitmarker", "HeadshotHitmarkerSound", "HeadshotHitmarker.wav", new ConfigDescription("Supported Files WAV OGG", null, new ConfigurationManagerAttributes { Order = 120 }));
            KillHitmarkerSound = Config.Bind<string>("AmandsHitmarker", "KillHitmarkerSound", "KillHitmarker.wav", new ConfigDescription("Supported Files WAV OGG", null, new ConfigurationManagerAttributes { Order = 110 }));
            ArmorSound = Config.Bind<string>("AmandsHitmarker", "ArmorSound", "Armor.wav", new ConfigDescription("Supported Files WAV OGG", null, new ConfigurationManagerAttributes { Order = 106 }));
            ArmorBreakSound = Config.Bind<string>("AmandsHitmarker", "ArmorBreakSound", "ArmorBreak.wav", new ConfigDescription("Supported Files WAV OGG", null, new ConfigurationManagerAttributes { Order = 102 }));

            HitmarkerColor = Config.Bind<Color>("Colors", "HitmarkerColor", new Color(1.0f, 1.0f, 1.0f, 1.0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 180 }));
            ArmorColor = Config.Bind<Color>("Colors", "ArmorColor", new Color(0.2826087f, 0.6086956f, 1.0f, 1.0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 170 }));
            BearColor = Config.Bind<Color>("Colors", "BearColor", new Color(1.0f, 0.0f, 0.0f, 1.0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 160 }));
            UsecColor = Config.Bind<Color>("Colors", "UsecColor", new Color(1.0f, 0.0f, 0.1f, 1.0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 150 }));
            ScavColor = Config.Bind<Color>("Colors", "ScavColor", new Color(1.0f, 0.3043478f, 0.0f, 1.0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 140 }));
            ThrowWeaponColor = Config.Bind<Color>("Colors", "ThrowWeaponColor", new Color(0.4130435f, 0.0f, 1.0f, 1.0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 130 }));
            FollowerColor = Config.Bind<Color>("Colors", "FollowerColor", new Color(1.0f, 0.8043478f, 0.0f, 1.0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 124 }));
            BossColor = Config.Bind<Color>("Colors", "BossColor", new Color(1.0f, 0.8043478f, 0.0f, 1.0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 120 }));
            BleedColor = Config.Bind<Color>("Colors", "BleedColor", new Color(1.0f, 0.0f, 0.0f, 0.69f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 110 }));
            PoisonColor = Config.Bind<Color>("Colors", "PoisonColor", new Color(0.0f, 1.0f, 0.0f, 0.69f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 106, IsAdvanced = true }));

            HitmarkerDebug = Config.Bind<bool>("Debug", "HitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 230, IsAdvanced = true }));
            HeadshotHitmarkerDebug = Config.Bind<bool>("Debug", "HeadshotHitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 220, IsAdvanced = true }));
            BleedHitmarkerDebug = Config.Bind<bool>("Debug", "BleedHitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 210, IsAdvanced = true }));
            PoisonHitmarkerDebug = Config.Bind<bool>("Debug", "PoisonHitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 208, IsAdvanced = true }));
            ArmorHitmarkerDebug = Config.Bind<bool>("Debug", "ArmorHitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 200, IsAdvanced = true }));
            BearHitmarkerDebug = Config.Bind<bool>("Debug", "BearHitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 190, IsAdvanced = true }));
            UsecHitmarkerDebug = Config.Bind<bool>("Debug", "UsecHitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 180, IsAdvanced = true }));
            ScavHitmarkerDebug = Config.Bind<bool>("Debug", "ScavHitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 170, IsAdvanced = true }));
            ThrowWeaponHitmarkerDebug = Config.Bind<bool>("Debug", "ThrowWeaponHitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 160, IsAdvanced = true }));
            FollowerHitmarkerDebug = Config.Bind<bool>("Debug", "FollowerHitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 154, IsAdvanced = true }));
            BossHitmarkerDebug = Config.Bind<bool>("Debug", "BossHitmarkerDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 150, IsAdvanced = true }));
            HitmarkerSoundDebug = Config.Bind<bool>("Debug", "HitmarkerSoundDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 140, IsAdvanced = true }));
            HeadshotHitmarkerSoundDebug = Config.Bind<bool>("Debug", "HeadshotHitmarkerSoundDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 130, IsAdvanced = true }));
            ArmorSoundDebug = Config.Bind<bool>("Debug", "ArmorSoundDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 128, IsAdvanced = true }));
            ArmorBreakSoundDebug = Config.Bind<bool>("Debug", "ArmorBreakSoundDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 126, IsAdvanced = true }));
            KillHitmarkerSoundDebug = Config.Bind<bool>("Debug", "KillHitmarkerSoundDebug", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 120, IsAdvanced = true }));
            ReloadFiles = Config.Bind<bool>("Debug", "ReloadFiles", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 110, IsAdvanced = true }));
            ReloadBattleUIScreen = Config.Bind<bool>("Debug", "ReloadBattleUIScreen", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 100, IsAdvanced = true }));

            StaticHitmarkerOnly = Config.Bind<bool>("Static", "StaticHitmarkerOnly", false, new ConfigDescription("Use only the static hitmarker image", null, new ConfigurationManagerAttributes { Order = 140, IsAdvanced = true }));
            StaticSizeDelta = Config.Bind<Vector2>("Static", "StaticSizeDelta", new Vector2(200.0f, 200.0f), new ConfigDescription("Static hitmarker size", null, new ConfigurationManagerAttributes { Order = 130, IsAdvanced = true }));
            StaticSizeDeltaSpeed = Config.Bind<float>("Static", "StaticSizeDeltaSpeed", 0.01f, new ConfigDescription("Animation size increase", null, new ConfigurationManagerAttributes { Order = 120, IsAdvanced = true }));
            StaticOpacity = Config.Bind<float>("Static", "StaticOpacity", 0.25f, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 110, IsAdvanced = true }));


            EnableKillfeed = Config.Bind<bool>("AmandsKillfeed", "EnableKillfeed", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 290 }));
            KillTextColor = Config.Bind<Color>("AmandsKillfeed", "TextColor", new Color(0.84f, 0.88f, 0.95f, 1f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 280, IsAdvanced = true }));
            KillFontSize = Config.Bind<int>("AmandsKillfeed", "FontSize", 26, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 270 }));
            KillFontOutline = Config.Bind<float>("AmandsKillfeed", "FontOutline", 0.01f, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 260, IsAdvanced = true }));
            KillFontUpperCase = Config.Bind<bool>("AmandsKillfeed", "FontUpperCase", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 150, IsAdvanced = true }));
            KillChildSpacing = Config.Bind<int>("AmandsKillfeed", "TextSpacing", -26, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 240 }));
            KillPreset = Config.Bind<EKillPreset>("AmandsKillfeed", "Preset", EKillPreset.Center, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 230 }));
            KillRectPosition = Config.Bind<Vector2>("AmandsKillfeed", "RectPosition", new Vector2(0f, -250f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 220 }));
            KillRectPivot = Config.Bind<Vector2>("AmandsKillfeed", "RectPivot", new Vector2(0.5f, 1f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 210, IsAdvanced = true }));
            KillChildDirection = Config.Bind<bool>("AmandsKillfeed", "Invert TextDirection", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 200, IsAdvanced = true }));
            KillTextAlignment = Config.Bind<TextAlignmentOptions>("AmandsKillfeed", "TextAlignment", TextAlignmentOptions.Right, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 190, IsAdvanced = true }));
            KillTime = Config.Bind<float>("AmandsKillfeed", "Time", 2f, new ConfigDescription("", new AcceptableValueRange<float>(0.1f, 20.0f), new ConfigurationManagerAttributes { Order = 180 }));
            KillOpacitySpeed = Config.Bind<float>("AmandsKillfeed", "OpacitySpeed", 0.08f, new ConfigDescription("", new AcceptableValueRange<float>(0.01f,1.0f), new ConfigurationManagerAttributes { Order = 170 }));
            KillUpperText = Config.Bind<bool>("AmandsKillfeed", "EnableUpperText", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 160, IsAdvanced = true }));
            KillStart = Config.Bind<EKillStart>("AmandsKillfeed", "Start", EKillStart.Weapon, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 150 }));
            KillNameColor = Config.Bind<EKillNameColor>("AmandsKillfeed", "Name", EKillNameColor.Colored, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 140}));
            KillNameSingleColor = Config.Bind<Color>("AmandsKillfeed", "SingleColor", new Color(1.0f, 0.0f, 0.0f, 1.0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 130, IsAdvanced = true }));
            KillEnd = Config.Bind<EKillEnd>("AmandsKillfeed", "End", EKillEnd.Experience, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 120 }));
            KillDistanceThreshold = Config.Bind<int>("AmandsKillfeed", "DistanceThreshold", 50, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 110 }));

            new AmandsDamagePatch().Enable();
            new AmandsArmorDamagePatch().Enable();
            new AmandsKillPatch().Enable();

            AmandsHitmarkerHelper.Init();
        }
    }
    public class AmandsDamagePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("ApplyDamageInfo", BindingFlags.Instance | BindingFlags.Public);
        }
        [PatchPostfix]
        private static void PatchPostFix(ref Player __instance, DamageInfo damageInfo, EBodyPart bodyPartType)
        {
            if (AmandsHitmarkerClass.localPlayer != null && damageInfo.Player == AmandsHitmarkerClass.localPlayer)
            {
                AmandsHitmarkerClass.hitmarker = true;
                AmandsHitmarkerClass.damageInfo = damageInfo;
                AmandsHitmarkerClass.bodyPart = bodyPartType;
            }
        }
    }
    public class AmandsArmorDamagePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ArmorComponent).GetMethod("ApplyDamage", BindingFlags.Instance | BindingFlags.Public);
        }
        [PatchPostfix]
        private static void PatchPostFix(ref ArmorComponent __instance, DamageInfo damageInfo, float armorDamage)
        {
            if (AmandsHitmarkerClass.localPlayer != null && damageInfo.Player == AmandsHitmarkerClass.localPlayer && armorDamage > 0)
            {
                AmandsHitmarkerClass.armorHitmarker = true;
                AmandsHitmarkerClass.armorDamage = armorDamage;
                AmandsHitmarkerClass.armorDamageInfo = damageInfo;
                if (__instance.Repairable.Durability == 0)
                {
                    AmandsHitmarkerClass.armorBreak = true;
                }
            }
        }
    }
    public class AmandsKillPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("OnBeenKilledByAggressor", BindingFlags.Instance | BindingFlags.NonPublic);
        }
        [PatchPostfix]
        private static void PatchPostFix(ref Player __instance, Player aggressor, DamageInfo damageInfo, EBodyPart bodyPart, EDamageType lethalDamageType)
        {
            if (AmandsHitmarkerClass.localPlayer != null && aggressor == AmandsHitmarkerClass.localPlayer && __instance != AmandsHitmarkerClass.localPlayer)
            {
                AmandsHitmarkerClass.killHitmarker = true;
                AmandsHitmarkerClass.killDamageInfo = damageInfo;
                AmandsHitmarkerClass.killBodyPart = bodyPart;
                AmandsHitmarkerClass.killRole = Traverse.Create(Traverse.Create(__instance.Profile.Info).Field("Settings").GetValue<object>()).Field("Role").GetValue<WildSpawnType>();
                AmandsHitmarkerClass.killPlayerName = __instance.Profile.Nickname;
                AmandsHitmarkerClass.killPlayerSide = __instance.Side;
                AmandsHitmarkerClass.killDistance = Vector3.Distance(aggressor.Position, __instance.Position);
                AmandsHitmarkerClass.lethalDamageType = lethalDamageType;
                AmandsHitmarkerClass.killLevel = __instance.Profile.Info.Level;
                AmandsHitmarkerClass.killWeaponName = AmandsHitmarkerHelper.Localized(damageInfo.Weapon.ShortName,0);
            }
        }
    }
}
