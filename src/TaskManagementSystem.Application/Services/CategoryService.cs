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
                    throw new InvalidModelException("Invalid category data.");

                var getCategoryByNameResponse = await GetCategoryByNameAsync(categoryDto.Name);
                if (getCategoryByNameResponse != null && getCategoryByNameResponse.IsSuccessful && getCategoryByNameResponse.Data != null)
                    throw new EntityAlreadyExistsException($"Category {categoryDto.Name} already exists.");

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

        public async Task<ServiceResponse<bool>> DeleteCategoryByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new InvalidModelException("Invalid category data.");

                var getCategoryByIdResponse = await GetCategoryByIdAsync(id);
                if (getCategoryByIdResponse == null || !getCategoryByIdResponse.IsSuccessful || getCategoryByIdResponse.Data == null)
                    throw new CategoryNotFoundException("Category not found.");

                //TODO: _taskItemService.HasTaskItemsByCategoryId(id)

                await _categoryRepositoryGeneric.DeleteAsync(id);

                return ServiceResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category.");
                return ServiceResponse<bool>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResponse<List<CategoryDto>>> GetAllCategoriesAsync()
        {
            try
            {
                var result = await _categoryRepositoryGeneric.GetAllAsync();
                var categories = result?.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })?.ToList();

                return ServiceResponse<List<CategoryDto>>.Success(categories ?? new List<CategoryDto>());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error pulling categories.");
                return ServiceResponse<List<CategoryDto>>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResponse<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new InvalidModelException("Invalid category id.");

                var result = await _categoryRepositoryGeneric.GetByIdAsync(id);

                if (result == null)
                    throw new CategoryNotFoundException("Category not found.");

                return ServiceResponse<CategoryDto>.Success(new CategoryDto
                {
                    Id = result.Id,
                    Name = result.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error pulling category.");
                return ServiceResponse<CategoryDto>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResponse<CategoryDto>> GetCategoryByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new InvalidModelException("Invalid category name.");

                var result = await _categoryRepository.GetCategoryByName(name);

                if (result == null)
                    throw new CategoryNotFoundException("Category not found.");

                return ServiceResponse<CategoryDto>.Success(new CategoryDto
                {
                    Id = result.Id,
                    Name = result.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error pulling category.");
                return ServiceResponse<CategoryDto>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto)
        {
            try
            {
                if (categoryDto == null
                    || string.IsNullOrWhiteSpace(categoryDto.Name)
                    || id <= 0)
                    throw new InvalidModelException("Invalid category data.");

                var getCategoryByIdResponse = await GetCategoryByIdAsync(id);
                if (getCategoryByIdResponse == null || !getCategoryByIdResponse.IsSuccessful || getCategoryByIdResponse.Data == null)
                    throw new CategoryNotFoundException("Category not found.");

                var getCategoryByNameResponse = await GetCategoryByNameAsync(categoryDto.Name);
                if (getCategoryByNameResponse != null && getCategoryByNameResponse.IsSuccessful && getCategoryByNameResponse.Data != null && getCategoryByNameResponse.Data.Id != id)
                    throw new EntityAlreadyExistsException("Category already exists.");

                await _categoryRepositoryGeneric.UpdateAsync(new Category
                {
                    Id = id,
                    Name = categoryDto.Name
                });
                return ServiceResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category.");
                return ServiceResponse<bool>.Failure(ex.Message);
            }
        }
    }
}
