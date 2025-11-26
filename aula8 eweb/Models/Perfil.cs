using System;
using System.ComponentModel.DataAnnotations;

namespace aula8_eweb.Models
{
    public class Perfil
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        public string? UserName { get; set; }
    }
}