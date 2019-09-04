using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByContact : MonoBehaviour
{
    //public GameObject explosion; //
    //public GameObject playerExplosion; //

    public float times;
    private bool is_floor = false;
    Transform tr;

    void OnTriggerEnter(Collider other)
    {
        //Instantiate(explosion, transform.position, transform.rotation); //
        if (other.tag == "floor")
        {
            is_floor = true;
            //var rb = GetComponent<Rigidbody>();
            //if (rb != null)
            //{
            //    rb.mass = 2f;
            //    rb.isKinematic = false;
            //    rb.AddForce(transform.forward * 10);
            //}
            //if (Interface.instance.game_done == false)
            //{
            //    StartCoroutine(ShowFadeVfx(cube, new_color, time));
            //    if (rb != null)
            //    {
            //        rb.AddForce(transform.up * 100);
            //    }
            //}

            //Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
        }
        else
        {
        }

        Destroy(gameObject, times);

    }

    void Start()
    {
        tr = GetComponent<Transform>();
    }

    void Update()
    {

        if (is_floor)
        {
            tr.transform.Translate(Vector3.back * Time.deltaTime);
        }
    }
}
