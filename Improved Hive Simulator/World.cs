using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Improved_Hive_Simulator
{
    [Serializable]
    public class World
    {
        private double nectarHarvestedPerNewFlower = 50.0;
        public double NectarHarvestedPerNewFlower { get { return nectarHarvestedPerNewFlower; } set { nectarHarvestedPerNewFlower = value; } }
        private int fieldMinX = 15;
        public int FieldMinX { get { return fieldMinX; } set { fieldMinX = value; } }
        private int fieldMinY = 177;
        public int FieldMinY { get { return fieldMinY; } set { fieldMinY = value; } }
        private int fieldMaxX = 690;
        public int FieldMaxX { get { return fieldMaxX; } set { fieldMaxX = value; } }
        private int fieldMaxY = 290;
        public int FieldMaxY { get { return fieldMaxY; } set { fieldMaxY = value; } }

        private Hive hive;
        public Hive Hive { get { return hive; } }

        private List<Bee> bees;
        public List<Bee> Bees { get { return bees; } }
        
        private List<Flower> flowers;
        public List<Flower> Flowers { get { return flowers; } }

        public World(Informacio informacioCallback)
        {
            bees = new List<Bee>();
            flowers = new List<Flower>();
            hive = new Hive(this, informacioCallback);
            Random random = new Random();
            for (int i = 0; i < 10; i++)
                AddFlower(random);
        }

        public void Go(Random random)
        {
            Hive.Go(random);

            for(int i = Bees.Count - 1; i >= 0; i--)
            {
                Bee bee = Bees[i];
                bee.Go(random);
                if (bee.CurrentState == BeeState.Retired)
                    Bees.Remove(bee);
            }

            double totalNectarHarvested = 0;
            for(int i = Flowers.Count - 1; i >= 0; i--)
            {
                Flower flower = Flowers[i];
                flower.Go();
                totalNectarHarvested += flower.NectarHarvested;
                if (!flower.Alive)
                    Flowers.Remove(flower);
            }

            if(totalNectarHarvested > NectarHarvestedPerNewFlower)
            {
                foreach (Flower flower in Flowers)
                    flower.NectarHarvested = 0;
                AddFlower(random);
            }
        }       

        private void AddFlower(Random random)
        {
            Point location = new Point(random.Next(FieldMinX, FieldMaxX),
                                       random.Next(FieldMinY, FieldMaxY));
            Flower newFlower = new Flower(location, random);
            Flowers.Add(newFlower);
        }
    }
}
