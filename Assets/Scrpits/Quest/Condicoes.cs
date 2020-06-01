using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Condicoes  
{
    public enum TipoCondicao { IDA, FALA, COMBATE, INTERACAO, PEGA_ITEM, DEVOLVE_ITEM,};

    public TipoCondicao tipoCondicao;

    public Vector3 local;
    public string descricao;

    #region HIDDENVARIABLES
    [SerializeField]
    private Dialogo dialogoDaCondição;
    [SerializeField]
    private bool isOnScene;
    [SerializeField]
    private string nameOnScene;

    public ItemPickup itemDaCondicao;

    public List<GameObject> inimigosDaCondicao;
    [SerializeField]
    private float raioDeSpawn = 2;

    public float distanciaChegada = 5;

    public Interagivel interagivel;

    #endregion

    GameObject inimigoParent;

    public bool IsOnScene()
    {
        return isOnScene;
    }


    public void CleanUnsusedConditions()
    {
        if(tipoCondicao != TipoCondicao.COMBATE)
        {
            inimigosDaCondicao.Clear();
        }

        if (tipoCondicao != TipoCondicao.PEGA_ITEM && tipoCondicao != TipoCondicao.DEVOLVE_ITEM)
        {
            itemDaCondicao = null;
        }

        if(tipoCondicao != TipoCondicao.INTERACAO && tipoCondicao != TipoCondicao.DEVOLVE_ITEM)
        {
            interagivel = null;
        }
    }

    public bool ChecaCondicao()
    {
        bool volta = false;

        if(tipoCondicao == TipoCondicao.IDA)
        {
            volta = CheckIda();
        }

        if(tipoCondicao == TipoCondicao.COMBATE)
        {
            volta = CheckCombate();
        }

        if(tipoCondicao == TipoCondicao.INTERACAO)
        {
            volta = CheckInteracao();
        }

        if(tipoCondicao == TipoCondicao.PEGA_ITEM)
        {
            volta = itemPego;
        }

        if(tipoCondicao == TipoCondicao.DEVOLVE_ITEM)
        {
            volta = CheckDevolveItem();
        }

        if(tipoCondicao == TipoCondicao.FALA)
        {
            volta = CheckFala();
        }

        return volta;
    }

    #region IDA
    private bool CheckIda()
    {
        if (Vector3.Distance(Player.player.transform.position, local) < distanciaChegada)
        {
            return true;
        }

        return false;
    }
    #endregion

    #region COMBATE
    private bool CheckCombate()
    {
        bool retorno = false;
        if (inimigoParent.transform.childCount <= 0)
        {
            retorno = true;
        }
        return retorno;
    }
    #endregion

    #region INTERACAO
    bool interagiu = false;
    private bool CheckInteracao()
    {
        return interagiu;
    }

    private void OnInteracao(Interagivel interagido)
    {
        if(interagido == interagivel)
        {
            interagiu = true;
            if(tipoCondicao == TipoCondicao.DEVOLVE_ITEM)
            {
                Inventario.inventario.RemoverItem(itemDaCondicao.item);
            }
        }
    }
    #endregion

    #region PEGA_ITEM

    bool itemPego = false;

    private void OnItemPego(Item item)
    {
        if (item == itemDaCondicao.item)
        {
            itemPego = true;
            return;
        }
        itemPego = false;
    }

    #endregion

    #region DEVOLVE_ITEM
    // a parte mais importante desse código está na região de interação, pois aqui foi reutilizada sua estrutula lógica.

    private bool CheckDevolveItem()
    {
        return interagiu;
    }

    #endregion

    #region FALA
    bool falou = false;

    bool CheckFala()
    {
        return falou;
    }

    void OnFalaTerminada(Dialogo dialogo)
    {
        if(dialogo.whosDialog == this.descricao)
        {
            falou = true;
        }
    }

    #endregion

    public Vector3 SpawnRandomico()
    {
        Vector3 loc = local + (Random.insideUnitSphere * raioDeSpawn);
        loc.y = local.y;
        return loc;
    }

    public void AtivarCondicao()
    {      
        if (inimigosDaCondicao.Count>0) {

            inimigoParent = new GameObject("InimigoHolder " + descricao);
            Transform parentao = GameObject.Find("CondHolder").transform;
            foreach (GameObject inimigo in inimigosDaCondicao)
            {
                GameObject inimigoInstanciado = MonoBehaviour.Instantiate(inimigo, SpawnRandomico(), Quaternion.identity);
                inimigoInstanciado.transform.SetParent(inimigoParent.transform);
            }
        }

        if(tipoCondicao == TipoCondicao.INTERACAO || tipoCondicao == TipoCondicao.DEVOLVE_ITEM || tipoCondicao == TipoCondicao.FALA)
        {
            interagiu = false;
            if (!isOnScene)
            {
                GameObject interagivelObj = MonoBehaviour.Instantiate(interagivel.gameObject, local, interagivel.gameObject.transform.rotation);
                interagivel = interagivelObj.GetComponent<Interagivel>();       
            }
            else
            {
                interagivel = GameObject.Find(nameOnScene).GetComponent<Interagivel>();
                local = interagivel.transform.position;
            }
            EventsController.onInteracao += OnInteracao;
        }

        if(tipoCondicao == TipoCondicao.FALA)
        {
            falou = false;
            dialogoDaCondição.whosDialog = this.descricao;
            interagivel.SetDialogoCondicao(dialogoDaCondição);
            EventsController.onDialogoTerminado += OnFalaTerminada;
        }

        if(tipoCondicao == TipoCondicao.PEGA_ITEM)
        {
            itemPego = false;
            EventsController.onItemPego += OnItemPego;
            GameObject itemObj = MonoBehaviour.Instantiate(itemDaCondicao.gameObject);
            itemObj.transform.position = local;          
        }
    }
}