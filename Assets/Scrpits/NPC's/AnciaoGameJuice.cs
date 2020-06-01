using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnciaoGameJuice : MonoBehaviour
{
    [SerializeField]
    private GameObject cadeira;
    [SerializeField]
    private GameObject pontoObservar;
    [SerializeField]
    private NavMeshAgent navMesh;

    private bool sentar;
    [SerializeField]
    private Animator anim;
    void Start()
    {
        StartCoroutine(Escolha());
    }

    private IEnumerator Escolha()
    {
        if (true)
        {
            anim.SetBool("Idle", false);
            navMesh.SetDestination(cadeira.transform.position);
            sentar = true;
        }
        else
        {
            yield return new WaitForSeconds(20f);
            StartCoroutine(Escolha());
        }
        yield return new WaitForSeconds(0);
    }
    private void Update()
    {
        if (sentar && Vector3.Distance(this.gameObject.transform.position, cadeira.gameObject.transform.position) <= 0.58f)
        {
            navMesh.isStopped = true;
            anim.SetBool("Idle",true);
            StartCoroutine(Sentar());
        }
    }
    private IEnumerator Sentar()
    {
        cadeira.GetComponent<BoxCollider>().isTrigger = true;
        anim.SetTrigger("Sentar");
        sentar = false;
        yield return new WaitForSeconds(1f);
        this.gameObject.transform.LookAt(pontoObservar.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            cadeira.GetComponent<BoxCollider>().isTrigger = false;
            anim.SetBool("PlayerPerto",true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            cadeira.GetComponent<BoxCollider>().isTrigger = true;
            anim.SetBool("PlayerPerto", false);
            StartCoroutine(Escolha());
        }
    }
}
