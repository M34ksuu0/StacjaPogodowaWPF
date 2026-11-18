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
