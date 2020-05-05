using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Caminhar : StateMachineBehaviour
{
    float aux;
    NavMeshHit hit;
    NavMeshAgent animal;
    Vector3 finalPosition;
    Collider maisPerto;
    Vector3 destino;

    public Vector3 RandomNavMeshGenerator(Animator animator, float raioCaminhada)
    {
        Vector3 randomDirection = Random.insideUnitSphere * raioCaminhada;
        randomDirection += animator.gameObject.transform.position;
        NavMesh.SamplePosition(randomDirection, out hit, raioCaminhada, 1);

        if (NavMesh.SamplePosition(randomDirection, out hit, raioCaminhada, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animal = animator.gameObject.GetComponent<NavMeshAgent>();
        Vector3 destino = RandomNavMeshGenerator(animator, 10);
        animal.SetDestination(destino);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector3.Distance(finalPosition, animator.gameObject.transform.position) < 1.5 && aux <= 0)
        {
            int escolha = Random.Range(1, 10);
            if (escolha == 1)
            {

                Collider[] arvore = Physics.OverlapSphere(animator.transform.position, 15f, LayerMask.GetMask("Arvore"));
                if (arvore.Length > 0)
                {
                    maisPerto = arvore[0];
                    for (int i = 1; i < arvore.Length; i++)
                    {
                        if (Vector3.Distance(maisPerto.transform.position, animator.gameObject.transform.position) < Vector3.Distance(arvore[i].transform.position, animator.gameObject.transform.position))
                        {
                            maisPerto = arvore[i];
                        }
                    }

                    animal.SetDestination(maisPerto.transform.position);
                    animal.transform.LookAt(maisPerto.transform);

                    if (Vector3.Distance(animal.transform.position, maisPerto.transform.position) < 2f)
                    {
                        animator.SetTrigger("Comer");
                        animator.SetBool("Caminhando", false);
                        animator.SetBool("Comendo", true);
                    }
                }
                aux = 20;
            }
            else if (escolha == 2)
            {
                bool lagao = Physics.CheckSphere(animator.gameObject.transform.position, 25f, LayerMask.GetMask("Lago"), QueryTriggerInteraction.Ignore);
                if (lagao)
                {
                    animator.SetBool("Caminhando", false);
                    animator.SetBool("Bebendo_Agua", true);
                    animator.SetTrigger("Beber_Agua");
                }
            }
            else if (escolha == 3)
            {
                destino = RandomNavMeshGenerator(animator, 10);
                animal.SetDestination(destino);
            }
            else
            {
                animator.SetBool("Caminhando", false);
                animator.SetBool("Comendo", false);
            }
        }
        if (aux >= -900)
            aux -= Time.deltaTime;
        else
            aux = 0;
    }
}
