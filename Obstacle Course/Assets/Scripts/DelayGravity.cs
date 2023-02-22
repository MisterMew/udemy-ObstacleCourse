using UnityEngine;

public class DelayGravity : MonoBehaviour
{
    /* Variables */
    [Tooltip("... time in seconds(s)")]
    [SerializeField] private float gravityDelayTime = 0F;
    private Rigidbody rb = null;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= gravityDelayTime)
        {
            rb.useGravity = true;
        }
    }
}
