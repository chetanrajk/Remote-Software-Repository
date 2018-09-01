///////////////////////////////////////////////////////////////////////
// RepositoryCore.cpp - Executes all the operation on Repository	  //
//														              //
// ver 1.0                                                           //
// Chetanraj Kadam, CSE687 - Object Oriented Design, Spring 2018     //
///////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* -------------------
* This package
* -
* Required Files:
* ---------------
* RepositroyCore.h
*
* Maintenance History:
* --------------------
* ver 1.0 :  25 Feb 2018
* - first release
*/

#include "RepositoryCore.h"

using namespace NoSqlDb;

/*
* - This demo code was simply lifted from Executive.cpp
*/
#ifdef TEST_REPOSITORY
int main()
{
	Utilities::Title("TestRepositoryCore");
	Utilities::putline();

	DbCore<PayLoad> db;
	PayLoad pl;
	DbElement<PayLoad> elem;

	elem.name("Executive.h.1");
	elem.descrip("John");
	pl.value() = "../Server/Executive/Executive.h.1";
	pl.categories().push_back("Header file");
	pl.categories().push_back("Execution");
	pl.status("Open");
	elem.payLoad(pl);
	elem.addChildKey("DbCore::DbCore.h.1");
	elem.addChildKey("PayLoad::PayLoad.h.1");
	db["Executive::Executive.h.1"] = elem;
	pl.categories().clear();
	elem.children().clear();

	CheckIn chkIn;
	Keys dep;
	std::string path;
	std::cout << "\n\n----> Case 1: File is present and in Open status:";
	dep.push_back("DbCore::DbCore.h.1");
	path = "../Local/PayLoad/PayLoad.h.1";
	chkIn.checkInFile(db, "PayLoad", "PayLoad.h.1", "Robert", dep, path);
	dep.clear();

	CheckOut chObj;
	std::cout << "\n\n----> Checkout particular file:";
	chObj.checkOutFile(db, "Executive", "Executive.h.1", "../Local/Checkout/");

	Browse bObj;
	bObj.browseDb(db);
	return 0;
}
#endif