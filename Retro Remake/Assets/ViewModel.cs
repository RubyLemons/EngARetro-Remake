using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewModel : MonoBehaviour
{
    [SerializeField] Freelook freelook;
    [SerializeField] Freeroam freeroam;

    [Header("Positioning")]

    public Transform targetModel;

    [SerializeField] Vector3 offset;

    [Header("Sway")]

    [Range(0, 1)] [SerializeField] float[] smooth = new float[2] { 0.05f, 0.075f };
    [SerializeField] float sens = 1.5f;

    [SerializeField] float movementIntensity = -5.625f;

    void Update()
    {
        //positioning
        targetModel.localPosition = offset;

        //animate
        bool lookLimit = freelook.mouseDelta.y == 90 || freelook.mouseDelta.y == -90;

        float moveDirection = (freeroam.ground) ? Input.GetAxisRaw("Horizontal") : 0;

        Quaternion mouseDelta = Quaternion.Euler((!lookLimit) ? Input.GetAxis("Mouse Y") * sens : 0, -Input.GetAxis("Mouse X") * sens, -Input.GetAxis("Mouse X") * sens);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, mouseDelta, smooth[0]);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(Vector3.forward * moveDirection * movementIntensity), smooth[1]);
    }
}
