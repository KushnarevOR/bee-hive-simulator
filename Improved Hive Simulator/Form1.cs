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
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Drawing.Printing;

namespace Improved_Hive_Simulator
{
    public partial class Form1 : Form
    {
        private HiveForm hiveForm = new HiveForm();
        private FieldForm fieldForm = new FieldForm();
        private ControlPanel panelForm;
        private Renderer renderer;
        private World world;
        private Random random = new Random();
        private DateTime start = DateTime.Now;
        private DateTime end;
        private int framesRun = 0;

        public Form1()
        {
            InitializeComponent();         
            MoveChildForms();
            hiveForm.Show(this);
            fieldForm.Show(this);
            ResetSimulator();
            panelForm = new ControlPanel(world);

            timer1.Interval = 50;
            timer1.Tick += new EventHandler(RunFrame);
            timer1.Enabled = false;
            frameRate_label.Text = "N/A";
            UpdateStats(new TimeSpan());
        }

        private void MoveChildForms()
        {
            hiveForm.Location = new Point(Location.X + Width + 10, Location.Y);
            fieldForm.Location = new Point(Location.X, Location.Y + Math.Max(Height, hiveForm.Height) + 10);
        }

        private void UpdateStats(TimeSpan frameDuration)
        {
            bees_label.Text = world.Bees.Count.ToString();
            flowers_label.Text = world.Flowers.Count.ToString();
            honeyInHive_label.Text = String.Format("{0:f3}", world.Hive.Honey);
            double nectar = 0;
            foreach (Flower flower in world.Flowers)
                nectar += flower.Nectar;
            nectarInFlowers_label.Text = String.Format("{0:f3}", nectar);
            frameRun_label.Text = framesRun.ToString();
            double milliSeconds = frameDuration.TotalMilliseconds;
            if (milliSeconds != 0.0)
                frameRate_label.Text = string.Format("{0:f0}, ({1:f3}ms)", 1000 / milliSeconds, milliSeconds);
        }

        private void RunFrame(object sender, EventArgs e)
        {
            framesRun++;
            world.Go(random);
            end = DateTime.Now;
            TimeSpan frameDuration = end - start;
            start = end;
            UpdateStats(frameDuration);
            hiveForm.Invalidate();
            fieldForm.Invalidate();
        }

        private void startSimulation_Click(object sender, EventArgs e)
        {
            simulationControl();
        }

        private void simulationControl()
        {
            if (timer1.Enabled)
            {
                toolStrip1.Items[0].Text = "Resume Simulation";
                timer1.Stop();
            }
            else
            {
                toolStrip1.Items[0].Text = "Pause Simulation";
                timer1.Start();
            }
        }

        private void reset_Click(object sender, EventArgs e)
        {
            ResetSimulator();
            if (!timer1.Enabled)
                toolStrip1.Items[0].Text = "Start Simulation";
        }

