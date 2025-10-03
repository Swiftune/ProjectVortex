using UnityEngine;
using UnityEngine.InputSystem;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int LockedMin, LockedMax;
    [SerializeField] bool invertY;
    float rotX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //get input
        float mouseX = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime;

        //use invertY 
        if (invertY)
        {
            rotX += mouseY;
        }
        else
        {
            rotX -= mouseY;
        }

        //clampo camera on x-axis 
        rotX = Mathf.Clamp(rotX, LockedMin, LockedMax);

        //rotate camera about x-axis 
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        //roatate player about y-axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
