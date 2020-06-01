using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmaPlayer : MonoBehaviour
{
    [SerializeField]
    private int Dano;

    public SkinnedMeshRenderer mesh;

    private void Start()
    {
        if (gameObject.activeSelf)
            Inventario.inventario.armaMesh = mesh;
    }
    private int CalculaDano()
    {
        return Dano + Inventario.inventario.armaEquipada.dano + Random.Range(-5, 5);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Inimigo" && (Player.player.estadoPlayer == EstadoPlayer.ATACANDO || Player.player.estadoPlayer == EstadoPlayer.RECARREGAVEL) && !other.isTrigger)
        {
            Debug.Log("Vai tomar no cuuuuuuuuuuuu");
            other.gameObject.GetComponent<Inimigo>().ReceberDano(CalculaDano());
        }
        if (other.gameObject.tag == "Girafa" && (Player.player.estadoPlayer == EstadoPlayer.ATACANDO || Player.player.estadoPlayer == EstadoPlayer.RECARREGAVEL) && !other.isTrigger)
        {
            Debug.Log("Vai tomar no cuuuuuuuuuuuu");
            other.gameObject.GetComponent<Girafa>().ReceberDano(CalculaDano());
        }
    }
}
