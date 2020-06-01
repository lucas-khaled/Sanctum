using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Holder_Quest : Button, ISelectHandler
{
    public GameObject bandeiraAtiva;
    public Quest referenciaQuest;
    public Text referenciaTitulo;

    private List<Condicoes> condicoesQuest = new List<Condicoes>();
    
    protected override void Start()
    {
        base.Start();
        referenciaTitulo.text = referenciaQuest.nome;
    }

    public override void OnSelect(BaseEventData eventData) {
        base.OnSelect(eventData);
        QuestLogUI.questLogUI.AtualizarDescricao(referenciaQuest);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (eventData.clickCount == 2 && !referenciaQuest.IsRealizada())
            QuestLogUI.questLogUI.TrocarQuestHUD(referenciaQuest, bandeiraAtiva);
    }
    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        QuestLogUI.questLogUI.TrocarQuestHUD(referenciaQuest,bandeiraAtiva);
    }

    public override void OnDeselect(BaseEventData eventData) {
        base.OnDeselect(eventData);
        QuestLogUI.questLogUI.LimparDescricoes();
    }

}
