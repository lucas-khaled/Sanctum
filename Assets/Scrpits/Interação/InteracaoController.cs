using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteracaoController : MonoBehaviour
{
    [SerializeField]
    private float raioInteracao = 3f;

    private Interagivel interagivelAtual = null;

    #region SINGLETON
    public static InteracaoController instance;

    private void Awake()
    {
        instance = this;
        EventsController.onPlayerStateChanged += onPlayerStateChanged;
    }
    #endregion

    public void Interact()
    {
        if(interagivelAtual != null && (Player.player.estadoPlayer == EstadoPlayer.COMBATE || Player.player.estadoPlayer == EstadoPlayer.NORMAL))
        {
            interagivelAtual.Interact();
        }
    }

    private void onPlayerStateChanged(EstadoPlayer estadoPlayer)
    {
        if (estadoPlayer == EstadoPlayer.NORMAL || estadoPlayer == EstadoPlayer.COMBATE)
            InvokeRepeating("InteragivelMaisProximo", 1f, 0.1f);
        else
        {
            CancelInvoke("InteragivelMaisProximo");
            if (interagivelAtual != null)
            {
                interagivelAtual.SwitchImagemInteracao(false);
                interagivelAtual = null;
            }
        }
    }

    private void Start()
    {
        if (Player.player.estadoPlayer == EstadoPlayer.NORMAL || Player.player.estadoPlayer == EstadoPlayer.COMBATE)
            InvokeRepeating("InteragivelMaisProximo", 0, 0.1f);
    }

    private void InteragivelMaisProximo()
    {
        Collider[] hit = Physics.OverlapSphere(Player.player.transform.position, raioInteracao, LayerMask.GetMask("Interagivel"));

        if (hit.Length > 0)
        {
            int numIndex = 0;

            foreach(Collider colisor in hit)
            {
                if (!colisor.isTrigger)
                {
                    break;
                }
                else
                {
                    numIndex++;
                }
            }

            if (hit.Length > numIndex)
            {
                Interagivel atual = hit[numIndex].GetComponent<Interagivel>();
                if (interagivelAtual == atual)
                    return;

                if (interagivelAtual != null)
                    interagivelAtual.SwitchImagemInteracao(false);

                interagivelAtual = atual;
                interagivelAtual.SwitchImagemInteracao(true);
            }

            else
            {
                if (interagivelAtual != null)
                {
                    interagivelAtual.SwitchImagemInteracao(false);
                    interagivelAtual = null;
                }
            }
        }

        else
        {
            if (interagivelAtual != null)
            {
                interagivelAtual.SwitchImagemInteracao(false);
                interagivelAtual = null;
            }
        }
    }
}
