using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class StartScene : EditorWindow {
	static Object midObj;
//	static List<Object> players;
	static Object player1;
	static Object player2;
	static Object player3;
	static Object player4;
	static Object respawnPlayers;
	static Object camera;
	static Object decalObj;

	[MenuItem ("Window/Scene Setup")]
	static void Init () 
	{
		StartScene window = (StartScene)EditorWindow.GetWindow (typeof (StartScene));
		decalObj = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Bloodsplatter/decalGO.prefab", typeof(GameObject));
		midObj = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/NewSceneNeeded/MidObject.prefab", typeof(GameObject)); 
		player1 = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/NewSceneNeeded/Player1.prefab", typeof(GameObject));
		player2 = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/NewSceneNeeded/Player2.prefab", typeof(GameObject));
		player3 = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/NewSceneNeeded/Player3.prefab", typeof(GameObject));
		player4 = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/NewSceneNeeded/Player4.prefab", typeof(GameObject));
		respawnPlayers = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/NewSceneNeeded/RespawnPlayers.prefab", typeof(GameObject));
		camera = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/NewSceneNeeded/Main Camera.prefab", typeof(GameObject));
	}

	void OnGUI () 
	{
		if(GUILayout.Button("Make Scene Work")) {
			GameObject DecalObjInst = PrefabUtility.InstantiatePrefab(decalObj as GameObject) as GameObject;
			DecalObjInst.name = "decalGO";

			GameObject RespawnPlayersInst = PrefabUtility.InstantiatePrefab(respawnPlayers as GameObject) as GameObject;
			RespawnPlayersInst.name = "RespawnPlayers";

			GameObject MidObjectInst = PrefabUtility.InstantiatePrefab(midObj as GameObject) as GameObject;
			MidObjectInst.name = "MidObject";

			GameObject Player1Inst = PrefabUtility.InstantiatePrefab(player1 as GameObject) as GameObject;
			Player1Inst.name = "Player1"; 
			Player1Inst.GetComponent<AttachToMidObject>().midObj = MidObjectInst.GetComponent<MidObject>(); 
			Player1Inst.GetComponent<PlayerStats>().playerDeath = RespawnPlayersInst.GetComponent<PlayerDeath>();

			GameObject Player2Inst = PrefabUtility.InstantiatePrefab(player2 as GameObject) as GameObject;
			Player2Inst.name = "Player2";
			Player2Inst.GetComponent<AttachToMidObject>().midObj = MidObjectInst.GetComponent<MidObject>(); 
			Player2Inst.GetComponent<PlayerStats>().playerDeath = RespawnPlayersInst.GetComponent<PlayerDeath>();

			GameObject Player3Inst = PrefabUtility.InstantiatePrefab(player3 as GameObject) as GameObject;
			Player3Inst.name = "Player3";
			Player3Inst.GetComponent<AttachToMidObject>().midObj = MidObjectInst.GetComponent<MidObject>(); 
			Player3Inst.GetComponent<PlayerStats>().playerDeath = RespawnPlayersInst.GetComponent<PlayerDeath>();

			GameObject Player4Inst = PrefabUtility.InstantiatePrefab(player4 as GameObject) as GameObject;
			Player4Inst.name = "Player4";
			Player4Inst.GetComponent<AttachToMidObject>().midObj = MidObjectInst.GetComponent<MidObject>(); 
			Player4Inst.GetComponent<PlayerStats>().playerDeath = RespawnPlayersInst.GetComponent<PlayerDeath>();

			GameObject MainCamInst = PrefabUtility.InstantiatePrefab(camera as GameObject) as GameObject;
			MainCamInst.name = "Main Camera";
//			MainCamInst.GetComponent<FollowTargets>().targets


		}
	}

}
