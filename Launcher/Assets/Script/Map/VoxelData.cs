using UnityEngine;

public static class VoxelData
{

    public static readonly int ChunkWidth = 16; // 청크 가로 길이 
    public static readonly int ChunkHeight = 128; // 청크 높이 
    public static readonly int WorldSizeInChunks = 100; // 월드 크기 

    public static int WorldSizeInVoxels // 월드 전체 크기 
    {
        get { return WorldSizeInChunks * ChunkWidth; }
    }

    public static readonly int ViewDistanceInChunks = 5; // 시야 거리 

    public static readonly int TextureAtlasSizeInBlocks = 4; // 텍스처 아트라스 크기 
    
    //아틀라스 값 정규화
    public static float NormalizeBlockTextureSize 
    {
        get { return 1f / (float)TextureAtlasSizeInBlocks; }
    }

    public static readonly Vector3[] vexelVerts = new Vector3[8] // 블럭 정점 위치 
    {
        new Vector3(0.0f,0.0f,0.0f),
        new Vector3(1.0f,0.0f,0.0f),
        new Vector3(1.0f,1.0f,0.0f),
        new Vector3(0.0f,1.0f,0.0f),
        new Vector3(0.0f,0.0f,1.0f),
        new Vector3(1.0f,0.0f,1.0f),
        new Vector3(1.0f,1.0f,1.0f),
        new Vector3(0.0f,1.0f,1.0f),

    };
    public static readonly Vector3[] faceChunk = new Vector3[6] // 블럭 정규화 법선 
    {
       new Vector3(0.0f,0.0f,-1.0f),
       new Vector3(0.0f,0.0f,1.0f),
       new Vector3(0.0f,1.0f,0.0f),
       new Vector3(0.0f,-1.0f,0.0f),
       new Vector3(-1.0f,0.0f,0.0f),
       new Vector3(1.0f,0.0f,0.0f),
    };

    public static readonly int[,] voxelTris = new int[6, 4] // 블럭 면 정점 인덱스 
    {
        {0,3,1,2},//뒷면
        {5,6,4,7},//앞면
        {3,7,2,6},//윗면
        {1,5,0,4},//아래면
        {4,7,0,3},//왼쪽면
        {1,2,5,6} //오른쪽면
    };

    public static readonly Vector2[] VoxelUvs = new Vector2[4]  // 블록의 면을 구성하는 정점 인덱스
    {
        new Vector2 (0.0f,0.0f),
        new Vector2 (0.0f,1.0f),
        new Vector2 (1.0f,0.0f),
        new Vector2 (1.0f,1.0f),
    };
}
