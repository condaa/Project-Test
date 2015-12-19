using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IntelligentScissors
{
    public partial class MainForm : Form
    {









        List<Point> AnchorPts;
        float[] DashPattern = { 1, 2 };
        Point AnchorSize = new Point(5, 5);
        float clr = 0.0f;
        float W8interval = .02f;
        ShortestPath_Operations path_finder;

        List<Point> Mainselction;
        Point[] curr_path; 

        int curr_source  =  -1 ,prev_source =-1  ;
       


       

        void init()
        {
            AnchorPts = new List<Point>();
            Mainselction = new List<Point>(); 
        }




        public MainForm()
        {
            InitializeComponent();
            init(); 
        }

        RGBPixel[,] ImageMatrix;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
                AnchorPts.Clear(); 
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
            List<List<Edge>> graph = GraphOperations.Graph_Constraction(ImageMatrix);


          path_finder = new ShortestPath_Operations(graph, ImageOperations.GetWidth(ImageMatrix));
     


            
        }





      
        public void update(MouseEventArgs e)
        {
            var g = pictureBox1.CreateGraphics();
            if (clr > W8interval * 2)
            {

                var p1 = new Point(0, 0);
                var p3 = e.Location;
                //  g. DrawLine(new Pen(Brushes.Blue), p1, p3);


                customDrawer.drawCrossHair(g, e, new Pen(Brushes.BlueViolet));

                if (ImageMatrix != null)
                {

                    var mouseNode = Helper.Flatten(e.X, e.Y, ImageOperations.GetWidth(ImageMatrix));
                    int source = curr_source ;


                 


                    if (source != -1)
                    {
                        Point[] path = null ; 
                        
                        if (source != prev_source) {
                              path = path_finder.Pathfind(source, mouseNode, PFmode.update);
 }
                        else 
                         path = path_finder.Pathfind(source, mouseNode, PFmode.exist);
                    
                    
                        if ( path.Length > 10 ) 
                        customDrawer.drawDottedLine(g, new Pen( Brushes.Aqua ) , path, DashPattern);

                        prev_source = source;
                        curr_path = path; 

                    }

                }






                clr = 0.0f;
            }

            if (clr > W8interval)
            {
                pictureBox1.Refresh();
                g.Dispose();
            }

            clr += .019f;
        } 






        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value ;
            ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
        }

        private void pictureBox1_MouseMove_1(object sender, MouseEventArgs e)
        {
            update(e);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

            if (ImageMatrix != null)
            {
                var g = e.Graphics;
                for (int i = 0; i < AnchorPts.Count; i++)
                {
                    g.FillEllipse(Brushes.Yellow, new Rectangle(
                        new Point(AnchorPts[i].X - AnchorSize.X / 2, AnchorPts[i].Y - AnchorSize.Y / 2),
                        new Size(AnchorSize)));
                }

                if (Mainselction != null && Mainselction.Count > 5)
                    customDrawer.drawDottedLine(e.Graphics, new Pen(Brushes.Orange), Mainselction.ToArray(), DashPattern);

            }
            
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (ImageMatrix != null)
            {

                if ( curr_path != null )
                Helper.AppendToList<Point>(Mainselction, curr_path);

                AnchorPts.Add(e.Location);
                curr_source = Helper.Flatten(AnchorPts[AnchorPts.Count - 1].X, AnchorPts[AnchorPts.Count - 1].Y, ImageOperations.GetWidth(ImageMatrix));
                
            }
        }

    }
}



public class customDrawer
{


    customDrawer()
    {
    }

    /// <summary>
    /// old function to draw dashed line 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="arr"></param>
    /// <param name="interval"></param>
    static void drawDottedLine(PaintEventArgs e, Pen p, PointF[] arr, int interval)
    {

        for (int i = 0; i < arr.Length - interval; i += interval * 2)
        {
            PointF[] tmpArr = new PointF[interval];
            for (int j = 0, ii = i; j < interval; j++, ii++)
                tmpArr[j] = arr[ii];

            e.Graphics.DrawCurve(p, tmpArr);

        }


    }
    /// <summary>
    /// new func 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="arr"></param>
    /// <param name="_dash_vals"></param>
    public static void drawDottedLine(Graphics g, Pen p, Point[] arr, float[] _dash_vals)
    {

        p.DashPattern = _dash_vals;
        g.DrawCurve(p, arr);
    }


    public static void drawDottedLine(Graphics g, Pen p, Point A, Point B, float[] _dash_vals)
    {
        Point[] arr = new Point[2];
        arr[0] = A;
        arr[1] = B;

        drawDottedLine(g, p, arr, _dash_vals);

    }

    public static void drawCrossHair(Graphics g, MouseEventArgs em, Pen P)
    {
        var orgin = em.Location;
        // horizontal 
        var p1 = new Point(0, orgin.Y);
        var p3 = new Point(1000, orgin.Y);

        // vertical
        var p2 = new Point(orgin.X, 0);
        var p4 = new Point(orgin.X, 1000);


        g.DrawLine(P, p1, p3);
        g.DrawLine(P, p2, p4);

    }

}
