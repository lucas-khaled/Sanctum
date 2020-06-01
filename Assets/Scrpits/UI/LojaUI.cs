using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LojaUI : MonoBehaviour
{
    public Transform contentVendedor, contentPlayer;
    public GameObject slotPrefab, painelLoja;

    [Header("InfoPlayer")]
    [SerializeField]
    private Text playerMoedas;
    [SerializeField]
    private Text playerFama;
    [SerializeField]
    private Text inventarioPeso;

    [Header("InfoItem")]
    [SerializeField]
    private Text itemMoedas;
    [SerializeField]
    private Text itemFama;
    [SerializeField]
    private Text itemPeso;
    [SerializeField]
    private Text itemNome;
    [SerializeField]
    private Text itemDescricao;
    [SerializeField]
    private GameObject painelInfoItem;
    [SerializeField]
    private Image spriteItem;

    [Header("Botões")]
    [SerializeField]
    private Button btnVender;
    [SerializeField]
    private Button btnComprar;

    [HideInInspector]
    public int contCompraVenda;

    List<GameObject> listaVendedor = new List<GameObject>(), listaPlayer = new List<GameObject>();
    Holder_Item holderAtual;
    Loja lojaAtual;

    public static LojaUI lojaUi;
    private void Awake()
    {
        lojaUi = this;
    }

    public void FecharLoja()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        CameraController.cameraInstance.Trava = false;

        painelLoja.SetActive(false);

        if (lojaAtual.gameObject.GetComponent<Kambim>())
        {
            lojaAtual.gameObject.GetComponent<Kambim>().FimInteracao();
        }
        else if (lojaAtual.gameObject.GetComponent<Atriet>())
        {
            lojaAtual.gameObject.GetComponent<Atriet>().FimInteracao();
        }

        ClearLoja();
        lojaAtual = null;
    }

    public IEnumerator AbrirLoja(Loja loja)
    {
        if (lojaAtual == null)
        {
            lojaAtual = loja;

            if (lojaAtual.gameObject.GetComponent<Kambim>())
            {
                lojaAtual.gameObject.GetComponent<Kambim>().Conversa();
            }
            else if (lojaAtual.gameObject.GetComponent<Atriet>())
            {
                lojaAtual.gameObject.GetComponent<Atriet>().Conversa();
            }

            contCompraVenda = 0;

            yield return new WaitForSeconds(0.4f);

            while (true)
            {
                Debug.Log("Ainda n cusao");
                if ((Input.GetButton("Interact")) && !DialogueSystem.sistemaDialogo.IsDialogEnded())
                {
                    Debug.Log("Ihaaaa");
                    break;
                }

                yield return 0;
            }

            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            CameraController.cameraInstance.Trava = true;

            painelLoja.SetActive(true);

            ClearLoja();

            CarregaInfoPlayer();

            CarregaItensPlayer();
            CarregaItensVendedor(loja.itensAVenda);
        }
    }

    void CarregaInfoPlayer()
    {
        playerMoedas.text = Player.player.status.Dinheiro.ToString();
        playerFama.text = Player.player.status.Fama.ToString();
        inventarioPeso.text = Inventario.inventario.pesoInventario + "/" + Inventario.inventario.pesoMaximo;
    }

    void ClearLoja()
    {
        if (listaVendedor.Count > 0)
        {
            foreach (GameObject go in listaVendedor)
            {
                Destroy(go);
            }
            listaVendedor.Clear();
        }

        if (listaPlayer.Count > 0)
        {
            foreach (GameObject go in listaPlayer)
            {
                Destroy(go);
            }

            listaPlayer.Clear();
        }
    }

    private void CarregaItensPlayer()
    {
        foreach(Item item in Inventario.inventario.Getitens())
        {
            GameObject instancia = Instantiate(slotPrefab);
            instancia.GetComponent<Holder_Item>().isLoja = true;
            instancia.GetComponent<Holder_Item>().item = item;

            instancia.transform.SetParent(contentPlayer);
            instancia.transform.localScale = Vector3.one;

            listaPlayer.Add(instancia);
        }
    }

    private void CarregaItensVendedor(List<Item> itens)
    {
        foreach (Item item in itens)
        {
            GameObject instancia = Instantiate(slotPrefab);
            instancia.GetComponent<Holder_Item>().isLoja = true;
            instancia.GetComponent<Holder_Item>().item = item;

            instancia.transform.SetParent(contentVendedor);
            instancia.transform.localScale = Vector3.one;

            listaVendedor.Add(instancia);
        }
    }

    public void SetItemSelecionado(Holder_Item holder)
    {
        itemNome.text = holder.item.nome;
        itemDescricao.text = holder.item.descricao;
        itemMoedas.text = holder.item.custoMoeda.ToString();
        itemPeso.text = holder.item.peso.ToString();
        spriteItem.sprite = holder.item.icone;

        if(holder.item.GetType() == typeof(Arma))
        {
            Arma arminha = (Arma)holder.item; 
            itemFama.text = arminha.famaMinima.ToString();
        }

        StartCoroutine(WaitForSetButtonSelection(holder));
    }

    IEnumerator WaitForSetButtonSelection(Holder_Item holder)
    {
        yield return new WaitForSecondsRealtime(0.1f);
        painelInfoItem.SetActive(true);
        if (holder.transform.parent == contentPlayer.transform)
        {
            btnComprar.interactable = false;
            btnComprar.enabled = false;
            btnComprar.gameObject.GetComponent<Image>().color = btnComprar.colors.disabledColor;

            btnVender.interactable = true;
            btnVender.enabled = true;
            btnVender.gameObject.GetComponent<Image>().color = btnVender.colors.normalColor;
        }

        else
        {
            btnComprar.interactable = true;
            btnComprar.enabled = true;
            btnComprar.gameObject.GetComponent<Image>().color = btnComprar.colors.normalColor;

            btnVender.interactable = false;
            btnVender.enabled = false;
            btnVender.gameObject.GetComponent<Image>().color = btnVender.colors.disabledColor;
        }

        holderAtual = holder;
    }

    public void DeselectItem()
    {
        StartCoroutine(DeselectItemWait());
    }

    IEnumerator DeselectItemWait()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        painelInfoItem.SetActive(false);

        btnComprar.interactable = false;
        btnComprar.enabled = false;
        btnComprar.gameObject.GetComponent<Image>().color = btnComprar.colors.disabledColor;

        btnVender.interactable = false;
        btnVender.enabled = false;
        btnVender.gameObject.GetComponent<Image>().color = btnVender.colors.disabledColor;

        holderAtual = null;
    }

    public void VenderItem()
    {
        contCompraVenda--;
        Debug.Log(contCompraVenda);
        Item vendido = holderAtual.item;
        lojaAtual.itensAVenda.Add(vendido);
        Inventario.inventario.RemoverItem(vendido);

        GameObject go = listaPlayer.Find(x => x.GetComponent<Holder_Item>() == holderAtual);
        listaPlayer.Remove(go);

        listaVendedor.Add(go);
        go.transform.SetParent(contentVendedor);
        go.transform.localScale = Vector3.one;

        Player.player.status.Dinheiro += vendido.custoMoeda;
        CarregaInfoPlayer();

        
    }

    public void ComprarItem()
    {
        if(Player.player.status.Dinheiro < holderAtual.item.custoMoeda)
        {
            return;
        }
        contCompraVenda++;

        Item vendido = holderAtual.item;
        lojaAtual.itensAVenda.Remove(vendido);
        Inventario.inventario.AddItem(vendido);

        GameObject go = listaVendedor.Find(x => x.GetComponent<Holder_Item>() == holderAtual);
        listaVendedor.Remove(go);

        listaPlayer.Add(go);
        go.transform.SetParent(contentPlayer);
        go.transform.localScale = Vector3.one;

        Player.player.status.Dinheiro -= vendido.custoMoeda;
        CarregaInfoPlayer();
    }

}
