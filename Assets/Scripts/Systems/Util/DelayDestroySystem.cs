using Unity.Entities;

namespace DOTS
{
    /// <summary>
    /// 逻辑层 延迟自我销毁
    /// </summary>
    public class DelayDestroySystem : SystemBase
    {
        EntityCommandBufferSystem m_Barrier;

        protected override void OnCreate()
        {
            m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = m_Barrier.CreateCommandBuffer().AsParallelWriter();
            var dt = Time.DeltaTime;

            Entities.ForEach((Entity entity, int nativeThreadIndex, ref DelayDestroyComponent delayDestroyComponent) =>
            {
                delayDestroyComponent.delay -= dt;
                if (delayDestroyComponent.delay < 0.0f)
                {
                    commandBuffer.DestroyEntity(nativeThreadIndex, entity);
                }
            }).ScheduleParallel();
            m_Barrier.AddJobHandleForProducer(Dependency);
        }
    }
}