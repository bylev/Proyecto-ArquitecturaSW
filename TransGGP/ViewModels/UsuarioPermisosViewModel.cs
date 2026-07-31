using TransGGP.Application.Security;

namespace TransGGP.ViewModels
{
    public class UsuarioPermisosViewModel
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

        public PermisosUsuario ToPermisosUsuario()
        {
            return new PermisosUsuario
            {
                PuedeVerServicios = PuedeVerServicios,
                PuedeEditarServicios = PuedeEditarServicios,
                PuedeVerClientes = PuedeVerClientes,
                PuedeEditarClientes = PuedeEditarClientes,
                PuedeVerOperadores = PuedeVerOperadores,
                PuedeEditarOperadores = PuedeEditarOperadores,
                PuedeVerUnidades = PuedeVerUnidades,
                PuedeEditarUnidades = PuedeEditarUnidades,
                PuedeVerSemirremolques = PuedeVerSemirremolques,
                PuedeEditarSemirremolques = PuedeEditarSemirremolques,
                PuedeVerDollys = PuedeVerDollys,
                PuedeEditarDollys = PuedeEditarDollys,
                PuedeVerConfiguraciones = PuedeVerConfiguraciones,
                PuedeEditarConfiguraciones = PuedeEditarConfiguraciones
            };
        }
    }
}
