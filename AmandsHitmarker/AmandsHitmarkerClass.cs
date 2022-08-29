using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using EFT;
using EFT.UI;
using Comfort.Common;

namespace AmandsHitmarker
{
    public class AmandsHitmarkerClass : MonoBehaviour
    {
        private static GameObject gameSceneCanvas;
        private static Dictionary<string, Sprite> LoadedSprites = new Dictionary<string, Sprite>();
        private static Dictionary<string, AudioClip> LoadedAudioClips = new Dictionary<string, AudioClip>();
        private static Sprite sprite;
        private static SessionCountersClass sessionCounters;
        private static LocalPlayer localPlayer;
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
        private static Vector3 DebugOffset = Vector3.zero;
        private static GameObject BleedHitmarker;
        private static RectTransform BleedRect;
        private static Image BleedImage;
        private static GameObject StaticHitmarker;
        private static Image StaticHitmarkerImage;
        private static RectTransform StaticHitmarkerRect;

        private static float HitmarkerAlpha = 0.0f;
        private static Color HitmarkerColor = new Color(1.0f, 1.0f, 1.0f);
        private static bool wasBleed = false;
        private static AudioClip audioClip;

        private static long hitCount = 0;
        private static long lastHitCount = 0;
        private static long causeArmorDamage = 0;
        private static long lastCauseArmorDamage = 0;
        private static long headShots = 0;
        private static long lastHeadShots = 0;
        private static long kills = 0;
        private static long lastKills = 0;
        private static long killedBear = 0;
        private static long lastKilledBear = 0;
        private static long killedUsec = 0;
        private static long lastKilledUsec = 0;
        private static long killedSavage = 0;
        private static long lastKilledSavage = 0;
        private static long killedWithThrowWeapon = 0;
        private static long lastKilledWithThrowWeapon = 0;
        private static long killedBoss = 0;
        private static long lastKilledBoss = 0;

        private static int UpdateInterval = 0;

