using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawQuesGizmo : MonoBehaviour
{
    Condicoes condHolder;

    List<GameObject> instanciados = new List<GameObject>();

    public void SetCondicaoOnHolder(Condicoes c)
    {
        condHolder = c;
    }
    private void OnDrawGizmos()
    {
        if (condHolder != null)
        {
            Gizmos.color = GizmoColor();
            Gizmos.DrawWireSphere(condHolder.local, condHolder.distanciaChegada);
            CondicaoInstatiantes();
        }
    }

    Color GizmoColor()
    {
        Color retorno = Color.black;
        if (condHolder.tipoCondicao == Condicoes.TipoCondicao.COMBATE)
        {
            retorno = Color.red;
        }

        if (condHolder.tipoCondicao == Condicoes.TipoCondicao.IDA)
        {
            retorno = Color.yellow;
        }

        if (condHolder.tipoCondicao == Condicoes.TipoCondicao.INTERACAO)
        {
            retorno = Color.white;
        }

        if (condHolder.tipoCondicao == Condicoes.TipoCondicao.PEGA_ITEM)
        {
            retorno = Color.magenta;
        }
        
        return retorno;
    }


    void CondicaoInstatiantes()
    {
        if (instanciados.Count == 0)
        {
            if ((condHolder.tipoCondicao == Condicoes.TipoCondicao.INTERACAO || condHolder.tipoCondicao == Condicoes.TipoCondicao.DEVOLVE_ITEM) && condHolder.interagivel != null)
            {
                if (!condHolder.IsOnScene())
                {
                    instanciados.Add((GameObject)Instantiate(condHolder.itemDaCondicao.gameObject, condHolder.local, Quaternion.identity));
                }
                else
                {
                    Clean();
                }
            }

            if (condHolder.tipoCondicao == Condicoes.TipoCondicao.COMBATE && condHolder.inimigosDaCondicao.Count>0)
            {
                foreach(GameObject inimigo in condHolder.inimigosDaCondicao)
                {
                    instanciados.Add(Instantiate(inimigo, condHolder.SpawnRandomico(), Quaternion.identity));
                }
            }           

            if ((condHolder.tipoCondicao == Condicoes.TipoCondicao.PEGA_ITEM || condHolder.tipoCondicao == Condicoes.TipoCondicao.DEVOLVE_ITEM) && condHolder.itemDaCondicao != null)
            {
                instanciados.Add((GameObject)Instantiate(condHolder.itemDaCondicao.gameObject, condHolder.local, Quaternion.identity));
            }

            AparentarInstanciados();
        }
    }

    void AparentarInstanciados()
    {
        foreach(GameObject go in instanciados)
        {
            go.transform.SetParent(transform);
        }
    }

    public void Clean()
    {
        foreach (GameObject destruir in instanciados)
        {
            DestroyImmediate(destruir);
        }
    }

    private void Awake()
    {
        Clean();
        Destroy(this.gameObject);
    }
}
