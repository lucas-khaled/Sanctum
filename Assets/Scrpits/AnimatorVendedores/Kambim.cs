using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Kambim : MonoBehaviour
{
    [Header("Audios")]
    [SerializeField]
    private AudioClip venda;
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private Animator anim;
    [SerializeField]
    private NavMeshAgent vendedor;
    private bool playerPerto;

    public int contVenderComprar;

    [SerializeField]
    private Dialogo dialogoDormindo;
    [SerializeField]
    private Dialogo dialogoAcordado;

    Vector3 finalPosition;
    Collider maisPerto;
    Vector3 destino;
    NavMeshHit hit;

    private void Update()
    {
        if (anim.GetBool("Andando"))
        {
            if (Vector3.Distance(this.gameObject.transform.position, destino) < 0.3f)
            {
                anim.SetBool("Andando", false);
                vendedor.isStopped = true;
                StartCoroutine(Escolha());
            }
        }
        if (playerPerto)
        {
            anim.SetBool("Andando", false);
            vendedor.isStopped = true;
        }
    }
    private void Start()
    {
        if (Random.Range(0, 2) == 0)
        {
            anim.SetBool("Idle2", false);
        }
        else
        {
            anim.SetBool("Idle2", true);
        }
        StartCoroutine(Escolha());
    }
    private void Andar(Vector3 destino, bool move = true)
    {
        anim.SetBool("Andando", true);
        if (!move)
        {
            vendedor.isStopped = true;
            return;
        }

        vendedor.isStopped = false;
        this.destino = destino;
        vendedor.SetDestination(destino);
    }
    private IEnumerator Escolha()
    {
        int random = Random.Range(0, 100);
        if (random > 90 && !playerPerto && !anim.GetBool("Dormindo"))
        {
            anim.SetTrigger("Dormir");
            anim.SetBool("Dormindo", true);

        }
        else if (random > 70 && !playerPerto && !anim.GetBool("Dormindo"))
        {
            Andar(RandomNavMeshGenerator(4f));
        }
        else if (random > 30)
        {
            anim.SetBool("Idle2", false);
        }
        else
        {
            yield return new WaitForSeconds(10f);
            StartCoroutine(Escolha());
        }
    }
    
    public void FimInteracao()
    {
        contVenderComprar = LojaUI.lojaUi.contCompraVenda;

        audioSource.PlayOneShot(venda);

        if (contVenderComprar > 0)
            anim.SetTrigger("Compra");
        else if (contVenderComprar < 0)
            anim.SetTrigger("Venda");
        else if (Random.Range(0, 2) == 0)
            anim.SetTrigger("Compra");
        else
            anim.SetTrigger("Venda");

    }

    public void Conversa()
    {
        if (anim.GetBool("Dormindo"))
        {
            anim.SetBool("Dormindo", false);
            DialogueSystem.sistemaDialogo.IniciaDialogo(dialogoDormindo);
            anim.ResetTrigger("Cumprimentar");
        }
        else
        {
            DialogueSystem.sistemaDialogo.IniciaDialogo(dialogoAcordado);
            anim.SetTrigger("Interacao");
        }
    }

    public Vector3 RandomNavMeshGenerator(float raioCaminhada)
    {
        Vector3 randomDirection = Random.insideUnitSphere * raioCaminhada;
        randomDirection += this.gameObject.transform.position;
        NavMesh.SamplePosition(randomDirection, out hit, raioCaminhada, 1);

        if (NavMesh.SamplePosition(randomDirection, out hit, raioCaminhada, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            vendedor.isStopped = true;
            anim.SetBool("Andando", false);
            anim.SetTrigger("Cumprimentar");
            playerPerto = true;
            anim.SetBool("PlayerPerto", true);
        }
        StopCoroutine(Escolha());
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            anim.ResetTrigger("Dormir");
            playerPerto = false;
            anim.SetBool("PlayerPerto", false);
            StartCoroutine(Escolha());
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
