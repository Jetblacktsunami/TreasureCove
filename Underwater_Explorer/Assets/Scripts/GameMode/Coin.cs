using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
public class Coin : MonoBehaviour 
{
	public ParticleSystem pickUpEffect;
	public Transform _transform;

	/**********************************
	*  checks if a player has entered its trigger
	*	if true then it rewards the player 
	*		with a coin if he has open space
	*	
	*	then destroys itself
	*
	*	NOTE: I do not do object pooling for these
	*	coins, doing so can potentially increase performance
	*	ever so slightly but not enough exist to create a 
	*	huge impact
	  ********************************/
	public void Start()
	{
		_transform = GetComponent<Transform> ();
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			if( PlayerInfo.Player.IncreaseCurrentCoins() )
			{
				ParticleSystem.Instantiate ( pickUpEffect, _transform.position, Quaternion.identity);
				Destroy (this.gameObject);
			}
		}
	}

}
