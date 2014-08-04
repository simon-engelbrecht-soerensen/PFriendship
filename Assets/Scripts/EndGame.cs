using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;

public class EndGame : MonoBehaviour {
	public int playersExited;
	public bool midExited;
	public int playersToExit = 4;
	public FollowTargets followTargets;
	public bool gameEnd;
	public float minimumGold = 1000f;
	public GameObject midObject;
	public bool tutorial;
//	public string tutorialName;
	public UILabel endText1;
	public UILabel endText2;

	public Highscores highscores;

	PlayerIndex player1 = PlayerIndex.One;
	private Vector3 inputDir;
	public bool buttonA;
	string stringToEdit = "Enter name here!";

	private bool nextable = false;
	private bool nextScreen = false;
	void Start () {
		midObject = GameObject.Find("MidObject");
		highscores = midObject.GetComponent<Highscores>();
		followTargets = Camera.main.GetComponent<FollowTargets>();
		endText1.enabled = false;
		endText2.enabled = false;
	}

	void Update () {
		GamePadState p1 = GamePad.GetState(player1);
		inputDir = new Vector3(p1.ThumbSticks.Left.X, p1.ThumbSticks.Left.Y,  0);
		// tag for højde at der kan være døde

		if(nextable)
		{
					if(p1.Buttons.A == ButtonState.Pressed)
					{
//						buttonA = true;
//						Debug.Log ("TEST");
						nextable = false;
						highscores.SetScores(stringToEdit, (int)midObject.GetComponent<MidObject>().goldCount);
						Application.LoadLevel("MainMenu");
					}
					
		}
		if(gameEnd)
		{
//			Time.timeScale = 0;
			endText1.enabled = true;
			endText2.enabled = true;
			endText2.text = midObject.GetComponent<MidObject>().goldCount.ToString();
			if(!nextScreen)
			{
				nextScreen = true;
				StartCoroutine(NextScreen());
			}
		}
	}

	IEnumerator OnTriggerEnter(Collider col)
	{
		if(tutorial)
		{
			if(col.gameObject.tag == "Player")
			{
				if(!col.gameObject.GetComponent<Movement>().exited)
				{
					
					playersExited += 1;
					col.gameObject.GetComponent<Movement>().StartCoroutine("ExitWorld");
					if(playersExited < playersToExit)
					{
						followTargets.targets.Remove(col.gameObject.transform);
					}
					else
					{
						yield return new WaitForSeconds(2);
//						Application.LoadLevel(tutorialName);
//						gameEnd = true;
					}
					//				
				}
			}

		}
		if(col.gameObject.tag == "Player")
		{
//			Debug.Log ("ENTERED");
			if(midObject.GetComponent<MidObject>().goldCount > minimumGold)
			{
				if(!col.gameObject.GetComponent<Movement>().exited)
				{

					playersExited += 1;
					col.gameObject.GetComponent<Movement>().StartCoroutine("ExitWorld");
					if(playersExited < playersToExit)
					{
						followTargets.targets.Remove(col.gameObject.transform);
					}
					else
					{
						yield return new WaitForSeconds(2);
						gameEnd = true;
					}
	//				
				}
			}

		}
		if(col.gameObject.name == "MidObject")
		{
			midExited = true;
		}
	}

	void OnGUI()
	{
		if(gameEnd)
		{	
//			Time.timeScale = 0;
		


			stringToEdit = GUI.TextField (new Rect (Screen.width/2 - 100, Screen.height - 250, 200, 20),stringToEdit, 25);
//			if(GUI.Button(new Rect(100,100,100,100), "restart"))
//			{	
//				Application.LoadLevel(Application.loadedLevel);
//				gameEnd = false;
//				Time.timeScale = 1;
//			}
		}
	}

	IEnumerator NextScreen()
	{
		yield return new WaitForSeconds(1);
		nextable = true;

	}
}
