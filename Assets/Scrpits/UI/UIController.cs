using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class UIController : MonoBehaviour
{
    public Image questAceitaTerminada;
    public GameObject questLogHUD;
    public GameObject posicao;

    [SerializeField]
    private Image playerLifeBar;
    [SerializeField]
    private Image XpBar;
    [SerializeField]
    private Image Skill;

    private GameObject tituloAtual;
    private GameObject textoAtual;

    [SerializeField]
    private Transform ContentGlosa;

    [SerializeField]
    private Scrollbar scrollGlosa;

    [SerializeField]
    private GameObject Usavel;

    [SerializeField]
    private GameObject RenderTyva;
    [SerializeField]
    private GameObject RenderIovelik;

    //public Transform Inicio;

    private GameObject[] Bases;

    [SerializeField]
    private GameObject Pause, painelMorte;

    [SerializeField]
    private GameObject painelUsáveis;
    [SerializeField]
    private GameObject painelQuestLog;


    bool questLogAberto = false;
    private List<Transform> listinha = new List<Transform>();


    #region SINGLETON

    public static UIController uiController;

    private void Awake()
    {
        uiController = this;
    }

    #endregion
    private void Start()
    {
        InicializarPainelUsasveis();
        AtualizarPainelUsaveis();
        AtualizaRender();

        /*float valor;
        audioMixer.GetFloat("volumeMaster", out valor);
        sliderJogo.value = valor;
        audioMixer.GetFloat("volumeEfeitos", out valor);
        sliderEfeitos.value = valor;
        audioMixer.GetFloat("volumeMusica", out valor);
        sliderMusica.value = valor;
        */

    }

    protected virtual void Update() {        
       PauseOn();

        /*if (Input.GetKey("J")) { //trocar para GetButtonDown("QuestLog") e setar o botão
            // Pausar jogo
            AbrirQuestLog();
            questLogAberto = true;            
        }

        if (Input.GetKey("J") && questLogAberto == true) { //trocar para GetButtonDown("QuestLog") e setar o botão
                FecharQuestLog();
                questLogAberto = false;
                // Retomar jogo
            }*/
    }

    #region WORLD CANVAS

    public void InitCBT(string text, GameObject CBTprefab, Transform hitCanvas)
    {
        GameObject temp = Instantiate(CBTprefab) as GameObject;

        RectTransform tempRect = temp.GetComponent<RectTransform>();
        temp.transform.SetParent(hitCanvas);
        tempRect.transform.localPosition = CBTprefab.transform.localPosition;
        tempRect.transform.localScale = CBTprefab.transform.localScale;
        tempRect.transform.localRotation = CBTprefab.transform.localRotation;
        temp.GetComponent<Animator>().SetTrigger("Hit");
        temp.GetComponent<Text>().text = text;

        Destroy(temp.gameObject, 2);
    }

    public void LifeBar(Image lifebar, float value)
    {
        lifebar.fillAmount = value;
    }
    #endregion

    public void SaveGame()
    {
        SaveSystem.Save();
    }

    public void LifeBar(float value)
    {
        playerLifeBar.fillAmount = value;
    }

    public void AtualizarTextoGlossario(GameObject titulo, GameObject texto)
    {
        if(tituloAtual != null)
        {
            tituloAtual.SetActive(false);
        }
        if(textoAtual != null)
        {
            textoAtual.SetActive(false);
            textoAtual.transform.SetParent(ContentGlosa.parent.parent.parent);
            
        }
        
        tituloAtual = titulo;
        textoAtual = texto;
        tituloAtual.SetActive(true);
        textoAtual.SetActive(true);
        textoAtual.transform.SetParent(ContentGlosa);
        scrollGlosa.value = 1;
    }
    public void XPbar(float value)
    {
        XpBar.fillAmount = value;
    }
    public void SkillCD(float value)
    {
        Skill.fillAmount = value;

    }


    public void InicializarPainelUsasveis(){

        for(int i = 0; i < painelUsáveis.transform.childCount; i++){
            listinha.Add(painelUsáveis.transform.GetChild(i));
        }
        for(int i = 0; i < Player.player.status.maxColetavel; i++){
            listinha[i].gameObject.SetActive(true);
        }
    }

    public void AtualizarPainelUsaveis(){

        int numFilho = 1;

        if(GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.TYVA)
        {
            numFilho = 2;
        }

        for(int i = 0; i < Player.player.status.QntColetavel; i++){
            listinha[i].GetChild(numFilho).gameObject.SetActive(true);
        }
        for(int i = Player.player.status.maxColetavel-1; i > Player.player.status.QntColetavel-1; i--){
            listinha[i].GetChild(numFilho).gameObject.SetActive(false);
        }
    }

    public void AtualizaRender()
    {
        if(GameController.gameController.GetPersonagemEscolhido() ==  TipoPlayer.TYVA)
        {
            RenderTyva.SetActive(true);
        }
        else if(GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.IOVELIK)
        {
            RenderIovelik.SetActive(true);
        }
    }


    public void AbrirQuestLog() {
        painelQuestLog.SetActive(true);
    }

    public void FecharQuestLog() {
        painelQuestLog.SetActive(false);
    }

    public void PauseOn(){

        if(Input.GetKey(KeyCode.Escape) && Player.player.estadoPlayer != EstadoPlayer.MORTO){

        Pause.SetActive(true);
        CameraController.cameraInstance.Trava = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;

        }

    }

    public void PauseOff(){

        Pause.SetActive(false);
        Cursor.visible = false;
        CameraController.cameraInstance.Trava = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    public void PainelMorteOn()
    {
        painelMorte.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MudarCena(string cena)
    {
        SceneManager.LoadScene(cena);
    }
}
