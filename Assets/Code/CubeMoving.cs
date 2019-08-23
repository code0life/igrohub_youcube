using System.Collections;
using UnityEngine;

public class CubeMoving : MonoBehaviour
{
    public int speed = 9;
    public GameObject vfx_rotating_complete;
    
    private float step_size;
    private Vector3 axis_direction = Vector3.zero;
    private Vector3 current_position = Vector3.zero;
    private bool toggle = false;
    
    private IEnumerator coroutine_animation;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 GetNextDirection(Vector3 axis)
    {
        if (axis.sqrMagnitude <= 1) 
            return Vector3.one;
        
        toggle = !toggle;
        return toggle ? Vector3.forward : Vector3.right;
    }

    private void StartRotation()
    {
        coroutine_animation = DoRotation(); 
        StartCoroutine(coroutine_animation);
    }

    IEnumerator ShowVfx()
    {
        GameObject vfx = Instantiate(vfx_rotating_complete);
        vfx.transform.localScale = Vector3.one * step_size;
        vfx.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        
        yield return new WaitForSeconds(2);
        
        Destroy(vfx);
    }

    IEnumerator DoRotation()
    {
        var edge_delta = step_size * 0.5f;
        Vector3 around_pos = current_position + axis_direction * edge_delta;
        around_pos.y = 0;
        
        Vector3 dir_rotating = Vector3.Cross(Vector3.up, axis_direction);
        
        for (var i = 0; i < speed; i ++) {
            transform.RotateAround(around_pos, dir_rotating, 90.0f/(float)speed);
            yield return null;
        }
        
        axis_direction = Vector3.zero;
        if(GetComponent<AI>() != null)
            GetComponent<AI>().Warp(transform.position);
        
        StartCoroutine(ShowVfx());
        
        coroutine_animation = null; 
    }
    
    public void Rotate(Vector3 axis, float width)
    {
        if(coroutine_animation != null)
            return;
        
        step_size = width;
        current_position = transform.position;
        axis_direction = Vector3.Scale(axis.normalized, GetNextDirection(axis)).normalized;
            
        StartRotation();
    }
}
