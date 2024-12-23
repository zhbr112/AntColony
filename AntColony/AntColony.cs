using System.Collections.Concurrent;
using System.Text;

public class AntColony
{
    List<(int AntCount, double Alpha, double Beta)> param;
    private double rho;
    private int iterations;
    private double[][] distanceMatrix;
    private double[][] pheromoneMatrix;
    private int numCities;

    public List<int>? BestTour { get; private set; }
    public double BestTourLength { get; private set; }

    public AntColony(
        double[][] _distanceMatrix,
        List<(int, double, double)> _param,
        int _iterations = 1000,
        double _rho = 0.5
        )
    {
        param = _param;
        distanceMatrix = _distanceMatrix;
        numCities = _distanceMatrix.Length;
        iterations = _iterations;
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
        Lock An =new();
        var q = new StringBuilder();
        var antCount = param.Sum(x => x.AntCount);
        for (int iteration = 0; iteration < iterations; iteration++)
        {
            var ants = new ConcurrentBag<Ant>();

            var antC = 0;
            var paramN = 0;
            Parallel.For(0, antCount, i =>
            {

                var ant = new Ant(distanceMatrix, pheromoneMatrix, param[paramN].Alpha, param[paramN].Beta);
                lock(An)
                {
                    antC++;
                    if (antC > param[paramN].AntCount)
                    {
                        antC = 0;
                        paramN++;
                    }
                }
              

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
            q.Append($"{iteration} {BestTourLength}\n");           
            UpdatePheromones(ants);
        }
        File.AppendAllText("q.txt", q.ToString());
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
