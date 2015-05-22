﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletorDAL
{
	public class User
	{
		public int  ID { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public int AdminLevel { get; set; }
		public bool IsActive { get; set; }

		public User()
		{
			
		}

		public User(string username, string password, int adminLevel)
		{
			Username = username;
			Password = password;
			AdminLevel = adminLevel;
			IsActive = true;
		}
	}
}