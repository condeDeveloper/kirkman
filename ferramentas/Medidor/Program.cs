using Conde.Kirkman;

if (args.Length == 0)
{
    Console.WriteLine("uso: medidor <existencia|simetrias|escalas|quinze>");
    return 1;
}

switch (args[0])
{
    case "existencia":
        Existencia();
        break;
    case "simetrias":
        SimetriasTabela();
        break;
    case "escalas":
        Escalas();
        break;
    case "quinze":
        Quinze();
        break;
    default:
        Console.WriteLine($"comando desconhecido: {args[0]}");
        return 1;
}

return 0;

static void Existencia()
{
    Console.WriteLine("A busca em cada tamanho, contra a condicao conhecida");
    Console.WriteLine("(sistemas = -1 quer dizer que a busca so procurou o primeiro)");
    Console.WriteLine("pontos  v mod 6 e 1 ou 3  trios inteiros  sistemas  nos");
    foreach (var l in Medidas.Existencia([3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15]))
    {
        Console.WriteLine($"{l.Pontos,-8}{(l.CondicaoVale ? "sim" : "nao"),-17}{(l.TrioInteiro ? "sim" : "nao"),-16}{l.Sistemas,-10}{l.Nos}");
    }
}

static void SimetriasTabela()
{
    Console.WriteLine("Os sistemas rotulados contra as simetrias de um deles");
    Console.WriteLine("pontos  sistemas  simetrias  produto  v!       classes");
    foreach (var l in Medidas.SimetriasTabela([3, 7, 9]))
    {
        Console.WriteLine($"{l.Pontos,-8}{l.Sistemas,-10}{l.SimetriasDeUm,-11}{l.Produto,-9}{l.Fatorial,-9}{l.Classes}");
    }
}

static void Escalas()
{
    Console.WriteLine("A escala das colegiais");
    Console.WriteLine("meninas  dias  conseguiu  nos");
    foreach (var l in Medidas.Escalas([3, 9, 15]))
    {
        Console.WriteLine($"{l.Meninas,-9}{l.Dias,-6}{(l.Conseguiu ? "sim" : "NAO"),-11}{l.Nos}");
    }

    Console.WriteLine();
    foreach (var meninas in new[] { 9, 15 })
    {
        var (sistema, dias, trios) = Medidas.Conferir(meninas);
        Console.WriteLine($"{meninas} meninas: {trios} trios, e sistema de Steiner: {(sistema ? "sim" : "NAO")}, dias completos: {(dias ? "sim" : "NAO")}");
    }
}

static void Quinze()
{
    Console.WriteLine("As quinze colegiais de Kirkman, sete dias");
    foreach (var linha in Medidas.Quinze())
    {
        Console.WriteLine(linha);
    }
}
