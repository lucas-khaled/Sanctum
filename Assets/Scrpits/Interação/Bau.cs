using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InventarioData;

public class Bau : Interagivel
{
    public List<Item> itens = new List<Item>();
    [SerializeField]
    Animator anim;

    [Header("Audios")]
    [SerializeField]
    private AudioClip abrir;
    [SerializeField]
    private AudioSource audioSource;

    public bool someSeVazia;

    protected override void Start()
    {
        base.Start();

        if (GameController.gameController.IsLoadedGame())
            LoadBau();
    }

    void LoadBau()
    {
        BauSave mySave = new BauSave(null, null);
        bool isThere = false;

        foreach(BauSave bs in SaveSystem.data.inventarioData.baus)
        {
            if(bs.nomeBau == this.name)
            {
                isThere = true;
                mySave = bs;
                break;
            }
        }

        if (isThere)
        {
            itens.Clear();
            itens.AddRange(mySave.itens);
        }
    }

    public override void Interact()
    {
        base.Interact();

        audioSource.PlayOneShot(abrir);

        if (anim != null)
        {
            anim.SetBool("Aberto", true);
        }

        BauUI.bauUI.SetBau(this);

        if(someSeVazia)
        {
            StartCoroutine(Some());
        }

    }

    private IEnumerator Some()
    {
        yield return new WaitForSeconds(0.01f);
        if (itens.Count == 0)
        {
            Destroy(this.gameObject);
        }
    }
}
