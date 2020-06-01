using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventario : MonoBehaviour
{

    #region SINGLETON

    public static Inventario inventario;
    private void Awake()
    {
        inventario = this;
    }

    #endregion

    [HideInInspector]
    public SkinnedMeshRenderer armaMesh;

    public float pesoMaximo;   
    public Arma armaDefault;

    [HideInInspector]
    public Arma armaEquipada;

    List<Item> itens = new List<Item>();
    private float pesoInventario = 0;


    private void Start()
    {
        armaEquipada = armaDefault;
    }

    public bool AddItem(Item item)
    {
        bool inseriu = false;
        if (pesoInventario <= pesoMaximo)
        {
            itens.Add(item);
            pesoInventario += item.peso;
            inseriu = true;

            if (EventsController.onItemPego != null)
            {
                EventsController.onItemPego.Invoke(item);
            }
        }

        return inseriu;
    }

    public void RemoverItem(Item item)
    {
        itens.Remove(item);
        pesoInventario -= item.peso;
    }

    #region ARMA
    public void EquipArma(Arma arma)
    {
        if(Player.player.status.Level >= arma.nivelMinimo)
        {
            UnequipArma();
            armaEquipada = arma;
            itens.Remove(arma);
            armaMesh.sharedMesh = arma.armaMesh;
        }
    }
    
    public void UnequipArma()
    {
        if (armaEquipada != null)
        {
            armaMesh.sharedMesh = null;
            itens.Add(armaEquipada);
            armaEquipada = null;
        }
    }
    #endregion

    //so de teste. Apagar depois
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RemoverItem(itens[0]);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Arma arma= (Arma)itens.Find(x => x.GetType() == typeof(Arma));
            EquipArma(arma);
        }
    }
}
