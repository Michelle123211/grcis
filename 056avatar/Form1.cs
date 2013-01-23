﻿using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Scene3D;

namespace _056avatar
{
  public partial class Form1 : Form
  {
    protected SceneBrep scene = new SceneBrep();

    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    bool loaded = false;

    /// <summary>
    /// Are we allowed to use VBO?
    /// </summary>
    bool useVBO = true;

    #region OpenGL globals

    private uint[] VBOid = new uint[ 2 ];       // vertex array (colors, normals, coords), index array
    private int stride = 0;                     // stride for vertex array

    #endregion

    #region FPS counter

    long lastFpsTime = 0L;
    int frameCounter = 0;
    long triangleCounter = 0L;

    #endregion

    public Form1 ()
    {
      InitializeComponent();
      String[] tok = "$Rev$".Split( ' ' );
      Text += " (rev: " + tok[ 1 ] + ')';
    }

    private void buttonOpen_Click ( object sender, EventArgs e )
    {
      OpenFileDialog ofd = new OpenFileDialog();

      ofd.Title = "Open Scene File";
      ofd.Filter = "Wavefront OBJ Files|*.obj;*.obj.gz" +
          "|All scene types|*.obj";

      ofd.FilterIndex = 1;
      ofd.FileName = "";
      if ( ofd.ShowDialog() != DialogResult.OK )
        return;

      WavefrontObj objReader = new WavefrontObj();
      objReader.MirrorConversion = false;
      int faces = objReader.ReadBrep( ofd.FileName, scene );
      scene.BuildCornerTable();
      int errors = scene.CheckCornerTable( null );
      scene.GenerateColors( 12 );

      labelFaces.Text = String.Format( "{0}: {1} faces, {2} errors", ofd.SafeFileName, faces, errors );
      PrepareDataBuffers();
      glControl1.Invalidate();
    }

    private void buttonGenerate_Click ( object sender, EventArgs e )
    {
      Cursor.Current = Cursors.WaitCursor;
      string param = textParam.Text;

      scene.Reset();
      Construction cn = new Construction();
      int faces = cn.AddMesh( scene, Matrix4.Identity, param );
      scene.BuildCornerTable();
      int errors = scene.CheckCornerTable( null );
      if ( !scene.HasColors() )
        scene.GenerateColors( 12 );

      Cursor.Current = Cursors.Default;

      if ( faces == 0 )
        faces = scene.Triangles;
      labelFaces.Text = String.Format( "{0} faces, {1} errors", faces, errors );
      PrepareDataBuffers();
      glControl1.Invalidate();
    }

    private void buttonSave_Click ( object sender, EventArgs e )
    {
      //if ( outputImage == null ) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      //outputImage.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      loaded = true;

      // OpenGL init code:
      GL.ClearColor( Color.DarkBlue );
      GL.Enable( EnableCap.DepthTest );
      GL.ShadeModel( ShadingModel.Flat );

      // VBO init:
      GL.GenBuffers( 2, VBOid );
      if ( GL.GetError() != ErrorCode.NoError )
        useVBO = false;

      SetupViewport( true );

      Application.Idle += new EventHandler( Application_Idle );
    }

    private void glControl1_Resize ( object sender, EventArgs e )
    {
      if ( !loaded ) return;

      SetupViewport( false );
      glControl1.Invalidate();
    }

    private void glControl1_Paint ( object sender, PaintEventArgs e )
    {
      Render();
    }

    private void control_PreviewKeyDown ( object sender, PreviewKeyDownEventArgs e )
    {
      if ( e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right )
        e.IsInputKey = true;
    }

    /// <summary>
    /// Prepare VBO content and upload it to the GPU.
    /// </summary>
    private void PrepareDataBuffers ()
    {
      if ( useVBO &&
           scene != null &&
           scene.Triangles > 0 )
      {
        GL.EnableClientState( ArrayCap.VertexArray );
        if ( scene.HasNormals() )
          GL.EnableClientState( ArrayCap.NormalArray );
        if ( scene.HasColors() )
          GL.EnableClientState( ArrayCap.ColorArray );

        // Vertex array: [color] [normal] coord
        GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
        int vertexBufferSize = scene.VertexBufferSize( true, false, true, true );
        GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)vertexBufferSize, IntPtr.Zero, BufferUsageHint.StaticDraw );
        IntPtr videoMemoryPtr = GL.MapBuffer( BufferTarget.ArrayBuffer, BufferAccess.WriteOnly );
        unsafe
        {
          stride = scene.FillVertexBuffer( (float*)videoMemoryPtr.ToPointer(), true, false, true, true );
        }
        GL.UnmapBuffer( BufferTarget.ArrayBuffer );
        GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );

        // Index buffer
        GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );
        GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)(scene.Triangles * 3 * sizeof( uint )), IntPtr.Zero, BufferUsageHint.StaticDraw );
        videoMemoryPtr = GL.MapBuffer( BufferTarget.ElementArrayBuffer, BufferAccess.WriteOnly );
        unsafe
        {
          scene.FillIndexBuffer( (uint*)videoMemoryPtr.ToPointer() );
        }
        GL.UnmapBuffer( BufferTarget.ElementArrayBuffer );
        GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
      }
      else
      {
        GL.DisableClientState( ArrayCap.VertexArray );
        GL.DisableClientState( ArrayCap.NormalArray );
        GL.DisableClientState( ArrayCap.ColorArray );

        if ( useVBO )
        {
          GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
          GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)0, IntPtr.Zero, BufferUsageHint.StaticDraw );
          GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
          GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );
          GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)0, IntPtr.Zero, BufferUsageHint.StaticDraw );
          GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
        }
      }
    }
  }
}
