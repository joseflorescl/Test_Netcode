using UnityEngine;

public class PlayerCubeMovement : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;

    PlayerInputManager playerInputManager;
    Rigidbody rb;
    Vector3 move;

    IPlayerAuthorityProvider authorityProvider;
    bool hasAuthority;

    private void Awake()
    {
        Debug.Log("PlayerCubeMovement Awake", gameObject);

        playerInputManager = GetComponent<PlayerInputManager>();
        rb = GetComponent<Rigidbody>();
        authorityProvider = GetComponent<IPlayerAuthorityProvider>();

        // Permitir singleplayer, y como OnNetworkSpawn puede ocurrir antes:
        if (authorityProvider == null)
        {
            hasAuthority = true;
            ManageAuthority();
        }

    }

    private void OnEnable()
    {
        if (authorityProvider != null)
            authorityProvider.OnAuthorityChanged += OnAuthorityChanged;
    }    

    private void OnDisable()
    {
        if (authorityProvider != null)
            authorityProvider.OnAuthorityChanged -= OnAuthorityChanged;
    }
    
    private void OnAuthorityChanged(bool value)
    {
        hasAuthority = value;
        ManageAuthority();
    }

    void ManageAuthority()
    {
        if (!hasAuthority)
        {
            enabled = false; //Destroy(this)?
        }
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
