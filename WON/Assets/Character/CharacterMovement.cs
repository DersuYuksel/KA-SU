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
        // Animator ve CharacterController bileþenlerine eriþiyoruz
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>(); // Karakterin fiziksel hareketi için
    }

    void Update()
    {
        // Kullanýcý giriþlerini alýyoruz (WASD tuþlarý)
        float x = Input.GetAxis("Horizontal"); // A ve D tuþlarýyla X ekseninde hareket
        float y = Input.GetAxis("Vertical");   // W ve S tuþlarýyla Y ekseninde hareket

        // X ve Y deðerlerini animatördeki Blend Tree parametrelerine atýyoruz
        animator.SetFloat("X", x * 1f); // X için -0.5 sola, 0.5 saða hareketi temsil ediyor
        animator.SetFloat("Y", Mathf.Abs(y*2)); // Y: 1 koþma, 0.5 yürüme, 0 durma olacak þekilde ayarlanýr

        // Karakterin hýzýný hesapla (yürüme veya koþma)
        float speed = (y > 0.5f) ? runSpeed : walkSpeed;

        // Hareket vektörünü hesaplýyoruz
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
