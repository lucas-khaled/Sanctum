using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distancia : Inimigo
{
    [Header("Audios")]
    [SerializeField]
    private AudioClip tiro;

    public GameObject imagemAlerta;
    public GameObject bala;
    public Transform pontoTiro;
    public Transform pontoAlerta;

    private GameObject alerta;
    protected override IEnumerator Atacar()
    {
        canAttack = false;

        Coroutine mira = StartCoroutine(Mirar());

        alerta = Instantiate(imagemAlerta, pontoAlerta);
        alerta.AddComponent<FaceCamera>();
        alerta.transform.SetParent(transform);

        yield return new WaitForSeconds(3f);

        Destroy(alerta);
        anim.SetTrigger("Atirar");
        audioSource.PlayOneShot(tiro);
        anim.SetBool("Atacar", false);
        StopCoroutine(mira);

        GameObject balaInstanciada = Instantiate(bala, pontoTiro.position, pontoTiro.transform.rotation);
        balaInstanciada.GetComponent<Bala>().SetCasterCollider(this.GetComponent<Collider>());
        

        yield return new WaitForSeconds(velocidadeAtaque);
        canAttack = true;

    }

    IEnumerator Mirar()
    {
        anim.SetBool("Atacar", true);

        while (true)
        {
            this.transform.LookAt(Player.player.transform);
            yield return null;
        }
    }

    protected override void Morrer()
    {
        base.Morrer();

        Destroy(GetComponentInChildren(typeof(FaceCamera), true));
    }
}
