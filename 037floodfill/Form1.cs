﻿using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace _037floodfill
{
  public partial class Form1 : Form
  {
    protected Thread aThread = null;

    protected Bitmap input = null;

    protected Bitmap working = null;

    volatile protected bool cont = true;

    delegate void SetImageCallback ( Bitmap newImage );

    protected void SetImage ( Bitmap newImage )
    {
      if ( pictureBox1.InvokeRequired )
      {
        SetImageCallback si = new SetImageCallback( SetImage );
        BeginInvoke( si, new object[] { newImage } );
      }
      else
      {
        pictureBox1.Image = newImage;
        pictureBox1.Invalidate();
      }
    }

    delegate void SetTextCallback ( string text );

    protected void SetText ( string text )
    {
      if ( labelQueue.InvokeRequired )
      {
        SetTextCallback st = new SetTextCallback( SetText );
        BeginInvoke( st, new object[] { text } );
      }
      else
        labelQueue.Text = text;
    }

    delegate void StopFillCallback ();

    protected void StopFill ()
    {
      if ( aThread == null ) return;

      if ( buttonStart.InvokeRequired )
      {
        StopFillCallback ea = new StopFillCallback( StopFill );
        BeginInvoke( ea );
      }
      else
      {
        // actually stop the fill:
        cont = false;
        aThread.Join();
        aThread = null;

        // GUI stuff:
        buttonStart.Enabled = true;
        buttonStop.Enabled = false;
      }
    }

    public Form1 ()
    {
      InitializeComponent();
    }

    public void FillProcess ()
    {
      int granularity = (int)numericGranul.Value;
      int counter;
      int wid = working.Width  - 1;
      int hei = working.Height - 1;

      int x = (int)numericXstart.Value;
      if ( x > wid ) x = wid;
      int y = (int)numericYstart.Value;
      if ( y > hei ) y = hei;
      Queue<Point> q = new Queue<Point>();
      int maxQ;

      Color orig = working.GetPixel( x, y );
      if ( orig.Equals( Color.Red ) )
      {
        SetText( "Queue: 0 (Max = 0)" );
        StopFill();
        return;
      }

      working.SetPixel( x, y, Color.Red );
      q.Enqueue( new Point( x, y ) );
      maxQ = 1;
      counter = 1;
      int frameNo = 0;

      while ( cont && q.Count > 0 )
      {
        Point p = q.Dequeue();
        x = p.X;
        y = p.Y;

        if ( x > 0 && working.GetPixel( x - 1, y ).Equals( orig ) )
        {
          working.SetPixel( x - 1, y, Color.Red );
          q.Enqueue( new Point( x - 1, y ) );
        }

        if ( x < wid && working.GetPixel( x + 1, y ).Equals( orig ) )
        {
          working.SetPixel( x + 1, y, Color.Red );
          q.Enqueue( new Point( x + 1, y ) );
        }

        if ( y > 0 && working.GetPixel( x, y - 1 ).Equals( orig ) )
        {
          working.SetPixel( x, y - 1, Color.Red );
          q.Enqueue( new Point( x, y - 1 ) );
        }

        if ( y < hei && working.GetPixel( x, y + 1 ).Equals( orig ) )
        {
          working.SetPixel( x, y + 1, Color.Red );
          q.Enqueue( new Point( x, y + 1 ) );
        }

        if ( q.Count > maxQ ) maxQ = q.Count;

        if ( (++counter % granularity) == 1 ||
             q.Count == 0 )
        {
          SetImage( new Bitmap( working ) );
          SetText( String.Format( "Queue: {0} (Max = {1})", q.Count, maxQ ) );
          if ( checkSnap.Checked )
          {
            string fileName = String.Format( "out{0:0000}.png", frameNo++ );
            Bitmap s = new Bitmap( working );
            s.Save( fileName, System.Drawing.Imaging.ImageFormat.Png );
          }
        }
      }

      StopFill();
    }

    private void buttonStart_Click ( object sender, EventArgs e )
    {
      if ( aThread != null ||
           input == null ) return;

      buttonStart.Enabled = false;
      buttonStop.Enabled  = true;
      cont = true;

      working = new Bitmap( input );

      aThread = new Thread( new ThreadStart( this.FillProcess ) );
      aThread.Start();
    }

    private void buttonLoad_Click ( object sender, EventArgs e )
    {
      OpenFileDialog ofd = new OpenFileDialog();

      ofd.Title = "Open Image File";
      ofd.Filter = "Bitmap Files|*.bmp" +
          "|Gif Files|*.gif" +
          "|JPEG Files|*.jpg" +
          "|PNG Files|*.png" +
          "|TIFF Files|*.tif" +
          "|All image types|*.bmp;*.gif;*.jpg;*.png;*.tif";

      ofd.FilterIndex = 6;
      ofd.FileName = "";
      if ( ofd.ShowDialog() != DialogResult.OK )
        return;

      input = (Bitmap)Image.FromFile( ofd.FileName );

      // Automatic filename parsing..
      buttonLoad.Text = ofd.SafeFileName;
      string[] parts = ofd.SafeFileName.Split( '.' );
      int len = parts.Length;
      if ( len >= 3 )
      {
        int xStart = 0, yStart = 0;
        if ( Int32.TryParse( parts[ len - 3 ], out xStart ) &&
             Int32.TryParse( parts[ len - 2 ], out yStart ) )
        {
          numericXstart.Value = xStart;
          numericYstart.Value = yStart;
        }
      }
    }

    private void buttonStop_Click ( object sender, EventArgs e )
    {
      StopFill();
    }

    private void Form1_FormClosing ( object sender, FormClosingEventArgs e )
    {
      StopFill();
    }
  }
}
