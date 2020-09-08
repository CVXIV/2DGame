using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionAudioPanel : BasePanel {
    public Slider volume_slider;
    public Slider sound_slider;

    private void Start() {
        volume_slider.value = PlayerPrefs.GetFloat(ConstantVar.Volume, 0.5f);
        sound_slider.value = PlayerPrefs.GetFloat(ConstantVar.Sound, 0.5f);
    }

    public void OnVolumeChange(float value) {
        PlayerPrefs.SetFloat(ConstantVar.Volume, value);
        volume_slider.value = value;
    }

    public void OnSoundChange(float value) {
        PlayerPrefs.SetFloat(ConstantVar.Sound, value);
        sound_slider.value = value;
    }

}