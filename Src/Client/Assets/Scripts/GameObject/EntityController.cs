using Assets.Scripts.Battle;
using Assets.Scripts.Entities;
using Assets.Scripts.Managers.Me;
using Entities;
using Managers;
using SkillBridge.Message;
using UnityEngine;

/// <summary>
/// 根据数据操作角色行为
/// </summary>
public class EntityController : MonoBehaviour, IEntityNotify, IEntityController
{
    public Animator anim;
    public Rigidbody rb;
    //private AnimatorStateInfo currentBaseState;

    public Entity entity;

    private Vector3 position;
    private Vector3 direction;
    private Quaternion rotation;

    private Vector3 lastPosition;
    private Quaternion lastRotation;

    //public float speed;
    //public float animSpeed = 1.5f;
    //public float jumpPower = 3.0f;

    public bool isPlayer = false;

    public RideController rideController;

    private int currentRide = 0;

    public Transform rideBone;

    public EntityEffectManager EffectMgr;

    // Use this for initialization
    void Start()
    {
        if (entity != null)
        {
            EntityManager.Instance.RegisterEntityChangeNotify(entity.entityId, this);
            //this.UpdateTransform(); //设置初始位置
        }

        if (!this.isPlayer)
            rb.useGravity = false;
    }

    void OnDestroy()
    {
        if (entity != null)
            Debug.LogFormat("{0} OnDestroy :ID:{1} POS:{2} DIR:{3} SPD:{4} ", this.name, entity.entityId, entity.position, entity.direction, entity.speed);

        if (UIWorldEleManagerMe.Instance != null)
        {
            UIWorldEleManagerMe.Instance.RemoveCharacterNameBar(this.transform);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.entity == null)
            return;

        //玩家位置更新
        this.entity.OnUpdate(Time.fixedDeltaTime);

        //玩家与怪物同步
        if (!this.isPlayer)
        {
            this.UpdateTransform();
        }
    }

    void UpdateTransform()
    {
        this.position = GameObjectTool.LogicToWorld(entity.position);
        this.direction = GameObjectTool.LogicToWorld(entity.direction);

        this.transform.forward = this.direction;
        this.lastPosition = this.position;
        this.lastRotation = this.rotation;

        this.rb.MovePosition(this.position);
    }

    /// <summary>
    /// 状态改变通知（执行实体动画事件）
    /// </summary>
    /// <param name="entityEvent"></param>
    public void OnEntityEvent(EntityEvent entityEvent, int param)
    {
        switch (entityEvent)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move", false);
                anim.SetTrigger("Idle");
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                anim.SetTrigger("Jump");
                break;
            case EntityEvent.Ride:
                {
                    this.Ride(param);
                }
                break;
        }
        if (this.rideController != null) this.rideController.OnEntityEvent(entityEvent, param);
    }


    /// <summary>
    /// 移除通知
    /// </summary>
    public void OnEntityRemoved()
    {
        if (UIWorldEleManagerMe.Instance != null)
        {
            UIWorldEleManagerMe.Instance.RemoveCharacterNameBar(transform);
        }
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 数据改变通知
    /// </summary>
    /// <param name="entity"></param>
    public void OnEntityChanged(Entity entity)
    {
        //Debug.LogFormat("OnEntityChanged :ID:{0} POS:{1} DIR:{2} SPD:{3} ", entity.entityId, entity.position, entity.direction, entity.speed);
    }




    public void Ride(int rideId)
    {
        if (currentRide == rideId) return;
        currentRide = rideId;
        if (rideId > 0)
        {
            this.rideController = GameObjectManager.Instance.LoadRide(rideId, this.transform);
        }
        else
        {
            Destroy(this.rideController.gameObject);
            this.rideController = null;
        }

        if (this.rideController == null)
        {
            this.anim.transform.localPosition = Vector3.zero;
            this.anim.SetLayerWeight(1, 0);
        }
        else
        {
            this.rideController.SetRider(this);
            this.anim.SetLayerWeight(1, 1);
        }
    }

    public void SetRidePotision(Vector3 position)
    {
        this.anim.transform.position = position + (this.anim.transform.position - this.rideBone.position);
    }

    void OnMouseDown()
    {
        Creature target = entity as Creature;
        if (target.IsCurrentPlayer)
            return;
        BattleManager.Instance.CurrentTarget = target;
    }

    public void PlayAnim(string name)
    {
        anim.SetTrigger(name);
    }

    public void SetStandby(bool standby)
    {
        anim.SetBool("Standby", standby);
    }

    public void UpdateDirection()
    {
        direction = GameObjectTool.LogicToWorld(entity.direction);
        transform.forward = direction;
        lastRotation = rotation;
    }

    public void PlayEffect(EffectType type, string name, Creature target, float duration)
    {
        Transform transform = target.Controller.GetTransform();
        if(type==EffectType.Position || type == EffectType.Hit)
        {
            FXManager.Instance.PlayEffect(type, name, transform, target.GetHitOffset(), duration);
        }
        else
        {
            EffectMgr.PlayEffect(type, name, transform, target.GetHitOffset(), duration);
        }
    }

    public void PlayEffect(EffectType type, string name, NVector3 position, float duration)
    {
        if (type == EffectType.Position || type == EffectType.Hit)
            FXManager.Instance.PlayEffect(type, name, null, GameObjectTool.LogicToWorld(position), duration);
        else
            EffectMgr.PlayEffect(type, name, null, GameObjectTool.LogicToWorld(position), duration);
    }

    public Transform GetTransform()
    {
        return this.transform;
    }
}
