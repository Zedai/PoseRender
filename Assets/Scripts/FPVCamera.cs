using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPVCamera : MonoBehaviour
{
    [Range(0.1f, 1.0f)]
    public float speed;
    [Range(0.1f, 1.0f)]
    public float sensitivity;

    private Transform cameraTransform;
    public static bool select = false;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Focus")){
            if(select){
                select = false;     
                Cursor.lockState = CursorLockMode.Locked;
			}  
            else{
                select = true;
                Cursor.lockState = CursorLockMode.None;
			}
		}
        if(select){
            return;  
		}
        float Pan = Input.GetAxis("Mouse X");
        float Tilt = -1 * Input.GetAxis("Mouse Y");

        if (Input.GetAxis("Truck") > 0)
        {
            cameraTransform.position += transform.right * speed;
        }
        if (Input.GetAxis("Truck") < 0)
        {
            cameraTransform.position -= transform.right * speed;
        }
        if (Input.GetAxis("Dolly") > 0)
        {
            cameraTransform.position += transform.forward * speed;
        }
        if (Input.GetAxis("Dolly") < 0)
        {
            cameraTransform.position -= transform.forward * speed;
        }
        if (Input.GetAxis("Pedestal") > 0)
        {
            cameraTransform.position += transform.up * speed;
        }
        if (Input.GetAxis("Pedestal") < 0)
        {
            cameraTransform.position -= transform.up * speed;
        }
        if ((Input.GetAxis("Pan") != 0) || (Input.GetAxis("Tilt") != 0))
        {
            if ((cameraTransform.localEulerAngles.x > 80 && cameraTransform.localEulerAngles.x <= 180 && Tilt > 0) ||
                (cameraTransform.localEulerAngles.x > 180 && cameraTransform.localEulerAngles.x < 280 && Tilt < 0))
            {
                Tilt = 0;
            }
            cameraTransform.Rotate(Tilt * sensitivity, Pan * sensitivity, 0.0f, Space.Self);
            Quaternion q = transform.rotation;
            q.eulerAngles = new Vector3(q.eulerAngles.x, q.eulerAngles.y, 0);
            cameraTransform.rotation = q;
        }
    }
}
