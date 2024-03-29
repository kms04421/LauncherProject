using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunt : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    int vertexIndex = 0;
    List<Vector3> vectices = new List<Vector3>();
    List<int> tringles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    World world; // ¸Ê ºí·°Á¤º¸

    void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();

        PopulateVoxelMap();
        CreateChunkMesh();
        CreateMesh();
    }
    void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    if(y < 1)
                    {
                        voxelMap[x, y, z] = 0;
                    }else if(y == VoxelData.ChunkHeight-1)
                    {
                        voxelMap[x, y, z] = 2;
                    }
                    else
                    {
                        voxelMap[x, y, z] = 1;
                    }
                   
                }
            }
        }
    }

    void CreateChunkMesh()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    AddVoxelDataToChunk(new Vector3(x, y, z));
                }
            }
        }
    }

    void AddVoxelDataToChunk(Vector3 pos)
    {
        for (int p = 0; p < 6; p++)
        {
            if(!ChunkVoxel(pos + VoxelData.faceChunk[p]))
            {

                byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];

                vectices.Add(pos + VoxelData.vexelVerts[VoxelData.voxelTris[p, 0]]);
                vectices.Add(pos + VoxelData.vexelVerts[VoxelData.voxelTris[p, 1]]);
                vectices.Add(pos + VoxelData.vexelVerts[VoxelData.voxelTris[p, 2]]);
                vectices.Add(pos + VoxelData.vexelVerts[VoxelData.voxelTris[p, 3]]);

                AddTexture(world.blockTypes[blockID].GetTextureID(p));

                tringles.Add(vertexIndex);
                tringles.Add(vertexIndex + 1);
                tringles.Add(vertexIndex + 2);
                tringles.Add(vertexIndex + 2);
                tringles.Add(vertexIndex + 1);
                tringles.Add(vertexIndex + 3);
                vertexIndex += 4;

            }
         
        }

    }
    bool ChunkVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x > VoxelData.ChunkWidth -1 || y < 0 || y > VoxelData.ChunkHeight -1 || z < 0  || z > VoxelData.ChunkWidth-1 )
        {
            return false;
        }

        return world.blockTypes[voxelMap[x, y, z]].isSolid;
    }
    void CreateMesh()
    {

        Mesh mesh = new Mesh();
        mesh.vertices = vectices.ToArray();
        mesh.triangles = tringles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    void AddTexture (int textureID)
    {
        float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks);

        x *= VoxelData.NormalizeBlockTextureSize;
        y *= VoxelData.NormalizeBlockTextureSize;

        y = 1f - y - VoxelData.NormalizeBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.NormalizeBlockTextureSize));
        uvs.Add(new Vector2(x + VoxelData.NormalizeBlockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.NormalizeBlockTextureSize, y + VoxelData.NormalizeBlockTextureSize));
    }
}
