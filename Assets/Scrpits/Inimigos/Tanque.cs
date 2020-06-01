using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanque : Inimigo
{
    [SerializeField]
    private float cooldownGeral = 1.5f;
    public GameObject escudo;

    private bool choose = true;
    public override void ReceberDano(int danoRecebido)
    {
        if(!escudo.activeSelf)
            base.ReceberDano(danoRecebido);
    }

    protected override IEnumerator Atacar()
    {
        choose = false;
        Coroutine c = StartCoroutine(base.Atacar());

        yield return c;
        Debug.Log("Ataque Tanque");

        anim.SetBool("Idle", true);
        yield return new WaitForSeconds(cooldownGeral);
        choose = true;
    }

    IEnumerator Defender()
    {
        ataqueCooldown = velocidadeAtaque;
        anim.SetBool("Defendendo", true);
        escudo.SetActive(true);
        choose = false;

        yield return new WaitForSeconds(2);

        anim.SetBool("Defendendo", false);
        escudo.SetActive(false);

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
            bool mover = true;

            float distancia = Vector3.Distance(collider.gameObject.transform.position, gameObject.transform.position);

            Debug.Log(distancia);
            if (distancia <= distanciaAtaque)
            {
                anim.SetBool("Idle", true);
                mover = false;
                if (ataqueCooldown <= 0 && choose)
                    EscolheAcao();
            }

            ataqueCooldown -= Time.deltaTime * 1;
            Movimentar(collider.transform.position, mover);
        }
    }
}
