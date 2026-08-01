using System.Security.Claims;
using TransGGP.Domain.Models;

namespace TransGGP.Application.Security
{
    public static class Permisos
    {
        public const string ClaimType = "permiso";

        public const string ServiciosLeer = "Servicios.Leer";
        public const string ServiciosEditar = "Servicios.Editar";
        public const string ClientesLeer = "Clientes.Leer";
        public const string ClientesEditar = "Clientes.Editar";
        public const string OperadoresLeer = "Operadores.Leer";
        public const string OperadoresEditar = "Operadores.Editar";
        public const string UnidadesLeer = "Unidades.Leer";
        public const string UnidadesEditar = "Unidades.Editar";
        public const string SemirremolquesLeer = "Semirremolques.Leer";
        public const string SemirremolquesEditar = "Semirremolques.Editar";
        public const string DollysLeer = "Dollys.Leer";
        public const string DollysEditar = "Dollys.Editar";
        public const string ConfiguracionesLeer = "Configuraciones.Leer";
        public const string ConfiguracionesEditar = "Configuraciones.Editar";

        public static readonly IReadOnlyList<ModuloPermiso> Modulos = new List<ModuloPermiso>
        {
            new("Servicios", "Servicios", ServiciosLeer, ServiciosEditar),
            new("Clientes", "Clientes", ClientesLeer, ClientesEditar),
            new("Operadores", "Operadores", OperadoresLeer, OperadoresEditar),
            new("Unidades", "Unidades", UnidadesLeer, UnidadesEditar),
            new("Semirremolques", "Semirremolques", SemirremolquesLeer, SemirremolquesEditar),
            new("Dollys", "Dollys", DollysLeer, DollysEditar),
            new("Configuraciones", "Configuraciones", ConfiguracionesLeer, ConfiguracionesEditar)
        };

        public static IEnumerable<string> ObtenerPermisos(Usuario usuario)
        {
            if (usuario.Rol == "Admin")
                return Todos();

            var permisos = new List<string>();

            Agregar(permisos, usuario.PuedeVerServicios, usuario.PuedeEditarServicios, ServiciosLeer, ServiciosEditar);
            Agregar(permisos, usuario.PuedeVerClientes, usuario.PuedeEditarClientes, ClientesLeer, ClientesEditar);
            Agregar(permisos, usuario.PuedeVerOperadores, usuario.PuedeEditarOperadores, OperadoresLeer, OperadoresEditar);
            Agregar(permisos, usuario.PuedeVerUnidades, usuario.PuedeEditarUnidades, UnidadesLeer, UnidadesEditar);
            Agregar(permisos, usuario.PuedeVerSemirremolques, usuario.PuedeEditarSemirremolques, SemirremolquesLeer, SemirremolquesEditar);
            Agregar(permisos, usuario.PuedeVerDollys, usuario.PuedeEditarDollys, DollysLeer, DollysEditar);
            Agregar(permisos, usuario.PuedeVerConfiguraciones, usuario.PuedeEditarConfiguraciones, ConfiguracionesLeer, ConfiguracionesEditar);

            return permisos;
        }

        public static IEnumerable<string> Todos()
        {
            return Modulos.SelectMany(m => new[] { m.PermisoLeer, m.PermisoEditar });
        }

        public static bool TienePermiso(this ClaimsPrincipal usuario, string permiso)
        {
            return usuario.IsInRole("Admin") || usuario.HasClaim(ClaimType, permiso);
        }

        private static void Agregar(List<string> permisos, bool puedeVer, bool puedeEditar, string permisoLeer, string permisoEditar)
        {
            if (puedeVer || puedeEditar)
                permisos.Add(permisoLeer);

            if (puedeEditar)
                permisos.Add(permisoEditar);
        }
    }

    public record ModuloPermiso(string Nombre, string Etiqueta, string PermisoLeer, string PermisoEditar);
}
