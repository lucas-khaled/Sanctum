using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    [SerializeField]
    private Texture2D cursor;

    [SerializeField]
    private UnityEngine.UI.Slider volumeGeral;
    [SerializeField]
    private UnityEngine.UI.Slider volumeEfeitos;
    [SerializeField]
    private UnityEngine.UI.Slider volumeMusica;
    [SerializeField]
    private UnityEngine.Audio.AudioMixer audioMixer;

    private bool rodou;

    public void setVolumeAudio()
    {
        GameController.gameController.volumeGeral = volumeGeral.value;
        audioMixer.SetFloat("volumeMaster", volumeGeral.value);
        GameController.gameController.volumeEfeitos = volumeEfeitos.value;
        audioMixer.SetFloat("volumeEfeitos", volumeEfeitos.value);
        GameController.gameController.volumeMusica = volumeMusica.value;
        audioMixer.SetFloat("volumeMusica", volumeMusica.value);
    }

    public void getVolumeAudio()
    {
        float valor;
        audioMixer.GetFloat("volumeMaster",out valor);
        volumeGeral.value = valor;
        audioMixer.GetFloat("volumeEfeitos", out valor);
        volumeEfeitos.value = valor;
        audioMixer.GetFloat("volumeMusica", out valor);
        volumeMusica.value = valor;

    }

    private void Start()
    {
        getVolumeAudio();

        Cursor.SetCursor(cursor, Vector3.zero, CursorMode.ForceSoftware);
    }

    public void CarregaJogo()
    {
        GameController.gameController.LoadGame();
    }

    public void MudaCena(string cena)
    {
        GameController.gameController.ChangeScene(cena);
    }

    public void SairJogo()
    {
        GameController.gameController.FecharJogo();
    }
}
