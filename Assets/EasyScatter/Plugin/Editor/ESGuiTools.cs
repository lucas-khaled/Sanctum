/***********************************************
                    Easy Scatter
	Copyright © 2014-2015 The Hedgehog Team
      http://www.thehedgehogteam.com/Forum/
		
	     The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using UnityEditor;
using System.Collections;

public class ESGuiTools{

	
	public static void Title(string text,bool bold= true,int width=200){
			
		if (bold){
			text = "<b><size=11>" + text + "</size></b>";
		}
		else{
			text = "<size=11>" + text + "</size>";
		}
		
		GUILayout.Toggle(true,text,"dragtab",GUILayout.Width(width));
			
	}

	public static void SimpleTitle(string text){

		GUIStyle labelStyle = new GUIStyle("label");
		labelStyle.fontStyle = FontStyle.Bold;
		labelStyle.fontSize = 11;
		EditorGUILayout.LabelField(text,labelStyle);
	}


	public static bool FoldOut(string text,bool foldOut, bool bold=true, bool endSpace=true){

		if (bold){
			text = "<b><size=11>" + text + "</size></b>";
		}
		if (foldOut){
			text = "\u25BC " + text;
		}
		else{
			text = "\u25BA " + text;
		}
		
		if ( !GUILayout.Toggle(true,text,"dragtab")){
			foldOut=!foldOut;
		}
		
		if (!foldOut && endSpace)GUILayout.Space(5f);
		
		return foldOut;
	}

	public static bool SimpleFoldOut(string text,bool foldOut, bool bold=true,bool leftAligment=false, bool endSpace=true){

		if (foldOut){
			text = "\u25BC " + text;
		}
		else{
			text = "\u25BA " + text;
		}
		
		if ( !GUILayout.Toggle(true,text,"label")){
			foldOut=!foldOut;
		}
		
		if (!foldOut && endSpace)GUILayout.Space(5f);
		
		return foldOut;
	}
	
	public static bool ToogleButton(bool state,string label,  ref Texture2D preview ,int width=0,GameObject prefab=null, bool showPreview=false){

		if (width!=0){
			state = GUILayout.Toggle( state,label,new GUIStyle("Button"), GUILayout.Width(width) ); 
		}
		else{

			GUIContent content = new GUIContent();

			if (showPreview){
				if ( preview ==null){
					preview = AssetPreview.GetAssetPreview( prefab);
					content.image = preview;
					content.text = label;
					state = GUILayout.Toggle( state,content,new GUIStyle("Button"),GUILayout.Height(20) ); 
				}
				else{
					content.image = preview;
					content.text="";
					state = GUILayout.Toggle( state,content,new GUIStyle("Button"),GUILayout.Height(75) ); 
				}
			}
			else{
				content.text = label;
				state = GUILayout.Toggle( state,content,new GUIStyle("Button"),GUILayout.Height(20) ); 
			}


		}
	
		return state;
	}

	static public bool Button(string label,Color color,int width,int height=0, bool leftAligment=false, Texture2D icon=null ){
		
		GUI.backgroundColor  = color;
		GUIStyle buttonStyle = new GUIStyle("Button");
		
		if (leftAligment)
			buttonStyle.alignment = TextAnchor.MiddleLeft;

		GUIContent guiContent = new GUIContent(label,icon);
		if (height==0){
			if (GUILayout.Button( guiContent,buttonStyle,GUILayout.Width(width))){
				GUI.backgroundColor = Color.white;
				return true;	
			}
		}
		else{
			if (GUILayout.Button( guiContent,buttonStyle,GUILayout.Width(width),GUILayout.Height(height))){
				GUI.backgroundColor = Color.white;
				return true;	
			}			
		}
		GUI.backgroundColor = Color.white;		
		
		return false;
	}

	public static void BeginGroup(int padding=0){
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(padding);
		EditorGUILayout.BeginHorizontal( GUILayout.MinHeight(10f));
		GUILayout.BeginVertical();
		GUILayout.Space(2f);
		
	}

	public static void EndGroup(bool endSpace = true){
		GUILayout.Space(3f);
		GUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(3f);
		GUILayout.EndHorizontal();
		
		if (endSpace){
			GUILayout.Space(10f);
		}
	}
}

	