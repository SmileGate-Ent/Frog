using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{
    [SerializeField] AudioMixer m_AudioMixer;
    [SerializeField] Slider m_MusicBGMSlider;
    [SerializeField] Slider m_MusicSFXSlider;

    const string BGMParam = "BGMParam";
    const string SFXParam = "SFXParam";

    void Awake()
    {
        InitSlider(BGMParam, m_MusicBGMSlider, SetBGMVolume);
        InitSlider(SFXParam, m_MusicSFXSlider, SetSFXVolume);
    }

    void InitSlider(string param, Slider slider, UnityAction<float> action)
    {
        slider.onValueChanged.AddListener(action);

        if (m_AudioMixer.GetFloat(param, out var expVol))
        {
            var linVol = ExpToLin(expVol);
            Debug.Log($"Set slider '{param}' value to '{linVol}' (linear)");
            slider.SetValueWithoutNotify(linVol);
        }
    }

    static float LinToExp(float linVol) => Mathf.Log10(linVol) * 20;
    static float ExpToLin(float expVol) => (float)(Math.Pow(10, expVol / 20));

    void SetBGMVolume(float linVol)
    {
        m_AudioMixer.SetFloat(BGMParam, LinToExp(linVol));
    }

    void SetSFXVolume(float linVol)
    {
        m_AudioMixer.SetFloat(SFXParam, LinToExp(linVol));
    }
}