﻿using Dapper;
using LISy.Entities;
using LISy.Entities.Users;
using System;
using System.Data;
using System.Linq;
using System.Windows.Documents;
using LISy.Entities.Users.Patrons;

namespace LISy.Managers.DataManagers
{
	/// <summary>
	/// Contains database commands for users
	/// </summary>
	static class UsersDataManager
	{
		/// <summary>
		/// Adds new Patron to the database.        
		/// </summary>
		/// <param name="user">Patron, which is going to be added.</param>
		/// <param name="login">Login of the patron.</param>
		/// <param name="password">Password of the patron.</param>
		public static bool AddUser(IUser user, string login, string password)
		{
			if (user == null)
			{
				throw new ArgumentNullException();
			}

			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("LibraryDB")))
			{
				var output = connection.Query<bool>("dbo.spUsers_IsUserInTable @FirstName, @SecondName, @Phone",
					new
					{ FirstName = user.FirstName, SecondName = user.SecondName, Phone = user.Phone }).ToList();
				if (!output[0])
				{
					long cardNumber = CredentialsManager.AddUserCredentials(login, password);

					user.CardNumber = cardNumber;
					connection.Execute("dbo.spUsers_AddUser @FirstName, @SecondName, @CardNumber, @Phone, @Address, @Type",
						new
						{
							FirstName = user.FirstName,
							SecondName = user.SecondName,
							CardNumber = user.CardNumber,
							Phone = user.Phone,
							Address = user.Address,
							Type = user.GetType().ToString().Split('.').Last()
						});
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Deletes a Patron from the database.
		/// </summary>
		/// <param name="user">Patron, which is going to be deleted.</param>
		public static void DeleteUser(IUser user)
		{

			if (user == null)
			{
				throw new ArgumentNullException();
			}

			CredentialsManager.DeleteUserCredentials(user.CardNumber);

			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("LibraryDB")))
			{
				connection.Execute("dbo.spUsers_DeleteUser @CardNumber", user);
			}
		}

		/// <summary>
		/// Replaces <code>Patron</code> on <code>newPatron</code> in the database.
		/// </summary>
		/// <param name="newUser">Patron, which is going to be instead of <code>Patron</code>.</param>
		public static void EditUser(IUser newUser)
		{
			if (newUser == null)
			{
				throw new ArgumentNullException();
			}

			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("LibraryDB")))
			{
				connection.Execute("dbo.spUsers_ModifyUser @CardNumber, @FirstName, @SecondName, @Phone, @Address",
					new
					{
						CardNumber = newUser.CardNumber,
						FirstName = newUser.FirstName,
						SecondName = newUser.SecondName,
						Phone = newUser.Phone,
						Address = newUser.Address
					});
			}
		}

		public static int GetNumberOfUsers()
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("LibraryDB")))
			{
				var output = connection.Query<int>("dbo.spUsers_GetNumberOfUsers").ToList();
				return (output[0]);
			}
		}

		public static IUser[] GetUsersList()
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("LibraryDB")))
			{
				var output = connection.Query<TempUser>("dbo.spUsers_GetAllUsers").ToArray();
				IUser[] users = new IUser[output.Count()];
				for (int i = 0; i < users.GetLength(0); i++)
				{
					TempUser user = output[i];
					switch (user.Type)
					{
						case Faculty.TYPE:
							users[i] = new Faculty(user.FirstName, user.SecondName, user.Phone, user.Address, "");
							break;
						case Student.TYPE:
							users[i] = new Student(user.FirstName, user.SecondName, user.Phone, user.Address);
							break;
						case Librarian.TYPE:
							users[i] = new Librarian(user.FirstName, user.SecondName, user.Phone, user.Address);
							break;
					}

					users[i].CardNumber = output[i].CardNumber;
				}
				return users;
			}
		}
	}

	class TempUser
	{
		public string FirstName { get; set; }

		public string SecondName { get; set; }

		public int CardNumber { get; set; }

		public string Phone { get; set; }

		public string Address { get; set; }

		public string Type { get; set; }
	}
}
