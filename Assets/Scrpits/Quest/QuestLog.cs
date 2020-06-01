using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestLog : MonoBehaviour
{

    #region SINGLETON
    public static QuestLog questLog;

    private void Awake()
    {
        questLog = this;
      
    }

    #endregion

    List<Quest> questsAceitas = new List<Quest>();
    List<Quest> questsFinalizadas = new List<Quest>();

    List<QuestChecker> activeQuestCheckers = new List<QuestChecker>();

    private void Start()
    {
        if (GameController.gameController.IsLoadedGame())
        {
            LoadQuestLog();
        }
    }

    void LoadQuestLog()
    {
        foreach(Quest qa in SaveSystem.data.questData.questsAceitas)
        {
            /*questsAceitas.Add(qa);
            AddQuestChecker(qa);*/

            AdicionarQuest(qa);
        }

        foreach(Quest qf in SaveSystem.data.questData.questsFinalizadas)
        {
            qf.TerminaMissao();
            questsFinalizadas.Add(qf);
            EventsController.onQuestLogChange.Invoke(qf, true);
        }
    }

    public List<Quest> getQuestFinalizadas()
    {
        return questsFinalizadas;
    }

    public List<Quest> getQuestAceitas()
    {
        return questsAceitas;
    }

    public void AdicionarQuest(Quest quest)
    {
        //adiciona na lista de quest aceitas
        questsAceitas.Add(quest);

        //fala para a quest que ela foi aceita
        quest.AceitarQuest();


        AddQuestChecker(quest);

        if (EventsController.onQuestLogChange != null) {
            EventsController.onQuestLogChange.Invoke(quest);
        }
    }

    void AddQuestChecker(Quest quest)
    {
        //Cria um Questchecker e passa qual a quest
        GameObject check = new GameObject("Checker " + quest.nome, typeof(QuestChecker));
        QuestChecker checker = check.GetComponent<QuestChecker>();
        checker.SetQuestOnHolder(quest);
        activeQuestCheckers.Add(checker);

        //parenteia o game obj para organização
        check.transform.SetParent(transform);
    }

    public void FinalizarQuest(QuestChecker questChecker)
    {
        //passa a quest para as finalizadas
        questsAceitas.Remove(questChecker.GetQuestOnHolder());
        questsFinalizadas.Add(questChecker.GetQuestOnHolder());

        //remove da lista de checkers e destroy seu gameobject
        activeQuestCheckers.Remove(questChecker);
        Destroy(questChecker.gameObject);

        if (EventsController.onQuestLogChange != null)
        {
            EventsController.onQuestLogChange.Invoke(questChecker.GetQuestOnHolder(), true);
        }
    }

    /*void CheckCondicoes()
    {
        if (questsAceitas.Count > 0)
        {
            foreach (Quest q in questsAceitas)
            {
                if (q.getCondicaoAtual().ChecaCondicao() && q.getCondicaoAtual().GetNaoRealizadaEAtiva())
                {
                    q.getCondicaoAtual().SetCondicaoRealizada();
                }
            }
        }
    }*/
}
