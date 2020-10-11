using UnityEngine;
using System.Collections;
using System.Collections.Generic ;
using System.Xml ;
using System.Xml.Serialization ;
using System.IO ;
using UnityEngine.SceneManagement;

public class FPSInputTracker : MonoBehaviour 
{
	//the fileName we'll be using for a prefix
	static string xmlSaveFileName = "FPSInputTracker" ;

	//time (in seconds) that we wait to record a new change in look direction
	//avoids recording mouse movement data every frame if the user is sitting there moving the mouse back and forth all over the place
	//..set in the inspector panel of the editor. Set to zero if you want to keep track of every single time the mouse has moved.
	public float lookTrackingTimeFilter = 0.5f ;

	//the function that this enables is somewhat costly as it is now, check as true in the inspector if you want the method GetObjectsInCameraView() to run.
	//view the method this runs for info on how to modify this script to run better with it.
	public bool listObjectsInSight = false ;
	

	//used to keep track of the last mouse position so we know when it's been moved (the first person controller has been turned)
	//[0] is used for the camera movement tracking, [1] is used for the controller turn tracking
	Vector3[] lastMousePosition = new Vector3[]{Vector3.zero, Vector3.zero} ;



	//this is the path that we'll save/load xml data from
	public static string _path 
	{
		get
		{
			//after you are finished testing, change this line to [string s = Application.persistentDataPath ;]
			//as it is now, it saves into a folder in the Assets folder. After finished testing and you change the value of this variable..
			//..it will save into a folder in your built project's folder.
			//--... and if you want to save/load it from elsewhere, just change this to whatever it is you're doing.
			string s = Application.dataPath ;

			//going to use a folder named "UserSaveLogs" to save the xml files into and to retrieve them from
			s = System.IO.Path.Combine(s, "UserSaveLogs") ;

			//if the "UserSaveLogs" folder doesn't exist, create it
			if(!Directory.Exists(s))
				Directory.CreateDirectory(s) ;

			return s ;
		}
	}

	#region movement classes----------------------------------------------------------------------------
	public class Movement
	{
		//most variable names are self-explanitory...

		public Vector3 startPosition ;
		public Vector3 endPosition ;
		public float startTime, endTime, totalTime, distanceMoved ;

		//default constructor for this class
		public Movement()
		{
			this.startPosition = Vector3.zero ;
			this.startTime = 0f ;
			this.endTime = 0f ;
			this.totalTime = 0f ;
			this.endPosition = Vector3.zero ;
			this.distanceMoved = 0f ;
		}

		//another constructor variant
		public Movement(Vector3 startPosition)
		{
			this.startPosition = startPosition ;
			this.startTime = Time.timeSinceLevelLoad ;
		}

		//function to finish filling in our data when we've stopped this movement
		public void EndMovement(Vector3 endPosition)
		{
			this.endTime = Time.timeSinceLevelLoad ;
			this.totalTime = endTime - startTime ;
			this.endPosition = endPosition ;
			this.distanceMoved = Vector3.Distance(this.startPosition, this.endPosition) ;
		}
	}


	public class MovementData
	{
		//most variable names are self-explanitory...


		public float idleTime ;

		public Movement movement ;

		//default constructor for this class
		public MovementData()
		{
			idleTime = 0f ;
			movement = new Movement() ;
		}
		//another constructor variant
		public MovementData(Movement movement, float previousEndTime)
		{
			this.movement = movement ;
			this.idleTime = movement.startTime - previousEndTime ;
		}
	}
	#endregion


	//list to keep track of the controller movements while playing
	public List<Movement> movements = new List<Movement>() ;



	#region mouse/looking classes -----------------------------------------------------------------------------------------
	public class MouseTracks
	{
		//used so we'll know how long we were looking in each direction later
		public float startTime = 0f ;

		//most variable names are self-explanitory...
		public Vector3 forwardDirection ;
		
		//class to store info about objects in the sight of the camera
		public class ObjectInSight
		{
			//the name of this object
			public string name ;
			//this objects world position
			public Vector3 position ;
			
			//default constructor
			public ObjectInSight(){}

			//another constructor
			public ObjectInSight(GameObject obj)
			{
				this.name = obj.name ;
				this.position = obj.transform.position ;
			}

		}

		public class ObjectDirectlyAhead : ObjectInSight
		{
			//the world position that is the direct focus on the object (center of myCam, out forward, direct hit position)
			public Vector3 focusPoint ;

