using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Company_Management.Modules
{
    public class Validator
    {

        // validate email format
        public static bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return true;

            var mail = new MailAddress(email);
            bool isValidEmail = mail.Host.Contains(".");
            if (!isValidEmail)
            {
                return false;
            }
            return true;
        }

        // Validate phone number column contain only Numeric values.
        public static bool IsPhoneNumber(string number)
        {
            if (string.IsNullOrEmpty(number)) return true;

            if (number != null) return Regex.IsMatch(number, "^[0-9]+$");
            else return false;
        }
    }


}
