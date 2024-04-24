/*************************************************************************************
 *
 * 文 件 名:   CohesionSystem.cs
 * 描    述: 
 * 
 * 创 建 者：  洪金敏 
 * 创建时间：  2024-04-23 21:09:53
*************************************************************************************/

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Boid.DOP
{
    [UpdateInGroup(typeof(BoidsSystemGroup))]
    public partial struct CohesionSystem : ISystem
    {
        [BurstCompile]
        private partial struct Job : IJobEntity
        {
            public float CohesionWeight;

            [ReadOnly]
            public BufferLookup<NeighborBuffer> NeighborsLookup;

            public void Execute([ChunkIndexInQuery] int sortKey, in Entity entity, in LocalTransform trans,
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

                var averagePos = float3.zero;
                for (int i = 0; i < neighbors.Length; i++)
                {
                    averagePos += neighbors[i].Position;
                }

                averagePos /= neighbors.Length;
                var dAccel = (averagePos - trans.Position) * CohesionWeight;
                acceleration.Value += dAccel;
            }
        }

        public BufferLookup<NeighborBuffer> NeighborsLookup;

        public void OnCreate(ref SystemState state)
        {
            NeighborsLookup = state.GetBufferLookup<NeighborBuffer>(true);
        }

        public void OnUpdate(ref SystemState state)
        {
            NeighborsLookup.Update(ref state);

            var job = new Job
            {
                CohesionWeight = Bootstrap.Param.cohesionWeight,
                NeighborsLookup = NeighborsLookup
            };
            job.ScheduleParallel();
        }
    }
}