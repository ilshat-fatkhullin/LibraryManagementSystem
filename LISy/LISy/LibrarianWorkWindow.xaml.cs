﻿using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace LISy
{
    /// <summary>
    /// Логика взаимодействия для LibrarianWorkWindow.xaml
    /// </summary>
    public partial class LibrarianWorkWindow : Window
    {
        public LibrarianWorkWindow()
        {
            InitializeComponent();
        }

        private void choose_user_card_Click(object sender, RoutedEventArgs e)
        {
            //grid_Loaded();
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {

        }

        private void add_user_Click(object sender, RoutedEventArgs e)
        {
            AddUserCard addUserCard = new AddUserCard();
            addUserCard.Owner = this;
            addUserCard.Show();
        }

        private void add_doc_Click(object sender, RoutedEventArgs e)
        {

        }

        private void show_all_Click(object sender, RoutedEventArgs e)
        {

        }
        private void grid_Loaded(object sender, RoutedEventArgs e)
        {
            List<MyUserTable> listOfUsers = new List<MyUserTable>();
           
        }
        private void grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MyUserTable path = DataGridInfo.SelectedItem as MyUserTable;
            // окно modify для изменение значение для этого юзера
        }
    }
}
