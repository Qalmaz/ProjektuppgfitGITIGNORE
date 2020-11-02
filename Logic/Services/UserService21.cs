﻿using Logic.DAL;
using Logic.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Services
{
    public class UserService21 : IAdmin, IUser
    {
        private MechanicDB _mechanics;
        private DataAccess<MechanicDB> _mechanicdb;
        private ErrandDB _errands;
        private DataAccess<ErrandDB> _erranddb;
        private UserDB _users;
        private DataAccess<UserDB> _userdb;

        public UserService21()
        {
            _mechanicdb = new DataAccess<MechanicDB>();
            _erranddb = new DataAccess<ErrandDB>();
            _userdb = new DataAccess<UserDB>();
        }

        //Ändrat till public pga interface. Går det att få till med private?
        public List<Mechanic> AvailableMechanics(Errand errand)
        {
            _errands = _erranddb.GetEntities();
            _errands.ListDB.Add(errand);
            _mechanics = _mechanicdb.GetEntities();
            

            var mechanicsAvailable = new List<Mechanic>();

            foreach (var mechanic in _mechanics.DBList)
            {
                var errandCount = mechanic.NumberOfErrands.Count;
                foreach (var skill in mechanic.Skills)
                {
                    if (errand.Issue == skill)
                    {
                        if (errandCount < 2 && errandCount >= 0)
                        {
                            mechanicsAvailable.Add(mechanic);
                        }
                    }
                } 
            }

            return mechanicsAvailable;
        }

        public void AddMechanic(string firstName, string lastName, DateTime dob)
        {

            Mechanic mechanic = new Mechanic(firstName, lastName, dob);

            _mechanics = _mechanicdb.GetEntities();
            _mechanics.DBList.Add(mechanic);
            _mechanicdb.AddEntity(_mechanics);

        }

        public void RemoveMechanic(Mechanic mechanic)
        {
            _mechanics = _mechanicdb.GetEntities();

            foreach (var item in _mechanics.DBList)
            {
                if (item.MechanicID == mechanic.MechanicID)
                {
                    _mechanics.DBList.Remove(item);
                    _mechanicdb.AddEntity(_mechanics);
                    return;
                }
            }
        }

        /// <summary>
        /// Admin och användare kan lägga till kompetenser.
        /// </summary>
        /// <param name="mechanic"></param>
        /// <param name="skill"></param>
        public void AddSkill(Mechanic mechanic, String skill)
        {
            _mechanics = _mechanicdb.GetEntities();

            foreach (var item in _mechanics.DBList)
            {
                if (item.MechanicID == mechanic.MechanicID)
                {
                    //Hur många skills?? Kontrollera för att ej lägga in för många! 5!
                    mechanic.Skills.Add(skill);
                    _mechanicdb.AddEntity(_mechanics);
                    return;
                }
            }
        }

        /// <summary>
        /// Admin lägger till en användare.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="admin"></param>
        public void AddUser(string username, string password, bool admin)
        {
            User user = new User(username, password, admin); //Metod för userid behövs i user-klassen.
            
            _users = _userdb.GetEntities();
            _users.DBList.Add(user);
            _userdb.AddEntity(_users);
        }

        /// <summary>
        /// Admin tar bort användare (och mekaniker kopplad till användare).
        /// </summary>
        /// <param name="user"></param>
        public void RemoveUser(User user)
        {
            _users = _userdb.GetEntities();
            _mechanics = _mechanicdb.GetEntities();

            foreach (var item in _users.DBList)
            {
                if (item.UserID == user.UserID)
                {
                    foreach (var mechanic in _mechanics.DBList)
                    {
                        if (mechanic.MechanicID == item.UserID)
                        {
                            _mechanics.DBList.Remove(mechanic);
                            _mechanicdb.AddEntity(_mechanics);
                        }
                    }

                    _users.DBList.Remove(item);
                    _userdb.AddEntity(_users);
                    return;
                }
            }
        }

    }
}