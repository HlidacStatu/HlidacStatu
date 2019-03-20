# Knihovna DatasetConnector

[![NuGet](https://img.shields.io/nuget/dt/HlidacStatu.Api.Dataset.Connector.svg)](https://www.nuget.org/packages/HlidacStatu.Api.Dataset.Connector)
[![NuGet](https://img.shields.io/nuget/v/HlidacStatu.Api.Dataset.Connector.svg)](https://www.nuget.org/packages/HlidacStatu.Api.Dataset.Connector)

Knihovna pro .NET Framework napsaná ve standardu 1.6 umožňující snadnou práci s Datasety Hlídače státu.

## Použití knihovny

### Vytvoření instance konektoru

Pro vytvoření instance konektoru je nutné znát Váš `ApiToken`, který lze získat po příhlášení na stránkách [Hlídače Státu - API pro vývojáře](https://www.hlidacstatu.cz/api/v1/Index). 

```
    IDatasetConnector datasetConnector = new DatasetConnector(ApiToken);
```

Jakmile máme instanci vytvořenou, je potřeba definovat vlastní dataset.

### Definice datasetu

Dataset je v knihovně popsaný třídou [Dataset.cs](DatasetConnector/Dataset.cs), která je generická, protože JSON schema se automaticky generuje na základě generického parametru. Generický parametr musí splňovat podmínku, že implementuje rozhranní `IDatasetItem`, který vyžaduje definici parametru `Id`. Jednotlivé parametry odpovídají definici datové sady a jejich popis lze nalézt v [dokumentaci](https://hlidacstatu.docs.apiary.io/#reference/datasety-rozsirene-datove-sady-hlidace-statu/datasety).

Jelikož definice datasetu se v průběhu práce s ním pravděpodobně měnit nebude, je možné ji mít v programu uloženou např. jako statickou proměnnou 

```
		public static Dataset<Rizeni> dataset = new Dataset<Rizeni>(
			name: "Insolvenční rejstřík",
			datasetId: "insolvencni-rejstrik",
			origUrl: "https://isir.justice.cz/isir/common/index.do",
			description: "Seznam dlužníků vedený v insolvenčním rejstříku, proti kterým bylo zahájeno insolvenční řízení po 1. lednu 2008 a nebyli z rejstříku vyškrtnuti dle § 425 insolvenčního zákona.",
			sourceCodeUrl: "https://github.com/rpliva/HlidacStatu-InsolvencniRejstrik",
			orderList: new string[,] { 
				{ "Soud", "Soud" }, 
				{ "Datum zahájení řízení", "ZahajeniRizeni" }, 
				{ "Jméno/název", "Nazev" }, 
				{ "Aktualní stav", "AktualniStav" }
			},
			betaVersion: false,
			searchResultTemplate: SearchResultTemplate,
			detailTemplate: DetailTemplate);
```

kde `Rizeni` definuje strukturu datasetu (v tomto případě insolvenčního rejstříku) a proměnné `SearchResultTemplate` a `DetailTemplate` definují šablony použité pro vykreslení seznamu výsledků vyhledávání a detailu položky datasetu. Více informací k definici šablony lze nalézt v popisu API - [HTML Template](https://hlidacstatu.docs.apiary.io/#reference/html-teplate-syntaxe,-funkce).

Vlastnosti `datasetId` a generované `JsonSchema` se nesmí měnit. Při nutnosti jejich změny je potřeba nejprve starý dataset odstranit a poté vytvořit nový se změněnými hodnotami.

### Ověření existence datasetu

Pro ověření, zda dataset již existuje, slouží metoda `DatasetExists`, která jako parametr přijímá definici datasetu a vrací `true`, pokud je daný dataset již zaregistrován v Hlídači státu, jinak vrací `false`.

```
    var datasetExists = await datasetConnector.DatasetExists(dataset)
```

### Vytvoření nového datasetu

Nový dataset se v Hlídači státu vytvoří (zaregistruje) voláním metody `RegisterDataset`, která jako parametr přijímá definici datasetu a vrací id datasetu v Hlídači státu (`datasetId` není povinná položka a pokud není vyplněna, Hlídač státu ji automaticky odvodí z názvu datasetu, tato hodnota je následně vrácena. Pokud je hodnota `datasetId` vyplněna, je použita při registraci a vrácena).

```
    var datasetId = await datasetConnector.RegisterDataset(dataset);
```

Pokud již vytvářený dataset existuje, je vyvolána výjimka `DatasetConnectorException`.

### Změna definice datasetu

Existující dataset lze upravit kromě hodnot `datasetId` a `JsonSchema` (pro jejich změnu je nutné nejprve dataset smazat a následně vytvořit nový). Úprava datasetu se provede voláním metody `UpdateDataset`, která jako parametr přijímá definici datasetu a vrací string `Ok`, pokud se dataset podařilo upravit.

```
    var updateResult = await datasetConnector.UpdateDataset(changedDataset);
```

Pokud upravovaný dataset neexistuje, je vyvolána výjimka `DatasetConnectorException`.

### Smazání datasetu

Definici datasetu, včetně všech nahraných záznamů, lze odstranit voláním metody `DeleteDataset`, která jako parametr přijímá definici datasetu.

```
    await datasetConnector.DeleteDataset(dataset);
```

Pokud upravovaný dataset neexistuje, je vyvolána výjimka `DatasetConnectorException`.

### Přidání (změna) záznamu do datasetu

Záznam se do datasetu přidá voláním metody `AddItemToDataset`, která jako první parametr přijímá definici datasetu a jako druhý parametr přidávaný záznam. V případě, že záznam na základě `Id` v datasetu již existuje, bude nahrazen nově vkládaným záznamem. Návratová hodnota metody je `Id` záznamu.

```
    var result = await datasetConnector.AddItemToDataset(dataset, rizeni)
```
