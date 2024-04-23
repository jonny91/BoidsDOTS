// *************************************************************************************
//  *
//  * 文 件 名:   Simulation.cs
//  * 描    述:
//  *
//  * 创 建 者：  洪金敏
//  * 创建时间：  2024-04-23 18:04
// *************************************************************************************

using Unity.Entities;

namespace Boid.DOP
{
    public struct SimulationEnv : IComponentData
    {
        public int BoidCount;
        public int CreatePerFrame;
        public Entity BoidPrefab;
    }
}