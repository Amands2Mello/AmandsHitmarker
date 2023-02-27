using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using EFT;
using Comfort.Common;
using TMPro;
using static EFT.Player;
using HarmonyLib;
using EFT.UI;
using EFT.Counters;
using System.Data;

namespace AmandsHitmarker
{
    public class AmandsHitmarkerClass : MonoBehaviour
    {
        public static GameObject killListGameObject;
        public static AmandsAnimatedText LastAmandsAnimatedText;
        public static bool hitmarker;
        public static DamageInfo damageInfo = new DamageInfo();
        public static EBodyPart bodyPart = EBodyPart.Chest;
        public static bool armorHitmarker;
        //public static float armorDamage;
        //public static DamageInfo armorDamageInfo;
        public static bool armorBreak;
        public static bool killHitmarker;
        public static DamageInfo killDamageInfo = new DamageInfo();
        public static EPlayerSide killPlayerSide;
        public static EBodyPart killBodyPart = EBodyPart.Chest;
        public static WildSpawnType killRole;
        public static string killPlayerName;
        public static int killExperience;
        public static float killDistance;
        public static EDamageType lethalDamageType;
        public static int killLevel;
        public static string killWeaponName;
        private static RectTransform rectTransform;
        private static VerticalLayoutGroup verticalLayoutGroup;

        public static int Kills;
        public static int VictimLevelExp;
        public static int VictimBotLevelExp;
        public static float HeadShotMult;
        public static float LongShotDistance;
        public static List<int> Combo = new List<int>();

        public static GameObject ActiveUIScreen;
        private static Dictionary<string, Sprite> LoadedSprites = new Dictionary<string, Sprite>();
        private static Dictionary<string, AudioClip> LoadedAudioClips = new Dictionary<string, AudioClip>();
        private static Sprite sprite;
        public static LocalPlayer localPlayer;
        public static GameObject PlayerSuperior;
        public static FirearmController firearmController;
        public static SSAA FPSCameraSSAA;
        private static float FPSCameraSSAARatio;
        private static GameObject TLH;
        private static GameObject TRH;
        private static GameObject BLH;
        private static GameObject BRH;
        private static RectTransform TLHRect;
        private static RectTransform TRHRect;
        private static RectTransform BLHRect;
        private static RectTransform BRHRect;
        private static Image TLHImage;
        private static Image TRHImage;
        private static Image BLHImage;
        private static Image BRHImage;
        private static Vector3 TLHOffset = new Vector3(-1, 1, 0);
        private static Vector3 TRHOffset = new Vector3(1, 1, 0);
        private static Vector3 BLHOffset = new Vector3(-1, -1, 0);
        private static Vector3 BRHOffset = new Vector3(1, -1, 0);
        private static Vector3 TLHOffsetAnim = Vector3.zero;
        private static Vector3 TRHOffsetAnim = Vector3.zero;
        private static Vector3 BLHOffsetAnim = Vector3.zero;
        private static Vector3 BRHOffsetAnim = Vector3.zero;
        private static GameObject BleedHitmarker;
        private static RectTransform BleedRect;
        private static Image BleedImage;
        private static GameObject StaticHitmarker;
        private static Image StaticHitmarkerImage;
        private static RectTransform StaticHitmarkerRect;
        private static GameObject ArmorHitmarker;
        private static Image ArmorHitmarkerImage;
        private static RectTransform ArmorHitmarkerRect;
        private static bool ForceHitmarkerPosition = false;
        private static Vector3 HitmarkerPosition = Vector3.zero;
        private static Vector3 HitmarkerPositionSnapshot = Vector3.zero;
        private static Vector3 position = Vector3.zero;
        private static Vector3 weaponDirection = Vector3.zero;

        private static bool UpdateHitmarker = false;
        private static float HitmarkerTime = 0.0f;
        private static float HitmarkerOpacity = 0.0f;
        private static float HitmarkerCenterOffset = 0.0f;
        private static Color HitmarkerColor = new Color(1.0f, 1.0f, 1.0f);
        private static Color BleedColor = new Color(1.0f, 0.0f, 0.0f);
        private static AudioClip audioClip;
        private static Color ArmorHitmarkerColor = new Color(1.0f, 1.0f, 1.0f);

        private static bool DebugMode = false;
        private static Vector3 DebugOffset = Vector3.zero;
        private static List<string> DebugWeapons = new List<string>() { "RD-704", "MGSL", "P90", "MCX .300 BLK", "G36 E", "FN 5-7", "AXMC", "SV-98" };
        private static List<string> DebugNames = new List<string>() { "Mellone", "1234567890123456", "SuperLongName", "1", "12", "123", "aaaaa", "AAAAAAAAAAAAAA", "Debug", "Test" };

