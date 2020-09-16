using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField]
    private string XLookAxisName = "Mouse X";
    [SerializeField]
    private string YLookAxisName = "Mouse Y";

    [SerializeField]
    private float CameraAngleMin = -20.0f;
    [SerializeField]
    private float CameraAngleMax = 60.0f;

    private Transform PlayerTorso;

    public float Sensitivity = 100f;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerTorso = transform.Find("Torso");

        if (PlayerTorso == null)
            throw new System.Exception("The player must have a GameObject named 'Torso' which contains a Camera.");

        if (PlayerTorso.GetComponentInChildren<Camera>() == null)
            throw new System.Exception("A Camera must be attached to the Player FPS Torso->Head");
    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxis(XLookAxisName) * Sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(YLookAxisName) * Sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, CameraAngleMin, CameraAngleMax);

        transform.Rotate(Vector3.up * mouseX);
        PlayerTorso.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}