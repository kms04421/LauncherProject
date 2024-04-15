using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
    List<Chunk> chunksToUpdate = new List<Chunk>();

    bool applyingModifications = false;

    /*     private static World instance;
     * public static World Instance()
        {
            // �ν��Ͻ��� ���� �������� �ʾҴٸ� ����
            {
                instance = new World();
            }
            return instance;
        }
    */

    Queue<VoxelMod> modifications = new Queue<VoxelMod>();

    private void Start()
    {
        Random.InitState(seed); // �õ� �ʱ�ȭ 
        playerLastChunkCoord = GetChunkCorrdFromVector3(player.position); // ������ �÷��̾� ûũ ��ġ
        spawnPosition = new Vector3((VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight + 2f, VoxelData.WorldSizeInChunks); //���� ��ġ ���� 
        GenerateWorld();//���� ����
        

        Invoke("playerAct", 2);
    }

    private void Update()
    {
        WorldUpdate();

    }
    void playerAct()
    {
        player.GetComponent<PlayerController>().enabled = true;
    }
    void WorldUpdate()
    {
        PlayerChunkCoord = GetChunkCorrdFromVector3(player.position); // ���� �÷��̾� ûũ ��ǥ ������Ʈ 
        if (!PlayerChunkCoord.Equals(playerLastChunkCoord)) // �÷��̾� ��ǥ ����� ûũ ��ǥ ���� 
        {
            CheckViewDistance();
        }

        if (modifications.Count > 0 && !applyingModifications)
        {
            StartCoroutine(ApplyModifications());
        }
        if (chunksToCreate.Count > 0)
        {
            CreateChunk();
        }
        if (chunksToUpdate.Count > 0)
        {
            UpdateChunks();
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

        while (modifications.Count > 0)
        {
            VoxelMod v = modifications.Dequeue();

            ChunkCoord c = GetChunkCorrdFromVector3(v.position);

            if (chunks[c.x, c.z] == null)
            {
                chunks[c.x, c.z] = new Chunk(c, this, true);
                activeCunks.Add(c);
            }

            chunks[c.x, c.z].modifgications.Enqueue(v);
            if (!chunksToUpdate.Contains(chunks[c.x, c.z]))
            {
                chunksToUpdate.Add(chunks[c.x, c.z]);
            }

        }
        for (int i = 0; i < chunksToUpdate.Count; i++)
        {
            chunksToUpdate[0].UpdateChunk();
            chunksToUpdate.RemoveAt(0);
        }

        player.position = spawnPosition; // �÷��̾� ��ġ ���� 
   
    }



    /*   IEnumerator CreateChunks() // ûũ ���� �ڷ�ƾ 
       {
           isCreatingChunk = true;

           while (chunksToCreate.Count > 0)
           {
               chunks[chunksToCreate[0].x, chunksToCreate[0].z].Init(); // ûũ �ʱ�ȭ �� ���� 

               chunksToCreate.RemoveAt(0);

               yield return null;
           }

           isCreatingChunk = false;
       }*/
    ChunkCoord GetChunkCorrdFromVector3(Vector3 pos) // ûũ ��ġ ��ȯ 
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return new ChunkCoord(x, z);
    }

    public Chunk GetChunkFromVector3(Vector3 pos) // ûũ �迭 ���� ��ȯ 
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

        if (yPos == terrainHeight)
        {
            if (Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.treeZoneScale) > biome.treeZoneThreshold)
            {
                if (Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.treePlacementScale) > biome.treePlacementThreshold)
                {
                    Structure.MakeTree(pos, modifications, biome.minTreeHeight, biome.maxTreeHeight);
                }

            }

        }

        return voxelValue;


    }

    void CreateChunk()
    {

        ChunkCoord c = chunksToCreate[0];
        chunksToCreate.RemoveAt(0);
        activeCunks.Add(c);
        chunks[c.x, c.z].Init();

    }

    void UpdateChunks()
    {

        bool updated = false;
        int index = 0;

        while (!updated && index < chunksToUpdate.Count - 1)
        {

            if (chunksToUpdate[index].isVoxelMapPopulated)
            {
                chunksToUpdate[index].UpdateChunk();
                chunksToUpdate.RemoveAt(index);
                updated = true;
            }
            else
            {
                index++;

            }

        }

    }

    IEnumerator ApplyModifications()
    {
        applyingModifications = true;
        int count = 0;
        while (modifications.Count > 0)
        {
            VoxelMod v = modifications.Dequeue();

            ChunkCoord c = GetChunkCorrdFromVector3(v.position);

            if (chunks[c.x, c.z] == null)
            {
                chunks[c.x, c.z] = new Chunk(c, this, true);
                activeCunks.Add(c);
            }

            chunks[c.x, c.z].modifgications.Enqueue(v);
            if (!chunksToUpdate.Contains(chunks[c.x, c.z]))
            {
                chunksToUpdate.Add(chunks[c.x, c.z]);
            }
            count++;
            if (count > 200)
            {
                count = 0;
                yield return null;
            }
        }
        applyingModifications = false;
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

public class VoxelMod
{
    public Vector3 position;
    public byte id;

    public VoxelMod()
    {
        position = new Vector3();
        id = 0;
    }
    public VoxelMod(Vector3 _position, byte _id)
    {
        position = _position;
        id = _id;

    }

}