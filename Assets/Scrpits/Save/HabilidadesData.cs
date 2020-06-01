using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HabilidadesData
{
    public int[] indiceAtivas;

    public HabilidadesData()
    {
        indiceAtivas = null;
        ArvoreDeHabilidades ar = GameObject.FindObjectOfType<ArvoreDeHabilidades>();

        if(ar!=null)
            indiceAtivas = ar.GetActualSkillsActivityIndexes();
    }
}
