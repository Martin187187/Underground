using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DensityGenerator : MonoBehaviour {

    const int threadGroupSize = 8;
    public ComputeShader densityShader;

    public Type type;

    protected List<ComputeBuffer> buffersToRelease;

    void OnValidate() {
        if (FindObjectOfType<MeshGenerator>()) {
            FindObjectOfType<MeshGenerator>().RequestMeshUpdate();
        }
    }

    public virtual ComputeBuffer Generate (ComputeBuffer dataBuffer, ComputeBuffer pointsBuffer, int numPointsPerAxis, float boundsSize, Vector3 worldBounds, Vector3 centre, Vector3 offset, float spacing, float isoLevel) {
        int numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
        int numThreadsPerAxis = Mathf.CeilToInt (numPointsPerAxis / (float) threadGroupSize);
        // Points buffer is populated inside shader with pos (xyz) + density (w).
        // Set paramaters
        densityShader.SetBuffer (0, "points", pointsBuffer);
        densityShader.SetBuffer (0, "data2", dataBuffer);
        densityShader.SetInt("val", (int)type);
        densityShader.SetInt ("numPointsPerAxis", numPointsPerAxis);
        densityShader.SetFloat ("boundsSize", boundsSize);
        densityShader.SetVector ("centre", new Vector4 (centre.x, centre.y, centre.z));
        densityShader.SetVector ("offset", new Vector4 (offset.x, offset.y, offset.z));
        densityShader.SetFloat ("spacing", spacing);
        densityShader.SetVector("worldSize", worldBounds);
        densityShader.SetFloat("isoLevel", isoLevel);

        // Dispatch shader
        densityShader.Dispatch (0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

        if (buffersToRelease != null) {
            foreach (var b in buffersToRelease) {
                b.Release();
            }
        }

        // Return voxel data buffer so it can be used to generate mesh
        return pointsBuffer;
    }
}