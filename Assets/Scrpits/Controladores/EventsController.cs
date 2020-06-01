using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventsController
{
    public delegate void OnMorteInimigo(int xp);
    public static OnMorteInimigo onMorteInimigoCallback;

    public delegate void OnPlayerChangeState(EstadoPlayer estadoPlayer);
    public static OnPlayerChangeState onPlayerStateChanged;

    public delegate void OnInteracao(Interagivel interagivel);
    public static OnInteracao onInteracao;

    public delegate void OnItemPego(Item item);
    public static OnItemPego onItemPego;

    public delegate void OnInventarioChange(Item item, bool mudanca);
    public static OnInventarioChange onInventarioChange; 

    public delegate void OnQuestLogChange(Quest quest, bool endQuest = false);
    public static OnQuestLogChange onQuestLogChange;

    public delegate void OnCondicaoTerminada(Quest quest);
    public static OnCondicaoTerminada onCondicaoTerminada;

    public delegate void OnDialogoTerminado(Dialogo dialogo);
    public static OnDialogoTerminado onDialogoTerminado;

    public delegate void OnLinhaTerminada(Dialogo dialogo);
    public static OnLinhaTerminada onLinhaTerminada;

    //public delegate void OnQuestConditionChange(Quest quest, List<Condicoes> condicoes);
    //public static OnQuestConditionChange onQuestConditionChange;
}

