// Repositories/ExcelDishRepository.cs
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DishApi.Models;

namespace DishApi.Repositories
{
    public class ExcelDishRepository
    {
        private readonly string _filePath = "Dishes.xlsx";

        public ExcelDishRepository()
        {
            // Ensure the file exists
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException("The Excel file does not exist.", _filePath);
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
                    var dishType = worksheet.Cells[row, 5].Value.ToString();
                    Dish dish = dishType switch
                    {
                        "MainDish" => new MainDish
                        {
                            Id = (int)worksheet.Cells[row, 1].Value,
                            Name = worksheet.Cells[row, 2].Value.ToString(),
                            Description = worksheet.Cells[row, 3].Value.ToString(),
                            Price = (decimal)worksheet.Cells[row, 4].Value,
                            MainIngredient = "Pasta" // Example main ingredient
                        },
                        "Dessert" => new Dessert
                        {
                            Id = (int)worksheet.Cells[row, 1].Value,
                            Name = worksheet.Cells[row, 2].Value.ToString(),
                            Description = worksheet.Cells[row, 3].Value.ToString(),
                            Price = (decimal)worksheet.Cells[row, 4].Value,
                            IsGlutenFree = true // Example property
                        },
                        _ => null // Skip unknown types
                    };

                    if (dish != null)
                    {
                        dishes.Add(dish);
                    }
                }
            }
            return await Task.FromResult(dishes);
        }

        public async Task<Dish> GetDishByIdAsync(int id)
        {
            var dishes = await GetAllDishesAsync();
            return dishes.FirstOrDefault(d => d.Id == id);
        }

        public async Task AddDishAsync(Dish dish)
        {
            var dishes = await GetAllDishesAsync();
            dish.Id = dishes.Count > 0 ? dishes.Max(d => d.Id) + 1 : 1;

            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                worksheet.Cells[rowCount + 1, 1].Value = dish.Id;
                worksheet.Cells[rowCount + 1, 2].Value = dish.Name;
                worksheet.Cells[rowCount + 1, 3].Value = dish.Description;
                worksheet.Cells[rowCount + 1, 4].Value = dish.Price;
                worksheet.Cells[rowCount + 1, 5].Value = dish.GetDishType(); // Set the type

                package.Save();
            }
        }

        public async Task UpdateDishAsync(Dish dish)
        {
            var dishes = await GetAllDishesAsync();
            if (dishes.Any(d => d.Id == dish.Id))
            {
                using (var package = new ExcelPackage(new FileInfo(_filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        if ((int)worksheet.Cells[row, 1].Value == dish.Id)
                        {
                            worksheet.Cells[row, 2].Value = dish.Name;
                            worksheet.Cells[row, 3].Value = dish.Description;
                            worksheet.Cells[row, 4].Value = dish.Price;
                            worksheet.Cells[row, 5].Value = dish.GetDishType(); // Update the type
                            break;
                        }
                    }
                    package.Save();
                }
            }
        }

        public async Task DeleteDishAsync(int id)
        {
            var dishes = await GetAllDishesAsync();
            if (dishes.Any(d => d.Id == id))
            {
                using (var package = new ExcelPackage(new FileInfo(_filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        if ((int)worksheet.Cells[row, 1].Value == id)
                        {
                            worksheet.DeleteRow(row);
                            break;
                        }
                    }
                    package.Save();
                }
            }
        }
    }
}
