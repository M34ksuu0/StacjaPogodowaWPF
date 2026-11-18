# StacjaPogodowa

Aplikacja desktopowa **WPF (C#)** do przeglądania i edycji pomiarów ze stacji pogodowych. Dane są wczytywane z pliku tekstowego `a.txt` z nietypowym separatorem `;`.

Projekt jest wersją C# programu napisanego wcześniej w C++.

## Funkcje

- Odczyt pomiarów z pliku tekstowego (separator `;`)
- Osobny zapis pomiarów do pliku
- Dodawanie nowego pomiaru z poziomu aplikacji
- Podświetlanie wierszy z najwyższą (**MAX**, kolor zielony) i najniższą (**MIN**, kolor niebieski) temperaturą
- Filtrowanie pomiarów po nazwie stacji (lista rozwijana)
- Automatyczne odświeżanie podświetlenia po każdej zmianie danych (wczytanie pliku, dodanie pomiaru, zmiana stacji w filtrze)

## Format pliku danych

Każda linia to jeden pomiar, pola oddzielone średnikiem, a linia kończy się średnikiem:

```
Stacja;Data;Temperatura;Wartość;
```

Przykład (`a.txt`):

```
STO1;2026-03-01;18.5;12.5;
STO2;2023-03-01;20.1;43;
```

| Pole | Format | Przykład |
|---|---|---|
| Stacja | tekst | `STO1` |
| Data | `rrrr-MM-dd` | `2026-03-01` |
| Temperatura | liczba z kropką dziesiętną | `18.5` |
| Wartość | liczba z kropką dziesiętną | `12.5` |

## Wymagania

- Windows
- .NET 8 SDK (lub nowszy) z obsługą WPF
- Visual Studio 2022 (opcjonalnie)

## Uruchomienie

1. Sklonuj repozytorium:

   ```bash
   git clone <adres-repozytorium>
   cd StacjaPogodowa
   ```

2. Upewnij się, że plik `a.txt` znajduje się w folderze projektu.

3. Uruchom aplikację:

   ```bash
   dotnet run
   ```

   Albo otwórz projekt w Visual Studio i naciśnij **F5**.

### Ścieżka do pliku danych

Ścieżka jest ustawiona w `MainWindow.xaml.cs`:

```csharp
private readonly string _path =
    @"C:\Users\maxxi\Documents\StacjaPogodowa\a.txt";
```

Jeśli projekt leży w innym miejscu, zmień tę ścieżkę.

## Obsługa

| Element | Działanie |
|---|---|
| **Wczytaj z pliku** | Wczytuje pomiary z `a.txt` i odświeża tabelę |
| **Zapisz do pliku** | Zapisuje aktualną listę pomiarów do `a.txt` |
| **Stacja** (lista rozwijana) | Pokazuje pomiary tylko wybranej stacji lub wszystkich (`(wszystkie)`) |
| **Dodaj pomiar** | Dodaje pomiar z pól tekstowych (stacja, data, temperatura, wartość) |

Kolory wierszy w tabeli:

- 🟩 zielony: maksymalna temperatura
- 🟦 niebieski: minimalna temperatura

MAX i MIN są liczone wśród aktualnie widocznych wierszy, więc po wybraniu stacji dotyczą tylko jej pomiarów.

## Struktura projektu

```
StacjaPogodowa/
├── StacjaPogodowa.csproj
├── MainWindow.xaml        # interfejs (tabela, filtr, formularz dodawania)
├── MainWindow.xaml.cs     # model Measurement, odczyt/zapis pliku, logika
└── a.txt                  # plik z pomiarami
```

## Technologie

- C#
- WPF (XAML)
- .NET 8
