using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Reflection;
using System;
using System.Linq;
using Valkyrie.VPaint;

[InitializeOnLoad]
public class VPaint : EditorWindow 
{			
	public static VPaint Instance;
	
	static VPaint ()
	{
		var types =	typeof(Editor).Assembly.GetTypes();
		for(int i = 0; i < types.Length; i++)
		{
			var t = types[i];
			if(t.Name == "ShaderUtil")
			{
				getShaderSourceChannelsMethod = t.GetMethod("GetSourceChannels", BindingFlags.Static | BindingFlags.Public);
				break;
			}
		}
	}
	
	public static void OpenEditor (){ 
		EditorWindow.GetWindow<VPaint>("VPaint");
	}
	public static void OpenEditorAsUtility ()
	{
		EditorWindow.GetWindow<VPaint>();
	}
	
	[MenuItem("GameObject/VPaint/Create VPaint Group")]
	public static void CreatePaintGroup ()
	{
		var objs = new List<MeshRenderer>();
		foreach(var go in Selection.gameObjects)
		{
			objs.AddRange(go.GetComponentsInChildren<MeshRenderer>());
		}
		
		var vpg = new GameObject("New VPaint Group").AddComponent<VPaintGroup>();
		foreach(var obj in objs)
		{
			var vc = obj.GetComponent<VPaintObject>();
			if(!vc)
			{
				var mf = obj.GetComponent<MeshFilter>();
				var mr = obj.GetComponent<MeshRenderer>();
				if(mf && mr)
					vc = obj.gameObject.AddComponent<VPaintObject>();
			}
			vpg.AddColorer(vc);
		}
		
		Selection.activeObject = vpg;
		OpenEditor();
		
		
	}
	
	[MenuItem("GameObject/VPaint/Open Manual")]
	public static void OpenDocumentation ()
	{
		string path = (Application.dataPath)+ "/VPaint/Manual.pdf";
		if(!File.Exists(path))
		{
			Debug.LogError("VPaint manual could not be found at " + path);
		}
		else
		{
			Application.OpenURL (path);
		}
	}
	
	public enum VertexTool
	{
		Paint,
		Erase,
		Smooth,
	}
	public VertexTool tool = VertexTool.Paint;
		
	bool _enabled = true;
	public bool enabled {
		get{
			return _enabled && !EditorApplication.isPlaying;
		}
		set{ _enabled = value; }
	}
	
	bool _lockSelection;
	public bool lockSelection {
		get{ return _lockSelection; }
		set{
			if(_lockSelection && value == false)
			{
				ValidateSelection();
			}
			_lockSelection = value;
		}
	}
	
	public bool overrideTool = false;
	
	public bool dirty = false;
	
	private float deltaTime;
	private float lastTime = 0;
	
	private SceneView lastDrawnSceneView;
	
//	private MethodInfo method_SetSearchFilter;
	
//	Dictionary<MeshCollider, VPaintObject> editColliders;
	
	[NonSerialized] public UnityEngine.Object _layerCacheObject;
	IVPaintable _layerCache;
	public IVPaintable layerCache
	{
		get{
			if(_layerCache == null)
			{
				if(_layerCacheObject)
				{
					layerCache = _layerCacheObject as IVPaintable;
				}
			}
			return _layerCache;
		}
		private set{
//			if(_layerCache != value)
//			{
//				if(_layerCache != null)
//				{
					Cleanup(false);
//				}
//			}
			_layerCache = value;
			if(_layerCache != null)
			{
				_layerCache.GetLayerStack().Sanitize();
//				_layerCache.GetLayerStack().BuildPaintDataDictionary();
				ClearMaterialSwaps();
				_layerCacheObject = _layerCache as UnityEngine.Object;
				
				currentEditingContents = _layerCache.GetVPaintObjects();
				currentEditingContentsMask = new bool[currentEditingContents.Length];
				for(int i = 0; i < currentEditingContents.Length; i++)
				{
					currentEditingContents[i].index = i;
					currentEditingContentsMask[i] = true;
				}
				
				layerStack.Sanitize(new List<IVPaintIdentifier>(currentEditingContents));
				
				baseLayer = _layerCache.GetBaseLayer();
				
//				SetupColliders();
				
				ReloadLayers();
				
				if(vertexColorPreviewEnabled) EnableVertexColorPreview();
				
				PushUndo("Start Painting " + _layerCacheObject.name);
			}
			else
			{
				_layerCacheObject = null;
				currentEditingContents = new VPaintObject[0];
			}
		}
	}
	public VPaintLayerStack layerStack
	{
		get{
			if(layerCache == null) return null;
			return layerCache.GetLayerStack();
		}
	}
	
	public void AddColorers (List<VPaintObject> colorers)
	{
		foreach(var vc in colorers)
		{
			layerCache.AddColorer(vc);
		}
		CleanupInstances();
		layerCache = layerCache;
	}
	public bool ContainsColorer (VPaintObject colorer)
	{
		var cols = layerCache.GetVPaintObjects();
		return cols.Contains(colorer);
	}
	
	static MethodInfo getShaderSourceChannelsMethod;
	public static bool SupportsColors (Shader shader)
	{
		if(!shader.isSupported)
		{
			return true;
		}
		if(getShaderSourceChannelsMethod == null)
		{
			return true;
		}
		var str = getShaderSourceChannelsMethod.Invoke(null, new object[]{shader}) as string;
		
		if(str.Contains("color")) return true;
		return false;
	}
	
	public VPaintLayer lastLoadedLayer;
	
	public VPaintLayer originalBaseLayer = new VPaintLayer();
	public VPaintLayer baseLayer = new VPaintLayer();
	
	private bool modifierHeld = false;
	
	private Vector3 savedMousePosition;
	
	public int _currentPaintLayer {
		get{ return layerStack.currentLayer; }
		set{ layerStack.currentLayer = value; }
	}
	
	public VPaintObject[] currentEditingContents = new VPaintObject[0];
	
	public bool[] currentEditingContentsMask = new bool[0];
	public IEnumerable<VPaintObject> maskedVPaintObjects ()
	{
		var cordoneBounds = VertexEditorControls.GetCordoneBounds();
		for(int i = 0; i < currentEditingContents.Length; i++)
		{
			if(!currentEditingContentsMask[i]) continue;
			var c = currentEditingContents[i];
			if(!c) continue;
			
			if(VertexEditorControls.cordoneEnabled)
			{
				if(!cordoneBounds.Intersects(c.GetBounds())) continue;
			}
			yield return c;
		}
	}
	
	public bool vertexColorPreviewEnabled = false;
	Dictionary<VertexColorPreviewMode, Material> vertexColorPreviewMaterials = new Dictionary<VertexColorPreviewMode, Material>();
	Material GetColorPreviewMaterial (VertexColorPreviewMode mode)
	{
		if(!vertexColorPreviewMaterials.ContainsKey(mode))
		{
			vertexColorPreviewMaterials.Add(mode, new Material(Shader.Find(GetPreviewShaderPath(mode))));
		}
		return vertexColorPreviewMaterials[mode];
	}
	
	string GetPreviewShaderPath (VertexColorPreviewMode mode)
	{
		switch(mode)
		{
			default:
			case VertexColorPreviewMode.RGB:
				return "VPaint/Unlit/VertexColorsRGB";
			case VertexColorPreviewMode.R:
				return "VPaint/Unlit/VertexColorsR";
			case VertexColorPreviewMode.G:
				return "VPaint/Unlit/VertexColorsG";
			case VertexColorPreviewMode.B:
				return "VPaint/Unlit/VertexColorsB";
			case VertexColorPreviewMode.A:
				return "VPaint/Unlit/VertexColorsA";
		}
	}
	
	public void ClearMaterialSwaps ()
	{
		foreach(var vc in currentEditingContents)
		{
			if(!vc) continue;
			vc.ResetMaterial();
		}
	}
	
	public void SwapMaterial (VPaintObject vc, Material m)
	{		
		vc.SetInstanceMaterial(m);
	}
	
	public void SwapAllMaterials (Material m)
	{
		foreach(VPaintObject vc in currentEditingContents)
		{
			if(!vc) continue;
			SwapMaterial(vc, m);
		}
	}
	
	public enum VertexColorPreviewMode
	{
		RGB,
		R,G,B,A,
	}
	VertexColorPreviewMode currentPreviewMode = VertexColorPreviewMode.RGB;
	public void EnableVertexColorPreview ()
	{
		EnableVertexColorPreview(currentPreviewMode);
	}
	public void EnableVertexColorPreview (VertexColorPreviewMode mode)
	{
		ClearMaterialSwaps();
		SwapAllMaterials(GetColorPreviewMaterial(mode));
		currentPreviewMode = mode;
		vertexColorPreviewEnabled = true;
	}
	
	public void DisableVertexColorPreview ()
	{
		ClearMaterialSwaps();
		vertexColorPreviewEnabled = false;
	}
	
	public void ToggleVertexColorPreview ()
	{
		vertexColorPreviewEnabled = !vertexColorPreviewEnabled;
		if(vertexColorPreviewEnabled) EnableVertexColorPreview(VertexColorPreviewMode.RGB);
		else DisableVertexColorPreview();
	}
	
	public void PushUndo (string s)
	{
		Undo.RegisterUndo(_layerCacheObject, s);
		var mb = _layerCacheObject as MonoBehaviour;
		if(mb)
		{
			var instance = mb.GetComponent<VPaintStorageCacheInstance>();
			if(instance)
			{
				instance.isDirty = true;
			}
		}
	}
	
