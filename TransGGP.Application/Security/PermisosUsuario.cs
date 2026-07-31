using TransGGP.Domain.Models;

namespace TransGGP.Application.Security
{
    public class PermisosUsuario
    {
        public bool PuedeVerServicios { get; set; }
        public bool PuedeEditarServicios { get; set; }
        public bool PuedeVerClientes { get; set; }
        public bool PuedeEditarClientes { get; set; }
        public bool PuedeVerOperadores { get; set; }
        public bool PuedeEditarOperadores { get; set; }
        public bool PuedeVerUnidades { get; set; }
        public bool PuedeEditarUnidades { get; set; }
        public bool PuedeVerSemirremolques { get; set; }
        public bool PuedeEditarSemirremolques { get; set; }
        public bool PuedeVerDollys { get; set; }
        public bool PuedeEditarDollys { get; set; }
        public bool PuedeVerConfiguraciones { get; set; }
        public bool PuedeEditarConfiguraciones { get; set; }

        public void AplicarA(Usuario usuario)
        {
            usuario.PuedeEditarServicios = PuedeEditarServicios;
            usuario.PuedeVerServicios = PuedeVerServicios || PuedeEditarServicios;
            usuario.PuedeEditarClientes = PuedeEditarClientes;
            usuario.PuedeVerClientes = PuedeVerClientes || PuedeEditarClientes;
            usuario.PuedeEditarOperadores = PuedeEditarOperadores;
            usuario.PuedeVerOperadores = PuedeVerOperadores || PuedeEditarOperadores;
            usuario.PuedeEditarUnidades = PuedeEditarUnidades;
            usuario.PuedeVerUnidades = PuedeVerUnidades || PuedeEditarUnidades;
            usuario.PuedeEditarSemirremolques = PuedeEditarSemirremolques;
            usuario.PuedeVerSemirremolques = PuedeVerSemirremolques || PuedeEditarSemirremolques;
            usuario.PuedeEditarDollys = PuedeEditarDollys;
            usuario.PuedeVerDollys = PuedeVerDollys || PuedeEditarDollys;
            usuario.PuedeEditarConfiguraciones = PuedeEditarConfiguraciones;
            usuario.PuedeVerConfiguraciones = PuedeVerConfiguraciones || PuedeEditarConfiguraciones;
        }

        public static PermisosUsuario DesdeUsuario(Usuario usuario)
        {
            return new PermisosUsuario
            {
                PuedeVerServicios = usuario.PuedeVerServicios,
                PuedeEditarServicios = usuario.PuedeEditarServicios,
                PuedeVerClientes = usuario.PuedeVerClientes,
                PuedeEditarClientes = usuario.PuedeEditarClientes,
                PuedeVerOperadores = usuario.PuedeVerOperadores,
                PuedeEditarOperadores = usuario.PuedeEditarOperadores,
                PuedeVerUnidades = usuario.PuedeVerUnidades,
                PuedeEditarUnidades = usuario.PuedeEditarUnidades,
                PuedeVerSemirremolques = usuario.PuedeVerSemirremolques,
                PuedeEditarSemirremolques = usuario.PuedeEditarSemirremolques,
                PuedeVerDollys = usuario.PuedeVerDollys,
                PuedeEditarDollys = usuario.PuedeEditarDollys,
                PuedeVerConfiguraciones = usuario.PuedeVerConfiguraciones,
                PuedeEditarConfiguraciones = usuario.PuedeEditarConfiguraciones
            };
        }

        public static PermisosUsuario Todos()
        {
            return new PermisosUsuario
            {
                PuedeVerServicios = true,
                PuedeEditarServicios = true,
                PuedeVerClientes = true,
                PuedeEditarClientes = true,
                PuedeVerOperadores = true,
                PuedeEditarOperadores = true,
                PuedeVerUnidades = true,
                PuedeEditarUnidades = true,
                PuedeVerSemirremolques = true,
                PuedeEditarSemirremolques = true,
                PuedeVerDollys = true,
                PuedeEditarDollys = true,
                PuedeVerConfiguraciones = true,
                PuedeEditarConfiguraciones = true
            };
        }
    }
}
