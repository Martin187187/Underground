using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
[ExecuteInEditMode]
public class MeshGenerator : MonoBehaviour {
    const int threadGroupSize = 8;

    [Header ("General Settings")]
    public ComputeShader merchData;
    public DensityGenerator[] densityGenerator;
    public ChunkLoader loader;

    public bool fixedMapSize;
    [ConditionalHide (nameof (fixedMapSize), true)]
    public Vector3Int numChunks = Vector3Int.one;
    [ConditionalHide (nameof (fixedMapSize), false)]
    public Transform viewer;
    [ConditionalHide (nameof (fixedMapSize), false)]
    public float viewDistance = 30;

    [Space ()]
    public bool autoUpdateInEditor = true;
    public bool autoUpdateInGame = true;
    public ComputeShader shader;
    public Material mat;
    public bool generateColliders;

    [Header ("Voxel Settings")]
    public float isoLevel;
    public float boundsSize = 1;
    public Vector3 offset = Vector3.zero;

    [Range (2, 100)]
    public int numPointsPerAxis = 30;

    [Header ("Gizmos")]
    public bool showBoundsGizmo = true;
    public Color boundsGizmoCol = Color.white;

    GameObject chunkHolder;
    const string chunkHolderName = "Chunks Holder";
    List<Chunk> chunks;
    Dictionary<Vector3Int, Chunk> existingChunks;
    Queue<Chunk> recycleableChunks;

    // Buffers
    ComputeBuffer triangleBuffer;
    ComputeBuffer pointsBuffer;
    ComputeBuffer triCountBuffer;
    ComputeBuffer pointsBuffer2;
    ComputeBuffer dataBuffer;

    bool settingsUpdated;

    Vector3 selectedPosition = Vector3.zero;

    ConcurrentBag<MeshValues> chunksReadyToUpdate = new ConcurrentBag<MeshValues>();

    private Color[] colorArray = {Color.red,Color.green,Color.blue,new Color(0,0, 0,1)};

    void OnEnable(){
        if (Application.isPlaying && !fixedMapSize) {
            InitVariableChunkStructures ();

            var oldChunks = FindObjectsOfType<Chunk> ();
            for (int i = oldChunks.Length - 1; i >= 0; i--) {
                Destroy (oldChunks[i].gameObject);
            }
        }
    }
    void Awake () {
        if (Application.isPlaying && !fixedMapSize) {
            InitVariableChunkStructures ();

            var oldChunks = FindObjectsOfType<Chunk> ();
            for (int i = oldChunks.Length - 1; i >= 0; i--) {
                Destroy (oldChunks[i].gameObject);
            }
        }
    }

    void Update () {
        // Update endless terrain
        if ((Application.isPlaying && !fixedMapSize)) {
            updateThreadChunks();
            Run ();
            
        }
        
        if (settingsUpdated) {
            RequestMeshUpdate ();
            settingsUpdated = false;
        }
                    
    }

    public void Run () {
        CreateBuffers ();

        if (fixedMapSize) {
            InitChunks ();
            UpdateAllChunks ();

        } else {
            if (Application.isPlaying) {
                InitVisibleChunks ();
            }
        }

        // Release buffers immediately in editor
        if (!Application.isPlaying) {
            ReleaseBuffers ();
        }

    }


    public void RequestMeshUpdate () {
        if ((Application.isPlaying && autoUpdateInGame) || (!Application.isPlaying && autoUpdateInEditor)) {
            Run ();
        }
    }

    void InitVariableChunkStructures () {
        recycleableChunks = new Queue<Chunk> ();
        chunks = new List<Chunk> ();
        existingChunks = new Dictionary<Vector3Int, Chunk> ();
        mat.SetFloat("_step", boundsSize/(numPointsPerAxis-1));
    }

