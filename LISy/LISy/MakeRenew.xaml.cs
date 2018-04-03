﻿using LISy.Entities;
using LISy.Managers;
using System;
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
    /// Логика взаимодействия для MakeRenew.xaml
    /// </summary>
    public partial class MakeRenew : Window
    {
        private long copyID;
        private long documentID;
        private long patronID;
        private BookingHistory bookingHistoryWindow;

		/// <summary>
		/// Initializes MakeRenew window.
		/// </summary>
		/// <param name="patronID">Id of watching patron.</param>
		/// <param name="documentID">Id of looking document.</param>
		/// <param name="copyID">Id of looking copy.</param>
		/// <param name="bookingHistoryWindow">Previous window.</param>
        public MakeRenew(long patronID, long documentID, long copyID, BookingHistory bookingHistoryWindow)
        {
            InitializeComponent();
            this.copyID = copyID;
            this.documentID = documentID;
            this.patronID = patronID;
            this.bookingHistoryWindow = bookingHistoryWindow;
        }

		/// <summary>
		/// Checks renew status.
		/// </summary>
        public void CheckRenewStatus() {
            if (checkRenewStatusBox.IsChecked == true)
            {
                PatronDataManager.RenewDocument(documentID, patronID);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            CheckRenewStatus();
            bookingHistoryWindow.UpdateDataGrid();
            this.Close();
        }
    }
}
