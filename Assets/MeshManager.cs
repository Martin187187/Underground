using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
[ExecuteInEditMode]
public class MeshManager : MonoBehaviour
{
    public MeshGenerator meshGenerator;
    public ChunkLoader loader;
    public Transform player;
    // Start is called before the first frame update

    

    public void EditTerrain(Vector3 pos, float rad, float strengh){

        Vector3 start = new Vector3(pos.x-rad, pos.y-rad, pos.z-rad);
        Vector3 end = new Vector3(pos.x+rad, pos.y+rad, pos.z+rad);

        Vector3Int startChunk = getChunkCoords(start);
        Vector3Int endChunk = getChunkCoords(end);
        
        int numPointsPerAxis = meshGenerator.numPointsPerAxis;

        for (int x = startChunk.x; x <= endChunk.x; x++) {
            for (int y = startChunk.y; y <= endChunk.y; y++) {
                for (int z = startChunk.z; z <= endChunk.z; z++) {


                    Vector3Int coord = new Vector3Int (x, y, z);
                    if(loader.isLoaded(coord)){
                        ChunkData chunkData = loader.getData(coord);
                        Vector4[] data = chunkData.data;


                        /*
                        for(int i = 0; i < data.Length; i++){
                            Vector3 vertexPos = new Vector3(data[i].x, data[i].y, data[i].z);
                            float distance = Vector3.Distance(pos, vertexPos);
                            if(distance<rad)
                            data[i] = new Vector4(data[i].x,data[i].y, data[i].z, Mathf.Max(data[i].w-(strengh),0));
                        }
                        */
                        Parallel.For(0, data.Length, i => {
                            Vector3 vertexPos = new Vector3(data[i].x, data[i].y, data[i].z);
                            float distance = Vector3.Distance(pos, vertexPos);
                            if(distance<rad){
                                data[i] = new Vector4(data[i].x,data[i].y, data[i].z, Mathf.Max(data[i].w-strengh,0));
                            }
                        });
                        loader.InsertChunk(coord, new ChunkData(data, chunkData.type));
                        meshGenerator.EditTerrain(coord);
                    } else {
                        ChunkData chunkData = meshGenerator.GeneratePoints(coord);
                        loader.InsertChunk(coord, chunkData);
                    }
                }
            }
        }
    }
        public Vector3Int getChunkCoords(Vector3 position){
        Vector3 ps = position / meshGenerator.boundsSize;
        Vector3Int coord = new Vector3Int (Mathf.RoundToInt (ps.x), Mathf.RoundToInt (ps.y), Mathf.RoundToInt (ps.z));
        return coord;
    }


}
