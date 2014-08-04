using UnityEngine;
using System.Collections;

public class SortMode : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.camera.transparencySortMode = TransparencySortMode.Perspective;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
