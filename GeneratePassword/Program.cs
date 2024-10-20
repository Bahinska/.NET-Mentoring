namespace GeneratePassword
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string password = "mySecurePassword!123";

            byte[] salt = PasswordHasher.GenerateRandomSalt();

            string hashedPassword = PasswordHasher.GeneratePasswordHashUsingSalt(password, salt);

            Console.WriteLine("Salt: " + Convert.ToBase64String(salt));
            Console.WriteLine("Hashed Password: " + hashedPassword);

            bool isPasswordValid = PasswordHasher.VerifyPassword(password, hashedPassword, salt);
            Console.WriteLine("Password verification: " + (isPasswordValid ? "Success" : "Failed"));
        }
    }
}
