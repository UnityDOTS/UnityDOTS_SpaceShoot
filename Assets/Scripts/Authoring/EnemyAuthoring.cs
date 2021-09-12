using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS
{
    [ConverterVersion("joe", 1)]
    public class EnemyAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject m_DestructionParticle; // 爆炸粒子
        public GameObject m_Bullet; // 子弹

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new MovementComponent());
            dstManager.AddComponentData(entity, new EnemyComponent() { ChangeDestinationCountdown = 0.1f, DestructionParticle = conversionSystem.GetPrimaryEntity(m_DestructionParticle) });
            dstManager.AddComponentData(entity, new ShooterComponent() { StartPoint = float3.zero, BulletEntity = conversionSystem.GetPrimaryEntity(m_Bullet), ShootIntervalTime = 0.5f, ShootingCountdown = 0.1f });
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(m_DestructionParticle);
            referencedPrefabs.Add(m_Bullet);
        }
    }
}