using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    // ���嶯������5��bool����
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsForwardWalking = Animator.StringToHash("IsForwardWalking");
    private static readonly int IsBackWalking = Animator.StringToHash("IsBackWalking");
    private static readonly int IsRightWalking = Animator.StringToHash("IsRightWalking");
    private static readonly int IsLeftWalking = Animator.StringToHash("IsLeftWalking");



    void Awake()
    {
        animator = GetComponent<Animator>();
    }


    public void UpdateAnimation(Vector2 moveDirection, float currentAngle)
    {
        // �Ȱ����е�����״̬���Ϊ false
        animator.SetBool(IsWalking, false);
        animator.SetBool(IsForwardWalking, false);
        animator.SetBool(IsBackWalking, false);
        animator.SetBool(IsRightWalking, false);
        animator.SetBool(IsLeftWalking, false);


        if (moveDirection != Vector2.zero)
        {
            animator.SetBool(IsWalking, true);

            float angle = currentAngle;
            //Debug.Log($"MoveDirection: {moveDirection}, Angle: {angle}");

            if (angle >= -45f && angle <= 45f) // ����
            {
                animator.SetBool(IsRightWalking, true);
            }
            else if (angle >= 135f || angle <= -135f) // ����
            {
                animator.SetBool(IsLeftWalking, true);
            }
            else if (angle > 45f && angle < 135f) // ���ϣ���������
            {
                animator.SetBool(IsBackWalking, true);
            }
            else if (angle < -45f && angle > -135f) // ���£���������
            {
                animator.SetBool(IsForwardWalking, true);
            }
        }
    }
    public void SetIdleAnimation()
    {
        animator.SetBool(IsWalking, false);
        animator.SetBool(IsForwardWalking, false);
        animator.SetBool(IsBackWalking, false);
        animator.SetBool(IsRightWalking, false);
        animator.SetBool(IsLeftWalking, false);

        // ǿ���л��� Idle
        animator.Play("Idle");
    }
}