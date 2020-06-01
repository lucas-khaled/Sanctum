using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestChecker : MonoBehaviour
{
    Quest questHolder;
    Condicoes condHolder;

    public void SetCondicaoOnHolder(Condicoes c)
    {        
        condHolder = c;       
        //se tiver nova condição ele começa a checagem novamente
        if (condHolder != null)
        {
            condHolder.AtivarCondicao();
            InvokeRepeating("CheckCondicaoHolder", 1f, 1f);
        }
        else
        {
            questHolder.TerminaMissao();
            QuestLog.questLog.FinalizarQuest(this);
        }
    }

    //retorna a quest que está checando
    public Quest GetQuestOnHolder()
    {
        return questHolder;
    }


    public void SetQuestOnHolder(Quest q)
    {
        if (q != null)
        {
            questHolder = q;
            SetCondicaoOnHolder(q.getCondicaoAtual());
        }
    }

    private void CheckCondicaoHolder()
    {
        //verifica se a condição é verdadeira
        if (condHolder.ChecaCondicao())
        {
            //se for, ele cancela a checagem e e seta a nova condição
            EventsController.onCondicaoTerminada.Invoke(questHolder);
            CancelInvoke("CheckCondicaoHolder");
            SetCondicaoOnHolder(questHolder.ProximaCondicao());
        }
    }

    private void OnDrawGizmos()
    {
        if (condHolder != null)
        {
            Gizmos.DrawWireSphere(condHolder.local, condHolder.distanciaChegada);
        }
    }
}
