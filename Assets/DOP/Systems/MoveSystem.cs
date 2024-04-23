/*************************************************************************************
 *
 * 文 件 名:   MoveSystem.cs
 * 描    述: 
 * 
 * 创 建 者：  洪金敏 
 * 创建时间：  2024-04-23 19:08:52
*************************************************************************************/
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Boid.DOP
{
    [UpdateAfter(typeof(BoidsSystemGroup))]
    public partial struct MoveSystem : ISystem
    {
        private partial struct Job : IJobEntity
        {
            public float DeltaTime;
            public float MinSpeed;
            public float MaxSpeed;

            void Execute(ref Velocity velocity, ref LocalTransform trans, ref Acceleration acceleration)
            {
                var pos = trans.Position;
                var rot = trans.Rotation;
                velocity.Value += acceleration.Value * DeltaTime;
                var dir = math.normalizesafe(velocity.Value);
                var speed = math.length(velocity.Value);
                velocity.Value = math.clamp(speed, MinSpeed, MaxSpeed) * dir;
                pos += velocity.Value * DeltaTime;
                rot = quaternion.LookRotationSafe(dir, math.up());
                acceleration.Value = float3.zero;
                trans.Position = pos;
                trans.Rotation = rot;
            }
        }

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationEnv>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var minSpeed = Bootstrap.Param.minSpeed;
            var maxSpeed = Bootstrap.Param.maxSpeed;

            var job = new Job()
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                MinSpeed = minSpeed,
                MaxSpeed = maxSpeed,
            };
            // job.Run();
            job.ScheduleParallel();
        }
    }
}