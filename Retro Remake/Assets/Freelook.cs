using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freelook : MonoBehaviour
{
    public Camera cam;

    [SerializeField] Transform camBone;
    Quaternion camBoneOffset;

    [Space(10)]

    public float sens = 1;
    [Range(0, 1)] [SerializeField] float smooth = 0.625f;

    [HideInInspector] public Vector2 mouseDelta;

    void Start()
    {
        camBoneOffset = camBone.transform.localRotation;
    }

    void Update()
    {
        mouseDelta += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        mouseDelta.y = Mathf.Clamp(mouseDelta.y, -90, 90);

        Vector3 camDirection = (Vector3.left * mouseDelta.y) * sens;
        Vector3 charDiretion = (Vector3.up * mouseDelta.x) * sens;

        cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, Quaternion.Euler(camDirection - camBoneOffset.eulerAngles) * (camBone.localRotation), smooth);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(charDiretion), smooth);

        //cam.transform.localPosition = camBone.transform.localPosition + Vector3.up;
    }
}
