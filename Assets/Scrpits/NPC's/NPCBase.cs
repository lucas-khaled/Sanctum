using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCBase : Interagivel
{
    [SerializeField]
    protected Animator anim;
    //[SerializeField]
    //protected int maxVida;
    //[SerializeField]
    //protected int vida;

    [SerializeField]
    private bool idleDuplo;

    protected bool playerPerto;
    protected int fama;
    private NavMeshAgent navMesh;
    protected StatusPlayer statusPl;
    /*Vector3 finalPosition;
    Collider maisPerto;
    Vector3 destino;
    NavMeshHit hit;*/

    [SerializeField]
    private Dialogo dialogoFamaBaixa;
    [SerializeField]
    private Dialogo dialogoFamaAlta;

    /*protected virtual void Update()
    {
        if (Vector3.Distance(this.gameObject.transform.position, destino) < 0.15f)
        {
            if (!anim.GetBool("Idle"))
            {
                anim.SetBool("Idle", false);
                StartCoroutine(Escolher());
            }
            
        }
    }*/

    /*public Vector3 RandomNavMeshGenerator(float raioCaminhada)
    {
        Vector3 randomDirection = Random.insideUnitSphere * raioCaminhada;
        randomDirection += this.gameObject.transform.position;
        NavMesh.SamplePosition(randomDirection, out hit, raioCaminhada, 1);

        if (NavMesh.SamplePosition(randomDirection, out hit, raioCaminhada, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }*/
    protected override void Start()
    {
        base.Start();
        this.statusPl = Player.player.status;
        if (idleDuplo)
            StartCoroutine(Escolher());
        //this.navMesh = this.GetComponent<NavMeshAgent>();
    }
    public override void Interact()
    {
        base.Interact();
        if (fama < 200)
        {
            DialogueSystem.sistemaDialogo.IniciaDialogo(dialogoFamaBaixa);
        }
        else
        {
            DialogueSystem.sistemaDialogo.IniciaDialogo(dialogoFamaAlta);
        }
    }

    protected IEnumerator Escolher()
    {
        int random = Random.Range(0, 100);
        if (random < 40 && anim.GetFloat("Idle") == 0)
        {
            anim.SetTrigger("MudarParaEsquerda");
            anim.SetFloat("Idle", 1);
        }
        else if (random < 80 && anim.GetFloat("Idle") == 1)
        {
            anim.SetTrigger("MudarParaDireita");
            anim.SetFloat("Idle", 0);
        }

        yield return new WaitForSeconds(15f);

        StartCoroutine(Escolher());

    }
    

    /*protected virtual IEnumerator Escolher()
    {
        if (Random.Range(0, 100) > 75 && !playerPerto)
        {
            Andar(RandomNavMeshGenerator(4f));
        }

        yield return new WaitForSeconds(15f);

        StartCoroutine(Escolher());
    }*/

    /*protected virtual void Andar(Vector3 destino)
    {
        anim.SetBool("Idle", false);
        anim.SetBool("Corrida", false);

        navMesh.isStopped = false;
        navMesh.SetDestination(destino);
    }*/
    /*protected virtual void Correr(Vector3 destino)
    {
        anim.SetBool("Idle", false);
        anim.SetBool("Corrida", true);

        navMesh.isStopped = false;
        navMesh.SetDestination(destino);
    }*/
    /*protected void Morrer()
    {
        StopAllCoroutines();
        navMesh.isStopped = true;
        Debug.Log("Morri");
    }*/
    /*public void ReceberDano(int danoRecebido)
    {
        vida -= danoRecebido;

        if (vida <= 0)
        {
            anim.SetTrigger("Tomar dano");
            anim.SetTrigger("Morrer");
            Morrer();
        }
        else
        {
            anim.SetTrigger("Tomar dano");
            anim.SetFloat("Vida", vida);
        }

    }*/
    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            fama = statusPl.Fama;
            this.transform.LookAt(other.transform);
            if (fama < 200)
            {
                anim.SetTrigger("InteracaoRuim");
            }
            else
            {
                anim.SetTrigger("InteracaoBoa");
            }
            playerPerto = true;
        }
    }
    protected void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            this.transform.LookAt(other.transform);
        }
    }
    protected void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerPerto = false;
            navMesh.isStopped = false;
        }
    }
}
