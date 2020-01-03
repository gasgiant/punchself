using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator headAnimator;
    public float punchForce;

    public bool InBiteProcess { get { return biteRoutine != null; } }
    public bool isBiting;

    Animator animator;

    bool isleanedleft = false;
    bool isleanedright = false;

    Coroutine biteRoutine;


    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TakePunch(float multiplyer, bool blocked)
    {
        float force = multiplyer * punchForce;
        if (blocked)
        {
            force *= 0.2f;
            rb.AddForce(Vector2.right * force, ForceMode2D.Impulse);
        }
        else
        {
            StopBite();
            if (Mathf.Sign(rb.velocity.x) == multiplyer && Mathf.Abs(rb.velocity.x) > 1)
                force *= 2f;
            rb.AddForce(Vector2.right * force, ForceMode2D.Impulse);
            if (multiplyer > 0)
            {
                isleanedright = true;
                animator.SetBool("IsMovingRight", true);
                headAnimator.SetTrigger("PunchedRight");
                ObjectPooler.Instance.SpawnFromPool("HitParticles",
                    headAnimator.transform.position - Vector3.forward * 10, Quaternion.identity);
            }
            else
            {
                isleanedleft = true;
                animator.SetBool("IsMovingLeft", true);
                headAnimator.SetTrigger("PunchedLeft");
                ObjectPooler.Instance.SpawnFromPool("HitParticles",
                    headAnimator.transform.position - Vector3.forward * 10, Quaternion.identity);
            }
        }
    }

    public void StartBite()
    {
        if (biteRoutine == null)
            biteRoutine = StartCoroutine(BiteRoutine());
    }

    public void StopBite()
    {
        if (biteRoutine != null)
        {
            isBiting = false;
            StopCoroutine(biteRoutine);
            biteRoutine = null;
        }
    }

    IEnumerator BiteRoutine()
    {
        headAnimator.SetTrigger("StartBite");
        yield return new WaitForSeconds(20f / 60);
        isBiting = true;
        AudioManager.Instance.PlaySound("Bite");
        yield return new WaitForSeconds(4f / 60);
        CameraController.RandomShake(2, 0.05f, 3, 0, 1);

        isBiting = false;
        biteRoutine = null;
    }

    private void Update()
    {
        if (isleanedleft && rb.velocity.x > -3)
        {
            isleanedleft = false;
            animator.SetBool("IsMovingLeft", false);
        }

        if (isleanedright && rb.velocity.x < 3)
        {
            isleanedright = false;
            animator.SetBool("IsMovingRight", false);
        }

        
    }
}
