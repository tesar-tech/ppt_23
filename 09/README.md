# 09 - Propojení aplikací na Azure a GH pages, appsettings, ASPNETCORE_ENVIRONMENT

Nyní je API ve stavu, že je schopno nám vrátit požadavek. Nejsou nastavení CORS (resp. jsou nastaveny na localhost, cože je nám v pordukci k ničemu). Požadavek je vyřízen jen přes doménu API.

Nyní potřebujeme vyměnit nastavený origin localhostu (`policy.WithOrigins("https://localhost:1111")`) za origin gh pages.

Chceme však neustále mít aplikaci funkční i na localhostu. Ke změně libovolného nastavení při změně prostředí (`ASPNETCORE_ENVIRONMENT`) slouží soubory appsettings.json.

## Přenastavení CORS

- Aktuálně máme CORS nastavený tak, aby propouštěl požadavky z domény localhostu: 

  ```csharp
  builder.Services.AddCors(corsOptions => corsOptions.AddDefaultPolicy(policy =>
      policy.WithOrigins("https://localhost:1111")
  ```

- Nicméně klientská aplikace nám běží na github doméně `https://tesar-tech.github.io`
- Jak rozlišit, že je aplikace nasazená (a povolit origin githubu.io) nebo, že je ve vývoji a povolit origin localhostu? 
- Aplikace to dokáže rozlišit pomocí `ASPNETCORE_ENVIRONMENT`. Napříkad zde:
  - swagger použijeme jenom při vývoji

    ```csharp
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    ```

- Jak aplikace pozná, že je v `Development`?
  - řekli jsme jí to v launchSettings.json :  `"ASPNETCORE_ENVIRONMENT": "Development"`
  - dalším prostředím je `Staging` a `Production`.
- Nasazené aplikaci "vnutíme" Production.
  - launchSetting.json se při publikování aplikace na server nedostane
  - řekneme jí to v Azure s pomocí konfigurace (tento krok dělat nemusíme, jelikož `Production` je [defaultním nastavením](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-6.0#azure-app-service))
   ![](media/msazureconfig.png) 
  - Nyní můžeme používad odbočku `app.Environment.IsProduction()`
  - Nicméně je lepší používat appsettings.json konfiguraci:

## appsettings.json

- v api projektu vidíte soubor appsettings.json a appsettings.Development.json
- Fungují tak, že dle prostředí se použije dané nastavení (například Development) a když některé nastavení není v Development, použije se to z appsettings.json.
- Tedy do appsettings.json dáváte konfiguraci, která je nezávislá na prostředí.
- Vytvořte produkční app settings a umístěte tam konfiguraci `CorsAllowedOrigin` s hodnotou vaší github pages domény. (origin je bez cesty a lomítka na konci)
- To stejné udělejte pro development.
- Konfiguraci si v Program.cs vyzvednete pomocí: `var corsAllowedOrigin = builder.Configuration.GetSection("CorsAllowedOrigins").Get<string[]>();`
  - vyzkoušejte, že vám to funguje

### Přenastavení adresy api na klientovi

- Stejný "problém" s natvrdo vepsaným nastavením máme i na straně klienta.
- Nastavujeme zde URI pro komunikaci s api.
- přidejte `appsettings.json` (a jeho `Development` a `Production` varianty) do `wwwroot` složky.
  - přidejte patřičné nastavení. 
- jednu hodnotu typu string můžete nastavení vyzvednout pomocí: 


- Nastavte správné hodnoty do souborů `appsettings.Production.json` a `appsettings.Development.json`. Jak na klientovi, tak v api.

- Otestujte aplikace, poté je pushněte...
- Nyní by aplikace měla fungovat i nasazená

## Některé nedostatky

- Build a nasazení aplikací je nezávislý proces. Může to dojít do nešťastné chvíle, kdy se změny projeví jen v jedné aplikaci (api nebo klient). Druhá se kvůli chybě třeba nesestaví.
  - Máme pak api nekompatibilní s klientskou aplikací do doby, než problém vyřešíme. To je špatně.
- Měl by existovat Staging. Stejná aplikace (se stejným nasatavením) na azure s vlastní databází, kde se celý proces otestuje (a k tomu používat `appsettings.Staging.json`). Tady pro zjednodušení máme rovnou produkci.
- Mělo by existovat víc větví, než jen jedna (master). Jedna zvlášť na vývoj, na testování, různé verze atd...
- Aplikace by měla obsahovat automatizované testování. Testy se pak spouští před každým nasazením aplikace, díky tomu se odhalí část problémů (které by se jinak odhalili až za běhu).
  - Se složitostí aplikace roste potřeba testů (vývojáři si přestávají pamatovat co kde je s čím propojené a co se kde v důsledku této a této změny může pokazit...)
- Aktuálně využívaná App service na azure startuje pomalu. Aplikace běží zdarma a jedním z omezení je i pomalý studený start.