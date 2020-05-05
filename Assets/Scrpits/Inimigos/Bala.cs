using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Bala : MonoBehaviour
{
    public float velocidadeBala;
    public int danoBala;
    public string targetTag;

    Collider casterCollider;
    Rigidbody rb;

    public void SetCasterCollider(Collider caster)
    {
        casterCollider = caster;
    }

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        Destroy(this.gameObject, 4);
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = this.transform.forward * velocidadeBala;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other == casterCollider)
            return;

        if (other.gameObject.CompareTag(targetTag) && Player.player.estadoPlayer != EstadoPlayer.MORTO)
        {
            Debug.Log("Acertei o certo");
            other.GetComponent<IVulnerable>().ReceberDano(danoBala);
        }

        Destroy(this.gameObject);
        
    }
}
