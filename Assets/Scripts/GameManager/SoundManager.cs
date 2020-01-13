using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    public enum Sound
    {
        PlayerMove,
        PlayerUnableMove,
        PlayerPush,
        PlayerGoal,
        GameStart,
        StageStart,
        MenuOpen,
        MenuClick,
        Loop0,
        Loop1,
        Loop2,
        Loop3,
        Loop4,
        Loop5,
        Loop6,
        Loop7,
        Loop8,
        Loop9,
        Loop10,
        StageClear,
        MenuMiss,
        CharacterSelect,
        GameClearFirework,
        AlertPopup,
        AlertYes,
        AlertNo,
        ImagePopup,
        Ready
    }
    private static Dictionary<Sound, float> soundTimerDictionary;
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;
    private static GameObject bgmGameObject;
    private static AudioSource bgmAudioSource;

    public static void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.PlayerMove] = 0f;
    }
    public static void PlaySound(Sound sound, Vector3 position)
    {
        if (isAblePlaySound(sound) && ConfigurationData.efxSound)
        {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = position;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(sound);
            audioSource.maxDistance = 100f;
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.dopplerLevel = 0f;
            audioSource.Play();

            Object.Destroy(soundGameObject, audioSource.clip.length);
        }
    }
    public static void PlaySound(Sound sound)
    {
        if(isAblePlaySound(sound) && ConfigurationData.efxSound)
        {
            if (oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("Sound");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
            }
            oneShotAudioSource.PlayOneShot(GetAudioClip(sound));
        }
    }
    public static void PlayLoopSound(Sound sound, bool isForceChange = false)
    {
        if (isAblePlaySound(sound))
        {
            if (bgmGameObject == null)
            {
                bgmGameObject = new GameObject("BgmSound");
                bgmAudioSource = bgmGameObject.AddComponent<AudioSource>();
                bgmAudioSource.loop = true;
                bgmAudioSource.clip = GetAudioClip(sound);
                bgmAudioSource.Play();
                if (!ConfigurationData.bgmSound)
                    bgmAudioSource.Pause();
            }
            if(isForceChange)
            {
                bgmAudioSource.clip = GetAudioClip(sound);
                bgmAudioSource.Play();
                if (!ConfigurationData.bgmSound)
                    bgmAudioSource.Pause();
            }
            
        }
    }
    public static void RePlayLoopSound()
    {
        if(ConfigurationData.bgmSound)
        {
            if (bgmGameObject != null && bgmAudioSource != null)
                bgmAudioSource.UnPause();
        }
        else
        {
            bgmAudioSource.Pause();
        }

    }
    private static bool isAblePlaySound(Sound sound)
    {
        switch(sound)
        {
            default:
                return true;
            case Sound.PlayerMove:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = .05f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return true;

                
        }
    }
    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach(var s in GameAssetsManager.instance.SoundAudioClips)
        {
            if (s.sound == sound)
                return s.audioClip;
        }
        Debug.LogError(sound + " 오디오 클립이 없음");
        return null;
    }
}

[System.Serializable]
public class SoundAudioClip
{
    public SoundManager.Sound sound;
    public AudioClip audioClip;
}
