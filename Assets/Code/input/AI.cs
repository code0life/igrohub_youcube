using Code.input;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour, IInput
{
    public enum EnumBehavior
    {
        ESCAPE,
        NEUTRAL,
        CHASE
    }
    
    public EnumBehavior behavior = EnumBehavior.NEUTRAL;
    public float attention_radius = 8;
    public GameObject target;
    
    public Vector3 axis
    {
        get
        {    
            if((transform.position - agent.pathEndPosition).magnitude < step_size)
                return Vector3.zero;
                
            Vector3 dir = (agent.pathEndPosition - transform.position).normalized;
            return new Vector3(Mathf.Round(dir.x), 0, Mathf.Round(dir.z));
        }
    }

    private float step = 0;
    public float step_size
    {
        get { return step; }
        set { step = value;  }
    }


    public Vector3 direction_from_target
    {
        get
        {
            return transform.position - target.transform.position;
        }
    }
    
    public float distance_to_target
    {
        get
        {
            return direction_from_target.magnitude;
        }
    }
    
    private NavMeshAgent agent;
    
    void Awake()
    {
        agent = gameObject.AddComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false; 
        agent.updateUpAxis = false;
        agent.speed = 20;
    }
    
    public void Warp(Vector3 position)
    {
        agent.Warp(position);
    }

    // Update is called once per frame
    void Update()
    {
        switch (behavior)
        {
            case EnumBehavior.CHASE:
                if (distance_to_target < attention_radius)
                    agent.destination = target.transform.position;
                break;
            case EnumBehavior.ESCAPE:
                if (distance_to_target < attention_radius)
                    agent.destination = target.transform.position + direction_from_target.normalized * (attention_radius + 2);
                break;
        }
    }
}
