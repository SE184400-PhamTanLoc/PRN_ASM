using System.Threading.Tasks;
using BusinessLayer.DTO;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using BCrypt.Net;

namespace BusinessLayer.Service
{
    public class AuthService
    {
        private readonly CustomerRepository _customerRepository;

        public AuthService(CustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<RegisterResponse> RegisterAsync(CustomerCreateDTO dto)
        {
            // Check if email exists
            if (await _customerRepository.EmailExistsAsync(dto.Email))
            {
                return new RegisterResponse
                {
                    IsSuccess = false,
                    Message = "Email already exists."
                };
            }

            // Hash password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Create customer entity
            var customer = new Customer
            {
                Name = dto.FullName,
                Email = dto.Email,
                Password = hashedPassword
            };

            // Save to DB
            var created = await _customerRepository.CreateAsync(customer);

            return new RegisterResponse
            {
                IsSuccess = true,
                Message = "Register successful!",
                CustomerId = created.CustomerId,
                FullName = created.Name,
                Email = created.Email
            };
        }

        public async Task<LoginResponse> LoginAsync(CustomerLoginDTO dto)
        {
            var customer = await _customerRepository.GetByEmailAsync(dto.Email);
            if (customer == null)
            {
                return new LoginResponse
                {
                    IsSuccess = false,
                    Message = "Email or password is incorrect."
                };
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, customer.Password);
            if (!isPasswordValid)
            {
                return new LoginResponse
                {
                    IsSuccess = false,
                    Message = "Email or password is incorrect."
                };
            }

            return new LoginResponse
            {
                IsSuccess = true,
                Message = "Login successful!",
                CustomerId = customer.CustomerId,
                FullName = customer.Name,
                Email = customer.Email
            };
        }
    }
}