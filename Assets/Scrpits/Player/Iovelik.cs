using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Iovelik : Player
{
    [Header("Valores Iovelik")]
    public GameObject escudo;
    
    public float tempoRecargaEscudo = 0.5f;
    [SerializeField]
    private float valorDanoArea;
    [SerializeField]
    private float raioDanoArea;
    [SerializeField]
    private int coolDownDanoArea;
    private float esperaDanoArea = 0;
    private float distancia;

    private float recarregaEscudo;
    bool canAttack = true;
    public float RecarregaEscudo
    {
        get
        {
            return recarregaEscudo;
        }

        private set
        {
            recarregaEscudo = Mathf.Clamp(value, 0, status.tempoEscudo);

            if (value >= 0 && value <= status.tempoEscudo)
            {
                UIController.uiController.SkillCD((float)recarregaEscudo/status.tempoEscudo);//Controlador da barra de recarga da skill
            }

            if (value >= status.tempoEscudo)
                escudoCarregado = true;

            if (value <= 0)
            {
                
                escudoCarregado = false;
                AtivarEscudo(false);
            }
        }
    }

    private bool escudoCarregado = true;

    protected override void Start()
    {
        base.Start();

        status.QntColetavel = 3;
        RecarregaEscudo = status.tempoEscudo;
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(1) && esperaDanoArea <= 0 && status.QntColetavel > 0 && estadoPlayer == EstadoPlayer.COMBATE)
        {
            StartCoroutine(danoArea());
        }

        if(esperaDanoArea > 0)
        {
            esperaDanoArea -= Time.deltaTime;
        }
        RecarregaEscudo = (escudo.activeSelf) ? RecarregaEscudo-Time.deltaTime : RecarregaEscudo + tempoRecargaEscudo * Time.deltaTime;


        if (Input.GetButtonDown("Recarregavel") && escudoCarregado && recarregaEscudo > 0 && (estadoPlayer == EstadoPlayer.COMBATE || estadoPlayer == EstadoPlayer.ATACANDO))
        {
            animator.SetBool("Escudo", true);
            AtivarEscudo(true);
            return;
        }
        if(Input.GetButtonUp("Recarregavel"))
        {
            animator.SetBool("Escudo", false);
            AtivarEscudo(false);
            escudoCarregado = false;
        }
    }

    public override void ReceberDano(int danoRecebido)
    {
        if(!escudo.activeSelf)
            base.ReceberDano(danoRecebido);
    }

    private IEnumerator AtaqueEmArea()
    {
        yield return null;
    }

    private void AtivarEscudo(bool ativo)
    {
        escudo.SetActive(ativo);
        estadoPlayer = ativo ? EstadoPlayer.RECARREGAVEL : EstadoPlayer.COMBATE;
    }

    private IEnumerator danoArea()
    {
        estadoPlayer = EstadoPlayer.ATACANDO;
        animator.SetTrigger("Especial");
        canAttack = false;

        yield return new WaitForSeconds(1f);

        Debug.Log("Toma porra");

        Collider[] hit = Physics.OverlapSphere(posicaoHit.position, raioDanoArea, LayerMask.GetMask("Inimigo"));

        foreach (Collider inimigoArea in hit)
        {
            distancia = Vector3.Distance(inimigoArea.gameObject.transform.position, this.gameObject.transform.position);
            inimigoArea.gameObject.GetComponent<Inimigo>().ReceberDano((int)(valorDanoArea / distancia));
            esperaDanoArea = coolDownDanoArea;
        }

        status.QntColetavel--;
        yield return WaitForAnimation("Especial");

        estadoPlayer = EstadoPlayer.COMBATE;

    }
}
