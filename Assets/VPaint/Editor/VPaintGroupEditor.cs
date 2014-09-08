using UnityEngine;
using UnityEditor;
using Valkyrie.VPaint;

[CustomEditor(typeof(VPaintGroup))]
public class VPaintGroupEditor : Editor 
{
	public override void OnInspectorGUI ()
	{
		var vpg  = target as VPaintGroup;
		
		int totalColors = 0;
		int totalColorers = 0;
		foreach(var layer in vpg.layerStack.layers)
		{
			foreach(var data in layer.paintData)
			{
				totalColors += data.colors.Length;
				totalColorers ++;
			}
		}
		
		EditorGUIUtility.LookLikeInspector();
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Bytes: " + EditorUtility.FormatBytes(totalColors * sizeof(float) * 5));
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Open VPaint"))
		{
			VPaint.OpenEditor();
		}
		EditorGUILayout.EndHorizontal();
		DrawDefaultInspector();
	}
}
