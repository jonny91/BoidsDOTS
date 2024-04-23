// *************************************************************************************
//  *
//  * 文 件 名:   NeighborBuffer.cs
//  * 描    述:
//  *
//  * 创 建 者：  洪金敏
//  * 创建时间：  2024-04-23 19:04
// *************************************************************************************

using Unity.Entities;
using Unity.Mathematics;

namespace Boid.DOP
{
    public struct NeighborBuffer : IBufferElementData
    {
        public float3 Position;
        public quaternion Rotation;
        public Acceleration Acceleration;
        public Velocity Velocity;
    }
}