			//default constructor
			public ObjectDirectlyAhead(){} 

			//another constructor to set up some values when we raycast later on
			public ObjectDirectlyAhead(RaycastHit hit)
			{
				this.name = hit.transform.name ;
				this.position = hit.transform.position ;
				this.focusPoint = hit.point ;
			}
		}

		
		//the object we're directly facing
		[XmlElement("ObjectDirectlyAhead")]
		public ObjectDirectlyAhead objectDirectlyAhead ;
 
		//all other objects visible by the camera
		[XmlArray("OtherObjectsInSight"), XmlArrayItem("Object")]
		public ObjectInSight[] objectsInSight ;
			
		//default constructor
		public MouseTracks(){}
		
		//another constructor...
		public MouseTracks(Vector3 forwardDirection, params GameObject[] objectsInSight)
		{
			this.forwardDirection = forwardDirection ;
			this.startTime = Time.timeSinceLevelLoad ;

			if(objectsInSight.Length > 0)
				SetObjectsInSight(objectsInSight) ;
		}

		//and yet another constructor
		public MouseTracks(Vector3 forwardDirection, RaycastHit hit, params GameObject[] objectsInSight)
		{
			this.forwardDirection = forwardDirection ;
			this.startTime = Time.timeSinceLevelLoad ;
			this.objectDirectlyAhead = new ObjectDirectlyAhead(hit) ;

			//we use >1 here because we know that one object in sight will be the one we hit with the raycast
			if(objectsInSight.Length > 1)
			{
				//going to exclude the object we hit with our raycast we sent in 
				List<GameObject> objs = new List<GameObject>() ;
				for(int i = 0 ; i < objectsInSight.Length ; i++)
				{
					//skip the object we hit with our raycast
					if(objectsInSight[i] == hit.transform.gameObject) continue ;

					objs.Add(objectsInSight[i]) ;
				}
				SetObjectsInSight(objs.ToArray()) ;
			}
		}

		void SetObjectsInSight(GameObject[] objs)
		{
			this.objectsInSight = new ObjectInSight[objs.Length] ;

			for(int i = 0 ; i < objs.Length ; i++)
			{
				this.objectsInSight[i] = new ObjectInSight(objs[i]) ;
			}
		}
	}

	//class for the complete mouse data we'll be saving later
	public class MouseData
	{
		public float totalTimeFacingDirection ;
		public MouseTracks tracks ;
		
		//default constructor
		public MouseData()
		{
			totalTimeFacingDirection = 0f ;
			tracks = new MouseTracks() ;
		}
		//another constructor
		public MouseData(MouseTracks tracks, float time)
		{
			this.tracks = tracks ;
			this.totalTimeFacingDirection = time - tracks.startTime ;
		}
	}

	#endregion

	//list to keep track of the mouse movements while playing
	static List<MouseTracks> tracks = new List<MouseTracks>() ;


	
	//---this class is what gets saved-to/retrieved-from an xml file-->
	[XmlRoot("UserLog")]
	public class UserLog
	{
		[XmlElement("TotalTimeTakenToComplete")]
		public float totalTime ;
		[XmlElement("TotalDistanceTraveled")]
		public float totalDistance ;

		[XmlArray("MovementData"), XmlArrayItem("Information")]
		public MovementData[] movementData ;

		[XmlArray("LookData"), XmlArrayItem("Information")]
		public MouseData[] mouseData ;

		//default constructor
		public UserLog()
		{
			this.totalTime = 0f ;
			this.totalDistance = 0f ;
			movementData = new MovementData[0] ;
			mouseData = new MouseData[0] ;
		}

		//another constructor
		public UserLog(List<Movement> movements, List<MouseTracks> tracks)
		{
			if(movements.Count <= 0)
			{
				this.movementData = new MovementData[0] ;
				//early-out and don't save, because something is screwy and the user has somehow gotten to the end-trigger-zone without having to move ???
				return ;
			}

			MovementData[] data = new MovementData[movements.Count] ;  
			
			for(int i = 0 ; i < movements.Count ; i++)
			{
				float previousEndTime = (i == 0) ? 0f : movements[i-1].endTime ;
				data[i] = new MovementData(movements[i], previousEndTime) ;
				this.totalDistance += data[i].movement.distanceMoved ;
			}

			this.movementData = data ;


			this.totalTime = data[data.Length-1].movement.endTime - (data[0].movement.startTime - data[0].idleTime) ;


			if(tracks.Count <= 0)
			{
				this.mouseData = new MouseData[0] ;
				return ;
			}
			
			List<MouseData> mData = new List<MouseData>() ; 
			
			for(int i = 0 ; i < tracks.Count ; i++)
			{
				float time = (i+1 < tracks.Count) ? tracks[i+1].startTime : Time.timeSinceLevelLoad ;

				mData.Add(new MouseData(tracks[i], time)) ;
			}
			
			this.mouseData = mData.ToArray() ;
		}



