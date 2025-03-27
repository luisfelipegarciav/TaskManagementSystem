namespace TaskManagementSystem.Application
{
    public interface ICategoryService
    {
        Task<ServiceResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryDto categoryDto);
        Task<ServiceResponse<CategoryDto>> GetCategoryByIdAsync(int id);
        Task<ServiceResponse<CategoryDto>> GetCategoryByNameAsync(string name);
        Task<ServiceResponse<List<CategoryDto>>> GetAllCategoriesAsync();
        Task<ServiceResponse<bool>> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto);
    }
}
