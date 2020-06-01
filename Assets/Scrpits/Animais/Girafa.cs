using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Girafa : MonoBehaviour
{
    [Header("Referências")]
    public Image lifeBar;
    public Animator anim;
    public GameObject CBTprefab;
    public NavMeshAgent animal;
    Vector3 finalPosition;
    Collider maisPerto;
    Vector3 destino;
    NavMeshHit hit;

    //Criar Array de Itens para ele dropar
    [Header("Valores")]
    [SerializeField]
    private int maxVida;
    [SerializeField]
    private float velocidade;
    [SerializeField]
    private int vida;
    [SerializeField]
    private int experienciaMorte;

    private Transform hitCanvas;

    //auxiliares
    private float aux = 0;
    private bool vaicomer;
    private int corrida;
    private bool correndo;

    private void Update()
    {
        if (anim.GetBool("Caminhando")){
            if (Vector3.Distance(this.gameObject.transform.position, destino) < 1f)
            {
                anim.SetBool("Caminhando", false);
                StartCoroutine(Escolher());
            }
        }
        if (vaicomer)
        {
            if ((Vector3.Distance(this.gameObject.transform.position, destino) < 5f))
            {
                animal.SetDestination(this.gameObject.transform.position);
                this.gameObject.transform.LookAt(destino);
                StartCoroutine(Comer());
            }
            vaicomer = false;
        }
        if (correndo && corrida > 0)
        {
            if (Vector3.Distance(this.gameObject.transform.position, destino) < 1.5f)
            {
                destino = RandomNavMeshGenerator(20f);
                animal.SetDestination(destino);
                corrida--;
            }
        }
        else if (correndo && corrida <= 0)
        {
            GetComponent<NavMeshAgent>().speed = 0.8f;
            anim.SetBool("Correndo", false);
            correndo = false;
        }
    }
    private IEnumerator Escolher()
    {
        int escolha = Random.Range(0, 4);
        if (escolha == 0)
        {
            Debug.Log("ANDAAA");
            destino = RandomNavMeshGenerator(10f);
            Movimentar(destino);
        }
        else if (escolha == 2)
        {
            Debug.Log("Soninho");
            StartCoroutine(Deitado());
        }
        else if (escolha == 3)
        {
            Debug.Log("COMEEE");
            Collider[] arvore = Physics.OverlapSphere(this.transform.position, 15f, LayerMask.GetMask("Arvore"));
            if (arvore.Length > 0)
            {
                maisPerto = arvore[0];
                for (int i = 1; i < arvore.Length; i++)
                {
                    if (Vector3.Distance(maisPerto.transform.position, this.gameObject.transform.position) < Vector3.Distance(arvore[i].transform.position, this.gameObject.transform.position))
                    {
                        maisPerto = arvore[i];
                    }
                }
                vaicomer = true;
                destino = maisPerto.transform.position;
                Movimentar(destino);
            }
            else
            {
                StopCoroutine(Escolher());
                StartCoroutine(Escolher());
            }
        }
        else if (escolha == 1)
        {
            Debug.Log("STO DE BOA");
            yield return new WaitForSeconds(13f);
            StartCoroutine(Escolher());
        }
    }
    private void Start()
    {
        StartCoroutine(Escolher());
        Vida = maxVida;
        hitCanvas = transform.Find("Hit_life");
        anim.SetFloat("Vida", maxVida);
    }
    public int Vida
    {
        get { return vida; }

        private set
        {
            vida = Mathf.Clamp(value, 0, maxVida);
            UIController.uiController.LifeBar(lifeBar, ((float)vida / maxVida));
        }
    }
    public IEnumerator Deitado()
    {
        anim.SetBool("Deitado", true);
        yield return new WaitForSeconds(20f);
        anim.SetTrigger("Levantar");
        anim.SetBool("Deitado", false);
        yield return new WaitForSeconds(3f);
        destino = RandomNavMeshGenerator(10f);
        yield return new WaitForSeconds(0.5f);
        Movimentar(destino);
        StartCoroutine(Escolher());
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
    void Morrer()
    {
        StopAllCoroutines();
        animal.Stop();
        Debug.Log("Morri");
    }
    public virtual void ReceberDano(int danoRecebido)
    {
        Vida -= danoRecebido;

        //chama metodo do UIController para exibir o dano no worldCanvas
        UIController.uiController.InitCBT(danoRecebido.ToString(), CBTprefab, hitCanvas);


        if (vida <= 0)
        {
            anim.SetTrigger("Tomar dano");
            anim.SetTrigger("Morrer");
            Morrer();
        }
        else
        {
            anim.SetBool("Deitado", false);
            anim.SetTrigger("Tomar dano");
            anim.SetFloat("Vida", vida);
            Correr();
        }

    }
    protected void Movimentar(Vector3 destino, bool move = true)
    {
        anim.SetBool("Caminhando", true);
        if (!move)
        {
            animal.isStopped = true;
            return;
        }

        animal.isStopped = false;
        animal.SetDestination(destino);
    }
    private void Awake()
    {
        // checa se há um canvas como filho, se não houver, cria um filho worldSpace Canvas e o nomea adequadamente.
        if (transform.GetComponentInChildren<Canvas>() == null)
        {
            GameObject hitLife = new GameObject();
            hitLife.transform.SetParent(this.transform);
            hitLife.transform.position = new Vector3(0, 1, 0);
            hitLife.name = "Hit_Life";
            hitLife.AddComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    private void Correr()
    {
        anim.SetBool("Correndo", true);
        correndo = true;
        corrida = Random.Range(2, 6);
        GetComponent<NavMeshAgent>().speed = 4;
        destino = RandomNavMeshGenerator(20f);
        animal.SetDestination(destino);
    }
    private void OnCollisionExit(Collision collision)
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }
    private IEnumerator Comer()
    {
        anim.SetBool("Caminhando", false);
        anim.SetBool("Comendo", true);
        yield return new WaitForSeconds(9f);
        anim.SetBool("Comendo",false);
        StartCoroutine(Escolher());
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, 20f);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, 2);
    }
}
