using UnityEngine;
using System.Collections;

public class GoldEffect : MonoBehaviour {
	private Color startColor;
	public bool flash;
	public float flashSpeed = 10;

	void Start () {
		startColor = this.renderer.material.color;
		StartCoroutine("RandomStart");
	}
	

	void Update () {
//		if(flash)
//		{
//			StartCoroutine("FlashWhite");
//		}
	}
	IEnumerator RandomStart()
	{
		float timeBetween = Random.Range(3,15);

		float mTime = 0; 
		float counter = 0;
		bool countUp;
		while(true)
		{
			counter += Time.deltaTime * 1f;
	//		if(mTime < 1)
	//		{
	//			mTime += Time.deltaTime * 1;
	//		}
			if(counter > timeBetween)
			{
				counter = 0;
				timeBetween = Random.Range(1,6);
				flashSpeed = Random.Range(1f,3f);
				StartCoroutine("FlashWhite");
				flash = true;

			}


			yield return null;
		}


			
		
	}
	IEnumerator FlashWhite()
	{
		float mTime = 0;
		bool goneWhite = false;
//		flash = true;
		while(flash)
		{
			if(!goneWhite)
			{
				if(mTime < 1)
				{
					mTime += Time.deltaTime * flashSpeed;
					this.renderer.material.color = Color.Lerp(startColor, Color.white, mTime);
				}
				else
				{
					goneWhite = true;
					mTime = 0;
				}
			}
			if(goneWhite)
			{
				if(mTime < 1)
				{
					mTime += Time.deltaTime * flashSpeed;
					this.renderer.material.color = Color.Lerp(Color.white, startColor, mTime);
				}
				else
				{
					flash = false;
					mTime = 0;
				}
			}
			yield return null;
		}


	}
}
