using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 声音管理器。
/// 【重建文件】原文件在工程快照中缺失，按 PlayMusic/PlaySound 调用点
/// 及 Resources 下现有音频资源重建，功能可用（双 AudioSource：音乐循环 + 音效单次）。
/// </summary>
public class SoundManager : MonoSingleton<SoundManager>
{
    private AudioSource musicAudioSource;
    private AudioSource soundAudioSource;

    protected override void OnStart()
    {
        this.musicAudioSource = this.gameObject.AddComponent<AudioSource>();
        this.musicAudioSource.loop = true;
        this.musicAudioSource.playOnAwake = false;

        this.soundAudioSource = this.gameObject.AddComponent<AudioSource>();
        this.soundAudioSource.loop = false;
        this.soundAudioSource.playOnAwake = false;
    }

    /// <summary>
    /// 播放背景音乐（SoundDefine.Music_* 常量，循环）。
    /// </summary>
    public void PlayMusic(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(name);
        if (clip == null)
        {
            Debug.LogWarningFormat("PlayMusic: clip [{0}] not found", name);
            return;
        }
        if (this.musicAudioSource.clip != clip)
        {
            this.musicAudioSource.clip = clip;
            this.musicAudioSource.Play();
        }
    }

    /// <summary>
    /// 播放音效（SoundDefine.SFX_* 常量，单次）。
    /// </summary>
    public void PlaySound(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(name);
        if (clip == null)
        {
            Debug.LogWarningFormat("PlaySound: clip [{0}] not found", name);
            return;
        }
        this.soundAudioSource.PlayOneShot(clip);
    }
}
