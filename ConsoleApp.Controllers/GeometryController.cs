using System;
using ConsoleApp.IServices;

namespace ConsoleApp.Controllers
{
    public class GeometryController<TGeometry, TKey>
    {
        private IGeometryService<TGeometry, TKey> _geometryService;
        //todo добавить валидацию и обработку ошибок
        public TKey SaveGeometry(TGeometry geometry)
        {
            return _geometryService.Save(geometry);
        }
    }
}