	public void Cleanup (bool cleanupInstances = true)
	{
//		if(editColliders != null)
//		{
//			foreach(var kvp in editColliders) GameObject.DestroyImmediate(kvp.Key.gameObject);
//			editColliders.Clear();
//		}
		foreach(var vc in currentEditingContents)
		{
			if(vc.editorCollider)
			{
				GameObject.DestroyImmediate(vc.editorCollider);
			}
		}
		
		if(cleanupInstances) CleanupInstances();
		foreach(var kvp in vertexColorPreviewMaterials)
			GameObject.DestroyImmediate(kvp.Value);
		vertexColorPreviewMaterials.Clear();
		ClearMaterialSwaps();
	}
	public void CleanupInstances ()
	{
		foreach(var vc in currentEditingContents)
		{
			if(!vc) continue;
			vc.ResetInstances();
		}
	}
	
	private VPaintLayer currentPaintLayer
	{
		get{
			if(layerStack == null) return null;
			if(layerStack.layers.Count == 0)
			{
				layerStack.layers.Add(new VPaintLayer());
			}
			if(_currentPaintLayer < 0 || layerStack.layers.Count <= _currentPaintLayer)
				_currentPaintLayer = Mathf.Clamp(_currentPaintLayer, 0, layerStack.layers.Count-1);
			return layerStack.layers[_currentPaintLayer];
		}
	}
	
	public void OnEnable () 
	{
		Instance = this;
		
//		method_SetSearchFilter = typeof(SceneView).GetMethod("SetSearchFilter", BindingFlags.NonPublic | BindingFlags.Instance);
		
		
		VertexEditorColors.Deserialize();
		
		VertexEditorControls.Load();
		
//		ReloadLayers();
//		PushUndo("Open Vertex Painter");
		
//		SetupColliders ();
		
		objectsGroup.foldout = currentEditingContents.Length < 200;
		
		RegisterCallbacks ();
		
		ValidateSelection();
	}
	
	public void OnDisable () {
		Instance = null;
		
//		if(lastDrawnSceneView){
//			method_SetSearchFilter.Invoke(lastDrawnSceneView, new object[]{ "", 0, true } );
//		}
		
		DisableVertexColorPreview();
//		Cleanup();
		
		DeregisterCallbacks();
	}
	
	void RegisterCallbacks ()
	{
		EditorApplication.update += OnEditorUpdateCallback;
		SceneView.onSceneGUIDelegate += OnSceneGUICallback;
		
		//UndoRedo callback
		var undoRedoPerformedEvent = typeof(EditorApplication).GetField("undoRedoPerformed", BindingFlags.Static | BindingFlags.NonPublic);
		var dg = undoRedoPerformedEvent.GetValue(null) as EditorApplication.CallbackFunction;
		EditorApplication.CallbackFunction callback = OnUndoRedoPerformedCallback;
		undoRedoPerformedEvent.SetValue(null, Delegate.Combine(dg, callback));		
	}
	void DeregisterCallbacks ()
	{
		EditorApplication.update -= OnEditorUpdateCallback;
		SceneView.onSceneGUIDelegate -= OnSceneGUICallback;
		
		//UndoRedoCallback
		var undoRedoPerformedEvent = typeof(EditorApplication).GetField("undoRedoPerformed", BindingFlags.Static | BindingFlags.NonPublic);
		var dg = undoRedoPerformedEvent.GetValue(null) as EditorApplication.CallbackFunction;
		EditorApplication.CallbackFunction callback = OnUndoRedoPerformedCallback;
		undoRedoPerformedEvent.SetValue(null, Delegate.RemoveAll(dg, callback));
	}
	
	void ValidateSelection ()
	{		
		if(!enabled){
			layerCache = null;
			return;
		}
		
		bool reload = true;
		if(_layerCacheObject) {
			if(_layerCacheObject is Component) {
				if(Selection.activeGameObject && GetIVPaintable(Selection.activeGameObject) == _layerCacheObject) {
					reload = false;
				}
			} else {
				if(Selection.activeObject == _layerCacheObject) {
					reload = false;
				}
			}
		}
		if(reload) {
			if(Selection.activeGameObject){
				if(!EditorUtility.IsPersistent(Selection.activeGameObject)){
					layerCache = GetIVPaintable(Selection.activeGameObject);
				}
				else layerCache = null;
			} else {
				layerCache = Selection.activeObject as IVPaintable;
			}
			
			Repaint();
		}
	}
	
	public static IVPaintable GetIVPaintable (GameObject go)
	{
		if(!go) return null;
		var behaviours = go.GetComponents<MonoBehaviour>();
		foreach(var b in behaviours)
		{
			var p = b as IVPaintable;
			if(p != null) return p;
		}
		return null;
	}
	
	void OnUndoRedoPerformedCallback ()
	{
		CleanupInstances();
		ReloadLayers();
//		SetupColliders();
		
		if(vertexColorPreviewEnabled) EnableVertexColorPreview();
		if(layerCache != null)
		{
			if(currentEditingContents.Length != layerCache.GetVPaintObjects().Length)
			{
				layerCache = layerCache;
			}
		}
	}
	
	void OnEditorUpdateCallback ()
	{
		Repaint();
		SceneView.RepaintAll();
	}
	
	void OnSceneGUICallback (SceneView view)
	{
		OnSceneGUI();
	}
	
//	public void SetupColliders ()
//	{
//		try{
//			if(editColliders == null) editColliders = new Dictionary<MeshCollider, VPaintObject>();
//			editColliders.Clear();
//	
//			for(int i = 0; i < currentEditingContents.Length; i++)
//			{
//				var vc = currentEditingContents[i];
//				string str = "";
//				if(vc.editorCollider)
//				{
//					editColliders.Add(vc.editorCollider, vc);
//					str = "("+i+"/"+currentEditingContents.Length+") Registering VPaintObject Collider for " + vc.name;
//				}
//				else
//				{
//					var mf = vc.GetComponent<MeshFilter>();
//					if(!mf) continue;
//					
//					GameObject go = new GameObject("Editor Collider");
//					go.hideFlags = HideFlags.HideInHierarchy;
//					go.transform.parent = vc.transform;
//					go.transform.localPosition = Vector3.zero;
//					go.transform.localRotation = Quaternion.identity;
//					go.transform.localScale = Vector3.one;
//					
//					vc.editorCollider = go.AddComponent<MeshCollider>();
//					vc.editorCollider.sharedMesh = vc.originalMesh ? vc.originalMesh : mf.sharedMesh;
//					
//					editColliders.Add(vc.editorCollider, vc);
//					
//					str = "("+i+"/"+currentEditingContents.Length+") Creating VPaintObject Collider for " + vc.name;
//				}
//				EditorUtility.DisplayProgressBar("Initializing VPaint", str, (float)i/currentEditingContents.Length);
//			}
//		
//		}finally{
//			EditorUtility.ClearProgressBar();
//		} 
//	}
	Bounds GetBounds (VPaintObject vc)
	{
		if(vc.editorCollider) return vc.editorCollider.bounds;
		return vc.renderer.bounds;
	}
	public MeshCollider GetCollider (VPaintObject vc)
	{
		if(!vc.editorCollider)
		{
			var mf = vc.GetComponent<MeshFilter>();
			if(!mf) return null;
			
			GameObject go = new GameObject("Editor Collider");
			go.hideFlags = HideFlags.HideInHierarchy;
			go.transform.parent = vc.transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			
			vc.editorCollider = go.AddComponent<MeshCollider>();
			vc.editorCollider.sharedMesh = vc.originalMesh ? vc.originalMesh : mf.sharedMesh;
		}
		return vc.editorCollider;
	}
	
	public void OnSelectionChange ()
	{
		if(!_lockSelection) ValidateSelection();
	}
	
	bool IsRightClick(Event e)
	{
		return Event.current.type == EventType.MouseDown && Event.current.button == 1;
	}
	
	VPaintInspectorGroup mainGroup = new VPaintInspectorGroup();
	VPaintInspectorGroup colorsGroup = new VPaintInspectorGroup();
	VPaintInspectorGroup controlsGroup = new VPaintInspectorGroup();
	VPaintInspectorGroup layersGroup = new VPaintInspectorGroup();
	VPaintInspectorGroup objectsGroup = new VPaintInspectorGroup();
	VPaintInspectorGroup cordoneGroup = new VPaintInspectorGroup();
	GenericMenu rightClickMenu;
	
	bool HasPro {
		get{ return UnityEditorInternal.InternalEditorUtility.HasPro(); }
	}
	
