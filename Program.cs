using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        int populationSize = 50;
        int generations = 100;
        List<City> cities = InitializeCities();
        Population pop = new Population(populationSize, cities);

        for (int g = 0; g < generations; g++)
        {
            List<List<City>> newRoutes = new List<List<City>>();
            while (newRoutes.Count < populationSize)
            {
                List<City> parent1 = pop.SelectByRoulette();
                List<City> parent2 = pop.SelectByRoulette();
                List<City> child = Crossover(parent1, parent2); // Implement your crossover method
                Mutate(child); // Implement your mutation method
                newRoutes.Add(child);
            }

            pop.Routes = newRoutes;
            Console.WriteLine($"Generation {g + 1}: Best Distance = {1 / pop.GetBestFitness()}");
        }

        Console.WriteLine("Finished!");
    }

    public static List<City> InitializeCities()
    {
        return new List<City>
        {
            new City(0, 0),
            new City(1, 2),
            new City(3, 5),
            new City(6, 1),
            new City(4, 4)
        };
    }

    public static List<City> Crossover(List<City> parent1, List<City> parent2)
    {
        var child = new List<City>();
        
        child.AddRange(parent1.Take(3));
        child.AddRange(parent2.Where(city => !child.Contains(city)));
        
        return child;
    }

    public static void Mutate(List<City> route)
    {
        Random rand = new Random();
        int pos1 = rand.Next(route.Count);
        int pos2 = rand.Next(route.Count);
        City temp = route[pos1];
        route[pos1] = route[pos2];
        route[pos2] = temp;
    }
}

public class City
{
    public double X { get; set; }
    public double Y { get; set; }

    public City(double x, double y)
    {
        X = x;
        Y = y;
    }

    public static double CalculateDistance(City a, City b)
    {
        return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
    }
}

public class Population
{
    public List<List<City>> Routes { get; set; }

    public Population(int size, List<City> cities)
    {
        Routes = new List<List<City>>();
        for (int i = 0; i < size; i++)
        {
            var route = new List<City>(cities);
            Shuffle(route);
            Routes.Add(route);
        }
    }

    private static void Shuffle(List<City> list)
    {
        Random rand = new Random();
        int n = list.Count;
        while (n > 1)
        {
            int k = rand.Next(n--);
            City temp = list[n];
            list[n] = list[k];
            list[k] = temp;
        }
    }

    public List<City> SelectByRoulette()
    {
        double totalFitness = 0;
        List<double> cumulativeFitness = new List<double>();

        foreach (var route in Routes)
        {
            totalFitness += Fitness(route);
            cumulativeFitness.Add(totalFitness);
        }

        Random rand = new Random();
        double spin = rand.NextDouble() * totalFitness;

        for (int i = 0; i < cumulativeFitness.Count; i++)
        {
            if (spin <= cumulativeFitness[i])
            {
                return new List<City>(Routes[i]);
            }
        }

        return null;
    }

    public double GetBestFitness()
    {
        double bestFitness = 0;
        foreach (var route in Routes)
        {
            double fitness = Fitness(route);
            if (fitness > bestFitness)
            {
                bestFitness = fitness;
            }
        }

        return bestFitness;
    }

    private static double Fitness(List<City> route)
    {
        double totalDistance = 0;
        for (int i = 0; i < route.Count - 1; i++)
        {
            totalDistance += City.CalculateDistance(route[i], route[i + 1]);
        }
        totalDistance += City.CalculateDistance(route[route.Count - 1], route[0]); // Return to start
        return 1 / totalDistance; // Inverse of total distance
    }
}
