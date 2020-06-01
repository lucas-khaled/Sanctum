using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ItemManagerWindow : EditorWindow
{

    string pathBase = "Assets/Itens/";
    
    List<Item> allItems = new List<Item>();

    Item itemAtual;


    [MenuItem("Window/Item Manager")]
    public static void ShowWindow()
    {
        GetWindow<ItemManagerWindow>("Manager de Itens");
    }

    private void GetAllItensAvaliable()
    {
        Item[] itens = GetAtPath<Item>("Itens/");

        allItems.Clear();

        foreach(Item item in itens)
        {
             allItems.Add(item);
        }
    }

    public void MakeANewItem()
    {
        Item novoItem = new Item();
        novoItem.nome = "Novo Item" + allItems.Count+1;

        string novoPath = pathBase + novoItem.nome + ".asset";

        AssetDatabase.CreateAsset(novoItem, novoPath);
    }

    public void MakeANewArma()
    {
        Arma novaArma = new Arma();
        novaArma.nome = "Nova Arma" + allItems.Count + 1;

        string novoPath = pathBase + novaArma.nome + ".asset";

        AssetDatabase.CreateAsset(novaArma, novoPath);
    }

    public void RemoveItem()
    {
        string path = AssetDatabase.GetAssetPath(itemAtual);
        itemAtual = null;

        AssetDatabase.DeleteAsset(path);
    }

    private GUIStyle VerticalAlligment()
    {
        GUIStyle vertical = new GUIStyle();
        vertical.alignment = TextAnchor.UpperCenter;

        return vertical;
    }

    private void OnGUI()
    {
        GetAllItensAvaliable();


        GUILayout.BeginHorizontal("Items");

        #region BUTTONS
        GUILayout.BeginVertical();

        GUILayout.BeginScrollView(Vector2.zero);

        GUILayout.Label("Items", VerticalAlligment());
        GUILayout.Space(10);

        foreach (Item item in allItems)
        {
            if (item.GetType() != typeof(Arma))
            {
                if (GUILayout.Button(item.nome))
                {
                    itemAtual = item;
                }
                GUILayout.Space(3);
            }
        }

        EditorGUILayout.Separator();

        GUILayout.Label("Armas", VerticalAlligment());
        GUILayout.Space(10);

        foreach (Item item in allItems)
        {
            if (item.GetType() == typeof(Arma))
            {
                if (GUILayout.Button(item.nome))
                {
                    itemAtual = item;
                }
                GUILayout.Space(3);
            }
        }

        GUILayout.EndScrollView();

        #region Opções
        EditorGUILayout.Separator();

        GUILayout.Label("Opções", VerticalAlligment());
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        

        if(GUILayout.Button("Novo Item"))
        {
            MakeANewItem();
        }

        if(GUILayout.Button("Nova Arma"))
        {
            MakeANewArma();
        }

        if (itemAtual != null) {
            if (GUILayout.Button("Remover Item"))
            {
                RemoveItem();
            }
        }

        GUILayout.EndHorizontal();
        #endregion

        GUILayout.EndVertical();
        #endregion

        EditorGUILayout.Separator();

        #region INFO

        if (itemAtual != null)
        {
            GUILayout.BeginVertical();

            itemAtual.nome = EditorGUILayout.TextField("Nome: ", itemAtual.nome);
            GUILayout.Space(3);
            itemAtual.custoMoeda = EditorGUILayout.IntField("Custo em Moedas: ", itemAtual.custoMoeda);
            GUILayout.Space(3);
            itemAtual.peso = EditorGUILayout.FloatField("Peso: ", itemAtual.peso);
            GUILayout.Space(3);

            itemAtual.isItemMissao = EditorGUILayout.Toggle("Item é de Missão: ", itemAtual.isItemMissao);
            GUILayout.Space(6);

            GUILayout.Label("Ícone");
            itemAtual.icone = (Sprite)EditorGUILayout.ObjectField(itemAtual.icone, typeof(Sprite));
            GUILayout.Space(3);

            GUILayout.Label("Descrição:");
            itemAtual.descricao = EditorGUILayout.TextArea(itemAtual.descricao);
            GUILayout.Space(10);

            #region Arma
            if (itemAtual.GetType() == typeof(Arma))
            {
                EditorGUILayout.Separator();
                GUILayout.Label("Valores de Arma", VerticalAlligment());
                GUILayout.Space(10);

                Arma armaAtual = (Arma)itemAtual;

                armaAtual.dano = EditorGUILayout.IntField("Dano: ", armaAtual.dano);
                GUILayout.Space(3);

                armaAtual.nivelMinimo = EditorGUILayout.IntField("Nivel Míninimo: ", armaAtual.nivelMinimo);
                GUILayout.Space(3);

                armaAtual.famaMinima = EditorGUILayout.IntField("Fama Mínima: ", armaAtual.famaMinima);
                GUILayout.Space(3);

                armaAtual.armaPlayer = (TipoPlayer)EditorGUILayout.EnumPopup("Player que usa essa arma", armaAtual.armaPlayer);
                GUILayout.Space(3);

                GUILayout.Label("Mesh");
                armaAtual.armaMesh = (Mesh)EditorGUILayout.ObjectField(armaAtual.armaMesh, typeof(Mesh));
            }
            #endregion


            GUILayout.EndVertical();
        }

        #endregion

        GUILayout.EndHorizontal();
    }

    public static T[] GetAtPath<T>(string path)
    {
        ArrayList al = new ArrayList();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);
        
        foreach (string fileName in fileEntries)
        {
            int index = fileName.LastIndexOf("/");
            string localPath = "Assets/" + path;

            if (index > 0)
                localPath += fileName.Substring(index);

            if (!localPath.Contains(".meta"))
            {
                Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

                if (t != null)
                    al.Add(t);
            }
        }
        T[] result = new T[al.Count];
        for (int i = 0; i < al.Count; i++)
            result[i] = (T)al[i];

        return result;
    }
}
