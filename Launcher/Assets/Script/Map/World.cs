using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int seed;
    public BiomeAttributes biome;

    public Transform player;
    public Vector3 spawnPosition;


    public Material material;
    public BlockType[] blockTypes;

    Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunls, VoxelData.WorldSizeInChunls];

    List<ChunkCoord> activeCunks = new List<ChunkCoord>();
    ChunkCoord PlayerChunkCoord;
    ChunkCoord playerLastChunkCoord;
    private void Start()
    {
        Random.InitState(seed);

        spawnPosition = new Vector3((VoxelData.WorldSizeInChunls * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight + 2f, VoxelData.WorldSizeInChunls);
        GenerateWorld();//월드 생성
        playerLastChunkCoord = GetChunkCorrdFromVector3(player.position); // 마지막 플레이어 청크 위치
    }

    private void Update()
    {
        PlayerChunkCoord = GetChunkCorrdFromVector3(player.position);
        if(!PlayerChunkCoord.Equals(playerLastChunkCoord))
        {
            CheckViewDistance();
        }
    }

    void GenerateWorld()
    {
        for (int x = (VoxelData.WorldSizeInChunls / 2) - VoxelData.ViewDistanceInChunks; x < (VoxelData.WorldSizeInChunls / 2) + VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = (VoxelData.WorldSizeInChunls / 2) - VoxelData.ViewDistanceInChunks; z < (VoxelData.WorldSizeInChunls / 2) + VoxelData.ViewDistanceInChunks; z++)
            {
                CreateNewChunk(x, z);
            }
        }

        player.position = spawnPosition;
    }
    ChunkCoord GetChunkCorrdFromVector3 (Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return new ChunkCoord(x, z);
    }
    void CheckViewDistance()
    {

        ChunkCoord coord = GetChunkCorrdFromVector3(player.position);

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeCunks);

        for(int x = coord.x - VoxelData.ViewDistanceInChunks; x< coord.x + VoxelData.ViewDistanceInChunks; x++)
        {
            for(int z = coord.z - VoxelData.ViewDistanceInChunks; z < coord.z + VoxelData.ViewDistanceInChunks; z++)
            {
                if (IsChunkWorld(new ChunkCoord(x,z)))
                {
                    if(chunks[x,z] == null)
                    {
                        CreateNewChunk(x, z);
                    }
                    else if (!chunks[x,z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        activeCunks.Add(new ChunkCoord(x, z));
                    }
                }

                for(int i = 0; i < previouslyActiveChunks.Count; i++)
                {
                    if(previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                    {
                        previouslyActiveChunks.RemoveAt(i);
                    }
                }
            }
        }

        foreach(ChunkCoord c in previouslyActiveChunks)
        {
            chunks[c.x, c.z].isActive = false;
        }
    }

    public byte GetVoxel(Vector3 pos)//블럭 정보 세팅
    {
        int yPos = Mathf.FloorToInt(pos.y);

        if(!InVoxelInWorld(pos))
        {
            return 0;
        }


        if (yPos == 0) // 최하단 블럭 
        {
            return 1;

        }
            
        int terrainHeight = Mathf.FloorToInt(biome.terrainHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.terrainScale)) + biome.solidGroundHeight;
        byte voxelValue = 0;

        if (yPos == terrainHeight)
        {
            voxelValue =  3;

        }
        else if(yPos < terrainHeight && yPos > terrainHeight -4)
        {
            voxelValue = 5;
        }
        else if(yPos > terrainHeight)
        {
            return 0;
        }
        else
        {
            voxelValue = 2;
        }

        if(voxelValue == 2)
        {
            foreach(Lode lode in biome.lodes)
            {
                if (yPos > lode.minHeight && yPos < lode.maxHeight)
                {
                    if(Noise.Get3DPerlin(pos,lode.noiseOffset, lode.scale , lode.threshold))
                    {
                        voxelValue = lode.blockID;
                    }
                }
            }
          
        }
        
        return voxelValue;
        
         
    }
    void CreateNewChunk(int x,int z)
    {
        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
        activeCunks.Add(new ChunkCoord(x,z));
    }

    bool IsChunkWorld(ChunkCoord coord)
    {
        if(coord.x > 0 && coord.x < VoxelData.WorldSizeInChunls -1 && coord.z > 0 &&
            coord.z < VoxelData.WorldSizeInChunls -1 )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool InVoxelInWorld(Vector3 pos) // 사이즈 초과 방지 체크
    {
        if(pos.x >= 0 && pos.x <VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight &&
           pos.z >= 0 && pos.z <VoxelData.WorldSizeInVoxels )
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