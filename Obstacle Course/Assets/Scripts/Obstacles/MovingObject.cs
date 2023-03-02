using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Handles the transit system for moving objects.
/// </summary>
public class MovingObject : MonoBehaviour
{
    /* Variables */
    [Header("OBJECT")]
    [Tooltip("This object MUST be in the scene, NOT a prefab.")]
    [SerializeField] private Transform targetObject = null;


    [Header("WAYPOINTS")]
    [Tooltip("List of transforms that determine where the object will move between.")]
    [SerializeField] private List<Transform> objectWaypoints = null;
    private int targetWaypoint = 0;


    [Header("CONFIG")]
    [Tooltip("Enable to allow movement automatically.")]
    [SerializeField] private bool reverseMovement = false;
    [Tooltip("The travel speed of the object.")]
    [SerializeField, Range(0F, 100F)] private float objectspeed = 0F;

    [Header("Object Stopping")]
    [SerializeField] private bool delayAtDestination = false;
    [SerializeField, Range(1F, 60F)] private float delayDuration = 0F;

    [Header("Player Interaction")]
    [Tooltip("if TRUE: Object will only move while the player is touching it")]
    [SerializeField] private bool playerActivated = false;
    [SerializeField] private bool lockUntilActivated = false;
    [Tooltip("-1 to start at scene position. The waypoint that a object will ALWAYS return to (element number from 'Object Waypoints' list)")]
    [SerializeField] private int homeWaypoint = 0;

    private bool moveObject = true;
    private bool atDestination = false;
    private bool playerMissing = true;

    /// <summary>
    /// Called once on before first Update frame
    /// </summary>
    private void Start()
    {
        /* Set Starting Position */
        if (!lockUntilActivated || homeWaypoint > -1)
            targetObject.transform.position = objectWaypoints[homeWaypoint].position;

        /* Check if player Activated */
        if (playerActivated)
            moveObject = false;
    }

    /// <summary>
    /// Runs once every fixed update frame
    /// </summary>
    private void FixedUpdate() => MoveObject(moveObject); //Moves the object

    /// <summary>
    /// Called once every frame
    /// </summary>
    private void Update()
    {
        /* Validate Destination */
        if (AtDestination(targetWaypoint))
            atDestination = true;
        else
            atDestination = false;

        /* Validate Player Activation */
        if (playerActivated && AtDestination(homeWaypoint) && playerMissing)
            moveObject = false;
    }

    /// <summary>
    /// If a collisison is detected
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return; //Return if NOT player

        playerMissing = false;

        if (playerActivated && AtDestination(homeWaypoint)) //If at homePoint
            ValidateObjectPosition();                    //Validate next destination

        moveObject = true;
    }

    /// <summary>
    /// If a collision is no longer detected
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return; //Return if NOT player

        playerMissing = true;
    }

    /// <summary>
    /// Validate if the object is at the home waypoint
    /// </summary>
    /// <returns></returns>
    private bool AtDestination(int waypoint)
    {
        if (targetObject.transform.position == objectWaypoints[waypoint].position)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Moves the object to the next waypoint
    /// </summary>
    /// <param name="move">False to prevent movement</param>
    private void MoveObject(bool move = true)
    {
        if (move == false) return;

        targetObject.transform.position = Vector3.MoveTowards(targetObject.transform.position, objectWaypoints[targetWaypoint].position, objectspeed * Time.deltaTime); //Move the object

        if (atDestination) //If at destination
            ValidateObjectPosition(); //Validate next move
    }

    /// <summary>
    /// Handles the objects waypoint to waypoint movement
    /// </summary>
    private async void ValidateObjectPosition()
    {
        moveObject = false;

        /* Delay Destination */
        if (delayAtDestination) //If delay is enabled
            await Task.Delay((int)(delayDuration * 1000)); //Delay task

        SetNextDestination(); //Sets the objects next destination
    }

    /// <summary>
    /// Determine which way the objects will transition
    /// </summary>
    private void SetNextDestination()
    {
        if (atDestination)
        {
            if (reverseMovement)
                /* Reversed Movement */
                targetWaypoint = (targetWaypoint == 0) ? objectWaypoints.Count : (targetWaypoint - 1);
            else
                /* Normal Movement */
                targetWaypoint = targetWaypoint == (objectWaypoints.Count - 1) ? 0 : (targetWaypoint + 1);

            moveObject = true;
            atDestination = false;
        }
    }
}
