using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarFlight
{
    /// <summary>
    /// Тип космического объекта
    /// </summary>
    public enum ObjectType 
    { 
        Star,
        Planet1,
        Planet2,    
        Planet3,
        Planet4,
    }

    /// <summary>
    /// 3D-точка
    /// </summary>
    internal class Point3D: IComparable 
    {
        public double X, Y, Z;
        public ObjectType ObjectType = ObjectType.Star;

        public Point3D(double x, double y, double z, ObjectType objectType = ObjectType.Star)
        {
            X = x;
            Y = y;
            Z = z;
            ObjectType = objectType;
        }
        /// <summary>
        /// Реализация метода сравнения для сортировки звезд по удаленности от наблюдателя. 
        /// Для простоты учитывается только координата Z
        /// </summary>
        /// <param name="obj">Сравниваемый объект (другая 3D-точка)</param>
        /// <returns></returns>
        public int CompareTo(object? obj)
        {
            Point3D? point3D = obj as Point3D;
            if (Z > point3D?.Z) 
                return 1;
            if (Z == point3D?.Z) 
                return 0;
            return -1;
        }
    }
}
