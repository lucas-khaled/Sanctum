using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FazerEscolha : MonoBehaviour
{

    public void Escolher(string personagem)
    {
        GameController.gameController.EscolherPersonagem(personagem);
        GameController.gameController.ChangeScene("Mapa");
    }
}
