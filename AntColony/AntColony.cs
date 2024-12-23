using System.Collections.Concurrent;

public class AntColony
{
    private double alpha;
    private double beta;
    private double rho;
    private int antCount;
    private int iterations;

    private double[][] distanceMatrix;
    private double[][] pheromoneMatrix;
    private int numCities;

    public List<int> BestTour { get; private set; }
    public double BestTourLength { get; private set; }

    public AntColony(
        double[][] _distanceMatrix,
        int _antCount = 10,
        int _iterations = 1000,
        double _alpha = 1.0,
        double _beta = 1.0,
        double _rho = 0.5)
    {
        distanceMatrix = _distanceMatrix;
        numCities = _distanceMatrix.Length;
        antCount = _antCount;
        iterations = _iterations;
        alpha = _alpha;
        beta = _beta;
        rho = _rho;

        pheromoneMatrix = new double[numCities][];
        double initialPheromone = 10.0 / numCities;
        for (int i = 0; i < numCities; i++)
        {
            pheromoneMatrix[i] = new double[numCities];
            for (int j = 0; j < numCities; j++)
            {
                pheromoneMatrix[i][j] = initialPheromone;
            }
        }

        BestTourLength = double.MaxValue;
        BestTour = null;
    }


    public List<int> Solve()
    {
        Lock Best = new();

        for (int iteration = 0; iteration < iterations; iteration++)
        {
            var ants = new ConcurrentBag<Ant>();
            Parallel.For(0, antCount, (i, state) =>
            {
                var ant = new Ant(distanceMatrix, pheromoneMatrix, alpha, beta);

                while (ant.Tour.Count < numCities)
                {
                    if (ant.SelectNextCity() == -1) return;
                }
                ant.CompleteTour();

                if (ant.Tour.Count < numCities + 1) return;

                ants.Add(ant);
                //Console.WriteLine(ant.TourLength);
                //Console.WriteLine(string.Join(" -> ", ant.Tour));
                lock (Best)
                    if (ant.TourLength < BestTourLength)
                    {
                        BestTourLength = ant.TourLength;
                        BestTour = new List<int>(ant.Tour);
                    }
            });

            UpdatePheromones(ants);
        }

        return BestTour;
    }

    private void UpdatePheromones(ConcurrentBag<Ant> ants)
    {
        for (int i = 0; i < numCities; i++)
        {
            for (int j = 0; j < numCities; j++)
            {
                pheromoneMatrix[i][j] *= rho;
            }
        }

        foreach (var ant in ants)
        {
            double pheromoneDeposit = 10.0 / ant.TourLength;

            for (int i = 0; i < ant.Tour.Count - 1; i++)
            {
                pheromoneMatrix[ant.Tour[i]][ant.Tour[i + 1]] += pheromoneDeposit;
            }
        }
    }
}