    void InitVisibleChunks () {
        if (chunks==null) {
            return;
        }
        CreateChunkHolder ();

        Vector3 p = viewer.position;
        Vector3 ps = p / boundsSize;
        Vector3Int viewerCoord = new Vector3Int (Mathf.RoundToInt (ps.x), Mathf.RoundToInt (ps.y), Mathf.RoundToInt (ps.z));

        int maxChunksInView = Mathf.CeilToInt (viewDistance / boundsSize);
        float sqrViewDistance = viewDistance * viewDistance;

        // Go through all existing chunks and flag for recyling if outside of max view dst
        for (int i = chunks.Count - 1; i >= 0; i--) {
            Chunk chunk = chunks[i];
            Vector3 centre = CentreFromCoord (chunk.coord);
            Vector3 viewerOffset = p - centre;
            Vector3 o = new Vector3 (Mathf.Abs (viewerOffset.x), Mathf.Abs (viewerOffset.y), Mathf.Abs (viewerOffset.z)) - Vector3.one * boundsSize / 2;
            float sqrDst = new Vector3 (Mathf.Max (o.x, 0), Mathf.Max (o.y, 0), Mathf.Max (o.z, 0)).sqrMagnitude;
            if (sqrDst > sqrViewDistance) {
                existingChunks.Remove (chunk.coord);
                recycleableChunks.Enqueue (chunk);
                chunks.RemoveAt (i);
            }
        }

        for (int x = -maxChunksInView; x <= maxChunksInView; x++) {
            for (int y = -maxChunksInView; y <= maxChunksInView; y++) {
                for (int z = -maxChunksInView; z <= maxChunksInView; z++) {
                    Vector3Int coord = new Vector3Int (x, y, z) + viewerCoord;

                    if (existingChunks.ContainsKey (coord)) {
                        continue;
                    }

                    Vector3 centre = CentreFromCoord (coord);
                    Vector3 viewerOffset = p - centre;
                    Vector3 o = new Vector3 (Mathf.Abs (viewerOffset.x), Mathf.Abs (viewerOffset.y), Mathf.Abs (viewerOffset.z)) - Vector3.one * boundsSize / 2;
                    float sqrDst = new Vector3 (Mathf.Max (o.x, 0), Mathf.Max (o.y, 0), Mathf.Max (o.z, 0)).sqrMagnitude;

                    // Chunk is within view distance and should be created (if it doesn't already exist)
                    if (sqrDst <= sqrViewDistance) {

                        Bounds bounds = new Bounds (CentreFromCoord (coord), Vector3.one * boundsSize);
                        if (IsVisibleFrom (bounds, Camera.main)) {
                            if (recycleableChunks.Count > 0) {
                                Chunk chunk = recycleableChunks.Dequeue ();
                                chunk.coord = coord;
                                chunk.Reset();
                                chunk.SetUp(mat, generateColliders);
                                existingChunks.Add (coord, chunk);
                                chunks.Add (chunk);
                                UpdateChunkMesh (chunk);
                            } else {
                                Chunk chunk = CreateChunk (coord);
                                chunk.coord = coord;
                                chunk.SetUp (mat, generateColliders);
                                existingChunks.Add (coord, chunk);
                                chunks.Add (chunk);
                                UpdateChunkMesh (chunk);
                            }
                        }
                    }

                }
            }
        }
    }

    public bool IsVisibleFrom (Bounds bounds, Camera camera) {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes (camera);
        return GeometryUtility.TestPlanesAABB (planes, bounds);
    }

    public bool EditTerrain(Vector3Int coord){
        if (!existingChunks.ContainsKey (coord))
            return false;
        
        Chunk chunk = existingChunks[coord];
        UpdateChunkMesh(chunk);
        return true;
    }

    public Vector3Int getIndexFromPosition(Vector3 v, Vector3Int coord){
        int numVoxelsPerAxis = numPointsPerAxis-1;
        float step = boundsSize/numVoxelsPerAxis;

        Vector3 k = v + Vector3.one*(numVoxelsPerAxis*0.5f);
        int x = Mathf.RoundToInt(k.x/step - coord.x * (float)numVoxelsPerAxis);
        int y = Mathf.RoundToInt(k.y/step - coord.y * (float)numVoxelsPerAxis);
        int z = Mathf.RoundToInt(k.z/step - coord.z * (float)numVoxelsPerAxis);

        return new Vector3Int(x,y,z);
    }

    public Vector3Int getIndexFromPositionRaw(Vector3 v){
        int numVoxelsPerAxis = numPointsPerAxis-1;
        float step = boundsSize/numVoxelsPerAxis;

        int x = Mathf.RoundToInt(v.x/step );
        int y = Mathf.RoundToInt(v.y/step );
        int z = Mathf.RoundToInt(v.z/step );

        return new Vector3Int(x,y,z);
    }

