using System;
using System.Numerics;

namespace Tutorial;

public class SolidSphere
{
    //converted from https://stackoverflow.com/questions/5988686/creating-a-3d-sphere-in-opengl-using-visual-c/5989676#5989676
    
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
    }
    
    private readonly float[] vertices;
    private readonly float[] normals;
    private readonly float[] texcoords;
    private readonly uint[] indices;
    private readonly uint textureID;

    public float[] Vertices { get => vertices; }
    public float[] Normals { get => normals; }
    public float[] TexCoords { get => texcoords; }
    public uint[] Indices { get => indices; }
    public uint TextureId { get => textureID; }
    
    public Vertex[] Points
    {
        get
        {
            Vertex[] v;
            v = new Vertex[vertices.Length / 3];
            for (int i = 0; i < vertices.Length / 3; i++)
            {
                v[i].Position.X = vertices[i*3];
                v[i].Position.Y = vertices[i*3 + 1];
                v[i].Position.Z = vertices[i*3 + 2];

                v[i].Normal.X = normals[i*3];
                v[i].Normal.Y = normals[i*3 + 1];
                v[i].Normal.Z = normals[i*3 + 2];

                i++;
            }
            
            return v;
        }
    }

    public float MinY
    {
        get
        {
            float minY = 0;
            
            for (int i = 1; i < vertices.Length; i=i+3)
            {
                minY = Math.Min(minY, vertices[i]);
            }

            return minY;
        }
    }
    
    public float MaxY
    {
        get
        {
            float maxY = 0;
            
            for (int i = 1; i < vertices.Length; i=i+3)
            {
                maxY = Math.Max(maxY, vertices[i]);
            }
            
            return maxY;
        }
    }
    

    public SolidSphere(float radius, uint rings, uint sectors)
    {
        float R = 1/(float)(rings-1);
        float S = 1/(float)(sectors-1);
        int r, s, i;

        vertices = new float[rings * sectors * 3];
        normals = new float[rings * sectors * 3];
        texcoords = new float[rings * sectors * 2];
        indices = new uint[rings * sectors * 4];
        
        i = 0;
        for(r = 0; r < rings; r++) for(s = 0; s < sectors; s++) {
                float x = (float)(Math.Cos(2*Math.PI * s * S) * Math.Sin( Math.PI * r * R ));    
                float y = (float)Math.Sin( -Math.PI / 2 + Math.PI * r * R );
                float z = (float)(Math.Sin(2*Math.PI * s * S) * Math.Sin( Math.PI * r * R ));

                texcoords[i*2] = s*S;
                texcoords[i*2+1] = r*R;

                vertices[i*3] = x * radius;
                vertices[i*3+1] = y * radius;
                vertices[i*3+2] = z * radius;

                normals[i*3] = x;
                normals[i*3+1] = y;
                normals[i*3+2] = z;
                
                i++;
        }

        i = 0;
        for(r = 0; r < rings; r++) for(s = 0; s < sectors; s++) {
                indices[i*4] = (ushort)(r * sectors + s);
                indices[i*4+1] = (ushort)(r * sectors + (s+1));
                indices[i*4+2] = (ushort)((r+1) * sectors + (s+1));
                indices[i*4+3] = (ushort)((r+1) * sectors + s);

                i++;
        }
    }
}