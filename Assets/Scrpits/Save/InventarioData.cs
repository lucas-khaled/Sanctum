using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventarioData
{
    public Item[] itens;
    public Arma armaEquipada;

    public BauSave[] baus;

    public InventarioData(Inventario inventory)
    {
        itens = inventory.Getitens().ToArray();
        armaEquipada = inventory.armaEquipada;

        List<Bau> bausNaCena = new List<Bau>();

        foreach(Bau b in GameObject.FindObjectsOfType<Bau>())
        {
            if (b.someSeVazia)
                continue;

            bausNaCena.Add(b);
        }

        baus = new BauSave[bausNaCena.Count];

        for(int i=0; i<bausNaCena.Count; i++)
        {
            baus[i] = new BauSave(bausNaCena[i].name, bausNaCena[i].itens.ToArray());        
        }
    }

    [System.Serializable]
    public struct BauSave
    {
        public string nomeBau;
        public Item[] itens;

        public BauSave(string nome, Item[] itens)
        {
            nomeBau = nome;
            this.itens = itens;
        }
    }
}
