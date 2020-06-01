using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int maxColetavel;
    public int numAtaque;
    public float tempoEscudo;
    public float tempoDashTotal;
    public int maxVida;
    public int dinheiro, fama, vida, qntColetavel;
    public int XPRequisito;
    public int experiencia, level;
    public int danoMedio;
    public int qualPlayer;
    
    public PlayerData(StatusPlayer status)
    {
        maxColetavel = status.MaxColetavel;
        numAtaque = status.NumAtaque;
        tempoEscudo = status.TempoEscudo;
        tempoDashTotal = status.tempoDashTotal;
        maxVida = status.maxVida;
        dinheiro = status.Dinheiro;
        fama = status.Fama;
        vida = status.Vida;
        qntColetavel = status.QntColetavel;
        XPRequisito = status.XPRequisito;
        qualPlayer = (int)GameController.gameController.GetPersonagemEscolhido();
    }
}
