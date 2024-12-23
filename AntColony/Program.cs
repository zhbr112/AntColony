using System.Diagnostics;
using System.Text.Json;

//double[][] distances = [
//            [0,3,0,0,1,0],
//            [3,0,8,0,0,3],
//            [0,3,0,1,0,1],
//            [0,0,8,0,1,0],
//            [3,0,0,3,0,0],
//            [3,3,3,5,4,0],
//            ];

double[][] distances;
using (StreamReader r = new StreamReader("graph1000.json"))
{
    string json = r.ReadToEnd();
    distances = JsonSerializer.Deserialize<double[][]>(json)??[[0]];
}


Stopwatch stopWatch = new Stopwatch();
stopWatch.Start();

var aco = new AntColony(distances, [(500,1,1), (500, 2, 1), (500, 1, 2)], _iterations: 50, _rho: 0.5);
var bestTour = aco.Solve();

stopWatch.Stop();
TimeSpan ts = stopWatch.Elapsed;
Console.WriteLine("RunTime " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10));


Console.WriteLine("Best Tour:");
Console.WriteLine(string.Join(" -> ", bestTour));
Console.WriteLine($"Tour Length: {aco.BestTourLength}");