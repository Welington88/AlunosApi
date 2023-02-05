using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlunosApi.Models
{
    [Table("ALUNOS")]
    public class Alunos
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(80)]
        public String? Nome { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public String? Email { get; set; }

        [Required]
        public int Idade { get; set; }
    }
}