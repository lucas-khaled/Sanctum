/***********************************************
                    Easy Scatter
	Copyright © 2014-2015 The Hedgehog Team
      http://www.thehedgehogteam.com/Forum/
		
	     The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

[System.Serializable]
public class ScatterPreference {

	// Brush
	public bool showBrushSize = true;
	public int brushDetail = 18;
	public float minBrushDrawSize =0.5f;
	public Color brushColor =new Color(24f/255f,118f/255f,175f/255f );
		
	public bool showCentralDot = true;
	public float dotSize=0.1f;

	public bool showNormal = true;
	public Color normalColor = new Color(147f/255f,244f/255f,66f/255f );

	// Prefab
	public bool keepPrefabLink=true;
	public bool showPrefabPreview=true;

	// Inspector
	public int maxSize = 200;
	public int maxAmount = 100;
	public float maxFlux = 50;


	public bool LoadPreference(){

		ScatterPreference  objXml = null;

		string[] guids = AssetDatabase.FindAssets( "EasyScaterPref",null);
		
		if (guids.Length >0){
			string path = AssetDatabase.GUIDToAssetPath( guids[0]);

			Stream fs = new FileStream(path,FileMode.Open);
			XmlSerializer serializer = new XmlSerializer(typeof(ScatterPreference));

			objXml = (ScatterPreference)serializer.Deserialize( fs);
			fs.Close();
		}

		if (objXml!=null){
			this.showBrushSize = objXml.showBrushSize;
			this.brushDetail = objXml.brushDetail;
			this.minBrushDrawSize = objXml.minBrushDrawSize;
			this.brushColor = objXml.brushColor;

			this.showCentralDot = objXml.showCentralDot;
			this.dotSize = objXml.dotSize;

			this.showNormal = objXml.showNormal;
			this.normalColor = objXml.normalColor;
			this.keepPrefabLink = objXml.keepPrefabLink;
			this.showPrefabPreview = objXml.showPrefabPreview;

			this.maxSize = objXml.maxSize;
			this.maxAmount = objXml.maxAmount;
			this.maxFlux = objXml.maxFlux;

			return true;
		}
		else{
			SavePreference();
			return false;
		}
	}

	public void SavePreference(){

		string[] guids = AssetDatabase.FindAssets( "EasyScatterWindow",null);

		string path = AssetDatabase.GUIDToAssetPath( guids[0]);
		
		path = Path.GetDirectoryName( path) + "/EasyScaterPref.xml";
		
		Stream fs = new FileStream(path, FileMode.Create);
		XmlWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
		XmlSerializer serializer = new XmlSerializer(typeof(ScatterPreference));
		serializer.Serialize(writer, this);
		writer.Close(); 
		
		AssetDatabase.Refresh();
	}

	public void LoadDefault(){
		showBrushSize = true;
		brushDetail = 18;
		minBrushDrawSize =0.5f;
		brushColor =new Color(24f/255f,118f/255f,175f/255f );
		
		showCentralDot = true;
		dotSize=0.1f;
		
		showNormal = true;
		normalColor = new Color(147f/255f,244f/255f,66f/255f );

		keepPrefabLink = true;
		showPrefabPreview = true;

		maxSize = 200;
		maxAmount = 100;
		maxFlux = 50;

		SavePreference();
	}
}
