using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escolher : MonoBehaviour
{
    public void EscolherPersonagem(string pers)
    {
        GameController.gameController.EscolherPersonagem(pers);
        GameController.gameController.ChangeScene("Mapa");
    }
}
