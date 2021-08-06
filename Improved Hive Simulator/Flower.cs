using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Improved_Hive_Simulator
{
    [Serializable]
    public class Flower
    {
        private const int LifeSpanMin = 15000;//minimum flower lifespan
        private const int LifeSpanMax = 30000;//maximum flower lifespan
        private const double InitialNectar = 1.5;//initial nectar amount
        private const double MaxNectar = 5.0;//max nectar amount is gathered from flower
        private const double NectarAddedPerTurn = 0.01;//nectar amount is added to flower per turn
        private const double NectarGatheredPerTurn = 0.3;//nectar amount is gathered from flower per turn

        public Point Location { get; private set; }
        public int Age { get; private set; }
        public bool Alive { get; private set; }
        public double Nectar { get; private set; }
        public double NectarHarvested { get; set; }
        private int lifespan;

        public Flower(Point location, Random random)
        {
            this.Location = location;
            this.Age = 0;
            this.Alive = true;
            this.Nectar = InitialNectar;
            this.lifespan = random.Next(LifeSpanMin, LifeSpanMax + 1);
        }

        public double HarvestNectar()
        {
            if (NectarGatheredPerTurn > Nectar)
                return 0;
            else
            {
                NectarHarvested += NectarGatheredPerTurn;
                Nectar -= NectarGatheredPerTurn;
                return NectarGatheredPerTurn;
            }
        }

        public void Go()
        {
            if (Age == lifespan)
                Alive = false;
            else
            {
                Nectar += NectarAddedPerTurn;
                if (Nectar > MaxNectar)
                    Nectar = MaxNectar;
            }
        }
    }
}
