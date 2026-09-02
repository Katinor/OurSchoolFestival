using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public enum EBackgroundSound
{
    Pastel
}

public enum EEffectSound
{
    QuestionAppear,
    QuestionChoose,
    Success,
    PayingMoney
}

[Serializable]
class CBackgroundSound
{
    public EBackgroundSound soundType;
    public AudioSource audioClip;
    public float volume = 1.0f;
    public bool loop = true;
}

[Serializable]
class CEffectSound
{
    public EEffectSound soundType;
    public AudioSource audioClip;
    public float volume = 1.0f;
    public bool loop = false;
}

public class SoundManager : MonoBehaviour
{
    [SerializeField] private List<CBackgroundSound> _backgroundSounds;
    [SerializeField] private List<CEffectSound> _effectSounds;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _seSlider;

    Dictionary<EBackgroundSound, AudioSource> _bgmDict;
    Dictionary<EEffectSound, AudioSource> _seDict;

    private void Awake()
    {
        _bgmDict = new Dictionary<EBackgroundSound, AudioSource>();
        _seDict = new Dictionary<EEffectSound, AudioSource>();
        ResetDictionary();
    }
    void Start()
    {
        
    }

    void Update()
    {
        SetVolume();
    }

    private void ResetDictionary()
    {
        foreach (var bgm in _backgroundSounds)
        {
            if (!_bgmDict.ContainsKey(bgm.soundType))
            {
                _bgmDict.Add(bgm.soundType, bgm.audioClip);
                _bgmDict[bgm.soundType].volume = bgm.volume;
                _bgmDict[bgm.soundType].loop = bgm.loop;
            }
            else
            {
                CPrint.Error($"중복된 사운드 있음 - {bgm.soundType}");
            }
        }
        foreach (var se in _effectSounds)
        {
            if (!_seDict.ContainsKey(se.soundType))
            {
                _seDict.Add(se.soundType, se.audioClip);
                _seDict[se.soundType].volume = se.volume;
                _seDict[se.soundType].loop = se.loop;
            }
            else
            {
                CPrint.Error($"중복된 사운드 있음 - {se.soundType}");
            }
        }
    }

    private void SetVolume()
    {
        float bgmVolume = _bgmSlider.value;
        float seVolume = _seSlider.value;

        if (bgmVolume <= 0.0001f)
        {
            _audioMixer.SetFloat("BGMLevel", -80f);
        }
        else
        {
            _audioMixer.SetFloat("BGMLevel", Mathf.Log10(bgmVolume) * 20);
        }

        if (seVolume <= 0.0001f)
        {
            _audioMixer.SetFloat("SELevel", -80f);
        }
        else
        {
            _audioMixer.SetFloat("SELevel", Mathf.Log10(seVolume) * 20);
        }
    }

    public void PlaySE(EEffectSound soundType)
    {
        if (_seDict.ContainsKey(soundType))
        {
            _seDict[soundType].Play();
        }
        else
        {
            CPrint.Error($"사운드 없음 - {soundType}");
        }
    }

    public void PlayBGM(EBackgroundSound soundType)
    {
        foreach(var audioSource in _bgmDict.Values)
        {
            audioSource.Stop();
        }
        if (_bgmDict.ContainsKey(soundType))
        {
            _bgmDict[soundType].Play();
        }
        else
        {
            CPrint.Error($"사운드 없음 - {soundType}");
        }
    }
}
