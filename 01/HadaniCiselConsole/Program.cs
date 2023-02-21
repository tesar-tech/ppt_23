
Console.WriteLine("Ahoj vítej ve hře");


int mysleneCislo = NahodneCislo();

while (true)
{
    Console.Write("Hadej cislo:");
    string? vstup = Console.ReadLine();

    if (vstup == "konec")
        break;

    if (!int.TryParse(vstup, out int hadaneCislo))
    {
        Console.WriteLine("Napiš číslo");
        continue;
    }

    string vetsiNeboMensi = hadaneCislo < mysleneCislo ? "menší" : "větší";

    Console.WriteLine($" Tvoje cislo je {vetsiNeboMensi} nez myslene");

    if (hadaneCislo == mysleneCislo)
    {
        Console.WriteLine($"Uhadl jsi cislo {hadaneCislo}");
        Console.WriteLine("Chces znovu (A/N)?");


        string? aNn = Console.ReadLine();
        
        if (aNn?.ToLower() == "a")
        {
            Console.Clear();
            mysleneCislo = NahodneCislo();
        }
        else
            break;

    }

}



int NahodneCislo() => Random.Shared.Next(1, 100);








