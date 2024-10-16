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
    private bool isAttacking = false;  // Sald�r� s�ras�nda hareketi engellemek i�in flag

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Yerde olup olmad���n� kontrol et
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  // Yere d��t���m�zde z�plama sonras� h�z� s�f�rla
        }

        // Sald�r� ba�lad���nda hareketi durdur
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            anim.SetTrigger("Attack");
            isAttacking = true;  // Sald�r� ba�lad���nda hareketi engelle
        }

        // Sald�r� animasyonu bitene kadar bekle
        if (isAttacking)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            // Sald�r� animasyonu bitene kadar hareket engellenir
            if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 1.0f)
            {
                isAttacking = false;  // Animasyon bitti�inde hareketi tekrar a�
            }
        }

        // Hareketi sadece sald�r� yap�lm�yorsa aktif et
        if (!isAttacking)
        {
            HandleMovement();  // Hareket ve z�plama kontrollerini yap
        }

        // Z�plama i�lemi, yaln�zca sald�r� yap�lm�yorsa
        if (Input.GetButtonDown("Jump") && isGrounded && !isAttacking)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Yer�ekimi etkisi
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Hareket vekt�r�n� hesapla
        Vector3 move = transform.right * x + transform.forward * z;

        // E�er sald�r� animasyonu bitmi�se, karakteri hareket ettir
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Y�n animasyonlar�
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

    // Belirli bir animasyonun oynay�p oynamad���n� kontrol et
    bool IsAnimationPlaying(string animationName)
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }
}
