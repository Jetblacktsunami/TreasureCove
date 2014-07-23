using UnityEngine;
using System.Collections;
using System.Threading;

public class CoinSpawner : MonoBehaviour 
{
	public GameObject _Coins; //prefab of objects you want to be spawned in
	public Vector3[] spawnAbleArea = new Vector3[2]; //what are valid coordinates to spawn the objects
	public float spawnInterval; //how many do you want to spawn in seconds
	private float _spawnInterval; //this will be constantly modified and then reassigned to spawnInterval

	/* register event listeners */
	void OnEnable()
	{
		GameModeCapture.GameOverEvent += GameOver;
	}

	/* unregister event listeners */
	void OnDisable()
	{
		GameModeCapture.GameOverEvent -= GameOver;
	}

	/* as long as the game is going we are going to keep spawning coins every (spawnInterval) seconds */
	private void Update()
	{
		if( _spawnInterval <= 0)
		{
			GameObject.Instantiate ( _Coins, new Vector3 ( Random.Range (spawnAbleArea[0].x , spawnAbleArea[1].x ),Random.Range (spawnAbleArea[0].y , spawnAbleArea[1].y ),Random.Range (spawnAbleArea[0].z , spawnAbleArea[1].z ) )  , Quaternion.identity);
			_spawnInterval = spawnInterval;
		}

		_spawnInterval -= Time.deltaTime;
	}

	/* when the player gets game over we unregister and destroy ourselves*/
	private	void GameOver()
	{
		GameModeCapture.GameOverEvent -= GameOver;
		Destroy (this);
	}
}