        private void SendMessage(int ID, string Message)
        {
            statusStrip1.Items[0].Text = "Bee #" + ID + ": " + Message;
            var beeGroups =
                from bee in world.Bees
                group bee by bee.CurrentState into beeGroup
                orderby beeGroup.Key
                select beeGroup;
            listBox1.Items.Clear();
            foreach(var group in beeGroups)
            {
                string s;
                if (group.Count() == 1)
                    s = "";
                else
                    s = "s";
                listBox1.Items.Add(group.Key.ToString() + ": " + group.Count() + " bee" + s);
                if(group.Key == BeeState.Idle && group.Count() == world.Bees.Count() && framesRun > 0)
                {
                    listBox1.Items.Add("Simulation ended: all bees are idle");
                    toolStrip1.Items[0].Text = "Simulation ended";
                    statusStrip1.Items[0].Text = "Simulation ended";
                    timer1.Enabled = false;
                }
            }
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            World currentWorld = world;
            int currentFramesRun = framesRun;

            if (timer1.Enabled)
                timer1.Stop();

            openFileDialog1.InitialDirectory = @"C:\Users\Olga\Desktop\Hive folder";
            openFileDialog1.Filter = "Objects (*.world)|*.world|All files (*.*)|*.*";
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.Title = "Choose a file with simulation to load";
            DialogResult result = openFileDialog1.ShowDialog();
            if(result == DialogResult.OK)
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (Stream input = File.OpenRead(openFileDialog1.FileName))
                {
                    try
                    {
                        world = (World)bf.Deserialize(input);
                        framesRun = (int)bf.Deserialize(input);
                    }
                    catch(SerializationException ex)
                    {
                        MessageBox.Show("An error occured while opening the excuse '" + openFileDialog1.FileName + "'\n" + ex.Message, "Unable to open the excuse", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        world = currentWorld;
                        framesRun = currentFramesRun;
                    }
                }
            }

            world.Hive.informacioCallback = new Informacio(SendMessage);
            foreach (Bee bee in world.Bees)
                bee.informacioCallback = new Informacio(SendMessage);
            if (toolStrip1.Items[0].Text != "Start Simulation")
                simulationControl();

            renderer = new Renderer(world, hiveForm, fieldForm);
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {   
            if (timer1.Enabled)
                timer1.Stop();
            saveFileDialog1.InitialDirectory = @"C:\Users\Olga\Desktop\Hive folder";
            saveFileDialog1.Filter = "Objects (*.world)|*.world|All files (*.*)|*.*";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.Title = "Choose a file to save the current simulation";
            DialogResult result = saveFileDialog1.ShowDialog();
            if(result == DialogResult.OK)
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (Stream output = File.OpenWrite(saveFileDialog1.FileName))
                {
                    bf.Serialize(output, world);
                    bf.Serialize(output, framesRun);
                }
            }
            if(toolStrip1.Items[0].Text != "Start Simulation")
                simulationControl();
        }

        private void printToolStripButton_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
                timer1.Stop();

            PrintDocument document = new PrintDocument();
            document.PrintPage += new PrintPageEventHandler(document_PrintPage);
            PrintPreviewDialog preview = new PrintPreviewDialog();
            preview.Document = document;
            preview.ShowDialog(this);
            if (toolStrip1.Items[0].Text != "Start Simulation")
                simulationControl();
        }

        private void document_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Size stringSize;
            using (Font arial24Bold = new Font("Arial", 24, FontStyle.Bold))
            {
                stringSize = Size.Ceiling(g.MeasureString("Bee Simulator", arial24Bold));
                g.FillEllipse(Brushes.Gray,
                    new Rectangle(e.MarginBounds.X + 2, e.MarginBounds.Y + 2,
                    stringSize.Width + 30, stringSize.Height + 30));
                g.FillEllipse(Brushes.Black,
                    new Rectangle(e.MarginBounds.X, e.MarginBounds.Y,
                    stringSize.Width + 30, stringSize.Height + 30));
                g.DrawString("Bee Simulator", arial24Bold,
                    Brushes.Gray, e.MarginBounds.X + 17, e.MarginBounds.Y + 17);
                g.DrawString("Bee Simulator", arial24Bold,
                    Brushes.White, e.MarginBounds.X + 15, e.MarginBounds.Y + 15);
            }

            int tableX = e.MarginBounds.X + (int)stringSize.Width + 50;
            int tableWidth = e.MarginBounds.X + e.MarginBounds.Width - tableX - 20;
            int firstColumnX = tableX + 2;
            int secondColumnX = tableX + (tableWidth / 2) + 5;
            int tableY = e.MarginBounds.Y;
            tableY = PrintTableRow(e.Graphics, tableX, tableWidth, firstColumnX, secondColumnX, tableY, "Bees", bees_label.Text);
            tableY = PrintTableRow(e.Graphics, tableX, tableWidth, firstColumnX, secondColumnX, tableY, "Flowers", flowers_label.Text);
            tableY = PrintTableRow(e.Graphics, tableX, tableWidth, firstColumnX, secondColumnX, tableY, "Honey in Hive", honeyInHive_label.Text);
            tableY = PrintTableRow(e.Graphics, tableX, tableWidth, firstColumnX, secondColumnX, tableY, "Nectar in Flowers", nectarInFlowers_label.Text);
            tableY = PrintTableRow(e.Graphics, tableX, tableWidth, firstColumnX, secondColumnX, tableY, "Frames Run", frameRun_label.Text);
            tableY = PrintTableRow(e.Graphics, tableX, tableWidth, firstColumnX, secondColumnX, tableY, "Frame Rate", frameRate_label.Text);

