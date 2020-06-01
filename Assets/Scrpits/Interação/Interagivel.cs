using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class Interagivel : MonoBehaviour
{
    public GameObject worldCanvas;

    protected bool isPartOfDialogue;

    private List<Dialogo> dialogoCondicao = new List<Dialogo>();

    public void SetDialogoCondicao(Dialogo dialogo)
    {
        isPartOfDialogue = true;
        dialogoCondicao.Add(dialogo);
        EventsController.onDialogoTerminado += OnDialogoTerminado;
    }

    protected virtual void Start()
    {
        isPartOfDialogue = false;
        worldCanvas.SetActive(false);
    }

    public void SwitchImagemInteracao(bool isActual)
    {
        if (GameController.gameController.QualOrigemInput() == OrigemInput.MOUSE)
        {
            worldCanvas.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = GameController.gameController.interagivelTeclado;
        }
        else
        {
            worldCanvas.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = GameController.gameController.interagivelJoystick;
        }
        worldCanvas.SetActive(isActual);
    }

    public virtual void Interact()
    {
        if (EventsController.onInteracao != null)
        {
            EventsController.onInteracao.Invoke(this);
        }

        if (isPartOfDialogue)
        {
            DialogueSystem.sistemaDialogo.IniciaDialogo(dialogoCondicao[0]);
        }
    }

    void OnDialogoTerminado(Dialogo dialogo)
    {
        if(!isPartOfDialogue)
        {
            return;
        }

        if(dialogo.whosDialog == dialogoCondicao[0].whosDialog)
        {
            dialogoCondicao.RemoveAt(0);

            if(dialogoCondicao.Count == 0)
            {
                isPartOfDialogue = false;
            }
        }
    }

}
