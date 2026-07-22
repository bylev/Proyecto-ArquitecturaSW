using TransGGP.Application.Services;
using TransGGP.Domain.Models;
using TransGGP.xUnit.Fakes;

namespace TransGGP.xUnit.Services;

public class ClienteServiceTests
{
    [Fact]
    public void RegistrarCliente_AgregaClienteCorrectamente()
    {
        // Arrange
        var repositorio = new FakeClienteRepository();
        var servicio = new ClienteService(repositorio);

        var cliente = new Cliente
        {
            Id = 1,
            Nombre = "Construcciones Verticales"
        };

        // Act
        var resultado = servicio.RegistrarCliente(cliente);

        // Assert
        Assert.Same(cliente, resultado);
        Assert.Single(repositorio.Clientes);
        Assert.Equal("Construcciones Verticales", repositorio.Clientes[0].Nombre);
    }

    [Fact]
    public void ObtenerPorId_CuandoExiste_RetornaCliente()
    {
        // Arrange
        var repositorio = new FakeClienteRepository();

        repositorio.Clientes.Add(new Cliente
        {
            Id = 5,
            Nombre = "Cliente de prueba"
        });

        var servicio = new ClienteService(repositorio);

        // Act
        var resultado = servicio.ObtenerPorId(5);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(5, resultado.Id);
        Assert.Equal("Cliente de prueba", resultado.Nombre);
    }

    [Fact]
    public void EliminarCliente_EliminaClienteExistente()
    {
        // Arrange
        var repositorio = new FakeClienteRepository();

        repositorio.Clientes.Add(new Cliente
        {
            Id = 10,
            Nombre = "Cliente a eliminar"
        });

        var servicio = new ClienteService(repositorio);

        // Act
        servicio.EliminarCliente(10);

        // Assert
        Assert.Empty(repositorio.Clientes);
        Assert.Null(servicio.ObtenerPorId(10));
    }
}