using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
	public static GameManager gameManager;
	public MainMenu mainMenu;
	public int winPoint=0;
	

	private void Awake()
	{
		if (!gameManager)
		{
			
			gameManager = this;
			DontDestroyOnLoad(this);
		}
		mainMenu.Update();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{			
			RestartScene();	
			
		}
		if (Input.GetKeyDown("escape"))
			
		{
			Application.Quit();

		}
		if (winPoint == 4)
		{
			//Debug.Log("game won");
			GameManager.gameManager.GameWon();
		}
		if (Input.GetKeyDown(KeyCode.Z))

		{	
			
				
				Time.timeScale = 0;
			
	     }
		if (Input.GetKeyDown(KeyCode.X))
		{
			
			Time.timeScale = 1f;
		}

	}

	public void RestartScene()
	{
		
		winPoint = 0;
		//Debug.Log(winPoint);
		Time.timeScale = 1f;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void GameOver()
	{
		Time.timeScale = 0;
		mainMenu.GameOver();
	}
	public void GameWon()
	{
		Time.timeScale = 0;
		mainMenu.GameWon();
	}

	public static void ChangeCharacterArea(Character character, List<Vector3> newAreaVertices)
	{
		int newAreaVerticesCount = newAreaVertices.Count;
		if (newAreaVerticesCount > 0)
		{
			List<Vector3> areaVertices = character.areaVertices;
			int startPoint = character.GetClosestAreaVertice(newAreaVertices[0]);
			int endPoint = character.GetClosestAreaVertice(newAreaVertices[newAreaVerticesCount - 1]);

	
			// Selecting waste vertices for clockwise area
			List<Vector3> wasteVertices = new List<Vector3>();
			for (int i = startPoint; i != endPoint; i++)
			{
				if (i == areaVertices.Count)
				{
					if (endPoint == 0)
					{
						break;
					}

					i = 0;
				}
				wasteVertices.Add(areaVertices[i]);
			}
			wasteVertices.Add(areaVertices[endPoint]);

			// Add new vertices to rough area
			List<Vector3> roughAreaClock = new List<Vector3>(areaVertices);
			for (int i = 0; i < newAreaVerticesCount; i++)
			{
				roughAreaClock.Insert(i + startPoint, newAreaVertices[i]);
			}

			// remove waste vertices & calculate clockwise area size
			roughAreaClock = roughAreaClock.Except(wasteVertices).ToList();
			float clockwiseArea = Mathf.Abs(roughAreaClock.Take(roughAreaClock.Count - 1).Select((p, i) => (roughAreaClock[i + 1].x - p.x) * (roughAreaClock[i + 1].z + p.z)).Sum() / 2f);

			// Selecting waste vertices for anticlockwise area
			wasteVertices.Clear();
			for (int i = startPoint; i != endPoint; i--)
			{
				if (i == -1)
				{
					if (endPoint == areaVertices.Count - 1)
					{
						break;
					}

					i = areaVertices.Count - 1;
				}
				wasteVertices.Add(areaVertices[i]);
			}
			wasteVertices.Add(areaVertices[endPoint]);

			List<Vector3> wasteAreaAnticlockwise = new List<Vector3>(areaVertices);
			for (int i = 0; i < newAreaVerticesCount; i++)
			{
				wasteAreaAnticlockwise.Insert(startPoint, newAreaVertices[i]);
			}

			wasteAreaAnticlockwise = wasteAreaAnticlockwise.Except(wasteVertices).ToList();
			float anticlockwiseArea = Mathf.Abs(wasteAreaAnticlockwise.Take(wasteAreaAnticlockwise.Count - 1).Select((p, i) => (wasteAreaAnticlockwise[i + 1].x - p.x) * (wasteAreaAnticlockwise[i + 1].z + p.z)).Sum() / 2f);

			// Finding greatest area size
			character.areaVertices = clockwiseArea > anticlockwiseArea ? roughAreaClock : wasteAreaAnticlockwise;
		}

		character.UpdateArea();
	}

	public static bool IsPointInside(Vector2 testPoint,Vector2[] polygon )
	{
		bool inside = false;
		int j = polygon.Count() - 1;
		for (int i = 0; i < polygon.Count(); i++)
		{
			if (polygon[i].x < testPoint.x && polygon[j].x >= testPoint.x || polygon[j].x < testPoint.x && polygon[i].x >= testPoint.x)
			{
				if (polygon[i].y + (testPoint.x - polygon[i].x) / (polygon[j].x - polygon[i].x) * (polygon[j].y - polygon[i].y) < testPoint.y)
				{
					inside = !inside;
				}
			}
			j = i;
		}
		return inside;
	}
}