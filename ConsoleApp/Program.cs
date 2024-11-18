using DataAccess.Repositories.ConsoleApp;
using DataAccess.Repositories.ConsoleApp.Interfaces;
using DomainModels;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;


namespace ConsoleApp
{
    class Program
    {

        static void Main(string[] args)
        {
            /*using (PostgreApplicationContext db = new PostgreApplicationContext())
            {
                //db.Database.EnsureCreated();
                GeometryOriginal geometryOriginal = new GeometryOriginal();
                geometryOriginal.Data = Polygon.Empty;
                db.GeometryOriginals.Add(geometryOriginal);
                db.SaveChanges();
            }*/
            var services = new ServiceCollection();
            services.AddTransient<IRepository<GeometryWithFragments<Polygon, Polygon>, int>, MockRepository>();
            var serviceProvider = services.BuildServiceProvider();
            var service = serviceProvider.GetRequiredService<IRepository<GeometryWithFragments<Polygon, Polygon>, int>>();
        }
}

}