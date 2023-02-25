using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /* Variables */
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float jumpHeight = 0f;

    private float xValue, yValue, zValue = 0;

    // Update is called once per frame
    void Update()
    {
        xValue = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        yValue = Input.GetAxis("Jump") * jumpHeight * Time.deltaTime;
        zValue = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        transform.Translate(xValue, yValue, zValue);
    }
}
