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



using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CrudProj.Server.Models;

namespace CrudProj.Server.Repositories
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
                    Dish dish;

                    if (dishType == "MainDish")
                    {
                        dish = new MainDish
                        {
                            Id = (int)worksheet.Cells[row, 1].Value,
                            Name = worksheet.Cells[row, 2].Value.ToString(),
                            Description = worksheet.Cells[row, 3].Value.ToString(),
                            Price = (int)worksheet.Cells[row, 4].Value, // Read as int
                            MainIngredient = worksheet.Cells[row, 6].Value.ToString() // Read main ingredient
                        };
                    }
                    else if (dishType == "Dessert")
                    {
                        dish = new Dessert
                        {
                            Id = (int)worksheet.Cells[row, 1].Value,
                            Name = worksheet.Cells[row, 2].Value.ToString(),
                            Description = worksheet.Cells[row, 3].Value.ToString(),
                            Price = (int)worksheet.Cells[row, 4].Value, // Read as int
                            IsGlutenFree = (bool)worksheet.Cells[row, 7].Value // Read gluten-free status
                        };
                    }
                    else
                    {
                        continue; // Skip unknown types
                    }

                    dishes.Add(dish);
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
            // Generate a new ID for the dish
            var dishes = await GetAllDishesAsync();
            dish.Id = dishes.Count > 0 ? dishes.Max(d => d.Id) + 1 : 1;

            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                // Add the new dish to the next row in the Excel sheet
                worksheet.Cells[rowCount + 1, 1].Value = dish.Id; // ID
                worksheet.Cells[rowCount + 1, 2].Value = dish.Name; // Name
                worksheet.Cells[rowCount + 1, 3].Value = dish.Description; // Description
                worksheet.Cells[rowCount + 1, 4].Value = dish.Price; // Price

                // Set the type and additional properties based on the dish type
                if (dish is MainDish mainDish)
                {
                    worksheet.Cells[rowCount + 1, 5].Value = mainDish.GetDishType(); // Type
                    worksheet.Cells[rowCount + 1, 6].Value = mainDish.MainIngredient; // Main Ingredient
                    worksheet.Cells[rowCount + 1, 7].Value = ""; // Gluten Free (not applicable for MainDish)
                }
                else if (dish is Dessert dessert)
                {
                    worksheet.Cells[rowCount + 1, 5].Value = dessert.GetDishType(); // Type
                    worksheet.Cells[rowCount + 1, 6].Value = ""; // Main Ingredient (not applicable for Dessert)
                    worksheet.Cells[rowCount + 1, 7].Value = dessert.IsGlutenFree; // Gluten Free
                }

                // Save the changes to the Excel file
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
                            worksheet.Cells[row, 4].Value = dish.Price; // Update as int
                            worksheet.Cells[row, 5].Value = dish.GetDishType(); // Update the type
                            if (dish is MainDish mainDish)
                            {
                                worksheet.Cells[row, 6].Value = mainDish.MainIngredient; // Update main ingredient
                            }
                            if (dish is Dessert dessert)
                            {
                                worksheet.Cells[row, 7].Value = dessert.IsGlutenFree; // Update gluten-free status
                            }
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
