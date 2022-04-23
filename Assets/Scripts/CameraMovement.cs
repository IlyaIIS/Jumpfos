using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform Target;
    private Camera cam;
    void Start()
    {
        cam = GetComponent<Camera>();
        transform.position = Target.position;
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(transform.position.x - Target.position.x) > 4 || //todo: �������� ��������� 3 � 4 �� ����������, ��������� �� ������� ������� ������� ��������� (���������� �� ������ ������, ����� �������� ������ �������� ��������)
            Mathf.Abs(transform.position.y - Target.position.y) > 3)
            transform.position = Vector3.Lerp(transform.position,Target.position,0.001f);

        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }
}
