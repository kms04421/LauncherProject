using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;



public class World : MonoBehaviour
{
    public int seed; // 월드 시드 
    public BiomeAttributes biome; // 바이옴 속성 

    public Transform player; // 플레이 위치 
    public Vector3 spawnPosition; // 스폰위치 


    public Material material; // 마테리얼 
    public BlockType[] blockTypes; // 블록 유형 배열 

    Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks]; // 청크 배열 

    List<ChunkCoord> activeCunks = new List<ChunkCoord>(); // 활성 청크 목록 
    ChunkCoord PlayerChunkCoord; //  플레이어 청크 좌표 
    ChunkCoord playerLastChunkCoord; // 마지막 플레이어 청크 좌표 

    List<ChunkCoord> chunksToCreate = new List<ChunkCoord>(); // 생성할 청크 목록 

    private bool isCreatingChunk; //청크 생성 여부 


    private void Start()
    {
        Random.InitState(seed); // 시드 초기화 

        spawnPosition = new Vector3((VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight + 2f, VoxelData.WorldSizeInChunks); //스폰 위치 설정 
        GenerateWorld();//월드 생성
        playerLastChunkCoord = GetChunkCorrdFromVector3(player.position); // 마지막 플레이어 청크 위치

        
    }

    private void Update()
    {
        WorldUpdate();

    }
    
    void WorldUpdate() 
    {
        PlayerChunkCoord = GetChunkCorrdFromVector3(player.position); // 현제 플레이어 청크 좌표 업데이트 
        if (!PlayerChunkCoord.Equals(playerLastChunkCoord)) // 플레이어 좌표 변경시 청크 좌표 변경 
        {
            CheckViewDistance();
        }

        if (chunksToCreate.Count > 0 && !isCreatingChunk) // 생성할 청크 존재 시 생성 
        {
            StartCoroutine("CreateChunks"); // 동시 실행 안하니까 문제 x

        }

    }

    void GenerateWorld() // 월드 생성 
    {
        for (int x = (VoxelData.WorldSizeInChunks / 2) - VoxelData.ViewDistanceInChunks; x < (VoxelData.WorldSizeInChunks / 2) + VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = (VoxelData.WorldSizeInChunks / 2) - VoxelData.ViewDistanceInChunks; z < (VoxelData.WorldSizeInChunks / 2) + VoxelData.ViewDistanceInChunks; z++)
            {
                chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, true);
                activeCunks.Add(new ChunkCoord(x, z));

            }
        }

        player.position = spawnPosition; // 플레이어 위치 설정 
        CheckViewDistance();
    }

    IEnumerator CreateChunks() // 청크 생성 코루틴 
    {
        isCreatingChunk = true;

        while (chunksToCreate.Count > 0)
        {
            chunks[chunksToCreate[0].x, chunksToCreate[0].z].Init(); // 청크 초기화 및 생성 

            chunksToCreate.RemoveAt(0);

            yield return null;
        }

        isCreatingChunk = false;
    }
    ChunkCoord GetChunkCorrdFromVector3(Vector3 pos) // 청크 위치 반환 
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

    void CheckViewDistance() // 청크 활성화 
    {

        ChunkCoord coord = GetChunkCorrdFromVector3(player.position);
        playerLastChunkCoord = PlayerChunkCoord;

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeCunks); // 이전 활성 청크 목록 복사

        // 주변 청크 활성화 
        for (int x = coord.x - VoxelData.ViewDistanceInChunks; x < coord.x + VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = coord.z - VoxelData.ViewDistanceInChunks; z < coord.z + VoxelData.ViewDistanceInChunks; z++)
            {
                if (IsChunkWorld(new ChunkCoord(x, z)))
                {
                    if (chunks[x, z] == null)
                    {
                        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, false); // 청크 생성 

                       

                        chunksToCreate.Add(new ChunkCoord(x, z)); // 생성할 청크 목록에 추가 
                    }
                    else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;

                    }
                    activeCunks.Add(new ChunkCoord(x, z)); // 활성화한 청크 추가 
                }

                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                {
                    if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z))) // 이전 청크 중복 정보 제거 
                    {
                        previouslyActiveChunks.RemoveAt(i);
                    }
                }
            }
        }

        foreach (ChunkCoord c in previouslyActiveChunks) // 이전 청크 비활성화 
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

    public byte GetVoxel(Vector3 pos)//블럭 정보 세팅
    {
        int yPos = Mathf.FloorToInt(pos.y);

        if (!InVoxelInWorld(pos))
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
    bool InVoxelInWorld(Vector3 pos) // 사이즈 초과 방지 체크
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