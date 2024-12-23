public class Ant
{
    private double alpha;
    private double beta;
    double[][] distanceMatrix;
    double[][] pheromoneMatrix;

    public List<int> Tour { get; private set; }
    public bool[] Visited { get; private set; }
    public double TourLength { get; private set; }
    private Random Rnd;

    public Ant(double[][] _distanceMatrix, double[][] _pheromoneMatrix, double _alpha, double _beta)
    {
        distanceMatrix = _distanceMatrix;
        pheromoneMatrix = _pheromoneMatrix;
        alpha = _alpha;
        beta = _beta;

        Rnd = new Random();

        Tour = new List<int>();
        TourLength = 0;

        Visited = new bool[distanceMatrix.Length];
        Array.Fill(Visited, false);

        int startCity = Rnd.Next(distanceMatrix.Length);
        Tour.Add(startCity);
        Visited[startCity] = true;
    }

    public int SelectNextCity()
    {
        int currentCity = Tour[^1];
        int numCities = distanceMatrix.Length;

        var probabilities = new List<(int City, double Probability)>();
        double totalProbability = 0;

        for (int city = 0; city < numCities; city++)
        {
            if (distanceMatrix[currentCity][city] == 0) continue;
            if (!Visited[city])
            {
                double probability = Math.Pow(pheromoneMatrix[currentCity][city], alpha) * Math.Pow(1.0 / distanceMatrix[currentCity][city], beta);

                probabilities.Add((city, probability));
                totalProbability += probability;
            }
        }

        var normalizedProbabilities = probabilities.Select(p => (p.City, p.Probability / totalProbability)).ToList();

        double randomValue = Rnd.NextDouble();
        double cumulativeProbability = 0;

        foreach (var (city, probability) in normalizedProbabilities)
        {
            cumulativeProbability += probability;
            if (cumulativeProbability >= randomValue)
            {
                Tour.Add(city);
                Visited[city] = true;
                return city;
            }
        }

        return -1;
    }

    public void CompleteTour()
    {
        int startCity = Tour[0];
        int lastCity = Tour[Tour.Count - 1];
        if (distanceMatrix[lastCity][startCity] == 0) return;

        Tour.Add(startCity);

        for (int i = 0; i < Tour.Count - 1; i++)
        {
            TourLength += distanceMatrix[Tour[i]][Tour[i + 1]];
        }
    }
}