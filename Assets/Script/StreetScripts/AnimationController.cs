using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    // 定义动画机中5个布尔参数
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsForwardWalking = Animator.StringToHash("IsForwardWalking");
    private static readonly int IsBackWalking = Animator.StringToHash("IsBackWalking");
    private static readonly int IsRightWalking = Animator.StringToHash("IsRightWalking");
    private static readonly int IsLeftWalking = Animator.StringToHash("IsLeftWalking");



    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // 更新动画状态的方法
    public void UpdateAnimation(Vector2 moveDirection, float currentAngle)
    {
        // 先把所有的行走状态标记为 false
        animator.SetBool(IsWalking, false);
        animator.SetBool(IsForwardWalking, false);
        animator.SetBool(IsBackWalking, false);
        animator.SetBool(IsRightWalking, false);
        animator.SetBool(IsLeftWalking, false);


        // 如果当前敌人有移动方向
        if (moveDirection != Vector2.zero)
        {
            animator.SetBool(IsWalking, true); // 设置为行走状态

            // 计算与水平 X 轴之间的角度
            float angle = currentAngle;
            //Debug.Log($"MoveDirection: {moveDirection}, Angle: {angle}");

            // 角度判断逻辑
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
        // 停止所有行走动画，确保敌人处于静止状态
        animator.SetBool(IsWalking, false);
        animator.SetBool(IsForwardWalking, false);
        animator.SetBool(IsBackWalking, false);
        animator.SetBool(IsRightWalking, false);
        animator.SetBool(IsLeftWalking, false);

        // 强制切换到 Idle 动画状态
        animator.Play("Idle");
    }
}
