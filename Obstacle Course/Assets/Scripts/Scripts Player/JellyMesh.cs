using UnityEngine;

public class JellyMesh : MonoBehaviour
{
    [SerializeField] private float intensity = 1f;
    [SerializeField] private float mass = 1f;
    [SerializeField] private float stiffness = 1f;
    [SerializeField] private float damping = 0.75f;

    private Mesh originalMesh, meshClone;
    private MeshRenderer renderer;
    private JellyVertex[] jellyVertex;
    private Vector3[] vertexArray;


    // Start is called before the first frame update
    void Start()
    {
        originalMesh = GetComponent<MeshFilter>().sharedMesh;
        meshClone = Instantiate(originalMesh);
        GetComponent<MeshFilter>().sharedMesh = meshClone;
        renderer = GetComponent<MeshRenderer>();
        jellyVertex = new JellyVertex[meshClone.vertices.Length];

        for (int i = 0; i < meshClone.vertices.Length; i++)
            jellyVertex[i] = new JellyVertex(i, transform.TransformPoint(meshClone.vertices[i]));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        vertexArray = originalMesh.vertices;
        for (int i = 0; i < jellyVertex.Length; i++)
        {
            Vector3 target = transform.TransformPoint(vertexArray[jellyVertex[i].id]);
            float intensity = (1 - (renderer.bounds.max.y - target.y) / renderer.bounds.size.y) * this.intensity;
            jellyVertex[i].Shake(target, mass, stiffness, damping);

            target = transform.InverseTransformPoint(jellyVertex[i].position);
            vertexArray[jellyVertex[i].id] = Vector3.Lerp(vertexArray[jellyVertex[i].id], target, intensity);

        }
        meshClone.vertices = vertexArray;
    }

    public class JellyVertex
    {
        public int id;
        public Vector3 position;
        public Vector3 velocity, force;

        public JellyVertex(int _id, Vector3 _pos)
        {
            id = _id;
            position = _pos;
        }

        public void Shake(Vector3 target, float m, float s, float d)
        {
            force = (target - position) * s;
            velocity = (velocity + force / m) * d;
            position += velocity;

            if ((velocity + force + force / m).magnitude < 0.001f)
                position = target;
        }

    }
}