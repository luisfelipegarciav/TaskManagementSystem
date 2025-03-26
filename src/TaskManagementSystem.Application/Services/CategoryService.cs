using Microsoft.Extensions.Logging;
using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Application
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepositoryGeneric;
        private readonly ILogger<CategoryService> _logger;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(
            IRepository<Category> categoryRepositoryGeneric,
            ILogger<CategoryService> logger,
            ICategoryRepository categoryRepository
            )
        {
            _categoryRepositoryGeneric = categoryRepositoryGeneric;
            _logger = logger;
            _categoryRepository = categoryRepository;
        }

        public async Task<ServiceResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryDto categoryDto)
        {
            try
            {
                if(categoryDto == null || string.IsNullOrWhiteSpace(categoryDto.Name))
                    return ServiceResponse<CategoryDto>.Failure("Unable to process data for category.");

                var existingCategory = await _categoryRepository.GetCategoryByName(categoryDto.Name);
                if (existingCategory != null)
                    return ServiceResponse<CategoryDto>.Failure("Category already exists.");

                var category = new Category { Name = categoryDto.Name };

                var createdCategory = await _categoryRepositoryGeneric.AddAsync(category);

                return ServiceResponse<CategoryDto>.Success(new CategoryDto
                {
                    Id = createdCategory.Id,
                    Name = createdCategory.Name
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category.");
                return ServiceResponse<CategoryDto>.Failure(ex.Message);
            }
        }

        public Task<ServiceResponse<List<CategoryDto>>> GetAllCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