    public float getStep(){
        
        int numVoxelsPerAxis = numPointsPerAxis-1;
        float step = boundsSize/numVoxelsPerAxis;
        return step;

    }
    public Vector3 getIndexFromPositionRasterd(Vector3 v){
        int numVoxelsPerAxis = numPointsPerAxis-1;
        float step = boundsSize/numVoxelsPerAxis;

        int x = Mathf.RoundToInt(v.x/step );
        int y = Mathf.RoundToInt(v.y/step );
        int z = Mathf.RoundToInt(v.z/step );

        return new Vector3(x,y,z)*step;
    }

    public Vector3 getPositionFromIndex(Vector3 v, Vector3Int coord){
        return CentreFromCoord(coord)  * boundsSize + v * boundsSize / (numPointsPerAxis - 1) - Vector3.one*boundsSize/2;
    }

    public ChunkData GeneratePoints(Vector3Int coord){
        int numVoxelsPerAxis = numPointsPerAxis - 1;
        int numThreadsPerAxis = Mathf.CeilToInt (numVoxelsPerAxis / (float) threadGroupSize);
        float pointSpacing = boundsSize / (numPointsPerAxis - 1);
        Vector3 centre = CentreFromCoord (coord);
        Vector3 worldBounds = new Vector3 (numChunks.x, numChunks.y, numChunks.z) * boundsSize;
        
        
        merchData.SetInt ("numPointsPerAxis", numPointsPerAxis);
        merchData.SetBuffer (0, "points", pointsBuffer);
        merchData.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);
        
        foreach(DensityGenerator gen in densityGenerator)
            gen.Generate (dataBuffer, pointsBuffer, numPointsPerAxis, boundsSize, worldBounds, centre, offset, pointSpacing, isoLevel);
        

        /*
        merchData.SetBuffer (0, "points", pointsBuffer);
        merchData.SetBuffer (0, "points2", pointsBuffer2);
        merchData.SetBuffer (0, "data", dataBuffer);
        
        merchData.SetInt ("numPointsPerAxis", numPointsPerAxis);
        merchData.SetFloat ("isoLevel", isoLevel);
        merchData.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);
        */
        Vector4[] data = new Vector4[numPointsPerAxis*numPointsPerAxis*numPointsPerAxis];
        pointsBuffer.GetData(data);
        
        int[] type = new int[numPointsPerAxis*numPointsPerAxis*numPointsPerAxis];
        dataBuffer.GetData(type);

