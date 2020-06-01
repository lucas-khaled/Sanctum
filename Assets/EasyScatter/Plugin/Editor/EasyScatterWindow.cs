/***********************************************
                    Easy Scatter
	Copyright © 2014-2015 The Hedgehog Team
      http://www.thehedgehogteam.com/Forum/
		
	     The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[System.Serializable]
public class EasyScatterWindow : EditorWindow {

	static string Version="1.1.6";
	static string date="September 11, 2016";

	[MenuItem("Window/Easy Scatter")]
	static void ScatterMeneu() {
		
		EasyScatterWindow window = (EasyScatterWindow) EditorWindow.GetWindow (typeof (EasyScatterWindow));
		window.Show();
	}
		
	#region Members
	private Vector2 scrollView = Vector2.zero;
	private int toolIndex=-1;
	
	private Texture2D[] icons = new Texture2D[8];
	private Texture2D[] toolbarIcons = new Texture2D[4];
	private Texture2D deleteCursor;

	// Brush
	[SerializeField]
	private ScatterBrush brush = new ScatterBrush();
	private SerializedObject so;


	// Prefab
	private List<ScatteredObject> scatteredObjects = new List<ScatteredObject>();
	private bool isScatteredObjetWaitToDelete = false;

	[SerializeField]
	private ScatteredObject prefabGlobalSetting = new ScatteredObject();

	// To compute drag distance
	private Vector3 oldPosition;

	// Layer
	private Transform rootPivot;
	private bool isLayerWaitToDelete = false;	
	private int previousStartIndex = 0;
	
	// Selection
	private List<Transform> dynamicSelections = new List<Transform>();
	[SerializeField]
	private ScatteredObject scatteredModel = new ScatteredObject();
	private bool isSelectionShowOrder = true;
	private bool isSelectionSurface=false;
	private bool isSelectionKeepRot = false;
	private bool isRelativeRotation = false;

	private ScatterBrush.AlignVector selectionAlignVector;

	private Transform LastTransform;

	// Preference
	[SerializeField]
	private ScatterPreference preference = null;//new ScatterPreference();

	#endregion

	#region Editor Callback
	void OnEnable(){
		this.titleContent = new GUIContent("Easy Scatter");
		this.minSize = new Vector2(325,100);
		InitIncons();
		so = new UnityEditor.SerializedObject( this);
		preference = new ScatterPreference();
		preference.LoadPreference();
		SceneView.onSceneGUIDelegate += SceneGUI;

	}

	void OnDisable(){
		SceneView.onSceneGUIDelegate -= SceneGUI;
	}
	
	void OnGUI () {

		int oldIndex = toolIndex;
		toolIndex = GUILayout.SelectionGrid( toolIndex,toolbarIcons ,4);

		scrollView = EditorGUILayout.BeginScrollView( scrollView);

		switch (toolIndex){
		case -1:
			EditorGUILayout.HelpBox("No tool selected \nPlease select a tool", MessageType.None);
			break;
		case 0:
			EditorGUILayout.HelpBox("Paint gameobject \nSelect prefabs or drag'n drop, then click to paint", MessageType.None);
			DrawPaintInspector();
			break;
		case 1:
			EditorGUILayout.HelpBox("Applies changes to a selection of gameobject", MessageType.None);
			bool newSelection = false;
			if (oldIndex != toolIndex) newSelection = true;
			DrawSelectionInspector( newSelection);
			break;
		case 2:
			DrawPreferenceInspector();
			break;
		case 3:
			DrawInfosInspector();
			break;
		}

		DragDrop();

		ShortCut();

		EditorGUILayout.EndScrollView();

	}

	void OnSelectionChange(){

		if (toolIndex==1){

			int count = Selection.transforms.Length;

			// one selection
			if (count==1){
				dynamicSelections.Clear();
				dynamicSelections.Add (Selection.activeTransform);
			}
			else if (count==dynamicSelections.Count+1){
				if (dynamicSelections.Contains( Selection.activeTransform)){
					dynamicSelections = Selection.transforms.ToList<Transform>();
				}
				else{
					dynamicSelections.Add (Selection.activeTransform);
				}

			}
			else if (count<dynamicSelections.Count){
				dynamicSelections = dynamicSelections.Intersect(Selection.transforms.ToList<Transform>()).ToList();
			}
			else if (count>dynamicSelections.Count){
				dynamicSelections = Selection.transforms.ToList<Transform>();
			}

		}

		//
		if (Selection.activeGameObject){

			ScatterRoot tmpRoot = Selection.activeGameObject.GetComponent<ScatterRoot>();
			if (tmpRoot){
				rootPivot = Selection.activeGameObject.transform;
				scatteredObjects = rootPivot.GetComponent<ScatterRoot>().scatteredObjects;
			}
		}
	}
	
	void SceneGUI(SceneView sceneView){
	
		ShortCut();

		if (toolIndex ==1){
			GUIDrawSelection();
		}

		// Scale rotate last transform
		if (Event.current.type == EventType.MouseDrag && Event.current.button == 0 && Event.current.control){
			Vector2 delta = Event.current.delta;

			if (LastTransform){
				LastTransform.localScale -= Vector3.one * (delta.y/100f);
				LastTransform.Rotate( Vector3.up * delta.x );
			}

		}


		// Exit if alt or ctrl key
		if (Event.current.alt && Event.current.type !=EventType.ScrollWheel || (Event.current.control  && toolIndex!=1 )) return;

		// Get current picked object
		Vector2 mousePos = Event.current.mousePosition;

		float pixelPerPoint = EditorGUIUtility.pixelsPerPoint;
		//#if UNITY_EDITOR_OSX
		mousePos.y = Screen.height - (mousePos.y * pixelPerPoint) - (40 * pixelPerPoint);
		mousePos.x = pixelPerPoint * mousePos.x;
		//#else
		//	mousePos.y = Screen.height - mousePos.y - 40;
		//#endif




		Camera camEditor = UnityEditor.SceneView.lastActiveSceneView.camera;
		if (camEditor==null) return;
		Ray ray = camEditor.ScreenPointToRay(mousePos);

		RaycastHit hit;
		if (Physics.Raycast( ray,out hit,Mathf.Infinity,brush.pickableLayer)){

			// Drawing
			if (toolIndex==0){

				var rect = new Rect( 0,0,Screen.width,Screen.height);
				EditorGUIUtility.AddCursorRect(rect, MouseCursor.Arrow);

				// Paint
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
				Tools.current = Tool.None;

				// Click
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0){
					DoPaint( hit.point, hit.normal);
					oldPosition = hit.point;
				}

				// Drag
				if (Event.current.type == EventType.MouseDrag && Event.current.button == 0){

					if (Vector3.Distance(oldPosition, hit.point)>brush.flux){
						DoPaint( hit.point, hit.normal);
						oldPosition = hit.point;
					}
				}

				// Right Click delete
				if ((Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown)&& Event.current.button == 1){
					if (isShift){
						Event.current.Use();
						DoDelete( hit.point );
					}
				}



			}

			// DrawBrush
			if (toolIndex==0 || toolIndex==2){
				GUIDrawBrush( hit.point, brush.size/2, hit.normal);
			}
		}
		
		sceneView.Repaint();

	}
	
	void OnInspectorUpdate() {
		Repaint();
	}

	void GUIDrawBrush (Vector3 position, float radius, Vector3 normal){


		radius = radius<preference.minBrushDrawSize?preference.minBrushDrawSize:radius;

		if (preference.showBrushSize){
			Vector3[] corners = new Vector3[preference.brushDetail+1];
			float step = 360f/preference.brushDetail;


			Quaternion rot = Quaternion.FromToRotation( Vector3.up,normal);

			for (int i=0; i<=corners.Length-1; i++){

				corners[i] = new Vector3( Mathf.Sin(step*i*Mathf.Deg2Rad), 0, Mathf.Cos(step*i*Mathf.Deg2Rad) ) * radius  + position;
				Vector3 dir = corners[i] - position;
				dir = rot * dir;
				corners[i] = dir + position;

				RaycastHit hit;
				if (Physics.Raycast(corners[i]+ normal.normalized, -normal,out hit,brush.size/2, brush.pickableLayer)){
					corners[i] = hit.point;
				}

			}

			Handles.color = preference.brushColor;
			Handles.DrawAAPolyLine(3, corners);
		}

		if (preference.showCentralDot){
			Handles.color = new Color(preference.brushColor.r,preference.brushColor.g,preference.brushColor.b,0.3f);
			Handles.DrawSolidDisc( position,normal,preference.dotSize);
			//Handles.DrawSolidDisc( position,normal,brush.size/2f);
		}

		if (preference.showNormal){
			Handles.color = preference.normalColor;
			if (brush.align2Surface){
                //Handles.ArrowCap( 0,position,Quaternion.LookRotation( normal,Vector3.up),2);
                Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(normal, Vector3.up), 2, EventType.Ignore);
            }
			else{
				//Handles.ArrowCap( 0,position,Quaternion.LookRotation( Vector3.up,Vector3.up),2);
                Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(Vector3.up, Vector3.up), 2, EventType.Ignore);
            }
		}

	}
	
	void GUIDrawSelection(){

		if (isSelectionShowOrder){
			int count=1;
			if (dynamicSelections.Count<500){
				foreach( Transform tr in dynamicSelections){
					if (tr){
					GUIStyle label = new GUIStyle("label");
					label.fontSize = 12;
					label.fontStyle = FontStyle.Bold;
					label.normal.textColor = Color.white;
					Handles.Label( tr.position + Vector3.up ,count.ToString(),label);
					count++;
					}
				}
			}
		}
	}
	#endregion

	#region Paint Inspector
	void DrawPaintInspector(){

		ESGuiTools.SimpleTitle("Root");
		ESGuiTools.BeginGroup();
		DrawLayerInspector();
		ESGuiTools.EndGroup();

		ESGuiTools.SimpleTitle("Brushes");
		ESGuiTools.BeginGroup();
		DrawBrushInspector();
		ESGuiTools.EndGroup();

		ESGuiTools.SimpleTitle("Prefabs");
		ESGuiTools.BeginGroup();
		DrawPrefabInspector();
		ESGuiTools.EndGroup();
	}

	void DrawLayerInspector(){


		int size = 18;

		GUI.enabled = !isLayerWaitToDelete;

		// New layer
		EditorGUILayout.BeginHorizontal();
		if (ESGuiTools.Button("New",Color.white,50,size)){
			if (!rootPivot || (rootPivot && rootPivot.childCount>0)){
				CreateRootPivot();
			}
		}

		// Set select
		if (ESGuiTools.Button("Get",Color.white,50,size)){
			GameObject tmpobj = (Selection.activeObject as GameObject);
			if (tmpobj){

				if (tmpobj.GetComponent<ScatterRoot>()){
					rootPivot = tmpobj.transform;
					scatteredObjects = rootPivot.GetComponent<ScatterRoot>().scatteredObjects;

				}
				else{
					if (EditorUtility.DisplayDialog("Easy Scatter warning","This GameObject is not an Easy Scatter Root \n Updating to Easy Scatter Root","Yes","No")){
						rootPivot = tmpobj.transform;
						rootPivot.gameObject.AddComponent<ScatterRoot>();
						InitScatteredList( rootPivot.gameObject);
					}
				}
			}
		}

		GUI.enabled = true;
		// Clear layer
		if (ESGuiTools.Button("Clear",Color.white,50,size)){
			isLayerWaitToDelete = !isLayerWaitToDelete;
		}
		if (isLayerWaitToDelete){	
		    if (ESGuiTools.Button("Confirm",Color.red,100,size)){
				ClearRootPivot();
				isLayerWaitToDelete = false;
				previousStartIndex = 0;
			}
		}

		EditorGUILayout.EndHorizontal();

		if (rootPivot){
			rootPivot.name = EditorGUILayout.TextField("Name",rootPivot.name);
		}

		int count=0;
		if (rootPivot){
			count = rootPivot.childCount;
		}
		EditorGUILayout.LabelField( count.ToString() + " objects in root");
	}
	
	void DrawBrushInspector(){

		brush.size = EditorGUILayout.Slider(new GUIContent("Size",null,"Page Up / Page Down"),brush.size,0,preference.maxSize);
		brush.amount = EditorGUILayout.IntSlider("Amount",brush.amount,1,preference.maxAmount);
		brush.flux = EditorGUILayout.Slider("Flux",brush.flux,0,preference.maxFlux);

		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Slope",GUILayout.Width(50));

		brush.minSlope = EditorGUILayout.FloatField("",brush.minSlope,GUILayout.Width(40));
		EditorGUILayout.MinMaxSlider( ref brush.minSlope,ref brush.maxSlope,0,90);
		brush.maxSlope = EditorGUILayout.FloatField("",brush.maxSlope,GUILayout.Width(40));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( "Align to", GUILayout.Width(50));
		brush.align2View = EditorGUILayout.ToggleLeft("view",brush.align2View, GUILayout.Width(42));

		brush.align2Surface = EditorGUILayout.ToggleLeft("surface",brush.align2Surface, GUILayout.Width(57));

		bool tmpPrevious = EditorGUILayout.ToggleLeft("previous",brush.align2Previous, GUILayout.Width(65));
		if (!brush.align2Previous && tmpPrevious){
			previousStartIndex = 0;
		}
		brush.align2Previous = tmpPrevious;
		if (brush.align2Previous){
			EditorGUILayout.BeginVertical();
			brush.alignVector = (ScatterBrush.AlignVector)EditorGUILayout.EnumPopup("",brush.alignVector ,GUILayout.Width(50));
			brush.loop = EditorGUILayout.ToggleLeft("loop",brush.loop, GUILayout.Width(42));
			EditorGUILayout.EndVertical();
		}

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		so.Update();
		SerializedProperty layer = so.FindProperty("brush.pickableLayer");
		EditorGUILayout.PropertyField( layer,true);
		so.ApplyModifiedProperties();
		
		brush.advanced = ESGuiTools.SimpleFoldOut("Advanced options",brush.advanced );
		if (brush.advanced){
			EditorGUI.indentLevel++;
			brush.overRide = EditorGUILayout.Toggle("Override",brush.overRide);
			brush.layer = EditorGUILayout.LayerField("Layer",brush.layer);
			brush.tag = EditorGUILayout.TagField("Tag",brush.tag);
			
			brush.lightmapStatic = EditorGUILayout.Toggle( "Lightmap Static",brush.lightmapStatic);
			brush.batchingStatic = EditorGUILayout.Toggle( "Batching Static",brush.batchingStatic);
			brush.occludeeStatic = EditorGUILayout.Toggle( "Occludee Static",brush.occludeeStatic);
			EditorGUI.indentLevel--;
		}

	}
	
	void DrawPrefabInspector(){

		EditorGUILayout.Space();

		#region Header
		EditorGUILayout.BeginHorizontal();
		if (ESGuiTools.Button("Unselect",Color.white,140,18)){
			for (int i=0;i<scatteredObjects.Count;i++){
				scatteredObjects[i].enable = false;
			}
			
		}
		
		if (ESGuiTools.Button("Clear",Color.white,50,18)){
			isScatteredObjetWaitToDelete = !isScatteredObjetWaitToDelete;
		}

		if (isScatteredObjetWaitToDelete){
			if (ESGuiTools.Button("Confirm",Color.red,100,18)){
				scatteredObjects.Clear();
				isScatteredObjetWaitToDelete = false;
			}
		}
		EditorGUILayout.EndHorizontal();


		// Global Setting
		DrawPrefabChild( prefabGlobalSetting,true);
		#endregion

		EditorGUILayout.Space();


		for (int i=0;i<scatteredObjects.Count;i++){
			if (scatteredObjects[i].prefab != null){
				DrawPrefabChild( scatteredObjects[i],false );
			}
			else{
				scatteredObjects.Remove(scatteredObjects[i]);
			}
		}

	}
		
	void DrawPrefabChild(ScatteredObject scatterChild, bool isGlobal){

		Color btnColor = Color.white;
		int size = 18;

		bool isDelete = false;
		
		#region GameObject

		EditorGUILayout.BeginHorizontal();
		// Enable

		GUI.enabled = !scatterChild.isWait2delete;
		bool tmpButtonState = ESGuiTools.ToogleButton(scatterChild.enable,scatterChild.prefab!=null?scatterChild.prefab.name:"Global setting",ref scatterChild.preview,0, scatterChild.prefab,preference.showPrefabPreview);

		if (!isGlobal){
			if (Event.current.control){
				scatterChild.enable = tmpButtonState;
			}
			else{
				if (!scatterChild.enable && tmpButtonState ){
					scatterChild.enable = true;
					
					List<ScatteredObject> tmpobject = scatteredObjects.FindAll( delegate( ScatteredObject s) {
						return s.enable==true && s!=scatterChild;
					}
					);
					
					foreach(ScatteredObject sco in tmpobject){
						sco.enable=false;
					}
				}
				else{
					scatterChild.enable = tmpButtonState;
				}
			}
		}
		else{
			scatterChild.enable = tmpButtonState;
		}
		
		
		// Disable other button
		GUI.enabled = true;
		
		
		// options
		if (!scatterChild.isWait2delete){
			
			btnColor = Color.white;
			
			
			// Scale
			if (scatterChild.isScale) btnColor = Color.green;
			if (ESGuiTools.Button("",btnColor,size+4,size,false,GetIcon(0))){
				scatterChild.isScale = !scatterChild.isScale;
			}
			btnColor = Color.white;
			
			
			// X Rotate
			if (scatterChild.isRotationX) btnColor = Color.green;
			if (ESGuiTools.Button("",btnColor,size+4,size,false,GetIcon(1))){
				scatterChild.isRotationX = !scatterChild.isRotationX;
			}
			btnColor = Color.white;
			
			
			// Y Rotate
			if (scatterChild.isRotationY) btnColor = Color.green;
			if (ESGuiTools.Button("",btnColor,size+4,size,false,GetIcon(2))){
				scatterChild.isRotationY = !scatterChild.isRotationY;
			}
			btnColor = Color.white;
			
			// Z Rotate
			if (scatterChild.isRotationZ) btnColor = Color.green;
			if (ESGuiTools.Button("",btnColor,size+4,size,false,GetIcon(3))){
				scatterChild.isRotationZ = !scatterChild.isRotationZ;
			}
			btnColor = Color.white;
			
			// Options
			if (scatterChild.showOption) btnColor = Color.green;
			if (ESGuiTools.Button("",btnColor,size+4,size,false,GetIcon(4))){
				scatterChild.showOption = !scatterChild.showOption;
			}
		}
		
		// Delete
		if (!isGlobal){
			if (ESGuiTools.Button("",Color.white,size+4,size,false,GetIcon(5))){
				scatterChild.isWait2delete = !scatterChild.isWait2delete;
			}
			
			if (scatterChild.isWait2delete){
				if (ESGuiTools.Button("Delete",Color.red,50,size)){
					scatteredObjects.Remove(scatterChild);
					isDelete = true;
				}
			}
		}
		EditorGUILayout.EndHorizontal();
		#endregion
		
		#region options
		if (!isDelete && scatterChild.showOption){
			
			ESGuiTools.BeginGroup();
			
			scatterChild.offset = EditorGUILayout.FloatField("Vertical offset",scatterChild.offset);
			
			scatterChild.isScaleOption = ESGuiTools.SimpleFoldOut("Scale options",scatterChild.isScaleOption);
			if (scatterChild.isScaleOption){
				EditorGUI.indentLevel++;
				EditorGUILayout.BeginHorizontal();
				scatterChild.uniformScale = EditorGUILayout.ToggleLeft("Uniform",scatterChild.uniformScale,GUILayout.Width(80) );
				scatterChild.isScaleParticle = EditorGUILayout.ToggleLeft("Particle",scatterChild.isScaleParticle,GUILayout.Width(80));
				scatterChild.isScaleLight = EditorGUILayout.ToggleLeft("Light",scatterChild.isScaleLight,GUILayout.Width(100));
				EditorGUILayout.EndHorizontal();
				if (scatterChild.uniformScale){
					EditorGUILayout.BeginHorizontal();
					scatterChild.xMinScale = EditorGUILayout.FloatField("",scatterChild.xMinScale, GUILayout.Width(50));
					EditorGUILayout.MinMaxSlider(ref scatterChild.xMinScale, ref scatterChild.xmaxScale,0,10);
					scatterChild.xmaxScale = EditorGUILayout.FloatField("",scatterChild.xmaxScale, GUILayout.Width(50));
					EditorGUILayout.EndHorizontal();
					
				}
				else{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("X",GUILayout.Width(30));
					scatterChild.xMinScale = EditorGUILayout.FloatField("",scatterChild.xMinScale, GUILayout.Width(50));
					EditorGUILayout.MinMaxSlider(ref scatterChild.xMinScale, ref scatterChild.xmaxScale,0,10);
					scatterChild.xmaxScale = EditorGUILayout.FloatField("",scatterChild.xmaxScale, GUILayout.Width(50));
					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Y",GUILayout.Width(30));
					scatterChild.yMinScale = EditorGUILayout.FloatField("",scatterChild.yMinScale, GUILayout.Width(50));
					EditorGUILayout.MinMaxSlider(ref scatterChild.yMinScale, ref scatterChild.yMaxScale,0,10);
					scatterChild.yMaxScale = EditorGUILayout.FloatField("",scatterChild.yMaxScale, GUILayout.Width(50));
					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Z",GUILayout.Width(30));
					scatterChild.zMinScale = EditorGUILayout.FloatField("",scatterChild.zMinScale, GUILayout.Width(50));
					EditorGUILayout.MinMaxSlider(ref scatterChild.zMinScale, ref scatterChild.zMaxScale,0,10);
					scatterChild.zMaxScale = EditorGUILayout.FloatField("",scatterChild.zMaxScale, GUILayout.Width(50));
					EditorGUILayout.EndHorizontal();
				}
				EditorGUI.indentLevel--;
			}
			
			scatterChild.isRotationOption = ESGuiTools.SimpleFoldOut("Rotation options",scatterChild.isRotationOption);
			if (scatterChild.isRotationOption){
				EditorGUI.indentLevel++;
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("X",GUILayout.Width(30));
				scatterChild.xMinRot = EditorGUILayout.FloatField("",scatterChild.xMinRot, GUILayout.Width(50));
				EditorGUILayout.MinMaxSlider(ref scatterChild.xMinRot, ref scatterChild.xMaxRot,-180,180);
				scatterChild.xMaxRot = EditorGUILayout.FloatField("",scatterChild.xMaxRot, GUILayout.Width(50));
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Y",GUILayout.Width(30));
				scatterChild.yMinRot = EditorGUILayout.FloatField("",scatterChild.yMinRot, GUILayout.Width(50));
				EditorGUILayout.MinMaxSlider(ref scatterChild.yMinRot, ref scatterChild.yMaxRot,-180,180);
				scatterChild.yMaxRot = EditorGUILayout.FloatField("",scatterChild.yMaxRot, GUILayout.Width(50));
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("X",GUILayout.Width(30));
				scatterChild.zMinRot = EditorGUILayout.FloatField("",scatterChild.zMinRot, GUILayout.Width(50));
				EditorGUILayout.MinMaxSlider(ref scatterChild.zMinRot, ref scatterChild.zMaxRot,-180,180);
				scatterChild.zMaxRot = EditorGUILayout.FloatField("",scatterChild.zMaxRot, GUILayout.Width(50));
				EditorGUILayout.EndHorizontal();
				EditorGUI.indentLevel--;
			}
			
			ESGuiTools.EndGroup();
		}
		#endregion
	}
	#endregion

	#region Selection Inspector
	void DrawSelectionInspector(bool newSelection){

		if (newSelection) Tools.current = Tool.None;

		// Remove deleted transform
		List<Transform> tr = dynamicSelections.FindAll( delegate( Transform t) {
			return t==null;
		}
		);

		foreach(Transform t in tr){
			dynamicSelections.Remove(t);
		}


		// Inspector
		DrawSelectionSelection( newSelection);

		// Tools
		DrawSelectionTools();
	}


	void DrawSelectionSelection(bool newSelection){

		// Selection info
		ESGuiTools.SimpleTitle("Selection");
		ESGuiTools.BeginGroup();
		EditorGUILayout.BeginHorizontal();

		// From layer
		if (ESGuiTools.Button("From root",Color.white,90,18)){
			dynamicSelections = GetSelectionFromLayerToList();
			UpdateHierarchyFromSelection();
		}

		if (ESGuiTools.Button("Clear",Color.white,90,18)){
			dynamicSelections.Clear();
			UpdateHierarchyFromSelection();
		}

		isSelectionShowOrder = EditorGUILayout.ToggleLeft("Show order",isSelectionShowOrder, GUILayout.Width(110));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.LabelField( dynamicSelections.Count + " objects selected");
		ESGuiTools.EndGroup();

	}

	void DrawSelectionTools(){

		#region Align

		ESGuiTools.SimpleTitle("Align");
		ESGuiTools.BeginGroup();

		// Ground
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Ground")){
			Ground( dynamicSelections.ToArray());
		}
		so.Update();
		SerializedProperty layer = so.FindProperty("brush.pickableLayer");
		EditorGUILayout.PropertyField(layer,new  GUIContent(""), true, GUILayout.Width(100));
		so.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();


		// Aligh to surface
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Align to surface")){

			foreach(Transform tr in dynamicSelections){
				Undo.RecordObject  (tr.gameObject.transform, "Easy Scatter : Aligned to surface");
				AlignToSurface(tr,Vector3.up,isSelectionKeepRot);
			}
		}
		isSelectionKeepRot = EditorGUILayout.ToggleLeft("Keep rotation",isSelectionKeepRot, GUILayout.Width(100));
		EditorGUILayout.EndHorizontal();


		// ALign previous
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Align to previous")){
			//AlignToPrevious( dynamicSelections.ToArray(), selectionAlignVector,brush.loop,Vector3.up,isSelectionSurface );
		}
		selectionAlignVector = (ScatterBrush.AlignVector)EditorGUILayout.EnumPopup("",selectionAlignVector ,GUILayout.Width(50));
		brush.loop = EditorGUILayout.ToggleLeft("loop",brush.loop, GUILayout.Width(42));
		isSelectionSurface = EditorGUILayout.ToggleLeft("2 Surface",isSelectionSurface,GUILayout.Width(80));
		EditorGUILayout.EndHorizontal();
		ESGuiTools.EndGroup();
	
		#endregion

		#region Transform
		ESGuiTools.SimpleTitle("Transform");

		#region Offset
		ESGuiTools.BeginGroup();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Apply offset")){
			SelectionOffset( dynamicSelections.ToArray(), scatteredModel.offset);
		}
		scatteredModel.offset = EditorGUILayout.FloatField("",scatteredModel.offset,GUILayout.Width(100));
		EditorGUILayout.EndHorizontal();
	
		#endregion

		#region Scale
		EditorGUILayout.Space();
		ESGuiTools.BeginGroup(5);
		scatteredModel.uniformScale = EditorGUILayout.ToggleLeft("Uniform scale",scatteredModel.uniformScale );
		scatteredModel.isScaleParticle = EditorGUILayout.ToggleLeft("Particle",scatteredModel.isScaleParticle,GUILayout.Width(80));
		scatteredModel.isScaleLight = EditorGUILayout.ToggleLeft("Light",scatteredModel.isScaleLight,GUILayout.Width(100));

		if (scatteredModel.uniformScale){
			EditorGUILayout.BeginHorizontal();
			scatteredModel.xMinScale = EditorGUILayout.FloatField("",scatteredModel.xMinScale, GUILayout.Width(40));
			EditorGUILayout.MinMaxSlider(ref scatteredModel.xMinScale, ref scatteredModel.xmaxScale,0,10);
			scatteredModel.xmaxScale = EditorGUILayout.FloatField("",scatteredModel.xmaxScale, GUILayout.Width(40));
			EditorGUILayout.EndHorizontal();
			
		}
		else{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("X",GUILayout.Width(30));
			scatteredModel.xMinScale = EditorGUILayout.FloatField("",scatteredModel.xMinScale, GUILayout.Width(50));
			EditorGUILayout.MinMaxSlider(ref scatteredModel.xMinScale, ref scatteredModel.xmaxScale,0,10);
			scatteredModel.xmaxScale = EditorGUILayout.FloatField("",scatteredModel.xmaxScale, GUILayout.Width(50));
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Y",GUILayout.Width(30));
			scatteredModel.yMinScale = EditorGUILayout.FloatField("",scatteredModel.yMinScale, GUILayout.Width(50));
			EditorGUILayout.MinMaxSlider(ref scatteredModel.yMinScale, ref scatteredModel.yMaxScale,0,10);
			scatteredModel.yMaxScale = EditorGUILayout.FloatField("",scatteredModel.yMaxScale, GUILayout.Width(50));
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Z",GUILayout.Width(30));
			scatteredModel.zMinScale = EditorGUILayout.FloatField("",scatteredModel.zMinScale, GUILayout.Width(50));
			EditorGUILayout.MinMaxSlider(ref scatteredModel.zMinScale, ref scatteredModel.zMaxScale,0,10);
			scatteredModel.zMaxScale = EditorGUILayout.FloatField("",scatteredModel.zMaxScale, GUILayout.Width(50));
			EditorGUILayout.EndHorizontal();
		}
		if (GUILayout.Button("Apply scale")){
			SelectionScale( dynamicSelections.ToArray(), scatteredModel);
		}
		ESGuiTools.EndGroup();
		#endregion

		#region rotation
		EditorGUILayout.Space();
		ESGuiTools.BeginGroup(5);
		isRelativeRotation = EditorGUILayout.Toggle("Relative rotation",isRelativeRotation);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("X",GUILayout.Width(30));
		scatteredModel.xMinRot = EditorGUILayout.FloatField("",scatteredModel.xMinRot, GUILayout.Width(50));
		EditorGUILayout.MinMaxSlider(ref scatteredModel.xMinRot, ref scatteredModel.xMaxRot,-180,180);
		scatteredModel.xMaxRot = EditorGUILayout.FloatField("",scatteredModel.xMaxRot, GUILayout.Width(50));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Y",GUILayout.Width(30));
		scatteredModel.yMinRot = EditorGUILayout.FloatField("",scatteredModel.yMinRot, GUILayout.Width(50));
		EditorGUILayout.MinMaxSlider(ref scatteredModel.yMinRot, ref scatteredModel.yMaxRot,-180,180);
		scatteredModel.yMaxRot = EditorGUILayout.FloatField("",scatteredModel.yMaxRot, GUILayout.Width(50));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("X",GUILayout.Width(30));
		scatteredModel.zMinRot = EditorGUILayout.FloatField("",scatteredModel.zMinRot, GUILayout.Width(50));
		EditorGUILayout.MinMaxSlider(ref scatteredModel.zMinRot, ref scatteredModel.zMaxRot,-180,180);
		scatteredModel.zMaxRot = EditorGUILayout.FloatField("",scatteredModel.zMaxRot, GUILayout.Width(50));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("X rotation")){
			SelectionRotate( dynamicSelections.ToArray(),scatteredModel,0,isRelativeRotation);
		}
		if (GUILayout.Button("Y rotation")){
			SelectionRotate( dynamicSelections.ToArray(),scatteredModel,1,isRelativeRotation);
		}
		if (GUILayout.Button("Z rotation")){
			SelectionRotate( dynamicSelections.ToArray(),scatteredModel,2,isRelativeRotation);
		}
		EditorGUILayout.EndHorizontal();
		ESGuiTools.EndGroup();
		ESGuiTools.EndGroup();
		#endregion

		#endregion


		#region Reset
		ESGuiTools.SimpleTitle("Reset");
		ESGuiTools.BeginGroup();
		// Transform
		if (GUILayout.Button("Reset Scale")){
			ResetTransformScale(dynamicSelections.ToArray(),scatteredModel );
			
		}
		if (GUILayout.Button("Reset Rotation")){
			ResetTransformRotation(dynamicSelections.ToArray());
		}
		if (GUILayout.Button("Reset")){
			ResetTransform(dynamicSelections.ToArray(),scatteredModel);
		}
		ESGuiTools.EndGroup();
		#endregion

	}
	#endregion

	#region Infos inspector
	void DrawInfosInspector(){
		EditorGUILayout.LabelField("Release date : " + date );
		EditorGUILayout.LabelField("Release version : " + Version);
		
		
		EditorGUILayout.Space();
		
		if (GUILayout.Button("Support")){
			Application.OpenURL("http://www.thehedgehogteam.com/Forum/");
		}
		
		if (GUILayout.Button("Official topic")){
			Application.OpenURL("http://forum.unity3d.com/threads/easy-scatter-prefab-brush-tool-paint-your-level.372382/");
		}

		ESGuiTools.SimpleTitle("Shortcuts");
		EditorGUILayout.LabelField("A\t\t: Select paint object tool");
		EditorGUILayout.LabelField("Q\t\t: Unselect paint object tool");
		EditorGUILayout.LabelField("Hold down ctrl\t: For prefab multi-selection");
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Num Pad 0\t\t: Reset brush");
		EditorGUILayout.LabelField("Page up / Page donw\t: Brush size");
		EditorGUILayout.LabelField("Num Pad +&-\t: Brush amount");
		EditorGUILayout.LabelField("Num Pad 1 to 5\t: Brush amount 1 to 5");
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Shift + wheel\t: Brush size");
		EditorGUILayout.LabelField("Ctrl + wheel\t: Brush amount");
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Shift + previous opt\t:");
		EditorGUILayout.LabelField("Restricted alignment to objects being created");
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Shift + right click \t: Delete select prefab");
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Hold donw click + Crtl \t: Scale & Rotate");

	}
	#endregion

	#region Preference inspector
	void DrawPreferenceInspector(){
		ESGuiTools.SimpleTitle("Prefab preferences");
		preference.keepPrefabLink = EditorGUILayout.Toggle("Preserve prefab link",preference.keepPrefabLink);
		preference.showPrefabPreview = EditorGUILayout.Toggle("Show prefab preview",preference.showPrefabPreview);
		EditorGUILayout.Space();

		ESGuiTools.SimpleTitle("Brush preferences");
		preference.showBrushSize = EditorGUILayout.Toggle("Show brush size",preference.showBrushSize);
		preference.brushColor = EditorGUILayout.ColorField("Color",preference.brushColor);
		preference.brushDetail = EditorGUILayout.IntSlider("Brush detail",preference.brushDetail,12,32);
		preference.minBrushDrawSize = EditorGUILayout.Slider("Min brush draw size",preference.minBrushDrawSize,0,1f);
		EditorGUILayout.Space();

		preference.showCentralDot = EditorGUILayout.Toggle("Show central dot", preference.showCentralDot);
		preference.dotSize = EditorGUILayout.Slider( "Dot size",preference.dotSize,0f,2f);
		EditorGUILayout.Space();
		preference.showNormal = EditorGUILayout.Toggle("Show Y orientation",preference.showNormal);
		preference.normalColor = EditorGUILayout.ColorField("Y orientation color",preference.normalColor);

		EditorGUILayout.Space();

		ESGuiTools.SimpleTitle("Brush setting");
		preference.maxSize = EditorGUILayout.IntField("Max size value",preference.maxSize);
		preference.maxAmount = EditorGUILayout.IntField("Max amount value",preference.maxAmount);
		preference.maxFlux = EditorGUILayout.FloatField("Max flux value",preference.maxFlux);


		if (GUI.changed){
			preference.SavePreference();
		}

	}
	#endregion

	#region Tools
	void DoPaint(Vector3 position, Vector3 normal){
		
		// Get selected gameobject
		List<ScatteredObject> objs = scatteredObjects.FindAll(
			delegate (ScatteredObject s){
			return s.enable == true;
		}
		);

		// Compute normal rotation
		Quaternion rot = Quaternion.FromToRotation( Vector3.up,normal);
		
		if (objs.Count>0){

			Camera camEditor = UnityEditor.SceneView.lastActiveSceneView.camera;
			for (int o=0;o<brush.amount;o++){

				Vector3 pos = position;

				// Position relative to brush radius
				float angle = Random.Range(-Mathf.PI*2,Mathf.PI *2);
				pos = position +   new Vector3( Mathf.Cos(angle),0,Mathf.Sin(angle)) * Random.Range(-brush.size/2f, brush.size/2f);

				// Compute position relative to normal
				Vector3 dir = pos - position;
				dir = rot * dir;
				pos = dir + position;
		

				// Cast against the scene
				RaycastHit hit;

				if (Physics.Raycast(pos +  normal.normalized,-normal,out hit,Mathf.Infinity,brush.pickableLayer )){
					// Create a layer, if no one exists
					if (!rootPivot){
						CreateRootPivot();
					}

					// Compute slope
					float slopeAngle = Mathf.Acos(Mathf.Clamp(hit.normal.normalized.y, -1f, 1f)) * Mathf.Rad2Deg;

					if  (slopeAngle>= brush.minSlope && slopeAngle<brush.maxSlope){
						pos=hit.point;

						// Create the new object
						int rndObj = Random.Range(0,objs.Count);
						pos.y = hit.point.y + objs[rndObj].offset;

						GameObject obj = null;
						if ( !objs[rndObj].isPrefab || !preference.keepPrefabLink){
							obj =  (GameObject)Instantiate(objs[rndObj].prefab, pos, Quaternion.identity);
						}
						else{
							obj = PrefabUtility.InstantiatePrefab(objs[rndObj].prefab ) as GameObject;
							obj.transform.position = pos;
						}


						Undo.RegisterCreatedObjectUndo (obj, "Easy Scatter : Painted object");

						// Apply random
						if (prefabGlobalSetting.enable){
							ScatteredObject.ApplyRS( obj.transform, prefabGlobalSetting);
						}
						else{
							ScatteredObject.ApplyRS( obj.transform, objs[rndObj]);
						}

						// Advanced option
						if (( !objs[rndObj].isPrefab || !preference.keepPrefabLink) || (preference.keepPrefabLink && objs[rndObj].isPrefab && brush.overRide)){
							obj.layer = brush.layer;
							obj.tag = brush.tag;
						
							StaticEditorFlags flag = brush.lightmapStatic?StaticEditorFlags.ContributeGI:0;
							flag |= brush.batchingStatic?StaticEditorFlags.BatchingStatic:flag;
							flag |= brush.occludeeStatic?StaticEditorFlags.OccludeeStatic:flag;
						
							GameObjectUtility.SetStaticEditorFlags( obj,flag);
						}
						
						// Parent
						obj.transform.parent = rootPivot;

						// Align to view
						if (brush.align2View){
							obj.transform.eulerAngles = new Vector3( obj.transform.eulerAngles.x,camEditor.transform.eulerAngles.y,obj.transform.eulerAngles.z);
						}

						// Align to previous
						if (brush.align2Previous){
							obj.transform.rotation =  Quaternion.FromToRotation(Vector3.up, hit.normal);
							AlignToPrevious(GetSelectionFromLayerToArray(previousStartIndex), brush.alignVector, brush.loop,brush.align2Surface );
						}

						// Align to surface
						if (brush.align2Surface && !brush.align2Previous){
							obj.transform.rotation =  Quaternion.FromToRotation(Vector3.up, hit.normal) * obj.transform.rotation ;
						}
		
						LastTransform = obj.transform;
						Selection.activeTransform = obj.transform;
					}
				}
			}
		}

	}


	void DoDelete(Vector3 position){


		List<GameObject> objs = scatteredObjects.Where(x=>x.enable).Select( x=>x.prefab).ToList<GameObject>();

		if (rootPivot && rootPivot.childCount>0){
			for (int i=0;i<rootPivot.childCount;i++){
				for( int j=0;j<objs.Count;j++){
					if (rootPivot.GetChild(i).name.Contains( objs[j].name)){

						float size = brush.size==0?20:brush.size;
						if (Vector3.Distance( position, rootPivot.GetChild(i).transform.position)<= size/2){
							DestroyImmediate( rootPivot.GetChild(i).gameObject);	
							i--;
							break;
						}
					}
				}
			}
		}

	}

	void AlignToSurface(Transform tr, Vector3 normal, bool addCurrentRotation=true){

		Collider[] cols = tr.GetComponentsInChildren<Collider>();
		for (int i=0;i<cols.Length;i++){
			cols[i].enabled = false;
		}

		RaycastHit hit;
		if (Physics.Raycast(tr.position + normal * 10 ,-normal,out hit,Mathf.Infinity,brush.pickableLayer)) {
			if (addCurrentRotation){
				tr.rotation =  Quaternion.FromToRotation(Vector3.up, hit.normal) * tr.rotation;
			}
			else{
				tr.rotation =  Quaternion.FromToRotation(Vector3.up, hit.normal);
			}
		}

		for (int i=0;i<cols.Length;i++){
			cols[i].enabled = true;
		}
	}

	void AlignToPrevious(Transform[] transforms,ScatterBrush.AlignVector vector, bool loop,bool alignSurface=false){
		
		Vector3 linear = Vector3.zero;
		
		Vector3[] positions = new Vector3[transforms.Length];
		for (int i=0; i<transforms.Length;i++){
			positions[i] = new Vector3(transforms[i].position.x,0, transforms[i].position.z);
		}

		int start =0;

		if (transforms.Length>1){
			for (int i=start; i<transforms.Length;i++){
				
				switch (i){
				case 0:
					if (loop){
						linear = ( positions[i+1] - positions[transforms.Length-1]);
					}
					else{
						linear = ( positions[i+1] - positions[i]);
					}
					break;
				default:
					if (i == transforms.Length-1){
						
						if (loop){
							linear = (  positions[0] - positions[i-1]);
						}
						else{
							linear = (positions[i-1]- positions[i] );
						}
					}
					else{

						linear = ( positions[i+1] - positions[i-1]);
						
					}
					break;
				}
				
				Undo.RecordObject  (transforms[i].gameObject.transform, "Easy Scatter : Aligned previous");
				
				Vector3 align = Vector3.zero;
				switch (vector){
				case ScatterBrush.AlignVector.Left:
					align = Vector3.left;
					break;
				case ScatterBrush.AlignVector.right:
					align = Vector3.right;
					break;
				case ScatterBrush.AlignVector.down:
					align = Vector3.down;
					break;
				case ScatterBrush.AlignVector.Up:
					align = Vector3.up;
					break;
				case ScatterBrush.AlignVector.Back:
					align = Vector3.back;
					break;
				case ScatterBrush.AlignVector.Front:
					align = Vector3.forward;
					break;
				}


				//transforms[i].eulerAngles = new Vector3( transforms[i].eulerAngles.x,Quaternion.FromToRotation(align, linear).eulerAngles.y,transforms[i].eulerAngles.z);
				transforms[i].rotation =   Quaternion.FromToRotation(align, linear)  ;

				if (alignSurface){
					AlignToSurface( transforms[i],transforms[i].TransformVector(Vector3.up));
				}
			}	
			
		}
	}

	void Ground(Transform[] transforms){

		foreach(Transform tr in transforms){
			Collider[] cols = tr.GetComponentsInChildren<Collider>();
			for (int i=0;i<cols.Length;i++){
				cols[i].enabled = false;
			}

			RaycastHit hit;
			if (Physics.Raycast(tr.position + Vector3.up * 2000 ,Vector3.down,out hit,Mathf.Infinity,brush.pickableLayer)) {
				Undo.RecordObject(tr.gameObject.transform,"Easy Scatter : ground");
				tr.transform.position = hit.point;
			}
			
			for (int i=0;i<cols.Length;i++){
				cols[i].enabled = true;
			}
		}
	}

	void SelectionOffset(Transform[] transforms, float offset){
		foreach(Transform tr in transforms){
			Undo.RecordObject(tr.gameObject.transform,"Easy Scatter : offset");
			tr.Translate( Vector3.up * offset);
		}
	}

	void SelectionScale(Transform[] transforms, ScatteredObject model){

		foreach(Transform tr in transforms){
			Undo.RecordObject(tr.gameObject.transform,"Easy Scatter : scale");
			if (model.uniformScale){
				float newSize = Random.Range( model.xMinScale, model.xmaxScale);
				tr.localScale = new Vector3(newSize,newSize,newSize);
				ScaleEffect( tr, newSize,scatteredModel.isScaleParticle,scatteredModel.isScaleLight);
			}
			else{
				tr.localScale = new Vector3(Random.Range( model.xMinScale, model.xmaxScale),Random.Range( model.yMinScale, model.yMaxScale),Random.Range( model.zMinScale, model.zMaxScale));
				ScaleEffect( tr, (tr.localScale.x+tr.localScale.y+tr.localScale.z)/3f,scatteredModel.isScaleParticle,scatteredModel.isScaleLight);
			}
		}
	}

	void SelectionRotate( Transform[] transforms,ScatteredObject model, int axis, bool isRelative=false){

		foreach(Transform tr in transforms){
			Undo.RecordObject(tr.gameObject.transform,"Easy Scatter : rotate");
			Vector3 rotation = tr.eulerAngles;

			switch (axis){
			case 0:
				if (!isRelative){
					rotation.x = Random.Range( model.xMinRot,model.xMaxRot);
				}
				else{
					rotation.x += Random.Range( model.xMinRot,model.xMaxRot);
				}
				break;
			case 1:
				if (!isRelative){
					rotation.y = Random.Range( model.yMinRot,model.yMaxRot);
				}
				else{
					rotation.y += Random.Range( model.yMinRot,model.yMaxRot);
				}
				break;
			case 2:
				if (!isRelative){
					rotation.z = Random.Range( model.zMinRot,model.zMaxRot);
				}
				else{
					rotation.z += Random.Range( model.zMinRot,model.zMaxRot);
				}
				break;
			}

			tr.transform.eulerAngles = rotation;
	

		}
	}

	void ResetTransformRotation(Transform[] selections){
		foreach(Transform tr in selections){
			Undo.RecordObject (tr.gameObject.transform, "Easy Scatter : Reset rotation");

			GameObject originPrefab = PrefabUtility.GetCorrespondingObjectFromSource( tr.gameObject) as GameObject;
			if (originPrefab){
				tr.rotation = originPrefab.transform.rotation;
			}
			else{
				tr.rotation = Quaternion.identity;
			}
		}
	}

	void ResetTransformScale(Transform[] selections, ScatteredObject scatteredModel){
		foreach(Transform tr in selections){
			Undo.RecordObject (tr.gameObject.transform, "Easy Scatter : Reset scale");

			GameObject originPrefab = PrefabUtility.GetCorrespondingObjectFromSource( tr.gameObject) as GameObject;

			if (originPrefab){
				tr.localScale = originPrefab.transform.localScale;
				ScaleEffect( tr, 1f/((tr.localScale.x + tr.localScale.y + tr.localScale.z)/3f),scatteredModel.isScaleParticle,scatteredModel.isScaleLight);
				//Debug.Log(originPrefab.transform.localScale);
			}
			else{
				ScaleEffect( tr, 1f/((tr.localScale.x + tr.localScale.y + tr.localScale.z)/3f),scatteredModel.isScaleParticle,scatteredModel.isScaleLight);
				tr.localScale = Vector3.one;
			}

		}
	}

	void ResetTransform(Transform[] selections, ScatteredObject scatteredModel){
		//foreach(Transform tr in selections){
			ResetTransformRotation( selections);
			ResetTransformScale( selections,scatteredModel);
			//Undo.RecordObject (tr.gameObject.transform, "Easy Scatter : Reset transform");
			//tr.rotation = Quaternion.identity;
			//ScaleEffect( tr, 1f/((tr.localScale.x + tr.localScale.y + tr.localScale.z)/3f),scatteredModel.isScaleParticle,scatteredModel.isScaleLight);
			//tr.localScale = Vector3.one;

		//}
	}

	void ScaleEffect(Transform tr, float scaleFactor, bool isScaleParticle, bool isScaleLight){

	
		// Light
		if (isScaleLight){
			Light[] lights = tr.GetComponentsInChildren<Light>();
			foreach(Light light in lights){
				light.range *= scaleFactor;
			}
		}

		// Particles system
		if (isScaleParticle){
			ParticleSystem[] systems = tr.GetComponentsInChildren<ParticleSystem>();
			
			foreach (ParticleSystem system in systems)
			{
                SerializedObject so = new SerializedObject(system);
                so.FindProperty("InitialModule.startSize.scalar").floatValue *= scaleFactor;
                so.FindProperty("InitialModule.startSpeed.scalar").floatValue *= scaleFactor;

                so.FindProperty("EmissionModule.rateOverDistance.scalar").floatValue *= scaleFactor;
                so.FindProperty("InitialModule.gravityModifier.scalar").floatValue *= scaleFactor;

                so.FindProperty("VelocityModule.x.scalar").floatValue *= scaleFactor;
                so.FindProperty("VelocityModule.y.scalar").floatValue *= scaleFactor;
                so.FindProperty("VelocityModule.z.scalar").floatValue *= scaleFactor;
                so.FindProperty("ClampVelocityModule.magnitude.scalar").floatValue *= scaleFactor;
                so.FindProperty("ClampVelocityModule.x.scalar").floatValue *= scaleFactor;
                so.FindProperty("ClampVelocityModule.y.scalar").floatValue *= scaleFactor;
                so.FindProperty("ClampVelocityModule.z.scalar").floatValue *= scaleFactor;
                so.FindProperty("ForceModule.x.scalar").floatValue *= scaleFactor;
                so.FindProperty("ForceModule.y.scalar").floatValue *= scaleFactor;
                so.FindProperty("ForceModule.z.scalar").floatValue *= scaleFactor;
                so.FindProperty("ColorBySpeedModule.range").vector2Value *= scaleFactor;
                so.FindProperty("SizeBySpeedModule.range").vector2Value *= scaleFactor;
                so.FindProperty("RotationBySpeedModule.range").vector2Value *= scaleFactor;

                so.ApplyModifiedProperties();
            }
		}
	}
	#endregion
	
	#region Privates methods
	void ClearRootPivot(){
		
		if (rootPivot){
			for (int i=0;i<rootPivot.childCount;i++){
				DestroyImmediate( rootPivot.GetChild(0).gameObject );
				i--;
			}
		}
	}
	
	void CreateRootPivot(){
		
		string label = "New scattering root";
		GameObject tmpObj = new GameObject(label,typeof(ScatterRoot));

		InitScatteredList(tmpObj);

		tmpObj.transform.position = Vector3.zero;

		rootPivot = tmpObj.transform;

		EditorUtility.SetDirty( tmpObj);
		Selection.activeObject = rootPivot;

	}

	void InitScatteredList(GameObject obj){

		List<ScatteredObject> tmplst = new List<ScatteredObject>();

		foreach(ScatteredObject o in scatteredObjects){
			tmplst.Add( o.Clone() as ScatteredObject);
		}

		obj.GetComponent<ScatterRoot>().scatteredObjects = tmplst;

	}

	
	void DragDrop(){
		
		Event evt = Event.current;
		
		switch (evt.type) {
		case EventType.DragUpdated:
		case EventType.DragPerform:
			
			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			
			if (evt.type == EventType.DragPerform) {
				
				foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences) {
					DoDrop(draggedObject );
				}
			}
			break;
		}
	}
	
	void DoDrop(UnityEngine.Object dragObject){

		if (dragObject.GetType() == typeof(GameObject) || dragObject.GetType() == typeof(Transform)){

			GameObject obj = null;
			if (dragObject.GetType() == typeof(GameObject)){
				obj = (GameObject)dragObject;
			}
			else{
				obj = ((Transform)dragObject).gameObject;
			}
			
			int result = scatteredObjects.FindIndex(
				delegate(ScatteredObject s)
				{
				return  s.prefab == obj ||  (s.prefab.name == obj.name && s.prefab != obj);
			}
			);


			if (result==-1){

				ScatteredObject so = new ScatteredObject();
				so.prefab = obj;
				so.enable = false;
				so.isPrefab = PrefabUtility.GetPrefabType(obj) == PrefabType.Prefab?true:false;

				scatteredObjects.Add( so);
			}
		}
	}
	
	void InitIncons(){
		if (toolbarIcons[0]==null){
			toolbarIcons[0] = (Texture2D)Resources.Load("paint");
		}
		if (toolbarIcons[1]==null){
			toolbarIcons[1] = (Texture2D)Resources.Load("select");
		}
		if (toolbarIcons[2]==null){
			toolbarIcons[2] = (Texture2D)Resources.Load("gear");
		}
		if (toolbarIcons[3]==null){
			toolbarIcons[3] = (Texture2D)Resources.Load("info");
		}

		if (deleteCursor == null){
			deleteCursor  = (Texture2D)Resources.Load("eraser");
		}
	}
	
	Texture2D GetIcon(int index){
		switch (index){
			// Scale
		case 0:
			if (icons[0]==null){
				icons[0] = (Texture2D)Resources.Load("scale");
			}
			return icons[0];
			// rotate X
		case 1:
			if (icons[1]==null){
				icons[1] = (Texture2D)Resources.Load("rotate_x");
			}
			return icons[1];
			// rotate y
		case 2:
			if (icons[2]==null){
				icons[2] = (Texture2D)Resources.Load("rotate_y");
			}
			return icons[2];
			// rotate z
		case 3:
			if (icons[3]==null){
				icons[3] = (Texture2D)Resources.Load("rotate_z");
			}
			return icons[3];
			// Gear
		case 4:
			if (icons[4]==null){
				icons[4] = (Texture2D)Resources.Load("gear");
			}
			return icons[4];
			// Close
		case 5:
			if (icons[5]==null){
				icons[5] = (Texture2D)Resources.Load("close");
			}
			return icons[5];
			// new
		case 6:
			if (icons[6]==null){
				icons[6] = (Texture2D)Resources.Load("new");
			}
			return icons[6];
			// get
		case 7:
			if (icons[7]==null){
				icons[7] = (Texture2D)Resources.Load("select");
			}
			return icons[7];
		}
		
		return null;
	}

	private bool isShift = false;

	void ShortCut(){

		// Unity move tool detection
		if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Q || Event.current.keyCode == KeyCode.W || Event.current.keyCode == KeyCode.R || Event.current.keyCode == KeyCode.E) && toolIndex==0){
			toolIndex = -1;
		}
		
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Q && toolIndex==0){
			toolIndex = -1;
		}

		// Select Paint tool
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.A){
			toolIndex = 0;
		}

		// Init brush
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Keypad0){
			brush.size =0;
			brush.amount=1;
			brush.flux=1;
		}

		// Brush Size
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.PageUp){
			brush.size += 2f;
		}
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.PageDown){
			brush.size -= 2f;
		}

		if ( Event.current.shift && Event.current.type == EventType.ScrollWheel){
			brush.size -= Event.current.delta.y;
			Event.current.Use();
		}


		// Amount
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.KeypadPlus){
			brush.amount++;
		}
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.KeypadMinus){
			brush.amount--;
		}
		

		if ( Event.current.control && Event.current.type == EventType.ScrollWheel)
        {
			brush.amount -= (int)Event.current.delta.y;
			Event.current.Use();
		}


		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Keypad1){
			brush.amount=1;
		}

		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Keypad2){
			brush.amount=2;
		}
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Keypad3){
			brush.amount=3;
		}
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Keypad4){
			brush.amount=4;
		}
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Keypad5){
			brush.amount=5;
		}

		// Auto select new copy
		if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.D && Event.current.control){
			dynamicSelections = Selection.transforms.ToList<Transform>();
		}



		// Shitf key for previous
		if ( Event.current.shift && !isShift ){
			if (rootPivot){
				previousStartIndex = rootPivot.childCount;
			}
			else{
				previousStartIndex=0;
			}
			isShift = true;
		}

		if ( !Event.current.shift && isShift ){
			isShift = false;
		}
	}

	List<Transform> GetSelectionFromLayerToList(){
		
		List<Transform> transforms = new List<Transform>();
		
		if (rootPivot){
			foreach(Transform tr in rootPivot){
				transforms.Add(tr);
			}
		}
		return transforms;
	}
	
	Transform[] GetSelectionFromLayerToArray(int startIndex=0){
		List<Transform> transforms = new List<Transform>();
		if (rootPivot){
			int i=0;
			foreach(Transform tr in rootPivot){
				if (i>=startIndex){
					transforms.Add(tr);
				}
				i++;
			}
		}
		return transforms.ToArray();
	}
	
	List<Transform> GetSelectionFromHierarchyToList(){
		
		List<Transform> transforms = new List<Transform>();
		
		foreach( Transform tr in Selection.transforms){
			if (!tr.GetComponentInChildren<Terrain>()){
				transforms.Add (tr);
			}
		}
		
		return transforms;
		
	}
	
	void UpdateHierarchyFromSelection(){
		Selection.objects = dynamicSelections.Select( x=>x.gameObject).ToArray();
		
	}
	#endregion

}


