using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscolhaPersonagemController : MonoBehaviour
{

    public GameObject tyva, iovelik;
    public Transform posicaoInicio;

    void Awake()
    {
        if (GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.TYVA)
        {
            Instantiate(tyva, posicaoInicio.position, posicaoInicio.rotation);
        }
        else if (GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.IOVELIK)
        {
            Instantiate(iovelik, posicaoInicio.position, posicaoInicio.rotation);
        }
    }
}
