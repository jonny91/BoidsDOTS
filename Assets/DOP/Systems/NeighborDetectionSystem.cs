// *************************************************************************************
//  *
//  * 文 件 名:   NeighborDetectionSystem.cs
//  * 描    述:
//  *
//  * 创 建 者：  洪金敏
//  * 创建时间：  2024-04-23 22:04
// *************************************************************************************

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Boid.DOP
{
    [UpdateInGroup(typeof(BoidsSystemGroup))]
    public partial class NeighborDetectionSystem : SystemBase
    {
        [BurstCompile]
        partial struct SearchJob : IJobEntity
        {
            public float ProdThresh;
            public float DistThresh;

            [ReadOnly]
            public NativeArray<Entity> AllEntities;

            [ReadOnly]
            public NativeArray<LocalTransform> AllTrans;

            [ReadOnly]
            public NativeArray<Acceleration> AllAccel;

            [ReadOnly]
            public NativeArray<Velocity> AllVelocities;

            public EntityCommandBuffer.ParallelWriter Ecb;

            [BurstCompile]
            void Execute([ChunkIndexInQuery] int sortKey, in LocalTransform trans, in Velocity velocity,
                in Acceleration acceleration, in Entity entity)
            {
                //清空现有 neighbors
                Ecb.RemoveComponent<NeighborBuffer>(sortKey, entity);
                var neighborBuffers = Ecb.AddBuffer<NeighborBuffer>(sortKey, entity);
                for (int i = 0; i < AllEntities.Length; i++)
                {
                    var otherEntity = AllEntities[i];
                    var otherTrans = AllTrans[i];
                    var otherAccel = AllAccel[i];
                    var otherVelocity = AllVelocities[i];

                    if (entity != otherEntity)
                    {
                        float3 pos0 = trans.Position;
                        float3 fwd0 = math.normalizesafe(velocity.Value);
                        float3 pos1 = otherTrans.Position;
                        var to = pos1 - pos0;
                        var dist = math.length(to);

                        if (dist < DistThresh)
                        {
                            var dir = math.normalizesafe(to);
                            var prod = Vector3.Dot(dir, fwd0);
                            if (prod > ProdThresh)
                            {
                                neighborBuffers.Add(new NeighborBuffer
                                {
                                    Position = otherTrans.Position,
                                    Rotation = otherTrans.Rotation,
                                    Acceleration = otherAccel,
                                    Velocity = otherVelocity,
                                });
                            }
                        }
                    }
                }
            }
        }

        private EntityQuery _boidCellQuery;


        protected override void OnCreate()
        {
            RequireForUpdate<SimulationEnv>();
            RequireForUpdate<BoidCell>();
            base.OnCreate();
            _boidCellQuery = GetEntityQuery(
                ComponentType.ReadOnly<BoidCell>(),
                ComponentType.ReadOnly<Velocity>(),
                ComponentType.ReadOnly<Acceleration>(),
                ComponentType.ReadOnly<LocalTransform>());
        }

        protected override void OnUpdate()
        {
            var param = Bootstrap.Param;
            float prodThresh = math.cos(math.radians(param.neighborFov));
            float distThresh = param.neighborDistance;

            // SystemAPI.Query<RefRO<LocalTransform>, RefRO<BoidCell>>().WithEntityAccess()
            // foreach (var (entity, trans) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<BoidCell>>())
            // {
            // }

            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var allEntities = _boidCellQuery.ToEntityArray(Allocator.TempJob);
            var allAccelerations = _boidCellQuery.ToComponentDataArray<Acceleration>(Allocator.TempJob);
            var allTrans = _boidCellQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
            var allVelocities = _boidCellQuery.ToComponentDataArray<Velocity>(Allocator.TempJob);
            var searchJob = new SearchJob
            {
                Ecb = ecb.AsParallelWriter(),
                ProdThresh = prodThresh,
                DistThresh = distThresh,
                AllEntities = allEntities,
                AllAccel = allAccelerations,
                AllTrans = allTrans,
                AllVelocities = allVelocities,
            };
            Dependency = searchJob.ScheduleParallel(_boidCellQuery, Dependency);
            Dependency.Complete();
            ecb.Playback(this.EntityManager);
            allEntities.Dispose(Dependency);
            allTrans.Dispose(Dependency);
            allVelocities.Dispose(Dependency);
            ecb.Dispose();
        }
    }
}