using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

public class Highscores : MonoBehaviour {
	public List<KeyValuePair<int,string>> highscoreListTest = new List<KeyValuePair<int, string>>();
//	public List<int> unsortedHighscoreScoresList;
//	public List<int> sortedHighscoreScoresList;
	public List<int> highscoreScoresList;
	public List<string> highscoreNamesList;
	public int[] highscoreScores;
	public string[] highscoreNames;
	public bool setHighscore;
	public bool getHighscore;
	public int score;
	public float scoreCountSpeed = 3;
	private float scoreFloat;
	public string name;
	public bool dead;
	private bool pauseCount;
	public bool deletePrefs;
	public MidObject midObj;
	void Start () {
		pauseCount = false;
//		highscoreScores = PlayerPrefsX.GetIntArray("Scores");
		highscoreScores = PlayerPrefsX.GetIntArray("HighscoreScores");
		highscoreNames = PlayerPrefsX.GetStringArray("HighscoreNames");
		highscoreScoresList = highscoreScores.ToList();
		highscoreNamesList = highscoreNames.ToList();
//		foreach(int hsS in highscoreScoresList)
//		{
//			foreach(string hsN in highscoreNamesList)
//			{
		for(int i = 0; i< highscoreNamesList.Count; i++)
		{
				highscoreListTest.Add(new KeyValuePair<int, string>(highscoreScoresList[i], highscoreNamesList[i]));
		}
//			}
//		}
//		unsortedHighscoreScoresList = highscoreScores.ToList();
	}
	
	void Update () {
		if(deletePrefs)
		{
			PlayerPrefs.DeleteAll();
		}
		if(!pauseCount)
		{
			scoreFloat = midObj.goldCount;
			score = (int)scoreFloat;
		}
//		if(dead)
//		{
//			setHighscore = true;
//		}
		if(setHighscore)
		{
			pauseCount = true;
			dead = false;
			setHighscore = false;
			SetScores(name, score);
			scoreFloat = 0;
		}
		if(getHighscore)
		{
			getHighscore = false;
//			Debug.Log (highscoreScores.Length);
			
		}
		
	}
	public void SetScores(string name, int score)
	{
		highscoreListTest.Add(new KeyValuePair<int, string>(score, name));
		highscoreListTest.Sort((KeyValuePair<int, string> hslt, KeyValuePair<int, string> hslt2) =>
		{
			return hslt.Key.CompareTo(hslt2.Key);
		});
		highscoreScoresList.RemoveRange(0, highscoreScoresList.Count);
		highscoreNamesList.RemoveRange(0, highscoreNamesList.Count);
		foreach (KeyValuePair<int, string> hslt in highscoreListTest)
		{

				highscoreScoresList.Add(hslt.Key);
				highscoreNamesList.Add(hslt.Value);

			Debug.Log(hslt.Value + ":" +hslt.Key);
		}

//		unsortedHighscoreScoresList.Add(score);

//		sortedHighscoreScoresList = unsortedHighscoreScoresList.OrderBy(i => i).ToList();
//		highscoreListTest.CopyTo(highscoreScores,0);
//		highscoreScores = highscoreListTest
//		highscoreScores.
		highscoreScores = highscoreScoresList.ToArray(); 
		highscoreNames = highscoreNamesList.ToArray();
//		highscoreNames.Reverse();
//		highscoreScores.Reverse();
		PlayerPrefsX.SetIntArray ("HighscoreScores", highscoreScores);
		PlayerPrefsX.SetStringArray ("HighscoreNames", highscoreNames);
	}
	void AddScore(string name, int score){
		
		//		int newScore;
		//		string newName;
		//		int oldScore;
		//		string oldName;
		//		newScore = score;
		//		newName = name;
		
		//		for(int i=0;i<10;i++){
		//			if(PlayerPrefs.HasKey(i+"HScore")){
		//				if(PlayerPrefs.GetInt(i+"HScore")<newScore){ 
		//					// new score is higher than the stored score
		//					oldScore = PlayerPrefs.GetInt(i+"HScore");
		//					oldName = PlayerPrefs.GetString(i+"HScoreName");
		//					PlayerPrefs.SetInt(i+"HScore",newScore);
		//					PlayerPrefs.SetString(i+"HScoreName",newName);
		//					newScore = oldScore;
		//					newName = oldName;
		//				}
		//			}else{
		//				PlayerPrefs.SetInt(i+"HScore",newScore);
		//				PlayerPrefs.SetString(i+"HScoreName",newName);
		//				newScore = 0;
		//				newName = "";
		//			}
		//		}
	}


}
