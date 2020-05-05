using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController gameController;

    private string personagemEscolhido; 

    private void Awake()
    {
        gameController = this;
        DontDestroyOnLoad(this);
    }

    public void EscolherPersonagem(string personagem)
    {
        personagemEscolhido = personagem;
    }

    public string GetPersonagemEscolhido()
    {
        return personagemEscolhido;
    }

    public void ChangeScene(string cena)
    {
        SceneManager.LoadScene(cena);
    }
}
