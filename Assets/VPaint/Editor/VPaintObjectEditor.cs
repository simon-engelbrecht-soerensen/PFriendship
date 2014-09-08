using UnityEngine;
using System.Collections;
using UnityEditor;
using Valkyrie.VPaint;

[CustomEditor(typeof(VPaintObject))]
[CanEditMultipleObjects()]
public class VPaintObjectEditor : Editor 
{
	public override void OnInspectorGUI ()
	{
		EditorGUIUtility.LookLikeInspector();
		DrawDefaultInspector();
		
		var vcol = (target as VPaintObject);
		
		var mr = vcol.GetComponent<MeshRenderer>();
		
		var mat = vcol.originalMaterial;
		if(!mat) mat = mr.sharedMaterial;
		
		bool supportsVertexColors = true;
		if(!mat
		|| !VPaint.SupportsColors(mat.shader)) 
		{
			supportsVertexColors = false;
		}
		if(!supportsVertexColors)
		{
			GUILayout.Space(4);
			Rect r = EditorGUILayout.BeginVertical();
			r.width+=4;
			r.x-=2;
			r.height+=4;
			r.y-=2;
			GUI.Box(r, GUIContent.none);
			GUILayout.Space(4);
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.wordWrap = true;
			style.normal.textColor = Color.black;
			style.fontSize = 12;
			GUILayout.Label("The material assigned to this object does not support vertex colors!", style);
			GUILayout.Space(4);
			GUILayout.Label("Assign a shader which supports vertex colors to display colors painted on this object.", style);
			GUILayout.Space(4);
			EditorGUILayout.EndVertical();
		}
		
		foreach(var obj in targets)
		{
			var vc = obj as VPaintObject;
			var mf = vc.GetComponent<MeshFilter>();
			if(!mf) continue;
			if(mf.sharedMesh != vc.originalMesh
			&& mf.sharedMesh != vc._mesh
			&& mf.sharedMesh != vc._meshNonSerialized)
			{
				Undo.RegisterUndo(new Object[]{vc, mf}, "Change Mesh");
				var m = mf.sharedMesh;
				vc.ResetInstances();
				vc.originalMesh = m;
				mf.sharedMesh = m;
			}
		}
	}
}
