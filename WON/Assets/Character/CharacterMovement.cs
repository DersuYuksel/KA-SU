using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public LayerMask layerMask; 

    [Range(0f, 1f)]
    public float DistancetoGround;

    private Animator animator;
    public float walkSpeed = 2.0f;
    public float runSpeed = 4.0f;
    private CharacterController controller;

    void Start()
    {
        // Animator ve CharacterController bile�enlerine eri�iyoruz
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>(); // Karakterin fiziksel hareketi i�in
    }

    void Update()
    {
        // Kullan�c� giri�lerini al�yoruz (WASD tu�lar�)
        float x = Input.GetAxis("Horizontal"); // A ve D tu�lar�yla X ekseninde hareket
        float y = Input.GetAxis("Vertical");   // W ve S tu�lar�yla Y ekseninde hareket

        // X ve Y de�erlerini animat�rdeki Blend Tree parametrelerine at�yoruz
        animator.SetFloat("X", x * 1f); // X i�in -0.5 sola, 0.5 sa�a hareketi temsil ediyor
        animator.SetFloat("Y", Mathf.Abs(y*2)); // Y: 1 ko�ma, 0.5 y�r�me, 0 durma olacak �ekilde ayarlan�r

        // Karakterin h�z�n� hesapla (y�r�me veya ko�ma)
        float speed = (y > 0.5f) ? runSpeed : walkSpeed;

        // Hareket vekt�r�n� hesapl�yoruz
        Vector3 move = transform.right * x + transform.forward * y;

        // CharacterController kullanarak hareket ettir
        controller.Move(move * speed * Time.deltaTime);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);

            //Left Foot
            RaycastHit hit;
            Ray ray = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
            if(Physics.Raycast(ray, out hit, DistancetoGround + 1f, layerMask)) {

                if (hit.transform.tag == "Walkable")
                {
                    Vector3 footPos = hit.point;
                    footPos.y += DistancetoGround;
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPos);
                }

            }
        }
    }
}
