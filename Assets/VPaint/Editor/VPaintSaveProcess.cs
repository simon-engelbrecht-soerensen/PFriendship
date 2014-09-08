using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class VPaintSaveProcess : 
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_2 && !UNITY_3_3 && !UNITY_3_4 && !UNITY_3_5
	UnityEditor.
#endif
	AssetModificationProcessor 
{
	
	public static string[] OnWillSaveAssets (string[] paths)
	{
		bool isScene = false;
		foreach(var s in paths)
		{
			string ext = Path.GetExtension(s);
			if(ext == ".unity")
			{
				isScene = true;
				break;
			}
		}
		
		if(isScene)
		{
			PrepareScene();
		}
		
		return paths;
	}
	
	static void PrepareScene ()
	{		
		Dictionary<VPaintObject, Color[]> vcsToReset = new Dictionary<VPaintObject, Color[]>();
		var vcs = GameObject.FindObjectsOfType(typeof(VPaintObject));
		foreach(var obj in vcs)
		{
			var vc = obj as VPaintObject;
			if(vc._mesh)
			{
				vcsToReset.Add(vc, vc._mesh.colors);
				vc.ResetInstances();
			}
			if(vc.editorCollider)
			{
				GameObject.DestroyImmediate(vc.editorCollider.gameObject);
			}
		}
		var vertexEditor = VPaint.Instance;
		if(vertexEditor)
		{
			vertexEditor.Cleanup();
			EditorApplication.delayCall += ()=>{
				if(vertexEditor.enabled)
				{
					vertexEditor.ReloadLayers();
					if(vertexEditor.vertexColorPreviewEnabled) 
						vertexEditor.EnableVertexColorPreview();
//					vertexEditor.SetupColliders();
				}
			};
		}
		
		EditorApplication.delayCall += ()=>{
			foreach(var kvp in vcsToReset)
			{
				if(kvp.Key) kvp.Key.SetColors(kvp.Value);
			}
		};
	}
	
}
