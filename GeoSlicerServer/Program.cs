using System;
using System.IO;
using DataAccess.PostgreSql;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;


namespace GeoSlicerServer
{
    class Program
    {

        static void Main(string[] args)
        {
            String currentDirectory = Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\DataAccess.PostgreSql";
            Console.WriteLine(currentDirectory);
            using (PostgreApplicationContext db = new SampleContextFactory().CreateDbContext(new []{currentDirectory}))
            {
                //db.Database.EnsureCreated();
                GeometryOriginal geometryOriginal = new GeometryOriginal();
                geometryOriginal.Data = Polygon.Empty;
                db.GeometryOriginals.Add(geometryOriginal);
                db.SaveChanges();
            }
        }
}

}