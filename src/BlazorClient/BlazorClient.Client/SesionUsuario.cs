namespace BlazorClient.Client;

// Temporal: guarda el Id del usuario con el que se está trabajando.
// Cuando exista el login, este valor saldrá del usuario autenticado.
public class SesionUsuario
{
    public int IdUsuario { get; set; } = 1;
}
