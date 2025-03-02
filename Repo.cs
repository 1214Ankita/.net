// Repositories/ExcelDishRepository.cs
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DishApi.Models;
using Newtonsoft.Json;

namespace DishApi.Repositories
{
    public class ExcelDishRepository
    {
        private readonly string _filePath = "Dishes.xlsx";

        public ExcelDishRepository()
        {
            // Ensure the file exists and has some sample data
            if (!File.Exists(_filePath))
            {
                CreateSampleData();
            }
        }

        private void CreateSampleData()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Dishes");
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Description";
                worksheet.Cells[1, 4].Value = "Price";
                worksheet.Cells[1, 5].Value = "Type"; // New column for type

                worksheet.Cells[2, 1].Value = 1;
                worksheet.Cells[2, 2].Value = "Spaghetti";
                worksheet.Cells[2, 3].Value = "Delicious spaghetti with marinara sauce.";
                worksheet.Cells[2, 4].Value = 12.99;
                worksheet.Cells[2, 5].Value = DishType.MainDish.ToString(); // Type

                worksheet.Cells[3, 1].Value = 2;
                worksheet.Cells[3, 2].Value = "Chocolate Cake";
                worksheet.Cells[3, 3].Value = "Rich chocolate cake with frosting.";
                worksheet.Cells[3, 4].Value = 5.99;
                worksheet.Cells[3, 5].Value = DishType.Dessert.ToString(); // Type

                package.SaveAs(new FileInfo(_filePath));
            }
        }

        public async Task<List<Dish>> GetAllDishesAsync()
        {
            var dishes = new List<Dish>();
            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var dishType = (DishType)Enum.Parse(typeof(DishType), worksheet.Cells[row, 5].Value.ToString());
                    var dish = new Dish
                    {
                        Id = (int)worksheet.Cells[row, 1].Value,
                        Name = worksheet.Cells[row, 2].Value.ToString(),
                        Description = worksheet.Cells[row, 3].Value.ToString(),
                        Price = (decimal)worksheet.Cells[row, 4].Value,
                        Type = dishType // Set the type
                    };

                    dishes.Add(dish);
                }
            }
            return await Task.FromResult(dishes);
        }
    }
}
