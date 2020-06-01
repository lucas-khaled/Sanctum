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
    public Arma armaDefaultIovelik;
    public Arma armaDefautTyva;

    [HideInInspector]
    public Arma armaEquipada;

    List<Item> itens = new List<Item>();
    public float pesoInventario = 0;


    void LoadInventario()
    {
        armaEquipada = SaveSystem.data.inventarioData.armaEquipada;
        armaMesh.sharedMesh = armaEquipada.armaMesh;

        foreach(Item item in SaveSystem.data.inventarioData.itens)
        {
            AddItem(item);
        }
        
    }

    private void Start()
    {
        if (GameController.gameController.IsLoadedGame())
        {
            LoadInventario();
        }
        else
        {
            if (GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.IOVELIK)
            {
                armaEquipada = armaDefaultIovelik;
            }
            else
            {
                armaEquipada = armaDefautTyva;
            }
        }
    }

    public List<Item> Getitens()
    {
        return itens;
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
            if(EventsController.onInventarioChange != null)
            {
                EventsController.onInventarioChange.Invoke(item, true);
            }
        }

        return inseriu;
    }

    public void RemoverItem(Item item)
    {
        itens.Remove(item);
        pesoInventario -= item.peso;
        if(EventsController.onInventarioChange != null)
        {
            EventsController.onInventarioChange.Invoke(item, false);
        }
    }

    #region ARMA
    public void EquipArma(Arma arma)
    {
        if(Player.player.status.Level >= arma.nivelMinimo)
        {
            UnequipArma();
            armaEquipada = arma;
            RemoverItem(arma);
            armaMesh.sharedMesh = arma.armaMesh;
        }
    }
    
    public void UnequipArma()
    {
        if (armaEquipada != null)
        {
            armaMesh.sharedMesh = null;
            AddItem(armaEquipada);
            armaEquipada = null;
        }
    }
    #endregion
}
