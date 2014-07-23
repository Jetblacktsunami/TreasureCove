using UnityEngine;
using System.Collections;

/***************************
 *	This entire scripts handles all 
 *		the text in the in-game scene
 * 
 * 	as well as the in-game panel
 * 	and the gameover panel
 * 
 *	NOTE: This doesnt update every frame,
 *	it only updates when it needs to so that
 *	performance doesnt take a hit from constantly
 *	overwriting values
 * *************************/

public class UIMenu : MonoBehaviour
{
	public UILabel coinsCollectedLabel;
	public UILabel coinsDepositedLabel;
	public UILabel currentAirPercentage;
	public UILabel timeLeftLabel;

	public UIPanel gameOverPanel;
	public UILabel totalPointsLabel;

	/* turn of the game over panel we dont need that yet*/
	void Awake()
	{
		gameOverPanel.gameObject.SetActive (false);
	}

	/* initialize the ui in case some values have changed from the UI */
	void Start()
	{
		UpdateAirListener ();
		UpdateCoinsListener();
	}

	/* register all the event listeners*/
	public void OnEnable()
	{
		PlayerInfo.CoinsUpdateEvent += UpdateCoinsListener;
		PlayerInfo.AirUpdateEvent += UpdateAirListener;
		GameModeCapture.TimeReadyEvent += TimeReadyListener;
		GameModeCapture.GameOverEvent += GameOverListener;
	}

	/* unregister all the event listeners*/
	public void OnDisable()
	{
		PlayerInfo.CoinsUpdateEvent -= UpdateCoinsListener;
		PlayerInfo.AirUpdateEvent -= UpdateCoinsListener;
		GameModeCapture.TimeReadyEvent -= TimeReadyListener;
		GameModeCapture.GameOverEvent -= GameOverListener;
	}
	
	/* when its game over, turn off in-game panel and show the game over panel*/
	public void GameOverListener()
	{
		gameOverPanel.gameObject.SetActive (true);
		totalPointsLabel.text = string.Concat ("Final Score: " + PlayerInfo.Player.GetDepositedCoins ());

		this.gameObject.SetActive (false);
	}

	/* updates the time label in-game, this is called once per second */
	public void TimeReadyListener(int min, int sec)
	{
		if(min < 1 && timeLeftLabel.color != Color.red)
		{
			timeLeftLabel.color = Color.red;
		}

		if(sec < 10)
		{
			timeLeftLabel.text = string.Concat ("Time ", min, " : 0", sec); 
		}
		else
		{
			timeLeftLabel.text = string.Concat ("Time ", min, " : ", sec); 
		}
	}

	/* Updates the percentage of air you have left, updates once per second, or when you pick up more air */
	public void UpdateAirListener()
	{
		currentAirPercentage.text = string.Concat("Air: ", PlayerInfo.Player.GetCurrentAir ().ToString(), "%");
	}

	/* updates all the coin info in your UI, this is only updated when you pick up/ deposit coin(s) */
	public void UpdateCoinsListener()
	{
		if(PlayerInfo.Player.GetCurrentCarryCoins() == PlayerInfo.Player.GetMaxCarryCoins())
		{
			coinsCollectedLabel.color = Color.red;
		}
		else
		{
			coinsCollectedLabel.color = Color.white;
		}

		coinsCollectedLabel.text = string.Concat( "Carrying: ", PlayerInfo.Player.GetCurrentCoins ().ToString());
		coinsDepositedLabel.text = string.Concat( "Deposited: ", PlayerInfo.Player.GetDepositedCoins ().ToString ());
	}
}
