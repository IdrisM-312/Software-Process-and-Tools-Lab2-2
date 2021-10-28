using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform target;//镜头跟踪对象
    public float smoothing;//平滑度

    public Vector2 minPosition;//镜头移动边界最小位置
    public Vector2 maxPosition;//镜头移动边界最大位置
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
                Vector3 targetPos = target.position;//跟踪对象的位置
                targetPos.x = Mathf.Clamp(targetPos.x, minPosition.x, maxPosition.x);
                targetPos.y = Mathf.Clamp(targetPos.y, minPosition.y, maxPosition.y);
                transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);//没太看懂，总之就是能跟
            }
        }
    }

    //up说这个用于在切换场景时更方便地修改边界
    public void SetCamPosLimit(Vector2 minPos, Vector2 maxPos)
    {
        minPosition = minPos;
        maxPosition = maxPos;
    }
}
