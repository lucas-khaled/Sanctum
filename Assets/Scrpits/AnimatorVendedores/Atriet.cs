using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Atriet : MonoBehaviour
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

    [SerializeField]
    Dialogo dialogoInteracao;

    public int contVenderComprar;

    Vector3 finalPosition;
    Collider maisPerto;
    Vector3 destino;
    NavMeshHit hit;

    private void Update()
    {
        if (!anim.GetBool("Idle"))
        {
            if (Vector3.Distance(this.gameObject.transform.position, destino) < 0.3f)
            {
                anim.SetBool("Idle", true);
                vendedor.isStopped = true;

                StartCoroutine(Escolha());
            }
        }
        if (playerPerto)
        {
            anim.SetBool("Idle", true);
            vendedor.isStopped = true;
        }
    }
    private void Start()
    {
        StartCoroutine(Escolha());
    }
    private void Andar(Vector3 destino, bool move = true)
    {
        anim.SetBool("Idle", false);
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
        if (random > 75 && !playerPerto)
        {
            Andar(RandomNavMeshGenerator(4f));
        }
        else if (random > 50 && !playerPerto)
        {
            anim.SetTrigger("Soneca");
            yield return new WaitForSeconds(5f);
            StartCoroutine(Escolha());
        }
        else if (random > 30 && !playerPerto)
        {
            anim.SetTrigger("Alongar");
            yield return new WaitForSeconds(5f);
            StartCoroutine(Escolha());
        }
        else
        {
            yield return new WaitForSeconds(5f);
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

        StartCoroutine(Escolha());
    }

    public void Conversa()
    {
         anim.SetTrigger("Interacao");
         DialogueSystem.sistemaDialogo.IniciaDialogo(dialogoInteracao);
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
            anim.ResetTrigger("Soneca");
            anim.ResetTrigger("Alongar");
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
            anim.SetBool("PlayerPerto", false);
            StartCoroutine(Escolha());
            playerPerto = false;
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
