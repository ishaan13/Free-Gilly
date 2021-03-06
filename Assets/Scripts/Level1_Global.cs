﻿using UnityEngine;
using System.Collections;

static class Constants {
	
	// Bubbles
    public const int MAX_BUBBLES = 5;
	public const float BUBBLE_REGEN_TIME = 3.0f;
	public const float BUBBLE_SPAWN_OFFSET = 1.5f;
	public const bool WII_RUMBLE = true;
	
	// Stamina
	public const float STAMINA_DEC_RATE = 0.1f;
	public const float STAMINA_REGEN_TIME = 1.0f;
	public const float STAMINA_REGEN_AMT = 0.5f;
	
	//Used in Navigation Script 
	public const int MOVE_SPEED = 2;
}

public class Level1_Global : MonoBehaviour {

	public GameObject bubble;
	public GameObject g;
	
	// Player controller
	public GameObject player;
	
	// navAgents
	public GameObject navAgent;
	public GameObject obsNavAgent;
	
	public OVRCameraController CameraController = null;
	private Camera MainCam;
	
	private UniWiiCheck uniWii;
	
	public Vector3 direction;
	public Vector3 startPosition;
	
	// Health and stamina stats
	public int currentHealth;
	public int maxHealth;
	public float currentStamina;
	public float maxStamina;
	public float staminaRegenTimer;
	
	// Power-Ups
	public bool storedHealthPU;
	public bool storedStaminaPU;
	
	// Bubbles
	public int bubblesLeft;
	public float bubbleRegenTimer;
	
	// Score & Timer
	public int score;
	public float startTime;
	public float timer;
	
	// Spawn point array
	public GameObject[] spawnPositions;
	
	// SetOVRCameraController
	public void SetOVRCameraController(ref OVRCameraController cameraController)
	{
		CameraController = cameraController;
		CameraController.GetCamera(ref MainCam);
	}
	
	// Use this for initialization
	void Start () {
	    
		g = GameObject.Find("Gilly");
		SetOVRCameraController(ref CameraController);
		
		uniWii = gameObject.GetComponent<UniWiiCheck>();
		
		currentHealth = 100;
		maxHealth = 100;
		currentStamina = 100.0f;
		maxStamina = 100.0f;
		
		storedHealthPU = false;
		storedStaminaPU = false;
		
		bubblesLeft = Constants.MAX_BUBBLES;
		bubbleRegenTimer = Constants.BUBBLE_REGEN_TIME;
		staminaRegenTimer = Constants.STAMINA_REGEN_TIME;
		
		score = 0;
		startTime = Time.time;
		
	}
	
	void Awake() {
	
		// Select a random spawn position
		int pos = (int) Random.Range(0, spawnPositions.Length);
		Debug.Log(pos);
		setSpawnPoint(pos);
	}
	
	// Update is called once per frame
	void Update () {
		
		// Update timer
		timer = Time.time - startTime;
		
		// Game over condition
		if(currentHealth <= 0)
		{
			Debug.Log("Game Over!");
			
			// Restart from last checkpoint or return to main menu
		}
		
		if(currentStamina <= 0)
			currentStamina = 0;
		
		if(currentStamina < maxStamina)
		{
			staminaRegenTimer -= Time.deltaTime;
			if(staminaRegenTimer <= 0)
			{
				currentStamina += Constants.STAMINA_REGEN_AMT;
				staminaRegenTimer = Constants.STAMINA_REGEN_TIME;
			}
		}
		
		if(bubblesLeft < Constants.MAX_BUBBLES)
		{
			bubbleRegenTimer -= Time.deltaTime;
			if(bubbleRegenTimer <= 0)
			{
				// Add a bubble and reset timer
				bubblesLeft++;
				bubbleRegenTimer = Constants.BUBBLE_REGEN_TIME;
			}
		}
		
		// Shoot bubble
		if(uniWii.buttonBPressed || Input.GetKeyDown(KeyCode.P))
		{
			if(bubblesLeft > 0)
			{
				startPosition = g.transform.position;
				Vector3 startPos = MainCam.transform.position;
				direction = Vector3.forward;
		    	direction = MainCam.transform.rotation * direction;
			
				Vector3 dir = direction;
				dir.Normalize();
				Vector3 offset = dir * Constants.BUBBLE_SPAWN_OFFSET;
				startPosition = startPosition + offset;
		    
				// Create bubble
				Instantiate(bubble, startPos + offset, Quaternion.identity); //
			
				// Update bubbles left counter
				bubblesLeft--;
			}
		}
		
		// Use health power-up
		if(uniWii.button1Pressed)
		{
			if(storedHealthPU == true)
			{
				currentHealth += 25;
				
				if(currentHealth > 100)
					currentHealth = 100;
				
				storedHealthPU = false;
			}
		}
		
		// Use stamina power-up
		if(uniWii.button2Pressed)
		{
			if(storedStaminaPU == true)
			{
				currentStamina += 25;
				
				if(currentStamina > 100)
					currentStamina = 100;
				
				storedStaminaPU = false;
			}
		}
	}
	
	void setSpawnPoint(int pos)
	{
		SpawnPositionScript s = spawnPositions[pos].GetComponent<SpawnPositionScript>();
		
	 	//GameObject navObj, obsNavObj;
	 	//NavMeshAgent nav, obsNav;
	
		//navObj = GameObject.Find("NavAgent");
		//obsNavObj = GameObject.Find("SpawnNavAgent");
	    //nav = navObj.GetComponent<NavMeshAgent>();
		//obsNav = obsNavObj.GetComponent<NavMeshAgent>();
		
		// Place the player at the spawn position
		Instantiate(player, spawnPositions[pos].transform.position, Quaternion.identity); 
		//player.transform.position = spawnPositions[pos].transform.position;
		
		// Place the navAgents at the appropriate positions
		GameObject na = (GameObject) Instantiate(navAgent, s.navAgentPos.transform.position, Quaternion.identity) as GameObject;
		na.GetComponent<NavMeshAgent>().SetDestination(s.endPoint.transform.position);
		
		GameObject ona = (GameObject) Instantiate(obsNavAgent, s.obstacleAgentPos.transform.position, Quaternion.identity);
		ona.GetComponent<NavMeshAgent>().SetDestination(s.endPoint.transform.position);
		
		//Debug.Log(s.destination);
		//navObj.transform.position = s.navAgentPos.transform.position;
		//obsNavObj.transform.position = s.obstacleAgentPos.transform.position;
	}
	

}
