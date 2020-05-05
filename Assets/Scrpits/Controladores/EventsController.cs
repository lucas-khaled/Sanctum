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
}

