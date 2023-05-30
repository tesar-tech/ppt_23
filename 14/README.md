# 14 - Test 02 - Pracovníci

## [0.3] Tabulka pracovníků

Přidejte do databáze tabulku pracovníků

- Pracovníci jsou zodpovědní za úkon. Například: Radiologický asistent, který provedl CT vyšetření. Doktor, který při operaci využil elektrokauter.
- Vždy pouze jedna osoba na daný úkon
- Stále je však možné mít úkon bez pracovníka
- Nezapomeňte upravit i tabulku úkonů a přidat migraci

## [0.3] Seed pracovníků

- naplňte tabulku pracovníků v databázi
- předpokládá se, že máte hotový seed pro úkony. Pokud nikoliv, inspirujte se v ppt_23 repu.
- některé úkony budou mít pracovníka, některé nikoliv.

## [0.3] Pracovník do výpisu úkonů

Přidejte pracovníka (stačí jeho jméno) do výpisu úkonů (detail vybavení).

- Na vhodném místě budete muset použít metodu `ThenInclude` (používá se tam, kde chceme includnout na entitě, která je již inlcudnutá)
- Nepotřebujete vytvářet `PracovnikVm`, stačí když na vhodné místo přidáte propertu `PracovnikName`, mappster to pochopí...
- v případě, že není přiřazen pracovník vypiště "žádný pracovník"

## [0.1] Zelený úkon

- v případě, že je pracovník u úkonu zobrazte název (kód) úkonu zelenou barvou.

## [0.3] BONUS - Odebrání pracovníka

- Přidejte tlačítko na odebrání pracovníka
- Implementujte endpoint na adrese `/ukon/{id:guid}/removepracovnik`
  - endpoint bude HTTP PATCH. Nebude to DELETE jelikož žádnou entitu nemažeme (kdybychom smazali pracovníka, smazali bychom ho ze všech úkonů). 
  Nebude to PUT, jelikož pouze upravujeme část nějaké entity (v tomto případě úkonu, což by vám mělo napovědět jak pracovníka smazat)
  - upravte patřičně CORS
- V Blazoru vhodně endpoint zavolejte a navázejte na to patřičnou akci (úprava vybavení na klientovi)

## Fragmenty kódu, které se vám mohou hodit

```csharp
.Include(x=>x.Ukons).ThenInclude(x=>x.Pracovnik)
//
Random.Shared.Next(0, 100) % 2 == 1;
//
int toSkip = Random.Shared.Next(0, _db.Pracovniks.Count());
var pracovnik =_db.Pracovniks.Skip(toSkip).Take(1).First();

```