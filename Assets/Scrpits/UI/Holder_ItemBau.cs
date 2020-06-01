using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Holder_ItemBau : Button, ISelectHandler
{

    public Item item;
    public Image spriteItem;

    private int clickCount;
    private float selectedCount;

    protected override void Start()
    {
        base.Start();
        spriteItem.sprite = item.icone;
    }


    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);

        clickCount = 0;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if(eventData.clickCount == 2)
        {
            BauUI.bauUI.AddItemInventario(item);
            BauUI.bauUI.CloseDescricao();
        }

    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        dentro = false;

        BauUI.bauUI.CloseDescricao();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        dentro = true;

        StartCoroutine(MouseTimeOver());
    }

    bool dentro = false;

    IEnumerator MouseTimeOver()
    {

        yield return new WaitForSecondsRealtime(1);
        if (dentro)
        {
            BauUI.bauUI.ShowDescricao(item);
        }
    }


    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        BauUI.bauUI.AddItemInventario(item);
        BauUI.bauUI.CloseDescricao();
    }

}