		#region XML saving/loading -----------------------------------------------------------------------------------
		//script header must include [using]-> System.Xml, System.Xml.Serialization, System.IO

		//pass exactly this from within this script-> (Path.Combine(_path, xmlSaveFileName)) to save data
		public void Save(string path)
		{
			int index = 0 ;
			string p = path + "." + index.ToString() ;

			//while a file already exists named as our prefix-save name + "." + index, we increment index so we create a new file instead of overwriting one
			
			while(File.Exists(p + ".xml"))
			{
				index ++ ;
				p = path + "." + index.ToString() ;
			}

			var serializer = new XmlSerializer(typeof(UserLog)) ;
		
			var stream = new FileStream(p + ".xml", FileMode.Create) ;
	
			serializer.Serialize(stream, this) ;
			
			stream.Close() ;
		}

		//pass (Path.Combine(FPSInputTracker._path, fullFileNameWith.xml)) to retrieve data
		public UserLog Load(string path)
		{
			//if the file we're trying to load doesn't exist, return a new/blank UserLog()
			if(!File.Exists(path))
				return new UserLog() ;

			var serializer = new XmlSerializer(typeof(UserLog)) ;
			var stream = new FileStream(path, FileMode.Open) ;
			
			var ul = serializer.Deserialize(stream) as UserLog ;
			
			stream.Close() ;
			return ul ;
		}
		#endregion
	}


	
	//cache reference to this objects transform
	Transform myTrans ;
	//cache reference to the camera transform
	Transform myCam ;



	void Start()
	{
		myTrans = this.transform ;
		myCam = myTrans.Find("Main Camera") ;

		//just a check to make sure we have a camera object child for this thing
		if(!myCam || !myCam.GetComponent<Camera>())
		{
			Debug.LogError("Error : No child of name \"Main Camera\" was found on GameObject(\"" + myTrans.name + "\")\n" +
			               "FPSInputTracker is designed to work with the Standard Assets First Person Controller, which has a Main Camera child.\n" +
			               "Either you've re-named this object, this object doesn't have a camera component, or it doesn't exist in the this objects Hierarchy.\n" +
			               "FPSInputTracker will now be disabled to avoid further errors."
			               ) ;
			this.enabled = false ;
			return ;
		}

		AddMouseTrack() ;
		StartCoroutine(TrackLook()) ;
	}


	bool goalCompleted = false ;

	void Update()
	{
		TrackPositions() ;
	}


	//going to use this for when our character makes it to the end-zone/goal object
	IEnumerator OnTriggerEnter(Collider other)
	{
		//make sure in your tags list to create a new tag called "GoalTrigger" (or whatever you want to call it, and then change the string in this CompareTag("whatever") to match)
		//if this trigger we hit wasn't the goal or we've already completed this and moved back into the trigger before ending the scene then early-out of this function
		if(!other.CompareTag("Finish") || goalCompleted) yield break ;

		goalCompleted = true ;

		//stop our final movement
		EndMovement() ;

		//if we made it here, then we've hit our goal trigger, so save our data
		new UserLog(movements, tracks).Save(System.IO.Path.Combine(_path, xmlSaveFileName)) ;

		//this is where you'd add something to change to the next level/scene if you were going to.
		//eg :
		//yield return new WaitForSeconds(0.5f) ;
		//Application.LoadLevel(Application.loadedLevel + 1) ; to go to the next level in your build settings

		yield return new WaitForSeconds(2f) ;
        SceneManager.LoadScene("Last");
	}




	int currentMovementIndex = 0 ;
	bool isMoving = false ;

