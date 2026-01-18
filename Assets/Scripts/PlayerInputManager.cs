using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
   public Vector3 GetMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, vertical, 0);
        if (direction.magnitude > 1f)
        {
            return direction.normalized;
        }
        else
        {
            return direction.normalized;
        }

        
    }
}
