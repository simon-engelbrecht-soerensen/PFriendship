using UnityEngine;
using System.Collections;
using XInputDotNetPure;
public class PauseMenu : MonoBehaviour 
{
	public bool paused;

	PlayerIndex player1 = PlayerIndex.One;
	private Vector3 inputDir;
	public bool buttonA;	
	bool moved;
	private float moveLimit = 0.8f;

	public bool pressed;

	private int curSelected = 1;	
	public int maxMenuItems = 2;

	public UITexture controlsImg;

	public UITexture exitUI;
	public UITexture restartUI;

	public Texture2D exitTex;
	public Texture2D exitTexSelected;

	public Texture2D restartTex;
	public Texture2D restartTexSelected;

	private int restartNr = 1;
	private int exitNr = 2;
	void Start () 
	{
		paused = false;
		controlsImg.enabled = false;
		exitUI.enabled = false;
		restartUI.enabled = false;
	}

	void Update () 
	{		
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
//		GamePadState p2 = GamePad.GetState(player2);
//		GamePadState p3 = GamePad.GetState(player3);
//		GamePadState p4 = GamePad.GetState(player4);
//		if(p1.Buttons.Start == ButtonState.Released || p2.Buttons.Start == ButtonState.Released || p3.Buttons.Start == ButtonState.Released || p4.Buttons.Start == ButtonState.Released && pressed == 1)
//		{
//			pressed = 2;
//		}
		if(!paused)
		{
			controlsImg.enabled = false;
			exitUI.enabled = false;
			restartUI.enabled = false;
			if(p1.Buttons.Start == ButtonState.Released)
			{
				pressed = false;
			}
			if(!pressed)
			{
				if(p1.Buttons.Start == ButtonState.Pressed)
				{
					pressed = true;
					if(pressed)
					{
	//					pressed = false;
						togglePause();
						Debug.Log ("pause");
						paused = true;
					}
		//			paused = !paused;


				}
			}
		}

		if(paused)
		{
			controlsImg.enabled = true;
			exitUI.enabled = true;
			restartUI.enabled = true;


			if(curSelected == restartNr)
			{
				restartUI.mainTexture = restartTexSelected;
				if(buttonA)
				{
					Application.LoadLevel(Application.loadedLevel);
					paused = false;
					Time.timeScale = 1f;
				}
			}
			else
			{
				restartUI.mainTexture = restartTex;
			}
			if(curSelected == exitNr)
			{
				exitUI.mainTexture = exitTexSelected;
				if(buttonA)
				{
					Application.LoadLevel("MainMenu");
//					Application.Quit();
				}
			}
			else
			{
				exitUI.mainTexture = exitTex;
			}



			if(p1.Buttons.Start == ButtonState.Released)
			{
				pressed = false;
			}
			if(!pressed)
			{
				if(p1.Buttons.Start == ButtonState.Pressed)
				{
					pressed = true;
					if(pressed)
					{
						//					pressed = false;
						togglePause();
						Debug.Log ("pause");
						paused = false;
					}
				}
			}
		}
//		if(p1.Buttons.Start == ButtonState.Pressed || p2.Buttons.Start == ButtonState.Pressed || p3.Buttons.Start == ButtonState.Pressed || p4.Buttons.Start == ButtonState.Pressed && pressed == 2)
//		{
//			pressed = 0;
//
//			paused = false;
//			Time.timeScale = 1;
//		}

		
	}

	bool togglePause()
	{
		if(Time.timeScale == 0f)
		{
			Time.timeScale = 1f;
			return(false);
		}
		else
		{
			Time.timeScale = 0f;
			return(true);    
		}
	}
}

