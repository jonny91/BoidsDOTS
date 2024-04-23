/*************************************************************************************
 *
 * 文 件 名:   Simulation.cs
 * 描    述: 
 * 
 * 创 建 者：  洪金敏 
 * 创建时间：  2024-04-23 17:42:55
*************************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Boid.OOP
{
    public class Simulation : MonoBehaviour
    {
        [SerializeField]
        int boidCount = 100;

        [SerializeField]
        GameObject boidPrefab;

        [SerializeField]
        Param param;

        List<Boid> boids_ = new List<Boid>();

        public ReadOnlyCollection<Boid> boids => boids_.AsReadOnly();

        private void AddBoid()
        {
            var go = Instantiate(boidPrefab, Random.insideUnitSphere, Random.rotation);
            go.transform.SetParent(transform);
            var boid = go.GetComponent<Boid>();
            boid.simulation = this;
            boid.param = param;
            boids_.Add(boid);
        }

        private void RemoveBoid()
        {
            if (boids_.Count == 0) return;

            var lastIndex = boids_.Count - 1;
            var boid = boids_[lastIndex];
            Destroy(boid.gameObject);
            boids_.RemoveAt(lastIndex);
        }

        private void Update()
        {
            while (boids_.Count < boidCount)
            {
                AddBoid();
            }

            while (boids_.Count > boidCount)
            {
                RemoveBoid();
            }
        }

        private void OnDrawGizmos()
        {
            if (!param) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * param.wallScale);
        }
    }
}