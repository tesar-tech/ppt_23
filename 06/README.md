# 06 - Konzumace API (GET), HttpClient, DI/IoC, CORS

Nyní můžeme API dotazovat pomocí swaggeru. Tuto lekci propojíme klientskou aplikaci s api, tak aby mezi nimi docházelo ke komunikaci.

## Doplnění

- Automapper
- vracení 1 vybavení vs všech
- Results.Json, IResult
- Lambda zápis a return jednoho řádku

## launchSettings.json

- ve složce Properties
- specifikuje jak se aplikace spouští
- změňte soubor tak aby obsahoval jediný profil a to https (ostatní nejsou pro nás potřeba)
- také změňte adresu, klienta na port 1111, server na port 2222 (https)
- kompletní `launchSettings.json` pro klienta:

```json
{
  "profiles": {
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "vybaveni",
      "inspectUri": "{wsProtocol}://{url.hostname}:{url.port}/_framework/debug/ws-proxy?browser={browserInspectUri}",
      "applicationUrl": "https://localhost:1111",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

## Přidání HTTP klienta v Blazoru

V souboru `Program.cs` je už přidaný klient `HttpClient` do IoC kontejneru. Díky tomu můžeme použít třídu HttpClient aniž bychom se museli starat o její inicializaci a celý životní cyklus (to je IoC - **Inversion of Control**). (Má to další výhody, zejména při automatizovaném testování - o tom více v dalších lekcích).

Defaultně je adresa http klienta nastavena na stejnou doménu jako jako je běžící aplikace. Jelikož máme aplikaci rozdělenou, můusíme zde specifikovat adresu se kterou bude Blazor aplikace komunikovat. Je to adresa našeho web-api.

```csharp
//👇 nahraďte adresou api projektu, například "https://localhost:7058" (bez lomítka na konci !!!)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
```

Nyní si vytvoříme proměnnou `Http` typu `HttpClient` a "nainjectujeme" jí tuto závislost (DI - **Dependency Injection**). Všiměte si, že nikde nevytváříme daný objekt `HttpClient` (něco jako `new HttpClient()...`), o to se za nás postará kontejner (na pozadí). Kdybychom si o klienta požádali na jiné stránce, bude nám vrácen stejný objekt (v závislost na nastavení AddScoped).

```csharp
//Vybaveni.razor - html část
@inject HttpClient Http
//nebo
//Vybaveni.razor - c# část
[Inject] public HttpClient Http { get; set; } = null!;
```

## Povolení přístupu k api klientské aplikaci - CORS

V defaultním nastavení je z důvodu bezpečnosti webové api schopné příjmat pouze požadavky ze stejné domény, na které samo běží. V jiných případech (v tom našem), je nutné definovat CORS (Cross Origin Resource Sharing). Tedy nastavení, které uvolní defaultní restriktivní chování. [Cors in 100 Seconds](https://www.youtube.com/watch?v=4KHiSt0oLJ0)

```csharp
builder.Services.AddCors(corsOptions => corsOptions.AddDefaultPolicy(policy =>
    policy.WithOrigins("adresa klientské aplikace")//👈
    .WithMethods("vypište", "použité", "http", "metody")//👈 (musí být UPPERCASE)
    .AllowAnyHeader()
));
//někde za definicí proměnné app
app.UseCors();
```

Tímto jsme zařídili, že všechny požadavky s definovanými metodami přicházející z naší Blazor aplikace nebudou odmítnuty.

### Preflight requests

Z bezpečnostních důvodů musí být http metody PUT, PATCH a DELETE schváleny tzv. preflight requstem. Zjednodušeně: Před samotným požadavkem (například na smazání) se pošle preflight, jestli server toto umožňuje (jestli to umožňujě dané URL, která request poslala).

Sledujte záložku Network v F12.

## Http klient a GET

Místo vytvořeného seznamu vybavení získáme nyní data skrze API. K tomu slouží právě proměnná `Http`. V asynchronní ovrride metodě `OnInitializedAsync` zavoláme metodu `GetFromJsonAsync`. Toto je **generická** metoda. A tím generickým parametrem (<ve špičatých závorkách>) je typ dat, které má vracet. Respektive datový typ, na který se snaží napasovat příchozí JSON.

```csharp
List<VybaveniModel>? seznamVybaveni;

protected override async Task OnInitializedAsync()
{
    seznamVybaveni = await Http.GetFromJsonAsync<List<VybaveniModel>>("vybaveni");
}
```

- Odstraňte jakoukoliv jinou inicializaci proměnné `seznamVyvabeni` a udělejte ji nullable.
- Někde z vrchu udělejte null check a v případě defaultní hodnoty (null), zobrazte hlášku "načítám". (a použijte return, aby se zbytek kódu neprovedl).
- BONUS: Zobrazte načítací animaci:
  
  ```html
  <div class="absolute right-1/2 bottom-1/2  transform translate-x-1/2 translate-y-1/2 ">
    <div class=" border-t-transparent border-solid animate-spin  rounded-full border-teal-600 border-8 h-16 w-16"></div>
  </div>
  ```


## Http klient a DELETE

- Na stránce `Vybaveni.razor` změňte akci, která se odehrává po zavolání callbacku `DeleteItemCallBack`.
- Zavolejte metodu `DeleteVybaveni`. Přijme právě položku, kterou má smazat.
- Na Http klientovy existuje metoda `DeleteAsync`
  - Zadejte ji cestu, včetně parametrů, které api očekává
  - Jejím výstupem je `HttpResponseMessage` s propertou `IsSuccessStatusCode`, použijte ji a v případě úspěšného smazání (skrz api) odstraňte položku ze seznamu na klientovi.

## Http klient - PUT

- V principu jsou akce POST a PUT dosti podobné. Posílá se celý model.
- Volá se na callbacku `EditDoneCallback`.
- Model specifikujete v druhém parametru metody `PutAsJsonAsync`.
- Nyní nemáme mechanismus, který by při chybě na straně api vrátil provedené změny
  - Stačilo by uživatele informovat a doporučit obnovení stránky.

## Http klient a POST

- V případě POSTu je nutné vyzvednout přichozí Id nově vytvořené entity a přiřadit ho.
  - (Id musíme mít, kdybychom ho chtěli třeba rovnou smazat)
- `PostAsJsonAsync`  také vrací `HttpResponseMessage` a krom status kódu má ještě vlastnost `Content`.
  - Content obsahuje metodu pro čtení JSONu a převedení na požadovaný typ (`ReadFromJsonAsync`).
  - Formát je generický parametr (špičaté závorky)

## Endpoint vybaveni/{Id}

Tento endpoint využijeme na nové stránce, která bude zobrazovat podrobnější detail jednoho vybavení.
Aktuálně žádný takový detail nemáme, ten doplníme posléze. Bude to seznam oprav, které připadají k danému vybavení. Teď na stránce pouze vypíšeme vlastnosti jednoho vybavení (jméno, cenu,...)

[route parameters](https://blazor-university.com/routing/route-parameters/)

- Přidejte stránku s názvem `VybaveniDetail`.
  - route nastavte na "/vybaveni/{Id:guid}", nezapomeňte na direktivu `page`
  - přidejte parametr `Id`, parametr bude nastaven právě z routy.
- Do tabulky přidejte odkaz, který bude odkazovat na stránku detailu daného Id.

## Stránka detailu

- Na stránce vytvoříme jednoduchý grid, který zobrazí všechny detaily.

    ```html
    <div class="grid grid-cols-2"> 
    <div>Název</div>
    <div>@item.Name</div>
    </div>
    ```

- V metodě `OnInitializedAsync` načtěte data z API.
