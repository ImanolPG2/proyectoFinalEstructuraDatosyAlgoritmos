using System;
using System.Collections.Generic;
using System.Threading;

// Enumeraciones
public enum VehicleType { Car, Truck, Motorcycle }
public enum Direction { North, South, East, West }

// Clases
public class Vehicle
{
    public int Id { get; set; }
    public VehicleType Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public Direction Direction { get; set; }
    public int Speed { get; set; }

    // Constructor del vehículo
    public Vehicle(int id, VehicleType type, int x, int y, Direction direction)
    {
        Id = id;
        Type = type;
        X = x;
        Y = y;
        Direction = direction;
        Speed = new Random().Next(1, 4); // Velocidad aleatoria entre 1 y 3
    }

    // Método para mover el vehículo
    public void Move()
    {
        switch (Direction)
        {
            case Direction.North:
                Y -= Speed;
                break;
            case Direction.South:
                Y += Speed;
                break;
            case Direction.East:
                X += Speed;
                break;
            case Direction.West:
                X -= Speed;
                break;
        }
    }
}

public class Intersection
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsGreenLight { get; set; }

    // Constructor de la intersección
    public Intersection(int x, int y)
    {
        X = x;
        Y = y;
        IsGreenLight = true; // Inicialmente, el semáforo está en verde
    }

    // Método para cambiar el estado del semáforo
    public void ChangeLight()
    {
        IsGreenLight = !IsGreenLight;
    }
}

public class City
{
    public int Width { get; set; }
    public int Height { get; set; }
    public List<Vehicle> Vehicles { get; set; }
    public List<Intersection> Intersections { get; set; }
    private char[,] baseGrid;

    // Constructor de la ciudad
    public City(int width, int height)
    {
        Width = width;
        Height = height;
        Vehicles = new List<Vehicle>();
        Intersections = new List<Intersection>();
        InitializeBaseGrid();
        InitializeIntersections();
    }

    // Inicializa la cuadrícula base de la ciudad
    private void InitializeBaseGrid()
    {
        baseGrid = new char[Height, Width];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (y % 5 == 0 || x % 10 == 0)
                {
                    baseGrid[y, x] = '·'; // Representa las calles
                }
                else
                {
                    baseGrid[y, x] = ' '; // Espacio vacío
                }
            }
        }
    }

    // Inicializa las intersecciones en la ciudad
    private void InitializeIntersections()
    {
        for (int x = 10; x < Width; x += 10)
        {
            for (int y = 5; y < Height; y += 5)
            {
                Intersections.Add(new Intersection(x, y));
            }
        }
    }

    // Añade un nuevo vehículo a la ciudad
    public void AddVehicle()
    {
        int id = Vehicles.Count + 1;
        VehicleType type = (VehicleType)new Random().Next(3);
        int x, y;
        do
        {
            x = new Random().Next(Width);
            y = new Random().Next(Height);
        } while (baseGrid[y, x] != '·'); // Asegura que el vehículo se coloque en una calle
        Direction direction = (Direction)new Random().Next(4);
        Vehicles.Add(new Vehicle(id, type, x, y, direction));
    }

    // Actualiza la posición de todos los vehículos
    public void UpdateVehicles()
    {
        foreach (var vehicle in Vehicles)
        {
            int oldX = vehicle.X;
            int oldY = vehicle.Y;
            vehicle.Move();

            // Ajusta la posición para que los vehículos se mantengan dentro de la ciudad
            vehicle.X = (vehicle.X + Width) % Width;
            vehicle.Y = (vehicle.Y + Height) % Height;

            // Si la nueva posición no está en una calle, revierte al movimiento
            if (baseGrid[vehicle.Y, vehicle.X] != '·')
            {
                vehicle.X = oldX;
                vehicle.Y = oldY;
                // Opcionalmente, cambia la dirección
                vehicle.Direction = (Direction)new Random().Next(4);
            }
        }
    }

    // Actualiza el estado de los semáforos en las intersecciones
    public void UpdateIntersections()
    {
        foreach (var intersection in Intersections)
        {
            if (new Random().Next(10) == 0) // 10% de probabilidad de cambiar el semáforo
            {
                intersection.ChangeLight();
            }
        }
    }

    // Detecta colisiones entre vehículos
    public void DetectCollisions()
    {
        for (int i = 0; i < Vehicles.Count; i++)
        {
            for (int j = i + 1; j < Vehicles.Count; j++)
            {
                if (Vehicles[i].X == Vehicles[j].X && Vehicles[i].Y == Vehicles[j].Y)
                {
                    Console.WriteLine($"¡Colisión detectada entre Vehículo {Vehicles[i].Id} y Vehículo {Vehicles[j].Id}!");
                }
            }
        }
    }

    // Muestra el estado actual de la ciudad en la consola
    public void Display()
    {
        char[,] grid = (char[,])baseGrid.Clone();

        // Añade intersecciones a la cuadrícula
        foreach (var intersection in Intersections)
        {
            grid[intersection.Y, intersection.X] = intersection.IsGreenLight ? '@' : '&';
        }

        // Añade vehículos a la cuadrícula
        foreach (var vehicle in Vehicles)
        {
            grid[vehicle.Y, vehicle.X] = vehicle.Type.ToString()[0];
        }

        // Muestra la cuadrícula
        Console.Clear();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Console.Write(grid[y, x]);
            }
            Console.WriteLine();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Crea una ciudad de 51x21
        City city = new City(51, 21);

        // Añade 10 vehículos a la ciudad
        for (int i = 0; i < 10; i++)
        {
            city.AddVehicle();
        }

        // Bucle principal de la simulación
        while (true)
        {
            city.UpdateVehicles();
            city.UpdateIntersections();
            city.DetectCollisions();
            city.Display();
            Thread.Sleep(500); // Pausa de 500ms entre cada actualización
        }
    }
}