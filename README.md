# UserListHierarchySolution

This Console Application takes UserId as input Parameter and returns the List of all Users who are Subordinates

Program is divided into 4 parts

1. UserDirectManagerList . This is a  Dictionary<int,int> which stores  mapping of User and his immediate Manager. 
2.ManagerToUserMappings - This is a Dictionary <int,List<int>> which is reverse map of UserDirectManagerList.  List<int> is used since a manager can have several users mapped.
3.FindAllDirectSubordinateUsersIds-This method loops through ManagerToUserMappings for every user and gets the Direct subordinate list for every user that is a manager.
4.FindSubordinatesForAllUsers-This method returns all the subordinate user ids in list for each user whether directly or indirectly reporting. It loops over FindAllDirectSubordinateUsersIds for every user as manager.


	
