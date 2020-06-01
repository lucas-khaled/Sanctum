using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioController : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    public AudioMixer mixer;

    [SerializeField]
    private string parametroAudio;

    [SerializeField]
    private bool volumeGeral;
    [SerializeField]
    private bool volumeEfeitos;
    [SerializeField]
    private bool volumeMusica;

    private void Start()
    {
        if (volumeGeral)
        slider.value = GameController.gameController.volumeGeral;
        else if (volumeEfeitos)
            slider.value = GameController.gameController.volumeEfeitos;
        else
            slider.value = GameController.gameController.volumeMusica;

        mixer.SetFloat(parametroAudio, slider.value);
    }
    public void Volume(float valorSlider)
    {
        mixer.SetFloat(parametroAudio, valorSlider);
    }
}
