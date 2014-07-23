using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlockSupervisor : MonoBehaviour 
{
	public List<FlockAgent> fAgents = new List<FlockAgent>(400);
	private int totalFAgents = 0;				//number of FlockAgents in the above list
	private float neighborCount = 0;			//number of FlockAgents near the current FlockAgent

	public float flockDistance = 10f;			//how far can the FlockAgent see
	public float playerAvoidanceDistance = 1f;	//distance we begin avoiding player
	public float playerAvoidanceMult = 20f;		//used to mutliply the player avoidance vector

	public float startSeperationDistance = 5f; 	//what is the minimum seperation distance to begin pushing away
	public float flockMaxVelocity = 2f; 		//what is the fasted the flock can move
	public Vector3[] boundingBox = new Vector3[2]; //2 vectors used to create a "blocking volume"  for the flock

	public float seperationMult = 1f;			//multiplier for the seperation vector
	public float cohesionMult = 1f;				//multiplier for the cohesion vector
	public float alignmentMult = 1f;			//multiplier for the alignment vector


	private FlockAgent _currentFlockAgent;		//cache the current flock agent we are going to test
	private FlockAgent _compareFlockAgent;		//cache the data of the flock agent we are testing against
	private Vector3 diffVector = new Vector3();	//the difference ( direction ) between _current and _compare
	private float distanceSqr = 0f;				//the magnitude of the diffVector

	/**************************************************/
	/*  These are the vectors we will store the "Rules"
	 * 
	 *  These will be constantly reused so we do not need 
	 *   to call the garbage collector
	/***************************************************/
	private Vector3 cohesion = new Vector3();	
	private Vector3 seperation = new Vector3();
	private Vector3 alignment = new Vector3();
	private Vector3 bounds = new Vector3();
	private Vector3 futureVelocity = new Vector3();
	private Vector3 playerAvoid = new Vector3();

	private bool bSwarm = false;		//should the flock swarm into the player

	/* a property in order to avoid doing Find Calls */
	private static FlockSupervisor _fSupervisor;
	public static FlockSupervisor FSupervisor 
	{
		get
		{
			return _fSupervisor;			
		}
		private set
		{
			if( _fSupervisor == null)			
			{
				_fSupervisor = value;
			}
			else
			{
				Destroy(value);
			}
		}
	}

	/* assign a new value to the porperty before round starts */
	void Awake()
	{
		FlockSupervisor.FSupervisor = this;
	}

	/* initialize all data */
	void Start()
	{
		totalFAgents = fAgents.Count;
		flockDistance *= flockDistance;
		flockMaxVelocity *= flockMaxVelocity;
		startSeperationDistance *= startSeperationDistance;
	}

	/* The bread and butter of this whole script
	*	Calculates the 5 rules
	*		Cohesion - flock towards center of mass
	*		Seperation - avoid other flock units
	*		Alignment - flock towards general direction
	*		Bounds - avoid leaving the bounds of the map
	*		playerAvoid - avoid the player unless swarm is true
	*
	*	then we aggregate all the rules into the future velocity
	*/
	void Update()
	{
		for(int i = 0; i < totalFAgents ; i++)
		{
			_currentFlockAgent = fAgents[i];
			cohesion.Set (0f, 0f, 0f);
			seperation.Set (0f, 0f, 0f);
			alignment.Set (0f, 0f, 0f);
			bounds.Set (0f,0f,0f);
			futureVelocity.Set (0f, 0f,0f);
			neighborCount = 0;

			for(int j = 0; j < totalFAgents; j++)
			{
				_compareFlockAgent = fAgents[j];

				if(_compareFlockAgent != _currentFlockAgent)
				{
					diffVector = _compareFlockAgent._transform.position - _currentFlockAgent._transform.position;
					distanceSqr = diffVector.sqrMagnitude;
					if(  distanceSqr <= flockDistance  )
					{
						cohesion +=  _compareFlockAgent._transform.position;
						alignment += _compareFlockAgent._rigidbody.velocity;
						neighborCount++;
						if(distanceSqr <= startSeperationDistance )
						{	
							seperation -= diffVector;
						}
					}
				}
			}


			bounds = Vector3.Normalize( BoundsCheck(_currentFlockAgent._transform.position) );


			if(neighborCount > 0)
			{
				cohesion = ( ((cohesion / neighborCount)) - _currentFlockAgent._transform.position);
				alignment = ( alignment / neighborCount);
				seperation =  Vector3.Normalize ( seperation );
			}

			if(PlayerInfo.Player != null)
			{
				playerAvoid = PlayerInfo.Player._transform.position - _currentFlockAgent._transform.position;

				if(playerAvoid.sqrMagnitude <= playerAvoidanceDistance)
				{
					if(bSwarm)
					{
						playerAvoid *= -1;
					}
					
					cohesion = (cohesion) - (playerAvoid * playerAvoidanceMult);
				}
			}

			futureVelocity = (alignment * alignmentMult) + (seperation * seperationMult) + (cohesion * cohesionMult) + _currentFlockAgent._rigidbody.velocity + bounds;

			if( futureVelocity.sqrMagnitude > flockMaxVelocity)
			{
				_currentFlockAgent._rigidbody.velocity = Vector3.ClampMagnitude( futureVelocity, Mathf.Sqrt(flockMaxVelocity) );
			}
			else
			{
				_currentFlockAgent._rigidbody.velocity = futureVelocity;
			}
		}
	}

	private Vector3 BoundsCheck( Vector3 position )
	{
		Vector3 boundsCHK = Vector3.zero;

		if( position.x <= boundingBox[0].x )
		{
			boundsCHK.x = 10;
		}

		else if( position.x >= boundingBox[1].x)
		{
			boundsCHK.x = -10;
		}		
		if( position.y <= boundingBox[0].y )
		{
			boundsCHK.y = 10;
		}
		else if( position.y >= boundingBox[1].y)
		{
			boundsCHK.y = -10;
		}		

		if( position.z <= boundingBox[0].z )
		{
			boundsCHK.z = 10;
		}
		else if( position.z >= boundingBox[1].z)
		{
			boundsCHK.z = -10;
		}

		return boundsCHK;
	}

	/* register any event listeners */
	private void OnEnable()
	{
		GameModeCapture.GameOverEvent += GameOverListener;
	}

	/* unregister any event listeners */
	private void OnDisable()
	{
		GameModeCapture.GameOverEvent -= GameOverListener;
	}

	/* logic for when the game ends */
	private void GameOverListener()
	{
		bSwarm = true;
		playerAvoidanceDistance = 10000;
	}

	/* public function to add fAgents
	*	NOTE: for simplicity i do not check 
	*		if the object is already in the list
	  */
	public void Register( FlockAgent self )
	{
		fAgents.Add (self);
		totalFAgents += 1;
	}

	/* public function to add fAgents
	*	NOTE1: for simplicity i do not check 
	*		if the object is not in the list
	*
	*	NOTE2: This is never called in-game
	  */
	public void UnRegister ( FlockAgent self)
	{
		fAgents.Remove (self);
		totalFAgents -= 1;
	}

}
