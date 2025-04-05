using System;
using System.Collections.Generic;
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
        private readonly DispatcherTimer _timer;
        private GameConfiguration _currentGameConfig;
        private int _timeLeft;
        private bool _isGameActive;
        private bool _isGamePaused;
        private Card _firstSelectedCard;
        private Card _secondSelectedCard;
        private int _pairsFound;
        private int _movesMade;
        private Category _selectedCategory;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Card> Cards { get; private set; }
        public ObservableCollection<Category> Categories { get; private set; }

        public GameConfiguration CurrentGameConfig
        {
            get => _currentGameConfig;
            set
            {
                _currentGameConfig = value;
                OnPropertyChanged(nameof(CurrentGameConfig));
            }
        }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                if (_selectedCategory != null)
                {
                    _currentGameConfig.CategoryName = _selectedCategory.Name;
                }
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        public ICommand NewGameCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand LoadGameCommand { get; }
        public ICommand StatisticsCommand { get; }
        public ICommand CustomGameCommand { get; }
        public ICommand CardClickCommand { get; }
        public ICommand ShowAboutCommand { get; }

        public string TimeLeftText => $"Timp rămas: {TimeLeft / 60}:{TimeLeft % 60:D2}";
        public string MovesText => $"Mutări: {MovesMade}";
        public string PairsText => $"Perechi găsite: {PairsFound}";

        public int TimeLeft
        {
            get => _timeLeft;
            set
            {
                _timeLeft = value;
                OnPropertyChanged(nameof(TimeLeft));
                OnPropertyChanged(nameof(TimeLeftText));
            }
        }

        public int MovesMade
        {
            get => _movesMade;
            set
            {
                _movesMade = value;
                OnPropertyChanged(nameof(MovesMade));
                OnPropertyChanged(nameof(MovesText));
            }
        }

        public int PairsFound
        {
            get => _pairsFound;
            set
            {
                _pairsFound = value;
                OnPropertyChanged(nameof(PairsFound));
                OnPropertyChanged(nameof(PairsText));
            }
        }

        public MainGameViewModel(User currentUser)
        {
            try
            {
                _currentUser = currentUser;
                _categoryService = new CategoryService();
                _saveGameService = new SaveGameService();
                _statisticsService = new StatisticsService();

                _timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                _timer.Tick += Timer_Tick;

                NewGameCommand = new RelayCommand(StartNewGame, CanStartNewGame);
                SaveGameCommand = new RelayCommand(SaveGame);
                LoadGameCommand = new RelayCommand(LoadGame);
                StatisticsCommand = new RelayCommand(ShowStatistics);
                CustomGameCommand = new RelayCommand(StartCustomGame);
                CardClickCommand = new RelayCommand<Card>(OnCardClick, CanClickCard);
                ShowAboutCommand = new RelayCommand(ShowAbout);

                Categories = new ObservableCollection<Category>(_categoryService.GetAllCategories());
                Cards = new ObservableCollection<Card>();

                // Inițializare configurație implicită
                _currentGameConfig = new GameConfiguration
                {
                    Rows = 4,
                    Columns = 4,
                    TimeLimit = 300,
                    CategoryName = "Emoji",
                    StartTime = DateTime.Now
                };

                if (Categories.Any())
                {
                    SelectedCategory = Categories.First();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la inițializarea ViewModel: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private bool CanStartNewGame()
        {
            return SelectedCategory != null;
        }

        private bool CanClickCard(Card card)
        {
            return _isGameActive && !card.IsMatched && !card.IsFlipped;
        }

        private void OnCardClick(Card card)
        {
            if (_firstSelectedCard == null)
            {
                _firstSelectedCard = card;
                card.IsFlipped = true;
            }
            else if (_secondSelectedCard == null && card != _firstSelectedCard)
            {
                _secondSelectedCard = card;
                card.IsFlipped = true;
                CheckMatch();
            }
        }

        private async void CheckMatch()
        {
            MovesMade++;

            if (_firstSelectedCard.ImagePath == _secondSelectedCard.ImagePath)
            {
                _firstSelectedCard.IsMatched = true;
                _secondSelectedCard.IsMatched = true;
                PairsFound++;

                if (PairsFound == (_currentGameConfig.Rows * _currentGameConfig.Columns) / 2)
                {
                    _timer.Stop();
                    _statisticsService.AddGameWon(_currentUser.Username);
                    MessageBox.Show("Felicitări! Ai câștigat!", "Victorie", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                await System.Threading.Tasks.Task.Delay(1000);
                _firstSelectedCard.IsFlipped = false;
                _secondSelectedCard.IsFlipped = false;
            }

            _firstSelectedCard = null;
            _secondSelectedCard = null;
        }

        private void StartNewGame()
        {
            try
            {
                if (_isGameActive)
                {
                    var result = MessageBox.Show(
                        "Jocul curent va fi pierdut. Vrei să continui?",
                        "Confirmare",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.No)
                        return;
                }

                if (SelectedCategory == null)
                {
                    MessageBox.Show("Te rog selectează o categorie!");
                    return;
                }

                _currentGameConfig.CategoryName = SelectedCategory.Name;
                var category = _categoryService.GetCategoryByName(_currentGameConfig.CategoryName);

                if (category == null)
                {
                    MessageBox.Show($"Categoria {_currentGameConfig.CategoryName} nu a fost găsită!");
                    return;
                }

                if (category.ImagePaths.Count < (_currentGameConfig.Rows * _currentGameConfig.Columns) / 2)
                {
                    MessageBox.Show($"Nu sunt suficiente imagini în categoria {category.Name}!\n" +
                                  $"Sunt necesare cel puțin {(_currentGameConfig.Rows * _currentGameConfig.Columns) / 2} imagini.");
                    return;
                }

                InitializeGame();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la startarea jocului nou: {ex.Message}");
            }
        }

        private void InitializeGame()
        {
            try
            {
                var category = _categoryService.GetCategoryByName(_currentGameConfig.CategoryName);
                var random = new Random();
                var selectedImages = category.ImagePaths
                    .OrderBy(x => random.Next())
                    .Take((_currentGameConfig.Rows * _currentGameConfig.Columns) / 2)
                    .ToList();

                Cards.Clear();
                var cardPairs = new List<Card>();

                int cardIndex = 0;
                for (int row = 0; row < _currentGameConfig.Rows; row++)
                {
                    for (int col = 0; col < _currentGameConfig.Columns; col++)
                    {
                        if (cardIndex < selectedImages.Count)
                        {
                            cardPairs.Add(new Card(selectedImages[cardIndex], row, col));
                            cardPairs.Add(new Card(selectedImages[cardIndex], row, col));
                            cardIndex++;
                        }
                    }
                }

                cardPairs = cardPairs.OrderBy(x => random.Next()).ToList();

                foreach (var card in cardPairs)
                {
                    Cards.Add(card);
                }

                TimeLeft = _currentGameConfig.TimeLimit;
                MovesMade = 0;
                PairsFound = 0;
                _isGameActive = true;
                _timer.Start();
                _currentGameConfig.StartTime = DateTime.Now;
                _statisticsService.AddGamePlayed(_currentUser.Username);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la inițializarea jocului: {ex.Message}");
            }
        }

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeLeft--;
            if (TimeLeft <= 0)
            {
                _timer.Stop();
                _statisticsService.AddGamePlayed(_currentUser.Username);
                MessageBox.Show("Timpul a expirat!", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveGame()
        {
            if (_currentGameConfig == null) return;

            var savedGame = new SavedGame
            {
                Username = _currentUser.Username,
                Configuration = _currentGameConfig,
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

            _currentGameConfig = savedGame.Configuration;

            Cards.Clear();
            foreach (var cardState in savedGame.Cards)
            {
                Cards.Add(new Card(cardState.ImagePath, cardState.Row, cardState.Column)
                {
                    IsFlipped = cardState.IsFlipped,
                    IsMatched = cardState.IsMatched
                });
            }

            TimeLeft = savedGame.Configuration.TimeLimit -
                       (int)(DateTime.Now - savedGame.Configuration.StartTime).TotalSeconds;

            if (TimeLeft <= 0)
            {
                MessageBox.Show("Timpul pentru acest joc salvat a expirat!", "Game Over",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _timer.Start();
            MessageBox.Show("Joc încărcat cu succes!", "Încărcare", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowStatistics()
        {
            var statisticsWindow = new StatisticsWindow();
            statisticsWindow.ShowDialog();
        }

        private void StartCustomGame()
        {
            var viewModel = new CustomGameViewModel();
            viewModel.OnGameConfigurationSelected += (s, config) =>
            {
                config.CategoryName = _currentGameConfig.CategoryName;
                _currentGameConfig = config;
                InitializeGame();
            };

            var window = new CustomGameWindow(viewModel);
            window.ShowDialog();
        }

        private void ShowAbout()
        {
            MessageBox.Show(
                "Memory Game\n\n" +
                "Student: Dragomir Cezar Andrei\n" +
                "Email: cezar.dragomir@student.unitbv.ro\n" +
                "Grupa: 10LF232\n" +
                "Specializare: Informatică",
                "Despre",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
