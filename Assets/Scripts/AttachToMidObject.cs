using UnityEngine;
using System.Collections;
//using GamepadInput;
using XInputDotNetPure;

public class AttachToMidObject : MonoBehaviour {
	public Controls controls;
	public MidObject midObj; 
	public bool attatched;

	PlayerIndex player1 = PlayerIndex.One;
	PlayerIndex player2 = PlayerIndex.Two;
	PlayerIndex player3 = PlayerIndex.Three;
	PlayerIndex player4 = PlayerIndex.Four;
	public bool shoulderDown;
	public bool pressed;
	public bool released;
	void Start () {
	
	}

	void Update () {
		GamePadState p1 = GamePad.GetState(player1);
		GamePadState p2 = GamePad.GetState(player2);
		GamePadState p3 = GamePad.GetState(player3);
		GamePadState p4 = GamePad.GetState(player4);

//		Debug.Log (p1.Buttons.RightShoulder);
//		if (p1.Buttons.RightShoulder == ButtonState.Pressed && !pressed)
//		{
//			pressed = true;
//		}
//		if (XInputG.buttonADown == true)
//		{
//			Debug.Log ("test");
//		}
//		if(p2.Buttons.RightShoulder == ButtonState.Pressed) shoulderDown = true;
//		else shoulderDown = false;
//		if(p3.Buttons.RightShoulder == ButtonState.Pressed) shoulderDown = true;
//		else shoulderDown = false;
//		if(p4.Buttons.RightShoulder == ButtonState.Pressed) shoulderDown = true;
//		else shoulderDown = false;
		if(!midObj.charging)
		{
			switch(controls.playerNumber)
			{
			case 1:
	//			if(GamePad.GetButtonDown (GamePad.Button.RightShoulder, GamePad.Index.One))

				if (p1.Buttons.RightShoulder == ButtonState.Pressed && !attatched && !pressed)
				{
					if(midObj.target == null)
					{
						attatched = true;				
						midObj.target = this.gameObject;
						pressed = true;
					}
				}
				if (p1.Buttons.RightShoulder == ButtonState.Released)
				{
					pressed = false;
				}
				if(!pressed && attatched)
				{
					if(p1.Buttons.RightShoulder == ButtonState.Pressed && midObj.target == this.gameObject)
					{
						pressed = true;
						attatched = false;
						midObj.target = null;
						midObj.prevTarget = this.gameObject;
					}
				}
	//			if(shoulderDown)
	//			{
	//
	//				if(!attatched)
	//				{
	////					Debug.Log ("=!=!");
	//					if(midObj.target == null)
	//					{
	//						attatched = true;
	//
	//						midObj.target = this.gameObject;
	//
	//					}
	//				}
	//				else
	//				{
	//					if(midObj.target == this.gameObject)
	//					{
	//						midObj.target = null;
	//						attatched = false;
	//						midObj.prevTarget = this.gameObject;
	//					}
	//				}
	//			}
				break;
				
			case 2:

				if (p2.Buttons.RightShoulder == ButtonState.Pressed && !attatched && !pressed)
				{
					if(midObj.target == null)
					{
						attatched = true;				
						midObj.target = this.gameObject;
						pressed = true;
					}
				}
				if (p2.Buttons.RightShoulder == ButtonState.Released)
				{
					pressed = false;
				}
				if(!pressed && attatched)
				{
					if(p2.Buttons.RightShoulder == ButtonState.Pressed && midObj.target == this.gameObject)
					{
						pressed = true;
						attatched = false;
						midObj.target = null;
						midObj.prevTarget = this.gameObject;
					}
				}
				break;
				
			case 3:

				if (p3.Buttons.RightShoulder == ButtonState.Pressed && !attatched && !pressed)
				{
					if(midObj.target == null)
					{
						attatched = true;				
						midObj.target = this.gameObject;
						pressed = true;
					}
				}
				if (p3.Buttons.RightShoulder == ButtonState.Released)
				{
					pressed = false;
				}
				if(!pressed && attatched)
				{
					if(p3.Buttons.RightShoulder == ButtonState.Pressed && midObj.target == this.gameObject)
					{
						pressed = true;
						attatched = false;
						midObj.target = null;
						midObj.prevTarget = this.gameObject;
					}
				}
				break;
				
			case 4:

				if (p4.Buttons.RightShoulder == ButtonState.Pressed && !attatched && !pressed)
				{
					if(midObj.target == null)
					{
						attatched = true;				
						midObj.target = this.gameObject;
						pressed = true;
					}
				}
				if (p4.Buttons.RightShoulder == ButtonState.Released)
				{
					pressed = false;
				}
				if(!pressed && attatched)
				{
					if(p4.Buttons.RightShoulder == ButtonState.Pressed && midObj.target == this.gameObject)
					{
						pressed = true;
						attatched = false;
						midObj.target = null;
						midObj.prevTarget = this.gameObject;
					}
				}
				break;
				
			default:
				break;
				
			}
		}
	}
}
