using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace StacjaPogodowa
{
    // ---------- model pomiaru ----------
    public class Measurement : INotifyPropertyChanged
    {
        public string Station { get; set; } = "";
        public DateTime Date { get; set; }
        public double Temperature { get; set; }
        public double Value { get; set; }

        private bool _isMax;
        public bool IsMax { get => _isMax; set { _isMax = value; OnChanged(); } }

        private bool _isMin;
        public bool IsMin { get => _isMin; set { _isMin = value; OnChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnChanged([CallerMemberName] string? n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }

    public partial class MainWindow : Window
    {
        private const char Separator = ';';
        private const string DateFormat = "yyyy-MM-dd";
        private const string AllStations = "(wszystkie)";
        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        private readonly string _path =
            @"C:\Users\maxxi\Documents\StacjaPogodowa\a.txt";

        private readonly ObservableCollection<Measurement> _data = new();
        private readonly ICollectionView _view;
        private bool _updatingCombo;

        public MainWindow()
        {
            InitializeComponent();

            _view = CollectionViewSource.GetDefaultView(_data);
            _view.Filter = FilterByStation;
            Grid.ItemsSource = _view;

            TbDate.Text = DateTime.Now.ToString(DateFormat, Inv);

            if (File.Exists(_path)) LoadFromFile();
        }

        // ---------- odczyt z pliku (separator ;) ----------
        private static List<Measurement> ReadFile(string path)
        {
            var list = new List<Measurement>();
            foreach (var line in File.ReadAllLines(path, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                // linia kończy się ';', więc ostatni element jest pusty - pomijamy go
                var p = line.Split(Separator);
                if (p.Length < 4) continue;

                list.Add(new Measurement
                {
                    Station = p[0].Trim(),
                    Date = DateTime.ParseExact(p[1].Trim(), DateFormat, Inv),
                    Temperature = double.Parse(p[2].Trim(), NumberStyles.Float, Inv),
                    Value = double.Parse(p[3].Trim(), NumberStyles.Float, Inv)
                });
            }
            return list;
        }

        // ---------- zapis do pliku (osobno od odczytu) ----------
        private static void WriteFile(string path, IEnumerable<Measurement> items)
        {
            var sb = new StringBuilder();
            foreach (var m in items)
            {
                sb.Append(m.Station).Append(Separator)
                  .Append(m.Date.ToString(DateFormat, Inv)).Append(Separator)
                  .Append(m.Temperature.ToString(Inv)).Append(Separator)
                  .Append(m.Value.ToString(Inv)).Append(Separator)   // końcowy ';'
                  .AppendLine();
            }
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        // ---------- filtr po stacji ----------
        private bool FilterByStation(object o)
        {
            var sel = StationCombo.SelectedItem as string;
            if (string.IsNullOrEmpty(sel) || sel == AllStations) return true;
            return ((Measurement)o).Station == sel;
        }

        // ---------- odświeżanie po KAŻDEJ zmianie danych ----------
        private void Refresh()
        {
            RefreshStationList();
            _view.Refresh();
            HighlightMinMax();
        }

        private void RefreshStationList()
        {
            var previous = StationCombo.SelectedItem as string;
            _updatingCombo = true;

            var stations = _data.Select(m => m.Station).Distinct().OrderBy(s => s).ToList();
            stations.Insert(0, AllStations);
            StationCombo.ItemsSource = stations;
            StationCombo.SelectedItem =
                previous != null && stations.Contains(previous) ? previous : AllStations;

            _updatingCombo = false;
        }

        // MAX i MIN temperatury wśród aktualnie widocznych wierszy
        private void HighlightMinMax()
        {
            foreach (var m in _data) { m.IsMax = false; m.IsMin = false; }

            var visible = _view.Cast<Measurement>().ToList();
            if (visible.Count == 0) return;

            double max = visible.Max(m => m.Temperature);
            double min = visible.Min(m => m.Temperature);

            foreach (var m in visible)
            {
                if (m.Temperature == max) m.IsMax = true;
                if (m.Temperature == min) m.IsMin = true;
            }
        }

        // ---------- zdarzenia ----------
        private void LoadFromFile()
        {
            try
            {
                _data.Clear();
                foreach (var m in ReadFile(_path)) _data.Add(m);
                Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd odczytu pliku: " + ex.Message);
            }
        }

        private void Load_Click(object s, RoutedEventArgs e) => LoadFromFile();

        private void Save_Click(object s, RoutedEventArgs e)
        {
            try
            {
                WriteFile(_path, _data);
                MessageBox.Show("Zapisano: " + _path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu: " + ex.Message);
            }
        }

        private void Add_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbStation.Text) ||
                !DateTime.TryParseExact(TbDate.Text.Trim(), DateFormat, Inv,
                                        DateTimeStyles.None, out var date) ||
                !double.TryParse(TbTemp.Text.Trim(), NumberStyles.Float, Inv, out var temp) ||
                !double.TryParse(TbVal.Text.Trim(), NumberStyles.Float, Inv, out var val))
            {
                MessageBox.Show("Niepoprawne dane (data np. 2026-03-01; liczby z kropką, np. 18.5).");
                return;
            }

            _data.Add(new Measurement
            {
                Station = TbStation.Text.Trim(),
                Date = date,
                Temperature = temp,
                Value = val
            });
            Refresh();
        }

        private void StationCombo_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            if (_updatingCombo) return;
            _view.Refresh();
            HighlightMinMax();
        }
    }
}