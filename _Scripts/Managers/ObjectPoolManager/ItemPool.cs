using UnityEngine;

public class ItemPool : GenericPoolableObject
{
    //[SerializeField]
    //TypeObject typeObject;
    //public BulletMovementComponent moveComponent;
    //public AutoDestroy autoDestroy;

    //private void Awake()
    //{
    //    switch (typeObject)
    //    {
    //        case TypeObject.bullet:
    //            moveComponent = AddComponent<BulletMovementComponent>();
    //            break;
    //        case TypeObject.explosion:
    //            autoDestroy = GetComponent<AutoDestroy>();
    //            break;
    //        default:
    //            break;
    //    }
    //}

    public override void PrepareToUse()
    {
        base.PrepareToUse();
    }

    //public enum TypeObject
    //{
    //    bullet,
    //    explosion,

    //}
}
