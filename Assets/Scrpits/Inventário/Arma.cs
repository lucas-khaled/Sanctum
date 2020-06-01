using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TipoPlayer { TYVA, IOVELIK };

[CreateAssetMenu(fileName = "Nova Arma", menuName = "Inventário/Arma")]
public class Arma : Item
{
    public TipoPlayer armaPlayer;
    public int dano;
    public int nivelMinimo;
    public int famaMinima;
    public Mesh armaMesh;
}
