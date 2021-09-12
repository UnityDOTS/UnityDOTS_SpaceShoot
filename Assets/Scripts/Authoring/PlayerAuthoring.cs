using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS
{
    [ConverterVersion("joe", 1)]
    public class PlayerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public int m_Life; // 生命数
        public GameObject m_DestructionParticle; // 爆炸粒子
        public GameObject m_Bullet; // 子弹

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new MovementComponent() { Speed = 8.0f });
            dstManager.AddComponentData(entity, new PlayerComponent() { Score = 0, Life = m_Life, IsTouch = true, DestructionParticle = conversionSystem.GetPrimaryEntity(m_DestructionParticle) });
            dstManager.AddComponentData(entity, new ShooterComponent() { StartPoint = new float3(0, 0, 0.8f), BulletEntity = conversionSystem.GetPrimaryEntity(m_Bullet), ShootIntervalTime = 0.2f, ShootingCountdown = 0.0f });
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(m_DestructionParticle);
            referencedPrefabs.Add(m_Bullet);
        }
    }
}