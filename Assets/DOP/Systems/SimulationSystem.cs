// *************************************************************************************
//  *
//  * 文 件 名:   SimulationSystem.cs
//  * 描    述:
//  *
//  * 创 建 者：  洪金敏
//  * 创建时间：  2024-04-23 22:04
// *************************************************************************************

using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Boid.DOP
{
    [UpdateBefore(typeof(BoidsSystemGroup))]
    public partial struct SimulationSystem : ISystem
    {
        private Random _random;
        private int _boidCreatedCount;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationEnv>();
            var seed = (uint)DateTime.Now.Millisecond;
            _random = new Random(seed);
        }

        public void OnUpdate(ref SystemState state)
        {
            var initSpeed = Bootstrap.Param.initSpeed;
            var env = SystemAPI.GetSingleton<SimulationEnv>();

            for (int i = 0; i < env.CreatePerFrame; i++)
            {
                var boidEntity = state.EntityManager.Instantiate(env.BoidPrefab);
                state.EntityManager.SetComponentData(boidEntity, new LocalTransform()
                {
                    Position = _random.NextFloat3(),
                    Scale = 1,
                    Rotation = quaternion.identity,
                });
                state.EntityManager.SetComponentData(boidEntity, new Velocity()
                {
                    Value = _random.NextFloat3Direction() * initSpeed,
                });
                state.EntityManager.SetComponentData(boidEntity, new Acceleration
                {
                    Value = float3.zero
                });
            }

            _boidCreatedCount += env.CreatePerFrame;
            //分帧创建结束
            state.Enabled = _boidCreatedCount < env.BoidCount;
        }
    }
}