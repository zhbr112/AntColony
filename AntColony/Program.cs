using System.Diagnostics;

double[][] distances = [
            [0,3,0,0,1,0],
            [3,0,8,0,0,3],
            [0,3,0,1,0,1],
            [0,0,8,0,1,0],
            [3,0,0,3,0,0],
            [3,3,3,5,4,0],
            ];

//double[][] distances;
//using (StreamReader r = new StreamReader("graph1000.json"))
//{
//    string json = r.ReadToEnd();
//    distances = JsonSerializer.Deserialize<double[][]>(json);
//}


Stopwatch stopWatch = new Stopwatch();
stopWatch.Start();

var aco = new AntColony(distances, _antCount: 1000, _iterations: 10);
var bestTour = aco.Solve();

stopWatch.Stop();
TimeSpan ts = stopWatch.Elapsed;
string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
    ts.Hours, ts.Minutes, ts.Seconds,
    ts.Milliseconds / 10);
Console.WriteLine("RunTime " + elapsedTime);


Console.WriteLine("Best Tour:");
Console.WriteLine(string.Join(" -> ", bestTour));
Console.WriteLine($"Tour Length: {aco.BestTourLength}");


double c = 0;
for (int i = 0; i < bestTour.Count - 1; i++)
{
    c += distances[bestTour[i]][bestTour[i + 1]];
}
Console.WriteLine(c);
