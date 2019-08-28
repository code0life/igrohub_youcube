using Code.input;
using UnityEngine;

[RequireComponent(typeof(IInput))]
[RequireComponent(typeof(CubeMoving))]
public class Cube : MonoBehaviour
{
    private IInput input;
    private CubeMoving moving;
    internal CubeContent content;
    private BoxCollider collider;
    private Rigidbody rb;

    internal bool is_player
    {
        get { return tag == "Player"; }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<IInput>();
        moving = GetComponent<CubeMoving>();
        content = GetComponent<CubeContent>();

        collider = gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = is_player;
        collider.size = Vector3.one * content.width;   
        
        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.angularDrag = 0;

        input.step_size = content.width;
        
        if(is_player)
        {
            content.GenerateColored(percent: 0.3f);
            World.UpdateInterface(this);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(input.axis != Vector3.zero && input.axis != null)
            moving.Rotate(input.axis, content.width);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(is_player && other.gameObject.GetComponent<Cube>() != null)
            World.Impact(this, other.gameObject.GetComponent<Cube>());
    }

    public void Kill()
    {
        Destroy(gameObject);
        if (content.color == Color.black)
        {
            content.CubeClone(gameObject, 5f);
        }
        //Destroy(gameObject);
    }

    public void Invisible()
    {
        gameObject.SetActive(false);
    }
}
