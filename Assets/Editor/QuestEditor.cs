using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Quest))]
public class QuestEditor : Editor
{
    Quest q;
    int selecionado = 0;
    string[] options;
    List<GameObject> condHolders = new List<GameObject>();
    Transform placesRoot;

    SerializedObject obj;

    private void OnEnable()
    {
        obj = new SerializedObject(target);
        q = (Quest)target;
        placesRoot = GameObject.Find("CondHolder").transform;
        DrawPlaces(q);
    }

    private void OnDisable()
    {
        UndrawPlaces();
    }

    public override void OnInspectorGUI()
    {
        q.nome = EditorGUILayout.TextField("Título: ", q.nome);
        q.principal = EditorGUILayout.Toggle("Principal: ", q.principal);

        ShowConditions(obj.FindProperty("condicoes"));
        obj.ApplyModifiedProperties();

        ClearConditions();
    }

    void ClearConditions()
    {
        foreach(Condicoes c in q.condicoes)
        {
            c.CleanUnsusedConditions();
        }
    }

    void ShowConditions(SerializedProperty prop)
    {
        if (prop.isArray)
        {
            prop.arraySize = EditorGUILayout.IntField("Quantidade de Condições: ", prop.arraySize);
        }

        foreach(SerializedProperty p in prop)
        {
            string descricao = p.FindPropertyRelative("descricao").stringValue;
            if (string.IsNullOrEmpty(descricao))
                descricao = "Condição: ";

            GUILayout.BeginHorizontal();
            p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, descricao);
            GUILayout.EndHorizontal();

            if (p.isExpanded)
            {
                EditorGUILayout.PropertyField(p.FindPropertyRelative("tipoCondicao"));
                Condicoes.TipoCondicao tipoCond = (Condicoes.TipoCondicao)p.FindPropertyRelative("tipoCondicao").enumValueIndex;


                EditorGUILayout.PropertyField(p.FindPropertyRelative("local"));
                EditorGUILayout.PropertyField(p.FindPropertyRelative("descricao"));

                if (tipoCond == Condicoes.TipoCondicao.COMBATE)
                {
                    EditorGUILayout.PropertyField(p.FindPropertyRelative("inimigosDaCondicao"));
                    EditorGUILayout.PropertyField(p.FindPropertyRelative("raioDeSpawn"));
                }

                if (tipoCond == Condicoes.TipoCondicao.INTERACAO || tipoCond == Condicoes.TipoCondicao.DEVOLVE_ITEM)
                {
                    EditorGUILayout.PropertyField(p.FindPropertyRelative("interagivel"));
                }

                if (tipoCond == Condicoes.TipoCondicao.PEGA_ITEM || tipoCond == Condicoes.TipoCondicao.DEVOLVE_ITEM)
                {
                    EditorGUILayout.PropertyField(p.FindPropertyRelative("itemDaCondicao"));
                }

                if (tipoCond == Condicoes.TipoCondicao.IDA)
                {
                    EditorGUILayout.PropertyField(p.FindPropertyRelative("distanciaChegada"));
                }

                if(GUILayout.Button("Trazer Condição aqui"))
                {
                    Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2));
                    RaycastHit hit;

                    if(Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Ground")))
                    {
                        p.FindPropertyRelative("local").vector3Value = hit.point;
                    }
                }
            }          
        }
    }

    private void DrawPlaces(Quest q)
    {
        if (q.condicoes.Count > 0)
        {
            int i = 1;
            foreach (Condicoes c in q.condicoes)
            {
                GameObject condicaoHolder = new GameObject("Cond"+i, typeof(DrawQuesGizmo));
                condicaoHolder.transform.SetParent(placesRoot);

                condicaoHolder.GetComponent<DrawQuesGizmo>().SetCondicaoOnHolder(c);

                condHolders.Add(condicaoHolder);
                i++;
            }
        }
    }

    private void UndrawPlaces()
    {
        if (condHolders.Count > 0)
        {
            foreach(GameObject go in condHolders)
            {
                go.GetComponent<DrawQuesGizmo>().Clean();
                DestroyImmediate(go);
                
            }

            condHolders.Clear();
        }
    }
}
