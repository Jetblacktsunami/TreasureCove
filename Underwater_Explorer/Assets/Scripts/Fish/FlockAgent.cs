using UnityEngine;
using System.Collections;

/*************************************
 * these gameobjects are essenttially 
 * being treated as particles for the 
 * purpose of this project
 * 
 * it just holds variables and doesn't
 * hold any of the real behavior
 * ***********************************/
[RequireComponent (typeof(Rigidbody))]
public class FlockAgent : MonoBehaviour 
{
#if UNITY_EDITOR
	public static int count = 0;
#endif
	public Transform _transform; //cache the transform component
	public Rigidbody _rigidbody; //cache the rigidbody component

	// Use this for initialization
	void Start () 
	{
#if UNITY_EDITOR
		name = count.ToString();
		count++;
#endif
		_transform = GetComponent<Transform> ();
		_rigidbody = GetComponent<Rigidbody> ();
		_rigidbody.velocity = new Vector3(Random.Range (-1f , 1f), Random.Range (-1f , 1f), Random.Range (-1f , 1f));

		FlockSupervisor.FSupervisor.Register (this);
	}

	void OnDestroy()
	{
		FlockSupervisor.FSupervisor.Register (this);
	}
}
