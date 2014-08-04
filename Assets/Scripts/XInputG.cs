using UnityEngine;
using System.Collections;
using XInputDotNetPure;
public class XInputG : MonoBehaviour {

	bool playerIndexSet = false;

	public static PlayerIndex playerIndex;
	public static 	GamePadState state;
	public static GamePadState prevState;

	// Static variables for 
	
	public static  float h1 = 0.0f;	
	public static 	float v1 = 0.0f;	
	
	public static  float h2 = 0.0f;	
	public static  float v2 = 0.0f;	
	
	public static bool buttonA = false;	
	public static bool buttonB = false;	
	public static bool buttonX = false;	
	public static bool buttonY = false;	
	
	public static bool buttonADown = false;
	public static bool buttonBDown = false;
	public static bool buttonXDown = false;
	public static bool buttonYDown = false;	
	

	public static bool buttonAUp = false;
	public static bool buttonBUp = false;
	public static bool buttonXUp = false;
	public static bool buttonYUp = false;
	

	public static bool dpadUp = false;
	public static bool dpadDown = false;
	public static bool dpadLeft = false;
	public static bool dpadRight = false;	
	

	public static bool dpadUpDown = false;
	public static bool dpadDownDown = false;
	public static bool dpadLeftDown = false;
	public static bool dpadRightDown = false;


	public static bool dpadUpUp = false;
	public static bool dpadDownUp = false;
	public static bool dpadLeftUp = false;
	public static bool dpadRightUp = false;
	

	public static bool buttonStart = false;
	public static bool buttonBack = false;
	

	public static bool buttonStartDown = false;
	public static bool buttonBackDown = false;
	

	public static bool buttonStartUp = false;
	public static bool buttonBackUp = false;
	

	public static bool shoulderL = false;
	public static bool shoulderR = false;
	

	static bool shoulderLDown = false;
	static bool shoulderRDown = false;


	public static bool shoulderLUp = false;
	public static bool shoulderRUp = false;


	public static bool stickL = false;
	public static bool stickR = false;
	

	public static bool stickLDown = false;
	public static bool stickRDown = false;
	

	public static bool stickLUp = false;
	public static bool stickRUp = false;
	
	
	public static float triggerL = 0.0f;	
	public static float triggerR = 0.0f;
	
	
	