	Vector2 scrollPosition;
	public void OnGUI () 
	{	
		if(!EditorGUIUtility.isProSkin)
		{
			GUI.backgroundColor = new Color(1,1,1, 0.5f);
//			GUI.contentColor = new Color(0.4f, 0.4f, 0.4f, 1f);
		}
		
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
		
		GUI.enabled = true;
		
		Rect mainRect = EditorGUILayout.BeginVertical();
		
		mainGroup.title = ()=>{
			
			if(layerStack == null)
			{
				GUILayout.Label("VPaint Disabled");
			}
			else
			{
				GUILayout.Label("VPaint Enabled");
				GUILayout.FlexibleSpace();
				bool guiActiveCache = GUI.enabled;
				GUI.enabled = vertexColorPreviewEnabled;
				if(GUILayout.Button("Normal View"))
				{
					DisableVertexColorPreview();
				}
				Action<VertexColorPreviewMode, string> previewButton = (mode, name)=>{
					GUI.enabled = !vertexColorPreviewEnabled || currentPreviewMode != mode;
					if(GUILayout.Button(name))
					{
						EnableVertexColorPreview(mode);
					}
				};
				previewButton(VertexColorPreviewMode.RGB, "RGB");
				previewButton(VertexColorPreviewMode.R, "R");
				previewButton(VertexColorPreviewMode.G, "G");
				previewButton(VertexColorPreviewMode.B, "B");
				previewButton(VertexColorPreviewMode.A, "A");
				GUI.enabled = guiActiveCache;
			}
		};
		mainGroup.method = (r)=>{
			if(layerStack != null)
			{
				GUILayout.Space(5);
				EditorGUILayout.BeginHorizontal();
				
				EditorGUILayout.ObjectField("Target", _layerCacheObject, typeof(UnityEngine.Object), true);
				
				EditorGUILayout.EndHorizontal();
			}
		};
		mainGroup.width = position.width - 40;
		mainGroup.OnGUI();
		if(mainGroup.foldout)
		{
			if(!enabled)
			{
				enabled = true;
				ValidateSelection();
			}
		}
		else
		{
			if(enabled)
			{
				enabled = false;
				ValidateSelection();
			}
		}

		if(layerStack != null)
		{
			if(IsRightClick(Event.current))
			{
				rightClickMenu = new GenericMenu();
			}
			
			VPaintGUIUtility.BeginColumnView(position.width - 54);
			
			colorsGroup.method = ColorsGroupGUI;
			colorsGroup.title = ()=>{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Color Settings");
				GUILayout.FlexibleSpace();
				if(colorsGroup.foldout)
				{
					if(VPaintGUIUtility.FoldoutMenu())
					{
						var menu = new GenericMenu();
						menu.AddItem(new GUIContent("Flood Fill"), false, ()=>{
							FloodFill();
						});
						
						menu.AddItem(new GUIContent("Invert"), false, ()=>{
							InvertColors();
						});
						
						menu.AddItem(new GUIContent("Erase"), false, ()=>{
							EraseAll();
						});
						menu.ShowAsContext();
					}
				}
				EditorGUILayout.EndHorizontal();
			};
			colorsGroup.width = position.width-40;
			colorsGroup.OnGUI();
			
			controlsGroup.method = ControlsGroupGUI;
			controlsGroup.title = ()=>{GUILayout.Label("Tool Settings");};
			controlsGroup.width = position.width-40;
			controlsGroup.OnGUI();
			
			cordoneGroup.method = CordoneGroupGUI;
			cordoneGroup.title = ()=>{
				GUILayout.Label("Isolated Region");
				GUILayout.FlexibleSpace();
				if(VPaintGUIUtility.FoldoutMenu())
				{
					GenericMenu menu = new GenericMenu();
					menu.AddItem(new GUIContent("Reset"), false, ()=>{
						VertexEditorControls.cordoneEnabled = false;
						VertexEditorControls.cordonePosition = Vector3.zero;
						VertexEditorControls.cordoneSize = Vector3.one;
					});
					menu.AddItem(new GUIContent("Focus"), false, ()=>{
						if(!SceneView.lastActiveSceneView) return;
						VertexEditorControls.cordonePosition = SceneView.lastActiveSceneView.pivot;
					});
					menu.ShowAsContext();
				}
			};
			cordoneGroup.width = position.width-40;
			cordoneGroup.OnGUI();
			
			layersGroup.method = LayersGroupGUI;
			layersGroup.title = ()=>{ GUILayout.Label("Layer Hierarchy"); };
			layersGroup.width = position.width-40;
			layersGroup.OnGUI();
			
			objectsGroup.method = ObjectsGroupGUI;
			objectsGroup.title = ()=>{GUILayout.Label("Object Targets");};	
			objectsGroup.width = position.width-40;
			objectsGroup.OnGUI();
			
			if(IsRightClick(Event.current) && mainRect.Contains(Event.current.mousePosition))
			{
				rightClickMenu.ShowAsContext();
				Event.current.Use();
			}
			
			if(GUI.changed){
				EditorUtility.SetDirty(_layerCacheObject);
				dirty = true;
			}
			
			SceneViewKeyHandler();
		}
		
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.EndScrollView();
	}
	
	void AdjustmentsMenu (GenericMenu menu, string prefix = "Adjustments/")
	{
		var adjustments = new Dictionary<VPaintActionType, string>();
		var types = Enum.GetValues(typeof(VPaintActionType));
		foreach(var t in types) adjustments.Add((VPaintActionType)t, Enum.GetName(typeof(VPaintActionType), t));
		foreach(var kvp in adjustments)
		{
			var t = kvp.Key;
			var s = kvp.Value;
			menu.AddItem(new GUIContent(prefix + kvp.Value), false, ()=>{
				var window = EditorWindow.GetWindow<VPaintLayerAdjustment>(true);
				window.type = t;
				window.title = s + " Adjustment";
			});
		}
		menu.AddItem(new GUIContent(prefix + "Blending"), false, ()=>{
			EditorWindow.GetWindow<VPaintBlendWindow>(true, "Vertex Painter Blending");
		});
	}
	
	void LayersMenu (GenericMenu menu, string prefix = "Layers/")
	{
		menu.AddItem(new GUIContent(prefix + "New Layer (shift+n)"), false, NewLayer);
		menu.AddItem(new GUIContent(prefix + "Create Merged (shift+m)"), false, CreateMergedLayer);
		menu.AddItem(new GUIContent(prefix + "Collapse All (ctrl+shift+e)"), false, MergeAllLayers);
	}
	
	void ImportMenu (GenericMenu menu, string prefix = "Import/")
	{
		menu.AddItem(new GUIContent(prefix + "Base Layer"), false, ImportBaseLayer);
		menu.AddItem(new GUIContent(prefix + "Lightmaps"), false, ImportLightmaps);
		menu.AddItem(new GUIContent(prefix + "Texture"), false, ImportTexture);
		menu.AddItem(new GUIContent(prefix + "VPaint Group"), false, ImportLayerCache);	
	}
	
	void LayerMenu (GenericMenu menu, int layerIndex, string prefix = "")
	{
		if(layerStack.layers.Count == 1)
			menu.AddDisabledItem(new GUIContent("Remove Layer (shift+r)"));
		else menu.AddItem(new GUIContent("Remove Layer (shift+r)"), false, RemoveLayer, layerIndex);
		
		if(layerIndex == 0)
			menu.AddDisabledItem(new GUIContent("Merge Layer Down (shift+e)"));
		else menu.AddItem(new GUIContent("Merge Layer Down (shift+e)"), false, MergeLayerDown, layerIndex);
		
		menu.AddItem(new GUIContent("Duplicate Layer (shift+d)"), false, DuplicateLayer, layerIndex);
		
		menu.AddItem(new GUIContent("Move Layer Up (shift+up)"), false, MoveLayerUp, layerIndex);
		
		menu.AddItem(new GUIContent("Move Layer Down (shift+down)"), false, MoveLayerDown, layerIndex);
		
		menu.AddItem(new GUIContent("Convert To Gamma Space"), false, ConvertToGamma, layerIndex);
		menu.AddItem(new GUIContent("Convert To Linear Space"), false, ConvertToLinear, layerIndex);
		
		menu.AddItem(new GUIContent("Inspect Transparency"), false, ()=>{
			var transLayer = layerStack.layers[layerIndex].Clone();
			foreach(var pd in transLayer.paintData)
			{
				for(int t = 0; t < pd.colors.Length; t++)
				{
					pd.colors[t] = Color.white * pd.transparency[t];
				}
			}
			LoadLayer(transLayer);
		});
	}
	
