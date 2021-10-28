using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform target;//��ͷ���ٶ���
    public float smoothing;//ƽ����

    public Vector2 minPosition;//��ͷ�ƶ��߽���Сλ��
    public Vector2 maxPosition;//��ͷ�ƶ��߽����λ��
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {
            if (transform.position != target.position)
            {
                Vector3 targetPos = target.position;//���ٶ����λ��
                targetPos.x = Mathf.Clamp(targetPos.x, minPosition.x, maxPosition.x);
                targetPos.y = Mathf.Clamp(targetPos.y, minPosition.y, maxPosition.y);
                transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);//û̫��������֮�����ܸ�
            }
        }
    }

    //up˵����������л�����ʱ��������޸ı߽�
    public void SetCamPosLimit(Vector2 minPos, Vector2 maxPos)
    {
        minPosition = minPos;
        maxPosition = maxPos;
    }
}
