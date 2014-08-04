using UnityEditor;
using UnityEngine;


public class TestWindow : EditorWindow
{
	public static GameObject[] players;
	[MenuItem ("Window/Editor Window Test")]
	static void Init () 
	{
//		players[0] = GameObject.Find("Player1");
//		players[1] = GameObject.Find("Player2");
//		players[2] = GameObject.Find("Player3");
//		players[3] = GameObject.Find("Player4");

		// Get existing open window or if none, make a new one:
		TestWindow window = (TestWindow)EditorWindow.GetWindow (typeof (TestWindow));
	}
	void OnInspectorUpdate() {
		// Call Repaint on OnInspectorUpdate as it repaints the windows
		// less times as if it was OnGUI/Update
		Repaint();
	}	
	void OnGUI () {

		GameObject sel = Selection.activeGameObject;
//		if(sel.tag == "Player")
//		{
			Movement targetComp = sel.GetComponent<Movement>();
			MeleeAttack targetComp2 = sel.transform.GetComponentInChildren<MeleeAttack>();
			if (targetComp != null)
			{
				Editor editor = Editor.CreateEditor(targetComp);
//				editor.OnInspectorGUI();
				targetComp.drag = EditorGUILayout.FloatField("Drag",targetComp.drag);
				
//				editor.DrawDefaultInspector();
				EditorGUILayout.HelpBox("This is a help box", MessageType.Info);
				Editor editor2 = Editor.CreateEditor(targetComp2);
				editor2.OnInspectorGUI();  
			}
//		}
		

		
	}
}