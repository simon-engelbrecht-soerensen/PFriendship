using UnityEditor;
using UnityEngine;
using Valkyrie.VPaint;

public class VPaintLockEditorWindow : VPaintWindowBase
{
	public override bool LockSelection ()
	{
		return true;
	}
	public override bool OverrideTool ()
	{
		 return true;
	}
	public override void OnValidatedGUI ()
	{
		if(GUILayout.Button("Unlock"))
		{
			EditorApplication.delayCall += Close;
		}
		minSize = new Vector2(100, 40);
		maxSize = minSize;
	}
}
