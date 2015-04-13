using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Windows.Shapes;

namespace AppUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        String currentVersionContent;
        String nextVersionContent;
        int currentVersion=0;
        int nextVersion=0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            checkUpdate();
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            doUpgrade();
        }

        private String getCurrentDirectory() {
            return Directory.GetCurrentDirectory();
        }

        private int getVersion(String content) {
            return int.Parse(content.Split(Environment.NewLine.ToCharArray())[0]);
        }

        private void toLog(String str) {
            textBox.AppendText(str + Environment.NewLine);
        }
        private void clearLog() {
            textBox.Text = "";
        }
        private void checkUpdate() {
            clearLog();

            try
            {
                currentVersionContent = File.ReadAllText(getCurrentDirectory() + "/version");
                currentVersion = getVersion(currentVersionContent);
                toLog("Текущая версия: " + currentVersion);
            
                try
                {
                    /** Загружаем файл базы данных **/
                    toLog("Загружаю файл базы данных.");
                    dowloadFile("version_new", false);
                    nextVersionContent = File.ReadAllText(getCurrentDirectory() + "/version_new");
                    nextVersion = getVersion(nextVersionContent);

                    if (nextVersion > currentVersion)
                    {
                        toLog("Обнаружена новая версия: " + nextVersion);
                        button_Copy.IsEnabled = true;
                    }
                    else
                    {
                        toLog("Новых версий не обнаружено.");
                    }
                }
                catch
                {
                    toLog("Не удалось загрузить информацию об обновлениях.");
                }
            }
            catch
            {
                toLog("Не удалось найти файл с описанием версии.");
            }
        }
        private void doUpgrade() {
            String[] database = nextVersionContent.Split(Environment.NewLine.ToCharArray());
            for (int i = 1; i < database.Length; i++) {
                String filename = database[i].Split("|".ToCharArray())[0];
                if (!filename.Equals("")) {
                    int fileVersion = int.Parse(database[i].Split("|".ToCharArray())[1]);
                    if (fileVersion > currentVersion)
                    {
                        toLog("Обнаружена новая версия файла: "+ filename);
                        dowloadFile(filename, true);
                    }
                    else
                    {
                        toLog("Файл " + filename + " остается без изменений.");
                    }
                }
            }
            try
            {
                File.WriteAllText(getCurrentDirectory() + "/version", nextVersion + Environment.NewLine);
                button_Copy.IsEnabled = false;
                toLog("Обновление до версии " + nextVersion + " выполнено.");
            }
            catch
            {
                toLog("Обновление до версии " + nextVersion + " не удалось.");
            }
        }

        private void dowloadFile(String filename, Boolean showToLog) {
            string newResourceURI = "http://files.z-lab.me/distr/updater/" + filename;

            WebClient myWebClient = new WebClient();

            if(showToLog)toLog("Загружаю файл " + filename);
            
            myWebClient.DownloadFile(newResourceURI, filename);
        }
    }
}
