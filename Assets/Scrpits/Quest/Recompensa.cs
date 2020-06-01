using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Recompensa
{
    [SerializeField]
    Item item;
    [SerializeField]
    private int qntDinheiro;
    [SerializeField]
    private int XP;
    [SerializeField]
    private int fama;

    public int GetFama()
    {
        return fama;
    }

    public Item GetItem()
    {
        return item;
    }

    public int GetDinheiro()
    {
        return qntDinheiro;
    }

    public int GetXP()
    {
        return XP;
    }

    public void DarRecompensa()
    {
        if(item != null)
        {
            Inventario.inventario.AddItem(item);
        }

        Player.player.status.Dinheiro += qntDinheiro;
        Player.player.status.Experiencia += XP;
    }

}
