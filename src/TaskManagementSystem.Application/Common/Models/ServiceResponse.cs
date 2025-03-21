namespace TaskManagementSystem.Application
{
    public class ServiceResponse<T>
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        private ServiceResponse(bool isSuccessful, string message, T data)
        {
            IsSuccessful = isSuccessful;
            Message = message;
            Data = data;
        }

        public static ServiceResponse<T> Success(T data, string message = null)
        {
            return new ServiceResponse<T>(true, message, data);
        }

        public static ServiceResponse<T> Success(string message = null)
        {
            return new ServiceResponse<T>(true, message, default);
        }

        public static ServiceResponse<T> Failure(string message)
        {
            return new ServiceResponse<T>(false, message, default);
        }

        public static ServiceResponse<T> Failure()
        {
            return new ServiceResponse<T>(false, "An unexpected error occurred.", default);
        }
    }
}
