using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour 
{
	public delegate void CoinsUpdated();
	public static CoinsUpdated CoinsUpdateEvent;

	public delegate void AirUpdated();
	public static CoinsUpdated AirUpdateEvent;

	public delegate void PlayerDeath ();
	public static PlayerDeath PlayerDeathEvent;

	public Transform _transform;

	private int coinsDepositedValue = 0;
	private int currentCoinValue = 0;
	private int amountOfCoins = 0;
	public int maxCarryCoins = 10; //the max a player can carry

	public int airDecayRate = 2;
	private int refillAmount = 10;
	private int currentAir = 100;
	private int maxAir = 100;
	
	private static PlayerInfo _player;
	public static PlayerInfo Player
	{
		get
		{
			return _player;
		}
		private set
		{
			if(_player == null)
			{
				_player = value;
			}
			else
			{
				Destroy ( value );
			}
		}
	}

	private void OnEnable()
	{
		GameModeCapture.TimeReadyEvent += DecreaseAir;
	}

	private void OnDisable()
	{
		GameModeCapture.TimeReadyEvent -= DecreaseAir;
	}

	private void Awake()
	{
		PlayerInfo.Player = this;
		Player._transform = GetComponent<Transform> ();
	}

	private void DecreaseAir(int min, int sec)
	{
		currentAir -= airDecayRate;

		if(currentAir <= 0 && PlayerDeathEvent != null)
		{
			PlayerDeathEvent.Invoke();
		}

		if(AirUpdateEvent != null)
		{
			AirUpdateEvent.Invoke();
		}
	}

	public void IncreaseAir()
	{
		if(currentAir + refillAmount > maxAir)
		{
			currentAir = maxAir;
		}
		else
		{
			currentAir += refillAmount;
		}

		if(AirUpdateEvent != null)
		{
			AirUpdateEvent.Invoke();
		}
	}

	public int GetCurrentAir ()
	{
		return currentAir;
	}

	public int GetCurrentCarryCoins()
	{
		return amountOfCoins;
	}

	public int GetCurrentCoins()
	{
		return currentCoinValue;
	}

	public int GetMaxCarryCoins ()
	{
		return maxCarryCoins;
	}

	public int GetDepositedCoins()
	{
		return coinsDepositedValue;
	}

	public void depositCoinsInGame()
	{
		coinsDepositedValue += currentCoinValue * (amountOfCoins);
		currentCoinValue = 0;
		amountOfCoins = 0;

		if( CoinsUpdateEvent != null)
		{
			CoinsUpdateEvent.Invoke();
		}
	}

	public void depositCoinsEndGame()
	{
		coinsDepositedValue += currentCoinValue;
		amountOfCoins = 0;
		currentCoinValue = 0;
	}

	public bool IncreaseCurrentCoins()
	{
		if(amountOfCoins >= maxCarryCoins)
		{
			return false;
		}

		currentCoinValue += 10;
		amountOfCoins += 1;

		if( CoinsUpdateEvent != null)
		{
			CoinsUpdateEvent.Invoke();
		}

		return true;
	}
}
