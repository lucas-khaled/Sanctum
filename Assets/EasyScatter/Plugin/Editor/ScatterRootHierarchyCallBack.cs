/***********************************************
                    Easy Scatter
	Copyright © 2014-2015 The Hedgehog Team
      http://www.thehedgehogteam.com/Forum/
		
	     The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using System;
using UnityEditor;

[InitializeOnLoad]
public class ScatterRootHierarchyCallBack {

	private static readonly EditorApplication.HierarchyWindowItemCallback hiearchyItemCallback;
	private static Texture2D hierarchyIcon;
	private static Texture2D HierarchyIcon {
		get {
			if (ScatterRootHierarchyCallBack.hierarchyIcon==null){
				ScatterRootHierarchyCallBack.hierarchyIcon = (Texture2D)Resources.Load( "paint");
			}
			return ScatterRootHierarchyCallBack.hierarchyIcon;
		}
	}

	static ScatterRootHierarchyCallBack()
	{
		ScatterRootHierarchyCallBack.hiearchyItemCallback = new EditorApplication.HierarchyWindowItemCallback(ScatterRootHierarchyCallBack.DrawHierarchyIcon);
		EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(EditorApplication.hierarchyWindowItemOnGUI, ScatterRootHierarchyCallBack.hiearchyItemCallback);
		
	}
	
	private static void DrawHierarchyIcon(int instanceID, Rect selectionRect)
	{
		GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
		if (gameObject != null && gameObject.GetComponent<ScatterRoot>() != null)
		{
			Rect rect = new Rect(selectionRect.x + selectionRect.width - 16f, selectionRect.y, 16f, 16f);
			GUI.DrawTexture( rect,ScatterRootHierarchyCallBack.HierarchyIcon);
		}		

	}
}
