using UnityEngine;
using System.Collections;
using System.Collections.Generic ;
using System.IO ;

//attach this to any object, even the first person controller with the FPSInputTracker on it and running (it'll show the gui example on-screen while you're testing the tracker).

public class FPSTrackerExampleGUI : MonoBehaviour
{

	FPSInputTracker.UserLog[] userLogs = new FPSInputTracker.UserLog[0] ;


	Rect guiArea ;


	void Start()
	{
		GetUserLogs() ;

		float width = Screen.width - 30f ;
		float height = Screen.height - 30f ;
		guiArea = new Rect((Screen.width/2f)-(width/2f), (Screen.height/2f)-(height/2f), width, height) ;
	}

	/// <summary>
	/// Retrieves all of our saved xml file data in the UserSaveLogs folder we make with our FPSInputTracker.
	/// </summary>
	void GetUserLogs()
	{
		string directory = System.IO.Path.Combine(Application.dataPath, "UserSaveLogs") ;
		string[] fileNames = (Directory.Exists(directory)) ? Directory.GetFiles(directory) : new string[0] ;

		if(fileNames.Length <= 0)
			return ;

		List<string> xmlFiles = new List<string>() ;

		for(int i = 0 ; i < fileNames.Length ; i++)
		{
			if(fileNames[i].EndsWith(".xml"))
				xmlFiles.Add(fileNames[i]) ;
		}

		int count = xmlFiles.Count ;

		userLogs = new FPSInputTracker.UserLog[count] ;

		for(int i = 0 ; i < count ; i++)
		{
			userLogs[i] = new FPSInputTracker.UserLog().Load(System.IO.Path.Combine(FPSInputTracker._path, xmlFiles[i])) ;
		}

	}

	


	void OnGUI()
	{
		GUILayout.BeginArea(guiArea, GUI.skin.box) ;

		DoGUI() ;

		GUILayout.EndArea() ;
	}

	void DoGUI()
	{

		GUILayout.Label("User Logs") ;

		if(userLogs.Length <= 0)
		{
			GUILayout.Label("No Records Found") ;
			return ;
		}

		ShowLogSelect() ;

		ShowLogScroll() ;
	}


	int currentMoveData = 0 ;
	int currentMouseData = 0 ;
	int currentLog = 0 ;

	void ShowLogSelect()
	{
		GUILayout.BeginHorizontal(GUILayout.MinWidth(guiArea.width/2f), GUILayout.MaxWidth(guiArea.width/2f)) ;

		GUILayout.Label("<size='18'><b>User Movement Log</b></size> #" + (currentLog+1) + " of " + userLogs.Length) ;

		if(GUILayout.Button("Back"))
		{
			currentLog-- ;
			currentMoveData = 0 ;
			currentMouseData = 0 ;
		}

		if(GUILayout.Button("Next"))
		{
			currentLog++ ;
			currentMoveData = 0 ;
			currentMouseData = 0 ;
		}

		currentLog = Mathf.Clamp(currentLog, 0, userLogs.Length-1) ;

		GUILayout.EndHorizontal() ;
	}


	Vector2 scrollPos ;
	void ShowLogScroll()
	{
		scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MinWidth(guiArea.width-20f), GUILayout.MaxWidth(guiArea.width-20f), 
		                                      GUILayout.MinHeight(guiArea.height-70f), GUILayout.MaxHeight(guiArea.height-70f)
		                                      ) ;

		ShowMoveData() ;

		ShowMouseData() ;

		GUILayout.EndScrollView() ;
	}

	void ShowMoveData()
	{
		GUILayout.Label("<size='14'><color='red'><b>Movement Data</b></color></size>") ;

		if(userLogs[currentLog].movementData.Length <= 0)
			GUILayout.Label("No Movement Records Found") ;
		else
		{
			GUILayout.BeginHorizontal(GUILayout.MinWidth(guiArea.width/2f), GUILayout.MaxWidth(guiArea.width/2f)) ;
			
			if(GUILayout.Button("Back"))
				currentMoveData-- ;
			if(GUILayout.Button("Next"))
				currentMoveData++ ;
			
			currentMoveData = Mathf.Clamp(currentMoveData, 0, userLogs[currentLog].movementData.Length-1) ;
			
			GUILayout.EndHorizontal() ;


			GUILayout.BeginVertical("box") ;

			GUILayout.Label("#" + (currentMoveData + 1) + " of " + userLogs[currentLog].movementData.Length) ;

			GUILayout.Space(10f) ;

			GUILayout.Label("Idle Time : " + userLogs[currentLog].movementData[currentMoveData].idleTime.ToString()) ;
			Vector3 startPos = userLogs[currentLog].movementData[currentMoveData].movement.startPosition ;
			GUILayout.Label("Start Position : X(" + startPos.x + ") Y(" + startPos.y + ") Z(" + startPos.z + ")") ;




			GUILayout.EndVertical() ;

			GUILayout.Space(40f) ;
		}

	}

	void ShowMouseData()
	{	
		GUILayout.Label("<size='14'><color='red'><b>Camera/Look Data</b></color></size>") ;

		if(userLogs[currentLog].mouseData.Length <= 0)
			GUILayout.Label("No Camera-Look Records Found") ;
		else
		{
			GUILayout.BeginHorizontal(GUILayout.MinWidth(guiArea.width/2f), GUILayout.MaxWidth(guiArea.width/2f)) ;
			
			if(GUILayout.Button("Back"))
				currentMouseData-- ;
			if(GUILayout.Button("Next"))
				currentMouseData++ ;
			
			currentMouseData = Mathf.Clamp(currentMouseData, 0, userLogs[currentLog].mouseData.Length-1) ;
			
			GUILayout.EndHorizontal() ;


			GUILayout.BeginVertical("box") ;

			GUILayout.Label("#" + (currentMouseData + 1) + " of " + userLogs[currentLog].mouseData.Length) ;
			
			GUILayout.Space(10f) ;

			Vector3 fDir = userLogs[currentLog].mouseData[currentMouseData].tracks.forwardDirection ;
			GUILayout.Label("Forward Direction : X(" + fDir.x + ") Y(" + fDir.y + ") Z(" + fDir.z + ")") ;

			GUILayout.Label("Time Facing Direction : " + userLogs[currentLog].mouseData[currentMouseData].totalTimeFacingDirection) ;

			GUILayout.EndVertical() ;
		}
		
	}












}
