using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusPlayer : MonoBehaviour
{
    public int maxColetavel = 3;
    public float tempoEscudo = 3;
    public float tempoDashTotal = 2;
    public int maxVida = 100;

    private int dinheiro, fama, vida, qntColetavel;
    private int XPRequisito = 100;
    private int experiencia, level = 1;
    private const int MAXLEVEL = 100;
    private int danoMedio = 10;
    public StatusPlayer status;


    public int QntColetavel
    {
        get
        {
            return qntColetavel;
        }
        set
        {
            qntColetavel = Mathf.Clamp(value, 0, maxColetavel);
            UIController.uiController.Usandinho();
        }
    }

    public int Vida
    {
        get
        {
            return vida;
        }

        set
        {
            vida = Mathf.Clamp(value, 0, maxVida);
            UIController.uiController.LifeBar(((float)value / maxVida));//controle barra de vida
        }
    }

    public int Fama
    {
        get { return fama; }
        set
        {
            fama = value;
        }
    }

    public int Dinheiro
    {
        get
        {
            return dinheiro;
        }
        set
        {
            dinheiro = Mathf.Clamp(value, 0, int.MaxValue);
        }
    }

    public int DanoMedio
    {
        get
        {
            return danoMedio;
        }
        private set
        {
            danoMedio = value;
        }
    }

    public int Experiencia
    {
        get { return experiencia; }
        set
        {
            if (level < MAXLEVEL)
            {
                experiencia = value;
                if (experiencia >= XPRequisito)
                {
                    experiencia -= XPRequisito;
                    Level++;
                }
                UIController.uiController.XPbar(((float)experiencia / XPRequisito));//controle barra de xp
            }
        }
    }

    public int Level
    {
        get { return level; }
        set
        {
            level = value;
            XPRequisito += 50 + 10 * level;
            DanoMedio += 2;
        }
    }

    private void Awake()
    {
        vida = maxVida;
    }
}