	void LayersGroupGUI (Rect rect)
	{
			
		VPaintGUIUtility.DrawColumnRow(24, 
		(r)=>{
			GUILayout.Label("Adjustments");
			GUILayout.FlexibleSpace();
			if(VPaintGUIUtility.FoldoutMenu())
			{
				GenericMenu menu = new GenericMenu();
				AdjustmentsMenu(menu, "");
				menu.ShowAsContext();
			}
		},
		(r)=>{
			GUILayout.Label("Import");
			GUILayout.FlexibleSpace();
			if(VPaintGUIUtility.FoldoutMenu())
			{
				GenericMenu menu = new GenericMenu();
				ImportMenu(menu, "");
				menu.ShowAsContext();
			}
		},
		(r)=>{
			GUILayout.Label("Layers");
			GUILayout.FlexibleSpace();
			if(VPaintGUIUtility.FoldoutMenu())
			{
				GenericMenu menu = new GenericMenu();
				LayersMenu(menu, "");
				menu.ShowAsContext();
			}
		});
		
		for(int i = layerStack.layers.Count-1; 0 <= i; i--)
		{
			VPaintLayer layer = layerStack.layers[i];
			
			if(_currentPaintLayer == i)
			{
				GUILayout.Space(4);
				VPaintGUIUtility.columnViewBoxCount = 2;
			}
			
			Rect mainLayerRect = EditorGUILayout.BeginVertical();
			VPaintGUIUtility.DrawColumnRow(32, ()=>{
				
				bool selected = _currentPaintLayer == i;
				
				GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
				if(selected){
					var col = Color.green * 0.8f;
					col.a = 1f;
					labelStyle.normal.textColor = col;
				}
				
				EditorGUILayout.BeginHorizontal();
				
				bool en = EditorGUILayout.Toggle(layer.enabled, GUILayout.Width(16));
				if(en != layer.enabled)
				{
					layer.enabled = en;
					ReloadLayers();
				}
				
				GUILayout.Label(layer.name, labelStyle);
				
				if(selected)
				{
					GUILayout.FlexibleSpace();
					
					if(VPaintGUIUtility.FoldoutMenu())
					{
						GenericMenu menu = new GenericMenu();
						LayerMenu(menu, i, "");
						menu.ShowAsContext();
					}
				}
				
				EditorGUILayout.EndHorizontal();
			});
			
			if(_currentPaintLayer == i)
			{
				VPaintGUIUtility.DrawColumnRow(22, 
				()=>{
					string s = EditorGUILayout.TextField("Name: " , layer.name);
					if(s != layer.name)
					{
						PushUndo("Change Layer Name");
						layer.name = s;
					}
				});
				
//				layer.tag = EditorGUILayout.Popup("Tag", layer.tag, VPaintLayerTags.instance.tags.ToArray());
				
				VPaintGUIUtility.DrawColumnRow(22, 
				()=>{
					VPaintBlendMode blendMode = (VPaintBlendMode)EditorGUILayout.EnumPopup("Blend Mode", layer.blendMode);
					if(blendMode != layer.blendMode)
					{
						PushUndo("Change Layer Blend Mode");
						layer.blendMode = blendMode;
						ReloadLayers();
					}
				});
				
				VPaintGUIUtility.DrawColumnRow(22, 
				()=>{
					float op = EditorGUILayout.Slider("Opacity", layer.opacity, 0f, 1f);
					if(op != layer.opacity)
					{
						PushUndo("Modify Layer Opacity");
						layer.opacity = op;
						ReloadLayers();
					}
				});
				
				VPaintGUIUtility.DrawColumnRow(22, 
				()=>{
					GUILayout.Label("Channel Mask:");
					GUILayout.FlexibleSpace();
					GUILayout.Label("R:");
					bool maskR = GUILayout.Toggle(layer.maskR, GUIContent.none);
					if(maskR != layer.maskR)
					{
						PushUndo("Toggle Red Channel");
						layer.maskR = maskR;
						ReloadLayers();
					}
					
					GUILayout.Space(10);
					GUILayout.Label("G:");
					bool maskG = GUILayout.Toggle(layer.maskG, GUIContent.none);
					if(maskG != layer.maskG)
					{
						PushUndo("Toggle Green Channel");
						layer.maskG = maskG;
						ReloadLayers();
					}
					
					GUILayout.Space(10);
					GUILayout.Label("B:");
					bool maskB = GUILayout.Toggle(layer.maskB, GUIContent.none);
					if(maskB != layer.maskB)
					{
						PushUndo("Toggle Blue Channel");
						layer.maskB = maskB;
						ReloadLayers();
					}
					
					GUILayout.Space(10);
					GUILayout.Label("A:");
					bool maskA = GUILayout.Toggle(layer.maskA, GUIContent.none);
					if(maskA != layer.maskA)
					{
						PushUndo("Toggle Alpha Channel");
						layer.maskA = maskA;
						ReloadLayers();
					}
				});
				
				GUILayout.Space(4);
			}
			EditorGUILayout.EndVertical();
			
			if(_currentPaintLayer != i)
			{
				EditorGUIUtility.AddCursorRect(mainLayerRect, MouseCursor.Link);
			}
			
			if(Event.current.type == EventType.MouseDown && mainLayerRect.Contains(Event.current.mousePosition))
			{
				int saveInt = i;
				EditorApplication.delayCall += ()=>{ _currentPaintLayer = saveInt; };
			}
			
			VPaintGUIUtility.columnViewBoxCount = 1;
		}
	}
	
