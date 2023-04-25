# 10 -  Databáze, SQLite, EntityFramework, Mapster, Migrace

## Multiple startup projects

- Pro spuštění 2 projektů ve vs: Properties na .sln:

![](media/multipleprojs.png)

## Databáze a EntityFramework

- Databáze slouží k perzistentnímu uložení dat. Aktuálně máme "databázi" v proměnné, která drží stejná data pouze po dobu běhu aplikace.
- Přidání databáze uděláme tím nejjednodušším způsobem. Databáze bude jeden soubor (s příponou `.db`). Není nutné provozovat žádný server.
  - Nejpopulárnějším způsobem jak mít databázi jako jednoduchý soubor je SQLite.
- Dále využijeme abstrakce nad databází pomocí EntityFrameworku. Databázi nebudeme dotazovat pomocí SQL, ale pomocí C#.
  - EF se postará o překlad ze C# do SQL.
- Vztah mezi C# a SQL databází je přibližně takový:
  - Definice C# třídy: definice databázové tabulky (Například `public class Vybavení`)
  - Vlastnost v třídě: Sloupeček v tabulce (Například `public string Název {get; set;}`)
  - Vytvořený objekt dané C# třídy: Řádek v tabulce

### Přidání NuGet balíčku pro práci EF s SQLite databází

- (práce v api projektu)

- (opt1) V terminálu: `dotnet add package Microsoft.EntityFrameworkCore`, nebo
- (opt2) V Package Manageru (okno ve VS): `Install-Package Microsoft.EntityFrameworkCore`, nebo
- (opt3) Manuálně v VS: pravým na projekt -> Manage Nuget Packages -> Najít a instalovat.

- To samé pro SQLite
  - `dotnet add package Microsoft.EntityFrameworkCore.Sqlite`

- Jenom se tím upraví .csproj soubor

## Konfigurace databáze a DbContext

- Pro zachování pořádku dáme všechny databázové věci do složky `Data`.

> Rychlý způsob: Vybrat Api projekt -> `Shift+F2` -> napsat `Data/` (/ je důležité)

> Stejně tak u souborů


