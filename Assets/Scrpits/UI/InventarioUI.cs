using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventarioUI : MonoBehaviour
{
    [SerializeField]
    private GameObject Slot;
    [SerializeField]
    private GameObject Grid;
    [SerializeField]
    private GameObject ExcluirB;
    [SerializeField]
    private GameObject EquiparB;
    [SerializeField]
    private GameObject UsarB;
    [SerializeField]
    private Text Dinheiro_Atual;
    [SerializeField]
    private Text Peso_Atual;
    [SerializeField]
    private Text Info;
    [SerializeField]
    private Text Valor_venda;
    [SerializeField]
    private Text Titulo;
    [SerializeField]
    private Text peso;
    [SerializeField]
    private Image ArmaEq;


    [HideInInspector]
    public Item selecionado;

    List<GameObject> Bag = new List<GameObject>();


    public static InventarioUI inventarioUI;
    void Awake()
    {
        EventsController.onInventarioChange += AttUI;
        inventarioUI = this;
    }
    void Start()
    {
        PegaValores();
        ArmaEq.sprite = Inventario.inventario.armaEquipada.icone;  
    }

    public void AttUI(Item item, bool mudanca)
    {
        ArmaEq.sprite = Inventario.inventario.armaEquipada.icone;
        PegaValores();
        if (mudanca)
        {

            GameObject Obj = Slot;
            Obj.GetComponent<Holder_Item>().item = item;
            Obj.GetComponent<Holder_Item>().isLoja = false;
            GameObject mundano = (GameObject)Instantiate(Obj);
            mundano.transform.SetParent(Grid.transform);
            mundano.transform.localScale = Vector3.one;
            Bag.Add(mundano);

        }
        else
        {
            GameObject itemRemovido;
            itemRemovido = Bag.Find(x => x.GetComponent<Holder_Item>().item == item);
            Bag.Remove(itemRemovido);
            Destroy(itemRemovido);
        }

    }

    public void Excluir()
    {
        Inventario.inventario.RemoverItem(selecionado);
        ExcluirB.SetActive(false);
        EquiparB.SetActive(false);

    }

    public void Equipar()
    {
        Arma selection = (Arma)selecionado;

        if (selection.armaPlayer == GameController.gameController.GetPersonagemEscolhido())
        {
            Inventario.inventario.EquipArma((Arma)selecionado);
            ExcluirB.SetActive(false);
            EquiparB.SetActive(false);
        }
    }

    public void Usar()
    {
        Cura itemCura = (Cura)selecionado;
        Player.player.Curar(itemCura.quantidadeCura);

        Inventario.inventario.RemoverItem(selecionado);
        selecionado = null;
        ClearBotoes();
    }

    public void PegaValores()
    {
        Dinheiro_Atual.text = Player.player.status.Dinheiro.ToString();
        Peso_Atual.text = Inventario.inventario.pesoInventario.ToString();
    }

    IEnumerator ApareceExcluirWait()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        ExcluirB.SetActive(true);
    }

    public void ApareceExcluir()
    {
        StartCoroutine(ApareceExcluirWait());
    }

    IEnumerator ApareceEquiparWait()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        EquiparB.SetActive(true);
    }

    public void ApareceEquipar()
    {
        StartCoroutine(ApareceEquiparWait());
    }

    public void ApareceUsar()
    {
        StartCoroutine(ApareceUsarWait());
    }

    IEnumerator ApareceUsarWait()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        UsarB.SetActive(true);
    }

    public void ClearOpcoes()
    {
        StartCoroutine(ClearBotoes());
        Info.text = string.Empty;
        Titulo.text = string.Empty;
        peso.text = string.Empty;
        Valor_venda.text = string.Empty;
    }

    IEnumerator ClearBotoes()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        ExcluirB.SetActive(false);
        EquiparB.SetActive(false);
        UsarB.SetActive(false);
    }

    public void ShowInfo(Item item)
    {
        if(item == null)
        {
            Debug.Log("é o item");
        }

        if(Info.text == null)
        {
            Debug.Log("cu");
        }

        Info.text = (item.descricao != null) ? item.descricao: string.Empty;
        Titulo.text = item.nome;
        peso.text = item.peso.ToString();
        Valor_venda.text = item.custoMoeda.ToString();
        selecionado = item;
    }
}
