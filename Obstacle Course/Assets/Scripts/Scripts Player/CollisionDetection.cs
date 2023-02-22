using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    private Scoring scoring;

    private void Start() => scoring = FindObjectOfType<Scoring>();

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            if (scoring != null) 
                scoring.AddCollisionCount();

            collision.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(255,125,0,1);
            //CollisionFlash.InitFlash(collision.gameObject, Color.yellow);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
            collision.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
    }

}
