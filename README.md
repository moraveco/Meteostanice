# Meteostanice

Aplikace je vytvořena jako .NET 10 Web API s Worker Service na pozadí. Její hlavní zodpovědností je pravidelné stahování XML dat z meteostanice, jejich převod do formátu JSON a následné ukládání do SQLite databáze. Aplikace automaticky řeší i situace, kdy je zdrojová URL nedostupná, a v takovém případě uloží záznam o nedostupnosti. Pro vizualizaci nasbíraných dat obsahuje také jednoduchý webový dashboard.

### Požadavky pro spuštění

Pro spuštění projektu je nutné mít nainstalovanou jednu z následujících technologií:

* Docker a Docker Compose
* .NET 10 SDK (pro lokální spuštění)

### Konfigurace

Konfigurační soubory nejsou součástí repozitáře. Před spuštěním je nutné je vytvořit ručně ve složce `Meteostanice/`.

Vytvořte soubor `Meteostanice/appsettings.json`:

```json
{
  "MeteoSettings": {
    "XmlUrl": "[ZDE VLOŽTE XML URL]",
    "IntervalMinutes": 60
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=meteo.db"
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/meteo-.log",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

Vytvořte soubor `Meteostanice/appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

### Jak aplikaci spustit

Nejsnazší způsob spuštění je pomocí nástroje Docker. V kořenovém adresáři repozitáře stačí zadat příkaz:

```bash
docker compose build
docker compose up -d
```

Po úspěšném spuštění kontejneru se zpřístupní webový dashboard na adrese http://localhost:8080/dashboard.

Pokud chcete aplikaci spustit lokálně bez použití Dockeru, přejděte do složky Meteostanice a aplikaci spusťte:

```bash
cd Meteostanice
dotnet run
```

Konfiguraci adresy meteostanice a intervalu stahování lze upravit v souboru `appsettings.json` nebo pomocí proměnných prostředí.

### Časová náročnost

Implementace tohoto zadání, včetně přípravy Docker kontejneru a základního webového dashboardu, zabrala přibližně 3 hodiny.