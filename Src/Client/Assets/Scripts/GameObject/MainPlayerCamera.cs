using Models;
using UnityEngine;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    //public Transform viewPoint;W
    public GameObject player;

    public float followSpeed = 5f;

    public float rotateSpeed = 5f;

    Quaternion yaw = Quaternion.identity;


    void Start()
    {
        this.transform.position = player.transform.position;
        this.transform.rotation = player.transform.rotation;
    }

    void LateUpdate()
    {
        //保证摄像机照到角色
        if (player == null && User.Instance.CurrentCharacterObject != null)
        {
            player = User.Instance.CurrentCharacterObject.gameObject;
        }

        if (player == null)
            return;

        //if (!UIManager.Instance.isOpen)
        //{
        //    this.transform.position = player.transform.position;
        //    this.transform.rotation = player.transform.rotation;
        //}

        //if(transform.rotation != player.transform.rotation)
        //{
        //    Debug.Log(transform.rotation + " != "+ player.transform.rotation);
        //}

        transform.position = Vector3.Lerp(transform.position, player.transform.position, Time.deltaTime * followSpeed);
        if (Input.GetMouseButton(1))
        {
            Vector3 angleBase = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(angleBase.x - Input.GetAxis("Mouse Y") * rotateSpeed, angleBase.y + Input.GetAxis("Mouse X") * rotateSpeed, 0);
            Vector3 angle = transform.rotation.eulerAngles - player.transform.rotation.eulerAngles;
            angle.z = 0;
            yaw = Quaternion.Euler(angle);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, player.transform.rotation * yaw, Time.deltaTime * followSpeed);
        }

        if (Input.GetAxis("Vertical") > 0.01)
        {
            yaw = Quaternion.Lerp(yaw, Quaternion.identity, Time.deltaTime * followSpeed);
        }
    }
}
