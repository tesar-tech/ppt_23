// See https://aka.ms/new-console-template for more information


int mysleneCislo = VymysliSiCislo();

while (true)
{
    Console.WriteLine("Hádej číslo:");
    string? vstup = Console.ReadLine();
    if (vstup == null) return;

    if (vstup == "konec")
        break;

    if (!int.TryParse(vstup, out int hadaneCislo))
    { 
        Console.WriteLine("Není to číslo, piš číslo!!");
        continue;
    }

    if (hadaneCislo == mysleneCislo)
    {
        Console.WriteLine($"Ano, hadane cislo je {hadaneCislo}");
        Console.WriteLine($"Hrát znovu? (A/N)");
        string? hratZnovu = Console.ReadLine();
        if (hratZnovu != null && hratZnovu.ToLower() == "a")
        {
            Console.Clear();
            mysleneCislo = VymysliSiCislo();
        }
        else
            break;
    }
    string mensiNeboVetsi = hadaneCislo < mysleneCislo ? "MENŠÍ" : "VĚTŠÍ";
    Console.WriteLine($"Tvoje cislo je {mensiNeboVetsi} než myšlené číslo. Zkus to znovu");

}

Console.WriteLine("Díky za hru");
Console.ReadLine();

int VymysliSiCislo() => Random.Shared.Next(99) + 1;




