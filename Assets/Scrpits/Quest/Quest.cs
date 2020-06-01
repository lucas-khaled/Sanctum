using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Nova Quest", menuName = "Quest/Nova Quest")]
public class Quest : ScriptableObject
{
    public bool principal;
    public string nome;

    public Dialogo dialogo;

    public List<Condicoes> condicoes;

    Condicoes condicaoAtual;
    bool realizada, aceita;
    int numCondicoes, condicaoAtualIndex;

    [SerializeField]
    private Recompensa reward;

    private void Awake()
    {
        if (!GameController.gameController.IsLoadedGame())
            aceita = false;
    }

    public Recompensa getRecompensaAtual()
    {
        return reward;
    }

    public Condicoes getCondicaoAtual()
    {
        return condicaoAtual;
    }

    public void AceitarQuest()
    {
        if(!aceita)
            condicaoAtualIndex = 0;

        condicaoAtual = condicoes[condicaoAtualIndex];
        realizada = false;
        aceita = true;
    }

    public Condicoes getProximaCondicao()
    {
        return condicoes[condicaoAtualIndex + 1];
    }

    public Condicoes ProximaCondicao()
    {
        condicaoAtualIndex++;
        if (condicaoAtualIndex < condicoes.Count)
        {
            condicaoAtual = condicoes[condicaoAtualIndex];
        }
        else
        {
            condicaoAtual = null;
            //TerminaMissao();
        }

        return condicaoAtual;
    }

    public void TerminaMissao()
    {
        if (IsRealizada())
        {
            reward.DarRecompensa();
        }
        condicaoAtual = null;
        realizada = true;
    }

    public bool IsRealizada()
    {
        return realizada;
    }

    public bool IsAceita()
    {
        return aceita;
    }

    public void SetQuestNaoAceita()
    {
        aceita = false;
    }
}
