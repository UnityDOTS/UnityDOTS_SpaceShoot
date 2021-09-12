using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DOTS
{
    /// <summary>
    /// Player同步表现层
    /// </summary>
    public class PlayerVisual : MonoBehaviour
    {
        private Entity _player;
        private Transform CacheTransform;

        private void Awake()
        {
            CacheTransform = transform;
        }

        private void Update()
        {
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            _player = em.CreateEntityQuery(typeof(PlayerComponent)).GetSingletonEntity();
            if (em.Exists(_player))
            {
                CacheTransform.position = em.GetComponentData<Translation>(_player).Value;
                var playerComponent = em.GetComponentData<PlayerComponent>(_player);
                CacheTransform.localScale = playerComponent.Life == 0 ? Vector3.zero : Vector3.one;
            }
        }
    }
}