            g.DrawRectangle(Pens.Black, tableX, e.MarginBounds.Y, tableWidth, tableY - e.MarginBounds.Y);
            g.DrawLine(Pens.Black, secondColumnX, e.MarginBounds.Y, secondColumnX, tableY);

            using (Pen blackPen = new Pen(Brushes.Black, 2))
            using (Bitmap hiveBitmap = new Bitmap(hiveForm.ClientSize.Width, hiveForm.ClientSize.Height))
            using (Bitmap fieldBitmap = new Bitmap(fieldForm.ClientSize.Width, fieldForm.ClientSize.Height))
            {
                using (Graphics hiveGraphics = Graphics.FromImage(hiveBitmap))
                {
                    renderer.PaintHive(hiveGraphics);
                }
                int hiveWidth = e.MarginBounds.Width / 2;
                float ratio = (float)hiveBitmap.Height / hiveBitmap.Width;
                int hiveHeight = (int)(hiveWidth * ratio);
                int hiveX = e.MarginBounds.X + (e.MarginBounds.Width - hiveWidth) / 2;
                int hiveY = e.MarginBounds.Height / 3;
                g.DrawImage(hiveBitmap, hiveX, hiveY, hiveWidth, hiveHeight);
                g.DrawRectangle(blackPen, hiveX, hiveY, hiveWidth, hiveHeight);

                using (Graphics fieldGraphics = Graphics.FromImage(fieldBitmap))
                {
                    renderer.PaintField(fieldGraphics);
                }
                int fieldWidth = e.MarginBounds.Width;
                ratio = (float)fieldForm.Height / fieldForm.Width;
                int fieldHeight = (int)(fieldWidth * ratio);
                int fieldX = e.MarginBounds.X + (e.MarginBounds.Width - fieldWidth) / 2;
                int fieldY = e.MarginBounds.Y + e.MarginBounds.Height - fieldHeight;
                g.DrawImage(fieldBitmap, fieldX, fieldY, fieldWidth, fieldHeight);
                g.DrawRectangle(blackPen, fieldX, fieldY, fieldWidth, fieldHeight);
            }
        }

        private int PrintTableRow(Graphics printGraphics, int tableX,
            int tableWidth, int firstColumnX, int secondColumnX,
            int tableY, string firstColumn, string SecondColumn)
        {
            Font arial12 = new Font("Arial", 12);
            Size stringSize = Size.Ceiling(printGraphics.MeasureString(firstColumn, arial12));
            tableY += 2;
            printGraphics.DrawString(firstColumn, arial12, Brushes.Black,
                firstColumnX, tableY);
            printGraphics.DrawString(SecondColumn, arial12, Brushes.Black,
                secondColumnX, tableY);
            tableY += (int)stringSize.Height + 2;
            printGraphics.DrawLine(Pens.Black, tableX, tableY, tableX + tableWidth, tableY);
            arial12.Dispose();
            return tableY;
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            MoveChildForms();
        }

        private void ResetSimulator()
        {
            framesRun = 0;
            frameRate_label.Text = "N/A";
            world = new World(new Informacio(SendMessage));
            renderer = new Renderer(world, hiveForm, fieldForm);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            renderer.AnimateBees();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
                timer1.Stop();
            panelForm.ShowDialog();
        }
    }
}
