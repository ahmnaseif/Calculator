using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentExpression = "";
        private bool newInput = true;
        private string lastCalculation = "";
        private bool isProcessingInput = false;

        public MainWindow()
        {
            InitializeComponent();
            Display.Focus();
        }

        #region Input Handling
        private void Display_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (isProcessingInput) return;
            isProcessingInput = true;

            if (char.IsDigit(e.Text, 0) || e.Text == ".")
            {
                AppendToDisplay(e.Text);
                e.Handled = true;
            }
            else if (IsOperator(e.Text))
            {
                AppendOperator(e.Text);
                e.Handled = true;
            }

            isProcessingInput = false;
        }

        private void Display_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (isProcessingInput) return;
            isProcessingInput = true;

            switch (e.Key)
            {
                case Key.D0:
                case Key.NumPad0:
                    AppendToDisplay("0");
                    e.Handled = true;
                    break;
                case Key.D1:
                case Key.NumPad1:
                    AppendToDisplay("1");
                    e.Handled = true;
                    break;
                case Key.D2:
                case Key.NumPad2:
                    AppendToDisplay("2");
                    e.Handled = true;
                    break;
                case Key.D3:
                case Key.NumPad3:
                    AppendToDisplay("3");
                    e.Handled = true;
                    break;
                case Key.D4:
                case Key.NumPad4:
                    AppendToDisplay("4");
                    e.Handled = true;
                    break;
                case Key.D5:
                case Key.NumPad5:
                    AppendToDisplay("5");
                    e.Handled = true;
                    break;
                case Key.D6:
                case Key.NumPad6:
                    AppendToDisplay("6");
                    e.Handled = true;
                    break;
                case Key.D7:
                case Key.NumPad7:
                    AppendToDisplay("7");
                    e.Handled = true;
                    break;
                case Key.D8:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        AppendOperator("×");
                    else
                        AppendToDisplay("8");
                    e.Handled = true;
                    break;
                case Key.NumPad8:
                    AppendToDisplay("8");
                    e.Handled = true;
                    break;
                case Key.D9:
                case Key.NumPad9:
                    AppendToDisplay("9");
                    e.Handled = true;
                    break;
                case Key.Add:
                case Key.OemPlus:
                    AppendOperator("+");
                    e.Handled = true;
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    AppendOperator("-");
                    e.Handled = true;
                    break;
                case Key.Multiply:
                    AppendOperator("×");
                    e.Handled = true;
                    break;
                case Key.Divide:
                case Key.OemQuestion:
                    AppendOperator("/");
                    e.Handled = true;
                    break;
                case Key.Decimal:
                case Key.OemPeriod:
                    AppendDecimal();
                    e.Handled = true;
                    break;
                case Key.Back:
                    HandleBackspace();
                    e.Handled = true;
                    break;
                case Key.Enter:
                    CalculateResult();
                    e.Handled = true;
                    break;
                case Key.Escape:
                    ClearAll();
                    e.Handled = true;
                    break;
            }

            isProcessingInput = false;
        }

        private void Display_TextChanged(object sender, TextChangedEventArgs e)
        {
            Display.CaretIndex = Display.Text.Length;
        }
        #endregion

        #region Calculator Logic
        private void AppendToDisplay(string text)
        {
            if (newInput)
            {
                currentExpression = "";
                newInput = false;
            }
            currentExpression += text;
            Display.Text = currentExpression;
        }

        private void AppendOperator(string op)
        {
            if (currentExpression.Length == 0 && op != "-") return;

            if (newInput)
            {
                currentExpression = "";
                newInput = false;
            }

            currentExpression += " " + op + " ";
            Display.Text = currentExpression;
            newInput = false;
        }

        private void AppendDecimal()
        {
            if (newInput)
            {
                currentExpression = "0.";
                newInput = false;
            }
            else
            {
                string[] parts = currentExpression.Split(new[] { '+', '-', '×', '/' },
                    StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0 || !parts.Last().Contains("."))
                {
                    currentExpression += ".";
                }
            }
            Display.Text = currentExpression;
        }

        private void HandleBackspace()
        {
            if (currentExpression.Length > 0)
            {
                currentExpression = currentExpression.Remove(currentExpression.Length - 1);
                Display.Text = currentExpression;
                if (currentExpression.Length == 0)
                {
                    newInput = true;
                }
            }
        }

        private void CalculateResult()
        {
            try
            {
                string expression = currentExpression.Replace("×", "*");
                var result = new DataTable().Compute(expression, null);

                lastCalculation = $"{currentExpression} = {result}";
                HistoryLabel.Content = lastCalculation;

                currentExpression = result.ToString();
                Display.Text = currentExpression;
                newInput = true;
            }
            catch (Exception ex)
            {
                HandleCalculationError(ex is DivideByZeroException ?
                    "Error: Division by zero" : "Error in calculation");
            }
        }

        private void HandleCalculationError(string message)
        {
            Display.Text = message;
            HistoryLabel.Content = message;
            currentExpression = "";
            newInput = true;
        }

        private bool IsOperator(string value)
        {
            return value == "+" || value == "-" || value == "×" || value == "/";
        }
        #endregion

        #region Button Click Handlers
        private void Number_Click(object sender, RoutedEventArgs e)
        {
            if (isProcessingInput) return;
            isProcessingInput = true;

            Button button = (Button)sender;
            AppendToDisplay(button.Content.ToString());

            isProcessingInput = false;
        }

        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            if (isProcessingInput) return;
            isProcessingInput = true;

            Button button = (Button)sender;
            AppendOperator(button.Content.ToString());

            isProcessingInput = false;
        }

        private void BtnEquals_Click(object sender, RoutedEventArgs e)
        {
            if (isProcessingInput) return;
            isProcessingInput = true;

            CalculateResult();

            isProcessingInput = false;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        private void ClearAll()
        {
            currentExpression = "";
            newInput = true;
            Display.Text = "0";
            HistoryLabel.Content = "";
        }

        private void BtnBackspace_Click(object sender, RoutedEventArgs e)
        {
            if (isProcessingInput) return;
            isProcessingInput = true;

            HandleBackspace();

            isProcessingInput = false;
        }

        private void BtnDecimal_Click(object sender, RoutedEventArgs e)
        {
            if (isProcessingInput) return;
            isProcessingInput = true;

            AppendDecimal();

            isProcessingInput = false;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(lastCalculation))
            {
                int equalsIndex = lastCalculation.IndexOf(" = ");
                if (equalsIndex > 0)
                {
                    currentExpression = lastCalculation.Substring(0, equalsIndex);
                    Display.Text = currentExpression;
                    newInput = false;
                }
            }
        }
        #endregion
    }
}