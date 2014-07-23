using UnityEngine;
using System.Collections;

public class TreasureBox : MonoBehaviour 
{
	/* when the player enters its range it will 
	 * automatically tell the player to deposit his/ her coin(s)
	 */
	void OnTriggerEnter(Collider other)	
	{
		if(other.gameObject.tag == "Player")
		{
			PlayerInfo.Player.depositCoinsInGame();
		}
	}
}
