using System.ComponentModel.DataAnnotations;

namespace SimpleWebApplication.Models
{
    public class RegisterModel
    {
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password1 { get; set; }

        [DataType(DataType.Password), Compare("Password1")]
        public string Password2 { get; set; }
    }
}
