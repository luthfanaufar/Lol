using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VoiceRecognition.Resources;
using Windows.Phone.Speech.Recognition;
using System.ComponentModel;
using System.Diagnostics;

namespace VoiceRecognition
{
    public partial class MainPage : PhoneApplicationPage
    {
        Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        
        private BackgroundWorker bw = new BackgroundWorker();
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            AddSpeech();
            //if (bw.WorkerSupportsCancellation == true)
            //{
            //    bw.CancelAsync();
            //}
        }
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            stopwatch.Start();
            int second = 10;

            while(true)
            {
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                else if (stopwatch.Elapsed.TotalSeconds < second)
                {
                    // Perform a time consuming operation and report progress.
                    System.Threading.Thread.Sleep(500);
                    worker.ReportProgress((int)((stopwatch.Elapsed.TotalSeconds/second) * 100));
                }
                else break;
            }
        }
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                this.tbProgress.Text = "Canceled!";
            }

            else if (!(e.Error == null))
            {
                this.tbProgress.Text = ("Error: " + e.Error.Message);
            }

            else
            {
                this.tbProgress.Text = "Done!";
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //this.tbProgress.Text = (e.ProgressPercentage.ToString() + "%");
            tbProgress.Text = stopwatch.Elapsed.TotalSeconds.ToString();
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            //bw.RunWorkerAsync();
        }

        async private void AddSpeech()
        {
            try
            {               
                List<string> ListGrammars = new List<string>() { "Next" };
                SpeechRecognizerUI speech = new SpeechRecognizerUI();
                speech.Recognizer.Grammars.AddGrammarFromList("List Grammars", ListGrammars);

                SpeechRecognitionUIResult result = await speech.RecognizeWithUIAsync();
                if (result.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
                {
                    MessageBox.Show(result.RecognitionResult.Text);
                }
                
            }
            
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.HResult.ToString());
                MessageBox.Show(ex.Source);
                MessageBox.Show(ex.InnerException.Message);
                MessageBox.Show(ex.StackTrace);
            }

        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}