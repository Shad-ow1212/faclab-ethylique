using UnityEngine;
using UnityEngine.InputSystem;

public class playerCamera : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;
    public InputAction lookAction;

    private float xRotation;
    private float yRotation;

    private void OnEnable()
    {
        lookAction.Enable();
        Debug.Log("Enable");
    }
    private void OnDisable()
    {
        lookAction.Disable();
        Debug.Log("Disable");
    }

    private void Start() 
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() 
    {
        Vector2 mouseDelta = lookAction.ReadValue<Vector2>();

        float mouseX = mouseDelta.x * sensX;
        float mouseY = mouseDelta.y * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
