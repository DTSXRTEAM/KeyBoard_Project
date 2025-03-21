using UnityEngine;

public class CameraFollow : MonoBehaviour

{

    public Transform drone;   // The drone the camera will follow

    public Vector3 offset;    // The offset between the camera and the drone

    void Update()

    {

        transform.position = drone.position + offset;

        transform.LookAt(drone); // Make the camera look at the drone

    }

}