	void ColorsGroupGUI (Rect rect)
	{
		List<Rect> colorRects = new List<Rect>();
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		for(int i = 0; i < VertexEditorColors.colors.Count; i++)
		{
			if(i % 5 == 0 && i != 0)
			{
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
			}
			
			colorRects.Add(EditorGUILayout.BeginVertical());
			Color c = EditorGUILayout.ColorField(VertexEditorColors.colors[i], GUILayout.Width(40));
			if(c != VertexEditorColors.colors[i])
			{
				VertexEditorColors.colors[i] = c; 
				VertexEditorColors.Serialize();
			}
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(10);
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		
		for(int i = 0; i < colorRects.Count; i++)
		{
			Rect r = colorRects[i];
			r.y += 14;
			r.x += 4;
			if(EditorGUI.Toggle(r, VertexEditorControls.selectedColor == i))
				VertexEditorControls.selectedColor = i;
		}
		
		GUILayout.Space(25);
		
		VPaintGUIUtility.DrawColumnRow(20,
		(r)=>{
			GUILayout.Label("RGBA");
			GUILayout.FlexibleSpace();
			EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
			
			bool allSelected = VertexEditorControls.useR 
							&& VertexEditorControls.useG 
							&& VertexEditorControls.useB 
							&& VertexEditorControls.useA;
			
			if(r.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				VertexEditorControls.useR = !allSelected;
				VertexEditorControls.useG = !allSelected;
				VertexEditorControls.useB = !allSelected;
				VertexEditorControls.useA = !allSelected;
				Event.current.Use();
			}
			EditorGUILayout.Toggle(allSelected, GUILayout.Width(16));
		},
		(r)=>{
			GUILayout.Label("Red");
			GUILayout.FlexibleSpace();
			EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
			if(r.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				VertexEditorControls.useR = !VertexEditorControls.useR;
				Event.current.Use();
			}
			EditorGUILayout.Toggle(VertexEditorControls.useR, GUILayout.Width(16));
		},
		(r)=>{
			GUILayout.Label("Green");
			GUILayout.FlexibleSpace();
			EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
			if(r.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				VertexEditorControls.useG = !VertexEditorControls.useG;
				Event.current.Use();
			}
			EditorGUILayout.Toggle(VertexEditorControls.useG, GUILayout.Width(16));
		},
		(r)=>{
			GUILayout.Label("Blue");
			GUILayout.FlexibleSpace();
			EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
			if(r.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				VertexEditorControls.useB = !VertexEditorControls.useB;
				Event.current.Use();
			}
			EditorGUILayout.Toggle(VertexEditorControls.useB, GUILayout.Width(16));
		},
		(r)=>{
			GUILayout.Label("Alpha");
			GUILayout.FlexibleSpace();
			EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
			if(r.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				VertexEditorControls.useA = !VertexEditorControls.useA;
				Event.current.Use();
			}
			EditorGUILayout.Toggle(VertexEditorControls.useA, GUILayout.Width(16));
		});
	}
	
	void ControlsGroupGUI (Rect rect)
	{	
		VPaintGUIUtility.columnViewBoxCount = tool == VertexTool.Paint ? 2 : 1;
		VPaintGUIUtility.DrawColumnRow(24, 
		(r)=>{
			EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
			if(r.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				tool = VertexTool.Paint;
			}
			GUILayout.FlexibleSpace();
			GUILayout.Label("Paint");
			GUILayout.FlexibleSpace();
			
			VPaintGUIUtility.columnViewBoxCount = tool == VertexTool.Erase ? 2 : 1;
		},
		(r)=>{
			EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
			if(r.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				tool = VertexTool.Erase;
			}
			GUILayout.FlexibleSpace();
			GUILayout.Label("Erase");
			GUILayout.FlexibleSpace();
			
			VPaintGUIUtility.columnViewBoxCount = tool == VertexTool.Smooth ? 2 : 1;
		},
		(r)=>{
			EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
			if(r.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				tool = VertexTool.Smooth;
			}
			GUILayout.FlexibleSpace();
			GUILayout.Label("Smooth");
			GUILayout.FlexibleSpace();
		});
		VPaintGUIUtility.columnViewBoxCount = 1;
		
		VPaintGUIUtility.DrawColumnRow(24, 
		()=>{
			VertexEditorControls.radius = EditorGUILayout.Slider("Radius", VertexEditorControls.radius, 0, 30);
		});
		
		VPaintGUIUtility.DrawColumnRow(24, 
		()=>{
			VertexEditorControls.radius = Mathf.Clamp(EditorGUILayout.Slider("Radius (Fine)", VertexEditorControls.radius, 
					VertexEditorControls.radius - 1f, VertexEditorControls.radius + 1f), 0, 30);
		});
		
		VPaintGUIUtility.DrawColumnRow(24, 
		()=>{
			VertexEditorControls.strength = EditorGUILayout.Slider("Opacity", VertexEditorControls.strength, 0, 100);
		});
		VPaintGUIUtility.DrawColumnRow(24, 
		()=>{
			VertexEditorControls.falloff = EditorGUILayout.Slider("Falloff", VertexEditorControls.falloff, 0, 10);
		});
	}
	
	void CordoneGroupGUI (Rect rect)
	{
		VPaintGUIUtility.columnViewBoxCount = 1;
		VPaintGUIUtility.DrawColumnRow(24,
		(r)=>{
			
			Rect m = EditorGUILayout.BeginHorizontal();
			m.y = r.y;
			m.height = r.height;
			
			EditorGUIUtility.AddCursorRect(m, MouseCursor.Link);
			if(m.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				VertexEditorControls.cordoneEnabled = !VertexEditorControls.cordoneEnabled;
			}
			
			GUILayout.Label("Enabled");
			GUILayout.FlexibleSpace();
			
			Rect b = EditorGUILayout.BeginHorizontal();
			GUILayout.Space(20);
			EditorGUILayout.EndHorizontal();
			
			GUI.Toggle(b, VertexEditorControls.cordoneEnabled, GUIContent.none);
			
			EditorGUILayout.EndHorizontal();
			
//			GUILayout.Space(10);
			
		});
		GUI.enabled = VertexEditorControls.cordoneEnabled;
		VPaintGUIUtility.DrawColumnRow(24,
		(r)=>{
			VertexEditorControls.cordonePosition = EditorGUILayout.Vector3Field("Position:", VertexEditorControls.cordonePosition);
		});
		VPaintGUIUtility.DrawColumnRow(24,
		(r)=>{
			VertexEditorControls.cordoneSize = EditorGUILayout.Vector3Field("Size:", VertexEditorControls.cordoneSize);
		});
		GUI.enabled = true;
	}
	
	void ObjectsGroupGUI (Rect rect)
	{
		bool allMasked = true;
		foreach(var b in currentEditingContentsMask) allMasked &= b;
		
		VPaintGUIUtility.columnViewBoxCount = 2;
		VPaintGUIUtility.DrawColumnRow(24,
		()=>{
			bool maskAll = EditorGUILayout.Toggle(allMasked, GUILayout.Width(16));
			if(maskAll != allMasked)
			{
				for(int i = 0; i < currentEditingContentsMask.Length; i++) 
				{
					currentEditingContentsMask[i] = maskAll;
				}
			}
			
			GUILayout.Label(currentEditingContents.Length + " object" + (currentEditingContents.Length == 1 ? "" : "s"));
			
			GUILayout.FlexibleSpace();
			
			if(VPaintGUIUtility.FoldoutMenu())
			{
				GenericMenu menu = new GenericMenu();
				
				menu.AddItem(new GUIContent("Add Object(s)"), false, ()=>{
					EditorWindow.GetWindow<VPaintAddColorerWindow>(true);
				});				
				
				menu.AddItem(new GUIContent("Remove Selected"), false, ()=>{
					PushUndo("Remove selected");
					foreach(var vc in maskedVPaintObjects()){
						layerCache.RemoveColorer(vc);
					}
					CleanupInstances();
					layerCache = layerCache;
				});
				
				menu.AddSeparator("");
				
				menu.AddItem(new GUIContent("Flood Fill"), false, ()=>{
					FloodFill();
				});
				
				menu.AddItem(new GUIContent("Invert"), false, ()=>{
					InvertColors();
				});
				
				menu.AddItem(new GUIContent("Erase"), false, ()=>{
					EraseAll();
				});
				
				menu.ShowAsContext();
			}
			
		});
		
		VPaintGUIUtility.columnViewBoxCount = 1;
		for(int i = 0; i < currentEditingContents.Length; i++)
		{
			var vc = currentEditingContents[i];
			
			VPaintGUIUtility.DrawColumnRow(20,
			()=>{
				currentEditingContentsMask[i] = EditorGUILayout.Toggle(currentEditingContentsMask[i], GUILayout.Width(16));
				Rect labelRect = EditorGUILayout.BeginVertical();
				GUILayout.Label(vc.name);
				EditorGUILayout.EndVertical();
				EditorGUIUtility.AddCursorRect(labelRect, MouseCursor.Link);
				if(labelRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
				{
					EditorGUIUtility.PingObject(vc);
				}
				
				GUILayout.FlexibleSpace();
					
				if(VPaintGUIUtility.FoldoutMenu())
				{
					GenericMenu menu = new GenericMenu();
					menu.AddItem(new GUIContent("Flood"), false, ()=>{
						PushUndo("Flood " + vc.name);
						Flood(vc);
					});
					menu.AddItem(new GUIContent("Invert"), false, ()=>{
						PushUndo("Invert " + vc.name);
						Invert(vc);
					});
					menu.AddItem(new GUIContent("Erase"), false, ()=>{
						PushUndo("Erase " + vc.name);
						EraseAll(vc);
					});
					menu.AddItem(new GUIContent("Remove"), false, ()=>{
						PushUndo("Remove " + vc.name);
						layerCache.RemoveColorer(vc);
						CleanupInstances();
						layerCache = layerCache;
					});
					menu.ShowAsContext();
				}
				
			});
		}
	}
	
	void FloodFill ()
	{
		PushUndo("Flood Fill");
		foreach(VPaintObject vc in maskedVPaintObjects())
			Flood(vc);
		dirty = true;
	}
	
	void Flood (VPaintObject vc)
	{
		if(!vc) return;
		
		VPaintVertexData paintData = currentPaintLayer.GetOrCreate(vc);
		
		var verts = vc.GetMeshInstance().vertices;
		var transform = vc.transform;
		var cordoneBounds = VertexEditorControls.GetCordoneBounds();
		
		if(VertexEditorControls.cordoneEnabled && !cordoneBounds.Intersects(vc.GetBounds()))
			return;
		
		for(int i = 0; i < paintData.colors.Length; i++){
			Color oldColor = paintData.colors[i];
			Color newColor = VertexEditorControls.targetColor;
			
			if(VertexEditorControls.cordoneEnabled
			&& !cordoneBounds.Contains( transform.TransformPoint( verts[i] ) ))
			{
				newColor = oldColor;
			}
			else
			{
				if(!VertexEditorControls.useR) newColor.r = oldColor.r;
				if(!VertexEditorControls.useG) newColor.g = oldColor.g;
				if(!VertexEditorControls.useB) newColor.b = oldColor.b;
				if(!VertexEditorControls.useA) newColor.a = oldColor.a;
			}
			
			paintData.colors[i] = newColor;
			paintData.transparency[i] = 1f;
		}
		
		LoadObject(vc);
	}
	
	void EraseAll ()
	{
		PushUndo("Erase All");
		foreach(VPaintObject vc in maskedVPaintObjects())
			EraseAll(vc);
		dirty = true;
	}
	void EraseAll (VPaintObject vc)
	{
		if(!vc) return;
		
		var paintData = currentPaintLayer.GetOrCreate(vc);
		
		var verts = vc.GetMeshInstance().vertices;
		var transform = vc.transform;
		var cordoneBounds = VertexEditorControls.GetCordoneBounds();
		
		if(VertexEditorControls.cordoneEnabled && !cordoneBounds.Intersects(vc.GetBounds()))
			return;
		
		for(int i = 0; i < paintData.colors.Length; i++)
		{
			if(VertexEditorControls.cordoneEnabled
			&& !cordoneBounds.Contains( transform.TransformPoint( verts[i] ) ))
			{
				continue;
			}
			
			paintData.transparency[i] = 0;
		}
		
//		currentPaintLayer.paintData.Clear();
		
		LoadObject(vc);
//		ReloadLayers();
	}
	
	void InvertColors ()
	{
		PushUndo("Invert Colors");
		foreach(var vc in maskedVPaintObjects())
			Invert(vc);
		dirty = true;
	}
	
	void Invert (VPaintObject vc)
	{
		VPaintVertexData paintData = currentPaintLayer.GetOrCreate(vc);
		
		var verts = vc.GetMeshInstance().vertices;
		var transform = vc.transform;
		var cordoneBounds = VertexEditorControls.GetCordoneBounds();
		
		if(VertexEditorControls.cordoneEnabled && !cordoneBounds.Intersects(vc.GetBounds()))
			return;
		
		for(int i = 0; i < paintData.colors.Length; i++){
			Color c = paintData.colors[i];
			
			if(VertexEditorControls.cordoneEnabled
			&& !cordoneBounds.Contains( transform.TransformPoint( verts[i] ) ))
			{
				continue;
			}
			
			if(VertexEditorControls.useR) c.r = 1f - c.r;
			if(VertexEditorControls.useG) c.g = 1f - c.g;
			if(VertexEditorControls.useB) c.b = 1f - c.b;
			if(VertexEditorControls.useA) c.a = 1f - c.a;
			
			paintData.colors[i] = c;
		}
		LoadObject(vc);
//		ReloadLayers();	
	}
	
	void ImportLightmaps ()
	{
		EditorWindow.GetWindow<VPaintImportLightmapWindow>(true, "Import Lightmaps to Vertex Colors");
	}
	
	void ImportLayerCache ()
	{
		EditorWindow.GetWindow<VPaintImportColorsWindow>(true, "Import VPaint Group");
	}
	
	void ImportTexture ()
	{
		EditorWindow.GetWindow<VPaintImportTextureWindow>(true, "Import Texture");
	}
	
	void ImportBaseLayer ()
	{
		PushUndo("Import Base Layer");
		var layer = new VPaintLayer();
		foreach(var vc in currentEditingContents)
		{
			var data = layer.GetOrCreate(vc);
			data.colors = vc.originalMesh.colors;
			data.transparency = new float[data.colors.Length];
			for(int i = 0; i < data.transparency.Length; i++) data.transparency[i] = 1f;
		}
		layerStack.layers.Add(layer);
		ReloadLayers();
	}
	
	void MergeAllLayers ()
	{
		PushUndo("Collapse All");
		VPaintLayerStack stack = layerStack;
		VPaintLayer layer = stack.GetMergedLayer();
		layer.name = "Merged Layer";
		layer.blendMode = VPaintBlendMode.Opaque;
		stack.layers = new List<VPaintLayer>();
		stack.layers.Add(layer);
		ReloadLayers();
	}
	
	void CreateMergedLayer ()
	{
		PushUndo("Create Merged Layer");
		VPaintLayerStack stack = layerStack;
		VPaintLayer layer = stack.GetMergedLayer();
		layer.name = "Merged Layer";
		layer.blendMode = VPaintBlendMode.Opaque;
		stack.layers.Add(layer);
		ReloadLayers();
	}
	
	void NewLayer ()
	{
		PushUndo("New Paint Layer");
		VPaintLayer layer = new VPaintLayer();
		layer.name = "Layer " + (layerStack.layers.Count+1);
		layerStack.layers.Add(layer);
		ReloadLayers();
	}
	
	void MergeLayerDown (object o)
	{
		int i = (int) o;
		PushUndo("Create Merged Layer");
		var layer = layerStack.layers[i-1];
		var mergeLayer = layerStack.layers[i];
		layer.Merge(mergeLayer);
		layer.name = mergeLayer.name;
		layerStack.layers.RemoveAt(i);
		_currentPaintLayer = i-1;
		ReloadLayers();
	}
	
	void DuplicateLayer (object o)
	{
		int i = (int) o;
		PushUndo("Duplicate Paint Layer");
		VPaintLayer layer = layerStack.layers[i].Clone();
		layer.name += " (Clone)";
		layerStack.layers.Insert(i+1, layer);
		_currentPaintLayer = i+1;
		ReloadLayers();
	}
	
	void ConvertToGamma (object o)
	{
		int i = (int) o;
		PushUndo("Convert Layer To Gamma");
		VPaintLayer layer = layerStack.layers[i];
		foreach(var pd in layer.paintData)
		{
			for(int v = 0; v < pd.colors.Length; v++)
				pd.colors[v] = pd.colors[v].gamma;
		}
		ReloadLayers();
	}
	void ConvertToLinear (object o)
	{
		int i = (int) o;
		PushUndo("Convert Layer To Linear");
		VPaintLayer layer = layerStack.layers[i];
		foreach(var pd in layer.paintData)
		{
			for(int v = 0; v < pd.colors.Length; v++)
				pd.colors[v] = pd.colors[v].linear;
		}
		ReloadLayers();
	}
	
	void MoveLayerUp (object o)
	{
		int i = (int) o;
		PushUndo("Reorder Paint Layers");
		if(i < layerStack.layers.Count-1)
		{
			VPaintLayer l1 = layerStack.layers[i];
			VPaintLayer l2 = layerStack.layers[i+1];
			layerStack.layers[i] = l2;
			layerStack.layers[i+1] = l1;
			ReloadLayers();
			_currentPaintLayer++;
		}
	}
	
	void MoveLayerDown (object o)
	{
		int i = (int) o;
		PushUndo("Reorder Paint Layers");
		if(0 < i)
		{
			VPaintLayer l1 = layerStack.layers[i];
			VPaintLayer l2 = layerStack.layers[i-1];
			layerStack.layers[i] = l2;
			layerStack.layers[i-1] = l1;
			ReloadLayers();
			_currentPaintLayer--;
		}
	}
	
	void RemoveLayer (object o)
	{
		int i = (int) o;
		PushUndo("Remove Paint Layer");
		layerStack.layers.RemoveAt(i);
		i--;
		ReloadLayers();
	}
	
	public void ToggleWireframe () 
	{
//		if(lastDrawnSceneView.m_RenderMode != SceneView.RenderMode.Textured)
//			lastDrawnSceneView.m_RenderMode = SceneView.RenderMode.Textured;
//		else
//			lastDrawnSceneView.m_RenderMode = SceneView.RenderMode.TexWire;
	}
	
	public void OnSceneGUI () 
	{
		if(layerStack == null) return;
		
		Tools.current = Tool.None;
		if(Event.current.type == EventType.Layout) HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Native));
		
		if(overrideTool) return;
		
		deltaTime = Time.realtimeSinceStartup - lastTime;
		lastTime = Time.realtimeSinceStartup;
		
		if(SceneView.currentDrawingSceneView != lastDrawnSceneView){
			lastDrawnSceneView = SceneView.currentDrawingSceneView;
		}
		
		SceneViewKeyHandler();
		
		if(lastDrawnSceneView)
		{
			
			if(rightClickDown)
			{
				Vector3 input = new Vector3();				
				if(rightKey) input.x++;
				if(leftKey) input.x--;
				if(upKey) input.z++;
				if(downKey) input.z--;
				if(raiseKey) input.y++;
				if(lowerKey) input.y--;
				
				if(input != Vector3.zero){
					Vector3 camInput = input;
					camInput.z = 0;
					input.x = 0;
					
					float speed = noclipSpeed;
					
					if(Event.current.shift){
						noclipSpeedMod = Mathf.Clamp(noclipSpeedMod + deltaTime, 2f, 10f);
						speed *= noclipSpeedMod;
					}else{
						noclipSpeedMod = 2f;
					}
					
					//Move on the local X axis.. i dont know why I have to do this, but I do
					Camera.current.transform.position = Camera.current.transform.position
						+ ((Camera.current.transform.rotation * camInput.normalized) * deltaTime * speed);
					//Move on the local Z axis.. again, wtf!
					lastDrawnSceneView.pivot = lastDrawnSceneView.pivot + ((lastDrawnSceneView.rotation * input.normalized) * deltaTime * speed);	
				}
			}
		}
		
		if(currentPaintLayer != null && currentPaintLayer.enabled)
		{
			DoVertexPaint();
		}
		
		if(Event.current.type == EventType.MouseDown
		&& Event.current.button == 0
		&& !Event.current.alt)
			Event.current.Use();
	}
	
