# 11 SQLite v MS Azure App service

Posledním krokem k rozběhání aplikace "v internetech" je nasazení SQLite databáze.

V principu by to němlo být složité. SQLite databáze je stavěná tak, aby nepotřebovala žádný zvláštní server. Jenom si tak "sedí" vedl dllek a vy se k ní připojujete jako při vývoji.

I přes to je potřeba vyřešit několik věcí:

- Data musí být jiná než v databázi na vývoj. Nemůžeme mít databázi na vývoj a pokaždé ji pushnut přes git/nasadit na hosting. (v principu by to fungovalo, ale bylo by to k ničemu).
- Data v databázi musí zůstat i při nasazení nové verze aplikace.
- Musíme být schopni zařídit bezešvé migrování databáze v případě, že se objeví nové EF migrace.

Řešení:

- Databázi vyjmeme z repozitáře.
- Místo `dotnet ef database update` spustíme migraci v C# kódu:

  ```csharp
  app.Services.CreateScope().ServiceProvider
    .GetRequiredService<PptDbContext>()
    .Database.Migrate();
  ```

  - tímto se databáze i vytvoří v případě, že neexistuje.

Problém:

- Linuxový hosting na Azure App Service neumí pracovat s SQLite databází tak jak by bylo potřeba. Při snaze o vytvoření databáze vypisuje chybu zakončenou: `SQLite3 database is Locked`. Toto se děje v celé lokaci `/home` na stroji (dockeru), kde běží aplikace.
- Vysvětlení celé situace zde: https://stackoverflow.com/a/54051016/1154773

Řešení je několik možných:

- Využívat jinou službu pro skladování souborů. Azure Files nebo Azure Blob storage. Namountovat k tomu cestu přes Azure App Service. Správné řešení, ale bohužel školní licence neumožňuje tyto služby využívat. Resp. nejde je využívat zdarma.
- Druhé řešení: Dát databázi jinam než vedle dllek. Například do kořenového adresáře celého linuxového stroje (`/`). Není to ideální, ale naším účelům to postačí. Bohužel toto řešení neplní podmínku "data musí zůstat i po nasazení nové verze aplikace". Zkoušel jsem další možná řešení, ale na nic rozumného jsem nepřišel. Rozhodně to není production-ready. Nicméně nyní to takto uděláme.

## Jiná lokace databáze v produkci

Ve výsledku je to jednoduché.

- Vytvořte v appsettings položku, která rozliší mezi databází pro Developement a Production.
- Tu produkční hoďte do "/mojeDatabaze.db", tu dev nechte v "mojeDatabaze.db"

## Shrnutí

- Vyhoďte *.db soubory z gitu (přidejte do .gitignore)
- Migrujte db v C# kódu
- Změňte lokaci db pro produkci.

Vyzkoušejte přidání/upravení záznamu. Záznamy by měly vydržet jak přes refresh prohlížeče, tak při restartování aplikace, tak i při ~~nasazení nové verze aplikace~~.
