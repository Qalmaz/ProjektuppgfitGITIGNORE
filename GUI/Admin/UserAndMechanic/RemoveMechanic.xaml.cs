﻿using Logic.DAL;
using Logic.Entities;
using Logic.Services;
using System;
using System.Collections.Generic;
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
using System.Linq;

namespace GUI.Admin.UserAndMechanic
{
    /// <summary>
    /// Interaction logic for RemoveMechanic.xaml
    /// </summary>
    public partial class RemoveMechanic : Page
    {
        private IDataAccess<Mechanic> _mechanicdb;
        private IDataAccess<User> _userDB;
        private UserService _userservice;
        private MechanicService _mechanicService;
        
        private List<Mechanic> _mechanics;
        public RemoveMechanic()
        {
            _mechanicdb = new DataAccess<Mechanic>();
            _userDB = new DataAccess<User>();
            _userservice = new UserService();
            _mechanicService = new MechanicService();

            InitializeComponent();
            UpdateList();
           
        }
        private void RemoveMechanic_Click(object sender, RoutedEventArgs e)
        {
            if (RemoveList.SelectedItem == null)
            {
                return;
            }

            var mechanic = RemoveList.SelectedItem as Mechanic;
            if (_userDB.Load().FirstOrDefault(u => u.UserID == mechanic.MechanicID) != default)
            {
                var dr = MessageBox.Show("Detta kommer även att radera användaren som är knuten till mekanikern. Är du säker på att du vill radera mekanikern och dess användare?", "Radera mekaniker och den knytna användaren", MessageBoxButton.YesNo);

                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        {
                            var user = _userDB.Load().FirstOrDefault(u => u.UserID == mechanic.MechanicID);
                            _userservice.RemoveUser(user);                           
                            UpdateList();
                            break;
                        }
                    case MessageBoxResult.No:
                        {
                            return;
                        }
                }
            }
            _mechanicService.RemoveMechanic(mechanic);
            UpdateList();
            
        }

        private void UpdateList()
        {
            _mechanics = _mechanicdb.Load();
            var mechanic = _mechanics.FirstOrDefault(m => m.MechanicID == CurrentUser.user.UserID);
            _mechanics.Remove(mechanic);
            mechanic = _mechanics.FirstOrDefault(m => (m.FirstName == "Bosse") && (m.LastName == "Andersson")); //Hade funkat bättre med guid, men kan vara så här tills vidare.
            _mechanics.Remove(mechanic);
            RemoveList.ItemsSource = null;
            RemoveList.ItemsSource = _mechanics;
        }
    }
}
