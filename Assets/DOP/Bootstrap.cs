/*************************************************************************************
 *
 * 文 件 名:   Bootstrap.cs
 * 描    述: 
 * 
 * 创 建 者：  洪金敏 
 * 创建时间：  2024-04-23 22:34:42
*************************************************************************************/
using System;
using Unity.Rendering;
using UnityEngine;

namespace Boid.DOP
{
    public class Bootstrap : MonoBehaviour
    {
        public static Bootstrap Instance { get; private set; }

        public static Param Param => Instance.param;

        [SerializeField]
        int boidCount = 100;

        [SerializeField]
        private Vector3 boidScale = new Vector3(0.1f, 0.1f, 0.3f);

        [SerializeField]
        Param param;

        [SerializeField]
        RenderMesh renderer;

        private void Awake()
        {
            Instance = this;
        }
    }
}