using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    public static SettingsButton Instance { get; set; }

    public enum SettingsButtonState
    {
        Expanded,
        Unfolded
    }
    public SettingsButtonState curState = SettingsButtonState.Unfolded;

    public AudioSource audioSource;
    public Toggle audioToggle;
    public Slider audioSlider;
    public GameObject scrollView;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    public void OnSettingsButtonClicked()
    {
        if (curState == SettingsButtonState.Expanded)
        {
            curState = SettingsButtonState.Unfolded;
            scrollView.SetActive(false);
        }
        else
        {
            curState = SettingsButtonState.Expanded;
            scrollView.SetActive(true);
        }
    }
    public void OnAudioToggleChanged()
    {
        if (audioToggle.isOn)
        {
            audioSlider.interactable = true;
        }
        else
        {
            audioSlider.interactable = false;
        }
    }
    public void OnAudioSliderClicked()
    {
        float volume = audioSlider.value;
        audioSource.volume = volume;
    }
    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
