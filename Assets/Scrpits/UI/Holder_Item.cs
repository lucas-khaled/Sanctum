using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Holder_Item : Button, ISelectHandler
{

    public Item item;
    public Image Spritu_item;

    public bool isLoja = false;

    protected override void Start()
    {
        base.Start();
        if (item.icone != null)
        {
            Spritu_item.sprite = item.icone;
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        if (!isLoja)
        {
            InventarioUI.inventarioUI.ShowInfo(item);
            InventarioUI.inventarioUI.ApareceExcluir();
            if (item.GetType() == typeof(Arma))
            {
                Arma arma = (Arma)item;
                if (GameController.gameController.GetPersonagemEscolhido() == arma.armaPlayer)
                {
                    InventarioUI.inventarioUI.ApareceEquipar();
                }
            }

            else if(item.GetType() == typeof(Cura))
            {
                InventarioUI.inventarioUI.ApareceUsar();
            }
        }

        else
        {
            LojaUI.lojaUi.SetItemSelecionado(this);
        }
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);

        if (!isLoja)
        {
            InventarioUI.inventarioUI.ClearOpcoes();
        }

        else
        {
            LojaUI.lojaUi.DeselectItem();
        }
    }
}
