using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float gravity;
    public float jumpForce;
    private Rigidbody rb;
    public Transform currentPlanet;
    public Transform playerVisual;

    RaycastHit[] hits;
    Vector3 planetDir;
    Vector3 normalVector;
    Vector3 input;

    public bool isTouchingPlanetSurface = false;
    private Transform MainCameraTransform;
    public Transform CameraArmTransform;
    PlayerControls playerControls;
    Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        MainCameraTransform = Camera.main.transform;
        playerControls = new PlayerControls();
        playerControls.Movement.GamepadEastButton.performed += Jump;
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void FixedUpdate()
    {
        Movement();
        ApplyGravity();
        ApplyPlanetRotation();
    }

    void Jump(InputAction.CallbackContext context)
    {
        rb.velocity *= 0;
        rb.AddForce(normalVector * jumpForce, ForceMode.Impulse);
    }

    void Movement()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 cameraRotation = new Vector3(0, MainCameraTransform.localEulerAngles.y + CameraArmTransform.localEulerAngles.y, 0);
        Vector3 Dir = Quaternion.Euler(cameraRotation) * input;
        Vector3 movement_dir = (transform.forward * Dir.z + transform.right * Dir.x);
        Vector3 currentNormalVelocity = Vector3.Project(rb.velocity,normalVector.normalized);
        rb.velocity = currentNormalVelocity + (movement_dir * speed) ;

        if (movement_dir != Vector3.zero)
        {
            anim.SetBool("IsMoving", true);
            playerVisual.localRotation = Quaternion.LookRotation(Dir);
        }
        else
        {
            anim.SetBool("IsMoving", false);
        }

    }

    void ApplyGravity()
    {
        if (currentPlanet == null) return;

        hits = Physics.RaycastAll(transform.position, -transform.up, 1.0f);

        if (hits.Length == 0)
        {
            planetDir = currentPlanet.position - transform.position;
            hits = Physics.RaycastAll(transform.position, planetDir, 1.0f);
        }
        
        GetPlanetNormal();
        rb.AddForce(normalVector.normalized * gravity, ForceMode.Acceleration);
        hits = new RaycastHit[0];
    }

    void ApplyPlanetRotation()
    {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, normalVector) * transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 5f * Time.fixedDeltaTime);
    }

    void GetPlanetNormal()
    {
        if (currentPlanet == null) return;
        normalVector = (transform.position - currentPlanet.position).normalized;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform == currentPlanet)
            {
                normalVector = hits[i].normal.normalized;
                break;
            }
        }

        return;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == currentPlanet)
        {
            isTouchingPlanetSurface = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == currentPlanet)
        {
            isTouchingPlanetSurface = false;
        }
    }

}