        public void Start()
        {
            AHitmarkerPlugin.Thickness.SettingChanged += HitmarkerDebug;
            AHitmarkerPlugin.CenterOffset.SettingChanged += HitmarkerDebug;
            AHitmarkerPlugin.OffsetSpeed.SettingChanged += HitmarkerDebug;
            AHitmarkerPlugin.OpacitySpeed.SettingChanged += HitmarkerDebug;
            AHitmarkerPlugin.BleedSize.SettingChanged += BleedHitmarkerDebug;

            AHitmarkerPlugin.HitmarkerColor.SettingChanged += HitmarkerDebug;
            AHitmarkerPlugin.ArmorColor.SettingChanged += ArmorHitmarkerDebug;
            AHitmarkerPlugin.BearColor.SettingChanged += BearHitmarkerDebug;
            AHitmarkerPlugin.UsecColor.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.ScavColor.SettingChanged += ScavHitmarkerDebug;
            AHitmarkerPlugin.ThrowWeaponColor.SettingChanged += ThrowWeaponHitmarkerDebug;
            AHitmarkerPlugin.BossColor.SettingChanged += BossHitmarkerDebug;
            AHitmarkerPlugin.BleedColor.SettingChanged += BleedHitmarkerDebug;

            AHitmarkerPlugin.HitmarkerDebug.SettingChanged += HitmarkerDebug;
            AHitmarkerPlugin.HeadshotHitmarkerDebug.SettingChanged += HeadshotHitmarkerDebug; 
            AHitmarkerPlugin.BleedHitmarkerDebug.SettingChanged += BleedHitmarkerDebug;
            AHitmarkerPlugin.ArmorHitmarkerDebug.SettingChanged += ArmorHitmarkerDebug;
            AHitmarkerPlugin.UsecHitmarkerDebug.SettingChanged += UsecHitmarkerDebug;
            AHitmarkerPlugin.BearHitmarkerDebug.SettingChanged += BearHitmarkerDebug;
            AHitmarkerPlugin.ScavHitmarkerDebug.SettingChanged += ScavHitmarkerDebug;
            AHitmarkerPlugin.ThrowWeaponHitmarkerDebug.SettingChanged += ThrowWeaponHitmarkerDebug;
            AHitmarkerPlugin.BossHitmarkerDebug.SettingChanged += BossHitmarkerDebug;
            AHitmarkerPlugin.HitmarkerSoundDebug.SettingChanged += HitmarkerSoundDebug;
            AHitmarkerPlugin.HeadshotHitmarkerSoundDebug.SettingChanged += HeadshotHitmarkerSoundDebug;
            AHitmarkerPlugin.KillHitmarkerSoundDebug.SettingChanged += KillHitmarkerSoundDebug;

            ReloadFiles();
        }
        public void Update()
        {
            UpdateInterval += 1;
            if (sessionCounters != null)
            {
                sessionCounters.Counters.TryGetValue(GClass1865.HitCount, out hitCount);
                sessionCounters.Counters.TryGetValue(GClass1865.Kills, out kills);
                if (hitCount != lastHitCount || kills != lastKills)
                {
                    if (AHitmarkerPlugin.EnableSounds.Value && DebugOffset == Vector3.zero)
                    {
                        if (LoadedAudioClips.ContainsKey(AHitmarkerPlugin.HitmarkerSound.Value) && kills == lastKills)
                        {
                            audioClip = LoadedAudioClips[AHitmarkerPlugin.HitmarkerSound.Value];
                        }
                        else if (LoadedAudioClips.ContainsKey(AHitmarkerPlugin.KillHitmarkerSound.Value) && kills != lastKills)
                        {
                            audioClip = LoadedAudioClips[AHitmarkerPlugin.KillHitmarkerSound.Value];
                        }
                    }
                    wasBleed = hitCount == lastHitCount ? true : false;
                    lastHitCount = hitCount;
                    sessionCounters.Counters.TryGetValue(GClass1865.CauseArmorDamage, out causeArmorDamage);
                    sessionCounters.Counters.TryGetValue(GClass1865.HeadShots, out headShots);
                    sessionCounters.Counters.TryGetValue(GClass1865.KilledBear, out killedBear);
                    sessionCounters.Counters.TryGetValue(GClass1865.KilledUsec, out killedUsec);
                    sessionCounters.Counters.TryGetValue(GClass1865.KilledSavage, out killedSavage);
                    sessionCounters.Counters.TryGetValue(GClass1865.KilledWithThrowWeapon, out killedWithThrowWeapon);
                    sessionCounters.Counters.TryGetValue(GClass1865.KilledBoss, out killedBoss);
                    HitmarkerColor = AHitmarkerPlugin.HitmarkerColor.Value;
                    if (!AHitmarkerPlugin.StaticHitmarkerOnly.Value)
                    {
                        if (LoadedSprites.ContainsKey(AHitmarkerPlugin.Shape.Value))
                        {
                            sprite = LoadedSprites[AHitmarkerPlugin.Shape.Value];
                        }
                        TLHImage.sprite = sprite;
                        TRHImage.sprite = sprite;
                        BLHImage.sprite = sprite;
                        BRHImage.sprite = sprite;
                        TLHRect.sizeDelta = AHitmarkerPlugin.Thickness.Value;
                        TRHRect.sizeDelta = AHitmarkerPlugin.Thickness.Value;
                        BLHRect.sizeDelta = AHitmarkerPlugin.Thickness.Value;
                        BRHRect.sizeDelta = AHitmarkerPlugin.Thickness.Value;
                        TLHRect.localPosition = DebugOffset + TLHOffset * AHitmarkerPlugin.CenterOffset.Value;
                        TRHRect.localPosition = DebugOffset + TRHOffset * AHitmarkerPlugin.CenterOffset.Value;
                        BLHRect.localPosition = DebugOffset + BLHOffset * AHitmarkerPlugin.CenterOffset.Value;
                        BRHRect.localPosition = DebugOffset + BRHOffset * AHitmarkerPlugin.CenterOffset.Value;
                    }
                    StaticHitmarkerImage.sprite = LoadedSprites["StaticHitmarker.png"];
                    if (headShots != lastHeadShots)
                    {
                        lastHeadShots = headShots;
                        if (!AHitmarkerPlugin.StaticHitmarkerOnly.Value)
                        {
                            if (LoadedSprites.ContainsKey(AHitmarkerPlugin.HeadshotShape.Value))
                            {
                                sprite = LoadedSprites[AHitmarkerPlugin.HeadshotShape.Value];
                                TLHImage.sprite = sprite;
                                TRHImage.sprite = sprite;
                                BLHImage.sprite = sprite;
                                BRHImage.sprite = sprite;
                            }
                        }
                        StaticHitmarkerImage.sprite = LoadedSprites["StaticHeadshotHitmarker.png"];
                        if (AHitmarkerPlugin.EnableSounds.Value && kills != lastKills && DebugOffset == Vector3.zero)
                        {
                            if (LoadedAudioClips.ContainsKey(AHitmarkerPlugin.HeadshotHitmarkerSound.Value))
                            {
                                audioClip = LoadedAudioClips[AHitmarkerPlugin.HeadshotHitmarkerSound.Value];
                            }
                        }
                    }
                    lastKills = kills;
                    if (causeArmorDamage != lastCauseArmorDamage)
                    {
                        lastCauseArmorDamage = causeArmorDamage;
                        HitmarkerColor = AHitmarkerPlugin.ArmorColor.Value;
                    }
                    if (killedBear != lastKilledBear)
                    {
                        lastKilledBear = killedBear;
                        HitmarkerColor = AHitmarkerPlugin.BearColor.Value;
                    }
                    if (killedUsec != lastKilledUsec)
                    {
                        lastKilledUsec = killedUsec;
                        HitmarkerColor = AHitmarkerPlugin.UsecColor.Value;
                    }
                    else if (killedSavage != lastKilledSavage)
                    {
                        lastKilledSavage = killedSavage;
                        HitmarkerColor = AHitmarkerPlugin.ScavColor.Value;
                    }
                    if (killedWithThrowWeapon != lastKilledWithThrowWeapon)
                    {
                        lastKilledWithThrowWeapon = killedWithThrowWeapon;
                        HitmarkerColor = AHitmarkerPlugin.ThrowWeaponColor.Value;
                    }
                    if (killedBoss != lastKilledBoss)
                    {
                        lastKilledBoss = killedBoss;
                        HitmarkerColor = AHitmarkerPlugin.BossColor.Value;
                    }
                    if (wasBleed)
                    {
                        BleedRect.sizeDelta = AHitmarkerPlugin.BleedSize.Value;
                        BleedRect.localPosition = DebugOffset;
                    }
                    HitmarkerAlpha = 1.0f;
                    StaticHitmarkerRect.localPosition = DebugOffset;
                    StaticHitmarkerRect.sizeDelta = AHitmarkerPlugin.StaticSizeDelta.Value;

                    if (AHitmarkerPlugin.EnableSounds.Value && DebugOffset == Vector3.zero)
                    {
                        Singleton<GUISounds>.Instance.PlaySound(audioClip);
                    }
                }
                if (HitmarkerAlpha > 0)
                {
                    HitmarkerAlpha -= AHitmarkerPlugin.OpacitySpeed.Value;
                    if (AHitmarkerPlugin.StaticHitmarkerOnly.Value)
                    {
                        StaticHitmarkerImage.color = new Color(HitmarkerColor.r, HitmarkerColor.g, HitmarkerColor.b, HitmarkerColor.a * HitmarkerAlpha * AHitmarkerPlugin.StaticOpacity.Value);
                        StaticHitmarkerRect.sizeDelta += Vector2.one * AHitmarkerPlugin.StaticSizeDeltaSpeed.Value;
                        TLHImage.color = Color.clear;
                        TRHImage.color = Color.clear;
                        BLHImage.color = Color.clear;
                        BRHImage.color = Color.clear;
                    }
                    else
                    {
                        StaticHitmarkerImage.color = new Color(HitmarkerColor.r, HitmarkerColor.g, HitmarkerColor.b, HitmarkerColor.a * HitmarkerAlpha * AHitmarkerPlugin.StaticOpacity.Value);
                        StaticHitmarkerRect.sizeDelta += Vector2.one * AHitmarkerPlugin.StaticSizeDeltaSpeed.Value;
                        TLHRect.localPosition += TLHOffset * AHitmarkerPlugin.OffsetSpeed.Value;
                        TRHRect.localPosition += TRHOffset * AHitmarkerPlugin.OffsetSpeed.Value;
                        BLHRect.localPosition += BLHOffset * AHitmarkerPlugin.OffsetSpeed.Value;
                        BRHRect.localPosition += BRHOffset * AHitmarkerPlugin.OffsetSpeed.Value;
                        TLHImage.color = new Color(HitmarkerColor.r, HitmarkerColor.g, HitmarkerColor.b, HitmarkerColor.a * HitmarkerAlpha);
                        TRHImage.color = new Color(HitmarkerColor.r, HitmarkerColor.g, HitmarkerColor.b, HitmarkerColor.a * HitmarkerAlpha);
                        BLHImage.color = new Color(HitmarkerColor.r, HitmarkerColor.g, HitmarkerColor.b, HitmarkerColor.a * HitmarkerAlpha);
                        BRHImage.color = new Color(HitmarkerColor.r, HitmarkerColor.g, HitmarkerColor.b, HitmarkerColor.a * HitmarkerAlpha);
                    }
                    BleedImage.color = wasBleed ? new Color(AHitmarkerPlugin.BleedColor.Value.r, AHitmarkerPlugin.BleedColor.Value.g, AHitmarkerPlugin.BleedColor.Value.b, AHitmarkerPlugin.BleedColor.Value.a * HitmarkerAlpha) : Color.clear;
                }
                else
                {
                    DebugOffset = Vector3.zero;
                }

            }
            if (UpdateInterval > 500)
            {
                UpdateInterval = 0;
                hitCount += 1;
                if (gameSceneCanvas == null)
                {
                    gameSceneCanvas = GameObject.Find("Game Scene");
                    if (gameSceneCanvas != null)
                    {
                        // Devious code
                        BleedHitmarker = new GameObject("BleedHitmarker");
                        BleedRect = BleedHitmarker.AddComponent<RectTransform>();
                        BleedImage = BleedHitmarker.AddComponent<Image>();
                        BleedHitmarker.transform.SetParent(gameSceneCanvas.transform);
                        BleedImage.sprite = LoadedSprites["BleedHitmarker.png"];
                        BleedImage.raycastTarget = false;
                        BleedImage.color = new Color(1, 1, 1, 0);

                        StaticHitmarker = new GameObject("StaticHitmarker");
                        StaticHitmarkerRect = StaticHitmarker.AddComponent<RectTransform>();
                        StaticHitmarkerImage = StaticHitmarker.AddComponent<Image>();
                        StaticHitmarker.transform.SetParent(gameSceneCanvas.transform);
                        StaticHitmarkerImage.sprite = LoadedSprites["StaticHitmarker.png"];
                        StaticHitmarkerImage.raycastTarget = false;
                        StaticHitmarkerImage.color = new Color(1, 1, 1, 0);

                        TLH = new GameObject("TLH");
                        TLHRect = TLH.AddComponent<RectTransform>();
                        TLHImage = TLH.AddComponent<Image>();
                        TLH.transform.SetParent(gameSceneCanvas.transform);
                        TLHImage.sprite = LoadedSprites[AHitmarkerPlugin.Shape.Value];
                        TLHImage.raycastTarget = false;
                        TLHImage.color = new Color(1, 1, 1, 0);
                        TLHRect.localRotation = Quaternion.Euler(0, 0, 45);

                        TRH = new GameObject("TRH");
                        TRHRect = TRH.AddComponent<RectTransform>();
                        TRHImage = TRH.AddComponent<Image>();
                        TRH.transform.SetParent(gameSceneCanvas.transform);
                        TRHImage.sprite = LoadedSprites[AHitmarkerPlugin.Shape.Value];
                        TRHImage.raycastTarget = false;
                        TRHImage.color = new Color(1, 1, 1, 0);
                        TRHRect.localRotation = Quaternion.Euler(0, 0, -45);

                        BLH = new GameObject("BLH");
                        BLHRect = BLH.AddComponent<RectTransform>();
                        BLHImage = BLH.AddComponent<Image>();
                        BLH.transform.SetParent(gameSceneCanvas.transform);
                        BLHImage.sprite = LoadedSprites[AHitmarkerPlugin.Shape.Value];
                        BLHImage.raycastTarget = false;
                        BLHImage.color = new Color(1, 1, 1, 0);
                        BLHRect.localRotation = Quaternion.Euler(0, 0, -45);

                        BRH = new GameObject("BRH");
                        BRHRect = BRH.AddComponent<RectTransform>();
                        BRHImage = BRH.AddComponent<Image>();
                        BRH.transform.SetParent(gameSceneCanvas.transform);
                        BRHImage.sprite = LoadedSprites[AHitmarkerPlugin.Shape.Value];
                        BRHImage.raycastTarget = false;
                        BRHImage.color = new Color(1, 1, 1, 0);
                        BRHRect.localRotation = Quaternion.Euler(0, 0, 45);
                    }
                }
                if (sessionCounters == null || localPlayer == null)
                {
                    localPlayer = FindObjectOfType<LocalPlayer>();
                    if (localPlayer != null)
                    {
                        hitCount = 0;
                        lastHitCount = 0;
                        causeArmorDamage = 0;
                        lastCauseArmorDamage = 0;
                        headShots = 0;
                        lastHeadShots = 0;
                        kills = 0;
                        lastKills = 0;
                        killedBear = 0;
                        lastKilledBear = 0;
                        killedUsec = 0;
                        lastKilledUsec = 0;
                        killedSavage = 0;
                        lastKilledSavage = 0;
                        killedWithThrowWeapon = 0;
                        lastKilledWithThrowWeapon = 0;
                        killedBoss = 0;
                        lastKilledBoss = 0;
                        sessionCounters = localPlayer.Profile.Stats.SessionCounters;
                    }
                }
            }
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
            lastHitCount = -1;
        }
        public void HeadshotHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.HeadshotHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            lastHitCount = -1;
            lastHeadShots = -1;
        }
        public void BleedHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.BleedHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            lastKills = -1;
        }
        public void ArmorHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.ArmorHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            lastHitCount = -1;
            lastCauseArmorDamage = -1;
        }
        public void UsecHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.UsecHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            lastHitCount = -1;
            lastKills = -1;
            lastKilledUsec = -1;
        }
        public void BearHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.BearHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            lastHitCount = -1;
            lastKills = -1;
            lastKilledBear = -1;
        }
        public void ScavHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.ScavHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            lastHitCount = -1;
            lastKills = -1;
            lastKilledSavage = -1;
        }
        public void ThrowWeaponHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.ThrowWeaponHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            lastHitCount = -1;
            lastKills = -1;
            lastKilledWithThrowWeapon = -1;
        }
        public void BossHitmarkerDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.BossHitmarkerDebug.Value = false;
            DebugOffset = new Vector3(600, 0, 0);
            lastHitCount = -1;
            lastKills = -1;
            lastKilledBoss = -1;
        }
        public void HitmarkerSoundDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.HitmarkerSoundDebug.Value = false;
            if (LoadedAudioClips.ContainsKey(AHitmarkerPlugin.HitmarkerSound.Value))
            {
                Singleton<GUISounds>.Instance.PlaySound(LoadedAudioClips[AHitmarkerPlugin.HitmarkerSound.Value]);
            }
        }
        public void HeadshotHitmarkerSoundDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.HeadshotHitmarkerSoundDebug.Value = false;
            if (LoadedAudioClips.ContainsKey(AHitmarkerPlugin.HeadshotHitmarkerSound.Value))
            {
                Singleton<GUISounds>.Instance.PlaySound(LoadedAudioClips[AHitmarkerPlugin.HeadshotHitmarkerSound.Value]);
            }
        }
        public void KillHitmarkerSoundDebug(object sender, EventArgs e)
        {
            AHitmarkerPlugin.KillHitmarkerSoundDebug.Value = false;
            if (LoadedAudioClips.ContainsKey(AHitmarkerPlugin.KillHitmarkerSound.Value))
            {
                Singleton<GUISounds>.Instance.PlaySound(LoadedAudioClips[AHitmarkerPlugin.KillHitmarkerSound.Value]);
            }
        }
        public void DebugReloadFiles(object sender, EventArgs e)
        {
            if (AHitmarkerPlugin.ReloadFiles.Value)
            {
                ReloadFiles();
            }
        }
    }
}
