using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    // 定义动画机中5个bool参数
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
        // 先把所有的行走状态标记为 false
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

            if (angle >= -45f && angle <= 45f) // 向右
            {
                animator.SetBool(IsRightWalking, true);
            }
            else if (angle >= 135f || angle <= -135f) // 向左
            {
                animator.SetBool(IsLeftWalking, true);
            }
            else if (angle > 45f && angle < 135f) // 向上，背对着走
            {
                animator.SetBool(IsBackWalking, true);
            }
            else if (angle < -45f && angle > -135f) // 向下，正对着走
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

        // 强制切换到 Idle
        animator.Play("Idle");
    }
}
