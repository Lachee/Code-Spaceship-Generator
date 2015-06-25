using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace CodeSpaceshipGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Add a link to the LinkLabel.
            LinkLabel.Link link = new LinkLabel.Link();
            link.LinkData = "https://twitter.com/Managore/status/577252673621102592";
            linkLabel1.Links.Add(link);

            string path = "../../Form1.cs";

            int lineHeight = 10;
            int tabSize = 5;
            
            Color bkgColor = Color.FromArgb(34, 34, 34);
            Color polyColor = Color.White;

            GenerateImage(path, lineHeight, tabSize, bkgColor, polyColor);
        }

        public void GenerateImage(string path, int lineHeight, int tabSize, Color bkgColor, Color polyColor, int imageWidth = 256)
        {
            //Get all the points. Its length will always be the length of the image
            Point[] pointsA = GetTabPoints(path, imageWidth, 0, tabSize, lineHeight);
            Point[] pointsB = GetTabPoints(path, imageWidth, 0, -tabSize, lineHeight);


            //Prepare the bitmap
            Bitmap image = new Bitmap(512, pointsA.Length * lineHeight);

            //Background Color
            //Color bkgColor = Color.FromArgb(34, 34, 34);
            //Color polyColor = Color.White;
            //Color polyColorOther = Color.White;

            //Draw the polygon
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.FillRectangle(new SolidBrush(bkgColor), 0, 0, image.Width - 1, image.Height - 1);
                graphics.FillPolygon(new SolidBrush(polyColor), pointsA);
                graphics.FillPolygon(new SolidBrush(polyColor), pointsB);
            }

            //Set the image
            pictureBox1.Image = image;

            //Save the image
            if (checkbox_save.Checked)
            {
                string imageName = Path.GetFileName(path);
                image.Save(imageName + ".bmp");
            }
        }

        public Point[] GetTabPoints(string file, int x, int y, int xmod = 1, int ymod = 1)
        {
            if (!File.Exists(file))
            {
                //We cannot find the file, so return empty.
                Console.WriteLine(file + " does not exist!");
                return new Point[0];
            }

            //Prepare the points
            List<Point> points = new List<Point>();

            //Read the file lines
            string[] lines = File.ReadAllLines(file);

            //prepare the previous tab. This is used for when we come accross a empty line.
            int preTabs = 0;

            //Itterate through the lines
            for (int i = 0; i < lines.Length; i++)
            {
                //Get the line
                string ln = lines[i];

                if (ln == "")
                {
                    //if the line is empty, use the previous one and continue
                    string previousLine = i - 1 > 0 ? lines[i - 1] : "";
                    string nextLine = i + 1 < lines.Length ? lines[i+1] : "";

                    if (previousLine != "" && nextLine != "")
                        points.Add(new Point(x + ((preTabs + 1) * xmod), y + (i * ymod)));
                    else
                        points.Add(new Point(x + (preTabs * xmod), y + (i * ymod)));


                    continue;
                }

                //Count how many tabs there are
                int tabs = GetLeadingWhitespaceLength(ln);

                /*
                Console.WriteLine();
                Console.WriteLine("Line: " + ln);
                Console.WriteLine("Number of Tabs: " + tabs);
                Console.WriteLine();
                */

                //Has the tab count changed?
                if (tabs != preTabs)
                {
                    //We must step from previous tabs to current tabs
                    points.Add(new Point(x + preTabs * xmod, y + i * ymod));
                }

                //Add it to the point list
                points.Add(new Point(x + tabs * xmod, y + i * ymod));

                //Store it as the previous points
                preTabs = tabs;
            }

            points.Add(new Point(x + preTabs * xmod, y + lines.Length * ymod));
            points.Add(new Point(x , y + lines.Length * ymod));

            //return the point list
            return points.ToArray();
        }

        public int GetLeadingWhitespaceLength(string s, int tabLength = 4, bool trimToLowerTab = true)
        {
            if (s.Length < tabLength) return 0;

            int whiteSpaceCount = 0;

            while (whiteSpaceCount < s.Length && Char.IsWhiteSpace(s[whiteSpaceCount])) whiteSpaceCount++;

            if (whiteSpaceCount < tabLength) return 0;

            if (trimToLowerTab)
            {
                whiteSpaceCount -= whiteSpaceCount % tabLength;
            }

            return whiteSpaceCount;
        }

        private void btn_browse_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            textbox_path.Text = openFileDialog.FileName;
        }

        private void btn_generate_Click(object sender, EventArgs e)
        {

            if (!File.Exists(textbox_path.Text))
            {
                MessageBox.Show("The path '" + textbox_path.Text + "' does not exist!", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int lineHeight = 10;
            int tabSize = 5;

            Color bkgColor = Color.FromArgb(34, 34, 34);
            Color polyColor = Color.White;

            GenerateImage(textbox_path.Text, lineHeight, tabSize, bkgColor, polyColor);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }
    }
}

