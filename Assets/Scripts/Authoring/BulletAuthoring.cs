using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    [ConverterVersion("joe", 1)]
    public class BulletAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new MovementComponent());
            dstManager.AddComponentData(entity, new BulletComponent());
            dstManager.AddComponentData(entity, new DelayDestroyComponent() { delay = 1.7f });
        }
    }
}