# RomBooking - Booking System

Et komplett rombooking-system bygget i ASP.NET Core MVC.

## Funksjoner

- ✅ Brukerregistrering og innlogging
- ✅ Booking av rom på time/halvtime-basis
- ✅ Åpningstider: 08:00 - 16:00
- ✅ Automatisk blokkering av helligdager (norske)
- ✅ Automatisk blokkering av helg
- ✅ 5 rom inkludert (kan legge til flere)
- ✅ Oversikt over ledige/opptatte tider
- ✅ Mine bookinger-side
- ✅ Admin-panel for romadministrasjon
- ✅ Enkel å style med egen CSS

## Installasjon og kjøring

### Forutsetninger
- .NET 8.0 SDK eller nyere
- Visual Studio 2022 eller VS Code

### Steg 1: Last ned prosjektet
Pakk ut zip-filen til ønsket mappe.

### Steg 2: Åpne i Visual Studio
1. Dobbeltklikk på `RomBooking.sln` (hvis du bruker kommandolinje, hopp til steg 3)
2. Trykk F5 for å kjøre

### Steg 3: Kjøre fra kommandolinje
```bash
cd RomBooking
dotnet run
```

Applikasjonen vil starte på https://localhost:5001 (eller http://localhost:5000)

### Steg 4: Første gangs oppsett
1. Åpne nettleseren på https://localhost:5001
2. **Administrator-bruker er allerede opprettet:**
   - **E-post:** admin@rombooking.no
   - **Passord:** Admin123!
3. Eller registrer en vanlig bruker
4. Du er nå klar til å booke rom!

## Brukerroller

Systemet har to brukerroller:

### Admin
- **E-post:** admin@rombooking.no
- **Passord:** Admin123!
- Kan administrere rom (legge til, redigere, slette)
- Kan booke rom
- Ser "Administrer rom" i menyen

### Vanlige brukere
- Registrerer seg selv via "Registrer deg"-siden
- Kan kun booke rom
- Kan se og kansellere egne bookinger
- Ser IKKE "Administrer rom" i menyen

## Hvordan bruke systemet

### Booke et rom
1. Velg dato i kalenderen
2. Klikk på et ledig tidspunkt i ønsket rom
3. Velg starttid og sluttid
4. Legg til notater (valgfritt)
5. Klikk "Opprett booking"

### Se mine bookinger
1. Klikk på "Mine bookinger" i menyen
2. Her ser du kommende og tidligere bookinger
3. Du kan kansellere kommende bookinger

### Administrere rom
1. Klikk på "Administrer rom" i menyen
2. Her kan du:
   - Legge til nye rom
   - Redigere eksisterende rom
   - Slette rom (hvis de ikke har fremtidige bookinger)
   - Deaktivere rom

## Database
Systemet bruker SQLite som er en enkel fil-basert database. Databasefilen heter `rombooking.db` og opprettes automatisk første gang du kjører applikasjonen.

## Helligdager
Systemet støtter automatisk norske helligdager:
- Nyttårsdag (1. januar)
- Skjærtorsdag, Langfredag, Påskedag, 2. påskedag
- 1. mai
- 17. mai
- Kristi himmelfartsdag
- Pinsedag, 2. pinsedag
- 1. juledag, 2. juledag

Påsken beregnes automatisk for hvert år.

## Tilpasning

### Endre åpningstider
Rediger `Services/BookingService.cs` linje 18-19:
```csharp
private readonly TimeSpan _openingTime = new TimeSpan(8, 0, 0);  // 08:00
private readonly TimeSpan _closingTime = new TimeSpan(16, 0, 0); // 16:00
```

### Legge til flere rom ved oppstart
Rediger `Data/ApplicationDbContext.cs` linje 32-38 og legg til flere rom i `HasData()`.

### Endre design
All CSS finnes i `wwwroot/css/site.css` - her kan du style som du vil!

## Sikkerhet og administrasjon

### Endre admin-passord
For produksjon bør du endre admin-passordet:

1. Logg inn som admin
2. Endre passordet via brukerinnstillinger (kan legges til senere)

Eller endre direkte i databasen eller i `Program.cs` før første kjøring.

### Legge til flere administratorer
Bruk denne koden i `Program.cs` for å legge til flere admins, eller opprett en admin-side for å administrere brukere.

## Feilsøking

### Databasen opprettes ikke
Slett `rombooking.db` filen og kjør på nytt.

### Port-konflikt
Endre port i `Properties/launchSettings.json` eller i kommandolinjen:
```bash
dotnet run --urls "https://localhost:5555"
```

## Teknisk informasjon

### Prosjektstruktur
```
RomBooking/
├── Controllers/          # MVC Controllers
├── Models/              # Data modeller
├── Views/               # Razor views
├── Services/            # Business logic
├── Data/                # Database context
└── wwwroot/             # Statiske filer (CSS, JS)
```

### Database-modeller
- **ApplicationUser**: Brukerinfo med Identity
- **Room**: Rom med navn og beskrivelse
- **Booking**: Booking med tidspunkt og bruker

## Support
Dette er et ferdig, fungerende system. Du kan enkelt utvide det med flere funksjoner etter behov.
