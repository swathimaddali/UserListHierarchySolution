using System;
using System.Collections.Generic;
using System.Linq;


namespace UserListHierarchy
{
	public static class Program
	{
		static List<User> Users = new List<User>();
		static List<Role> Roles = new List<Role>();
		static Dictionary<int, List<int>> result = new Dictionary<int, List<int>>();	
		static Dictionary<int, int> UserDirectManagerList = new Dictionary<int, int>();

		public static void GetUserDirectManagerList()
		{

			var UserDirectManagerList = (from u in Users
										join r in Roles on u.role equals r.id into temp
										from t in temp
										join m in Users on t.pid equals m.role
										select new
										{
											eid = u == null ? 0 : u.id,
											mid = m == null ? 0 : m.id
										}).ToList();

		}

		
		public static void GetSubOrdinateList(int uid)
		{

			if (UserDirectManagerList == null)
				throw new InvalidOperationException("The sequence contains no elements");
			//get list of all subordinate users
			FindAllSubordinateUsersIds(UserDirectManagerList);
			if (result == null)
				Console.WriteLine("The user has no subordinates");

			List<User> UsersResult = new List<User>();
			if (result.ContainsKey(uid))
			{
				UsersResult = Users
			  .Where(t => result[uid].Contains(t.id)).ToList();
			}
			//DisplaySubordinates
			
			foreach (var user in Users)
			{
				Console.WriteLine(uid + " -> ");
				Console.WriteLine("ID {0},Name {1} ,Role{2}", user.id, user.name, user.role);
				
			}

		}




		public static void SetRoles()
		{

			Roles.Add(new Role { id = 1, name = "Admin", pid = 0 });
			Roles.Add(new Role { id = 2, name = "LM", pid = 1 });
			Roles.Add(new Role { id = 3, name = "Sup", pid = 2 });
			Roles.Add(new Role { id = 4, name = "Emp", pid = 3 });
			Roles.Add(new Role { id = 5, name = "Trainee", pid = 4 });


		}

		public static void SetUsers()
		{

			Users.Add(new User { name = "Adam", id = 1, role = 1 });
			Users.Add(new User { name = "Sam", id = 3, role = 3 });
			Users.Add(new User { name = "Mary", id = 4, role = 2 });
			Users.Add(new User { name = "Emily", id = 2, role = 4 });
			Users.Add(new User { name = "Steve", id = 5, role = 5 });
		}



		// Find all employees who directly or indirectly reports to a manager
		public static void FindAllSubordinateUsersIds(Dictionary<int, int> usersManagersMapping)
		{

			// store manager to employee mappings in a new map
			// List<Character> is used since a manager can have several employees mappeds
			Dictionary<int, List<int>> managerToUserMappings = new Dictionary<int, List<int>>();

			// fill above map with the manager to employee mappings
			foreach (var entry in usersManagersMapping)
			{
				int user = entry.Key;
				int manager = entry.Value;
				List<int> existingValue = null;

				// don't map an employee with itself
				if (user != manager)
				{
					if (!managerToUserMappings.TryGetValue(manager, out existingValue)) managerToUserMappings.Add(manager, new List<int>());

					managerToUserMappings[manager].Add(user);

				}
			}

			// find all reporting users (direct and indirect) for every manager
			// and store the result in a map
			foreach (var entry in usersManagersMapping)
				FindAllDirectSubordinateUsersIds(entry.Key, managerToUserMappings, result);
		}

		public static List<int> FindAllDirectSubordinateUsersIds(int managerId, Dictionary<int, List<int>> managerToUserMappings, Dictionary<int, List<int>> result)
		{
			if (!managerToUserMappings.ContainsKey(managerId))
				return null;
			if (result.ContainsKey(managerId))
				return result[managerId];

			List<int> managerUsers = managerToUserMappings[managerId];
			if (managerUsers != null)
			{

				// Retrieve all users reporting in-directly to the current manager
				foreach (int reportee in managerUsers.ToList())
				{
					// find all users reporting to the current user
					List<int> users = FindAllDirectSubordinateUsersIds(reportee,
							managerToUserMappings, result);
					// move those users to the current manager
					if (users != null)
						managerUsers.AddRange(users);

				}

			}
			// save the result to avoid re-computation and return it
			result.Add(managerId, managerUsers);
			return managerUsers;
		}

		public static void Main(string[] args)
		{
			try
			{
			
				SetUsers();
				SetRoles();

				Console.WriteLine("please enter the UserId to get Subordinate List");
				int key = Convert.ToInt32(Console.ReadLine());

				GetSubOrdinateList(key);
				// Keep the console open in debug mode.
				Console.WriteLine("Press any key to exit.");
				Console.ReadKey();
			}
			catch (Exception ex)
			{
				Console.WriteLine("error" + ex);
			}



		}

	}



}