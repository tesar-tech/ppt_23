
# 01 - Úvod, Hádání čísel, Blazor

## Obsah předmětu

> Vývoj, nasazení, testování a rozšiřování aplikací za účelem vyzkoušení standardního vývojového procesu. S důrazem na to jak programovat efektivně.

## Používané technologie

- .NET 7, C#
- ASP.NET Core 6
- Rest API, json, Swagger
- HTML/CSS
- Blazor Web Assembly
- SQL server, SQLite
- Entity Framework Core, Mappster
- Git
- GitHub
- Github Actions
- Visual Studio (Code)

## Obsah cvičení

- Opakování C#/.NET, seznámení s Blazorem
- Git, GitHub, markdown, github pages
- CI/CD (nasazení aplikace na pushnutí kódu)
- Blazor - komunikace komponent (binding, cascading values)
- Blazor - tvorba komponent, struktura projektu
- Css, tailwind
- Využití knihoven třetích stran (nuget)
- Tvorba vlastních knihoven, přidávání projektů
- ASP.NET core web api (serverová část aplikace)
- Přístup do databáze, její vytvoření a použití
- Entity framework - práce s databází
- Testování - Unit testy
- Testováni - integrační testy
- Autentifikace a autorizace, cookies, jwt, Registrace, Přihlášení, Resetování hesla…
- Nasazení aplikace - MS Azure

## Hádání čísel

- V `Program.cs` sestrojte jednoduchou hru hádání čísel.
- Program si vymyslí číslo v intervali 1-100
- Uživatel hádá číslo
- Program napovídá jestli je číslo větší nebo menší

Až budete mít základní funkcionalitu:

- dejte pozor ať je skutečně generované číslo od 1 do 100
- ověřte vstupy, tak aby hra uživatele upozornila, že nezadal číslo
- umožněte vstup `konec`, který hru ukončí
- po úspěšném odehrání dejte možnost hrát znovu
  - "generování" nového čísla dejte do metody (aby to bylo na jednom místě)
    - použijte lambda zápis
- vyvarujte se všech warningů 


## Blazor App

- Blazor je součástí asp.net core frameworku pro tvorbu webových aplikací. 
- Blazor umožňuje vytvářet SPA aplikace s pomocí C#. 
- (časem se dostaneme i k tvorbě backendu (api) části)
- vytvořit blazor appku:
- `dotnet new blazorwasm-empty -o "Ppt23.Client"`
  - blazorwasm-empty: typ projektu, ví které packages přidat do projektu, aby z toho byl Blazor
  - empty: méně "demo" věcí.
  - -o: název projektu (vytvoří složku projektu i soubor .csproj)
- tvorba .gitignore https://github.com/github/gitignore/blob/main/community/DotNet/core.gitignore
  - přidat ho do root složky (tam, kde je .git)
- puštění `dotnet watch`
- Změna textu Hello, World!
- Přidání stráky "HadaniCisel" pod url /hra
- Předělat konzolovou aplikaci do Blazoru

### Hádání čísel v Blazoru

- Pro zadávání čísel použijte `<input />`
- Do stránky přidejte sekci `code`
  
  ```razor
   @code
   {
    int cosi = 5;
   }
  ```

- přidejte tlačítko, kterým to odpálíte
  
  ```razor
  <button @onclick="Hadam">Hádám</button>
  ```

- přidjte metodu `Hadam`, kde kliknutí obsloužíte

- vytvořte proměnnou hadaneCislo
  - Propojte ji s inputem přidáním těchto vlstností `@bind-value=hadaneCislo @bind-value:event="oninput"`

- ...