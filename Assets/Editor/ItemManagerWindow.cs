using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Sprites;

public class ItemManagerWindow : EditorWindow
{

    string pathBase = "Assets/Itens/";
    
    List<Item> allItems = new List<Item>();

    Item itemAtual;

    Vector2 scrollPosition = Vector2.zero;

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

     void MakeANewCura()
     {
        Cura novaCura = new Cura();
        novaCura.nome = "Nova Cura" + (allItems.Count + 1);

        string novoPath = pathBase + novaCura.nome + ".asset";

        AssetDatabase.CreateAsset(novaCura, novoPath);
     }

     void MakeANewItem()
    {
        Item novoItem = new Item();
        novoItem.nome = "Novo Item" + (allItems.Count+1);

        string novoPath = pathBase + novoItem.nome + ".asset";

        AssetDatabase.CreateAsset(novoItem, novoPath);
    }

    void MakeANewArma()
    {
        Arma novaArma = new Arma();
        novaArma.nome = "Nova Arma" + (allItems.Count + 1);

        string novoPath = pathBase + novaArma.nome + ".asset";

        AssetDatabase.CreateAsset(novaArma, novoPath);
    }

     void RemoveItem()
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

    void ChangeName(Item item, string newName)
    {
        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(item), newName);
        Debug.Log("Roooodeiiii");
    }

    private void OnGUI()
    {
        GetAllItensAvaliable();


        GUILayout.BeginHorizontal("Items");

        #region BUTTONS
        GUILayout.BeginVertical();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("Items", VerticalAlligment());
        GUILayout.Space(10);

        foreach (Item item in allItems)
        {
            if (item.GetType() != typeof(Arma) && item.GetType() != typeof(Cura))
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

        EditorGUILayout.Separator();

        GUILayout.Label("Itens de Cura", VerticalAlligment());
        GUILayout.Space(10);

        foreach (Item item in allItems)
        {
            if (item.GetType() == typeof(Cura))
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

        if(GUILayout.Button("Nova Cura"))
        {
            MakeANewCura();
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

            GUILayout.BeginHorizontal();
            itemAtual.nome = EditorGUILayout.TextField("Nome: ", itemAtual.nome);
            itemAtual.name = itemAtual.nome;

            if (GUILayout.Button("Apply Name"))
            {
                ChangeName(itemAtual, itemAtual.nome);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            itemAtual.custoMoeda = EditorGUILayout.IntField("Custo em Moedas: ", itemAtual.custoMoeda);
            GUILayout.Space(3);
            itemAtual.peso = EditorGUILayout.FloatField("Peso: ", itemAtual.peso);
            GUILayout.Space(3);

            itemAtual.isItemMissao = EditorGUILayout.Toggle("Item é de Missão: ", itemAtual.isItemMissao);
            GUILayout.Space(6);

            GUILayout.Label("Ícone");
            //itemAtual.icone = (Sprite)EditorGUILayout.ObjectField(itemAtual.icone, typeof(Sprite));
            GUILayout.Space(3);

            if(itemAtual.icone != null)
            {
                Texture2D texture = SpriteUtility.GetSpriteTexture(itemAtual.icone, false);
                EditorGUI.DrawPreviewTexture(new Rect(new Vector2((position.width*(float)2/3)+100, (position.height/2)+100), Vector3.one*200), texture);
            }

            

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

            #region CURA

            if(itemAtual.GetType() == typeof(Cura))
            {
                EditorGUILayout.Separator();
                GUILayout.Label("Valor Cura");
                GUILayout.Space(10);

                Cura curaAtual = (Cura)itemAtual;

                curaAtual.quantidadeCura = EditorGUILayout.IntField("Quantidade de Cura", curaAtual.quantidadeCura);
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
    Texture2D GenerateTextureFromSprite(Sprite aSprite)
    {
        var rect = aSprite.rect;
        var tex = new Texture2D((int)rect.width, (int)rect.height);
        var data = aSprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
        tex.SetPixels(data);
        tex.Apply(true);
        return tex;
    }

}
