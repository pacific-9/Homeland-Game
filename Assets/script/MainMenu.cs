using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public GameObject gameOver;
	public GameObject gameWon;
	public Text scoreBoard;
	
	public List<Character> characters = new List<Character>();

	private void Awake()
	{
		GameManager.gameManager.mainMenu = this;
	}
	public void Update()
	{
		ShowScore();
	}

	public void ShowScore()
	{
	
		scoreBoard.text = "Score Board"+"\n"+
			characters[0].characterName + "\t" + characters[0].areaSize.ToString("0")+ "\n" +
			characters[1].characterName + "\t  " + characters[1].areaSize.ToString("0")+ "\n" +
			characters[2].characterName + "\t  " + characters[2].areaSize.ToString("0")+ "\n" +
			characters[3].characterName + "\t  " + characters[3].areaSize.ToString("0")+ "\n" +
			characters[4].characterName + "\t  " + characters[4].areaSize.ToString("0")+ "\n" 
			;
	}
	public void Retry()
	{
		
		GameManager.gameManager.RestartScene();
	}
	

	public void GameOver()
	{
		gameOver.SetActive(true);
	}
	public void GameWon()
	{
		gameWon.SetActive(true);
	}
	public void Quitgame()
	{
		Application.Quit();
	}

}