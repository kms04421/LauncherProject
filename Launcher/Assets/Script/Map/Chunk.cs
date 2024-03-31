using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public ChunkCoord coord;

    GameObject chunkObject;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    int vertexIndex = 0;
    List<Vector3> vectices = new List<Vector3>();
    List<int> tringles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    World world; // �� ��������

    public Chunk(ChunkCoord _coord, World _world)
    {
        coord = _coord;
        world = _world;
        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();

        meshRenderer.material = world.material;
        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
        chunkObject.name = "Chunk" + coord.x + ", " + coord.z;

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
                    voxelMap[x, y, z] = world.GetVoxel(new Vector3(x, y, z) + position);
                   
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

    public bool isActive
    {
        get { return chunkObject.activeSelf; }
        set { chunkObject.SetActive(value); }
    }
    public Vector3 position
    {
        get { return chunkObject.transform.position; }
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
    bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || 
            z < 0 || z > VoxelData.ChunkWidth - 1)
        {
            return false;
        }
        else
        {
            return true;
        }

    }
    bool ChunkVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);
       
        if (!IsVoxelInChunk(x,y,z))
        {         
            return world.blockTypes[world.GetVoxel(pos + position)].isSolid;
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


public class ChunkCoord
{
    public int x;
    public int z;

    public ChunkCoord(int _x ,int _z)
    {
        x = _x;
        z = _z;
    }

    public bool Equals (ChunkCoord other)
    {
        if (other == null)
        {
            return false;
        }
        else if (other.x == x && other.z == z)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
} 