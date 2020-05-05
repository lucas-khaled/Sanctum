using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baú : Interagivel
{
    public List<Item> itens = new List<Item>();
    private GameObject player;
    [SerializeField]
    Animator anim;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public override void Interact()
    {
        if (Inventario.inventario.AddItem(itens[0]))
        {
            anim.SetBool("Aberto", true);
            Debug.Log("IEI");
        }
    }
}
