
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace WPFClient;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    HubConnection connection;
    public MainWindow()
    {
        InitializeComponent();

        connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7216/chathub")
            .WithAutomaticReconnect()
            .Build();


        connection.Reconnecting += (sender) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = "Attempting to reconnect...";
                messages.Items.Add(newMessage); //adding to listbox in Mainwindow
            });
            return Task.CompletedTask;
        };

        connection.Reconnected += (sender) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = "Reconnected to the Server...";
                messages.Items.Clear();
                messages.Items.Add(newMessage); //adding to listbox in Mainwindow
            });
            return Task.CompletedTask;
        };

        connection.Closed += (sender) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = "Connection Closed";
                messages.Items.Add(newMessage); //adding to listbox in Mainwindow
                openConnection.IsEnabled = true;
                sendMessage.IsEnabled = false; //button is disabled can't send a message if connectio nis closed
            
            
            });
            return Task.CompletedTask;
        };



    }

    private async void openConnection_Click(object sender, RoutedEventArgs e)
    {
        connection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = $"{user}: {message}";
                messages.Items.Add(newMessage); //adding to listbox in Mainwindow
            });
        });


        //protects against trying a connection that doesn't work
        try 
        {
            await connection.StartAsync();
            messages.Items.Add("Connection Started");
            openConnection.IsEnabled = false; //disables button
            sendMessage.IsEnabled = true;
        }
        catch(Exception ex)
        {
            messages.Items.Add(ex.Message); //adds exception into list of messages
        }
    }

    private async void sendMessage_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            //WPF client is hard coded
            await connection.InvokeAsync("SendMessage", 
                "WPF Client", messageInput.Text);
        }
        catch(Exception ex)
        {
            messages.Items.Add(ex.Message); //adds exception into list of messages

        }



    }
}
