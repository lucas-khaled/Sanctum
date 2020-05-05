using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EstadoPlayer { NORMAL, COMBATE, ATACANDO, DANO, RECARREGAVEL, MORTO }

[RequireComponent(typeof(Rigidbody))] [RequireComponent(typeof(Collider))] [RequireComponent(typeof(StatusPlayer))] [RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour, IVulnerable
{
    protected Animator animator;
    private EstadoPlayer estado_player;

    [HideInInspector]
    public StatusPlayer status;

    [Header("Referências")]
    public Transform posicaoHit;
    public GameObject CBTprefab;

    [Header("Valores")]
    [SerializeField]
    private float velocidade = 2;
    [SerializeField]
    private float raioPercepcao;
    [SerializeField]
    private float raioAtaque = 2f;
    public int numAtaque = 2;

    protected Rigidbody rb;

    bool outroAtaque;
    private float attack = 0;
    private Transform hitCanvas;

    #region GETTERS & SETTERS

    public EstadoPlayer estadoPlayer
    {
        get { return estado_player; }

        protected set
        {
            estado_player = value;
            if (value == EstadoPlayer.NORMAL)
            {
                InvokeRepeating("ProcuraInimigo", 0f, 0.2f); 
            }
            EventsController.onPlayerStateChanged.Invoke(estado_player);
        }
    }
    #endregion

    #region PreSettings
    public static Player player;
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody>();
        status = GetComponent<StatusPlayer>();

        EventsController.onMorteInimigoCallback += OnMorteInimigo;

        player = this;     
    }

    protected virtual void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        estadoPlayer = EstadoPlayer.NORMAL;
        hitCanvas = transform.Find("Hit_life");
    }
    #endregion

    #region COMBATE
    private void MoverPlayerAtaque()
    {
        //transform.LookAt((transform.forward * velocidade * Input.GetAxis("Vertical")) + (transform.right * velocidade * Input.GetAxis("Horizontal")) + player.transform.position);
        rb.velocity = (transform.forward * 5);
    }

    private bool CheckCombo()
    {
        if(attack == 1)
        {
            attack = 0;
            return false;
        }
        else
        {
            attack += Mathf.Clamp(1f / numAtaque-1, 0, 1);
            return true;
        }
    }

    protected IEnumerator WaitForAnimation(string animacao)
    {

        while(animator.GetCurrentAnimatorStateInfo(0).IsName(animacao))
        {
            yield return null;
        }

    }

    int QualAtaque()
    {
        int x = 0;
        int retorno = 0;
        for(int i = 1; i<=numAtaque; i++)
        {
            retorno = i;
            if(attack == x)
            {
                break;
            }
            x += 1 / numAtaque-1;
        }
        return retorno;
    }

    private IEnumerator Atacar()
    {

        if (CheckCombo() && outroAtaque)
        {
            estadoPlayer = EstadoPlayer.ATACANDO;
            animator.SetBool("Atacando", true);

            outroAtaque = false;
           

            /*Collider[] hit = Physics.OverlapSphere(posicaoHit.position, raioAtaque, LayerMask.GetMask("Inimigo"));

            if (hit.Length > 0)
            {
                int danin = CalculaDano();
                hit[0].gameObject.GetComponent<Inimigo>().ReceberDano(danin);
            }*/

            yield return WaitForAnimation("Attack"+ QualAtaque());

            
            if (!outroAtaque)
            {
                //MoverPlayerAtaque();
                animator.SetBool("Atacando", false);
                estadoPlayer = EstadoPlayer.COMBATE;
                attack = 0;
            }
        }

    }

    public int CalculaDano()
    {
        return status.DanoMedio + Random.Range(-5, 5);
    } 
    // Passei essa parte do calcula dano para o script de ArmaPlayer, não faz sentido o player calcular o dano que
    // vai ser passado no inimigo pelo script ArmaPlayer.
    // mantive comentado só pra você saber o que aconteceu.

    private void Morrer()
    {
        estadoPlayer = EstadoPlayer.MORTO;
    }

    public virtual void ReceberDano(int danoRecebido)
    {
        status.Vida -= danoRecebido;
        UIController.uiController.InitCBT(danoRecebido.ToString(), CBTprefab, hitCanvas);
        string nomeAnim = "Dano";

        if(danoRecebido > status.maxVida*30/100)
        {
            nomeAnim += "Forte";
        }


        if (status.Vida <= 0)
        {
            nomeAnim = "Morte";
        }

        animator.applyRootMotion = true;
        StartCoroutine(Dano(nomeAnim));
        
    }

    private IEnumerator Dano(string animationName)
    {
        estadoPlayer = EstadoPlayer.DANO;

        animator.SetTrigger(animationName);
        yield return WaitForAnimation(animationName);

        if (animationName == "Morte")
        {
            Morrer();
        }
        else
        {
            estadoPlayer = EstadoPlayer.COMBATE;
        }
        
    }

    protected Vector3 ProcuraInimigo()
    {
        Collider[] hit = Physics.OverlapSphere(posicaoHit.position, raioPercepcao, LayerMask.GetMask("Inimigo"));

        if (hit.Length > 0)
        {
            foreach (Collider inimigo in hit)
            {
                    if (inimigo.GetComponent<Inimigo>().hostil && inimigo.gameObject.activeSelf)
                    {
                        estadoPlayer = EstadoPlayer.COMBATE;
                        CancelInvoke("ProcuraInimigo");
                        return inimigo.transform.position;
                    }
            }         
        }

        return Vector3.zero;

    }

    private IEnumerator ProcuraInimigoMorte()
    {
        yield return new WaitForSeconds(1);
        if (!Physics.CheckSphere(posicaoHit.position, raioPercepcao, LayerMask.GetMask("Inimigo")))
        {
            estadoPlayer = EstadoPlayer.NORMAL;
        }

    }
    #endregion

    /*void VerificarColetaveis()
    {
        
    }*/

    protected virtual void Update()
    {
        Movimento();
            

        if (Input.GetButtonDown("Attack") && estadoPlayer == EstadoPlayer.COMBATE)
        {
            outroAtaque = true;
            StartCoroutine(Atacar());
        }

        if (Input.GetButtonDown("Interact"))
        {
            Interagir();
        }
    }


    void Movimento()
    {
        if (estadoPlayer == EstadoPlayer.NORMAL || estadoPlayer == EstadoPlayer.COMBATE)
        {
            animator.applyRootMotion = false;
            float y = Mathf.Lerp(animator.GetFloat("VetY"), Input.GetAxis("Vertical"), 0.4f);
            float x = Mathf.Lerp(animator.GetFloat("VetX"), Input.GetAxis("Horizontal"), 0.4f);

            animator.SetFloat("VetX", x);
            animator.SetFloat("VetY", y);

            rb.velocity = ((transform.forward * y) + (transform.right * x)) * velocidade;
        }
    }


    public void Curar(int cura)
    {

    }

    private void Interagir()
    {
        if (InteracaoController.instance.interagivelAtual != null)
            InteracaoController.instance.interagivelAtual.Interact();     
    }

    void OnMorteInimigo(int xp)
    {
        status.Experiencia += xp;
        StartCoroutine(ProcuraInimigoMorte());
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, raioPercepcao);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, 2);
    }

}