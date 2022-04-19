using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Newtonsoft.Json;
using System.IO;
using System.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using QuizFinalVersion.Commands;
using QuizFinalVersion.Models;
using ListBoxItem = QuizFinalVersion.Models.ListBoxItem;

namespace QuizFinalVersion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ReadFromFile();
            InitGame();
        }

        private Quiz _quiz;
        private List<Question> _questions;
        private int _currentQuestion;

        private readonly QuizTimer _quizTimer = new QuizTimer();
        private readonly Dictionary<Question, bool> _answersDictionary = new Dictionary<Question, bool>();
        private readonly List<CheckBox> _checkBoxes = new List<CheckBox>();

        private void InitGame()
        {
            _checkBoxes.AddRange(new List<CheckBox>()
            {
                CorrectA, CorrectB, CorrectC, CorrectD
            });
        }

        private void ChangeQuestion()
        {
            QuestionName.Text = _questions[_currentQuestion].QuestionName;
            AnswerA.Text = _questions[_currentQuestion].AnswerA;
            AnswerB.Text = _questions[_currentQuestion].AnswerB;
            AnswerC.Text = _questions[_currentQuestion].AnswerC;
            AnswerD.Text = _questions[_currentQuestion].AnswerD;
        }

        private void QuizSetup()
        {
            _currentQuestion = 0;
            _questions = _quiz.Questions.OrderBy(i => Guid.NewGuid()).ToList();
            Title.Text = _quiz.Name;
            QuestionNumber.Text = $"Pytanie nr 1 / {_questions.Count}";
        }

        private void ReadFromFile(string fileName = "questionList.json")
        {
            try
            {
                _quiz = new ReadQuizFile<Quiz>().ReadJsonFile(fileName);
            }
            catch (Exception e)
            {
                Title.Text = "Błąd odczytu pliku";
                Title.Foreground = Brushes.Red;
                StartButton.IsEnabled = false;
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_checkBoxes.All(checkbox => (bool) checkbox.IsChecked == false))
            {
                return;
            }

            if (_currentQuestion == _questions.Count - 1)
            {
                NextButton.IsEnabled = false;
                var isCorrectAnswer = CheckIsCorrectAnswer();
                _answersDictionary.Add(_questions[_currentQuestion], isCorrectAnswer);
                EndQuiz();
            }
            else if (_currentQuestion < _questions.Count)
            {
                var isCorrectAnswer = CheckIsCorrectAnswer();

                _answersDictionary.Add(_questions[_currentQuestion], isCorrectAnswer);

                _currentQuestion++;
                ChangeQuestion();
                ChangeCheckboxesFlag();
            }

            QuestionNumber.Text = $"Pytanie nr {_currentQuestion + 1} / {_questions.Count}";
        }

        private bool CheckIsCorrectAnswer()
        {
            var mappedCheckbox = MapAnswerToCheckbox(_questions[_currentQuestion]);

            var isCorrectAnswer = false;
            var numberOfCorrectAnswers = 0;

            foreach (var checkbox in mappedCheckbox.Where(checkbox => (bool)checkbox.Key.IsChecked))
            {
                foreach (var answer in _questions[_currentQuestion].CorrectAnswers)
                {
                    if (checkbox.Value.Equals(answer))
                    {
                        isCorrectAnswer = true;
                        numberOfCorrectAnswers++;
                        break;
                    }
                }
            }

            if (numberOfCorrectAnswers != _questions[_currentQuestion].CorrectAnswers.Count)
            {
                isCorrectAnswer = false;
            }

            return isCorrectAnswer;
        }

        private void ChangeCheckboxesFlag()
        {
            foreach (var c in _checkBoxes)
            {
                c.IsChecked = false;
            }
        }

        private Dictionary<CheckBox, string> MapAnswerToCheckbox(Question question)
        {
            var dictionary = new Dictionary<CheckBox, string>
            {
                {CorrectA, question.AnswerA},
                {CorrectB, question.AnswerB},
                {CorrectC, question.AnswerC},
                {CorrectD, question.AnswerD}
            };

            return dictionary;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            QuizSetup();
            ChangeQuestion();
            ChangeCheckboxesFlag();
            QuestionListTextBlock.Visibility = Visibility.Hidden;
            AnswersList.Visibility = Visibility.Hidden;
            Results.Visibility = Visibility.Hidden;
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            NextButton.IsEnabled = true;
            _quizTimer.Seconds = 0;
            _answersDictionary.Clear();
            AnswersList.ItemsSource = null;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            EndQuiz();
        }

        private void EndQuiz()
        {
            QuestionListTextBlock.Visibility = Visibility.Visible;
            AnswersList.Visibility = Visibility.Visible;
            Results.Visibility = Visibility.Visible;
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            NextButton.IsEnabled = false;

            var listBoxItems = new List<ListBoxItem>();
            var totalCorrectAnswers = 0;

            if (_answersDictionary.Count > 0)
            {
                for (var i = 0; i < _answersDictionary.Count; i++)
                {
                    if (_answersDictionary.ElementAt(i).Value)
                    {
                        totalCorrectAnswers++;
                    }
                }
                
                foreach (var answer in _answersDictionary)
                {
                    if (answer.Value)
                    {
                        listBoxItems.Add(new ListBoxItem(answer.Key.QuestionName, new SolidColorBrush(Colors.Blue)));
                    }
                    else
                    {
                        listBoxItems.Add(new ListBoxItem(answer.Key.QuestionName, new SolidColorBrush(Colors.Red)));
                    }
                }

                AnswersList.Items.Clear();
                AnswersList.ItemsSource = listBoxItems;
                AnswersList.Items.Refresh();
            }

            Results.Text = $"Poprawne odpowiedzi: {totalCorrectAnswers} / {_questions.Count}";
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dt = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            dt.Tick += DtTicker;
            dt.Start();
        }

        private void DtTicker(object sender, EventArgs e)
        {
            if (StopButton.IsEnabled)
            {
                _quizTimer.Seconds++;
                Timer.Text = _quizTimer.TimerFormatter();
            }
        }

        private void AnswersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StartButton.IsEnabled)
            {
                ChangeCheckboxesFlag();

                var itemName = (AnswersList.SelectedItems[0] as ListBoxItem)?.Name;
                var selectedQuestion = _questions.Find(q => q.QuestionName.Equals(itemName));

                QuestionName.Text = selectedQuestion.QuestionName;
                AnswerA.Text = selectedQuestion.AnswerA;
                AnswerB.Text = selectedQuestion.AnswerB;
                AnswerC.Text = selectedQuestion.AnswerC;
                AnswerD.Text = selectedQuestion.AnswerD;

                var mappedCheckbox = MapAnswerToCheckbox(_questions[AnswersList.SelectedIndex]);
                foreach (var correctAnswer in selectedQuestion.CorrectAnswers)
                {
                    foreach (var checkbox in mappedCheckbox)
                    {
                        if (correctAnswer.Equals(checkbox.Value))
                        {
                            checkbox.Key.IsChecked = true;
                        }
                    }
                }
            }
        }
    }
}
