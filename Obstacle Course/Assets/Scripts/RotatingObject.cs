using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    /* Variables */
    [Tooltip("... degrees per second.")]
    [SerializeField] private float rotationSpeed = 0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
