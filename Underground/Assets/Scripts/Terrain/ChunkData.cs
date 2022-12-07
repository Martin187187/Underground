using UnityEngine;
public class ChunkData
    {
        public Vector4[] data;
        public int[] type;

        public ChunkData(Vector4[] data, int[] type){
            this.data = data;
            this.type = type;
        }
        public ChunkData(){}
    }