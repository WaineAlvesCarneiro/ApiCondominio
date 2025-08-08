namespace ApiCondominio.Infrastructure.SqlModels;

public class MoradorFlat
{
    // Dados do Morador
    public int id { get; set; }
    public string nome { get; set; } = null!;
    public string celular { get; set; } = null!;
    public string email { get; set; } = null!;
    public bool is_proprietario { get; set; }
    public DateTime data_entrada { get; set; }
    public DateTime? data_saida { get; set; }
    public DateTime data_inclusao { get; set; }
    public DateTime? data_alteracao { get; set; }
    public int imovel_id { get; set; }

    // Dados do Imovel
    public int imovelId { get; set; }
    public string bloco { get; set; } = null!;
    public string apartamento { get; set; } = null!;
    public string box_garagem { get; set; }
}

