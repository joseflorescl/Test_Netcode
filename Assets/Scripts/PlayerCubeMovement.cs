using UnityEngine;

public class PlayerCubeMovement : BasePlayerAuthorityProvider
{
    [SerializeField] float speed = 1.0f;

    PlayerInputManager playerInputManager;
    Rigidbody rb;
    Vector3 move;

    protected override void Awake()
    {
        base.Awake();

        Debug.Log("PlayerCubeMovement Awake", gameObject);
        playerInputManager = GetComponent<PlayerInputManager>();
        rb = GetComponent<Rigidbody>();
    }          

    private void Start()
    {
        print("PlayerCubeMovement Start");
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
