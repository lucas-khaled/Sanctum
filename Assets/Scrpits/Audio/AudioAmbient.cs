using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAmbient : MonoBehaviour
{
    [SerializeField]
    private GameObject[] audios;
    [SerializeField]
    private GameObject audioAtual;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            for (int i = 0; i < audios.Length; i++)
            {
                audios[i].SetActive(false);
            }
            audioAtual.SetActive(true);
        }
    }

}
