using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Novo Item", menuName = "Inventário/Item")]
public class Item : ScriptableObject
{
    public string nome;
    [TextArea]
    public string descricao;

    public bool isItemMissao;
    public int custoMoeda;
    public float peso;

    public Sprite icone;
}
