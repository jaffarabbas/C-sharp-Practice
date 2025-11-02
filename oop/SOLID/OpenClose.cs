using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oop.SOLID
{
    /// <summary>
    /// Interfase with a example of Open/Close Principle
    /// </summary>
    public interface IShape
    {
        double Area();
    }
    public class Rectangle : IShape
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double Area()
        {
            return Width * Height;
        }
    }

    public class Circle : IShape
    {
        public double Radius { get; set; }
        public double Area()
        {
            return Math.PI * Radius * Radius;
        }
    }
    internal class OpenClose
    {
        public void CalculateArea()
        {
            List<IShape> shapes = new List<IShape>
            {
                new Rectangle { Width = 4, Height = 5 },
                new Circle { Radius = 3 }
            };
            double totalArea = shapes.Sum(s => s.Area());
            Console.WriteLine($"Total Area: {totalArea}");
        }
    }
}
