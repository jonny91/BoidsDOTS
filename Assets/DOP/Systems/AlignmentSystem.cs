/*************************************************************************************
 *
 * 文 件 名:   AlignmentSystem.cs
 * 描    述: 
 * 
 * 创 建 者：  洪金敏 
 * 创建时间：  2024-04-23 20:09:38
*************************************************************************************/

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Boid.DOP
{
    [UpdateInGroup(typeof(BoidsSystemGroup))]
    public partial struct AlignmentSystem : ISystem
    {
        [BurstCompile]
        private partial struct Job : IJobEntity
        {
            public float AlignmentWeight;

            [ReadOnly]
            public BufferLookup<NeighborBuffer> NeighborsLookup;

            public void Execute([ChunkIndexInQuery] int sortKey, in Entity entity, in LocalTransform trans,
                in Velocity velocity,
                ref Acceleration acceleration)
            {
                if (!NeighborsLookup.TryGetBuffer(entity, out var neighbors))
                {
                    return;
                }

                if (neighbors.Length == 0)
                {
                    return;
                }
                var averageVelocity = float3.zero;
                for (int i = 0; i < neighbors.Length; i++)
                {
                    averageVelocity += neighbors[i].Velocity.Value;
                }

                averageVelocity /= neighbors.Length;
                var dAccel = (averageVelocity - velocity.Value) * AlignmentWeight;
                acceleration.Value += dAccel;
            }
        }

        public BufferLookup<NeighborBuffer> NeighborsLookup;

        public void OnCreate(ref SystemState state)
        {
            NeighborsLookup = state.GetBufferLookup<NeighborBuffer>(true);
            state.RequireForUpdate<SimulationEnv>();
        }

        public void OnUpdate(ref SystemState state)
        {
            NeighborsLookup.Update(ref state);

            var job = new Job
            {
                AlignmentWeight = Bootstrap.Param.alignmentWeight,
                NeighborsLookup = NeighborsLookup
            };
            job.ScheduleParallel();
        }
    }
}