- `PptDbContext.cs` - drží info o databázi (ze C# pohledu). Prozatím pouze podědí od třídy `:DbContext` (součástí namespace `Microsoft.EntityFrameworkCore`). A konstruktor přijme proměnnou `options` typu `DbContextOptions` (s generickým parametrem našeho DbContextu).
  - Tento parametr přijme i bázová třída `:base(options)` (tímto voláme při vytváření konstruktor bázové třídy (v tomto případě typu `DbContext`)).
- Přidáme `PptDbContext` jako servisu:
  
  ```csharp
  builder.Services.AddDbContext<PptDbContext>(opt => opt.UseSqlite("FileName=název databáze, musí končit: .db")); 👈
  ```

  - AddDbContext očekává generický parametr - jakýže to kontext se snažíme přidat. Dále můžeme specifikovat nastavení.
    - Že používáme SQLite řekneme právě zde. Kdybychom chtěli použít jiný databázový engine, bude stačit změna tohoto řádku. (má to však ještě fůru eventualit...)
    - Do metody `UseSqlite` posíláme parametr connection string. Jediné co je potřeba specifikovat je název databáze.

### Modelové třídy (~tabulky)

- Ve složce `Data` vytvořte třídu `Vybaveni`
- Tato třída bude reprezentovat tabulku `Vybaveni`
- Již máme třídu, která reprezentuje datovou strukturu pro vybavení (`VybaveniModel`), nicméně je vhodné vytvořit třídu zcela novou:
  - Ve Vybaveni budou jen ty data, která se skutečně namapují do databáze.
  - Třídy databáze máme pouze na jednom místě a slouží pouze k tomuto účelu.
  - Nechceme odhalovat strukturu databáze vnějšímu světu.
  - Píšeme duplicitní kód (prop `Name` bude jak v Modelu ve `Vybaveni`, tak ve `VybaveniModel`), ale za ten pořádek to stojí.
  - Musíme zařídit mapování z a do databázových tříd (AutoMapper, viz níže).
- Do třídy `Vybaveni` přidejte jenom ty prop, které se namapují do databáze.

### Specifikace Vybavení jako DbSet (~tabulka)

- Ještě musíme nějakým způsobem specifikovat, že třídu `Vybaveni` chceme použít jako databázovou tabulku.
- Do `PptDbContext` přidejte vlastnost typu `DbSet<>`. Její název bude název tabulky. Generická část datového typu je typ, který chcete pro definici tabulky použít.
  - Není nutné specifikovat setter...
  - V getteru bude Set s generickým typem `Vybaveni`

### Databázové Migrace

- Slouží k synchronizaci stavu struktury (částečně i dat) databáze a C# kódu zodpovědného za její tvoření.
- V souborech si zaznamenáváte jak taková databáze vypadá a jak se mění.
- Například při přidání nové tabulky je vytvořena migrace, která tuto operaci zaznamená do C# kódu.
- Následně se C# provede a databáze se podle toho aktualizuje.
- Celý tento postup práce s EF se nachází pod pojmem Code-First.
  - Nejdříve vytvoříte C# kód a podle toho se synchronizuje databáze.
- Celé migrace jsou dobré, aby nedocházelo ke konfliktům mezi C# kódem a skutečnou databází.

#### Přidání iniciální migrace a vytvoření databáze

- Nejprve je potřeba nainstalovat nástroj pro práci s migracemi:
  - `dotnet tool install --global dotnet-ef`
- Přidáme první migraci, která zachytí stav databáze:
  - `dotnet ef migrations add "initial migration"`
  - Aplikace se sestaví a je přidána složka a třída migrace:

    ![](media/migrace_soubory.png)
    - Ve třídě jsou v C# kódu 2 metody: `Up` a `Down`, pro updatování databáze na tuto migraci a pro "odmigrování" migrace.
      - `Up` v tomto případě přidává tabulku. `Down` tabulku maže.
- Updatujeme stav databáze dle iniciální migrace:
  - `dotnet ef database update`
- Nyní se ve složce vytvořil soubor `mojeDatabaze.db`
- Tuto operaci (přidání migrace a update databáze) je nutné provést při každé změně databázových tříd.
  - Je to trochu prudérní, v předchozích verzích EF existovalo nastavení na automatické migrování...
    - Trochu to zkrátilo čas, který je nutné tomu nyní věnovat, nicméně to ve finále dopadlo tak, že se vývojář střelil do vlastní nohy...


#### Jak si prohlédnout databázi

- Existuje spousta jednoduchých programů na prohlížení SQLite databází.
  - Například tento [online](https://sqliteviewer.app)
  - nebo pro Windows: `winget install -e --id DBBrowserForSQLite.DBBrowserForSQLite` ([gh](https://github.com/sqlitebrowser/sqlitebrowser))
- Po otevření databáze to bude vypadat přibližně takto:

  ![](media/dbbrowser.png)
  - Povšimněte si, že datum je uloženo jako TEXT. O převod do C# `DateTime` se stará právě EF.

### Dotazování databáze

- Je to skoro stejné jako dotazování "databáze" z proměnné. (A v tom je celé kouzlo a přínos EF)
- Nicméně třída, kterou vytáhneme z db (`Vybaveni`) je jiná než třída, kterou chceme poslat v jsonu (`VybaveniViewModel`).


### Přidání všech sloupců do tabulky

- Přidejte všechny nezbytné "sloupečky" do "tabulky" vybavení 
- Migrujte a updatněte db

### Přidání prvku do databáze

- V post endpointu upravte kód, tak aby namapoval `prichoziModel` na proměnnou typu `Vybaveni` a tento objekt uložil do databáze:

```csharp
app.MapPost("/vybaveni", (VybaveniVm prichoziModel, PptDbContext _db) =>
{
    prichoziModel.Id = Guid.Empty;

    Vybaveni en = new() {
        Name = prichoziModel.Name, 
        BoughtDateTime = prichoziModel.BoughtDateTime,
        LastRevisionDateTime = prichoziModel.LastRevisionDateTime,
        PriceCzk = prichoziModel.PriceCzk};

    //přidat do db.Vybavenis
    //uložit db (db.Save)
    return prichoziModel.Id;
});

```

- Budete k tomu potřebovat specifikovat proměnné `PptDbContext db`
- Všimněte si, že Id tady nevymýšlíme, ale naopak nulujeme. O primární klíče se stará databáze sama, nemůžeme (v běžném procesu) ji klíče vnutit.
  - Správně by to mělo být tak, že VybaveniVm by neobsahoval vůbec prop Id (byl by jiného typu, například VybaveniModelPost). Z důvodu zjednodušení používáme VybaveniVm všude (tam kde je i tam kde není Id potřeba).

- Vyzkoušejte ve swaggeru.
- Předělejte všechny endpointy tak, aby pracovaly s db.
  - Nezapomeňte na ukládání databáze.

#### Mapster

- Abychom nemuseli ručně opisovat každou proměnnou a měnit kód při přidání nové proměnné, použijeme knihovnu [Mapster](https://github.com/MapsterMapper/Mapster), která se v jednom příkazu postará o zkopírování vlastností z jedné proměnné do druhé.
  - `dotnet add package Mapster`
- Přidejte mappster a celé mapovíní prichoziModel udělejte v jednom krátkém řádku..

## Dále

- Dodělejte všechny ostatní endpointy vybavení, tak aby využívaly databázi.
  - Využijte Mapster (i když to bude fungovat i bez něj (ale časem už moc nebude))
  - Vyčistěte kód od `seznamVybaveni` (už není potřeba).
- Přidejte tabulku (třídu) `Revize` do databáze.
  - Postupujte stejně jako v případě vybavení (vytvoření třídy, přidaní DbSetu, migrace,...)
- Endpoint revize (ten, který byl přidán v testu) upravte tak aby fungoval s daty z databáze
  - (žádná data tam ale zatím nejsou)
  - Odstrňte všechen zbytečný kód týkající se revizí.
- Dodělejte `Loader` komponentu (pokud nemáte).
- Stáhněte si zmíněný prohlížeč SQLite databáze (a nebo jakýkoliv jiný). Přidejte data do revizí, ať vám endpoint funguje.

