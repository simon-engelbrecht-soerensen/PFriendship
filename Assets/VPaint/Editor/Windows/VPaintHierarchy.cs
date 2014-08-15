using UnityEngine;
using UnityEditor;
using Valkyrie.VPaint;

public class VPaintHierarchy : VPaintWindowBase
{
	Vector2 scrollPosition;
	public override void OnValidatedGUI ()
	{
		VPaintGUIUtility.BoxArea(4f, 4f, 1, ()=>
		{
			
			VPaintGUIUtility.BoxArea(0f, 4f, 1, ()=>
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Vertex Colorer");
				GUILayout.FlexibleSpace();
				GUILayout.Label("Enable Painting");
				EditorGUILayout.EndHorizontal();
			});
			
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
			for(int i = 0; i < VPaint.Instance.currentEditingContents.Length; i++)
			{
				var vc = VPaint.Instance.currentEditingContents[i];
				
				VPaintGUIUtility.BoxArea(0f, 4f, 1, ()=>
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Label(vc.name);
					GUILayout.FlexibleSpace();
					VPaint.Instance.currentEditingContentsMask[i] = GUILayout.Toggle(VPaint.Instance.currentEditingContentsMask[i], GUIContent.none);
					GUILayout.Space(5);
					EditorGUILayout.EndHorizontal();
				});
			}
			EditorGUILayout.EndScrollView();
		});
	}
	
	public override bool CloseOnInvalid ()
	{
		return false;
	}
}
