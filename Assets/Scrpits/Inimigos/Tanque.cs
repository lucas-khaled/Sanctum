using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanque : Inimigo
{
    [Header("Audios")]
    [SerializeField]
    private AudioClip defesa;

    [SerializeField]
    private float cooldownGeral = 1.5f;

    private bool choose = true, defendendo = false;
    public override void ReceberDano(int danoRecebido, Inimigo inim = null)
    {
        if (!defendendo)
            base.ReceberDano(danoRecebido);
        else
            audioSource.PlayOneShot(defesa);
    }

    protected override IEnumerator Atacar()
    {
        choose = false;
        Coroutine c = StartCoroutine(base.Atacar());

        yield return c;

        anim.SetBool("Idle", true);
        yield return new WaitForSeconds(cooldownGeral);
        choose = true;
    }

    IEnumerator Defender()
    {
        ataqueCooldown = velocidadeAtaque;
        anim.SetBool("Defendendo", true);
        choose = false;
        defendendo = true;

        yield return new WaitForSeconds(2);

        anim.SetBool("Defendendo", false);
        defendendo = false;

        yield return new WaitForSeconds(cooldownGeral);

        choose = true;
    }

    void EscolheAcao()
    {
        //talvez colocar probabilidade de acordo com a vida do tanque. 12/04/2020

        int chance = Mathf.CeilToInt(Random.Range(1, 100));

        if (chance >= 50)
            StartCoroutine(Atacar());
        else
        {
            StartCoroutine(Defender());
        }
    }

    protected override void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && hostil && Player.player.estadoPlayer != EstadoPlayer.MORTO)
        {
            anim.SetBool("Idle", false);
            bool mover = false;

            if (!defendendo)
            {
                mover = true;
            }

            float distancia = Vector3.Distance(collider.gameObject.transform.position, gameObject.transform.position);

            Debug.Log(distancia);
            if (distancia <= distanciaAtaque)
            {
                anim.SetBool("PertoPlayer", true);
                mover = false;
                if (ataqueCooldown <= 0 && choose)
                    EscolheAcao();
            }
            else
            {
                anim.SetBool("PertoPlayer", false);
            }

            ataqueCooldown -= Time.deltaTime * 1;
            Movimentar(collider.transform.position, mover);
        }
    }
}
