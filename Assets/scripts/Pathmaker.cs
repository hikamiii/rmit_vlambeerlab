using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathmaker : MonoBehaviour
{
    public int floorCounter = 0;
    public GameObject trapPrefab;
    public GameObject floorPrefab1;
    public GameObject floorPrefab2;
    public GameObject wallPrefab;
    public GameObject chestPrefab;
    public GameObject coinPrefab;
    public GameObject enemyPrefab1;
    public GameObject enemyPrefab2;
    public GameObject pathmakerSpherePrefab;

    public static int globalTileCount = 0;
    public static int maxFloorTile = 700;

    private static float turnProbability = 0.15f;
    private bool isPlacingTile = false;

    private const float tileCheckRadius = 0.11f;
    private List<GameObject> placedFloors = new List<GameObject>();

    private void Start()
    {
        turnProbability = Random.Range(0.0f, 0.4f);
    }

    void Update()
    {
        if (!isPlacingTile)
        {
            StartCoroutine(PlaceTile(0.02f));
        }
    }

    private IEnumerator PlaceTile(float delay)
    {
        isPlacingTile = true;

        if (globalTileCount >= maxFloorTile)
        {
            FinishGeneration();
            yield break;
        }

        if (floorCounter <= maxFloorTile)
        {
            float randomNumber = Random.Range(0.0f, 1.0f);
            float randomObject = Random.Range(0.0f, 1.0f);

            if (randomNumber < 0.15f && globalTileCount + 30 <= maxFloorTile)
            {
                if (CanCreateRoom(transform.position))
                {
                    yield return CreateRoom();
                }
            }
            else
            {
                if (randomNumber < turnProbability)
                {
                    transform.Rotate(0, Random.Range(0, 2) == 0 ? 90 : -90, 0);
                }

                if (!IsPositionOccupied(transform.position))
                {
                    GameObject selectedFloorPrefab = Random.Range(0, 2) == 0 ? floorPrefab1 : floorPrefab2;
                    GameObject newFloor = Instantiate(selectedFloorPrefab, transform.position, Quaternion.identity);
                    if (randomObject <= 0.02)
                    {
                        Instantiate(coinPrefab, transform.position, Quaternion.identity);
                    }
                    if (randomObject > 0.02 && randomObject <= 0.05)
                    {
                        GameObject selectedEnemy = Random.Range(0, 2) == 0 ? enemyPrefab1 : enemyPrefab2;
                        GameObject newEnemy = Instantiate(selectedEnemy, transform.position, Quaternion.identity);
                    }
                    if (randomObject > 0.05 && randomObject <= 0.1)
                    {
                        Instantiate(trapPrefab, transform.position, Quaternion.identity);
                    }

                    placedFloors.Add(newFloor);
                    floorCounter++;
                    globalTileCount++;
                }

                transform.Translate(Vector3.forward * 1);
            }
        }
        else
        {
            FinishGeneration();
        }

        yield return new WaitForSeconds(delay);
        isPlacingTile = false;
    }

    private IEnumerator CreateRoom()
    {
        Vector3 originalPosition = transform.position;
        GameObject selectedEnemy = Random.Range(0, 2) == 0 ? enemyPrefab1 : enemyPrefab2;

        if (!CanCreateRoom(originalPosition))
        {
            Debug.Log("Room creation aborted: Overlap detected.");
            yield break;
        }

        for (int x = 0; x < 7; x++)
        {
            for (int z = 0; z < 7; z++)
            {
                Vector3 roomPosition = originalPosition + new Vector3(x, 0, z);

                GameObject selectedFloorPrefab = Random.Range(0, 2) == 0 ? floorPrefab1 : floorPrefab2;
                GameObject newFloor = Instantiate(selectedFloorPrefab, roomPosition, Quaternion.identity);
                placedFloors.Add(newFloor);
                globalTileCount++;

                yield return null;
            }
        }
        float randomNumber = Random.Range(0.0f, 1.0f);

        if (randomNumber <= 0.2)
        {
            PlaceObject(chestPrefab, originalPosition + new Vector3(0, 0, 4));
            PlaceObject(coinPrefab, originalPosition + new Vector3(6, 0, 3));
            PlaceObject(selectedEnemy, originalPosition + new Vector3(5, 0, 5));
        }
        if (randomNumber > 0.2 && randomNumber <= 0.4)
        {
            PlaceObject(coinPrefab, originalPosition + new Vector3(3, 0, 3));
            PlaceObject(coinPrefab, originalPosition + new Vector3(5, 0, 2));
            PlaceObject(selectedEnemy, originalPosition + new Vector3(1, 0, 2));
        }
        if (randomNumber > 0.4 && randomNumber <= 0.6)
        {
            PlaceObject(chestPrefab, originalPosition + new Vector3(0, 0, 2));
            PlaceObject(chestPrefab, originalPosition + new Vector3(6, 0, 0));
        }
        if (randomNumber > 0.6 && randomNumber <= 0.8)
        {
            PlaceObject(selectedEnemy, originalPosition + new Vector3(2, 0, 3));
            PlaceObject(selectedEnemy, originalPosition + new Vector3(5, 0, 2));
            PlaceObject(selectedEnemy, originalPosition + new Vector3(4, 0, 4));
            PlaceObject(chestPrefab, originalPosition + new Vector3(2, 0, 0));
        }
        if (randomNumber > 0.8)
        {
            PlaceObject(chestPrefab, originalPosition + new Vector3(1, 0, 1));
            PlaceObject(selectedEnemy, originalPosition + new Vector3(4, 0, 2));
            PlaceObject(selectedEnemy, originalPosition + new Vector3(2, 0, 5));
        }

        transform.position = originalPosition + new Vector3(7, 0, 2);
        floorCounter += 30;

        Debug.Log("Room Created");
    }

    private void PlaceObject(GameObject prefab, Vector3 position)
    {
        Instantiate(prefab, position, Quaternion.identity);
    }

    private bool CanCreateRoom(Vector3 startPosition)
    {
        for (int x = 0; x < 7; x++)
        {
            for (int z = 0; z < 7; z++)
            {
                Vector3 checkPosition = startPosition + new Vector3(x, 0, z);

                if (IsPositionOccupied(checkPosition))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool IsPositionOccupied(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, tileCheckRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Floor"))
            {
                return true;
            }
        }
        return false;
    }

    private void FinishGeneration()
    {
        foreach (var floor in placedFloors)
        {
            GenerateWalls();
        }
        Destroy(gameObject);
    }

    private void GenerateWalls()
    {
        GameObject[] allFloors = GameObject.FindGameObjectsWithTag("Floor");

        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

        foreach (GameObject floor in allFloors)
        {
            foreach (Vector3 direction in directions)
            {
                Vector3 rayOrigin = floor.transform.position + Vector3.up * 0.2f; // Slightly above the tile to avoid ground collision

                if (!Physics.Raycast(rayOrigin, direction, 1f))
                {
                    Vector3 wallPosition = floor.transform.position + direction;
                    Quaternion wallRotation = Quaternion.LookRotation(-direction); // Rotate wall to face inward

                    if (wallPrefab != null)
                    {
                        Instantiate(wallPrefab, wallPosition, wallRotation);
                    }
                    else
                    {
                        Debug.LogError("wallPrefab is not assigned!");
                    }
                }
            }
        }
    }
}

// INTRO TO PROC GEN LAB
// all students: complete steps 1-6, as listed in this file
// optional: if you're up for a mind safari, complete the "extra tasks" to do at the very bottom

// STEP 1: ======================================================================================
// put this script on a Sphere... it SHOULD move around, and drop a path of floor tiles behind it
// STEP 2: ============================================================================================
// translate the pseudocode below

//	DECLARE CLASS MEMBER VARIABLES:
//	Declare a private integer called counter that starts at 0; 		// counter will track how many floor tiles I've instantiated
//	Declare a public Transform called floorPrefab, assign the prefab in inspector;
//	Declare a public Transform called pathmakerSpherePrefab, assign the prefab in inspector; 		// you'll have to make a "pathmakerSphere" prefab later
//		If counter is less than 50, then:
//			Generate a random number from 0.0f to 1.0f;
//			If random number is less than 0.25f, then rotate myself 90 degrees;
//				... Else if number is 0.25f-0.5f, then rotate myself -90 degrees;
//				... Else if number is 0.99f-1.0f, then instantiate a pathmakerSpherePrefab clone at my current position;
//			// end elseIf

//			Instantiate a floorPrefab clone at current position;
//			Move forward ("forward", as in, the direction I'm currently facing) by 5 units;
//			Increment counter;
//		Else:
//			Destroy my game object; 		// self destruct if I've made enough tiles already

// MORE STEPS BELOW!!!........

// STEP 3: =====================================================================================
// implement, test, and stabilize the system

//	IMPLEMENT AND TEST:
//	- save your scene! the code could potentially be infinite / exponential, and crash Unity
//	- put Pathmaker.cs on a sphere, configure all the prefabs in the Inspector, and test it to make sure it works
//	STABILIZE: 
//	- code it so that all the Pathmakers can only spawn a grand total of 500 tiles in the entire world; how would you do that?
//	- hint: declare a "public static int" and have each Pathmaker check this "globalTileCount", somewhere in your code? 
//      -  What is a 'static'?  Static???  Simply speak the password "static" to the instructor and knowledge will flow.
//	- Perhaps... if there already are enough tiles maybe the Pathmaker could Destroy my game object

// STEP 4: ======================================================================================
// tune your values...

// a. how long should a pathmaker live? etc.  (see: static  ---^)
// b. how would you tune the probabilities to generate lots of long hallways? does it... work?
// c. tweak all the probabilities that you want... what % chance is there for a pathmaker to make a pathmaker? is that too high or too low?



// STEP 5: ===================================================================================
// maybe randomize it even more?

// - randomize 2 more variables in Pathmaker.cs for each different Pathmaker... you would do this in Start()
// - maybe randomize each pathmaker's lifetime? maybe randomize the probability it will turn right? etc. if there's any number in your code, you can randomize it if you move it into a variable



// STEP 6:  =====================================================================================
// art pass, usability pass

// - move the game camera to a position high in the world, and then point it down, so we can see your world get generated
// - CHANGE THE DEFAULT UNITY COLORS
// - add more detail to your original floorTile placeholder -- and let it randomly pick one of 3 different floorTile models, etc. so for example, it could randomly pick a "normal" floor tile, or a cactus, or a rock, or a skull
// - or... make large city tiles and create a city.  Set the camera low so and une the values so the city tiles get clustered tightly together.

//		- MODEL 3 DIFFERENT TILES IN BLENDER.  CREATE SOMETHING FROM THE DEEP DEPTHS OF YOUR MIND TO PROCEDURALLY GENERATE. 
//		- THESE TILES CAN BE BASED ON PAST MODELS YOU'VE MADE, OR NEW.  BUT THEY NEED TO BE UNIQUE TO THIS PROJECT AND CLEARLY TILE-ABLE.

//		- then, add a simple in-game restart button; let us press [R] to reload the scene and see a new level generation
// - with Text UI, name your proc generation system ("AwesomeGen", "RobertGen", etc.) and display Text UI that tells us we can press [R]


// EXTRA TASKS TO DO, IF YOU WANT / DARE: ===================================================

// AVOID SPAWNING A TILE IN THE SAME PLACE AS ANOTHER TILE  https://docs.unity3d.com/ScriptReference/Physics.OverlapSphere.html
// Check out the Physics.OverlapSphere functionality... 
//     If the collider is overlapping any others (the tile prefab has one), prevent a new tile from spawning and move forward one space. 

// DYNAMIC CAMERA:
// position the camera to center itself based on your generated world...
// 1. keep a list of all your spawned tiles
// 2. then calculate the average position of all of them (use a for() loop to go through the whole list) 
// 3. then move your camera to that averaged center and make sure fieldOfView is wide enough?

// BETTER UI:
// learn how to use UI Sliders (https://unity3d.com/learn/tutorials/topics/user-interface-ui/ui-slider) 
// let us tweak various parameters and settings of our tech demo
// let us click a UI Button to reload the scene, so we don't even need the keyboard anymore.  Throw that thing out!

// WALL GENERATION
// add a "wall pass" to your proc gen after it generates all the floors
// 1. raycast out from each floor tile (that'd be 4 raycasts per floor tile, in a square "ring" around each tile?)
// 2. if the raycast "fails" that means there's empty void there, so then instantiate a Wall tile prefab
// 3. ... repeat until walls surround your entire floorplan
// (technically, you will end up raycasting the same spot over and over... but the "proper" way to do this would involve keeping more lists and arrays to track all this data)