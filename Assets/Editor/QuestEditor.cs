using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

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
        EditorGUILayout.PropertyField(obj.FindProperty("reward"));

        EditorGUILayout.PropertyField(obj.FindProperty("dialogo"));

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

                bool onScene = false;
                if (tipoCond == Condicoes.TipoCondicao.INTERACAO || tipoCond == Condicoes.TipoCondicao.DEVOLVE_ITEM || tipoCond == Condicoes.TipoCondicao.FALA)
                {
                    EditorGUILayout.PropertyField(p.FindPropertyRelative("isOnScene"));
                    onScene = p.FindPropertyRelative("isOnScene").boolValue;

                    if (onScene)
                    {
                        EditorGUILayout.PropertyField(p.FindPropertyRelative("nameOnScene"));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(p.FindPropertyRelative("interagivel"));
                    }
                }

                if (tipoCond == Condicoes.TipoCondicao.PEGA_ITEM || tipoCond == Condicoes.TipoCondicao.DEVOLVE_ITEM)
                {
                    EditorGUILayout.PropertyField(p.FindPropertyRelative("itemDaCondicao"));
                }

                if (tipoCond == Condicoes.TipoCondicao.IDA)
                {
                    EditorGUILayout.PropertyField(p.FindPropertyRelative("distanciaChegada"));
                }

                if(tipoCond == Condicoes.TipoCondicao.FALA)
                {
                    EditorGUILayout.PropertyField(p.FindPropertyRelative("dialogoDaCondição"));
                }

                if (onScene)
                {
                    if (GUILayout.Button("Levar Condição até o interagível"))
                    {
                        ToSceneInteractableEditor(p);
                    }
                }

                else
                {
                    if (GUILayout.Button("Trazer Condição aqui"))
                    {
                        Camera cam = SceneView.lastActiveSceneView.camera;
                        Ray ray = cam.ScreenPointToRay(new Vector3(((float)(cam.pixelWidth-1)/2), ((float)(cam.pixelHeight-1)/2)));
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Ground")))
                        {
                            p.FindPropertyRelative("local").vector3Value = hit.point;
                        }
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

     void ToSceneInteractableEditor(SerializedProperty p)
    {
       p.FindPropertyRelative("interagivel").objectReferenceValue = GameObject.Find(p.FindPropertyRelative("nameOnScene").stringValue).GetComponent<Interagivel>();       
       p.FindPropertyRelative("local").vector3Value = p.FindPropertyRelative("interagivel").FindPropertyRelative("transform").FindPropertyRelative("position").vector3Value;
    }
}
