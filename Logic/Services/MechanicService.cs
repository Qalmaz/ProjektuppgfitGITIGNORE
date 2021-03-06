﻿using Logic.DAL;
using Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Logic.Services
{
    public class MechanicService
    {
        private List<Mechanic> _mechanics;
        private IDataAccess<Mechanic> _mechanicdb;
        private List<Errand> _errands;
        private IDataAccess<Errand> _erranddb;
        private List<User> _users;
        private IDataAccess<User> _userdb;
        private Mechanic _mechanic;

        public MechanicService()
        {
            _mechanicdb = new DataAccess<Mechanic>();
            _erranddb = new DataAccess<Errand>();
            _userdb = new DataAccess<User>();
        }



        public Mechanic AddMechanic(string firstName, string lastName, DateTime dob)
        {

            Mechanic mechanic = new Mechanic(firstName, lastName, dob);

            _mechanicdb.Save(mechanic);

            return mechanic;

        }

        /// <summary>
        /// Tar bort en mekaniker permanent (och eventuell användare kopplad till mekanikern).
        /// </summary>
        /// <param name="mechanic"></param>
        public void RemoveMechanic(Mechanic mechanic)
        {
            _mechanics = _mechanicdb.Load();
            _users = _userdb.Load();

            foreach (var item in _mechanics)
            {
                if (item.MechanicID == mechanic.MechanicID)
                {
                    foreach (var user in _users)
                    {
                        if (user.UserID == item.MechanicID)
                        {
                            _users.Remove(user);
                            _userdb.Save(_users);
                            break;
                        }
                    }
                    
                    _mechanics.Remove(item);
                    _mechanicdb.Save(_mechanics);

                    return;
                }
            }
        }


        /// <summary>
        /// Returnerar en lista med den inloggade mekanikerns nuvarande kompetenser.
        /// </summary>
        /// <returns></returns>
        public List<string> ListSkills()
        {
            _mechanics = _mechanicdb.Load();

            _mechanic = _mechanics.FirstOrDefault(mechanic => mechanic.MechanicID == CurrentUser.user.UserID);

            return _mechanic.Skills;
        }

        /// <summary>
        /// Returnerar en lista med den inloggade mekanikerns pågående ärenden.
        /// </summary>
        /// <returns></returns>
        public List<Errand> ListActiveErrands()
        {
            _errands = _erranddb.Load();

            return _errands.Where(e => (e.MechanicID == CurrentUser.user.UserID) && (e.Status == true)).ToList();

        }

        public List<Errand> ListEndErrands()
        {
            _errands = _erranddb.Load();

            return _errands.Where(e => (e.MechanicID == CurrentUser.user.UserID) && (e.Status == false)).ToList();
        }

        public int DaysUntilBirthday(Mechanic mechanic)
        {

            DateTime today = DateTime.Today;
            DateTime next = mechanic.DateOfBirth.AddYears(today.Year - mechanic.DateOfBirth.Year);

            if (next < today)
                next = next.AddYears(1);

            int numDays = (next - today).Days;

            return numDays;
        }

        public Mechanic NextBirthday()
        {
            var mechanics = _mechanicdb.Load();
            mechanics = mechanics.OrderBy(m => DaysUntilBirthday(m)).ToList();
            return mechanics[0];
        }

        //Kopia av metoden som finns i klassen Mechanic
        public int CalculateAge(DateTime dob)
        {
            var today = DateTime.Today;

            int age = today.Year - dob.Year;

            if (DateTime.Now.DayOfYear < dob.DayOfYear)
            {
                age = age - 1;
            }

            return age;
        }


        /// <summary>
        /// Listar mekaniker som ej är tilldelade någon användare.
        /// </summary>
        /// <returns></returns>
        public List<Mechanic> MechanicNoUser()
        {
            _mechanics = _mechanicdb.Load();
            _users = _userdb.Load();
            var mechanicsNoUser = new List<Mechanic>();

            foreach (var m in _mechanics)
            {
                mechanicsNoUser.Add(m);

                foreach (var u in _users)
                {
                    if (u.UserID == m.MechanicID)
                    {
                        mechanicsNoUser.Remove(m);
                    }
                }
            }

            return mechanicsNoUser;
        }

        public Mechanic GetMechanic(Guid ID)
        {
            _mechanics = _mechanicdb.Load();
            var mechanic = _mechanics.FirstOrDefault(m => m.MechanicID == ID);
            return mechanic;
        }
    }
}
