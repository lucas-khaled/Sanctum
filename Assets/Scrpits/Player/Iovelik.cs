using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Iovelik : Player
{
    [Header("Audio Iovelik")]
    [SerializeField]
    private AudioClip martelada;
    [SerializeField]
    private AudioClip escudoSom;

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

    public bool habilidadeCura { get; set; }
    public bool refleteDano { get; set; }

    private float recarregaEscudo;
    bool canAttack = true;

    public string ataqueEspecialNome { get; set; }

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

        RecarregaEscudo = status.tempoEscudo;
        ataqueEspecialNome = "Especial";
        refleteDano = false;
        habilidadeCura = false;
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(1) && esperaDanoArea <= 0 && status.QntColetavel > 0 && estadoPlayer == EstadoPlayer.COMBATE)
        {
            danoArea();
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

    public override void ReceberDano(int danoRecebido, Inimigo inim = null)
    {
        if (!escudo.activeSelf)
        {
            if (status.QntColetavel > 0 && status.Vida - danoRecebido <= 0 && habilidadeCura)
            {
                Curar(Mathf.CeilToInt(status.maxVida * 2 / 10));
                status.QntColetavel--;
            }
            else
            {
                base.ReceberDano(danoRecebido, inim);
            }
        }
        else if (refleteDano && inim != null)
        {
            inim.ReceberDano(danoRecebido / 2);
        }
    }


    private void AtivarEscudo(bool ativo)
    {
        audioSource.PlayOneShot(escudoSom);
        escudo.SetActive(ativo);
        estadoPlayer = ativo ? EstadoPlayer.RECARREGAVEL : EstadoPlayer.COMBATE;
    }

    private void danoArea()
    {
        audioSource.PlayOneShot(martelada);
        estadoPlayer = EstadoPlayer.ATACANDO;
        animator.SetTrigger(ataqueEspecialNome);
        canAttack = false;
        status.QntColetavel--;
        estadoPlayer = EstadoPlayer.COMBATE;

    }

    public void AreaAttack()
    {
        Collider[] hit = Physics.OverlapSphere(posicaoHit.position, raioDanoArea, LayerMask.GetMask("Inimigo"));

        foreach (Collider inimigoArea in hit)
        {
            distancia = Vector3.Distance(inimigoArea.gameObject.transform.position, this.gameObject.transform.position);
            inimigoArea.gameObject.GetComponent<Inimigo>().ReceberDano((int)(valorDanoArea / distancia));
            esperaDanoArea = coolDownDanoArea;
        }
    }

    public void CheckCombo()
    {
        podeAtacar = false; 

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") && numClick == 1)
        {
            animator.SetInteger("Ataque", 0);
            numClick = 0;
            estadoPlayer = EstadoPlayer.COMBATE;
        }

        else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") && numClick >= 2)
        {
            animator.SetInteger("Ataque", 2);
            podeAtacar = true;
        }

        else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") && numClick == 2)
        {
            animator.SetInteger("Ataque", 0);
            numClick = 0;
            estadoPlayer = EstadoPlayer.COMBATE;
        }
        
        else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") && numClick >= 3)
        {
            animator.SetInteger("Ataque", 3);
            podeAtacar = true;
        }

        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
        {
            animator.SetInteger("Ataque", 0);
            numClick = 0;
            estadoPlayer = EstadoPlayer.COMBATE;
        }
    }
}
