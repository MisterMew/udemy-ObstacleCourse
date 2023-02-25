using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    /* Variables */
    private Scoring scoring;

    private void Start() => scoring = FindObjectOfType<Scoring>();

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            if (scoring != null) 
                scoring.AddCollisionCount();

            if (!collision.gameObject.GetComponentInChildren<CollisionFlash>())
                collision.gameObject.AddComponent<CollisionFlash>();

            collision.gameObject.GetComponentInChildren<CollisionFlash>().
                InitFlash(collision.gameObject, new Color32(255, 125, 0, 1));
        }
    }
}
