using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArvoreDeHabilidades : MonoBehaviour
{
    [SerializeField]
    private Skill[] skillsIovelik;
    [SerializeField]
    private Skill[] skillsTyva;
    [SerializeField]
    private Text textoInfo;

    private static Text textoPerk;
    [SerializeField]
    private Button ativarHabilidade;

    [SerializeField]
    private Button[] botoesTyva;
    [SerializeField]
    private Button[] botoesIovelik;

    [SerializeField]
    private GameObject painelTyva, painelIovelik;

    private static int qntPerk = 0;

    int indiceAtual, qntAtivas;

    private void Awake()
    {
        if (GameController.gameController.IsLoadedGame())
        {
            LoadActiveSkills();
        }
    }

    void LoadActiveSkills()
    {
        Skill[] activeSkills = null;
        if (GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.IOVELIK)
        {
            activeSkills = skillsIovelik;
        }
        else
        {
            activeSkills = skillsTyva;
        }

        foreach (int index in SaveSystem.data.habilidadesData.indiceAtivas)
        {
            activeSkills[index].AtivaSkill();
        }
    }

    private void Start()
    {
        SetHabilidadesAtivas();
        AtivaArvorePlayerEscolhido();
        textoPerk = textoInfo.transform.parent.GetChild(2).GetComponent<Text>();
    }

    public int[] GetActualSkillsActivityIndexes()
    {
        Skill[] activeSkills = null;
        if (GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.IOVELIK)
        {
            activeSkills = skillsIovelik;
        }
        else
        {
            activeSkills = skillsTyva;
        }

        List<int> retorno = new List<int>();

        for(int i = 0; i<activeSkills.Length; i++)
        {
            if (activeSkills[i].IsActive())
            {
                retorno.Add(i);
            }
        }

        return retorno.ToArray();
    } 

    public void Incrementa1Perk()
    {
        qntPerk++;
        textoPerk.text = qntPerk.ToString();
    }

    public static void IncrementaPerk()
    {
        qntPerk++;
        textoPerk.text = qntPerk.ToString();
    }

    private static void DecrementaPerk()
    {
        qntPerk--;
        textoPerk.text = qntPerk.ToString();
    }

    void AtivaArvorePlayerEscolhido()
    {
        if (GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.IOVELIK)
        {
            painelIovelik.SetActive(true);
        }

        else
        {
            painelTyva.SetActive(true);
        }
    }

    private void SetHabilidadesAtivas()
    {
        Skill[] skillsAtivar;
        Button[] buttonAtivar;
        if(GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.IOVELIK)
        {
            skillsAtivar = skillsIovelik;
            buttonAtivar = botoesIovelik;
        }
        else
        {
            skillsAtivar = skillsTyva;
            buttonAtivar = botoesTyva;
        }

        for(int i = 0; i<skillsAtivar.Length; i++)
        {
            if(i!=skillsAtivar.Length-1 && i%2 == 0)
            {
                skillsAtivar[i].podeAtivar = true;
                buttonAtivar[i].enabled = true;
                continue;
            }

            bool valor;
            if(i != skillsAtivar.Length-1 && i%2 == 1)
            {
                valor = skillsAtivar[i - 1].IsActive();
            }
            else
            {
                valor = (qntAtivas >= 6) ? true : false;
            }

            skillsAtivar[i].podeAtivar = valor;
        }

    }

    public void AtivaCondicaoIovelik()
    {
        if (GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.IOVELIK)
        {
            if (qntPerk > 0)
            {
                skillsIovelik[indiceAtual].AtivaSkill();
                qntAtivas++;
                SetHabilidadesAtivas();
                DecrementaPerk();
            }
        }
    }

    public void AtivaCondicaoTyva()
    {
        if (GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.TYVA)
        {
            if (qntPerk > 0)
            {
                skillsTyva[indiceAtual].AtivaSkill();
                qntAtivas++;
                SetHabilidadesAtivas();
                DecrementaPerk();
            }
        }
    }

    public void ShowInfoIovelikHabilidade(int index)
    {
        indiceAtual = index;
        textoInfo.text = skillsIovelik[index].descricaoHabilidade;
        if (skillsIovelik[index].podeAtivar && !skillsIovelik[index].IsActive())
        {
            ativarHabilidade.gameObject.SetActive(true);
        }
        else
        {
            ativarHabilidade.gameObject.SetActive(false);
        }
    }

    public void ShowInfoTyvaHabilidade(int index)
    {
        indiceAtual = index;
        textoInfo.text = skillsTyva[index].descricaoHabilidade;
        if (skillsTyva[index].podeAtivar && !skillsTyva[index].IsActive())
        {
            ativarHabilidade.gameObject.SetActive(true);
        }
        else
        {
            ativarHabilidade.gameObject.SetActive(false);
        }
    }
}