        return new ChunkData(data, type);
    }

    public void updateThreadChunks(){
        while(chunksReadyToUpdate.Count>0){
            MeshValues values;
            chunksReadyToUpdate.TryTake(out values);
            
            if (!existingChunks.ContainsKey (values.coord))
                continue;
            Chunk chunk = existingChunks[values.coord];
            Mesh mesh = chunk.mesh;
            mesh.Clear ();
            mesh.vertices = values.vertices;
            mesh.normals = values.normals;
            mesh.triangles = values.meshTriangles;
            mesh.colors = values.colors;
            /*
            Vector2[] uvs = new Vector2[mesh.vertexCount];
            for(int i = 0; i < mesh.vertexCount; i++){
                
                Vector3 normal = mesh.normals[i];

                float up = Vector3.Angle(normal, Vector3.up);
                float down = Vector3.Angle(normal, Vector3.down);
                float right = Vector3.Angle(normal, Vector3.right);
                float left = Vector3.Angle(normal, Vector3.left);
                float forward = Vector3.Angle(normal, Vector3.forward);
                float back = Vector3.Angle(normal, Vector3.back);

                float min = Mathf.Min(Mathf.Min(Mathf.Min(Mathf.Min(Mathf.Min(up, down), right), left), forward), back);
                
                if(min == up){
                    uvs[i] = new Vector2(mesh.vertices[i].x / (float)boundsSize, mesh.vertices[i].z / (float)boundsSize);
                } else if(min == down){
                    uvs[i] = new Vector2(mesh.vertices[i].z / (float)boundsSize, mesh.vertices[i].x / (float)boundsSize);
                } else if(min == right){
                    uvs[i] = new Vector2(mesh.vertices[i].z / (float)boundsSize, mesh.vertices[i].y / (float)boundsSize);
                } else if(min == left){
                    uvs[i] = new Vector2(mesh.vertices[i].y / (float)boundsSize, mesh.vertices[i].z / (float)boundsSize);
                } else if(min == forward){
                    uvs[i] = new Vector2(mesh.vertices[i].y / (float)boundsSize, mesh.vertices[i].x / (float)boundsSize);
                } else {
                    uvs[i] = new Vector2(mesh.vertices[i].x / (float)boundsSize, mesh.vertices[i].y / (float)boundsSize);
                } 
                
            }
            mesh.uv = uvs;
            */
            //mesh.RecalculateNormals();
            mesh.uv = values.uvs1;
            mesh.uv2 = values.uvs2;
            mesh.uv3 = values.uvs3;
            mesh.uv4 = values.information;
            chunk.SetUp(mat, generateColliders);

        }
    }
    
    public void CalculateMeshThread(Triangle[] tris, int numTris, Chunk chunk){
        
        MeshValues values = CalculateMesh(tris, numTris, chunk);
        chunksReadyToUpdate.Add(values);
    }

    public void UpdateChunkMesh (Chunk chunk) {

        chunk.isDirty = true;

        int numVoxelsPerAxis = numPointsPerAxis - 1;
        int numThreadsPerAxis = Mathf.CeilToInt (numVoxelsPerAxis / (float) threadGroupSize);

        Vector3Int coord = chunk.coord;


        if(loader.isLoaded(coord) && Application.isPlaying){
            ChunkData chunkData = loader.getData(coord);
            chunk.chunkData = chunkData;

            Vector4[] data = chunkData.data;
            pointsBuffer.SetData(data);
            int[] type = chunkData.type;
            dataBuffer.SetData(type);
        } else {
            chunk.chunkData = GeneratePoints(coord);
            loader.InsertChunk(coord, chunk.chunkData);
        }
        
        triangleBuffer.SetCounterValue (0);
        shader.SetBuffer (0, "points", pointsBuffer);
        shader.SetBuffer (0, "triangles", triangleBuffer);
        shader.SetBuffer (0, "data", dataBuffer);
        shader.SetInt ("numPointsPerAxis", numPointsPerAxis);
        shader.SetFloat ("isoLevel", isoLevel);

        shader.Dispatch (0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

        // Get number of triangles in the triangle buffer
        ComputeBuffer.CopyCount (triangleBuffer, triCountBuffer, 0);
        int[] triCountArray = { 0 };
        triCountBuffer.GetData (triCountArray);
        int numTris = triCountArray[0];

        // Get triangle data from shader
        Triangle[] tris = new Triangle[numTris];
        triangleBuffer.GetData (tris, 0, 0, numTris);

        
        
        
        if(false && Application.isPlaying&&!fixedMapSize){
            Thread thread = new Thread(() => CalculateMeshThread( tris,  numTris, chunk));
            thread.Start();
            return;
        } else {
            CalculateMeshFull(tris, numTris, chunk);
            
            chunk.SetUp(mat, generateColliders);
        }
    }

    public MeshValues CalculateMesh(Triangle[] tris, int numTris, Chunk chunk){
        Mesh mesh = chunk.mesh;
        var meshTriangles = new List<int>();
        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var colors = new List<Color>();
        var uvs1 = new List<Vector2>();
        var uvs2 = new List<Vector2>();
        var uvs3 = new List<Vector2>();
        var information = new List<Vector2>();
        for (int i = 0; i < numTris; i++) {
            for (int j = 0; j < 3; j++) {
                Vector3 currentVector = tris[i][j].position;
                if(vertices.Contains(currentVector)){
                    int index = vertices.IndexOf(currentVector);
                    meshTriangles.Add(index);
                } else {
                    meshTriangles.Add(vertices.Count);
                    vertices.Add(currentVector);
                    normals.Add(tris[i][j].normal);
                    colors.Add(colorArray[tris[i][j].data]);
                    uvs1.Add(new Vector2(tris[i][j].position.x / (float)boundsSize, tris[i][j].position.z / (float)boundsSize));
                    uvs2.Add(new Vector2(tris[i][j].position.z / (float)boundsSize, tris[i][j].position.y / (float)boundsSize));
                    uvs3.Add(new Vector2(tris[i][j].position.y / (float)boundsSize, tris[i][j].position.x / (float)boundsSize));
                    information.Add(new Vector2(tris[i][j].data,0));
                }
                
            }
        }
        return new MeshValues(chunk.coord, vertices.ToArray(), normals.ToArray(), meshTriangles.ToArray(), colors.ToArray(), uvs1.ToArray(), uvs2.ToArray(), uvs3.ToArray(), information.ToArray());
    }

    public void CalculateMeshFull(Triangle[] tris, int numTris, Chunk chunk){
        Mesh mesh = chunk.mesh;
        mesh.Clear();

        mesh.subMeshCount = 4;
        var vertices = new Vector3[numTris * 3];
        var normals = new Vector3[numTris * 3];
        var meshTriangles = new int[numTris * 3];
        var colors = new Color[numTris*3];
        var uvs1 = new Vector2[numTris*3];
        var uvs2 = new Vector2[numTris*3];
        var uvs3 = new Vector2[numTris*3];
        var information = new Vector2[numTris*3];

        for (int i = 0; i < numTris; i++) {
            for (int j = 0; j < 3; j++) {
                meshTriangles[i * 3 + j] = i * 3 + j;
                vertices[i * 3 + j] = tris[i][j].position;
                normals[i * 3 + j] = tris[i][j].normal;
                colors[i*3+j] = colorArray[tris[i][j].data];
                uvs1[i*3+j] = tris[i][j].data == 0? new Vector2(1,0) : new Vector2(0,1);
                uvs2[i*3+j] = new Vector2(tris[i][j].position.y / (float)boundsSize, tris[i][j].position.z / (float)boundsSize);
                uvs3[i*3+j] = new Vector2(tris[i][j].position.y / (float)boundsSize, tris[i][j].position.x / (float)boundsSize);
                information[i*3+j] = new Vector2(tris[i][j].data, 0);
            }
        }

        
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = meshTriangles;
        mesh.colors = colors;
        
        mesh.uv = uvs1;
        mesh.uv2 = uvs2;
        mesh.uv3 = uvs3;
        mesh.uv4 = information;

    }



    public void UpdateAllChunks () {

        // Create mesh for each chunk
        foreach (Chunk chunk in chunks) {
            UpdateChunkMesh (chunk);
        }

    }

    void OnDestroy () {
        if (Application.isPlaying) {
            ReleaseBuffers ();
        }
    }

    void CreateBuffers () {
        int numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
        int numVoxelsPerAxis = numPointsPerAxis - 1;
        int numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
        int maxTriangleCount = numVoxels * 5;

        // Always create buffers in editor (since buffers are released immediately to prevent memory leak)
        // Otherwise, only create if null or if size has changed
        if (!Application.isPlaying || (pointsBuffer == null || numPoints != pointsBuffer.count)) {
            if (Application.isPlaying) {
                ReleaseBuffers ();
            }
            triangleBuffer = new ComputeBuffer (maxTriangleCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Triangle)), ComputeBufferType.Append);
            pointsBuffer = new ComputeBuffer (numPoints, sizeof (float) * 4);
            triCountBuffer = new ComputeBuffer (1, sizeof (int), ComputeBufferType.Raw);
            pointsBuffer2 = new ComputeBuffer (numPoints, sizeof (float) * 4);
            dataBuffer = new ComputeBuffer (numPoints, sizeof (int));

        }
    }

    void ReleaseBuffers () {
        if (triangleBuffer != null) {
            triangleBuffer.Release ();
            pointsBuffer.Release ();
            triCountBuffer.Release ();
            pointsBuffer2.Release ();
            dataBuffer.Release();
        }
    }

    Vector3 CentreFromCoord (Vector3Int coord) {
        // Centre entire map at origin
        if (fixedMapSize) {
            Vector3 totalBounds = (Vector3) numChunks * boundsSize;
            return -totalBounds / 2 + (Vector3) coord * boundsSize + Vector3.one * boundsSize / 2;
        }

        return new Vector3 (coord.x, coord.y, coord.z) * boundsSize;
    }

    void CreateChunkHolder () {
        // Create/find mesh holder object for organizing chunks under in the hierarchy
        if (chunkHolder == null) {
            if (GameObject.Find (chunkHolderName)) {
                chunkHolder = GameObject.Find (chunkHolderName);
            } else {
                chunkHolder = new GameObject (chunkHolderName);
            }
        }
    }

    // Create/get references to all chunks
    void InitChunks () {
        CreateChunkHolder ();
        chunks = new List<Chunk> ();
        List<Chunk> oldChunks = new List<Chunk> (FindObjectsOfType<Chunk> ());

        // Go through all coords and create a chunk there if one doesn't already exist
        for (int x = 0; x < numChunks.x; x++) {
            for (int y = 0; y < numChunks.y; y++) {
                for (int z = 0; z < numChunks.z; z++) {
                    Vector3Int coord = new Vector3Int (x, y, z);
                    bool chunkAlreadyExists = false;

                    // If chunk already exists, add it to the chunks list, and remove from the old list.
                    for (int i = 0; i < oldChunks.Count; i++) {
                        if (oldChunks[i].coord == coord) {
                            chunks.Add (oldChunks[i]);
                            oldChunks.RemoveAt (i);
                            chunkAlreadyExists = true;
                            break;
                        }
                    }

                    // Create new chunk
                    if (!chunkAlreadyExists) {
                        var newChunk = CreateChunk (coord);
                        chunks.Add (newChunk);
                    }

                    chunks[chunks.Count - 1].SetUp (mat, generateColliders);
                }
            }
        }

        // Delete all unused chunks
        for (int i = 0; i < oldChunks.Count; i++) {
            oldChunks[i].DestroyOrDisable ();
        }
    }

    Chunk CreateChunk (Vector3Int coord) {
        GameObject chunk = new GameObject ($"Chunk ({coord.x}, {coord.y}, {coord.z})");
        chunk.transform.parent = chunkHolder.transform;
        Chunk newChunk = chunk.AddComponent<Chunk> ();
        newChunk.coord = coord;
        return newChunk;
    }

    
    void OnValidate() {
        settingsUpdated = true;
    }

    public struct Triangle {
#pragma warning disable 649 // disable unassigned variable warning
        public Vertex a;
        public Vertex b;
        public Vertex c;

        public Vertex this [int i] {
            get {
                switch (i) {
                    case 0:
                        return a;
                    case 1:
                        return b;
                    default:
                        return c;
                }
            }
        }
    }
