var mySkin : GUISkin;
var effect01 : GameObject;
var effect02 : GameObject;
var effect03 : GameObject;
var effect04 : GameObject;
var effect05 : GameObject;
var effect06 : GameObject;
var effect07 : GameObject;
var effect08 : GameObject;
var effect09 : GameObject;
var effect10 : GameObject;
var effect11 : GameObject;
var effect12 : GameObject;
var effect13 : GameObject;
var effect14 : GameObject;
var effect15 : GameObject;
var effect16 : GameObject;
var effect17 : GameObject;
var effect18 : GameObject;
var effect19 : GameObject;
var effect20 : GameObject;
var effect21 : GameObject;
var effect22 : GameObject;
var effect23 : GameObject;
var effect24 : GameObject;
var effect25 : GameObject;
var effect26 : GameObject;
var effect27 : GameObject;
var effect28 : GameObject;
var effect29 : GameObject;
var effect30 : GameObject;


function OnGUI ()
{
	GUI.skin = mySkin;
	
	GUI.Label (Rect (70,10,100,20), "FT CartoonEffect Volume01");

	if(GUI.Button (Rect (10,40,20,20), GUIContent ("", "AuraStream --- With 4 color pattern")))
	{	Instantiate(effect01, new Vector3(0, 0.1, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (40,40,20,20), GUIContent ("", "AuraWave --- With 4 color pattern")))
	{	Instantiate(effect02, new Vector3(0, 0.1, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (70,40,20,20), GUIContent ("", "ChargeEnd --- With 4 color pattern")))
	{	Instantiate(effect03, new Vector3(0, 1.5, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (100,40,20,20), GUIContent ("", "Coin")))
	{	Instantiate(effect04, new Vector3(0, 1.5, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (130,40,20,20), GUIContent ("", "DarkBall --- With 4 color pattern")))
	{	Instantiate(effect05, new Vector3(0, 1.5, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (160,40,20,20), GUIContent ("", "DarkDome --- With 4 color pattern")))
	{	Instantiate(effect06, new Vector3(0, 0.1, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (190,40,20,20), GUIContent ("", "Dash01")))
	{	Instantiate(effect07, new Vector3(0, 0.1, 0), Quaternion.Euler(0, 0, 0));	}
	
	if(GUI.Button (Rect (10,70,20,20), GUIContent ("", "EnergyBall --- With 4 color pattern")))
	{	Instantiate(effect08, new Vector3(0, 1.5, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (40,70,20,20), GUIContent ("", "FireCircle --- With 4 color pattern")))
	{	Instantiate(effect09, new Vector3(0, 0.1, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (70,70,20,20), GUIContent ("", "FireCylinder --- With 4 color pattern")))
	{	Instantiate(effect10, new Vector3(0, 0.1, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (100,70,20,20), GUIContent ("", "GroundBurst01 --- With 4 color pattern")))
	{	Instantiate(effect11, new Vector3(0, 0.1, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (130,70,20,20), GUIContent ("", "GroundBurst02 --- With 4 color pattern")))
	{	Instantiate(effect12, new Vector3(0, 0.1, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (160,70,20,20), GUIContent ("", "GroundFire01")))
	{	Instantiate(effect13, new Vector3(0, 0.1, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (190,70,20,20), GUIContent ("", "Guard01 --- With 4 color pattern")))
	{	Instantiate(effect14, new Vector3(0, 1.5, 0), Quaternion.Euler(0, 0, 0));	}

	if(GUI.Button (Rect (10,100,20,20), GUIContent ("", "Guard02 --- With 4 color pattern")))
	{	Instantiate(effect15, new Vector3(0, 1.5, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (40,100,20,20), GUIContent ("", "Guard03 --- With 4 color pattern")))
	{	Instantiate(effect16, new Vector3(0, 1.5, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (70,100,20,20), GUIContent ("", "Hit01 --- With 4 color pattern")))
	{	Instantiate(effect17, new Vector3(0, 1.5, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (100,100,20,20), GUIContent ("", "Hit02 --- With 4 color pattern")))
	{	Instantiate(effect18, new Vector3(0, 1.5, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (130,100,20,20), GUIContent ("", "Hit03 --- With 4 color pattern")))
	{	Instantiate(effect19, new Vector3(0, 1.5, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (160,100,20,20), GUIContent ("", "Landing01")))
	{	Instantiate(effect20, new Vector3(0, 0.1, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (190,100,20,20), GUIContent ("", "Landing02")))
	{	Instantiate(effect21, new Vector3(0, 0.1, 0), Quaternion.Euler(0, 0, 0));	}

	if(GUI.Button (Rect (10,130,20,20), GUIContent ("", "Landing03")))
	{	Instantiate(effect22, new Vector3(0, 0.1, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (40,130,20,20), GUIContent ("", "Lightning01 --- With 4 color pattern")))
	{	Instantiate(effect23, new Vector3(0, 0.1, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (70,130,20,20), GUIContent ("", "MuzzleFlash01 --- With 4 color pattern")))
	{	Instantiate(effect24, new Vector3(0, 1.5, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (100,130,20,20), GUIContent ("", "Shot01 --- With 4 color pattern")))
	{	Instantiate(effect25, new Vector3(0, 1.5, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (130,130,20,20), GUIContent ("", "Shot02 --- With 4 color pattern")))
	{	Instantiate(effect26, new Vector3(0, 1.5, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (160,130,20,20), GUIContent ("", "Sonic01 --- With 4 color pattern")))
	{	Instantiate(effect27, new Vector3(0, 1.5, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (190,130,20,20), GUIContent ("", "StormAura --- With 4 color pattern")))
	{	Instantiate(effect28, new Vector3(0, 0.1, 0), Quaternion.Euler(0, 0, 0));	}
	
	if(GUI.Button (Rect (10,160,20,20), GUIContent ("", "StormSphere --- With 4 color pattern")))
	{	Instantiate(effect29, new Vector3(0, 1.5, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (40,160,20,20), GUIContent ("", "TorchFire01")))
	{	Instantiate(effect30, new Vector3(0, 1.5, 0), Quaternion.Euler(0, 0, 0));	}

	
	GUI.Label (Rect (10,Screen.height-30,300,20), GUI.tooltip);
	
}