	void TrackPositions()
	{
		if(goalCompleted) return ;

		//movement tracking done according to user input rather than on a timed basis, because if you wanted to get positions every second or two, then 
		//the user could have changed direction of movement in-between timespans and it wouldn't be recorded.
		//this is more accurate, as it records every position that the movement direction was/is changed


		#region mouse-changed movement-------------------------------------------------------------------------------------------------------------------------
		//using the First person controller, moving the mouseX means that we've also changed our direction of movement (if we're moving) because this turns the entire controller.
		//I put it here to go along with the accuracy of the other input movement tracking so that it updates every single time the user turns while moving.
		if(Input.mousePosition.x != lastMousePosition[1].x)
			if(isMoving) StartMovement() ;
	
		lastMousePosition[1] = Input.mousePosition ;
		#endregion


		//if we just now pushed a movement button then we've started moving, regardless of if we were already moving before because now our direction has changed
		if(Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
		{
			StartMovement() ;
		}

		//if we just let up on a movement button
		if(Input.GetButtonUp("Horizontal") || Input.GetButtonUp("Vertical"))
		{
			//if we are still holding some other movement button
			if(Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
			{
				//if we made it into this, then that means we just changed directions again by letting off of one button
				StartMovement() ;
			}
			else //we have stopped moving now
			{
				EndMovement() ;
			}
		}
	}



	void StartMovement()
	{
		movements.Add(new Movement(myTrans.position)) ;
		
		//if we were already moving when we pressed this button
		if(isMoving)
		{
			EndMovement() ;
		}
		
		isMoving = true ;
	}

	void EndMovement()
	{
		//just to make sure when we hit the goal-trigger that it doesn't try to end our movement twice if we let off or push a move button at the same time as..
		//..entering the trigger, or let off right before and we slid into the trigger without pushing a button, this gets put in an if-statement
		if(movements.Count <= 0 || currentMovementIndex < movements.Count)
			movements[currentMovementIndex].EndMovement(myTrans.position) ;

		currentMovementIndex ++ ;
		isMoving = false ;
	}



	IEnumerator TrackLook()
	{
		if(goalCompleted) yield break ;

		//if we have not moved the mouse and changed the direction we're facing, while we're still facing the same direction just wait til next frame and check again
		while(Input.mousePosition == lastMousePosition[0]) yield return null ;

		if(tracks.Count > 0)
		{
			//if it hasn't been long enough since the last mousePosition change for our filter, then keep waiting one frame and check again until enough time has passed
			while(Time.timeSinceLevelLoad - tracks[tracks.Count-1].startTime < lookTrackingTimeFilter) yield return null ;
		}

		AddMouseTrack() ;
	
		//start this function up again
		StartCoroutine(TrackLook()) ;
	}


	void AddMouseTrack()
	{
		GameObject[] objsInSight = listObjectsInSight ? GetObjectsInCameraView() : new GameObject[0] ;

		//cast a ray to see what object (if any) is directly in front of us... since no distance value is used in the raycast then it will look as far as it possibly can
		RaycastHit hit ;
		//if we "see" an object
		if(Physics.Raycast(myCam.position, myCam.forward, out hit))
			tracks.Add(new MouseTracks(myCam.forward, hit, objsInSight)) ;
		else //if we DONT "see" a specific object
			tracks.Add(new MouseTracks(myCam.forward, objsInSight)) ;

		//update this to be the current mouse position
		lastMousePosition[0] = Input.mousePosition ;
	}


	GameObject[] GetObjectsInCameraView()
	{
		//NOTE : if you aren't planning on changing anything in the environment at all, not adding any new objects to the scene, not disabling renderer's, etc..
		//... then you could use the Start() function to grab all rendererables objects instead of getting them every time we call AddMouseTrack()

		//first get ALL gameobjects in the scene with renderers on them
		Renderer[] renderables = GameObject.FindObjectsOfType(typeof(Renderer)) as Renderer[] ;

		//now we'll make a list of all objects with renderers that are actually visible
		//(note : in the Doc's, it explains that even something that actually is NOT visible will be considered as visible if it is needed for shadows etc)
		//also note : if there's more than one camera running in the scene, if the other camera(s) can see something that our [myCam] cant see it will still be considered visible
		List<GameObject> visibles = new List<GameObject>() ;

		for(int i = 0 ; i < renderables.Length ; i++)
		{
			if(!renderables[i].isVisible) continue ;

			visibles.Add(renderables[i].gameObject) ;
		}

		return visibles.ToArray() ;
	}



}