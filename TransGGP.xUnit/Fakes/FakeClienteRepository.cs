using TransGGP.Application.Interfaces;
using TransGGP.Domain.Models;

namespace TransGGP.xUnit.Fakes;

public class FakeClienteRepository : IClienteRepository
{
    public List<Cliente> Clientes { get; } = new();

    public List<Cliente> ObtenerTodos()
    {
        return Clientes;
    }

    public Cliente? ObtenerPorId(int id)
    {
        return Clientes.FirstOrDefault(cliente => cliente.Id == id);
    }

    public void Agregar(Cliente cliente)
    {
        Clientes.Add(cliente);
    }

    public void Actualizar(Cliente cliente)
    {
        var clienteExistente = ObtenerPorId(cliente.Id);

        if (clienteExistente is not null)
        {
            clienteExistente.Nombre = cliente.Nombre;
        }
    }

    public void Eliminar(int id)
    {
        var cliente = ObtenerPorId(id);

        if (cliente is not null)
        {
            Clientes.Remove(cliente);
        }
    }
}