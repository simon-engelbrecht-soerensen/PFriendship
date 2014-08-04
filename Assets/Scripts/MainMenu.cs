using UnityEngine;
using System.Collections;
using XInputDotNetPure;

using System.Collections.Generic;
using System.Linq;
public class MainMenu : MonoBehaviour {
	public UITexture startGameUI;
	public UITexture controlsUI;
	public UITexture highscoreUI;
	public UITexture exitUI;
	public UITexture controlsImg;

	public Transform highscoreLabel;
	public GameObject highscoreIndv;

	public Texture2D startGameTex;
	public Texture2D startGameTexSelected;
	public int startGameOrderNr = 1;

	public Texture2D controlsTex;
	public Texture2D controlsTexSelected;
	public int controlsOrderNr = 2;

//	public Texture2D highscoreTex;
//	public Texture2D highscoreTexSelected;
//	public int highscoreOrderNr = 3;

	public Texture2D exitTex;
	public Texture2D exitTexSelected;
	public int exitOrderNr = 4;

	private int curSelected = 1;

	public int maxMenuItems = 4;

	PlayerIndex player1 = PlayerIndex.One;
	private Vector3 inputDir;
	public bool buttonA;

	bool moved;
	private float moveLimit = 0.8f;

	public string spilScene;
	public string highscoreScene;


	public List<int> unsortedHighscoreScoresList;
	public List<int> sortedHighscoreScoresList;
	public int[] highscoreScores = new int[10];
	public string[] highscoreNames = new string[10];



	void Start () {
		controlsImg.enabled = false;
		highscoreScores = PlayerPrefsX.GetIntArray("HighscoreScores");
		highscoreNames = PlayerPrefsX.GetStringArray("HighscoreNames");
//		highscoreScores.Reverse().ToArray();
		System.Array.Reverse(highscoreScores);
		System.Array.Reverse(highscoreNames);

		for(int i = 0; i< highscoreNames.Length; i++) 
		{
//			GameObject playerScore = Instantiate(highscoreIndv,highscoreLabel.transform.position,Quaternion.identity) as GameObject;
			GameObject playerScore = NGUITools.AddChild(this.gameObject, highscoreIndv) as GameObject;
			playerScore.transform.localPosition = new Vector3(highscoreLabel.transform.localPosition.x - 180, (highscoreLabel.transform.localPosition.y - 60) - i*70,0);
			playerScore.GetComponent<UILabel>().text = (i+1) + ". " + highscoreNames[i].ToUpper() + " - " + highscoreScores[i];
		}
	
	}

	void Update () {

		GamePadState p1 = GamePad.GetState(player1);
		inputDir = new Vector3(p1.ThumbSticks.Left.X, p1.ThumbSticks.Left.Y,  0);

		if(p1.Buttons.A == ButtonState.Pressed) buttonA = true;
		else buttonA = false;


		if(inputDir.y > -moveLimit && inputDir.y < moveLimit)//&& inputDir.x > -moveLimit && inputDir.x < moveLimit
		{
			moved = false;
		}

		if(inputDir.y < -moveLimit && !moved)
		{
			//move down
			moved = true;
			if(curSelected < maxMenuItems)
			{
				curSelected += 1;
			}
		}

		if(inputDir.y > moveLimit && !moved)
		{
			moved = true;
			if(curSelected > 1)
			{
				curSelected -= 1;
			}
		}




		if(curSelected == startGameOrderNr)
		{
			startGameUI.mainTexture = startGameTexSelected;
			if(buttonA)
			{
				Application.LoadLevel(spilScene);
			}
		}
		else
		{
			startGameUI.mainTexture = startGameTex;
		}

		if(curSelected == controlsOrderNr)
		{
			controlsUI.mainTexture = controlsTexSelected;
			if(buttonA)
			{
//				Application.LoadLevel(tutorialScene);
				controlsImg.enabled = true;

			}
		}
		else
		{
			controlsImg.enabled = false;
			controlsUI.mainTexture = controlsTex;
		}

//		if(curSelected == highscoreOrderNr)
//		{
//			highscoreUI.mainTexture = highscoreTexSelected;
//			if(buttonA)
//			{
//				Application.LoadLevel(highscoreScene);
//			}
//		}
//		else
//		{
//			highscoreUI.mainTexture = highscoreTex;
//		}

		if(curSelected == exitOrderNr)
		{
			exitUI.mainTexture = exitTexSelected;
			if(buttonA)
			{
				Application.Quit();
			}
		}
		else
		{
			exitUI.mainTexture = exitTex;
		}



	}
//	void OnGUI()
//	{ 
		
//		for(int i = 0; i< highscoreNames.Length; i++) 
//		{
//			Instantiate
////			GUI.Label(new Rect(Screen.width/2 -100, Screen.height/2-50 +i*15, 200, 100),highscoreScores[i].ToString());
////			GUI.Label(new Rect(Screen.width/2 -200, Screen.height/2-50+i*15, 200, 100),highscoreNames[i].ToString());
//		}
		
//	}
}
