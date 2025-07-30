using Microsoft.Extensions.Configuration;

namespace DataAccessLayer.Repository
{
    public class AdminRepository
    {
        private readonly IConfiguration _configuration;

        public AdminRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetAdminEmail()
        {
            return _configuration["AdminAccount:Email"];
        }

        public string GetAdminPassword()
        {
            return _configuration["AdminAccount:Password"];
        }

        public bool IsAdminAccount(string email)
        {
            return email.Equals(GetAdminEmail(), StringComparison.OrdinalIgnoreCase);
        }

        public bool ValidateAdminCredentials(string email, string password)
        {
            return IsAdminAccount(email) && password.Equals(GetAdminPassword());
        }
    }
} 