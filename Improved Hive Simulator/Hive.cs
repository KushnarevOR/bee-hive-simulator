using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Improved_Hive_Simulator
{
    [Serializable]
    public class Hive
    {
        private int initialBees = 6;
        public int InitialBees { get { return initialBees; } set { initialBees = value; } }
        private double initialHoney = 3.2;
        public double InitialHoney { get { return initialHoney; } set { initialHoney = value; } }
        private double maximumHoney = 15.0;
        public double MaximumHoney { get { return maximumHoney; } set { maximumHoney = value; } }
        private double nectarHoneyRatio = .25;
        public double NectarHoneyRatio { get { return nectarHoneyRatio; } set { nectarHoneyRatio = value; } }
        private int maximumBees = 15;
        public int MaximumBees { get { return maximumBees; } set { maximumBees = value; } }
        private double minimumHoneyForCreatingBees = 4.0;
        public double MinimumHoneyForCreatingBees { get { return minimumHoneyForCreatingBees; } set { minimumHoneyForCreatingBees = value; } }

        public double Honey { get; private set; }
        private Dictionary<string, Point> locations;
        private int beeCount = 0;
        private World world;

        [NonSerialized]
        public Informacio informacioCallback;

        public Hive(World world, Informacio informacioCallback)
        {
            this.world = world;
            this.Honey = InitialHoney;
            InitializeLocations();
            Random random = new Random();
            this.informacioCallback = informacioCallback;
            for (int i = 0; i < InitialBees; i++)
                AddBee(random);
        }

        private void InitializeLocations()
        {
            locations = new Dictionary<string, Point>()
            {
                {"Entrance", new Point(706, 99) },
                {"Nursery", new Point(144, 250) },
                {"HoneyFactory", new Point(278, 148) },
                {"Exit", new Point(300, 280) }
            };
        }

        public bool AddHoney(double Nectar)
        {
            double honeyToAdd = Nectar * NectarHoneyRatio;
            if (honeyToAdd + Honey > MaximumHoney)
                return false;
            Honey += honeyToAdd;
            return true;
        }

        public bool ConsumeHoney(double amount)
        {
            if (amount > Honey)
                return false;
            else
            {
                Honey -= amount;
                return true;
            }
        }

        public void AddBee(Random random)
        {
            beeCount++;
            int r1 = random.Next(100) - 50;
            int r2 = random.Next(100) - 50;
            Point startPoint = new Point(locations["Nursery"].X + r1,
                                         locations["Nursery"].Y + r2);
            Bee newBee = new Bee(beeCount, startPoint, this, world);
            newBee.informacioCallback += this.informacioCallback;
            world.Bees.Add(newBee);
        }

        public void Go(Random random)
        {
            if (world.Bees.Count < MaximumBees && Honey > MinimumHoneyForCreatingBees && random.Next(10) == 1)
                AddBee(random);
        }

        public Point GetLocation(string location)
        {
            if (locations.ContainsKey(location))
                return locations[location];
            else
                throw new ArgumentException("Unknown location: " + location);
        }
    }
}
