using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    public void MoveCharacterToPosition(Vector3 position)
    {
        targetPosition = position;
        isMoving = true;
    }

    private void Update()
    {
        if (isMoving)
        {
            // Tính toán hướng và khoảng cách đến điểm đích
            Vector3 direction = (targetPosition - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetPosition);

            // Di chuyển theo hướng với tốc độ đã cho
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Kiểm tra xem đã đến điểm đích chưa
            if (distance < 0.01f)
            {
                isMoving = false;
            }
        }
    }

}
