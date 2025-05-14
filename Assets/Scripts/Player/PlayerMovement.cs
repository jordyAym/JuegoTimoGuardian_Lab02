using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  private Rigidbody2D body;
  private SpriteRenderer spriteRenderer;
  private Animator anim;

  [SerializeField] private float speed ;
  [SerializeField] private float jumpForce;
  private bool grounded;

  //Scale 
  private Vector3 targetScale;
  private Vector3 normalScale = new Vector3(10f, 10f, 1f);
  private Vector3 smallScale = new Vector3(5f, 5f, 1f);
  private float normalSpeed = 5f;
  private float smallSpeed = 9f;

  private float normalJumpForce = 8f;
  private float smallJumpForce = 5f;
  private bool isSmall = false;
  private float scaleLerpSpeed = 5f;
  void Awake()
  {
    body = GetComponent<Rigidbody2D>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    anim = GetComponent<Animator>();
    speed = normalSpeed;
    targetScale = normalScale;
    jumpForce = normalJumpForce;
  }

  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    HandleScale();
    // transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleLerpSpeed);

    //Handle left and right movement
    float moveX = Input.GetAxisRaw("Horizontal");
    if (moveX > 0)
      spriteRenderer.flipX = false;
    else if (moveX < 0)
      spriteRenderer.flipX = true;
    body.velocity = new Vector2(moveX * speed, body.velocity.y);
    //Handle jump
    if (Input.GetKeyDown(KeyCode.Space) && grounded)
    {
      Jump();
    }

    anim.SetBool("isRunning", moveX != 0);
    anim.SetBool("isGrounded", grounded);
  }

  private void Jump()
  {
    body.velocity = new Vector2(body.velocity.x, jumpForce);
    anim.SetTrigger("jump");
    grounded = false;
  }

  void HandleScale()
  {
    if (Input.GetKeyDown(KeyCode.E))
    {
      isSmall = !isSmall;

      if (isSmall)
      {
        targetScale = smallScale;
        transform.localScale = targetScale;
        speed = smallSpeed;
        jumpForce = smallJumpForce;
      }
      else
      {
        targetScale = normalScale;
        transform.localScale = targetScale;

        speed = normalSpeed;
        jumpForce = normalJumpForce;
      }
    }
  }

  private void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.gameObject.tag == "Ground")
      grounded = true;
  }
}
