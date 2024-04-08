using UnityEngine;

public static class VoxelData
{

    public static readonly int ChunkWidth = 16; // ûũ ���� ���� 
    public static readonly int ChunkHeight = 128; // ûũ ���� 
    public static readonly int WorldSizeInChunks = 100; // ���� ũ�� 

    public static int WorldSizeInVoxels // ���� ��ü ũ�� 
    {
        get { return WorldSizeInChunks * ChunkWidth; }
    }

    public static readonly int ViewDistanceInChunks = 5; // �þ� �Ÿ� 

    public static readonly int TextureAtlasSizeInBlocks = 4; // �ؽ�ó ��Ʈ�� ũ�� 
    
    //��Ʋ�� �� ����ȭ
    public static float NormalizeBlockTextureSize 
    {
        get { return 1f / (float)TextureAtlasSizeInBlocks; }
    }

    public static readonly Vector3[] vexelVerts = new Vector3[8] // �� ���� ��ġ 
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
    public static readonly Vector3[] faceChunk = new Vector3[6] // �� ����ȭ ���� 
    {
       new Vector3(0.0f,0.0f,-1.0f),
       new Vector3(0.0f,0.0f,1.0f),
       new Vector3(0.0f,1.0f,0.0f),
       new Vector3(0.0f,-1.0f,0.0f),
       new Vector3(-1.0f,0.0f,0.0f),
       new Vector3(1.0f,0.0f,0.0f),
    };

    public static readonly int[,] voxelTris = new int[6, 4] // �� �� ���� �ε��� 
    {
        {0,3,1,2},//�޸�
        {5,6,4,7},//�ո�
        {3,7,2,6},//����
        {1,5,0,4},//�Ʒ���
        {4,7,0,3},//���ʸ�
        {1,2,5,6} //�����ʸ�
    };

    public static readonly Vector2[] VoxelUvs = new Vector2[4]  // ����� ���� �����ϴ� ���� �ε���
    {
        new Vector2 (0.0f,0.0f),
        new Vector2 (0.0f,1.0f),
        new Vector2 (1.0f,0.0f),
        new Vector2 (1.0f,1.0f),
    };
}
