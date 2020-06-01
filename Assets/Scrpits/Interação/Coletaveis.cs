using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coletaveis : Interagivel
{
    [SerializeField]
    private GameObject Cristal;
    [SerializeField]
    private GameObject Faca;

    TipoPlayer qualPlayer;
    GameObject objetoAtivo;

    protected override void Start()
    {
        base.Start();
        qualPlayer = GameController.gameController.GetPersonagemEscolhido();

        if(qualPlayer == TipoPlayer.IOVELIK)
        {
            Cristal.SetActive(true);
            Faca.SetActive(false);
            objetoAtivo = Cristal;
        }
        else
        {
            Faca.SetActive(true);
            Cristal.SetActive(false);
            objetoAtivo = Faca;
        }
    }

    public override void Interact()
    {
        base.Interact();
        if(Player.player.status.QntColetavel < Player.player.status.MaxColetavel)
        {
            Player.player.status.QntColetavel++;
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if(objetoAtivo!=null)
            objetoAtivo.transform.Rotate(Vector3.up*50*Time.deltaTime, Space.Self);
    }
}
