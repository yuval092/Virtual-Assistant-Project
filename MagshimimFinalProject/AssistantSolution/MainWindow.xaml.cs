using AssistantSolution.Classes;
using AssistantSolution.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AssistantSolution
{
    public partial class MainWindow : Window
    {
        List<Message> messages;

        public MainWindow()
        {
            InitializeComponent();

            ListBoxMain.SelectionChanged += ListBoxMain_SelectionChanged;
        }

        private void ListBoxMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxMain.SelectedItem != null)
                this.Title = (ListBoxMain.SelectedItem as Message).User;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (messageText.Text != "")
            {
                ListBoxMain.Items.Add(new Message() { User = "Yuval092: ", Text = messageText.Text });
                IntentAndSlotsResponse resp = CommunicationHandler.getIntentAndSlots(messageText.Text);

                string intent = Regex.Replace(resp.Intent, @"\t|\n|\r|", "");
                string slots = string.Join(", ", resp.Slots.ToArray());
                string input = messageText.Text.Replace('?', ' ').TrimEnd();
                // web services
                //WebServices.GetWeather();

                string result = WebServices.GetWeather(input, intent, resp.Slots);
                ListBoxMain.Items.Add(new Message() { User = "Assistant: ", Text = result });
            }

            messageText.Text = "";
        }
    }
}
