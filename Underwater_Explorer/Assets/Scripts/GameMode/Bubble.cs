using UnityEngine;
using System.Collections;

/*********************************
 * Attached to a capsule trigger
 * once the player collides it adds
 * more air to their tank
 * 
 * it also turns of the collider for
 * 2 seconds so it doesnt spam the 
 * main thread with function calls
 * and collision calls
 *********************************/
public class Bubble : MonoBehaviour 
{
	private	CapsuleCollider _Collider;
	public float resetTimer = 2f; 

	void Start()
	{
		_Collider = GetComponent<CapsuleCollider> ();
	}

	void OnTriggerStay(Collider other)
	{
		if( other.gameObject.tag == "Player")
		{
			PlayerInfo.Player.IncreaseAir();
			StartCoroutine(	TurnOffCollider());
		}
	}

	IEnumerator TurnOffCollider()
	{
		_Collider.enabled = false;
		yield return new WaitForSeconds ( resetTimer );
		_Collider.enabled = true;

	}
}
