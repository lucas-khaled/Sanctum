using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiverAnimationController : MonoBehaviour
{
    [SerializeField]
    Animator anim;

    public void Interagir()
    {
        if (Random.Range(0, 2) == 1)
        {
            anim.SetTrigger("Interacao");
        }
        else
        {
            anim.SetTrigger("Interacao2");
        }
    }

    private void Start()
    {
        if (contemParametro("Sentar"))
            anim.SetTrigger("Sentar");
    }

    public void AceitarQuest()
    {
        anim.SetTrigger("QuestAceita");
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            anim.SetBool("PlayerPerto", false);
            anim.SetTrigger("Sentar");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            anim.SetBool("PlayerPerto", true);
        }
    }

    // Procura um parâmetro no animator.
    private bool contemParametro(string nomeParametro)
    {
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == nomeParametro)
                return true;
        }
        return false;
    }
}
