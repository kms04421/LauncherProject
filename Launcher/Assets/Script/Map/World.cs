using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;



public class World : MonoBehaviour
{
    public int seed; // ���� �õ� 
    public BiomeAttributes biome; // ���̿� �Ӽ� 

    public Transform player; // �÷��� ��ġ 
    public Vector3 spawnPosition; // ������ġ 


    public Material material; // ���׸��� 
    public BlockType[] blockTypes; // ��� ���� �迭 

    Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks]; // ûũ �迭 

    List<ChunkCoord> activeCunks = new List<ChunkCoord>(); // Ȱ�� ûũ ��� 
    ChunkCoord PlayerChunkCoord; //  �÷��̾� ûũ ��ǥ 
    ChunkCoord playerLastChunkCoord; // ������ �÷��̾� ûũ ��ǥ 

    List<ChunkCoord> chunksToCreate = new List<ChunkCoord>(); // ������ ûũ ��� 

    private bool isCreatingChunk; //ûũ ���� ���� 


    private void Start()
    {
        Random.InitState(seed); // �õ� �ʱ�ȭ 

        spawnPosition = new Vector3((VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight + 2f, VoxelData.WorldSizeInChunks); //���� ��ġ ���� 
        GenerateWorld();//���� ����
        playerLastChunkCoord = GetChunkCorrdFromVector3(player.position); // ������ �÷��̾� ûũ ��ġ

        
    }

    private void Update()
    {
        WorldUpdate();

    }
    
    void WorldUpdate() 
    {
        PlayerChunkCoord = GetChunkCorrdFromVector3(player.position); // ���� �÷��̾� ûũ ��ǥ ������Ʈ 
        if (!PlayerChunkCoord.Equals(playerLastChunkCoord)) // �÷��̾� ��ǥ ����� ûũ ��ǥ ���� 
        {
            CheckViewDistance();
        }

        if (chunksToCreate.Count > 0 && !isCreatingChunk) // ������ ûũ ���� �� ���� 
        {
            StartCoroutine("CreateChunks"); // ���� ���� ���ϴϱ� ���� x

        }

    }

    void GenerateWorld() // ���� ���� 
    {
        for (int x = (VoxelData.WorldSizeInChunks / 2) - VoxelData.ViewDistanceInChunks; x < (VoxelData.WorldSizeInChunks / 2) + VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = (VoxelData.WorldSizeInChunks / 2) - VoxelData.ViewDistanceInChunks; z < (VoxelData.WorldSizeInChunks / 2) + VoxelData.ViewDistanceInChunks; z++)
            {
                chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, true);
                activeCunks.Add(new ChunkCoord(x, z));

            }
        }

        player.position = spawnPosition; // �÷��̾� ��ġ ���� 
        CheckViewDistance();
    }

    IEnumerator CreateChunks() // ûũ ���� �ڷ�ƾ 
    {
        isCreatingChunk = true;

        while (chunksToCreate.Count > 0)
        {
            chunks[chunksToCreate[0].x, chunksToCreate[0].z].Init(); // ûũ �ʱ�ȭ �� ���� 

            chunksToCreate.RemoveAt(0);

            yield return null;
        }

        isCreatingChunk = false;
    }
    ChunkCoord GetChunkCorrdFromVector3(Vector3 pos) // ûũ ��ġ ��ȯ 
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return new ChunkCoord(x, z);
    }

    public Chunk GetChunkFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return chunks[x, z];
    }

    void CheckViewDistance() // ûũ Ȱ��ȭ 
    {

        ChunkCoord coord = GetChunkCorrdFromVector3(player.position);
        playerLastChunkCoord = PlayerChunkCoord;

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeCunks); // ���� Ȱ�� ûũ ��� ����

        // �ֺ� ûũ Ȱ��ȭ 
        for (int x = coord.x - VoxelData.ViewDistanceInChunks; x < coord.x + VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = coord.z - VoxelData.ViewDistanceInChunks; z < coord.z + VoxelData.ViewDistanceInChunks; z++)
            {
                if (IsChunkWorld(new ChunkCoord(x, z)))
                {
                    if (chunks[x, z] == null)
                    {
                        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, false); // ûũ ���� 

                       

                        chunksToCreate.Add(new ChunkCoord(x, z)); // ������ ûũ ��Ͽ� �߰� 
                    }
                    else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;

                    }
                    activeCunks.Add(new ChunkCoord(x, z)); // Ȱ��ȭ�� ûũ �߰� 
                }

                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                {
                    if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z))) // ���� ûũ �ߺ� ���� ���� 
                    {
                        previouslyActiveChunks.RemoveAt(i);
                    }
                }
            }
        }

        foreach (ChunkCoord c in previouslyActiveChunks) // ���� ûũ ��Ȱ��ȭ 
        {
            chunks[c.x, c.z].isActive = false;

        }
    }

    public bool CheckForVoxel(Vector3 pos)
    {
    
        ChunkCoord thisChunk = new ChunkCoord(pos);
      
        if (!IsChunkWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.ChunkHeight)
        {         
            return false;

        }
        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
        {

            return blockTypes[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isSolid;
        }
        return blockTypes[GetVoxel(pos)].isSolid;
    }

    public byte GetVoxel(Vector3 pos)//�� ���� ����
    {
        int yPos = Mathf.FloorToInt(pos.y);

        if (!InVoxelInWorld(pos))
        {
            return 0;
        }


        if (yPos == 0) // ���ϴ� �� 
        {
            return 1;

        }

        int terrainHeight = Mathf.FloorToInt(biome.terrainHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.terrainScale)) + biome.solidGroundHeight;
        byte voxelValue = 0;

        if (yPos == terrainHeight)
        {
            voxelValue = 3;

        }
        else if (yPos < terrainHeight && yPos > terrainHeight - 4)
        {
            voxelValue = 5;
        }
        else if (yPos > terrainHeight)
        {
            return 0;
        }
        else
        {
            voxelValue = 2;
        }

        if (voxelValue == 2)
        {
            foreach (Lode lode in biome.lodes)
            {
                if (yPos > lode.minHeight && yPos < lode.maxHeight)
                {
                    if (Noise.Get3DPerlin(pos, lode.noiseOffset, lode.scale, lode.threshold))
                    {
                        voxelValue = lode.blockID;
                    }
                }
            }

        }

        return voxelValue;


    }

    bool IsChunkWorld(ChunkCoord coord)
    {
        
        if (coord.x > 0 && coord.x < VoxelData.WorldSizeInChunks - 1 && coord.z > 0 &&
            coord.z < VoxelData.WorldSizeInChunks - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool InVoxelInWorld(Vector3 pos) // ������ �ʰ� ���� üũ
    {
        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight &&
           pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}



[System.Serializable]
public class BlockType
{

    public string blockName;
    public bool isSolid;

    [Header("Texture Values")]
    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int botFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    public int GetTextureID(int faceIndex)
    {
        switch (faceIndex)
        {
            case 0:
                return backFaceTexture;

            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return botFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.Log("Error in GetTextId");
                return 0;
        }


    }
}