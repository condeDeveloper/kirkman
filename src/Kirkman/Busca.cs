namespace Conde.Kirkman;

/// <summary>
/// A busca por sistemas triplos: sempre o <b>primeiro par ainda descoberto</b>
/// é atendido, e só os trios que o contêm são tentados.
///
/// Essa ordem não é uma heurística: ela é o que impede o mesmo sistema de ser
/// montado em ordens diferentes e contado várias vezes. Com ela, cada sistema
/// é achado exatamente uma vez.
///
/// A busca não sabe que existe condição de existência nenhuma. Ela tenta em
/// todo tamanho, e nos tamanhos em que não há resposta ela volta de mãos
/// vazias — que é como a condição aparece aqui, medida em vez de citada.
/// </summary>
public sealed class Busca
{
    private readonly int _pontos;
    private readonly bool[,] _coberto;
    private readonly List<(int A, int B, int C)> _atual = [];

    public long Nos { get; private set; }

    public Busca(int pontos)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(pontos);
        _pontos = pontos;
        _coberto = new bool[pontos, pontos];
    }

    /// <summary>Quantos sistemas existem sobre pontos rotulados.</summary>
    public long Contar()
    {
        Nos = 0;
        _atual.Clear();
        Array.Clear(_coberto);
        return Passo(null);
    }

    /// <summary>O primeiro sistema, ou nulo se não houver nenhum.</summary>
    public Sistema? Primeiro()
    {
        Nos = 0;
        _atual.Clear();
        Array.Clear(_coberto);
        return Monta() ? new Sistema(_pontos, _atual) : null;
    }

    private (int A, int B)? PrimeiroParDescoberto()
    {
        for (var a = 0; a < _pontos; a++)
        {
            for (var b = a + 1; b < _pontos; b++)
            {
                if (!_coberto[a, b])
                {
                    return (a, b);
                }
            }
        }

        return null;
    }

    private long Passo(object? _)
    {
        Nos++;
        var par = PrimeiroParDescoberto();
        if (par is null)
        {
            return 1;
        }

        var (a, b) = par.Value;
        long total = 0;

        for (var c = 0; c < _pontos; c++)
        {
            if (c == a || c == b || Coberto(a, c) || Coberto(b, c))
            {
                continue;
            }

            Marcar(a, b, c, true);
            total += Passo(null);
            Marcar(a, b, c, false);
        }

        return total;
    }

    private bool Monta()
    {
        Nos++;
        var par = PrimeiroParDescoberto();
        if (par is null)
        {
            return true;
        }

        var (a, b) = par.Value;

        for (var c = 0; c < _pontos; c++)
        {
            if (c == a || c == b || Coberto(a, c) || Coberto(b, c))
            {
                continue;
            }

            Marcar(a, b, c, true);
            if (Monta())
            {
                return true;
            }

            Marcar(a, b, c, false);
        }

        return false;
    }

    /// <summary>
    /// O par esta coberto? A matriz so usa a metade de cima, entao o indice
    /// MENOR vem sempre primeiro. Ler sem normalizar le a metade vazia, deixa
    /// marcar um par duas vezes, e o desfazer de um ramo apaga a marca do
    /// outro: a busca volta a um estado que ela achava que ja tinha passado e
    /// nunca mais termina. Foi assim que a primeira versao estourou a pilha.
    /// </summary>
    private bool Coberto(int a, int b) => _coberto[Math.Min(a, b), Math.Max(a, b)];

    private void Marcar(int a, int b, int c, bool ligar)
    {
        _coberto[Math.Min(a, b), Math.Max(a, b)] = ligar;
        _coberto[Math.Min(a, c), Math.Max(a, c)] = ligar;
        _coberto[Math.Min(b, c), Math.Max(b, c)] = ligar;

        if (ligar)
        {
            _atual.Add((a, b, c));
        }
        else
        {
            _atual.RemoveAt(_atual.Count - 1);
        }
    }
}
