using UnityEngine;

public class PlayerCubeMovement : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;

    PlayerInputManager playerInputManager;
    Rigidbody rb;
    Vector3 move;

    private void Awake()
    {
        //Debug.Log("PlayerCubeMovement Awake", gameObject);
        playerInputManager = GetComponent<PlayerInputManager>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        move = playerInputManager.GetMovement();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = move * speed;
    }
}
