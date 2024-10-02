using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeroam : MonoBehaviour
{
    [Header("Movement")]

    [SerializeField] CharacterController controller;

    [SerializeField] Gun gun;

    [Space(10)]

    [SerializeField] float speed = 5;
    [SerializeField] float speedAdd = 1.5f;
    [Range(0, 1)] [SerializeField] float smooth = 0.25f;

    [Space(10)]

    public bool moving;
    public bool backwards;

    public bool fast;

    Vector3 direction;

    [Space(10)]

    [SerializeField] Freelook freelook;
    float initialFov;
    [SerializeField] float fovSmooth = 0.125f;
    [SerializeField] float fovAdd = 5.25f;

    [Header("Gravity")]

    [SerializeField] LayerMask layer;

    public bool ground;

    [SerializeField] float gravityScale = 37.5f;
    float velo;

    [SerializeField] float jumpHeight = 10;

    void Start()
    {
        initialFov = freelook.cam.fieldOfView;
    }

    void Update()
    {
        freelook.cam.fieldOfView = Mathf.Lerp(freelook.cam.fieldOfView, initialFov + (fast ? fovAdd : 0), fovSmooth);

        direction = Vector3.Lerp(direction, MoveInput(), smooth);
        Vector3 down = Gravity();

        float fastAdd = (fast ? speedAdd : 1);
        Vector3 move = (direction * (speed + fastAdd)) + down;
        controller.Move(move);

        if (Input.GetKeyDown(KeyCode.Space) && ground && !Token.inShop)
            velo = jumpHeight;

        fast = (Input.GetKey(KeyCode.LeftShift) && moving && !backwards && !gun.reloadAction && !gun.fireAction);
    }

    Vector3 MoveInput()
    {
        Vector3 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 moveInput = Vector3.Normalize(input);

        moving = moveInput.magnitude > 0;
        backwards = input.y < 0;

        Vector3 top = (transform.forward * moveInput.y) * Time.deltaTime;
        Vector3 left = (transform.right * moveInput.x) * Time.deltaTime;

        //give direction
        return (top + left);
    }

    Vector3 Gravity()
    {
        RaycastHit hit;
        ground = Physics.BoxCast(transform.position, new Vector3(controller.radius - 0.1f, controller.radius / 2, controller.radius - 0.1f), -transform.up, out hit, Quaternion.identity, controller.height / 2, layer);

        //velo = (!ground ? velo - gravityScale : -1) * Time.deltaTime;

        if (ground && velo <= 0)
            velo = -2.5f;
        else
            velo -= gravityScale * Time.deltaTime;

        velo = Mathf.Clamp(velo, -25, 25);

        //give down
        return (transform.up * velo) * Time.deltaTime;
    }
}
