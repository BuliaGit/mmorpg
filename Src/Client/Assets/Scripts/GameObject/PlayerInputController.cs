using UnityEngine;

using Entities;
using SkillBridge.Message;
using Services;
using Managers;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Runtime.Remoting;

public class PlayerInputController : MonoBehaviour
{
    public Rigidbody rb;
    CharacterState state;

    public Creature character;

    public float rotateSpeed = 2.0f;
    public float moveSpeed = 1;
    public float turnAngle = 10;
    public int speed;

    public EntityController entityController;

    public bool onAir = false;

    private Transform camTrans;
    private Vector3 camOffset;

    private NavMeshAgent agent;

    private bool autoNav = false;

    
    void Start()
    {
        state = CharacterState.Idle;

        //if (this.character == null)
        //{
        //    DataManager.Instance.Load();
        //    NCharacterInfo cinfo = new NCharacterInfo();
        //    cinfo.Id = 1;
        //    cinfo.Name = "Test";
        //    cinfo.ConfigId = 1;
        //    cinfo.Entity = new NEntity();
        //    cinfo.Entity.Position = new NVector3();
        //    cinfo.Entity.Direction = new NVector3();
        //    cinfo.Entity.Direction.X = 0;
        //    cinfo.Entity.Direction.Y = 100;
        //    cinfo.Entity.Direction.Z = 0;
        //    cinfo.attrDynamic = new NAttributeDynamic();
        //    this.character = new Creature(cinfo);

        //    if (entityController != null) entityController.entity = this.character;
        //}

        if (agent == null)
        {
            agent=gameObject.AddComponent<NavMeshAgent>();
            agent.stoppingDistance = 0.5f;
        }

        camTrans = Camera.main.transform;
        camOffset = transform.position - camTrans.position;
    }

    public void StartNav(Vector3 target)
    {
        StartCoroutine(BeginNav(target));
    }

    IEnumerator BeginNav(Vector3 target)
    {
        agent.SetDestination(target);
        yield return null;
        autoNav = true;
        if (state != CharacterState.Move)
        {
            state = CharacterState.Move;
            character.MoveForward();
            SendEntityEvent(EntityEvent.MoveFwd);
            agent.speed = character.speed / 100f;
        }
    }
    public void StopNav()
    {
        autoNav = false;
        agent.ResetPath();
        if (state != CharacterState.Idle)
        {
            state=CharacterState.Idle;
            rb.velocity = Vector3.zero;
            character.Stop();
            SendEntityEvent(EntityEvent.Idle);
        }
        //NavPathRender.Instance.SetPath(null, Vector3.zero);
    }

    public void NavMove()
    {
        if (agent.pathPending) return;
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            StopNav();
            return;
        }
        if (agent.pathStatus != NavMeshPathStatus.PathComplete) return;

        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f)
        {
            StopNav();
            return;
        }
        //NavPathRender.Instance.SetPath(agent.path, agent.destination);
        if (agent.isStopped || agent.remainingDistance < 1)
        {
            StopNav();
            return;
        }
    }
    void FixedUpdate()
    {
        if (character == null)
            return;

        if (autoNav)
        {
            NavMove();
            return;
        }

        if (UIManagerMe.Instance.isOpen) //打开UI面板不能移动
            return;

        if (InputManager.Instance !=null && InputManager.Instance.isInputMode) return;

        #region 向前向后
        float v = Input.GetAxis("Vertical");
        if (v > 0.01)
        {
            if (state != CharacterState.Move)
            {
                state = CharacterState.Move;
                this.character.MoveForward();
                this.SendEntityEvent(EntityEvent.MoveFwd);
            }
            //移动核心
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) * moveSpeed / 100f;
        }
        else if (v < -0.01)
        {
            if (state != CharacterState.Move)
            {
                state = CharacterState.Move;
                this.character.MoveBack();
                this.SendEntityEvent(EntityEvent.MoveBack);
            }
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
        }
        else
        {
            if (state != CharacterState.Idle)
            {
                state = CharacterState.Idle;
                this.rb.velocity = Vector3.zero;
                this.character.Stop();
                this.SendEntityEvent(EntityEvent.Idle);
            }
        }
        #endregion

        #region 向左向右
        float h = Input.GetAxis("Horizontal");
        if (h < -0.1 || h > 0.1)
        {
            this.transform.Rotate(0, h * rotateSpeed, 0);
            Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
            Quaternion rot = new Quaternion();
            rot.SetFromToRotation(dir, this.transform.forward);

            if (rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
            {
                character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
                rb.transform.forward = this.transform.forward;
                this.SendEntityEvent(EntityEvent.None);
            }
        }
        #endregion

        #region 跳跃
        if (Input.GetButtonDown("Jump"))
        {
            this.SendEntityEvent(EntityEvent.Jump);
        }
        #endregion
    }

    Vector3 lastPos;
    float lastSync = 0;

    void LateUpdate()
    {
        if (this.character == null) return;

        Vector3 offset = this.rb.transform.position - lastPos;
        this.speed = (int)(offset.magnitude * 100f / Time.deltaTime);
        ////Debug.LogFormat("LateUpdate velocity {0} : {1}", this.rb.velocity.magnitude, this.speed);
        this.lastPos = this.rb.transform.position;

        ////防止不同步
        Vector3Int goLogicPos = GameObjectTool.WorldToLogic(this.rb.transform.position);
        float logicOffset = (goLogicPos - this.character.position).magnitude;
        if (logicOffset > 50)
        {
            this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
            this.SendEntityEvent(EntityEvent.None);
        }

        this.transform.position = this.rb.transform.position;

        Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
        Quaternion rot = new Quaternion();
        rot.SetFromToRotation(dir, this.transform.forward);

        if(rot.eulerAngles.y>this.turnAngle && rot.eulerAngles.y < (360 - turnAngle))
        {
            character.SetDirection(GameObjectTool.WorldToLogic(transform.forward));
            SendEntityEvent(EntityEvent.None);
        }
    }


    /// <summary>
    /// 发送实体事件
    /// </summary>
    /// <param name="entityEvent">事件枚举</param>
    public void SendEntityEvent(EntityEvent entityEvent,int rideId = 0)
    {
        //播放我的动画
        if (entityController != null)       
        {
            entityController.OnEntityEvent(entityEvent,rideId);
        }

        //向其他玩家同步我的位置信息
        MapService.Instance.SendMapEntitySync(entityEvent, this.character.EntityData, rideId);
    }
}