	void Update () {		
		// Find a PlayerIndex, for a single player game		
		if ( !playerIndexSet ) {			
			for ( int i = 0; i < 4; ++i ) {

				PlayerIndex testPlayerIndex = PlayerIndex.One;
				switch ( i ) {
					
				case 0:					
					testPlayerIndex = PlayerIndex.One;					
					break;					
				case 1:					
					testPlayerIndex = PlayerIndex.Two;					
					break;					
				case 2:					
					testPlayerIndex = PlayerIndex.Three;					
					break;					
				case 3:					
					testPlayerIndex = PlayerIndex.Four;					
					break;					
				}
				

				GamePadState testState = GamePad.GetState(testPlayerIndex);
				if ( testState.IsConnected ) {					
					Debug.Log ( string.Format ( "GamePad found {0}", testPlayerIndex ) );					
					playerIndex = testPlayerIndex;					
					playerIndexSet = true;					
				}	
//				else {
//					Debug.Log ("no joystick :-(");
//				}
			}			
		}
		
		
		
		prevState = state;		
		state = GamePad.GetState ( playerIndex );		
		
		
		h1 = state.ThumbSticks.Left.X;		
		v1 = state.ThumbSticks.Left.Y;		
		
		
		h2 = state.ThumbSticks.Right.X;		
		v2 = state.ThumbSticks.Right.Y;		
		
		
		buttonA = ( state.Buttons.A == ButtonState.Pressed );		
		buttonB = ( state.Buttons.B == ButtonState.Pressed );		
		buttonX = ( state.Buttons.X == ButtonState.Pressed );		
		buttonY = ( state.Buttons.Y == ButtonState.Pressed );		
		
		
		buttonADown = ( buttonA && prevState.Buttons.A != ButtonState.Pressed );		
		buttonBDown = ( buttonB && prevState.Buttons.B != ButtonState.Pressed );		
		buttonXDown = ( buttonX && prevState.Buttons.X != ButtonState.Pressed );		
		buttonYDown = ( buttonY && prevState.Buttons.Y != ButtonState.Pressed );		
		
		
		buttonAUp = ( !buttonA && prevState.Buttons.A == ButtonState.Pressed );		
		buttonBUp = ( !buttonB && prevState.Buttons.B == ButtonState.Pressed );		
		buttonXUp = ( !buttonX && prevState.Buttons.X == ButtonState.Pressed );		
		buttonYUp = ( !buttonY && prevState.Buttons.Y == ButtonState.Pressed );		
		
		
		dpadUp = ( state.DPad.Up == ButtonState.Pressed );		
		dpadDown = ( state.DPad.Down == ButtonState.Pressed );		
		dpadLeft = ( state.DPad.Left == ButtonState.Pressed );		
		dpadRight = ( state.DPad.Right == ButtonState.Pressed );		
		
		
		dpadUpDown = ( dpadUp && prevState.DPad.Up != ButtonState.Pressed );		
		dpadDownDown = ( dpadDown && prevState.DPad.Down != ButtonState.Pressed );		
		dpadLeftDown = ( dpadLeft && prevState.DPad.Left != ButtonState.Pressed );		
		dpadRightDown = ( dpadRight && prevState.DPad.Right != ButtonState.Pressed );		
		
		
		dpadUpUp = ( !dpadUp && prevState.DPad.Up == ButtonState.Pressed );		
		dpadDownUp = ( !dpadDown && prevState.DPad.Down == ButtonState.Pressed );		
		dpadLeftUp = ( !dpadLeft && prevState.DPad.Left == ButtonState.Pressed );		
		dpadRightUp = ( !dpadRight && prevState.DPad.Right == ButtonState.Pressed );		
		
		
		buttonStart = ( state.Buttons.Start == ButtonState.Pressed );		
		buttonBack = ( state.Buttons.Back == ButtonState.Pressed );		
		
		
		buttonStartDown = ( buttonStart && prevState.Buttons.Start != ButtonState.Pressed );		
		buttonBackDown = ( buttonBack && prevState.Buttons.Back != ButtonState.Pressed );		
		
		
		buttonStartUp = ( !buttonStart && prevState.Buttons.Start == ButtonState.Pressed );		
		buttonBackUp = ( !buttonBack && prevState.Buttons.Back == ButtonState.Pressed );		
		
		
		shoulderL = ( state.Buttons.LeftShoulder == ButtonState.Pressed );		
		shoulderR = ( state.Buttons.RightShoulder == ButtonState.Pressed );		
		
		
		shoulderLDown = ( shoulderL && prevState.Buttons.LeftShoulder != ButtonState.Pressed );		
		shoulderRDown = ( shoulderR && prevState.Buttons.RightShoulder != ButtonState.Pressed );		
		
		
		shoulderLUp = ( !shoulderL && prevState.Buttons.LeftShoulder == ButtonState.Pressed );		
		shoulderRUp = ( !shoulderR && prevState.Buttons.RightShoulder == ButtonState.Pressed );		
		
		
		stickL = ( state.Buttons.LeftStick == ButtonState.Pressed );		
		stickR = ( state.Buttons.RightStick == ButtonState.Pressed );		
		
		
		stickLDown = ( stickL && prevState.Buttons.LeftStick != ButtonState.Pressed );		
		stickRDown = ( stickR && prevState.Buttons.RightStick != ButtonState.Pressed );		
		
		
		stickLUp = ( !stickL && prevState.Buttons.LeftStick == ButtonState.Pressed );		
		stickRUp = ( !stickR && prevState.Buttons.RightStick == ButtonState.Pressed );		
		
		
		triggerL = state.Triggers.Left;		
		triggerR = state.Triggers.Right;		
	}
	
	
	
	static void padVibration (float big, float small ) {		
		GamePad.SetVibration ( XInputG.playerIndex, big, small );		
	}
	
	
	
	static void stopPadVibration () {		
		GamePad.SetVibration( XInputG.playerIndex, 0, 0 );		
	}
}
