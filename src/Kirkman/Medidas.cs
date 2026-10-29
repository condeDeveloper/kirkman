namespace Conde.Kirkman;

/// <summary>
/// As tabelas do README. Cada número sai de um método daqui e está preso num
/// teste; só entra aqui o que eu vi o programa imprimir.
/// </summary>
public static class Medidas
{
    public readonly record struct LinhaExistencia(int Pontos, bool CondicaoVale, bool TrioInteiro, long Sistemas, long Nos);

    public readonly record struct LinhaSimetria(int Pontos, long Sistemas, long SimetriasDeUm, long Produto, long Fatorial, int Classes);

    public readonly record struct LinhaEscala(int Meninas, int Dias, bool Conseguiu, long Nos);

    /// <summary>
    /// A busca em cada tamanho, contra a condição conhecida. A busca não
    /// consulta a condição: ela tenta e volta de mãos vazias.
    /// </summary>
    public static List<LinhaExistencia> Existencia(IEnumerable<int> tamanhos)
    {
        var linhas = new List<LinhaExistencia>();

        foreach (var v in tamanhos)
        {
            var busca = new Busca(v);
            var quantos = v <= 9 ? busca.Contar() : busca.Primeiro() is null ? 0 : -1;
            linhas.Add(new LinhaExistencia(
                v,
                Sistema.CondicaoDeKirkman(v),
                v * (v - 1) % 6 == 0,
                quantos,
                busca.Nos));
        }

        return linhas;
    }

    /// <summary>
    /// A contagem de sistemas rotulados vezes as simetrias de um deles, contra
    /// o fatorial do número de pontos. Dois caminhos que não se falam.
    /// </summary>
    public static List<LinhaSimetria> SimetriasTabela(IEnumerable<int> tamanhos)
    {
        var linhas = new List<LinhaSimetria>();

        foreach (var v in tamanhos)
        {
            var sistemas = Simetrias.Todos(v);
            if (sistemas.Count == 0)
            {
                continue;
            }

            var simetrias = Simetrias.Quantas(sistemas[0]);
            linhas.Add(new LinhaSimetria(
                v,
                sistemas.Count,
                simetrias,
                sistemas.Count * simetrias,
                Simetrias.Fatorial(v),
                Simetrias.Classes(v)));
        }

        return linhas;
    }

    /// <summary>A escala das colegiais, em cada tamanho que faz sentido.</summary>
    public static List<LinhaEscala> Escalas(IEnumerable<int> tamanhos)
    {
        var linhas = new List<LinhaEscala>();

        foreach (var v in tamanhos)
        {
            var passeio = new Passeio(v);
            var escala = passeio.Montar();
            linhas.Add(new LinhaEscala(v, passeio.Dias, escala is not null, passeio.Nos));
        }

        return linhas;
    }

    /// <summary>A escala de quinze meninas em sete dias, escrita por extenso.</summary>
    public static List<string> Quinze()
    {
        var passeio = new Passeio(15);
        var escala = passeio.Montar();
        if (escala is null)
        {
            return [];
        }

        var linhas = new List<string>();
        for (var dia = 0; dia < escala.Count; dia++)
        {
            var filas = escala[dia].Select(f => $"{Letra(f.A)}{Letra(f.B)}{Letra(f.C)}");
            linhas.Add($"dia {dia + 1}: {string.Join("  ", filas)}");
        }

        return linhas;
    }

    private static char Letra(int menina) => (char)('A' + menina);

    /// <summary>
    /// A escala vista como sistema triplo passa no juiz, e cada dia reparte
    /// todas as meninas. São as duas exigências, conferidas separadamente.
    /// </summary>
    public static (bool Sistema_, bool Dias, int Trios) Conferir(int meninas)
    {
        var passeio = new Passeio(meninas);
        var escala = passeio.Montar();
        if (escala is null)
        {
            return (false, false, 0);
        }

        var sistema = Passeio.ComoSistema(meninas, escala);
        return (sistema.Valido(), Passeio.DiasCompletos(meninas, escala), sistema.Trios.Count);
    }
}
