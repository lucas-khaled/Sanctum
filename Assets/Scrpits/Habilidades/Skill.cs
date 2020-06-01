using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class Skill
{
    public TipoPlayer player;
    public string ObjectDeAcesso;
    public string VariavelDeAcesso;
    public float qntMudança;
    public string mudançaSeString;

    [TextArea]
    public string descricaoHabilidade;

    private bool ativo = false;

    public bool IsActive()
    {
        return ativo;
    }


    public bool podeAtivar{ get; set; }

    public void AtivaSkill()
    {
        ativo = true;

        var objAcesso = GameObject.FindObjectOfType(Type.GetType(ObjectDeAcesso));

        PropertyInfo varAcesso = objAcesso.GetType().GetProperty(VariavelDeAcesso);

        if(varAcesso == null)
        {
            return;
        }

        if (varAcesso.PropertyType == typeof(int))
        {
            varAcesso.SetValue(objAcesso, Mathf.CeilToInt(qntMudança), null);
        }

        else if(varAcesso.PropertyType == typeof(float))
        {
            varAcesso.SetValue(objAcesso, qntMudança, null);
        }

        else if (varAcesso.PropertyType == typeof(bool))
        {
            bool muda = (qntMudança >= 1) ? true : false;
            varAcesso.SetValue(objAcesso, muda);
        }

        else if(varAcesso.PropertyType == typeof(string))
        {
            varAcesso.SetValue(objAcesso, mudançaSeString, null);
        }

        podeAtivar = false;
    }
}
