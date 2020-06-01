using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loja : Interagivel
{
    public List<Item> itensAVenda { get; private set; }

    public int maxNumItens, minNumItens;

    [SerializeField]
    private Item[] possiveisItensAVenda;

    public override void Interact()
    {
        base.Interact();

        if (!isPartOfDialogue)
        {
            StartCoroutine(LojaUI.lojaUi.AbrirLoja(this));
        }
    }

    protected override void Start()
    {
        base.Start();
        itensAVenda = new List<Item>();
        RenovaItens();
        StartCoroutine(RenovacaoLoja());
    }

    void RenovaItens()
    {
        int numItens = Random.Range(minNumItens, maxNumItens + 1);
        int indexMin = Random.Range(0, possiveisItensAVenda.Length);

        itensAVenda.Clear();
        for(int i = 0; i<numItens; i++)
        {
            itensAVenda.Add(possiveisItensAVenda[(indexMin + i) % possiveisItensAVenda.Length]);
        }
    }

    IEnumerator RenovacaoLoja()
    {
        yield return new WaitForSecondsRealtime(3600);
        RenovaItens();
        StartCoroutine(RenovacaoLoja());
    }

}
