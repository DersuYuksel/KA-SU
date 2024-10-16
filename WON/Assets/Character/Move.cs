using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Move : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Animator anim;
    private bool isAttacking = false;  // Saldýrý sýrasýnda hareketi engellemek için flag

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Yerde olup olmadýðýný kontrol et
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  // Yere düþtüðümüzde zýplama sonrasý hýzý sýfýrla
        }

        // Saldýrý baþladýðýnda hareketi durdur
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            anim.SetTrigger("Attack");
            isAttacking = true;  // Saldýrý baþladýðýnda hareketi engelle
        }

        // Saldýrý animasyonu bitene kadar bekle
        if (isAttacking)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            // Saldýrý animasyonu bitene kadar hareket engellenir
            if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 1.0f)
            {
                isAttacking = false;  // Animasyon bittiðinde hareketi tekrar aç
            }
        }

        // Hareketi sadece saldýrý yapýlmýyorsa aktif et
        if (!isAttacking)
        {
            HandleMovement();  // Hareket ve zýplama kontrollerini yap
        }

        // Zýplama iþlemi, yalnýzca saldýrý yapýlmýyorsa
        if (Input.GetButtonDown("Jump") && isGrounded && !isAttacking)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Yerçekimi etkisi
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Hareket vektörünü hesapla
        Vector3 move = transform.right * x + transform.forward * z;

        // Eðer saldýrý animasyonu bitmiþse, karakteri hareket ettir
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Yön animasyonlarý
        if (Input.GetKeyDown(KeyCode.W))
        {
            anim.SetFloat("Forward", 1);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            anim.SetFloat("Forward", 0);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            anim.SetFloat("Back", 1);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            anim.SetFloat("Back", 0);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            anim.SetFloat("Left", 1);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            anim.SetFloat("Left", 0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            anim.SetFloat("Right", 1);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            anim.SetFloat("Right", 0);
        }
    }

    // Belirli bir animasyonun oynayýp oynamadýðýný kontrol et
    bool IsAnimationPlaying(string animationName)
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }
}
