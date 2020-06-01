using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : StateMachineBehaviour
{
    private double aux = 0;
    private NavMeshAgent animal;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animal = animator.gameObject.GetComponent<NavMeshAgent>();
        animal.SetDestination(animal.transform.position);
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (aux <= 0)
        {
            int random = Random.Range(1, 10);
            if (random <= 6)
            {
                animator.SetTrigger("Caminhar");
                animator.SetBool("Caminhando", true);
                animator.SetBool("Deitado", false);
            }
            if (random == 2)
            {
                random = Random.Range(1, 3);
                if (random > 6)
                {
                    animator.SetBool("Caminhando",false);
                    animator.SetTrigger("Deitar");
                }
            }
            aux = 10;
        }
        else
        {
            aux = aux - Time.deltaTime;
        }
    }
}
