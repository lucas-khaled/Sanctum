using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVulnerable
{
    void ReceberDano(int dano, Inimigo inim = null);
}
