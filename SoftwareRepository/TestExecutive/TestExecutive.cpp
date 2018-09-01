///////////////////////////////////////////////////////////////////////
// TestExecutive.cpp - Executes all the operation on Repository and   //
//                 demonstartes all the requirements                 //
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

#include "../RepositoryCore/RepositoryCore.h"

using namespace NoSqlDb;

auto putLine = [](size_t n = 1, std::ostream& out = std::cout)
{
	Utilities::putline(n, out);
};

class DbProvider
{
public:
	DbCore<PayLoad>& db() { return db_; }
private:
	static DbCore<PayLoad> db_;
};

DbCore<PayLoad> DbProvider::db_;

DbCore<PayLoad> makeDb()
{
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
	pl.categories().clear(); elem.children().clear();

	elem.name("DbCore.h.1");
	elem.descrip("Parker");
	pl.value() = "../Server/DbCore/DbCore.h.1";
	pl.categories().push_back("Header file");
	pl.categories().push_back("Functionality");
	pl.status("Close");
	elem.payLoad(pl);
	db["DbCore::DbCore.h.1"] = elem;
	pl.categories().clear(); elem.children().clear();

	elem.name("DbCore.cpp.1");
	elem.descrip("Parker");
	pl.value() = "../Server/DbCore/DbCore.cpp.1";
	pl.categories().push_back("Source file");
	pl.categories().push_back("Functionality");
	pl.status("Close");
	elem.payLoad(pl);
	db["DbCore::DbCore.cpp.1"] = elem;
	pl.categories().clear();elem.children().clear();

	elem.name("PayLoad.h.1");
	elem.descrip("Robert");
	pl.value() = "../Server/PayLoad/PayLoad.h";
	pl.categories().push_back("Header file");
	pl.categories().push_back("Functionality");
	pl.status("Close");
	elem.payLoad(pl);
	elem.addChildKey("DbCore::DbCore.h.1");
	db["PayLoad::PayLoad.h.1"] = elem;
	return db;
}

bool testR1()
{
	Utilities::Title("Demonstrating Requirement #1");
	std::cout << "\n  " << typeid(std::function<bool()>).name()
		<< ", declared in this function, "
		<< "\n  is only valid for C++11 and later versions.";
	putLine();
	return true; // would not compile unless C++11
}

bool testR2()
{
	Utilities::Title("Demonstrating Requirement #2");
	std::cout << "\n  A visual examination of all the submitted code "
		<< "will show only\n  use of streams and operators new and delete.";
	putLine();
	return true;
}

bool testR3()
{
	Utilities::Title("Demonstrating Requirement #3");

	std::cout << "\n\n----> Following Packages are provided with enforcing the \"Single Responsiblity Principle\" -:";
	std::cout << "\n\n\t- TestExecutive\n\t- RepositoryCore\n\t- CheckIn\n\t- CheckOut\n\t- Version\n\t- Browse";
	Utilities::putline();

	DbCore<PayLoad> db = makeDb();
	DbProvider dbp;
	DbElement<PayLoad> elem;
	PayLoad pl;

	Utilities::title("Making database storing metadata of files");

	elem.name("PayLoad.cpp.1");
	elem.descrip("Robert");
	pl.value() = "../Server/PayLoad/PayLoad.cpp.1";
	pl.categories().push_back("Header file");
	pl.categories().push_back("Functionality");
	pl.status("Close");
	elem.payLoad(pl);
	elem.addChildKey("DbCore::DbCore.h.1");
	db["PayLoad::PayLoad.cpp.1"] = elem;
	pl.categories().clear();
	elem.children().clear();

	elem.name("Executive.cpp.1");
	elem.descrip("Christopher");
	pl.value() = "../Server/Executive/Executive.cpp.1";
	pl.categories().push_back("Source file");
	pl.categories().push_back("Testing Execution");
	pl.status("Open");
	elem.payLoad(pl);
	elem.addChildKey("PayLoad::PayLoad.h.1");
	elem.addChildKey("Executive::Executive.h.1");
	db["Executive::Executive.cpp.1"] = elem;
	pl.categories().clear();
	elem.children().clear();

	dbp.db() = db;
	showDb(db);
	Utilities::putline();
	elem.payLoad().showDb(db);

	Utilities::putline();
	Utilities::putline();
	return true;
}

