using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereDensity : DensityGenerator {

    public float radius = 1;

    public override ComputeBuffer Generate (ComputeBuffer dataBuffer, ComputeBuffer pointsBuffer, int numPointsPerAxis, float boundsSize, Vector3 worldBounds, Vector3 centre, Vector3 offset, float spacing, float isoLevel) {
        densityShader.SetFloat ("radius", radius);
        return base.Generate (dataBuffer, pointsBuffer, numPointsPerAxis, boundsSize, worldBounds, centre, offset, spacing, isoLevel);
    }
}