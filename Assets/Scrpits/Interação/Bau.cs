using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bau : Interagivel
{
    public List<Item> itens = new List<Item>();
    [SerializeField]
    Animator anim;

    public override void Interact()
    {
        base.Interact();

        if (anim != null)
        {
            anim.SetBool("Aberto", true);
        }


        if (Inventario.inventario.AddItem(itens[0]))
        {
            itens.RemoveAt(0);
            Debug.Log("IEI");
        }
    }
}
