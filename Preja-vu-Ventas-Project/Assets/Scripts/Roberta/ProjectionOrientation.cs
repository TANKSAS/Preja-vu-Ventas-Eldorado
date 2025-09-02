using UnityEngine;

public class ProjectionOrientation : MonoBehaviour
{
    public GameObject centralObject;
    public ParticleSystem particle;
    public float distance;
    public Vector3 distanceAdjusment;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (centralObject != null)
        {
            distance = Vector3.Distance(this.transform.position, centralObject.transform.position + distanceAdjusment);
            //particle.startSize = new Vector3(x,distance,z);
            gameObject.transform.LookAt(centralObject.transform.position + distanceAdjusment);
        }
    }
}
