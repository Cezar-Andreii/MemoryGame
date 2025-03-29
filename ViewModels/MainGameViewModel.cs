using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Memoryy.Models;
using Memoryy.Services;
using Memoryy.Views;

namespace Memoryy.ViewModels
{
    public class MainGameViewModel : INotifyPropertyChanged
    {
        private readonly CategoryService _categoryService;
        private readonly SaveGameService _saveGameService;
        private readonly StatisticsService _statisticsService;
        private readonly User _currentUser;
        private readonly DispatcherTimer _gameTimer;
        private readonly Random _random;
        private ObservableCollection<Card> _cards;
        private Card _firstCard;
        private Card _secondCard;
        private bool _isProcessing;
        private string _selectedCategory;
        private int _timeRemaining;
        private GameConfiguration _currentConfig;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainGameViewModel(User currentUser)
        {
            _categoryService = new CategoryService();
            _saveGameService = new SaveGameService();
            _statisticsService = new StatisticsService();
            _currentUser = currentUser;
            _random = new Random();
            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromSeconds(1);
            _gameTimer.Tick += GameTimer_Tick;

            Categories = new ObservableCollection<string>(_categoryService.GetAllCategories().Select(c => c.Name));
            Cards = new ObservableCollection<Card>();
        }

        public ObservableCollection<string> Categories { get; }
        public ObservableCollection<Card> Cards
        {
            get => _cards;
            set
            {
                _cards = value;
                OnPropertyChanged(nameof(Cards));
            }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        public int TimeRemaining
        {
            get => _timeRemaining;
            set
            {
                _timeRemaining = value;
                OnPropertyChanged(nameof(TimeRemaining));
            }
        }

        public ICommand NewGameCommand => new RelayCommand(StartNewGame, CanStartNewGame);
        public ICommand CustomGameCommand => new RelayCommand(StartCustomGame, CanStartNewGame);
        public ICommand CardClickCommand => new RelayCommand<Card>(OnCardClick, CanClickCard);
        public ICommand SaveGameCommand => new RelayCommand(SaveGame, () => _currentConfig != null);
        public ICommand LoadGameCommand => new RelayCommand(LoadGame, () => _saveGameService.HasSavedGame(_currentUser.Username));
        public ICommand ShowStatisticsCommand => new RelayCommand(ShowStatistics);
        public ICommand ShowAboutCommand => new RelayCommand(ShowAbout);

        private bool CanStartNewGame()
        {
            return !string.IsNullOrEmpty(SelectedCategory);
        }

        private void StartNewGame()
        {
            var config = new GameConfiguration
            {
                Rows = 4,
                Columns = 4,
                TimeLimit = 300, // 5 minute
                CategoryName = SelectedCategory,
                StartTime = DateTime.Now
            };

            InitializeGame(config);
        }

        private void StartCustomGame()
        {
            var viewModel = new CustomGameViewModel();
            viewModel.OnGameConfigurationSelected += (s, config) =>
            {
                config.CategoryName = SelectedCategory;
                InitializeGame(config);
            };

            var window = new CustomGameWindow(viewModel);
            window.ShowDialog();
        }

        private void InitializeGame(GameConfiguration config)
        {
            _currentConfig = config;
            var category = _categoryService.GetCategoryByName(config.CategoryName);
            var images = category.ImagePaths.OrderBy(x => _random.Next()).Take((config.Rows * config.Columns) / 2).ToList();
            images.AddRange(images); // Duplicăm imaginile pentru perechi

            Cards.Clear();
            for (int i = 0; i < config.Rows; i++)
            {
                for (int j = 0; j < config.Columns; j++)
                {
                    Cards.Add(new Card(images[_random.Next(images.Count)], i, j));
                }
            }

            TimeRemaining = config.TimeLimit;
            _gameTimer.Start();
        }

        private bool CanClickCard(Card card)
        {
            return !_isProcessing && !card.IsMatched && !card.IsFlipped;
        }

        private void OnCardClick(Card card)
        {
            if (_firstCard == null)
            {
                _firstCard = card;
                card.IsFlipped = true;
            }
            else if (_secondCard == null)
            {
                _secondCard = card;
                card.IsFlipped = true;
                CheckMatch();
            }
        }

        private void CheckMatch()
        {
            _isProcessing = true;
            if (_firstCard.ImagePath == _secondCard.ImagePath)
            {
                _firstCard.IsMatched = true;
                _secondCard.IsMatched = true;
                CheckGameEnd();
            }
            else
            {
                System.Threading.Tasks.Task.Delay(1000).ContinueWith(_ =>
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        _firstCard.IsFlipped = false;
                        _secondCard.IsFlipped = false;
                        _firstCard = null;
                        _secondCard = null;
                        _isProcessing = false;
                    });
                });
            }
        }

        private void CheckGameEnd()
        {
            if (Cards.All(c => c.IsMatched))
            {
                _gameTimer.Stop();
                _statisticsService.AddGameWon(_currentUser.Username);
                MessageBox.Show("Felicitări! Ai câștigat!", "Victorie", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            TimeRemaining--;
            if (TimeRemaining <= 0)
            {
                _gameTimer.Stop();
                _statisticsService.AddGamePlayed(_currentUser.Username);
                MessageBox.Show("Timpul a expirat!", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveGame()
        {
            if (_currentConfig == null) return;

            var savedGame = new SavedGame
            {
                Username = _currentUser.Username,
                Configuration = _currentConfig,
                SaveTime = DateTime.Now,
                Cards = Cards.Select(c => new CardState
                {
                    ImagePath = c.ImagePath,
                    IsFlipped = c.IsFlipped,
                    IsMatched = c.IsMatched,
                    Row = c.Row,
                    Column = c.Column
                }).ToList()
            };

            _saveGameService.SaveGame(savedGame);
            MessageBox.Show("Joc salvat cu succes!", "Salvare", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadGame()
        {
            var savedGame = _saveGameService.LoadGame(_currentUser.Username);
            if (savedGame == null)
            {
                MessageBox.Show("Nu există joc salvat!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _currentConfig = savedGame.Configuration;
            SelectedCategory = savedGame.Configuration.CategoryName;

            Cards.Clear();
            foreach (var cardState in savedGame.Cards)
            {
                Cards.Add(new Card(cardState.ImagePath, cardState.Row, cardState.Column)
                {
                    IsFlipped = cardState.IsFlipped,
                    IsMatched = cardState.IsMatched
                });
            }

            TimeRemaining = savedGame.Configuration.TimeLimit -
                           (int)(DateTime.Now - savedGame.Configuration.StartTime).TotalSeconds;

            if (TimeRemaining <= 0)
            {
                MessageBox.Show("Timpul pentru acest joc salvat a expirat!", "Game Over",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _gameTimer.Start();
            MessageBox.Show("Joc încărcat cu succes!", "Încărcare", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowStatistics()
        {
            var statisticsWindow = new StatisticsWindow();
            statisticsWindow.ShowDialog();
        }

        private void ShowAbout()
        {
            MessageBox.Show(
                "Memory Game\n\n" +
                "Student: [Numele tău]\n" +
                "Email: [email@institutionala.ro]\n" +
                "Grupa: [Numărul grupei]\n" +
                "Specializarea: [Specializarea]",
                "Despre",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}