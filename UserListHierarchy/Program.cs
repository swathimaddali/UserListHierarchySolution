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

		/// <summary>
		/// Main method to instantiate the Program
		/// </summary>
		/// <remarks>
		/// This method inputs UserID and if valid returns the list of subordinates.
		/// </remarks>
		
		public static void Main(string[] args)
		{
			try
			{
				SetUsers();  
				SetRoles();
				
				Console.WriteLine("Press ESC to stop");
				while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
				{
					Console.WriteLine("please enter the UserId to get Subordinate List");
					int key = Convert.ToInt32(Console.ReadLine());
					GetSubOrdinateList(key);
				}
				
			}
			catch (Exception ex)
			{
				Console.WriteLine("error" + ex);
			}
		}

		public static void GetSubOrdinateList(int uid )
		{
			//Get <UserId,ManagerID> as mapping of user and direct manager.
			
			UserDirectManagerList = (from u in Users
			join i in Roles on u.role equals i.id into x
			from y in x
			join f in Users on y.pid equals f.role
			select new
			{
				eid = u == null ? 0 : u.id,
				mid = f == null ? 0 : f.id,

			}).ToDictionary(o => o.eid, o => o.mid);

			//If UserDirectManagerList is empty
			//If user is not in list of managers
			if ((UserDirectManagerList.Count == 0)|| (!UserDirectManagerList.Any(tr => tr.Value.Equals(uid))))
			{
				Console.WriteLine("No List of subordinates found");
				return;
			}
			

			//Get list of all subordinate users ids
			FindSubordinatesForAllUsers(UserDirectManagerList);
			if (result.Count==0 )
				Console.WriteLine("The user has no subordinates");

			List<User> UsersResult = new List<User>();
			if (result.ContainsKey(uid))
			{
				UsersResult = Users
			  .Where(t => result[uid].Contains(t.id)).ToList();
			}
			//DisplaySubordinates			
			Console.WriteLine("subordinates for ::" +uid );
			 foreach(var usr in UsersResult)
				Console.WriteLine("ID :{0},Name :{1} ,Role:{2}", usr.id, usr.name, usr.role);		
			

		}

		/// <summary>
		/// Method to set values for Roles object
		/// </summary>
		/// <remarks>
		/// For this test, using hardcoded properties in Roles object		
		/// </remarks>
		public static void SetRoles(){

			Roles.Add(new Role { id = 1, name = "Admin", pid = 0 });
			Roles.Add(new Role { id = 2, name = "LM", pid = 1 });
			Roles.Add(new Role { id = 3, name = "Sup", pid = 2 });
			Roles.Add(new Role { id = 4, name = "Emp", pid = 3 });
			Roles.Add(new Role { id = 5, name = "Trainee", pid = 4 });

		}

		/// <summary>
		/// Method to set values for Users object
		/// </summary>
		/// <remarks>
		/// For this test, using hardcoded properties in User object		
		/// </remarks>
		public static void SetUsers()
		{

			Users.Add(new User { name = "Adam", id = 1, role = 1 });
			Users.Add(new User { name = "Sam", id = 3, role = 3 });
			Users.Add(new User { name = "Mary", id = 4, role = 2 });
			Users.Add(new User { name = "Emily", id = 2, role = 4 });
			Users.Add(new User { name = "Steve", id = 5, role = 5 });
		}

		/// <summary>
		/// FindSubordinatesForAllUsers returns all the subordinate user ids in list for each user whether directly or indirectly reporting.
		/// <remarks>
		/// The method returns complete subordinate list in Dictionary<int,List<int>>	
		/// </remarks>
		
		public static void FindSubordinatesForAllUsers(Dictionary<int, int> usersManagersMapping)
		{
			//Create reverse map to store (mid,eid) mapping in dictionary
			Dictionary<int, List<int>> managerToUserMappings = new Dictionary<int, List<int>>();
		
			foreach (var entry in usersManagersMapping)
			{
				int user = entry.Key;
				int manager = entry.Value;
				List<int> existingValue = null;

				// if user and manager are same do not add.
				if (user != manager)
				{
					if (!managerToUserMappings.TryGetValue(manager, out existingValue)) managerToUserMappings.Add(manager, new List<int>());
					managerToUserMappings[manager].Add(user);
				}
			}
			//For every userid we want the list of all subordinates direct or indirect into result Dictionary.
			//If userid is not in managerToUserMappings, it is not manager so return
			
			foreach (var u in Users)
			{
					
				FindAllDirectSubordinateUsersIds(u.id, managerToUserMappings, result);
			}
			
		}
		/// <summary>
		/// FindAllDirectSubordinateUsersIds returns all the subordinate user ids in list for given user who is a manager.
		/// <remarks>
		/// The method returns direct subordinate list for given manager.	
		/// </remarks>
		
		public static List<int> FindAllDirectSubordinateUsersIds(int managerId, Dictionary<int, List<int>> managerToUserMappings, Dictionary<int, List<int>> result)
		{
			if (!managerToUserMappings.ContainsKey(managerId))
				return null;
			if (result.ContainsKey(managerId))
				return result[managerId];

			List<int> managerUsers = managerToUserMappings[managerId];
			if (managerUsers != null)			{

				// Retrieve all users reporting in-directly to the current manager
				foreach (int reportee in managerUsers.ToList())
				{
					// find all user ids reporting to the current user
					List<int> users = FindAllDirectSubordinateUsersIds(reportee,
							managerToUserMappings, result);
					// move those users to the current manager
					if (users != null)
						managerUsers.AddRange(users);
				}

			}
			// To avoid recursive run save to the result and return 
			result.Add(managerId, managerUsers);
			return managerUsers;
		}

		

	}



}