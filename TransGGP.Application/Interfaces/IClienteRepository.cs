using System;
using TransGGP.Domain.Models;


namespace TransGGP.Application.Interfaces
{
    public interface IClienteRepository
    {
        List<Cliente> ObtenerTodos();
        Cliente? ObtenerPorId(int id);
        void Agregar(Cliente cliente);

        void Eliminar(int id);
    }
}

