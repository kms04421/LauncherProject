using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Chunk   
{
    public ChunkCoord coord; // 청크 좌표 

    GameObject chunkObject; // 청크 오브젝트 
    MeshRenderer meshRenderer; // 메쉬 렌더
    MeshFilter meshFilter; // 메쉬 필터 

    int vertexIndex = 0; 
    List<Vector3> vectices = new List<Vector3>(); // 정점 인덱스 
    List<int> tringles = new List<int>(); // 
    List<Vector2> uvs = new List<Vector2>();
    public byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    World world; // 맵 블럭정보

    private bool _isActive;
    public bool isVoxelMapPopulated = false;

    MeshCollider meshCollider;
    public Chunk(ChunkCoord _coord, World _world , bool generateOnLooad)
    {
        coord = _coord;
        world = _world;

        isActive = true;

        if(generateOnLooad)
        {
            Init();
        }
    }

    
    public void Init() 
    {
        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();

        meshCollider = chunkObject.AddComponent<MeshCollider>();

        meshRenderer.material = world.material;
        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
        chunkObject.name = "Chunk" + coord.x + ", " + coord.z;

        PopulateVoxelMap();
        UpdateChunk();
 

        meshCollider.sharedMesh = meshFilter.mesh;
        chunkObject.layer = LayerMask.NameToLayer("Block");
    }

    void PopulateVoxelMap() // 블록맵 생성 
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
        isVoxelMapPopulated = true;
    }

    void UpdateChunk() // 메쉬 생성 
    {
        ClearMeshData();
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    if(world.blockTypes[voxelMap[x,y,z]].isSolid)
                    {
                        UpdateMeshData(new Vector3(x, y, z));
                    }

                }
            }
        }
        CreateMesh();
    }

    void ClearMeshData()
    {
        vertexIndex = 0;
        vectices.Clear();
        tringles.Clear();
        uvs.Clear();
      

    }

    public bool isActive // 청크 활성화 여부 
    {
        get { return _isActive; }
        set {
            _isActive = value;
            if (chunkObject != null)
            {
                chunkObject.SetActive(value);
           
            }
        }
    }

    
    public Vector3 position // 청크 위치 
    {
        get { return chunkObject.transform.position; }
    }



    void UpdateMeshData(Vector3 pos) // 블록 mesh 정보 
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

        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
            return false;
        else
            return true;

    }
    public void EditVoxel(Vector3 pos, byte newID)
    {

        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

        voxelMap[xCheck, yCheck, zCheck] = newID;

        UpdateSurroundingVoxels(xCheck, yCheck, zCheck);

        UpdateChunk();

    }

    void UpdateSurroundingVoxels(int x, int y, int z)
    {

        Vector3 thisVoxel = new Vector3(x, y, z);

        for (int p = 0; p < 6; p++)
        {

            Vector3 currentVoxel = thisVoxel + VoxelData.faceChunk[p];

            if (!IsVoxelInChunk((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z))
            {

                world.GetChunkFromVector3(currentVoxel + position).UpdateChunk();

            }

        }

    }
 
    bool ChunkVoxel(Vector3 pos) // 청크 내 블록 확인 
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);
        
        if (!IsVoxelInChunk(x,y,z))
        {
           
            return world.CheckForVoxel(pos + position);
        }
       
        return world.blockTypes[voxelMap[x, y, z]].isSolid;
    }


    public byte GetVoxelFromGlobalVector3 (Vector3 pos) // 블록 글로 벌 좌표 
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);
 
        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);
              
        return voxelMap[xCheck, yCheck, zCheck];
    }

    void CreateMesh() // 메쉬 생성 
    {

        Mesh mesh = new Mesh();
        mesh.vertices = vectices.ToArray();
        mesh.triangles = tringles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    void AddTexture (int textureID) // 텍스처 추가 
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
    public int x; // x 좌표 저장 
    public int z; // z 좌표 저장 

    public ChunkCoord()
    {
        x = 0;
        z = 0;
    }

    public ChunkCoord(int _x ,int _z)
    {
        x = _x;
        z = _z;
    }

    public ChunkCoord(Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int zCheck = Mathf.FloorToInt(pos.z);

        x = xCheck / VoxelData.ChunkWidth;
        z = zCheck / VoxelData.ChunkWidth;
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