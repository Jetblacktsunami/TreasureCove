using UnityEngine;
using System.Collections;

public class GameModeCapture : MonoBehaviour 
{
	public delegate void TimeReady(int min ,int sec); 
	public static TimeReady TimeReadyEvent; //event fired when the time should update

	public delegate void GameOver();
	public static GameOver GameOverEvent; //event fired when the player gets a gameover

	/* how long should the round last */
	public int durationSeconds;	
	public int durationMinutes;

	/* same as above but locks the game time so it can't be modified */
	private int _dSeconds;
	private int _dMinutes;

	/* rate per second TimeReadyEvent should fire */
	private const float _TimeDown = 1f;
	private float _timer;

	private bool bGameOver = false; //is the game over

	/* lock length of game */	
	private void Start()
	{
		_dMinutes = durationMinutes;
		_dSeconds = durationSeconds;
	}

	/* register event listeners */
	public void OnEnable()
	{
		PlayerInfo.PlayerDeathEvent += PlayerDeathListener;
	}

	/* unregister event listeners */
	public void OnDisable()
	{
		PlayerInfo.PlayerDeathEvent -= PlayerDeathListener;
	}

	/* logic for what should happen if the player dies (runs out of air) */
	private void PlayerDeathListener()
	{
		bGameOver = true;
		PlayerInfo.Player.depositCoinsEndGame ();
		if(GameOverEvent != null)
		{
			GameOverEvent.Invoke ();
		}
	}

	/* this just makes it so it fire its events when all conditions are met */
	void Update()
	{
		if(!bGameOver)
		{
			if(_timer <= 0)
			{
				if(_dSeconds == 0)
				{
					_dSeconds = 60;
					_dMinutes -= 1;
				}

				_dSeconds -= 1;
				_timer = _TimeDown;

				if(TimeReadyEvent != null)
				{
					TimeReadyEvent.Invoke( _dMinutes, _dSeconds);
				}
			}

			_timer -= Time.deltaTime;

			if(_dSeconds == 0 && _dMinutes == 0)
			{
				bGameOver = true;
				if(GameOverEvent != null)
				{
					GameOverEvent.Invoke();
				}
			}
		}
	}

}
