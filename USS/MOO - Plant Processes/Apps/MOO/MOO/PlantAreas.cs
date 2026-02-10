using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO
{
    /// <summary>
    /// 
    /// </summary>
    public class PlantArea
    {
        /// <summary>
        /// Id of area
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Area Name
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// List areas for both Minntac and Keetac
    /// </summary>
    public static class PlantAreaList
    {

        private static List<PlantArea> _minntacList = null;
        public static List<PlantArea> MinntacList
        {
            get
            {
                if (_minntacList == null)
                    _minntacList = GetMinntacList();
                return _minntacList;
            }
        }

        private static List<PlantArea> _keetacList = null;
        public static List<PlantArea> KeetacList
        {
            get
            {
                if (_keetacList == null)
                    _keetacList = GetKeetacList();
                return _keetacList;
            }
        }

        public static PlantArea GetByName(string name, List<PlantArea> SearchList)
        {
            PlantArea pa = SearchList.First(s => s.Name == name);
            return pa;
        }

        public static PlantArea GetByName(string name, MOO.Plant plant)
        {
            if (plant == Plant.Minntac)
                return GetByName(name, MinntacList);
            else
                return GetByName(name, KeetacList);
        }

        public static PlantArea GetById(int Id, List<PlantArea> SearchList)
        {
            PlantArea pa = SearchList.First(s => s.Id == Id);
            return pa;
        }

        public static PlantArea GetById(int Id, MOO.Plant plant)
        {
            if (plant == Plant.Minntac)
                return GetById(Id, MinntacList);
            else
                return GetById(Id, KeetacList);
        }


        /// <summary>
        /// Return list of Minntac Areas
        /// </summary>
        /// <returns></returns>
        private static List<PlantArea> GetMinntacList()
        {
            List<PlantArea> plantList = new();

            plantList.Add(new()
            {
                Id = 0,
                Name = "Admin"
            });
            plantList.Add(new()
            {
                Id = 1,
                Name = "Conc"
            });
            plantList.Add(new()
            {
                Id = 2,
                Name = "Crusher"
            });
            plantList.Add(new()
            {
                Id = 3,
                Name = "Mine"
            });
            plantList.Add(new()
            {
                Id = 4,
                Name = "Agg Step 2"
            });
            plantList.Add(new()
            {
                Id = 5,
                Name = "Agg Step 3"
            });
            plantList.Add(new()
            {
                Id = 6,
                Name = "Shops"
            });
            plantList.Add(new()
            {
                Id = 6,
                Name = "Lab"
            });
            plantList.Add(new()
            {
                Id = 6,
                Name = "Utility"
            });

            return plantList;
        }
        /// <summary>
        /// Return list of Keetac Areas
        /// </summary>
        /// <returns></returns>
        public static List<PlantArea> GetKeetacList()
        {
            List<PlantArea> plantList = new();

            plantList.Add(new()
            {
                Id = 0,
                Name = "Admin"
            });
            plantList.Add(new()
            {
                Id = 1,
                Name = "Conc"
            });
            plantList.Add(new()
            {
                Id = 2,
                Name = "Crusher"
            });
            plantList.Add(new()
            {
                Id = 3,
                Name = "Mine"
            });
            plantList.Add(new()
            {
                Id = 4,
                Name = "Pellet Plant"
            });
            plantList.Add(new()
            {
                Id = 5,
                Name = "Lab"
            });

            return plantList;
        }
    }
}
