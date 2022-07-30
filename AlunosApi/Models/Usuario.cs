using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AlunosApi.Models
{
	
[Table("USUARIO")]
public class Usuario
{
    [Key]
    [JsonIgnore]
    public int IdUsuario { get; set; }

    [Required]
    [Column("Nome", Order = 2, TypeName = "varchar(50)")]
    public String? Nome { get; set; }

    [Required]
    [StringLength(50)]
    public String? Senha { get; set; }

}
   
}

