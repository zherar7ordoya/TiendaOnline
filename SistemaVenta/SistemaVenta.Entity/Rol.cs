namespace Entity;

public partial class Rol
{
    public int IdRol { get; set; }
    public string? Descripcion { get; set; }
    public bool? EsActivo { get; set; }
    public DateTime? FechaRegistro { get; set; }
    public virtual ICollection<RolMenu> RolMenus { get; set; } = [];
    public virtual ICollection<Usuario> Usuarios { get; set; } = [];
}