public struct Vertex {
    public Vector3 position;
    public Vector3 normal;
    public Vector2Int id;
    public int data;
}
public class MeshValues {
    public Vector3Int coord;
    public Vector3[] vertices;
    public Vector3[] normals;
    public int[] meshTriangles;
    public Color[] colors;
    public Vector2[] uvs1;
    public Vector2[] uvs2;
    public Vector2[] uvs3;
    public Vector2[] information;

    public MeshValues(Vector3Int coord, Vector3[] vertices, Vector3[] normals, int[] meshTriangles, Color[] colors, Vector2[] uvs1, Vector2[] uvs2, Vector2[] uvs3, Vector2[] information){
        this.coord = coord;
        this.vertices = vertices;
        this.normals = normals;
        this.meshTriangles = meshTriangles;
        this.colors = colors;
        this.uvs1 = uvs1;
        this.uvs2 = uvs2;
        this.uvs3 = uvs3;
        this.information = information;
    }
}
    void OnDrawGizmos () {
        if (showBoundsGizmo) {
            Gizmos.color = boundsGizmoCol;

            List<Chunk> chunks = (this.chunks == null) ? new List<Chunk> (FindObjectsOfType<Chunk> ()) : this.chunks;
            foreach (var chunk in chunks) {
                Bounds bounds = new Bounds (CentreFromCoord (chunk.coord), Vector3.one * boundsSize);
                Gizmos.color = boundsGizmoCol;
                Gizmos.DrawWireCube (CentreFromCoord (chunk.coord), Vector3.one * boundsSize);
            }
        }
    }

    

}