using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscolhaPersonagemController : MonoBehaviour
{

    public GameObject tyva, iovelik;

    void Awake()
    {
        if(GameController.gameController.GetPersonagemEscolhido() == "Tyva")
        {
            tyva.SetActive(true);
            iovelik.SetActive(false);
        }
        else if (GameController.gameController.GetPersonagemEscolhido() == "Iovelik")
        {
            iovelik.SetActive(true);
            tyva.SetActive(false);
        }
    }
}
