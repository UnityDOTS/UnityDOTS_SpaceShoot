using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    [ConverterVersion("joe", 1)]
    public class AsteroidAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponents(entity, new ComponentTypes(
                typeof(MovementComponent),
                typeof(RotationComponent),
                typeof(AsteroidComponent)));
        }
    }
}