bool testR4()
{
	Utilities::Title("Demonstrating Requirement #4");

	DbProvider dbp;
	DbCore<PayLoad> db = dbp.db();

	CheckIn chkIn;
	Keys dep;
	std::string path;
	
	std::cout << "\n\n----> Case 1: File is present and in Open status:";
	dep.push_back("DbCore::DbCore.h.1");
	path = "../Local/Executive/Executive.h.1";
	dep.push_back("PayLoad::PayLoad.h.1");
	chkIn.checkInFile(db, "Executive", "Executive.h.1", "John", dep,path);
	dep.clear();

	std::cout << "\n\n----> Case 2: File not present present and so new entry";
	path = "../Local/DemoPkg/DemoFile.h.1";
	chkIn.checkInFile(db, "DemoPkg", "DemoFile.h.1", "XYZ", dep, path);
	dep.clear();

	std::cout << "\n\n----> Case 3: File is present and in Close status:";
	path = "../Local/DbCore/DbCore.h.1";
	chkIn.checkInFile(db, "DbCore", "DbCore.h.1", "Parker", dep, path);
	dep.clear();

	std::cout << "\n\n----> Case 4: File is present but check-in person is not owner:";
	dep.push_back("DbCore::DbCore.h.1");
	dep.push_back("PayLoad::PayLoad.h.1");
	path = "../Local/Executive/Executive.h.1";
	chkIn.checkInFile(db, "Executive", "Executive.h.1", "Abraham", dep, path);
	dep.clear();

	CheckOut chObj;
	std::cout << "\n\n----> Checkout particular file:";
	chObj.checkOutFile(db, "Executive", "Executive.h.1", "../Local/Checkout/");

	Utilities::putline();
	Utilities::putline();
	return true;
}

bool testR5()
{
	Utilities::Title("Demonstrating Requirement #5");

	DbCore<PayLoad> db;
	DbProvider dbp;
	db = dbp.db();

	Browse bObj;
	bObj.browseDb(db);

	Utilities::putline();
	Utilities::putline();
	return true;
}

bool testR6()
{
	Utilities::Title("Demonstrating Requirement #6");

	std::cout << "\n\n  Repository is submitted with several packages with closed check-in and one with open check-in";

	Utilities::putline();
	Utilities::putline();
	return true;
}

bool testR7()
{
	Utilities::Title("Demonstrating Requirement #7");

	std::cout << "\n  Test executive package executing all requirements of project";

	Utilities::putline();
	Utilities::putline();
	return true;
}

int main()
{
	Utilities::Title("<------------ TestExecutive ---------->");
	putLine();

	TestExecutive ex;

	// define test structures with test function and message

	TestExecutive::TestStr ts1{ testR1, "Use C++11" };
	TestExecutive::TestStr ts2{ testR2, "Use streams and new and delete" };

	TestExecutive::TestStr ts3{ testR3, "Different packages" };
	TestExecutive::TestStr ts4{ testR4, "Check-in and Check-out functionality" };
	TestExecutive::TestStr ts5{ testR5, "Browsing and pop up functionality" };
	TestExecutive::TestStr ts6{ testR6, "Packages in closed/open check-ins" };

	TestExecutive::TestStr ts7{ testR7, "Test Executive functionality" };
	// register test structures with TestExecutive instance, ex

	ex.registerTest(ts1);
	ex.registerTest(ts2);
	ex.registerTest(ts3);
	ex.registerTest(ts4);
	ex.registerTest(ts5);
	ex.registerTest(ts6);
	ex.registerTest(ts7);
	// run tests

	bool result = ex.doTests();
	if (result == true)
		std::cout << "\n\n ======>  All tests passed  <======";
	else
		std::cout << "\n  at least one test failed";

	putLine(2);
	return 0;
}