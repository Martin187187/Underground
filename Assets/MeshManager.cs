using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
[ExecuteInEditMode]
public class MeshManager : MonoBehaviour
{
    public MeshGenerator meshGenerator;
    public ChunkLoader loader;
    public Transform player;
    public float itemMultiplicator = 0.95f;
    // Start is called before the first frame update

    public void DeleteTerrain(Vector3 pos, float rad, float strengh, Dictionary<Type, float> usedTypes, Func<Vector3, bool> isEmbodied = null){

        
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
                        int[] type = chunkData.type;


                        /*
                        for(int i = 0; i < data.Length; i++){
                            Vector3 vertexPos = new Vector3(data[i].x, data[i].y, data[i].z);
                            float distance = Vector3.Distance(pos, vertexPos);
                            if(distance<rad)
                            data[i] = new Vector4(data[i].x,data[i].y, data[i].z, Mathf.Max(data[i].w-(strengh),0));
                        }
                        */
                        for(int i = 0; i < data.Length; i++) {
                            Vector3 vertexPos = new Vector3(data[i].x, data[i].y, data[i].z);
                            float distance = Vector3.Distance(pos, vertexPos);
                            Type ty = (Type)type[i];
                            if(distance<rad+(float)numPointsPerAxis/meshGenerator.boundsSize){
                                
                                if(ty == Type.Grass)
                                    type[i] = (int)Type.Dirt;
                            }
                            if(distance<rad){
                                if(isEmbodied == null || isEmbodied(vertexPos)){
                                    float cappedStrengh = Mathf.Min(Mathf.Max(0,data[i].w-meshGenerator.isoLevel+0.5f), Mathf.Abs(strengh));
                                    float cappedStrengh2 = Mathf.Min(Mathf.Max(0,data[i].w-meshGenerator.isoLevel), Mathf.Abs(strengh));
                                    data[i] = new Vector4(data[i].x,data[i].y, data[i].z, data[i].w - cappedStrengh);


                                    if(usedTypes.ContainsKey(ty)){
                                        float value;
                                        usedTypes.TryGetValue(ty, out value);
                                        usedTypes[ty] = value + cappedStrengh*itemMultiplicator;
                                    } else {
                                        usedTypes.TryAdd(ty, cappedStrengh*itemMultiplicator);
                                    }



                                }
                            }
                        }
                        loader.InsertChunk(coord, new ChunkData(data, type));
                        meshGenerator.EditTerrain(coord);
                    } else {
                        ChunkData chunkData = meshGenerator.GeneratePoints(coord);
                        loader.InsertChunk(coord, chunkData);
                    }
                }
            }
        }
    }
    
    public void AddTerrain(Vector3 pos, Vector3 direction, float rad, float strengh, Dictionary<Type, float> usedTypes, Func<Vector3, bool> isEmbodied = null, bool first = true){

        
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
                        int[] type = chunkData.type;

                        if(first)
                            for(int i = 0; i < data.Length; i++){
                                Vector3 vertexPos = new Vector3(data[i].x, data[i].y, data[i].z);
                                float distance = Vector3.Distance(pos, vertexPos);
                                if(distance<rad &&  data[i].z < meshGenerator.isoLevel+10){
                                    
                                    //transfer build sphere into the terrain by rad
                                    Vector3 normedDirection = direction.normalized * rad;
                                    AddTerrain(pos + direction, Vector3.zero, rad, strengh, usedTypes, isEmbodied, false);
                                    return;
                                }
                            }

                        
                        Parallel.For(0, data.Length, i => {
                            Vector3 vertexPos = new Vector3(data[i].x, data[i].y, data[i].z);
                            float distance = Vector3.Distance(pos, vertexPos);
                            if(distance<rad){
                                if((isEmbodied == null || isEmbodied(vertexPos))&&(type[i] == (int)Type.Beton || data[i].w<meshGenerator.isoLevel)){
                                    float cappedStrengh = Mathf.Min(Mathf.Max(0,-(data[i].w-meshGenerator.isoLevel-10f)), Mathf.Abs(strengh));
                                    //if(usedTypes.ContainsKey(Type.Beton) && usedTypes[Type.Beton] >= cappedStrengh){
                                        usedTypes[Type.Beton] -= cappedStrengh;
                                        data[i] = new Vector4(data[i].x,data[i].y, data[i].z, data[i].w + cappedStrengh);
                                        type[i] = (int)Type.Beton;
                                    //}
                                }
                            }
                        });
                        loader.InsertChunk(coord, new ChunkData(data, type));
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