	struct ColorData
	{
		public Color[] colors;
		public float[] transparency;
	}
	public void DoVertexPaint () {
		
		bool useEvent = false;
		
		Camera camera = Camera.current;
		if(!camera) return;
		Vector3 mousePosition = Event.current.mousePosition;
		
		Bounds cordoneBounds = VertexEditorControls.GetCordoneBounds();
		if((Event.current.type == EventType.Repaint || Event.current.type == EventType.Layout)
		&& VertexEditorControls.cordoneEnabled)
		{
			Handles.color = Color.yellow;
			var halfSize = VertexEditorControls.cordoneSize*0.5f;
			var pos = VertexEditorControls.cordonePosition;
			Handles.DrawPolyLine(
				pos + Vector3.Scale(halfSize, new Vector3( 1, 1, 1)),
				pos + Vector3.Scale(halfSize, new Vector3( 1, 1,-1)),
				pos + Vector3.Scale(halfSize, new Vector3(-1, 1,-1)),
				pos + Vector3.Scale(halfSize, new Vector3(-1, 1, 1)),
				pos + Vector3.Scale(halfSize, new Vector3( 1, 1, 1))
			);
			Handles.DrawPolyLine(
				pos + Vector3.Scale(halfSize, new Vector3( 1,-1, 1)),
				pos + Vector3.Scale(halfSize, new Vector3( 1,-1,-1)),
				pos + Vector3.Scale(halfSize, new Vector3(-1,-1,-1)),
				pos + Vector3.Scale(halfSize, new Vector3(-1,-1, 1)),
				pos + Vector3.Scale(halfSize, new Vector3( 1,-1, 1))
			);
			Handles.DrawPolyLine(
				pos + Vector3.Scale(halfSize, new Vector3( 1,-1, 1)),
				pos + Vector3.Scale(halfSize, new Vector3( 1, 1, 1))
			);
			Handles.DrawPolyLine(
				pos + Vector3.Scale(halfSize, new Vector3(-1,-1,-1)),
				pos + Vector3.Scale(halfSize, new Vector3(-1, 1,-1))
			);
			Handles.DrawPolyLine(
				pos + Vector3.Scale(halfSize, new Vector3( 1,-1,-1)),
				pos + Vector3.Scale(halfSize, new Vector3( 1, 1,-1))
			);
			Handles.DrawPolyLine(
				pos + Vector3.Scale(halfSize, new Vector3(-1,-1, 1)),
				pos + Vector3.Scale(halfSize, new Vector3(-1, 1, 1))
			);
		}
		
		if(!camera.pixelRect.Contains(mousePosition)) return;
		
		RaycastHit hit = new RaycastHit();
		hit.distance = Mathf.Infinity;
		Ray r = HandleUtility.GUIPointToWorldRay(mousePosition);
		
		if(VertexEditorControls.cordoneEnabled && !cordoneBounds.IntersectRay(r)) return;
		
//		if(editColliders == null) SetupColliders();
		
		MeshCollider hitCollider = null;
		VPaintObject hitObject = null;
		
//		foreach(var kvp in editColliders)
//		{
		foreach(var vc in currentEditingContents)
		{
			if(!currentEditingContentsMask[vc.index]) continue;
			if(VertexEditorControls.cordoneEnabled && !cordoneBounds.Intersects(GetBounds(vc))) continue;
			var col = GetCollider(vc);
			RaycastHit testHit;
			if(col.Raycast(r, out testHit, Mathf.Infinity))
			{
				if(!hitCollider || testHit.distance < hit.distance)
				{
					hit = testHit;
					hitCollider = col;
					hitObject = vc;
				}
			}
		}
		
		if(hitCollider) {
		
			if(VertexEditorControls.cordoneEnabled && !cordoneBounds.Contains(hit.point)) return;
			
			var handleNormal = -camera.transform.forward;
			Color handleColor = VertexEditorControls.targetColor;
			handleColor.a = 1f;
			Handles.color = handleColor;
			Handles.DrawWireDisc(hit.point, handleNormal, VertexEditorControls.radius * 0.999f);
			Handles.DrawWireDisc(hit.point, handleNormal, VertexEditorControls.radius * 1.001f);
			Handles.DrawWireDisc(hit.point, handleNormal, VertexEditorControls.radius);
			Handles.DrawWireDisc(hit.point, handleNormal, Mathf.Pow(0.5f, VertexEditorControls.falloff) * VertexEditorControls.radius);			
			
			if(Event.current.control && middleMouseDown)
			{	
				int[] triangles = hitCollider.sharedMesh.triangles;
				Color[] hitColors = hitObject.GetMeshInstance().colors;
					
				if(hitColors.Length!=0)
				{
					//Vertex indices
					int t1 = triangles[(hit.triangleIndex*3)+0];
					int t2 = triangles[(hit.triangleIndex*3)+1];
					int t3 = triangles[(hit.triangleIndex*3)+2];
					
					//Colors
					Color c1 = hitColors[t1];
					Color c2 = hitColors[t2];
					Color c3 = hitColors[t3];
					
					Vector3 b = hit.barycentricCoordinate;
					
					var sampleColor = c1 * b.x + c2 * b.y + c3 * b.z;
	
					VertexEditorControls.targetColor = sampleColor;
					useEvent = true;
				}
			}
			
			if(Event.current.type == EventType.MouseUp) modifierHeld = false;
			modifierHeld = false;
			
			if(Event.current.alt || Event.current.command)
				modifierHeld = true;

			if(!modifierHeld && !sampleKey && leftMouseDown
			&& ((Event.current.control && rightClickDown) || (!Event.current.control && !rightClickDown)) && !middleMouseDown){
				
//				Collider[] colliders = Physics.OverlapSphere(hit.point, VertexEditorControls.radius);
				
				Dictionary<VPaintObject, ColorData> colorsToApply = new Dictionary<VPaintObject, ColorData>();
				List<VPaintObject> affectedColorers = new List<VPaintObject>();
//				for(int i = 0; i < colliders.Length; i++){
//				foreach(var kvp in editColliders)
//				{
				foreach(var vc in currentEditingContents)
				{
						
//					Collider c = kvp.Key;
					
//					var mc = c as MeshCollider;
//					if(!mc) continue;
					
//					if(!editColliders.ContainsKey(mc)) continue;
					
					var mc = GetCollider(vc);
//					var mc = kvp.Key;
//					var vc = kvp.Value;
//					VPaintObject vc = editColliders[mc];
					if(!vc) continue;
					
					if(VertexEditorControls.cordoneEnabled && !cordoneBounds.Intersects(mc.bounds))
						continue;
					
					if(!currentEditingContentsMask[vc.index])
						continue;
					
					affectedColorers.Add(vc);
				}
				
				Color targColor = VertexEditorControls.targetColor;
				
				VertexTool useTool = tool;
				if(Event.current.shift) useTool = VertexTool.Smooth;
				if(Event.current.control) useTool = VertexTool.Erase;
				
				foreach(VPaintObject vc in affectedColorers) 
				{
					Mesh m = vc.GetMeshInstance();
					
					var pd = currentPaintLayer.GetOrCreate(vc);
					Color[] colors = pd.colors;
					float[] transparency = pd.transparency;
					
					Color[] newColors = new Color[colors.Length];
					float[] newTransparency = new float[transparency.Length];
					
					Vector3[] vertices = m.vertices;
					
					Transform t = vc.transform;
					
					bool used = false;
					
					float str = VertexEditorControls.strength / 30f;
					
					for(int b = 0; b < vertices.Length; b++)
					{
						Vector3 v = t.TransformPoint(vertices[b]);
						
						float distance = Vector3.Distance(v, hit.point);
						
						if(distance < VertexEditorControls.radius
						&& (!VertexEditorControls.cordoneEnabled || cordoneBounds.Contains(v)))
						{
							float affect = 
								Mathf.Clamp01(
									Mathf.Pow(
										1 - distance / VertexEditorControls.radius,
										VertexEditorControls.falloff
									) * str * deltaTime
								);
							
							float trans = transparency[b];
							Color oldColor = colors[b];
							
							if(useTool == VertexTool.Paint)
							{
//								trans = Mathf.Clamp01(trans + affect);
								trans = Mathf.Lerp(trans, 1f, affect);// + (Mathf.Pow(1 - trans, 4)));
								Color newColor = Color.Lerp(
									Color.Lerp(oldColor, targColor, Mathf.Pow(1 - trans, 4)),
									targColor, affect);// + (Mathf.Pow(1 - trans, 4)));
								
								if(!VertexEditorControls.useR) newColor.r = oldColor.r;
								if(!VertexEditorControls.useG) newColor.g = oldColor.g;
								if(!VertexEditorControls.useB) newColor.b = oldColor.b;
								if(!VertexEditorControls.useA) newColor.a = oldColor.a;
								
								newColors[b] = newColor;
								newTransparency[b] = trans;
							}
							if(useTool == VertexTool.Erase)
							{
								trans = Mathf.Clamp01(trans - affect);
								newColors[b] = oldColor;
								newTransparency[b] = trans;
							}
							if(useTool == VertexTool.Smooth)
							{
								Vector4 col = new Vector4();
								float tr = 0;
								float fac = 0;
								float rad = VertexEditorControls.radius - distance;
								foreach(var vc2 in affectedColorers)
								{									
									var vc2data = currentPaintLayer.Get(vc2);
									if(vc2data == null) continue;
									
									var vc2mesh = vc2.GetMeshInstance();
									var vc2verts = vc2mesh.vertices;
									var vc2transform = vc2.transform;
									
									for(int c = 0; c < vc2verts.Length; c++)
									{
										var vc2vert = vc2transform.TransformPoint(vc2verts[c]);
										
										if(VertexEditorControls.cordoneEnabled && !cordoneBounds.Contains(vc2vert))
											continue;
										
										float dst = Vector3.Distance(v, vc2vert);
										if(rad < dst) continue;
										
										float f = 1 - dst / rad;
										
										Vector4 sampC = (Vector4)vc2data.colors[c];
										float sampT = vc2data.transparency[c];
										col += sampC * f;
										tr += sampT * f;
										fac += f;
									}
								}
								if(fac != 0)
								{
									tr /= fac;
									col /= fac;
									
									Color newColor = Color.Lerp(oldColor, (Color)col, affect);
									trans = Mathf.Lerp(trans, tr, affect);
									
									if(!VertexEditorControls.useR) newColor.r = oldColor.r;
									if(!VertexEditorControls.useG) newColor.g = oldColor.g;
									if(!VertexEditorControls.useB) newColor.b = oldColor.b;
									if(!VertexEditorControls.useA) newColor.a = oldColor.a;
									
									newColors[b] = newColor;
									newTransparency[b] = trans;
								}
							}
							
							used = true;
						}
						else
						{
							newColors[b] = colors[b];
							newTransparency[b] = transparency[b];
						}
					}
					
					if(used){						
						colorsToApply.Add(vc, new ColorData(){colors = newColors, transparency = newTransparency});
						dirty = true;
					}
				}
				
				if(colorsToApply.Count != 0)
				{
					PushUndo("Paint Vertices");
					foreach(var kvp in colorsToApply)
					{
						SetColors(currentPaintLayer, kvp.Key, kvp.Value.colors, kvp.Value.transparency);
					}
				}
			}
			
		}
		
		if(useEvent) Event.current.Use();
		
	}
	
