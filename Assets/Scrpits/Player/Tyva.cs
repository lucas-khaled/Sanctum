using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tyva : Player
{
    float moveHorizontal;
    float moveVertical;

    [Header("Valores Tyva")]
    [SerializeField]
    private float velocidadeDash;
    [SerializeField]
    private float timeFaca;
    [SerializeField]
    private float timeDash;
    [SerializeField]
    private float tempoRecarga = 1;

    [Header("Audio Tyva")]
    [SerializeField]
    private AudioClip dash;
    [SerializeField]
    private AudioClip lancarFaca;

    private bool dashEspecial = false;

    public bool facaLetal { get; set; }

    public bool DashEspecial
    {
        get
        {
            return dashEspecial;
        }
        set
        {
            dashEspecial = value;
            animator.SetBool("DashEspecial", value);
        }
    }

    public float TempoRecarga
    {
        get
        {
            return tempoRecarga;
        }
        set
        {
            tempoRecarga = value;
        }
    }

    private float tempoDash;
    public float TempoDash
    {
        get
        {
            return tempoDash;
        }

        private set
        {
            tempoDash = Mathf.Clamp(value, 0, status.tempoDashTotal);

            if (value >= 0 && value <= status.tempoDashTotal)
            {
                UIController.uiController.SkillCD((float)tempoDash/status.tempoDashTotal);//Controlador UI da recarga
            }
        }
    }

    [Header("Referências Tyva")]
    public Transform posicaoFaca;
    private Transform pointerPosition;
    public GameObject faca;
    private float contadorFaca;

    #region PreSettings

    protected override void Start()
    {
        base.Start();
        //só para teste, deletar depois
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");
        facaLetal = false;
    }

    protected override void Awake()
    {
        base.Awake();
    }
    #endregion

    protected override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(1))
        {
            Faca();
        }       

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (TempoDash >= status.tempoDashTotal)
            {
                Vector3 movimento = new Vector3(moveHorizontal, 0.0f, moveVertical);
                StartCoroutine(Dash());
                TempoDash = 0;
            }
            else
            {            
                rb.velocity = Vector3.zero;
            }
        }

        TempoDash += Time.deltaTime*tempoRecarga;
        contadorFaca += Time.deltaTime;
    }

    IEnumerator Dash()
    {
        animator.SetTrigger("Dash");

        audioSource.PlayOneShot(dash);

        if (dashEspecial)
            estadoPlayer = EstadoPlayer.ATACANDO;
        else
            estadoPlayer = EstadoPlayer.RECARREGAVEL;

        float tempo = 0;

        while (tempo < timeDash)
        {
            rb.AddForce(transform.forward * velocidadeDash * Time.deltaTime, ForceMode.VelocityChange);
            tempo += Time.deltaTime;
            yield return null;
        }

        estadoPlayer = EstadoPlayer.COMBATE;
    }

    private void Faca()
    {
        if (contadorFaca < timeFaca || status.QntColetavel <= 0)
            return;

        Vector3 target = ProcuraInimigo();
        if (target == Vector3.zero)
            target = FacaFrontTarget();

        StartCoroutine(LancarFaca(target));
    }

    private IEnumerator LancarFaca(Vector3 target)
    {
        contadorFaca = 0;
        animator.SetTrigger("JogarAdaga");
        audioSource.PlayOneShot(lancarFaca);
        yield return new WaitForSeconds(0.5f);

        GameObject facaInstanciada = Instantiate(faca, posicaoFaca.position, faca.transform.rotation);
        target.y += 1;
        facaInstanciada.transform.LookAt(target);
        facaInstanciada.GetComponent<Bala>().SetCasterCollider(GetComponent<Collider>());

        if (facaLetal)
            facaInstanciada.GetComponent<Bala>().canBeLetal = true;

        status.QntColetavel--;
    }

    Vector3 FacaFrontTarget()
    {
        float rotY = transform.localEulerAngles.y * Mathf.Deg2Rad;

        Vector3 front = new Vector3(posicaoFaca.position.x + Mathf.Sin(rotY),
                                       posicaoFaca.position.y,
                                       posicaoFaca.position.z + Mathf.Cos(rotY));

        return front;
    }

    public void CheckCombo()
    {
        podeAtacar = false;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") && numClick == 1)
        {
            animator.SetInteger("Ataque", 0);
            numClick = 0;
            estadoPlayer = EstadoPlayer.COMBATE;
        }

        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") && numClick >= 2)
        {
            animator.SetInteger("Ataque", 2);
            podeAtacar = true;
        }

        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") && numClick == 2)
        {
            animator.SetInteger("Ataque", 0);
            numClick = 0;
            estadoPlayer = EstadoPlayer.COMBATE;
        }

        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") && numClick >= 3)
        {
            animator.SetInteger("Ataque", 3);
            podeAtacar = true;
        }

        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3") && numClick == 3)
        {
            animator.SetInteger("Ataque", 0);
            numClick = 0;
            estadoPlayer = EstadoPlayer.COMBATE;
        }

        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3") && numClick >= 4)
        {
            animator.SetInteger("Ataque", 4);
            podeAtacar = true;
        }

        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack4"))
        {
            animator.SetInteger("Ataque", 0);
            numClick = 0;
            estadoPlayer = EstadoPlayer.COMBATE;
        }
    }
}