        private static AnimationCurve animationCurve = new AnimationCurve();
        private static Keyframe[] keys;// = { new Keyframe(0f, 0f, 0f, 0f, 0.25f, 0.25f), new Keyframe(0.5f, 1f, 0f, 0f, 0.5f, 0.5f), new Keyframe(1f, 0f, 0f, 0f, 0.25f, 0.25f) };
        private static AnimationCurve AlphaAnimationCurve = new AnimationCurve();
        private static Keyframe[] AlphaKeys;// = { new Keyframe(1f, 1f, 0f, 0f, 0.25f, 0.25f), new Keyframe(1.5f, 0f, 0f, 0f, 0.25f, 0.25f) };
        public static void XPFormula()
        {
            BackendConfigSettingsClass backendConfigSettingsClass = Singleton<BackendConfigSettingsClass>.Instance;
            if (backendConfigSettingsClass != null)
            {
                object Experience = Traverse.Create(backendConfigSettingsClass).Field("Experience").GetValue<object>();
                if (Experience != null)
                {
                    object Kill = Traverse.Create(Experience).Field("Kill").GetValue<object>();
                    if (Kill != null)
                    {
                        VictimLevelExp = Traverse.Create(Kill).Field("VictimLevelExp").GetValue<int>();
                        VictimBotLevelExp = Traverse.Create(Kill).Field("VictimBotLevelExp").GetValue<int>();
                        HeadShotMult = Traverse.Create(Kill).Field("HeadShotMult").GetValue<float>();
                        LongShotDistance = Traverse.Create(Kill).Field("LongShotDistance").GetValue<float>();
                        object[] combo = Traverse.Create(Kill).Field("Combo").GetValue<object[]>();
                        Combo.Clear();
                        foreach (object c in combo)
                        {
                            Combo.Add(Traverse.Create(c).Field("Percent").GetValue<int>());
                        }
                    }
                }
            }
        }
        public int GetKillingBonusPercent(int killed)
        {
            int num = Mathf.Clamp(killed - 1, 0, Combo.Count - 1);
            return Combo[num];
        }
        public void Start()
        {
            animationCurve.keys = keys;
            AlphaAnimationCurve.keys = AlphaKeys;

            AHitmarkerPlugin.EnableHitmarker.SettingChanged += HitmarkerDebug;
            AHitmarkerPlugin.EnableArmorHitmarker.SettingChanged += ArmorHitmarkerDebug;
            AHitmarkerPlugin.EnableBleeding.SettingChanged += BleedHitmarkerDebug;
            AHitmarkerPlugin.Thickness.SettingChanged += HitmarkerDebug;
            AHitmarkerPlugin.CenterOffset.SettingChanged += HitmarkerDebug;
            AHitmarkerPlugin.ArmorOffset.SettingChanged += ArmorHitmarkerDebug;
            AHitmarkerPlugin.ArmorSizeDelta.SettingChanged += ArmorHitmarkerDebug;
            AHitmarkerPlugin.AnimatedTime.SettingChanged += UpdateHitmarkerAnimation;
            AHitmarkerPlugin.AnimatedAlphaTime.SettingChanged += UpdateHitmarkerAnimation;
            AHitmarkerPlugin.AnimatedAmplitude.SettingChanged += UpdateHitmarkerAnimation;
            AHitmarkerPlugin.BleedSize.SettingChanged += BleedHitmarkerDebug;

            AHitmarkerPlugin.HitmarkerColor.SettingChanged += HitmarkerDebug;
            AHitmarkerPlugin.ArmorColor.SettingChanged += ArmorHitmarkerDebug;
            AHitmarkerPlugin.BearColor.SettingChanged += BearHitmarkerDebug;
            AHitmarkerPlugin.UsecColor.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.ScavColor.SettingChanged += ScavHitmarkerDebug;
            AHitmarkerPlugin.ThrowWeaponColor.SettingChanged += ThrowWeaponHitmarkerDebug;
            AHitmarkerPlugin.FollowerColor.SettingChanged += FollowerHitmarkerDebug;
            AHitmarkerPlugin.BossColor.SettingChanged += BossHitmarkerDebug;
            AHitmarkerPlugin.BleedColor.SettingChanged += BleedHitmarkerDebug;
            AHitmarkerPlugin.PoisonColor.SettingChanged += PoisonHitmarkerDebug;

            AHitmarkerPlugin.HitmarkerDebug.SettingChanged += HitmarkerDebug;
            AHitmarkerPlugin.HeadshotHitmarkerDebug.SettingChanged += HeadshotHitmarkerDebug; 
            AHitmarkerPlugin.BleedHitmarkerDebug.SettingChanged += BleedHitmarkerDebug;
            AHitmarkerPlugin.PoisonHitmarkerDebug.SettingChanged += PoisonHitmarkerDebug;
            AHitmarkerPlugin.ArmorHitmarkerDebug.SettingChanged += ArmorHitmarkerDebug;
            AHitmarkerPlugin.ArmorBreakHitmarkerDebug.SettingChanged += ArmorBreakHitmarkerDebug;
            AHitmarkerPlugin.BearHitmarkerDebug.SettingChanged += BearHitmarkerDebug;
            AHitmarkerPlugin.UsecHitmarkerDebug.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.ScavHitmarkerDebug.SettingChanged += ScavHitmarkerDebug;
            AHitmarkerPlugin.ThrowWeaponHitmarkerDebug.SettingChanged += ThrowWeaponHitmarkerDebug;
            AHitmarkerPlugin.FollowerHitmarkerDebug.SettingChanged += FollowerHitmarkerDebug;
            AHitmarkerPlugin.BossHitmarkerDebug.SettingChanged += BossHitmarkerDebug;
            AHitmarkerPlugin.HitmarkerSoundDebug.SettingChanged += HitmarkerSoundDebug;
            AHitmarkerPlugin.HeadshotHitmarkerSoundDebug.SettingChanged += HeadshotHitmarkerSoundDebug;
            AHitmarkerPlugin.ArmorSoundDebug.SettingChanged += ArmorSoundDebug;
            AHitmarkerPlugin.ArmorBreakSoundDebug.SettingChanged += ArmorBreakSoundDebug;
            AHitmarkerPlugin.KillHitmarkerSoundDebug.SettingChanged += KillHitmarkerSoundDebug;
            AHitmarkerPlugin.ReloadFiles.SettingChanged += ReloadFilesDebug;
            AHitmarkerPlugin.ReloadBattleUIScreen.SettingChanged += ReloadBattleUIScreen;

            AHitmarkerPlugin.EnableKillfeed.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.KillTextColor.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.KillFontSize.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.KillFontOutline.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.KillFontUpperCase.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.KillChildSpacing.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.KillPreset.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.KillRectPosition.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.KillRectPivot.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.KillChildDirection.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.KillTextAlignment.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.KillOpacitySpeed.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.KillUpperText.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.KillStart.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.KillNameColor.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.KillNameSingleColor.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.KillEnd.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.KillDistanceThreshold.SettingChanged += UsecHitmarkerDebug;

            AHitmarkerPlugin.KillChildSpacing.SettingChanged += UpdateKillfeed;
            AHitmarkerPlugin.KillPreset.SettingChanged += UpdateKillPreset;
            AHitmarkerPlugin.KillRectPosition.SettingChanged += UpdateKillfeed;
            AHitmarkerPlugin.KillRectPivot.SettingChanged += UpdateKillfeed;

            keys = new Keyframe[] { new Keyframe(0f, 0f, 0f, 0f, 0.25f, 0.25f), new Keyframe(AHitmarkerPlugin.AnimatedTime.Value / 2, AHitmarkerPlugin.AnimatedAmplitude.Value, 0f, 0f, 0.5f, 0.5f), new Keyframe(AHitmarkerPlugin.AnimatedTime.Value, 0f, 0f, 0f, 0.25f, 0.25f) };
            AlphaKeys = new Keyframe[] { new Keyframe(AHitmarkerPlugin.AnimatedTime.Value, 1f, 0f, 0f, 0.25f, 0.25f), new Keyframe(AHitmarkerPlugin.AnimatedTime.Value + AHitmarkerPlugin.AnimatedAlphaTime.Value, 0f, 0f, 0f, 0.25f, 0.25f) };
            animationCurve.keys = keys;
            AlphaAnimationCurve.keys = AlphaKeys;

            ReloadFiles();
        }
        public void UpdateHitmarkerAnimation(object sender, EventArgs e)
        {
            keys = new Keyframe[] { new Keyframe(0f, 0f, 0f, 0f, 0.25f, 0.25f), new Keyframe(AHitmarkerPlugin.AnimatedTime.Value / 2, AHitmarkerPlugin.AnimatedAmplitude.Value, 0f, 0f, 0.5f, 0.5f), new Keyframe(AHitmarkerPlugin.AnimatedTime.Value, 0f, 0f, 0f, 0.25f, 0.25f) };
            AlphaKeys = new Keyframe[] { new Keyframe(AHitmarkerPlugin.AnimatedTime.Value, 1f, 0f, 0f, 0.25f, 0.25f), new Keyframe(AHitmarkerPlugin.AnimatedTime.Value + AHitmarkerPlugin.AnimatedAlphaTime.Value, 0f, 0f, 0f, 0.25f, 0.25f) };
            animationCurve.keys = keys;
            AlphaAnimationCurve.keys = AlphaKeys;
            HitmarkerDebug(null,null);
        }
        public void Update()
        {
            if ((hitmarker || killHitmarker) && ActiveUIScreen != null)
            {
                ForceHitmarkerPosition = false;
                bool tmpHitmarker = hitmarker;
                hitmarker = false;
                bool tmpArmorHitmarker = armorHitmarker;
                armorHitmarker = false;
                bool tmpArmorBreak = armorBreak;
                armorBreak = false;
                bool tmpKillHitmarker = killHitmarker;
                killHitmarker = false;

                if (!AHitmarkerPlugin.EnableBleeding.Value && (tmpKillHitmarker && !tmpHitmarker)) return;

                if (AHitmarkerPlugin.EnableSounds.Value && !DebugMode)
                {
                    if (LoadedAudioClips.ContainsKey(AHitmarkerPlugin.HitmarkerSound.Value))
                    {
                        audioClip = LoadedAudioClips[AHitmarkerPlugin.HitmarkerSound.Value];
                    }
                }
                HitmarkerColor = AHitmarkerPlugin.HitmarkerColor.Value;
                if (bodyPart == EBodyPart.Head)
                {
                    if (LoadedSprites.ContainsKey(AHitmarkerPlugin.HeadshotShape.Value))
                    {
                        sprite = LoadedSprites[AHitmarkerPlugin.HeadshotShape.Value];
                    }
                    StaticHitmarkerImage.sprite = LoadedSprites["StaticHeadshotHitmarker.png"];
                }
                else
                {
                    if (LoadedSprites.ContainsKey(AHitmarkerPlugin.Shape.Value))
                    {
                        sprite = LoadedSprites[AHitmarkerPlugin.Shape.Value];
                    }
                }
                if (tmpHitmarker)
                {
                    switch (damageInfo.DamageType)
                    {
                        case EDamageType.GrenadeFragment:
                        case EDamageType.Explosion:
                            ForceHitmarkerPosition = true;
                            break;
                    }
                }
                ArmorHitmarkerColor = Color.clear;
                if (tmpArmorHitmarker && !tmpKillHitmarker)
                {
                    HitmarkerColor = AHitmarkerPlugin.ArmorColor.Value;
                    if (AHitmarkerPlugin.EnableArmorHitmarker.Value != EArmorHitmarker.Disabled)
                    {
                        if (tmpArmorBreak)
                        {
                            ArmorHitmarkerColor = AHitmarkerPlugin.ArmorColor.Value;
                            ArmorHitmarkerImage.sprite = LoadedSprites[AHitmarkerPlugin.ArmorBreakShape.Value];
                        }
                        else if (AHitmarkerPlugin.EnableArmorHitmarker.Value != EArmorHitmarker.BreakingOnly)
                        {
                            ArmorHitmarkerColor = AHitmarkerPlugin.HitmarkerColor.Value;
                            ArmorHitmarkerImage.sprite = LoadedSprites[AHitmarkerPlugin.ArmorShape.Value];
                        }
                        ArmorHitmarkerRect.sizeDelta = AHitmarkerPlugin.ArmorSizeDelta.Value;
                    }
                    if (tmpArmorBreak && AHitmarkerPlugin.EnableSounds.Value && !DebugMode)
                    {
                        audioClip = LoadedAudioClips[AHitmarkerPlugin.ArmorBreakSound.Value];
                    }
                    else if (AHitmarkerPlugin.EnableSounds.Value && !DebugMode)
                    {
                        audioClip = LoadedAudioClips[AHitmarkerPlugin.ArmorSound.Value];
                    }
                }
                BleedColor = Color.clear;
                if (tmpKillHitmarker)
                {
                    switch (killPlayerSide)
                    {
                        case EPlayerSide.Usec:
                            HitmarkerColor = AHitmarkerPlugin.UsecColor.Value;
                            break;
                        case EPlayerSide.Bear:
                            HitmarkerColor = AHitmarkerPlugin.BearColor.Value;
                            break;
                        case EPlayerSide.Savage:
                            HitmarkerColor = AHitmarkerPlugin.ScavColor.Value;
                            break;
                    }
                    switch (lethalDamageType)
                    {
                        case EDamageType.GrenadeFragment:
                        case EDamageType.Explosion:
                            HitmarkerColor = AHitmarkerPlugin.ThrowWeaponColor.Value;
                            ForceHitmarkerPosition = true;
                            break;
                    }
                    if (killPlayerSide == EPlayerSide.Savage)
                    {
                        if (AmandsHitmarkerHelper.IsFollower(killRole)) HitmarkerColor = AHitmarkerPlugin.FollowerColor.Value;
                        if (AmandsHitmarkerHelper.IsBoss(killRole) || AmandsHitmarkerHelper.CountAsBoss(killRole)) HitmarkerColor = AHitmarkerPlugin.BossColor.Value;
                    }
                    if (AHitmarkerPlugin.KillChildDirection.Value)
                    {
                        CreateKillText();
                    }
                    string UpperText = "";
                    switch (lethalDamageType)
                    {
                        case EDamageType.LightBleeding:
                        case EDamageType.HeavyBleeding:
                            BleedColor = AHitmarkerPlugin.BleedColor.Value;
                            BleedRect.sizeDelta = AHitmarkerPlugin.BleedSize.Value;
                            BleedRect.localPosition = DebugOffset;
                            if (AHitmarkerPlugin.KillUpperText.Value)
                            {
                                UpperText = "<color=#" + ColorUtility.ToHtmlStringRGB(AHitmarkerPlugin.BleedColor.Value) + ">" + "BLEEDING" + "</color> ";
                            }
                            ForceHitmarkerPosition = true;
                            break;
                        case EDamageType.LethalToxin:
                        case EDamageType.Poison:
                            BleedColor = AHitmarkerPlugin.PoisonColor.Value;
                            BleedRect.sizeDelta = AHitmarkerPlugin.BleedSize.Value;
                            BleedRect.localPosition = DebugOffset;
                            if (AHitmarkerPlugin.KillUpperText.Value)
                            {
                                UpperText = "<color=#" + ColorUtility.ToHtmlStringRGB(AHitmarkerPlugin.PoisonColor.Value) + ">" + "POISON" + "</color> ";
                            }
                            ForceHitmarkerPosition = true;
                            break;
                    }
                    if (bodyPart == EBodyPart.Head && AHitmarkerPlugin.KillUpperText.Value)
                    {
                        if (AHitmarkerPlugin.KillHeadshotXP.Value == EHeadshotXP.On)
                        {
                            float BaseExp = 0;
                            switch (killPlayerSide)
                            {
                                case EPlayerSide.Usec:
                                    BaseExp = VictimLevelExp;
                                    break;
                                case EPlayerSide.Bear:
                                    BaseExp = VictimLevelExp;
                                    break;
                                case EPlayerSide.Savage:
                                    BaseExp = killExperience;
                                    if (BaseExp < 0)
                                    {
                                        BaseExp = VictimBotLevelExp;
                                    }
                                    break;
                            }
                            UpperText = UpperText + "HEADSHOT " + (int)(BaseExp * Mathf.Max(HeadShotMult - 1f, 0)) + "XP";
                        }
                        else
                        {
                            UpperText = UpperText + "HEADSHOT";
                        }
                    }
                    if (UpperText != "")
                    {
                        CreateUpperText(UpperText, (int)(AHitmarkerPlugin.KillFontSize.Value * 0.75), AHitmarkerPlugin.KillTime.Value, AHitmarkerPlugin.KillOpacitySpeed.Value);
                    }
                    if (bodyPart == EBodyPart.Head && AHitmarkerPlugin.KillUpperText.Value && killDistance >= LongShotDistance)
                    {
                        CreateUpperText("LONGSHOT", (int)(AHitmarkerPlugin.KillFontSize.Value * 0.75), AHitmarkerPlugin.KillTime.Value, AHitmarkerPlugin.KillOpacitySpeed.Value);
                    }
                    if (!AHitmarkerPlugin.KillChildDirection.Value)
                    {
                        CreateKillText();
                    }
                    if (AHitmarkerPlugin.EnableSounds.Value && !DebugMode)
                    {
                        if (killBodyPart == EBodyPart.Head && LoadedAudioClips.ContainsKey(AHitmarkerPlugin.HeadshotHitmarkerSound.Value))
                        {
                            audioClip = LoadedAudioClips[AHitmarkerPlugin.HeadshotHitmarkerSound.Value];
                        }
                        else if (LoadedAudioClips.ContainsKey(AHitmarkerPlugin.KillHitmarkerSound.Value))
                        {
                            audioClip = LoadedAudioClips[AHitmarkerPlugin.KillHitmarkerSound.Value];
                        }
                    }
                }
                if (AHitmarkerPlugin.EnableHitmarker.Value)
                {
                    if (firearmController == null && PlayerSuperior != null)
                    {
                        firearmController = PlayerSuperior.GetComponent<FirearmController>();
                    }
                    if (Camera.main != null)
                    {
                        FPSCameraSSAARatio = 1f;
                        if (FPSCameraSSAA != null && (FPSCameraSSAA.UsesDLSSUpscaler() || FPSCameraSSAA.UsesFSRUpscaler()))
                        {
                            FPSCameraSSAARatio = (float)FPSCameraSSAA.GetOutputHeight() / (float)FPSCameraSSAA.GetInputHeight();
                        }
                        Vector2 ScreenPointSnapshot = Camera.main.WorldToScreenPoint(damageInfo.HitPoint) * FPSCameraSSAARatio;
                        HitmarkerPositionSnapshot = new Vector2(ScreenPointSnapshot.x - (Screen.width / 2), ScreenPointSnapshot.y - (Screen.height / 2));
                    }
                    TLHImage.sprite = sprite; TRHImage.sprite = sprite;
                    BLHImage.sprite = sprite; BRHImage.sprite = sprite;
                    TLHRect.sizeDelta = AHitmarkerPlugin.Thickness.Value; TRHRect.sizeDelta = AHitmarkerPlugin.Thickness.Value;
                    BLHRect.sizeDelta = AHitmarkerPlugin.Thickness.Value; BRHRect.sizeDelta = AHitmarkerPlugin.Thickness.Value;
                    StaticHitmarkerRect.sizeDelta = AHitmarkerPlugin.StaticSizeDelta.Value;

                    HitmarkerCenterOffset = AHitmarkerPlugin.CenterOffset.Value;

                    HitmarkerPosition = Vector3.zero;
                    HitmarkerOpacity = 1.0f;
                    HitmarkerTime = 0.0f;
                    UpdateHitmarker = true;
                }
                if (AHitmarkerPlugin.EnableSounds.Value && Singleton<BetterAudio>.Instance != null && !DebugMode)
                {
                    Singleton<BetterAudio>.Instance.PlayNonspatial(audioClip, BetterAudio.AudioSourceGroupType.NonspatialBypass, 0.0f, AHitmarkerPlugin.SoundVolume.Value);
                }
            }
            if (UpdateHitmarker)
            {
                HitmarkerTime += Time.deltaTime;
                HitmarkerOpacity = AlphaAnimationCurve.Evaluate(HitmarkerTime);
                HitmarkerCenterOffset = AHitmarkerPlugin.CenterOffset.Value + animationCurve.Evaluate(HitmarkerTime);
                if (firearmController != null && !DebugMode)
                {
                    EHitmarkerPositionMode HitmarkerPositionMode = firearmController.IsAiming ? AHitmarkerPlugin.ADSHitmarkerPositionMode.Value : AHitmarkerPlugin.HitmarkerPositionMode.Value;
                    if (ForceHitmarkerPosition) HitmarkerPositionMode = EHitmarkerPositionMode.Center;
                    switch (HitmarkerPositionMode)
                    {
                        case EHitmarkerPositionMode.Center:
                            HitmarkerPosition = Vector3.zero;
                            break;
                        case EHitmarkerPositionMode.GunDirection:
                            position = firearmController.CurrentFireport.position;
                            weaponDirection = firearmController.WeaponDirection;
                            firearmController.AdjustShotVectors(ref position, ref weaponDirection);
                            Vector2 ScreenPoint = Camera.main.WorldToScreenPoint(position + (weaponDirection * 100)) * FPSCameraSSAARatio;
                            HitmarkerPosition = new Vector2(ScreenPoint.x - (Screen.width / 2), ScreenPoint.y - (Screen.height / 2));
                            break;
                        case EHitmarkerPositionMode.ImpactPoint:
                            Vector2 ScreenPoint2 = Camera.main.WorldToScreenPoint(damageInfo.HitPoint) * FPSCameraSSAARatio;
                            HitmarkerPosition = new Vector2(ScreenPoint2.x - (Screen.width / 2), ScreenPoint2.y - (Screen.height / 2));
                            break;
                        case EHitmarkerPositionMode.ImpactPointStatic:
                            HitmarkerPosition = HitmarkerPositionSnapshot;
                            break;
                    }
                }
                if (AHitmarkerPlugin.StaticHitmarkerOnly.Value)
                {
                    StaticHitmarkerImage.color = new Color(HitmarkerColor.r, HitmarkerColor.g, HitmarkerColor.b, HitmarkerColor.a * HitmarkerOpacity * AHitmarkerPlugin.StaticOpacity.Value);
                    StaticHitmarkerRect.sizeDelta += Vector2.one * AHitmarkerPlugin.StaticSizeDeltaSpeed.Value;
                    StaticHitmarkerRect.localPosition = DebugOffset + HitmarkerPosition;
                    TLHImage.color = Color.clear; TRHImage.color = Color.clear; 
                    BLHImage.color = Color.clear; BRHImage.color = Color.clear;
                    BleedImage.color = new Color(BleedColor.r, BleedColor.g, BleedColor.b, BleedColor.a * HitmarkerOpacity);
                    ArmorHitmarkerRect.localPosition = DebugOffset + HitmarkerPosition + AHitmarkerPlugin.ArmorOffset.Value;
                    ArmorHitmarkerImage.color = new Color(ArmorHitmarkerColor.r, ArmorHitmarkerColor.g, ArmorHitmarkerColor.b, ArmorHitmarkerColor.a * HitmarkerOpacity);
                }
                else
                {
                    StaticHitmarkerRect.sizeDelta += Vector2.one * AHitmarkerPlugin.StaticSizeDeltaSpeed.Value;
                    TLHOffsetAnim = TLHOffset * HitmarkerCenterOffset;
                    TRHOffsetAnim = TRHOffset * HitmarkerCenterOffset;
                    BLHOffsetAnim = BLHOffset * HitmarkerCenterOffset;
                    BRHOffsetAnim = BRHOffset * HitmarkerCenterOffset;
                    TLHRect.localPosition = DebugOffset + HitmarkerPosition + TLHOffsetAnim;
                    TRHRect.localPosition = DebugOffset + HitmarkerPosition + TRHOffsetAnim;
                    BLHRect.localPosition = DebugOffset + HitmarkerPosition + BLHOffsetAnim;
                    BRHRect.localPosition = DebugOffset + HitmarkerPosition + BRHOffsetAnim;
                    BleedRect.localPosition = DebugOffset + HitmarkerPosition;
                    StaticHitmarkerRect.localPosition = DebugOffset + HitmarkerPosition;
                    Color ImageColor = new Color(HitmarkerColor.r, HitmarkerColor.g, HitmarkerColor.b, HitmarkerColor.a * HitmarkerOpacity);
                    TLHImage.color = ImageColor; TRHImage.color = ImageColor; 
                    BLHImage.color = ImageColor; BRHImage.color = ImageColor;
                    BleedImage.color = new Color(BleedColor.r, BleedColor.g, BleedColor.b, BleedColor.a * HitmarkerOpacity);
                    ArmorHitmarkerRect.localPosition = DebugOffset + HitmarkerPosition + AHitmarkerPlugin.ArmorOffset.Value;
                    ArmorHitmarkerImage.color = new Color(ArmorHitmarkerColor.r, ArmorHitmarkerColor.g, ArmorHitmarkerColor.b, ArmorHitmarkerColor.a * HitmarkerOpacity);
                    StaticHitmarkerImage.color = new Color(HitmarkerColor.r, HitmarkerColor.g, HitmarkerColor.b, HitmarkerColor.a * HitmarkerOpacity * AHitmarkerPlugin.StaticOpacity.Value);
                }
                if (HitmarkerTime > (AHitmarkerPlugin.AnimatedTime.Value + AHitmarkerPlugin.AnimatedAlphaTime.Value))
                {
                    UpdateHitmarker = false;
                    DebugMode = false;
                    DebugOffset = Vector3.zero;
                }
            }
        }
        public static void CreateGameObjects(Transform parent)
        {
            killListGameObject = new GameObject("killList");
            rectTransform = killListGameObject.AddComponent<RectTransform>();
            killListGameObject.transform.SetParent(parent);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(0f, 0f);
            verticalLayoutGroup = killListGameObject.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.childControlHeight = false;
            verticalLayoutGroup.spacing = AHitmarkerPlugin.KillChildSpacing.Value;
            ContentSizeFitter contentSizeFitter = killListGameObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            rectTransform.localPosition = new Vector3(AHitmarkerPlugin.KillRectPosition.Value.x, AHitmarkerPlugin.KillRectPosition.Value.y, 0f);
            rectTransform.pivot = AHitmarkerPlugin.KillRectPivot.Value;

            ArmorHitmarker = new GameObject("ArmorHitmarker");
            ArmorHitmarkerRect = ArmorHitmarker.AddComponent<RectTransform>();
            ArmorHitmarkerImage = ArmorHitmarker.AddComponent<Image>();
            ArmorHitmarker.transform.SetParent(parent);
            ArmorHitmarkerImage.sprite = LoadedSprites[AHitmarkerPlugin.ArmorShape.Value];
            ArmorHitmarkerImage.raycastTarget = false;
            ArmorHitmarkerImage.color = Color.clear;

            BleedHitmarker = new GameObject("BleedHitmarker");
            BleedRect = BleedHitmarker.AddComponent<RectTransform>();
            BleedImage = BleedHitmarker.AddComponent<Image>();
            BleedHitmarker.transform.SetParent(parent);
            BleedImage.sprite = LoadedSprites["BleedHitmarker.png"];
            BleedImage.raycastTarget = false;
            BleedImage.color = Color.clear;

            StaticHitmarker = new GameObject("StaticHitmarker");
            StaticHitmarkerRect = StaticHitmarker.AddComponent<RectTransform>();
            StaticHitmarkerImage = StaticHitmarker.AddComponent<Image>();
            StaticHitmarker.transform.SetParent(parent);
            StaticHitmarkerImage.sprite = LoadedSprites["StaticHitmarker.png"];
            StaticHitmarkerImage.raycastTarget = false;
            StaticHitmarkerImage.color = Color.clear;

            TLH = new GameObject("TLH");
            TLHRect = TLH.AddComponent<RectTransform>();
            TLHImage = TLH.AddComponent<Image>();
            TLH.transform.SetParent(parent);
            TLHImage.sprite = LoadedSprites[AHitmarkerPlugin.Shape.Value];
            TLHImage.raycastTarget = false;
            TLHImage.color = Color.clear;
            TLHRect.localRotation = Quaternion.Euler(0, 0, 45);

            TRH = new GameObject("TRH");
            TRHRect = TRH.AddComponent<RectTransform>();
            TRHImage = TRH.AddComponent<Image>();
            TRH.transform.SetParent(parent);
            TRHImage.sprite = LoadedSprites[AHitmarkerPlugin.Shape.Value];
            TRHImage.raycastTarget = false;
            TRHImage.color = Color.clear;
            TRHRect.localRotation = Quaternion.Euler(0, 0, -45);

            BLH = new GameObject("BLH");
            BLHRect = BLH.AddComponent<RectTransform>();
            BLHImage = BLH.AddComponent<Image>();
            BLH.transform.SetParent(parent);
            BLHImage.sprite = LoadedSprites[AHitmarkerPlugin.Shape.Value];
            BLHImage.raycastTarget = false;
            BLHImage.color = Color.clear;
            BLHRect.localRotation = Quaternion.Euler(0, 0, -45);

            BRH = new GameObject("BRH");
            BRHRect = BRH.AddComponent<RectTransform>();
            BRHImage = BRH.AddComponent<Image>();
            BRH.transform.SetParent(parent);
            BRHImage.sprite = LoadedSprites[AHitmarkerPlugin.Shape.Value];
            BRHImage.raycastTarget = false;
            BRHImage.color = Color.clear;
            BRHRect.localRotation = Quaternion.Euler(0, 0, 45);
        }
        public static void DestroyGameObjects()
        {
            if (killListGameObject != null) Destroy(killListGameObject);
            if (ArmorHitmarker != null) Destroy(ArmorHitmarker);
            if (BleedHitmarker != null) Destroy(BleedHitmarker);
            if (StaticHitmarker != null) Destroy(StaticHitmarker);
            if (TLH != null) Destroy(TLH);
            if (TRH != null) Destroy(TRH);
            if (BLH != null) Destroy(BLH);
            if (BRH != null) Destroy(BRH);
        }
        public void CreateDebugText(string text, int fontSize, float time)
        {
            if (!AHitmarkerPlugin.EnableKillfeed.Value) return;
            GameObject TextGameObject = new GameObject("TextGameObject");
            TextGameObject.transform.SetParent(killListGameObject.transform);
            if (AHitmarkerPlugin.KillChildDirection.Value)
            {
                TextGameObject.transform.SetSiblingIndex(0);
            }
            AmandsAnimatedText TempAmandsAnimatedText = TextGameObject.AddComponent<AmandsAnimatedText>();
            TempAmandsAnimatedText.text = text;
            TempAmandsAnimatedText.color = AHitmarkerPlugin.KillTextColor.Value;
            TempAmandsAnimatedText.fontSize = fontSize;
            TempAmandsAnimatedText.outlineWidth = AHitmarkerPlugin.KillFontOutline.Value;
            TempAmandsAnimatedText.textAlignmentOptions = AHitmarkerPlugin.KillTextAlignment.Value;
            TempAmandsAnimatedText.EnableWaitAndStart = false;
            Destroy(TextGameObject, time);
        }
        public void CreateUpperText(string text, int fontSize, float time, float OpacitySpeed)
        {
            if (!AHitmarkerPlugin.EnableKillfeed.Value) return;
            GameObject TextGameObject = new GameObject("TextGameObject");
            TextGameObject.transform.SetParent(killListGameObject.transform);
            if (AHitmarkerPlugin.KillChildDirection.Value)
            {
                TextGameObject.transform.SetSiblingIndex(0);
            }
            AmandsAnimatedText TempAmandsAnimatedText = TextGameObject.AddComponent<AmandsAnimatedText>();
            TempAmandsAnimatedText.text = text;
            TempAmandsAnimatedText.color = AHitmarkerPlugin.KillTextColor.Value;
            TempAmandsAnimatedText.fontSize = fontSize;
            TempAmandsAnimatedText.outlineWidth = AHitmarkerPlugin.KillFontOutline.Value;
            TempAmandsAnimatedText.time = time;
            TempAmandsAnimatedText.OpacitySpeed = OpacitySpeed;
            TempAmandsAnimatedText.textAlignmentOptions = AHitmarkerPlugin.KillTextAlignment.Value;
            LastAmandsAnimatedText = TempAmandsAnimatedText;
            Destroy(TextGameObject, time * 10);
        }
        public void CreateKillText()
        {
            if (!AHitmarkerPlugin.EnableKillfeed.Value) return;
            string Start = "";
            string RoleName = "";
            Color RoleColor = Color.white;
            string Name = "";
            string End = "";
            switch (killPlayerSide)
            {
                case EPlayerSide.Usec:
                    RoleName = "USEC";
                    RoleColor = AHitmarkerPlugin.UsecColor.Value;
                    break;
                case EPlayerSide.Bear:
                    RoleName = "BEAR";
                    RoleColor = AHitmarkerPlugin.BearColor.Value;
                    break;
                case EPlayerSide.Savage:
                    RoleColor = AHitmarkerPlugin.ScavColor.Value;
                    break;
            }
            if (killPlayerSide == EPlayerSide.Savage)
            {
                RoleName = AmandsHitmarkerHelper.Localized(AmandsHitmarkerHelper.GetScavRoleKey(killRole), EStringCase.Upper);
                if (AmandsHitmarkerHelper.IsFollower(killRole)) RoleColor = AHitmarkerPlugin.FollowerColor.Value;
                if (AmandsHitmarkerHelper.IsBoss(killRole) || AmandsHitmarkerHelper.CountAsBoss(killRole)) RoleColor = AHitmarkerPlugin.BossColor.Value;
            }
            switch (AHitmarkerPlugin.KillStart.Value)
            {
                case EKillStart.PlayerWeapon:
                    if (localPlayer != null)
                    {
                        Color playerRoleColor = HitmarkerColor;
                        switch (AHitmarkerPlugin.KillNameColor.Value)
                        {
                            case EKillNameColor.SingleColor:
                                playerRoleColor = AHitmarkerPlugin.KillNameSingleColor.Value;
                                break;
                            case EKillNameColor.Colored:
                                switch (localPlayer.Profile.Side)
                                {
                                    case EPlayerSide.Usec:
                                        playerRoleColor = AHitmarkerPlugin.UsecColor.Value;
                                        break;
                                    case EPlayerSide.Bear:
                                        playerRoleColor = AHitmarkerPlugin.BearColor.Value;
                                        break;
                                    case EPlayerSide.Savage:
                                        playerRoleColor = AHitmarkerPlugin.ScavColor.Value;
                                        break;
                                }
                                break;
                        }
                        Start = "<b><color=#" + ColorUtility.ToHtmlStringRGB(playerRoleColor) + ">" + localPlayer.Profile.Nickname + "</color> " + killWeaponName + " </b> ";
                    }
                    else
                    {
                        Start = "<b>" + killWeaponName + "</b> ";
                    }
                    break;
                case EKillStart.Weapon:
                    Start = "<b>" + killWeaponName + "</b> ";
                    break;
                case EKillStart.WeaponRole:
                    Start = "<b>" + killWeaponName + " <color=#" + ColorUtility.ToHtmlStringRGB(RoleColor) + ">" + RoleName + "</color></b> ";
                    break;
            }
            switch (AHitmarkerPlugin.KillNameColor.Value)
            {
                case EKillNameColor.None:
                    Name = (killPlayerSide == EPlayerSide.Savage ? AmandsHitmarkerHelper.Transliterate(killPlayerName) : killPlayerName) + " ";
                    break;
                case EKillNameColor.SingleColor:
                    Name = "<color=#" + ColorUtility.ToHtmlStringRGB(AHitmarkerPlugin.KillNameSingleColor.Value) + ">" + (killPlayerSide == EPlayerSide.Savage ? AmandsHitmarkerHelper.Transliterate(killPlayerName) : killPlayerName) + "</color> ";
                    break;
                case EKillNameColor.Colored:
                    Name = "<color=#" + ColorUtility.ToHtmlStringRGB(HitmarkerColor) + ">" + (killPlayerSide == EPlayerSide.Savage ? AmandsHitmarkerHelper.Transliterate(killPlayerName) : killPlayerName) + "</color> ";
                    break;
            }
            if (killDistance > AHitmarkerPlugin.KillDistanceThreshold.Value && !(AHitmarkerPlugin.KillEnd.Value == EKillEnd.Distance || AHitmarkerPlugin.KillEnd.Value == EKillEnd.None))
            {
                End = "<b>" + ((int)killDistance) + "M</b>";
            }
            else
            {
                EKillEnd killEnd = AHitmarkerPlugin.KillEnd.Value;
                if (killEnd == EKillEnd.Level && killPlayerSide == EPlayerSide.Savage) killEnd = EKillEnd.Experience;
                switch (killEnd)
                {
                    case EKillEnd.Bodypart:
                        End = "<b>" + killBodyPart + "</b>";
                        break;
                    case EKillEnd.Role:
                        End = "<b>" + "<color=#" + ColorUtility.ToHtmlStringRGB(RoleColor) + ">" + RoleName + "</color></b>";
                        break;
                    case EKillEnd.Experience:
                        /*switch (killPlayerSide)
                        {
                            case EPlayerSide.Usec:
                            case EPlayerSide.Bear:
                                killExperience = 200;
                                break;
                            case EPlayerSide.Savage:
                                switch (killRole)
                                {
                                    case WildSpawnType.sectantPriest:
                                    case WildSpawnType.bossKilla:
                                        killExperience = 1200;
                                        break;
                                    case WildSpawnType.bossKojaniy:
                                        killExperience = 1100;
                                        break;
                                    case WildSpawnType.bossBully:
                                    case WildSpawnType.bossGluhar:
                                    case WildSpawnType.bossSanitar:
                                    case WildSpawnType.bossTagilla:
                                    case WildSpawnType.followerBigPipe:
                                    case WildSpawnType.followerBirdEye:
                                    case WildSpawnType.bossKnight:
                                        killExperience = 1000;
                                        break;
                                    case WildSpawnType.sectantWarrior:
                                        killExperience = 600;
                                        break;
                                    case WildSpawnType.followerSanitar:
                                        killExperience = 325;
                                        break;
                                    case WildSpawnType.followerKojaniy:
                                        killExperience = 300;
                                        break;
                                    case WildSpawnType.followerGluharSecurity:
                                        killExperience = 275;
                                        break;
                                    case WildSpawnType.followerGluharAssault:
                                        killExperience = 250;
                                        break;
                                    case WildSpawnType.followerGluharScout:
                                    case WildSpawnType.followerGluharSnipe:
                                    case WildSpawnType.followerBully:
                                        killExperience = 200;
                                        break;
                                    case WildSpawnType.pmcBot:
                                        killExperience = 500;
                                        break;
                                    case WildSpawnType.exUsec:
                                        killExperience = 225;
                                        break;
                                    default:
                                        killExperience = 100;
                                        break;
                                }
                                break;
                        }*/
                        float BaseExp = 0;
                        float HeadshotExp = 0;
                        float StreakExp = 0;
                        switch (killPlayerSide)
                        {
                            case EPlayerSide.Usec:
                                BaseExp = VictimLevelExp;
                                break;
                            case EPlayerSide.Bear:
                                BaseExp = VictimLevelExp;
                                break;
                            case EPlayerSide.Savage:
                                BaseExp = killExperience;
                                if (BaseExp < 0)
                                {
                                    BaseExp = VictimBotLevelExp;
                                }
                                break;
                        }
                        if (killBodyPart == EBodyPart.Head && AHitmarkerPlugin.KillHeadshotXP.Value == EHeadshotXP.OnFormula)
                        {
                            HeadshotExp = (int)((float)BaseExp * Mathf.Max(HeadShotMult - 1f,0));
                        }
                        if (AHitmarkerPlugin.KillStreakXP.Value)
                        {
                            if (Combo.Count != 0)
                            {
                                StreakExp = (int)((float)BaseExp * ((float)GetKillingBonusPercent(Kills) / 100f));
                            }
                        }
                        End = "<b>" + (int)(BaseExp + HeadshotExp + StreakExp) + "XP</b>";
                        break;
                    case EKillEnd.Distance:
                        End = "<b>" + ((int)killDistance) + "M</b>";
                        break;
                    case EKillEnd.DamageType:
                        End = "<b>" + lethalDamageType + "</b>";
                        break;
                    case EKillEnd.Level:
                        End = "<b>Level " + killLevel + "</b>";
                        break;
                }
            }
            GameObject TextGameObject = new GameObject("KillTextGameObject");
            TextGameObject.transform.SetParent(killListGameObject.transform);
            if (AHitmarkerPlugin.KillChildDirection.Value)
            {
                TextGameObject.transform.SetSiblingIndex(0);
            }
            AmandsAnimatedText TempAmandsAnimatedText = TextGameObject.AddComponent<AmandsAnimatedText>();
            TempAmandsAnimatedText.text = Start + Name + End;
            TempAmandsAnimatedText.color = AHitmarkerPlugin.KillTextColor.Value;
            TempAmandsAnimatedText.fontSize = AHitmarkerPlugin.KillFontSize.Value;
            TempAmandsAnimatedText.outlineWidth = AHitmarkerPlugin.KillFontOutline.Value;
            if (AHitmarkerPlugin.KillFontUpperCase.Value)
            {
                TempAmandsAnimatedText.fontStyles = FontStyles.UpperCase;
            }
            TempAmandsAnimatedText.time = AHitmarkerPlugin.KillTime.Value;
            TempAmandsAnimatedText.OpacitySpeed = AHitmarkerPlugin.KillOpacitySpeed.Value;
            TempAmandsAnimatedText.textAlignmentOptions = AHitmarkerPlugin.KillTextAlignment.Value;
            LastAmandsAnimatedText = TempAmandsAnimatedText;
            Destroy(TextGameObject, AHitmarkerPlugin.KillTime.Value * 10);
        }
        public void ReloadFiles()
        {
            string[] Files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "/BepInEx/plugins/Hitmarker/images/", "*.png");
            foreach (string File in Files)
            {
                LoadSprite(File);
            }
            string[] AudioFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "/BepInEx/plugins/Hitmarker/sounds/");
            foreach (string File in AudioFiles)
            {
                LoadAudioClip(File);
            }
        }
        async void LoadSprite(string path)
        {
            LoadedSprites[Path.GetFileName(path)] = await RequestSprite(path);
        }
        async Task<Sprite> RequestSprite(string path)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);
            var SendWeb = www.SendWebRequest();

            while (!SendWeb.isDone)
                await Task.Yield();

            if (www.isNetworkError || www.isHttpError)
            {
                return null;
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                return sprite;
            }
        }
        async void LoadAudioClip(string path)
        {
            LoadedAudioClips[Path.GetFileName(path)] = await RequestAudioClip(path);
        }
        async Task<AudioClip> RequestAudioClip(string path)
        {
            string extension = Path.GetExtension(path);
            AudioType audioType = AudioType.WAV;
            switch (extension)
            {
                case ".wav":
                    audioType = AudioType.WAV;
                    break;
                case ".ogg":
                    audioType = AudioType.OGGVORBIS;
                    break;
            }
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, audioType);
            var SendWeb = www.SendWebRequest();

            while (!SendWeb.isDone)
                await Task.Yield();

            if (www.isNetworkError || www.isHttpError)
            {
                return null;
            }
            else
            {
                AudioClip audioclip = DownloadHandlerAudioClip.GetContent(www);
                return audioclip;
            }
        }
        public void HitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.HitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            DebugMode = true;
            System.Random rnd = new System.Random();
            killPlayerName = DebugNames[rnd.Next(DebugNames.Count)];
            bodyPart = EBodyPart.Chest;
            lethalDamageType = EDamageType.Bullet;
            killLevel = 1;
            hitmarker = ActiveUIScreen != null;
        }
        public void HeadshotHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.HeadshotHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            DebugMode = true;
            System.Random rnd = new System.Random();
            killPlayerName = DebugNames[rnd.Next(DebugNames.Count)];
            bodyPart = EBodyPart.Head;
            lethalDamageType = EDamageType.Bullet;
            killLevel = 1;
            hitmarker = ActiveUIScreen != null;
        }
        public void BleedHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.BleedHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            DebugMode = true;
            killRole = WildSpawnType.assault;
            killPlayerSide = EPlayerSide.Savage;
            killDistance = 25;
            System.Random rnd = new System.Random();
            killWeaponName = DebugWeapons[rnd.Next(DebugWeapons.Count)];
            killPlayerName = DebugNames[rnd.Next(DebugNames.Count)];
            bodyPart = EBodyPart.Chest;
            lethalDamageType = EDamageType.HeavyBleeding;
            killLevel = 1;
            killHitmarker = ActiveUIScreen != null;
        }
        public void PoisonHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.PoisonHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            DebugMode = true;
            killRole = WildSpawnType.assault;
            killPlayerSide = EPlayerSide.Savage;
            killDistance = 25;
            System.Random rnd = new System.Random();
            killWeaponName = DebugWeapons[rnd.Next(DebugWeapons.Count)];
            killPlayerName = DebugNames[rnd.Next(DebugNames.Count)];
            bodyPart = EBodyPart.Chest;
            lethalDamageType = EDamageType.LethalToxin;
            killLevel = 1;
            killHitmarker = ActiveUIScreen != null;
        }
        public void ArmorHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.ArmorHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            DebugMode = true;
            System.Random rnd = new System.Random();
            killPlayerName = DebugNames[rnd.Next(DebugNames.Count)];
            bodyPart = EBodyPart.Chest;
            lethalDamageType = EDamageType.Bullet;
            killLevel = 1;
            hitmarker = true;
            armorHitmarker = ActiveUIScreen != null;
        }
        public void ArmorBreakHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.ArmorBreakHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            DebugMode = true;
            System.Random rnd = new System.Random();
            killPlayerName = DebugNames[rnd.Next(DebugNames.Count)];
            bodyPart = EBodyPart.Chest;
            lethalDamageType = EDamageType.Bullet;
            killLevel = 1;
            hitmarker = true;
            armorHitmarker = ActiveUIScreen != null;
            armorBreak = ActiveUIScreen != null;
        }
        public void UsecHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.UsecHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            DebugMode = true;
            killRole = WildSpawnType.bossKnight;
            killPlayerSide = EPlayerSide.Usec;
            killDistance = 25;
            System.Random rnd = new System.Random();
            killWeaponName = DebugWeapons[rnd.Next(DebugWeapons.Count)];
            killPlayerName = DebugNames[rnd.Next(DebugNames.Count)];
            bodyPart = EBodyPart.Chest;
            lethalDamageType = EDamageType.Bullet;
            killLevel = 1;
            hitmarker = ActiveUIScreen != null;
            killHitmarker = ActiveUIScreen != null;
        }
        public void BearHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.BearHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            DebugMode = true;
            killRole = WildSpawnType.followerBigPipe;
            killPlayerSide = EPlayerSide.Bear;
            killDistance = 25;
            System.Random rnd = new System.Random();
            killWeaponName = DebugWeapons[rnd.Next(DebugWeapons.Count)];
            killPlayerName = DebugNames[rnd.Next(DebugNames.Count)];
            bodyPart = EBodyPart.Chest;
            lethalDamageType = EDamageType.Bullet;
            killLevel = 1;
            hitmarker = ActiveUIScreen != null;
            killHitmarker = ActiveUIScreen != null;
        }
        public void ScavHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.ScavHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            DebugMode = true;
            killRole = WildSpawnType.assault;
            killPlayerSide = EPlayerSide.Savage;
            killDistance = 25;
            System.Random rnd = new System.Random();
            killWeaponName = DebugWeapons[rnd.Next(DebugWeapons.Count)];
            killPlayerName = DebugNames[rnd.Next(DebugNames.Count)];
            bodyPart = EBodyPart.Chest;
            lethalDamageType = EDamageType.Bullet;
            killLevel = 1;
            hitmarker = ActiveUIScreen != null;
            killHitmarker = ActiveUIScreen != null;
        }
        public void ThrowWeaponHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.ThrowWeaponHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            DebugMode = true;
            killRole = WildSpawnType.assault;
            killPlayerSide = EPlayerSide.Savage;
            killDistance = 25;
            System.Random rnd = new System.Random();
            killWeaponName = DebugWeapons[rnd.Next(DebugWeapons.Count)];
            killPlayerName = DebugNames[rnd.Next(DebugNames.Count)];
            bodyPart = EBodyPart.Chest;
            lethalDamageType = EDamageType.GrenadeFragment;
            killLevel = 1;
            hitmarker = ActiveUIScreen != null;
            killHitmarker = ActiveUIScreen != null;
        }
        public void FollowerHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.FollowerHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            DebugMode = true;
            killRole = WildSpawnType.followerBully;
            killPlayerSide = EPlayerSide.Savage;
            killExperience = 100;
            killDistance = 25;
            System.Random rnd = new System.Random();
            killWeaponName = DebugWeapons[rnd.Next(DebugWeapons.Count)];
            killPlayerName = DebugNames[rnd.Next(DebugNames.Count)];
            bodyPart = EBodyPart.Chest;
            lethalDamageType = EDamageType.Bullet;
            killLevel = 1;
            hitmarker = ActiveUIScreen != null;
            killHitmarker = ActiveUIScreen != null;
        }
        public void BossHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.BossHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            DebugMode = true;
            killRole = WildSpawnType.bossKnight;
            killPlayerSide = EPlayerSide.Savage;
            killExperience = 100;
            killDistance = 25;
            System.Random rnd = new System.Random();
            killWeaponName = DebugWeapons[rnd.Next(DebugWeapons.Count)];
            killPlayerName = DebugNames[rnd.Next(DebugNames.Count)]; 
            bodyPart = EBodyPart.Chest;
            lethalDamageType = EDamageType.Bullet;
            killLevel = 1;
            hitmarker = ActiveUIScreen != null;
            killHitmarker = ActiveUIScreen != null;
        }
        public void HitmarkerSoundDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.HitmarkerSoundDebug.Value = false;
            if (LoadedAudioClips.ContainsKey(AHitmarkerPlugin.HitmarkerSound.Value) && Singleton<BetterAudio>.Instance != null)
            {
                Singleton<BetterAudio>.Instance.PlayNonspatial(LoadedAudioClips[AHitmarkerPlugin.HitmarkerSound.Value], BetterAudio.AudioSourceGroupType.NonspatialBypass, 0.0f, AHitmarkerPlugin.SoundVolume.Value);
            }
        }
        public void HeadshotHitmarkerSoundDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.HeadshotHitmarkerSoundDebug.Value = false;
            if (LoadedAudioClips.ContainsKey(AHitmarkerPlugin.HeadshotHitmarkerSound.Value) && Singleton<BetterAudio>.Instance != null)
            {
                Singleton<BetterAudio>.Instance.PlayNonspatial(LoadedAudioClips[AHitmarkerPlugin.HeadshotHitmarkerSound.Value], BetterAudio.AudioSourceGroupType.NonspatialBypass, 0.0f, AHitmarkerPlugin.SoundVolume.Value);
            }
        }
        public void KillHitmarkerSoundDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.KillHitmarkerSoundDebug.Value = false;
            if (LoadedAudioClips.ContainsKey(AHitmarkerPlugin.KillHitmarkerSound.Value) && Singleton<BetterAudio>.Instance != null)
            {
                Singleton<BetterAudio>.Instance.PlayNonspatial(LoadedAudioClips[AHitmarkerPlugin.KillHitmarkerSound.Value], BetterAudio.AudioSourceGroupType.NonspatialBypass, 0.0f, AHitmarkerPlugin.SoundVolume.Value);
            }
        }
        public void ArmorSoundDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.ArmorSoundDebug.Value = false;
            if (LoadedAudioClips.ContainsKey(AHitmarkerPlugin.KillHitmarkerSound.Value) && Singleton<BetterAudio>.Instance != null)
            {
                Singleton<BetterAudio>.Instance.PlayNonspatial(LoadedAudioClips[AHitmarkerPlugin.ArmorSound.Value], BetterAudio.AudioSourceGroupType.NonspatialBypass, 0.0f, AHitmarkerPlugin.SoundVolume.Value);
            }
        }
        public void ArmorBreakSoundDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.ArmorBreakSoundDebug.Value = false;
            if (LoadedAudioClips.ContainsKey(AHitmarkerPlugin.KillHitmarkerSound.Value) && Singleton<BetterAudio>.Instance != null)
            {
                Singleton<BetterAudio>.Instance.PlayNonspatial(LoadedAudioClips[AHitmarkerPlugin.ArmorBreakSound.Value], BetterAudio.AudioSourceGroupType.NonspatialBypass, 0.0f, AHitmarkerPlugin.SoundVolume.Value);
            }
        }
        public void ReloadFilesDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.ReloadFiles.Value = false;
            ReloadFiles();
        }
        public void UpdateKillPreset(object sender, EventArgs e)
        {
            switch (AHitmarkerPlugin.KillPreset.Value)
            {
                case EKillPreset.Center:
                    AHitmarkerPlugin.KillChildDirection.Value = true;
                    AHitmarkerPlugin.KillRectPosition.Value = new Vector2(0f, -250f);
                    AHitmarkerPlugin.KillRectPivot.Value = new Vector2(0.5f, 1f);
                    AHitmarkerPlugin.KillTextAlignment.Value = TextAlignmentOptions.Right;
                    break;
                case EKillPreset.TopLeft:
                    AHitmarkerPlugin.KillChildDirection.Value = true;
                    AHitmarkerPlugin.KillRectPosition.Value = new Vector2(-((Screen.width / 2) - 30), 530f);
                    AHitmarkerPlugin.KillRectPivot.Value = new Vector2(0.0f, 1f);
                    AHitmarkerPlugin.KillTextAlignment.Value = TextAlignmentOptions.Left;
                    break;
                case EKillPreset.TopRight:
                    AHitmarkerPlugin.KillChildDirection.Value = true;
                    AHitmarkerPlugin.KillRectPosition.Value = new Vector2((Screen.width/2)-30, 530f);
                    AHitmarkerPlugin.KillRectPivot.Value = new Vector2(1f, 1f);
                    AHitmarkerPlugin.KillTextAlignment.Value = TextAlignmentOptions.Right;
                    break;
                case EKillPreset.BottomLeft:
                    AHitmarkerPlugin.KillChildDirection.Value = false;
                    AHitmarkerPlugin.KillRectPosition.Value = new Vector2(-((Screen.width / 2) - 30), -280f);
                    AHitmarkerPlugin.KillRectPivot.Value = new Vector2(0f, 0f);
                    AHitmarkerPlugin.KillTextAlignment.Value = TextAlignmentOptions.Left;
                    break;
                case EKillPreset.BottomRight:
                    AHitmarkerPlugin.KillChildDirection.Value = false;
                    AHitmarkerPlugin.KillRectPosition.Value = new Vector2((Screen.width / 2) - 30, -420.0f);
                    AHitmarkerPlugin.KillRectPivot.Value = new Vector2(1f, 0f);
                    AHitmarkerPlugin.KillTextAlignment.Value = TextAlignmentOptions.Right;
                    break;
            }
            if (rectTransform != null && verticalLayoutGroup != null)
            {
                rectTransform.localPosition = new Vector3(AHitmarkerPlugin.KillRectPosition.Value.x, AHitmarkerPlugin.KillRectPosition.Value.y, 0f);
                rectTransform.pivot = AHitmarkerPlugin.KillRectPivot.Value;
                verticalLayoutGroup.spacing = AHitmarkerPlugin.KillChildSpacing.Value;
            }
        }
        public void UpdateKillfeed(object sender, EventArgs e)
        {
            if (rectTransform != null && verticalLayoutGroup != null)
            {
                rectTransform.localPosition = new Vector3(AHitmarkerPlugin.KillRectPosition.Value.x, AHitmarkerPlugin.KillRectPosition.Value.y, 0f);
                rectTransform.pivot = AHitmarkerPlugin.KillRectPivot.Value;
                verticalLayoutGroup.spacing = AHitmarkerPlugin.KillChildSpacing.Value;
            }
        }
        public void ReloadBattleUIScreen(object sender, EventArgs e)
        {
            AHitmarkerPlugin.ReloadBattleUIScreen.Value = false;
            //ActiveUIScreen = null;
        }
    }
    public class AmandsAnimatedText : MonoBehaviour
    {
        public TMP_Text tMP_Text;
        public string text;
        public Color color = new Color(0.84f, 0.88f, 0.95f, 1f);
        public int fontSize = 26;
        public float outlineWidth = 0.01f;
        public FontStyles fontStyles = FontStyles.SmallCaps;
        public TextAlignmentOptions textAlignmentOptions = TextAlignmentOptions.Right;
        public float time = 2f;
        public float OpacitySpeed = 0.08f;
        public bool EnableWaitAndStart = true;
        private float Opacity = 1f;
        private float StartOpacity = 0f;
        private bool UpdateOpacity = false;
        private bool UpdateStartOpacity = false;

        public void Start()
        {
            tMP_Text = gameObject.AddComponent<TextMeshProUGUI>();
            if (tMP_Text != null)
            {
                tMP_Text.text = text;
                tMP_Text.color = color;
                tMP_Text.fontSize = fontSize;
                tMP_Text.outlineWidth = outlineWidth;
                tMP_Text.fontStyle = fontStyles;
                tMP_Text.alignment = textAlignmentOptions;
                tMP_Text.alpha = 0f;
                if (EnableWaitAndStart)
                {
                    WaitAndStart();
                    UpdateStartOpacity = true;
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private async void WaitAndStart()
        {
            await Task.Delay((int)(Math.Min(20f, time) * 1000));
            UpdateOpacity = true;
        }
        public void Update()
        {
            if (UpdateOpacity)
            {
                Opacity -= Math.Max(0.01f, OpacitySpeed);
                tMP_Text.alpha = Opacity;
                if (Opacity < 0)
                {
                    UpdateOpacity = false;
                    UpdateStartOpacity = false;
                    if (this == AmandsHitmarkerClass.LastAmandsAnimatedText)
                    {
                        AmandsHitmarkerClass.killListGameObject.DestroyAllChildren();
                    }
                }
            }
            else if (UpdateStartOpacity && StartOpacity < 1f)
            {
                StartOpacity += OpacitySpeed*2f;
                tMP_Text.alpha = StartOpacity;
            }
        }
    }
    public enum EArmorHitmarker
    {
        Disabled,
        Enabled,
        BreakingOnly
    }
    public enum EKillStart
    {
        None,
        PlayerWeapon,
        Weapon,
        WeaponRole
    }
    public enum EKillNameColor
    {
        None,
        SingleColor,
        Colored
    }
    public enum EKillEnd
    {
        None,
        Bodypart,
        Role,
        Experience,
        Distance,
        DamageType,
        Level
    }
    public enum EKillPreset
    {
        Center,
        TopRight,
        BottomRight,
        TopLeft,
        BottomLeft,
    }
    public enum EHitmarkerPositionMode
    {
        Center,
        GunDirection,
        ImpactPoint,
        ImpactPointStatic
    }
    public enum EHeadshotXP
    {
        Off,
        On,
        OnFormula
    }
}