	public void LoadLayerStack (VPaintLayerStack stack)
	{
		LoadLayers(stack.layers);
	}
	
	public void LoadLayers (List<VPaintLayer> layers)
	{
		ClearAllColors();
		List<VPaintObject> affectedColorers = new List<VPaintObject>();
		foreach(VPaintLayer layer in layers)
		{
			LoadLayer(layer, affectedColorers);
		}
		foreach(VPaintObject vc in affectedColorers)
		{
			vc.ApplyColorsBuilder();
		}
	}
	
	public void ClearAllColors () 
	{
		foreach(var vc in currentEditingContents)
		{
			if(!vc) continue;
			ClearColors(vc);
		}
	}
	
	public void ClearColors (VPaintObject vc)
	{
		if(!vc) return;
		
		Mesh m = vc.GetMeshInstance();
		if(!m) return;
		Color[] colors = m.colors;
		Vector3[] vertices = m.vertices;
		
		if(colors.Length != vertices.Length)
			colors = new Color[vertices.Length];
		
		for(int i = 0; i < colors.Length; i++)
		{
			colors[i] = Color.grey;
		}
		m.colors = colors;
	}
	
	public void LoadLayer (VPaintLayer layer)
	{
		ClearAllColors();
		
		List<VPaintObject> affectedColorers = new List<VPaintObject>();
		
		LoadLayer(layer, affectedColorers);
		
		foreach(VPaintObject vc in affectedColorers)
		{
			vc.ApplyColorsBuilder();
		}
		
		lastLoadedLayer = layer;
	}
	
