namespace Conde.Kirkman;

/// <summary>
/// O problema das colegiais, que Kirkman publicou em 1850: quinze meninas saem
/// em fila de três, cinco filas por dia, sete dias seguidos, e <b>duas delas
/// nunca andam juntas duas vezes</b>.
///
/// É um sistema triplo de Steiner com uma exigência a mais: os 35 trios têm que
/// se repartir em 7 <b>dias</b>, e cada dia precisa ser uma repartição completa
/// das quinze meninas em cinco trios sem ninguém de fora e ninguém repetido.
///
/// Um sistema com essa propriedade é chamado de resolúvel, e nem todo sistema
/// triplo é. Aqui os dias são montados direto: cada dia é escolhido inteiro
/// antes do seguinte, o que impede a mesma solução de aparecer em ordens
/// diferentes dentro de um dia.
/// </summary>
public sealed class Passeio
{
    private readonly int _meninas;
    private readonly int _porFila;
    private readonly bool[,] _juntas;
    private readonly List<List<(int A, int B, int C)>> _dias = [];

    public long Nos { get; private set; }

    public int Dias => (_meninas - 1) / (_porFila - 1);

    public Passeio(int meninas, int porFila = 3)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(meninas, 3);
        ArgumentOutOfRangeException.ThrowIfLessThan(porFila, 2);
        if (meninas % porFila != 0)
        {
            throw new ArgumentException("as meninas precisam caber em filas inteiras", nameof(meninas));
        }

        _meninas = meninas;
        _porFila = porFila;
        _juntas = new bool[meninas, meninas];
    }

    /// <summary>
    /// A primeira escala que funciona, ou nulo se não houver nenhuma. Cada
    /// elemento da lista é um dia, e cada dia é a lista das filas.
    /// </summary>
    public List<List<(int A, int B, int C)>>? Montar()
    {
        Nos = 0;
        _dias.Clear();
        Array.Clear(_juntas);
        return Dia(0) ? _dias.Select(d => d.ToList()).ToList() : null;
    }

    private bool Dia(int dia)
    {
        if (dia == Dias)
        {
            return true;
        }

        _dias.Add([]);
        var ocupada = new bool[_meninas];

        // A primeira menina de cada dia e fixada na fila que vem primeiro: as
        // filas de um dia nao tem ordem, e permitir todas as ordens multiplicaria
        // o trabalho por 5! sem achar nada novo.
        if (Fila(ocupada, 0))
        {
            return true;
        }

        _dias.RemoveAt(_dias.Count - 1);
        return false;
    }

    private bool Fila(bool[] ocupada, int dia)
    {
        Nos++;
        var primeira = -1;
        for (var m = 0; m < _meninas; m++)
        {
            if (!ocupada[m])
            {
                primeira = m;
                break;
            }
        }

        if (primeira < 0)
        {
            return Dia(_dias.Count);
        }

        ocupada[primeira] = true;

        for (var b = primeira + 1; b < _meninas; b++)
        {
            if (ocupada[b] || _juntas[primeira, b])
            {
                continue;
            }

            for (var c = b + 1; c < _meninas; c++)
            {
                if (ocupada[c] || _juntas[primeira, c] || _juntas[b, c])
                {
                    continue;
                }

                ocupada[b] = true;
                ocupada[c] = true;
                Marcar(primeira, b, c, true);
                _dias[^1].Add((primeira, b, c));

                if (Fila(ocupada, dia))
                {
                    return true;
                }

                _dias[^1].RemoveAt(_dias[^1].Count - 1);
                Marcar(primeira, b, c, false);
                ocupada[b] = false;
                ocupada[c] = false;
            }
        }

        ocupada[primeira] = false;
        return false;
    }

    private void Marcar(int a, int b, int c, bool ligar)
    {
        _juntas[a, b] = ligar;
        _juntas[b, a] = ligar;
        _juntas[a, c] = ligar;
        _juntas[c, a] = ligar;
        _juntas[b, c] = ligar;
        _juntas[c, b] = ligar;
    }

    /// <summary>
    /// A escala vista como sistema triplo: os trios de todos os dias juntos.
    /// Ela tem que passar no juiz, e é isso que liga este arquivo ao resto.
    /// </summary>
    public static Sistema ComoSistema(int meninas, List<List<(int A, int B, int C)>> escala)
    {
        ArgumentNullException.ThrowIfNull(escala);
        return new Sistema(meninas, escala.SelectMany(d => d));
    }

    /// <summary>Todo dia reparte todas as meninas, sem sobra e sem repetida.</summary>
    public static bool DiasCompletos(int meninas, List<List<(int A, int B, int C)>> escala)
    {
        ArgumentNullException.ThrowIfNull(escala);
        foreach (var dia in escala)
        {
            var vistas = new HashSet<int>();
            foreach (var (a, b, c) in dia)
            {
                if (!vistas.Add(a) || !vistas.Add(b) || !vistas.Add(c))
                {
                    return false;
                }
            }

            if (vistas.Count != meninas)
            {
                return false;
            }
        }

        return true;
    }
}
