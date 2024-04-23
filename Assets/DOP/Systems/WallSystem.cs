// *************************************************************************************
//  *
//  * 文 件 名:   WallSystem.cs
//  * 描    述:
//  *
//  * 创 建 者：  洪金敏
//  * 创建时间：  2024-04-23 21:24
// *************************************************************************************

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Boid.DOP
{
    [UpdateBefore(typeof(BoidsSystemGroup))]
    public partial struct WallSystem : ISystem
    {
        public partial struct Job : IJobEntity
        {
            public float Scale;
            public float Thresh;
            public float Weight;

            public void Execute(ref Acceleration acceleration, in LocalTransform trans)
            {
                var pos = trans.Position;
                var accel = acceleration.Value;
                accel += GetAccelAgainstWall(-Scale - pos.x, math.right(), Thresh, Weight) +
                         GetAccelAgainstWall(-Scale - pos.y, math.up(), Thresh, Weight) +
                         GetAccelAgainstWall(-Scale - pos.z, math.forward(), Thresh, Weight) +
                         GetAccelAgainstWall(+Scale - pos.x, math.left(), Thresh, Weight) +
                         GetAccelAgainstWall(+Scale - pos.y, math.down(), Thresh, Weight) +
                         GetAccelAgainstWall(+Scale - pos.z, math.back(), Thresh, Weight);
                acceleration.Value = accel;
            }

            private float3 GetAccelAgainstWall(float dist, float3 dir, float thresh, float weight)
            {
                if (dist < thresh)
                {
                    return dir * (weight / math.abs(dist / thresh));
                }

                return float3.zero;
            }
        }

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationEnv>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var param = Bootstrap.Param;
            var scale = param.wallScale * 0.5f;
            var thresh = param.wallDistance;
            var weight = param.wallWeight;

            var job = new Job()
            {
                Scale = scale,
                Thresh = thresh,
                Weight = weight,
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
            // in SystemAPI.Query<RefRW<Acceleration>, RefRO<LocalTransform>>())
        }

        public void OnDestroy(ref SystemState state)
        {
        }
    }
}