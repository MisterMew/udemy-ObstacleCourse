using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    /* Variables */
    [Tooltip("... degrees per second.")]
    [SerializeField] private float rotationSpeed = 0f;
    [SerializeField] private bool inverseRotation = false;
    private float rotate = 0f;

    // Update is called once per frame
    void Update()
    {
        rotate = rotationSpeed * Time.deltaTime;

        if (inverseRotation)
            rotate = -rotate;

        transform.Rotate(0, rotate, 0);
    }
}
