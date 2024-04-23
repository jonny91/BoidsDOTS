// *************************************************************************************
//  *
//  * 文 件 名:   SeparationSystem.cs
//  * 描    述:
//  *
//  * 创 建 者：  洪金敏
//  * 创建时间：  2024-04-23 21:04
// *************************************************************************************

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Boid.DOP
{
    [UpdateInGroup(typeof(BoidsSystemGroup))]
    public partial struct SeparationSystem : ISystem
    {
        private partial struct Job : IJobEntity
        {
            public float SeparationWeight;

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

                var force = float3.zero;
                for (int i = 0; i < neighbors.Length; i++)
                {
                    force += math.normalizesafe(trans.Position - neighbors[i].Position);
                }

                force /= neighbors.Length;
                var dAccel = force * SeparationWeight;
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
                SeparationWeight = Bootstrap.Param.separationWeight,
                NeighborsLookup = NeighborsLookup
            };
            job.ScheduleParallel();
        }
    }
}