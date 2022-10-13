using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using SixLabors.ImageSharp.Memory;

namespace Tutorial;

public class Sphere
{
    //converted from https://stackoverflow.com/a/31804515

    public bool _init;          // has been initiated ?
    public float x0, y0, z0;    // center of sphere [GCS]
    public float[,,] pos;       // vertex
    public float[,,] nor;       // normal
    public float[,,] txr;       // texcoord
    public float t;             // rotation angle [deg]

    private uint _txrid;
    private GL _gl;
    private uint _rings;
    private uint _sectors;

    public Sphere(uint rings, uint sectors, GL gl)
    {
        _init = false;

        pos = new float[sectors, rings, 3];
        nor = new float[sectors, rings, 3];
        txr = new float[sectors, rings, 2];

        _gl = gl;

        _txrid = 0;
        x0 = 0.0f;
        y0 = 0.0f;
        z0 = 0.0f;
        t = 0.0f;

    }

    ~Sphere()
    {
        if (_init) _gl.DeleteTexture(_txrid);
    }

    public unsafe void Init(float radius, string path)
    {
        if (!_init)
        {
            _init = true;

            _txrid = _gl.GenTexture();
            _gl.ActiveTexture(TextureUnit.Texture0);
            _gl.BindTexture(TextureTarget.Texture2D, _txrid);
        }

        float x, y, z, a, b, da, db;
        int ia, ib;

        // load texture
        using (var img = Image.Load<Rgba32>(path))
        {
            _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, (uint) img.Width, (uint) img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);

            img.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    fixed (void* data = accessor.GetRowSpan(y))
                    {
                        _gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, y, (uint) accessor.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                    }
                }
            });
            
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.ClampToEdge);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.ClampToEdge);
            //_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.Repeat);
            //_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.Repeat);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.LinearMipmapLinear);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);
            //_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.Nearest);
            //_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Nearest);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
            _gl.GenerateMipmap(TextureTarget.Texture2D);

            da = (float)((2.0 * Math.PI) / (_sectors - 1));
            db = (float)(Math.PI / (_rings - 1));
            for (ib = 0, b = (float)(-0.5 * Math.PI); ib < _rings; ib++, b += db)
            for (ia = 0, a = (float)0.0; ia < _sectors; ia++, a += da)
            {
                x = (float)(Math.Cos(b) * Math.Cos(a));
                y = (float)(Math.Cos(b) * Math.Sin(a));
                z = (float)(Math.Sin(b));
                nor[ia,ib,0] = x;
                nor[ia,ib,1] = y;
                nor[ia,ib,2] = z;
                pos[ia,ib,0] = radius * x;
                pos[ia,ib,1] = radius * y;
                pos[ia,ib,2] = radius * z;
            }
        }
    }

    void Draw()
    {
        if (!_init) return;
        int ia, ib0, ib1;
        /*
        glMatrixMode(GL_MODELVIEW);
        glPushMatrix();
        glLoadIdentity();
        glTranslatef(x0, y0, z0);
        glRotatef(90.0, 1.0, 0.0, 0.0); // rotate sphere's z axis (North) to OpenGL y axis (Up)
        glRotatef(-t, 0.0, 0.0, 1.0); // rotate sphere's z axis (North) to OpenGL y axis (Up)

        glEnable(GL_TEXTURE_2D);
        _gl.BindTexture(TextureTarget.Texture2D, _txrid);
        for (ib0 = 0, ib1 = 1; ib1 < _rings; ib0 = ib1, ib1++)
        {
            glBegin(GL_QUAD_STRIP);
            for (ia = 0; ia < na; ia++)
            {
                glNormal3fv(nor[ia][ib0]);
                glTexCoord2fv(txr[ia][ib0]);
                glVertex3fv(pos[ia][ib0]);
                glNormal3fv(nor[ia][ib1]);
                glTexCoord2fv(txr[ia][ib1]);
                glVertex3fv(pos[ia][ib1]);
            }

            glEnd();
        }
        

        glDisable(GL_TEXTURE_2D);
        glMatrixMode(GL_MODELVIEW);
        glPopMatrix();
        */
    }
}