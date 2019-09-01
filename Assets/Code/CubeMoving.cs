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
    
    public IEnumerator coroutine_animation;
    
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
        if (!Interface.instance.game_done)
        {
            StartCoroutine(coroutine_animation);
        }

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
    
    Vector3 GetCorectSideMove()
    {
        Vector3 side_move = transform.TransformDirection(Vector3.right) * 10;
        Vector3 newVector = new Vector3(side_move.x, side_move.y, side_move.z);
        var correct = transform.rotation;

        if (Vector3.Dot(axis_direction, Vector3.right) > 0)
        {
            newVector = Vector3.MoveTowards(transform.position, transform.position+Vector3.right * 10, 10f);
        }
        else if (Vector3.Dot(axis_direction, Vector3.right) < 0)
        {
            newVector = Vector3.MoveTowards(transform.position, transform.position + Vector3.left * 10, 10f);
        }
        else if (Vector3.Dot(axis_direction, Vector3.forward) > 0)
        {
            newVector = Vector3.MoveTowards(transform.position, transform.position + Vector3.forward * 10, 10f);
        }
        else if (Vector3.Dot(axis_direction, Vector3.forward) < 0)
        {
            newVector = Vector3.MoveTowards(transform.position, transform.position + Vector3.forward * -10, 10f);

        }

        return newVector;
    }
    Vector3 GetBestFreePath(float dist_old, GameObject target)
    {
        Debug.Log("GetBestFreePath");
        Vector3 new_side_move = Vector3.zero;

        if (IsFreePath(Vector3.right))
        {
            float dist_new = Vector3.Distance(target.GetComponent<Transform>().position, transform.position + Vector3.right);
            if (dist_new >= dist_old)
            {
                dist_old = dist_new;
                new_side_move = Vector3.right;
            }

        }

        if (IsFreePath(Vector3.left))
        {
            //Debug.Log("left");
            float dist_new = Vector3.Distance(target.GetComponent<Transform>().position, transform.position + Vector3.left);
            //Debug.Log("dist_new - " + dist_new);
            //Debug.Log("dist_old - " + dist_old);
            if (dist_new >= dist_old)
            {
                dist_old = dist_new;
                new_side_move = Vector3.left;
            }

        }

        if (IsFreePath(Vector3.forward))
        {
            //Debug.Log("forward");
            float dist_new = Vector3.Distance(target.GetComponent<Transform>().position, transform.position + Vector3.forward);
            if (dist_new >= dist_old)
            {
                dist_old = dist_new;
                new_side_move = Vector3.forward;
            }

        }

        if (IsFreePath(Vector3.forward * -1))
        {
            //Debug.Log("forward*-1");
            float dist_new = Vector3.Distance(target.GetComponent<Transform>().position, transform.position + Vector3.forward * -1);
            if (dist_new >= dist_old)
            {
                dist_old = dist_new;
                new_side_move = Vector3.forward * -1;
            }
        }

        return new_side_move;
    }

    public Vector3 GetFreePath()
    {
        Debug.Log("GetFreePath");
        //Vector3 new_side_move = Vector3.zero;
        GameObject target = GetComponent<AI>().target;
        float dist_old = Vector3.Distance(target.GetComponent<Transform>().position, transform.position);
        Vector3 best_side = GetBestFreePath(dist_old, target);

        return best_side;
    }

    public bool IsFreePath(Vector3 side)
    {
        Vector3 correct_side_move = GetCorectSideMove();

        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, side, 100.0F);

        if ( GetComponent<Cube>().is_player != true  )
        {
            //Debug.Log("IsFreePath - ");

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.GetComponent<Cube>() != null & hits[i].distance < step_size )
                {
                    Debug.DrawLine(transform.position, correct_side_move, Color.red, 1f);
                    RaycastHit hit = hits[i];
                    string temp_name = hit.transform.name;

                    if (temp_name != null)
                    {
                        return false;

                    }
                }

            }

        }

        return true;
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
