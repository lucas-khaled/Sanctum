﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class ArmaPlayer : MonoBehaviour
{
    [SerializeField]
    private int Dano;
    private BoxCollider colisor;


    public SkinnedMeshRenderer mesh;

    public static ArmaPlayer armaPlayer;

    private void Awake()
    {
        if (gameObject.activeSelf)
            Inventario.inventario.armaMesh = mesh;
    }

    private void Start()
    {
        armaPlayer = this;
        colisor = this.GetComponent<BoxCollider>();
    }

    public int CalculaDano()
    {
        return Dano + Inventario.inventario.armaEquipada.dano + Random.Range(-5, 5);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Inimigo" && (Player.player.estadoPlayer == EstadoPlayer.ATACANDO) && !other.isTrigger)
        {
            other.gameObject.GetComponent<Inimigo>().ReceberDano(CalculaDano());
            colisor.enabled = false;
        }
        if (other.gameObject.tag == "Girafa" && (Player.player.estadoPlayer == EstadoPlayer.ATACANDO) && !other.isTrigger)
        {
            other.gameObject.GetComponent<Girafa>().ReceberDano(CalculaDano());
            colisor.enabled = false;
        }
        if (other.gameObject.tag == "Tigre" && (Player.player.estadoPlayer == EstadoPlayer.ATACANDO) && !other.isTrigger)
        {
            other.gameObject.GetComponent<Tigre>().ReceberDano(CalculaDano());
            colisor.enabled = false;
        }
    }
}
