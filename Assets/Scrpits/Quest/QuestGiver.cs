using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : Interagivel
{
    [SerializeField]
    Quest[] quests;

    [SerializeField]
    Dialogo famaBaixa;
    [SerializeField]
    Dialogo famaMedia;
    [SerializeField]
    Dialogo famaAlta;
    [SerializeField]
    Dialogo semQuest;

    private GameObject icone;

    int questsAceitas = 0;

    public int GetQuestAceitas()
    {
        return questsAceitas;
    }

    private void Awake()
    {
        EventsController.onDialogoTerminado += DarQuest;
        EventsController.onLinhaTerminada += OnLinhaTerminada;
        EventsController.onQuestLogChange += OnQuestLogChanged;
        questsAceitas = 0;
    }

    protected override void Start()
    {
        base.Start();
        if (GameController.gameController.IsLoadedGame())
        {
            FindMeOnLoad();
        }
       DeAcceptQuests();
       icone = transform.Find("IconeMissao").gameObject;

        if (questsAceitas > 0)
        {
            OnQuestLogChanged(quests[questsAceitas - 1], true);
        }
    }




    void FindMeOnLoad()
    {
        string[] qgNames = SaveSystem.data.questData.giversNames;

        for(int i = 0; i < qgNames.Length; i++)
        {
            if (qgNames[i] == this.name)
            {
                questsAceitas = SaveSystem.data.questData.giversQntAceitas[i];
                break;
            }
        }
    }

    void DeAcceptQuests()
    {
        if (quests.Length > 0 && questsAceitas < quests.Length)
        {
            for (int i = questsAceitas; i<quests.Length;i++)
            {
                quests[questsAceitas].SetQuestNaoAceita();
            }
        }
    }

    public override void Interact()
    {
        base.Interact();

        if (!isPartOfDialogue)
        {
            if (questsAceitas != 0)
            {
                if (questsAceitas < quests.Length - 1 && !quests[questsAceitas - 1].IsRealizada())
                {
                    if (!quests[questsAceitas - 1].IsRealizada())
                    {
                        if (Player.player.status.Fama < 30)
                        {
                            DialogueSystem.sistemaDialogo.IniciaDialogo(famaBaixa);
                        }
                        else if (Player.player.status.Fama < 60)
                        {
                            DialogueSystem.sistemaDialogo.IniciaDialogo(famaMedia);
                        }
                        else
                        {
                            DialogueSystem.sistemaDialogo.IniciaDialogo(famaBaixa);
                        }
                    }
                    return;
                }

                else if(questsAceitas >= quests.Length)
                {
                    DialogueSystem.sistemaDialogo.IniciaDialogo(semQuest);
                }
            }

            if (!quests[questsAceitas].IsAceita())
            {
                quests[questsAceitas].dialogo.whosDialog = this.name;
                DialogueSystem.sistemaDialogo.IniciaDialogo(quests[questsAceitas].dialogo);
            }
        }
    }

    private void OnQuestLogChanged(Quest quest, bool endQuest = false)
    {       
        if (questsAceitas >= quests.Length)
        {
            if (questsAceitas != 0 && quest.nome == quests[questsAceitas - 1].nome && endQuest)
            {
                Debug.Log(quest.nome + " - Puleei - " + this.name);
                icone.SetActive(false);
            }
        }
        else
        {
            if(questsAceitas != 0 && quest.nome == quests[questsAceitas - 1].nome && endQuest)
            {
                Debug.Log(quest.nome + " - Despuleei - " + this.name);
                icone.SetActive(true);
            }
        }
    }

    private void OnLinhaTerminada(Dialogo dialogo)
    {
        if (dialogo.whosDialog == this.name)
        {
            QuestGiverAnimationController questGiverAnimation = GetComponent<QuestGiverAnimationController>();

            if (questGiverAnimation != null)
            {
                questGiverAnimation.Interagir();
            }
        }

    }

    void DarQuest(Dialogo dialogo)
    {
        if (dialogo.whosDialog == this.name)
        {
            QuestLog.questLog.AdicionarQuest(quests[questsAceitas]);
            icone.SetActive(false);
            questsAceitas++;
        }
    }
}
