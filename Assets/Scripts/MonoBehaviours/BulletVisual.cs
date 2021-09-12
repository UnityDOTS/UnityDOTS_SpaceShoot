using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DOTS
{
    /// <summary>
    /// Bullet同步表现层
    /// </summary>
    public class BulletVisual : MonoBehaviour
    {
        public Entity BulletEntity;
        private Vector3 _target;

        private void Update()
        {
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            if (em.Exists(BulletEntity))
            {
                _target = em.GetComponentData<Translation>(BulletEntity).Value;
            }
            else
            {
                Destroy(gameObject);
            }

            if (_target != null)
            {
                transform.position = _target;
            }
        }
    }
}