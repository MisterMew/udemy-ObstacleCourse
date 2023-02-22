using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /* Variables */
    [SerializeField] private float speed = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float xValue = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float zValue = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        transform.Translate(xValue, 0, zValue);
    }
}
