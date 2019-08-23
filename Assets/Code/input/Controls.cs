using Code.input;
using UnityEngine;

public class Controls : MonoBehaviour, IInput
{
    public Vector3 axis
    {
        get
        {
            return new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }
    }

    public float step_size { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
    }
}
