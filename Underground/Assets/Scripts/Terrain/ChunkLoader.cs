using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChunkLoader : MonoBehaviour
{
    
    public int maxSize = 0;
    private Dictionary<Vector3Int, ChunkData> loadedChuncks = new Dictionary<Vector3Int, ChunkData>();
    private Queue<Vector3Int> cycleChunks = new Queue<Vector3Int>();

    public bool isLoaded(Vector3Int coord){
        return loadedChuncks.ContainsKey(coord);
    }

    public ChunkData getData(Vector3Int coord){
        return loadedChuncks[coord];
    }

    public void InsertChunk(Vector3Int coord, ChunkData chunkData){

        if(isLoaded(coord)){
            loadedChuncks[coord] = chunkData;
        } else {
            if(maxSize<=loadedChuncks.Count){
                Vector3Int deleteCoord = cycleChunks.Dequeue();
                loadedChuncks.Remove(deleteCoord);
            }
            loadedChuncks.Add(coord, chunkData);
            cycleChunks.Enqueue(coord);
        }
    }


    

    
}