	public void LoadLayer (VPaintLayer layer, List<VPaintObject> affectedColorers)
	{
		bool cancelAllInvalid = false;
		foreach(var vc in currentEditingContents)
		{	
			if(!vc) continue;
			
			var paintData = layer.Get(vc);
			if(paintData != null)
			{
				Mesh m = vc.GetMeshInstance();
				if(m.vertices.Length != paintData.colors.Length){
					if(cancelAllInvalid)
					{
						layer.Remove(vc);
						continue;
					}
					var dialogResult = EditorUtility.DisplayDialogComplex(
						"Invalid Vertex Colors", 
						"The colorer " + vc.name + " contains vertex color data which does not match the source mesh, on layer '" + layer.name + "'. Attempt to copy colors to new mesh?",
						"Okay", "Cancel All", "Cancel");
					if(dialogResult == 0)
					{
						Color[] newColors = new Color[m.vertices.Length];
						float[] newTrans = new float[m.vertices.Length];
						for(int i = 0; i < paintData.colors.Length && i < m.vertices.Length; i++)
						{
							newColors[i] = paintData.colors[i];
							newTrans[i] = paintData.transparency[i];
						}
						paintData.colors = newColors;
						paintData.transparency = newTrans;
					}
					else
					{
						if(dialogResult == 2) cancelAllInvalid = true;
						layer.Remove(vc);
						continue;
					}
				}
				
				PaintObject(vc, layer, paintData);
				if(!affectedColorers.Contains(vc))
					affectedColorers.Add(vc);
			}
		}
	}
	
	public void LoadAllObjects ()
	{
		foreach(var vc in currentEditingContents)
		{
			LoadObject(vc);
		}
	}
	
	public void LoadObject (VPaintObject vc)
	{
		if(layerStack == null) return;
		LoadObject(vc, layerStack.layers);
	}
	
	public void LoadObject (VPaintObject vc, List<VPaintLayer> layers)
	{
		ClearColors(vc);
		PaintObject(vc, baseLayer);
		foreach(VPaintLayer layer in layers)
		{
			if(!layer.enabled) continue;
			PaintObject(vc, layer);
		}
		vc.ApplyColorsBuilder();
	}
	
	public void LoadObject (VPaintObject vc, VPaintLayer layer)
	{
		ClearColors(vc);
		PaintObject(vc, layer);
		vc.ApplyColorsBuilder();
	}
	
	public void PaintObject (VPaintObject vc, VPaintLayer layer)
	{		
		VPaintVertexData data = layer.Get(vc);
		if(data==null) return;

		PaintObject(vc, layer, data);
	}
	
	public static void PaintObject (VPaintObject vc, VPaintLayer layer, VPaintVertexData data)
	{
		if(vc.colorsBuilder == null)
		{
			vc.colorsBuilder = new Color[data.colors.Length];
		}
		
		if(vc.transparencyBuilder == null)
		{
			vc.transparencyBuilder = new float[data.colors.Length];
		}
		
		VPaintUtility.MergeColors(
			vc.colorsBuilder, vc.transparencyBuilder,
			data.colors, data.transparency,
			layer.blendMode, layer.opacity,
			layer.maskR, layer.maskG, layer.maskB, layer.maskA
		);		
	}
	
	public void SetColors (VPaintLayer layer, VPaintObject vc, Color[] colors, float[] transparency)
	{
		if(layerStack == null) return;
		VPaintVertexData data = layer.GetOrCreate(vc);
		SetColors(layer, data, colors, transparency);
		LoadObject(vc, layerStack.GetActiveLayers());
	}
	
	public void SetColors (VPaintLayer layer, VPaintVertexData data, Color[] colors, float[] transparency)
	{
		data.colors = colors;
		data.transparency = transparency;
		dirty = true;
	}
	
	public void ReloadLayers (){
		List<VPaintLayer> lyrs = new List<VPaintLayer>();
		if(baseLayer != null) lyrs.Add(baseLayer);
		if(layerStack != null) lyrs.AddRange(layerStack.GetActiveLayers());
		LoadLayers(lyrs);
	}
	
	public static Color GetSampleColor (List<VPaintObject> colorers, Vector3 position, float radius)
	{
		Vector4 avg = new Vector4();
		float fac = 0;
		foreach(VPaintObject vc in colorers)
		{
			Mesh m = vc.GetMeshInstance();
			
			Vector3[] vertices = m.vertices;
			
			Color[] colors = m.colors;
			
			Transform transform = vc.transform;
			
			for(int i = 0; i < vertices.Length; i++)
			{
				Vector3 vPos = transform.TransformPoint(vertices[i]);
				float dist = Vector3.Distance(position, vPos);
				
				if(radius < dist) continue;
				
				float factor = dist/radius;
				
				fac += factor;
				avg += (Vector4)(colors[i] * factor);
			}
		}
		
		return (Color)(avg/fac);		
	}
	
	private bool raiseKey = false;
	private bool lowerKey = false;
	private bool rightKey = false;
	private bool leftKey = false;
	private bool upKey = false;
	private bool downKey = false;
	private bool sampleKey = false;
	private bool leftMouseDown = false;
	private bool middleMouseDown = false;
	private bool rightClickDown = false;
	
	private const float noclipSpeed = 30f;
	private float noclipSpeedMod = 4f;
	public void SceneViewKeyHandler () {
		bool keyPressed = false;
		bool mousePressed = false;
		bool active = false;		
		
		if(Event.current.type == EventType.KeyDown){
			keyPressed = true;
			active = true;
		}else if(Event.current.type == EventType.KeyUp){
			keyPressed = true;
		}else if(Event.current.type == EventType.MouseDown){
			mousePressed = true;
			active = true;
		}else if(Event.current.type == EventType.MouseUp){
			mousePressed = true;
		}
		
		if(keyPressed){
			switch(Event.current.keyCode){
				case KeyCode.D:
				case KeyCode.RightArrow:
					rightKey = active;
					break;
				case KeyCode.A:
				case KeyCode.LeftArrow:
					leftKey = active;
					break;
				case KeyCode.W:
				case KeyCode.UpArrow:
					upKey = active;
					break;
				case KeyCode.S:
				case KeyCode.DownArrow:
					downKey = active;
					break;
				case KeyCode.E:
					raiseKey = active;
					break;
				case KeyCode.Q:
					lowerKey = active;
					break;
				case KeyCode.LeftShift:
					sampleKey = active;
					break;
			}
		}else if(mousePressed){
			if(Event.current.button == 0){
				leftMouseDown = active;
			}
			if(Event.current.button == 1){
				rightClickDown = active;
			}
			if(Event.current.button == 2){
				middleMouseDown = active;
			}
		}
		
		if(Event.current.type == EventType.KeyDown){
			switch(Event.current.keyCode)
			{
				case KeyCode.UpArrow:
					if(Event.current.shift)
					{
						MoveLayerUp(_currentPaintLayer);
						Event.current.Use();
					}
					break;
				case KeyCode.DownArrow:
					if(Event.current.shift)
					{
						MoveLayerDown(_currentPaintLayer);
						Event.current.Use();
					}
					break;
				case KeyCode.D:
					if(Event.current.shift)
					{
						DuplicateLayer(_currentPaintLayer);
						Event.current.Use();
					}
					break;
				case KeyCode.R:
					if(Event.current.alt)
					{
						RemoveLayer(_currentPaintLayer);
						Event.current.Use();
					}
					break;
				case KeyCode.E:
					if(Event.current.shift)
					{
						if(Event.current.control)
						{
							MergeAllLayers();
							Event.current.Use();
						}
						else
						{
							MergeLayerDown(_currentPaintLayer);
							Event.current.Use();
						}
					}
					break;
				case KeyCode.N:
					if(Event.current.shift)
					{
						NewLayer();
						Event.current.Use();
					}
					break;
				case KeyCode.M:
					if(Event.current.shift)
					{
						CreateMergedLayer();
						Event.current.Use();
					}
					break;
				case KeyCode.LeftBracket:
					VertexEditorControls.radius = Mathf.Clamp(VertexEditorControls.radius-0.25f, 0, Mathf.Infinity);
					Event.current.Use();
					break;
				case KeyCode.RightBracket:
					VertexEditorControls.radius = Mathf.Clamp(VertexEditorControls.radius+0.25f, 0, 30);
					Event.current.Use();
					break;
				case KeyCode.Minus:
					VertexEditorControls.strength = Mathf.Clamp(VertexEditorControls.strength-1, 0, Mathf.Infinity);
					Event.current.Use();
					break;
				case KeyCode.Equals:
					VertexEditorControls.strength = Mathf.Clamp(VertexEditorControls.strength+1, 0, 100);
					Event.current.Use();
					break;
				case KeyCode.Alpha0:
					VertexEditorControls.falloff = 10f;
					Event.current.Use();
					break;
				case KeyCode.BackQuote:
					VertexEditorControls.falloff = 0f;
					Event.current.Use();
					break;
				case KeyCode.Alpha1:
					VertexEditorControls.falloff = 1f;
					Event.current.Use();
					break;
				case KeyCode.Alpha2:
					VertexEditorControls.falloff = 2f;
					Event.current.Use();
					break;
				case KeyCode.Alpha3:
					VertexEditorControls.falloff = 3f;
					Event.current.Use();
					break;
				case KeyCode.Alpha4:
					VertexEditorControls.falloff = 4f;
					Event.current.Use();
					break;
				case KeyCode.Alpha5:
					VertexEditorControls.falloff = 5f;
					Event.current.Use();
					break;
				case KeyCode.Alpha6:
					VertexEditorControls.falloff = 6f;
					Event.current.Use();
					break;
				case KeyCode.Alpha7:
					VertexEditorControls.falloff = 7f;
					Event.current.Use();
					break;
				case KeyCode.Alpha8:
					VertexEditorControls.falloff = 8f;
					Event.current.Use();
					break;
				case KeyCode.Alpha9:
					VertexEditorControls.falloff = 9f;
					Event.current.Use();
					break;
				case KeyCode.Tab:
					if(Event.current.shift)
					{
						ToggleWireframe();
						Event.current.Use();
					}
					else
					{
						ToggleVertexColorPreview();
						Event.current.Use();
					}
					break;
			}
		}
		
	}
	
}
