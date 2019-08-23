using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    public GameObject cube;
    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - cube.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var cube_pos = cube.transform.position;
        cube_pos.y = 0;
        transform.position += (cube_pos + offset - transform.position)/50;
        transform.LookAt(cube_pos);
    }
}
