using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Improved_Hive_Simulator
{
    public class Renderer
    {
        private World world;
        private HiveForm hiveForm;
        private FieldForm fieldForm;
        private Bitmap HiveInside;
        private Bitmap HiveOutside;
        private Bitmap Flower;

        private Bitmap[] BeeAnimationLarge;
        private Bitmap[] BeeAnimationSmall;

        public Renderer(World world, HiveForm hiveForm, FieldForm fieldForm)
        {
            this.world = world;
            this.hiveForm = hiveForm;
            this.fieldForm = fieldForm;
            hiveForm.Renderer = this;
            fieldForm.Renderer = this;
            InitializeImages();
        }

        public void InitializeImages()
        {
            HiveInside = ResizeImage(Properties.Resources.Hive_big_, hiveForm.ClientRectangle.Width, hiveForm.ClientRectangle.Height);
            HiveOutside = ResizeImage(Properties.Resources.Hive, 85, 100);
            Flower = ResizeImage(Properties.Resources.Flower, 75, 75);
            BeeAnimationLarge = new Bitmap[]
            {
                ResizeImage(Properties.Resources.Bee_animation_1, 40, 40),
                ResizeImage(Properties.Resources.Bee_animation_2, 40, 40),
                ResizeImage(Properties.Resources.Bee_animation_3, 40, 40),
                ResizeImage(Properties.Resources.Bee_animation_4, 40, 40),
            };
            BeeAnimationSmall = new Bitmap[]
            {
                ResizeImage(Properties.Resources.Bee_animation_1, 20, 20),
                ResizeImage(Properties.Resources.Bee_animation_2, 20, 20),
                ResizeImage(Properties.Resources.Bee_animation_3, 20, 20),
                ResizeImage(Properties.Resources.Bee_animation_4, 20, 20),
            };
        }

        public void PaintHive(Graphics g)
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.FillRectangle(Brushes.LightSkyBlue, hiveForm.ClientRectangle);
            g.DrawImageUnscaled(HiveInside, 0, 0);
            foreach (Bee bee in world.Bees)
                if (bee.InsideHive)
                    g.DrawImageUnscaled(BeeAnimationLarge[cell], bee.Location.X, bee.Location.Y);
        }

        public void PaintField(Graphics g)
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            using (Pen brownPen = new Pen(Color.Brown, 6.0F))
            {
                g.FillRectangle(Brushes.SkyBlue, 0, 0,
                    fieldForm.ClientSize.Width, fieldForm.ClientSize.Height / 2);
                g.FillEllipse(Brushes.Yellow, new RectangleF(50, 15, 70, 70));
                g.FillRectangle(Brushes.Green, 0, fieldForm.ClientSize.Height / 2,
                    fieldForm.ClientSize.Width, fieldForm.ClientSize.Height / 2);
                g.DrawLine(brownPen, new Point(693, 0), new Point(693, 30));
                g.DrawImageUnscaled(HiveOutside, 650, 20);
                foreach(Flower flower in world.Flowers)
                {
                    g.DrawImageUnscaled(Flower, flower.Location.X, flower.Location.Y);
                }
                foreach(Bee bee in world.Bees)
                {
                    if (!bee.InsideHive)
                        g.DrawImageUnscaled(BeeAnimationSmall[cell],
                            bee.Location.X, bee.Location.Y);
                }
            }
        }

        public static Bitmap ResizeImage(Bitmap picture, int width, int height)
        {
            Bitmap resizedPicture = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(resizedPicture))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(picture, 0, 0, width, height);
            }
            return resizedPicture;
        }

        private int cell = 0;
        private int frame = 0;
        public void AnimateBees()
        {
            frame++;
            if (frame >= 6)
                frame = 0;
            switch (frame)
            {
                case 0: cell = 0; break;
                case 1: cell = 1; break;
                case 2: cell = 2; break;
                case 3: cell = 3; break;
                case 4: cell = 2; break;
                case 5: cell = 1; break;
                default: cell = 0; break;
            }
            hiveForm.Invalidate();
            fieldForm.Invalidate();
        }
    }
}
